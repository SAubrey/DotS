using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttackQueuer : MonoBehaviour {
    public static AttackQueuer I { get; private set; }
    public const int PU_TO_E = 0, E_TO_PU = 1;
    public const float WAIT_TIME = 3.0f;
    public GameObject FieldPanel;

    private AttackQueue enemy_queue, player_queue; // Queue of enemy attacks.
    private int attack_id = 0; // Unique identifier for each attack & line.
    public ParticleSystem blood_ps; // prefab
    public GameObject plane;
    void Awake() {
        if (I == null) {
            I = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        enemy_queue = new AttackQueue();
        player_queue = new AttackQueue();
    }

    public void add_attack(Slot att, Slot def) {
        Unit attacker = att.get_unit();
        if (attacker.is_playerunit) 
            get_player_queue().add_attack(att, def, attack_id);
        else 
            get_enemy_queue().add_attack(att, def, attack_id);
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
        if (attacks.Count == 0)
            return;
        Unit def_u = attacks[0].get_def_unit();
        int final_dmg = calc_final_group_dmg_taken(attacks);
        int state = def_u.take_damage(final_dmg);
        foreach (Attack a in attacks) {
            a.get_att_unit().attack();
            a.post_attack(state);
            LineDrawer.I.get_line(a.get_att_unit().attack_id).begin_fade();
        }
        create_hitsplat(final_dmg, state, attacks[0].get_def_slot());
        SoundManager.I.impact_player.play_hit_from_damage(final_dmg, state == Unit.DEAD); // Audio
    }
    
    /*
    An accurate sum of damage involving two or more attackers must be calculated
    with a running total of the defender's defense. Separate calculations will
    subtract the damage by defense twice or more over.
    */
    public static int calc_final_group_dmg_taken(List<Attack> atts, Unit preview_unit=null) {
        Unit defender = null;
        if (atts != null) {
            if (atts.Count > 0) {
                defender = atts[0].get_def_unit();
            }
        } else if (Selector.I.hovered_slot != null) {
            if (Selector.I.hovered_slot.has_enemy) {
                defender = Selector.I.hovered_slot.get_unit();
            }
        }
        if (defender == null)
            return 0;

        int defense = defender.get_defense();
        int sum_dmg = 0;
        int dmg;
        // First determine effect of preview unit.
        if (preview_unit != null) {
            dmg = Attack.get_dmg(preview_unit, defender);
            if (defender.defending && !preview_unit.has_attribute(Unit.PIERCING)) {
                // defense and remaining damage cancel each other out.
                int d = defense;
                defense = Mathf.Max(defense - dmg, 0);
                dmg = Mathf.Max(dmg - d, 0);
            }
            sum_dmg += dmg;
        }
        // No attacks are queued, we are only calculating the preview damage.
        if (atts == null) {
            if (defender.defending) {
                return Mathf.Max(sum_dmg - defense, 0);
            }
        }

        // Calcuate damage from existing attacks.
        foreach (Attack a in atts) {
            Unit att = a.get_att_unit();
            dmg = Attack.get_dmg(att, defender);
            if (defender.defending && !att.has_attribute(Unit.PIERCING)) {
                int d = defense;
                defense = Mathf.Max(defense - dmg, 0);
                dmg = Mathf.Max(dmg - d, 0);
            }
            sum_dmg += dmg;
        }
        if (defender.defending) {
            sum_dmg -= defense;
        }
        return Mathf.Max(sum_dmg, 0);
    }

    private void attack(Attack att) {
        int dmg = Attack.calc_final_dmg_taken(att.get_att_unit(), att.get_def_unit());
        int state = att.get_def_unit().get_post_dmg_state(dmg);
        SoundManager.I.impact_player.play_hit_from_damage(dmg, state == Unit.DEAD); // Audio
        create_hitsplat(dmg, state, att.get_def_slot());
        int attack_id = att.get_att_unit().attack_id;
        LineDrawer.I.get_line(attack_id).begin_fade();

        att.attack();
    }

    private void post_battle() {
        // Clear attacks and clean the battlefield
        reset();
        LineDrawer.I.clear();
        BattlePhaser.I.post_battle(); // after reset
    }

    private void create_hitsplat(int dmg, int state, Slot def_slot) {
        /*
        GameObject hs = GameObject.Instantiate(hit_splat_prefab);
        hs.transform.SetParent(FieldPanel.transform, false); 
        HitSplat hs_script = hs.GetComponent<HitSplat>();
        hs_script.init(dmg, state, def_slot);
*/
        Statics.create_rising_info_battle(
            dmg.ToString(), 
            Statics.get_unit_state_color(state), 
            def_slot.transform,
            TurnPhaser.I.active_disc.rising_info_prefab);
        // Blood streaks
        if (dmg > 0) {
            ParticleSystem psI = Instantiate(blood_ps);
            ParticleSysTracker pst = psI.GetComponent<ParticleSysTracker>();
            pst.init(def_slot.transform.position);
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
    // For each attacked unit, maintain a list of the attacks against it.
    private Dictionary<Unit, List<Attack>> groupings = new Dictionary<Unit, List<Attack>>();

    public void add_attack(Slot att, Slot def, int id) {
        //attack_list.Add(new Attack(att, def, id));
        if (!groupings.ContainsKey(def.get_unit()))
            groupings.Add(def.get_unit(), new List<Attack>());
        groupings[def.get_unit()].Add(new Attack(att, def, id));

        // display visually
        Vector3 s = att.transform.position;
        Vector3 e = def.transform.position;
        LineDrawer.I.draw_line(att.get_unit(), s, e, id);
    }

    /* Remove and return the next attack, prioritizing group attacks */
    public List<Attack> get_group() {
        if (groupings.Count <= 0)
            return null;
        // Get the first group of size 2+
        List<Attack> group = new List<Attack>();
        foreach (Unit defender in groupings.Keys) {
            if (groupings[defender].Count > 1) {
                group = groupings[defender];
                break;
            }
        }

        // If no group of size 2+ exists, pick any of size 1.
        if (group.Count == 0) { 
            foreach (Unit defender in groupings.Keys) {
                group = groupings[defender];
                break;
            }
        }
        groupings.Remove(group[0].get_def_unit());
        return group;
    }

    // Attackers maintain the attack ID. Assumes attackers can have only 1 attack.
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
    public void remove_attack(int attack_id) {
        Attack a = find_attack(attack_id);
        if (a != null) {
            //a.get_def_unit().subtract_preview_damage(a.get_att_unit().get_attack_dmg());
            groupings[a.get_def_unit()].Remove(a);
            if (groupings[a.get_def_unit()].Count < 1) // Remove group attack if empty.
                groupings.Remove(a.get_def_unit());
            
            a.get_att_unit().attack_set = false;
            a.get_def_slot().update_healthbar();
        }
        Debug.Log("Attempting to remove line. attack id: " + attack_id);
        LineDrawer.I.remove(attack_id);
    }

    public List<Attack> get_incoming_attacks(Unit defender) {
        return groupings.ContainsKey(defender) ? groupings[defender] : null;
    }

    public void reset() {
        LineDrawer.I.clear();
        groupings.Clear();
    }

    public bool has_attacks { get => groupings.Count > 0; }
}


/*
Attacks are 1:1 relationships, from att(attacker) to def(defdefer)
 */
public class Attack {
    public PlayerUnit punit { get; private set; }
    private Enemy enemy;
    private Slot att, def;
    public int direction;
    public int id;

    public Attack(Slot att, Slot def, int id) {
        this.att = att;
        this.def = def;
        this.id = id;
        att.get_unit().attack_id = id;

        if (att.get_unit().is_playerunit) {
            punit = att.get_punit();
            enemy = def.get_enemy();
            direction = AttackQueuer.PU_TO_E;
        } else {
            punit = def.get_punit();
            enemy = att.get_enemy();
            direction = AttackQueuer.E_TO_PU;
        }
    }

    public void attack() {
        // determine damage from flanking, rear
        //int dmg = get_att_unit().attack() + calc_dir_dmg();
        get_att_unit().attack();
        int dmg = calc_final_dmg_taken(get_att_unit(), get_def_unit());
        int state = get_def_unit().take_damage(dmg);
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
            TurnPhaser.I.active_disc.add_xp_in_battle(xp, enemy);
        }
    }

    public void post_enemy_attack(int state) {
        if (state == Unit.DEAD || state == Unit.INJURED) {
            enemy.clear_target();
            EnemyBrain.I.retarget();
        }

        // If the attack triggers grouping, reduce the actions of grouped units.
        if (punit.defending && punit.is_actively_grouping) {
            foreach (Unit u in punit.get_slot().get_group().get_grouped_units()) {
                u.num_actions--;
                punit.set_attribute_active(false);
            }
        }
    }

    public static int calc_final_dmg_taken(Unit att, Unit def) {
        return def.calc_dmg_taken(get_dmg(att, def), att.has_attribute(Unit.PIERCING));
    }

    // Accounts for grouping attack dmg.
    public static int get_dmg(Unit att, Unit def) {
        return att.get_attack_dmg() + calc_dir_dmg(att, def);
    }

    /* Accout for additional flank or rear damage.
    */
    public static int calc_dir_dmg(Unit att, Unit def) {
        Group attg = att.get_slot().get_group();
        Group defg = def.get_slot().get_group();
        // Check if attacking from behind.
        if ((defg.faces(Group.UP) && attg.row < defg.row) ||
            (defg.faces(Group.DOWN) && attg.row > defg.row) ||
            (defg.faces(Group.LEFT) && attg.col > defg.col) ||
            (defg.faces(Group.RIGHT) && attg.col < defg.col))
            return 2;
        
        // Check if attacking from a side flank.
        if (defg.faces(Group.UP) || defg.faces(Group.DOWN))
            return attg.row == defg.row ? 1 : 0;
        else
            return attg.col == defg.col ? 1 : 0;
    }

    public Slot get_def_slot() {
        return def;
    }
    
    public Slot get_att_slot() {
        return att;
    }

    public Unit get_att_unit() {
        return att.get_unit();
    }

    public Unit get_def_unit() {
        return def.get_unit();
    }
}