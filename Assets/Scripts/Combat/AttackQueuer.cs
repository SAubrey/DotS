using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackQueuer : MonoBehaviour {
    public const float WAIT_TIME = 3.0f;
    private Controller c;
    private LineDrawer line_drawer;
    public GameObject hit_splat_prefab;
    public GameObject FieldPanel;

    public const int PU_TO_E = 0;
    public const int E_TO_PU = 1;
    // Attacks queue must be searchable in case of deletion before the battle.
    
    private AttackQueue enemy_queue = new AttackQueue();
    private AttackQueue player_queue = new AttackQueue();

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        line_drawer = c.line_drawer;
    }

    public bool attempt_attack(Slot start, Slot end) {
        Unit attacker = start.get_unit();
        if (end.get_unit() == null)
            return false;
        if (start.get_unit().can_hit(end)) {
            if (attacker.is_playerunit()) {
                get_player_queue().enqueue(start, end, line_drawer);
            } else {
                get_enemy_queue().enqueue(start, end, line_drawer);
            }
            attacker.attack_set = true;
            return true;
        }
        return false;
    }
/*
TODO: Accumulate multiple attacks on a single unit and have those animate together.

Scan through all enemy attacks and create a list of lists of attacks that
target the same unit. 
*/
    /*
    Every unit that has an attack poised will attack regardless of the animation order
    Display attack animations for each simultaneously pulled unit
     */
    public IEnumerator battle() {
        bool toggle = true;
        Attack att;
        do {
            // Take turns giving each side a chance to show an attack. 
            if (toggle) {
                att = player_queue.dequeue();
                if (att == null) 
                    att = enemy_queue.dequeue();
            } else {
                att = enemy_queue.dequeue();
                if (att == null)
                    att = player_queue.dequeue();
            }
            toggle = !toggle;
            if (att != null) {
                attack(att);
                yield return new WaitForSeconds(WAIT_TIME / 2);
            }
        } while(att != null);

        // Clear attacks and clean the battlefield
        c.get_active_bat().post_battle();
        c.enemy_brain.clear_dead_enemies();
        reset();
        c.battle_phaser.post_battle(); // after reset
    }

    private void attack(Attack att) {
        create_hitsplat(att);
        int line_id = att.get_start_slot().get_unit().line_id;
        line_drawer.get_line(line_id).begin_fade();
        att.attack();
    }

    private void create_hitsplat(Attack att) {
        GameObject hs = GameObject.Instantiate(hit_splat_prefab);
        hs.transform.SetParent(FieldPanel.transform, false); 
        HitSplat hs_script = hs.GetComponent<HitSplat>();
        hs.transform.position = att.get_end_slot().transform.position;
        hs_script.init(att);
    }

    private void reset() {
        enemy_queue.reset();
        player_queue.reset();
    }
    public AttackQueue get_enemy_queue() {
        return enemy_queue;
    }

    public AttackQueue get_player_queue() {
        return player_queue;
    }
}


public class AttackQueue {
    public List<Attack> attack_list = new List<Attack>();

    public void enqueue(Slot start, Slot end, LineDrawer ld) {
        attack_list.Add(new Attack(start, end));
        // display visually
        Vector3 s = start.transform.position;
        Vector3 e = end.transform.position;
        ld.draw_line(start.get_unit(), s, e);
    }

    public Attack dequeue() {
        if (attack_list.Count <= 0) {
            return null;
        }
        Attack attack = attack_list[attack_list.Count - 1];
        if (attack != null) {
            attack_list.RemoveAt(attack_list.Count - 1);
            return attack;
        } else {
            return null;
        }
    }

    public Attack find_attack(PlayerUnit pu) {
        foreach (Attack a in attack_list) {
            if (a.get_punit() == pu)
                return a;
        }
        return null;
    }

    // Used when a player rescinds a planned attack.
    public void remove_attack(PlayerUnit pu, LineDrawer ld) {
        Attack Attack = find_attack(pu);
        attack_list.Remove(find_attack(pu));
        ld.remove(pu.line_id);
        pu.attack_set = false;
    }

    public void reset() {
        attack_list = new List<Attack>();
    }
}

/*
To Attack, iterate through player units as keys for either attack list.
 */
public class Attack {
    private PlayerUnit punit;
    private Enemy enemy;
    private Slot start;
    private Slot end;
    public int direction;
    
    public Attack(Slot start, Slot end) {
        this.start = start;
        this.end = end;
        if (start.get_unit().is_playerunit()) {
            punit = start.get_punit();
            enemy = end.get_enemy();
            direction = AttackQueuer.PU_TO_E;
        } else {
            punit = end.get_punit();
            enemy = start.get_enemy();
            direction = AttackQueuer.E_TO_PU;
        }
    }

    public void attack() {
        // determine damage from flanking, rear
        int dmg = get_start_slot().get_unit().attack() + 
                    calc_dir_dmg(start.get_group(), end.get_group());
        int state = get_end_slot().get_unit().take_damage(dmg);
        if (state == Unit.DEAD || state == Unit.INJURED) {
            if (direction == AttackQueuer.E_TO_PU) {
                enemy.clear_target();
                start.c.enemy_brain.retarget();
            }
        }
        if (state == Unit.DEAD && direction == AttackQueuer.PU_TO_E) {
            // Remove from player's battalion and update text.
            start.c.get_disc().change_var(Storeable.EXPERIENCE, enemy.xp);
        }
    }

    public int calc_dmg_taken() {
        int dmg = get_start_slot().get_unit().attack_dmg + 
                    calc_dir_dmg(start.get_group(), end.get_group());
        return get_end_slot().get_unit().calc_dmg_taken(dmg);
    }

    /*
    */
    public int calc_dir_dmg(Group att, Group def) {
        // Check if attacking from behind.
        if (def.faces(Group.UP) && att.row < def.row) 
            return 2;
        else if (def.faces(Group.DOWN) && att.row > def.row) 
            return 2;
        else if (def.faces(Group.LEFT) && att.col > def.col) 
            return 2;
        else if (def.faces(Group.RIGHT) && att.col < def.col)
            return 2;
        
        // Check if attacking from a side flank.
        if (def.faces(Group.UP) || def.faces(Group.DOWN))
            return att.row == def.row ? 1 : 0;
        else
            return att.col == def.col ? 1 : 0;
    }


    public PlayerUnit get_punit() {
        return punit;
    }

    public Enemy get_enemy() {
        return enemy;
    }

    public Slot get_end_slot() {
        return end;
    }
    
    public Slot get_start_slot() {
        return start;
    }
}