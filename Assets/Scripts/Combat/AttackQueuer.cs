using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackQueuer : MonoBehaviour {
    public const int PU_TO_E = 0;
    public const int E_TO_PU = 1;
    public const float WAIT_TIME = 3.0f;
    private Controller c;
    private LineDrawer line_drawer;
    public GameObject hit_splat_prefab;
    public GameObject FieldPanel;

    private AttackQueue enemy_queue = new AttackQueue();
    private AttackQueue player_queue = new AttackQueue();
    private int attack_id = 0; // Unique identifier for each attack & line.
    public ParticleSystem blood_ps; // prefab
    public GameObject plane;

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        line_drawer = c.line_drawer;
    }

    public void add_attack(Slot start, Slot end) {
        Unit attacker = start.get_unit();
        if (attacker.is_playerunit) 
            get_player_queue().add_attack(start, end, attack_id, line_drawer);
        else 
            get_enemy_queue().add_attack(start, end, attack_id, line_drawer);
        attack_id++;
        attacker.attack_set = true;
    }

    /*
    Alternates attack animations between sides. Group attacks, or 
    multiple attackers on a single unit, display simultaneously.
     */
    public IEnumerator battle() {
        bool toggle = true;
        List<Attack> attacks;
        do {
            if (toggle) {
                attacks = player_queue.get_group();
                if (attacks == null) 
                    attacks = enemy_queue.get_group();
            } else {
                attacks = enemy_queue.get_group();
                if (attacks == null)
                    attacks = player_queue.get_group();
            }
            toggle = !toggle;
            if (attacks != null) {
                if (attacks.Count > 1)
                    group_attack(attacks);
                else 
                    attack(attacks[0]);
                yield return new WaitForSeconds(WAIT_TIME / 2);
            }
        } while(attacks != null);
        post_battle();
    }

    /*
    Multiple units attacking the same unit will combine their attack damage
    before substracting by defense and then determining the state of the
    attacked unit. This is especially necessary for player units
    given that a player unit's resilience must be calculated against 
    with a single damage sum.
    Group attacks in this context are not attacks from a unit with
    the grouping attribute.
    */
    private void group_attack(List<Attack> attacks) {
        int sum_dmg = 0;
        foreach (Attack a in attacks) {
            sum_dmg += a.get_raw_dmg();
            a.get_start_unit().attack();
        }

        Unit end_u = attacks[0].get_end_unit();
        int state = end_u.take_damage(end_u.calc_dmg_taken(sum_dmg));
        foreach (Attack a in attacks) {
            a.post_attack(state);
            line_drawer.get_line(a.get_start_unit().attack_id).begin_fade();
        }
        create_hitsplat(sum_dmg, state, attacks[0].get_end_slot());
        c.sound_manager.impact_player.play_hit_from_damage(sum_dmg, state == Unit.DEAD); // Audio
    }

    private void attack(Attack att) {
        int dmg = att.calc_dmg_taken();
        int state = att.get_end_unit().get_post_dmg_state(dmg);
        c.sound_manager.impact_player.play_hit_from_damage(dmg, state == Unit.DEAD); // Audio
        create_hitsplat(dmg, state, att.get_end_slot());
        int attack_id = att.get_start_unit().attack_id;
        line_drawer.get_line(attack_id).begin_fade();

        att.attack();
    }

    private void post_battle() {
        // Clear attacks and clean the battlefield
        reset();
        c.line_drawer.clear();
        c.battle_phaser.post_battle(); // after reset
    }

    private void create_hitsplat(int dmg, int state, Slot end_slot) {
        GameObject hs = GameObject.Instantiate(hit_splat_prefab);
        hs.transform.SetParent(FieldPanel.transform, false); 
        HitSplat hs_script = hs.GetComponent<HitSplat>();
        hs_script.init(dmg, state, end_slot);
        // create XP hitsplat here if end unit is enemy?

        // Blood streaks
        if (dmg > 0) {
            ParticleSystem psI = Instantiate(blood_ps);
            ParticleSysTracker pst = psI.GetComponent<ParticleSysTracker>();
            pst.init(end_slot.transform.position);
        }
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
    Dictionary<Unit, List<Attack>> groupings = new Dictionary<Unit, List<Attack>>();

    public void add_attack(Slot start, Slot end, int id, LineDrawer ld) {
        //attack_list.Add(new Attack(start, end, id));
        if (!groupings.ContainsKey(end.get_unit()))
            groupings.Add(end.get_unit(), new List<Attack>());
        groupings[end.get_unit()].Add(new Attack(start, end, id));

        // display visually
        Vector3 s = start.transform.position;
        Vector3 e = end.transform.position;
        ld.draw_line(start.get_unit(), s, e, id);
    }

    public List<Attack> get_group() {
        if (groupings.Count <= 0)
            return null;
        // Get the first group of size 2+
        List<Attack> group = new List<Attack>();
        foreach (Unit u in groupings.Keys) {
            if (groupings[u].Count > 1) {
                group = groupings[u];
                break;
            }
        }

        // If no group of size 2+ exists, pick any of size 1.
        if (group.Count == 0) { 
            foreach (Unit u in groupings.Keys) {
                group = groupings[u];
                break;
            }
        }
        groupings.Remove(group[0].get_end_unit());
        return group;
    }

    // Attackers maintain the attack ID.
    public Attack find_attack(int attack_id) {
        foreach (List<Attack> attacks in groupings.Values) {
            foreach (Attack a in attacks) {
                if (a.id == attack_id)
                    return a;
            }
        }
        return null;
    }

    // Used when a player rescinds a planned attack.
    public void remove_attack(int attack_id, LineDrawer ld) {
        Attack a = find_attack(attack_id);
        if (a != null) {
            a.get_start_unit().attack_set = false;
            if (a.get_end_slot().is_showing_damage()) { // remove preview damage text.
                a.get_end_slot().show_preview_damage(false);
            }

            groupings[a.get_end_unit()].Remove(a);
            if (groupings[a.get_end_unit()].Count < 1) // Remove group attack if empty.
                groupings.Remove(a.get_end_unit());
        }
        ld.remove(attack_id);
    }

    public void reset() {
        groupings.Clear();
    }
}


/*
Attacks are 1:1 relationships, from start(attacker) to end(defender)
 */
public class Attack {
    private PlayerUnit punit;
    private Enemy enemy;
    private Slot start, end;
    public int direction;
    public int id;

    public Attack(Slot start, Slot end, int id) {
        this.start = start;
        this.end = end;
        this.id = id;
        start.get_unit().attack_id = id;

        if (start.get_unit().is_playerunit) {
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
        //int dmg = get_start_unit().attack() + calc_dir_dmg();
        get_start_unit().attack();
        int dmg = calc_dmg_taken();
        int state = get_end_unit().take_damage(dmg);
        post_attack(state);
    }

    public void post_attack(int state) {
        if (direction == AttackQueuer.E_TO_PU)
            post_enemy_attack(state);
        else 
            post_player_attack(state);
    }

    public void post_player_attack(int state) {
        int xp = enemy.take_xp_from_death();
        if (state == Unit.DEAD && xp > 0) {
            start.c.get_disc().change_var(Storeable.EXPERIENCE, xp, true);
        }
    }

    public void post_enemy_attack(int state) {
        if (state == Unit.DEAD || state == Unit.INJURED) {
            enemy.clear_target();
            start.c.enemy_brain.retarget();
        }

        // If the attack triggers grouping, reduce the actions of grouped units.
        if (punit.defending && punit.is_actively_grouping) {
            foreach (Unit u in punit.get_slot().get_group().get_grouped_units()) {
                u.num_actions--;
                punit.set_attribute_active(false);
            }
        }
    }

    public int calc_dmg_taken() {
        return get_end_unit().calc_dmg_taken(
            get_raw_dmg(), get_start_unit().has_attribute(Unit.PIERCING));
    }

    // Accounts for grouping attack dmg.
    public int get_raw_dmg() {
        return get_start_unit().get_attack_dmg() + calc_dir_dmg();
    }

    /* Accout for additional flank or rear damage.
    */
    public int calc_dir_dmg() {
        Group att = get_start_slot().get_group();
        Group def = get_end_slot().get_group();
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

    public Unit get_start_unit() {
        return start.get_unit();
    }

    public Unit get_end_unit() {
        return end.get_unit();
    }
}