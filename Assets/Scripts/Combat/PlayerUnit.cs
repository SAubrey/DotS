using UnityEngine;
using System.Collections.Generic;

public class PlayerUnit : Unit {
    public const int WARRIOR = 0;
    public const int SPEARMAN = WARRIOR + 1;
    public const int ARCHER = SPEARMAN + 1;
    public const int MINER = ARCHER + 1;
    public const int INSPIRATOR = MINER + 1;
    public const int SEEKER = INSPIRATOR + 1;
    public const int GUARDIAN = SEEKER + 1;
    public const int ARBALEST = GUARDIAN + 1;
    public const int SKIRMISHER = ARBALEST + 1;
    public const int PALADIN = SKIRMISHER + 1;
    public const int MENDER = PALADIN + 1;
    public const int CARTER = MENDER + 1;
    public const int DRAGOON = CARTER + 1;
    public const int SCOUT = DRAGOON + 1;
    public const int DRUMMER = SCOUT + 1;
    public const int SHIELD_MAIDEN = DRUMMER + 1;
    public const int PIKEMAN = SHIELD_MAIDEN + 1;

    public static List<int> unit_types = new List<int> { WARRIOR, SPEARMAN, ARCHER, 
        MINER, INSPIRATOR, SEEKER, GUARDIAN, ARBALEST, SKIRMISHER, PALADIN,
        MENDER, CARTER, DRAGOON, SCOUT, DRUMMER, SHIELD_MAIDEN, PIKEMAN };

    public const int EMPTY = 100; // Graphical lookup usage.
    public bool injured = false;

    protected void init(string name, int att, int def, int res, 
            int style, int atr1=0, int atr2=0, int atr3=0) {
        base.init(name, att, res, style, atr1, atr2, atr3);
        type = PLAYER;
        defense = def;
    }

    public static PlayerUnit create_punit(int ID) {
        PlayerUnit pu = null;
        if (ID == WARRIOR) pu = new Warrior(); 
        else if (ID == SPEARMAN) pu = new Spearman();
        else if (ID == ARCHER) pu = new Archer();
        else if (ID == MINER) pu = new Miner();
        else if (ID == INSPIRATOR) pu = new Inspirator();
        else if (ID == SEEKER) pu = new Seeker();
        else if (ID == GUARDIAN) pu = new Guardian();
        else if (ID == ARBALEST) pu = new Arbalest();
        else if (ID == SKIRMISHER) pu = new Skirmisher();
        else if (ID == PALADIN) pu = new Paladin();
        else if (ID == MENDER) pu = new Mender();
        else if (ID == DRUMMER) pu = new Drummer();
        else if (ID == PIKEMAN) pu = new Pikeman();
        else if (ID == CARTER) pu = new Carter();
        else if (ID == DRAGOON) pu = new Dragoon();
        else if (ID == SCOUT) pu = new Scout();
        else if (ID == SHIELD_MAIDEN) pu = new ShieldMaiden();
        return pu;
    }

    /* A player unit is injured if the damage taken is equal or greater
       than half its resilience rounded down. Returns true if dead.
       Returns 1 if dead, 2 if injured, 0 if no effect. 
       ! Do not remove a slot's unit directly! Cleanup happens after the
       battle sequence in Battalion.
       final damage = (attack power + flank damage) - defense
       */ 
    public override int take_damage(int final_dmg) {
        int state = get_post_dmg_state(final_dmg);
        health = (int)calc_hp_remaining(final_dmg);
        slot.update_healthbar(health);

        if (state == INJURED) {
            injured = true;
            slot.show_injured();
            slot.c.get_active_bat().add_injured_unit(this);
            slot.c.bat_loader.load_text(slot.c.get_active_bat(), ID);
        } else if (state == DEAD) {
            dead = true;
            slot.show_dead(); 
            slot.c.bat_loader.load_text(slot.c.get_active_bat(), ID);
            slot.c.get_active_bat().add_dead_unit(this);
        }
        if (defending) {
            num_actions--;
            defending = false; 
        }
        return state;
    }

    public bool attempt_move(Slot end) {
        if (!can_move(end))
            return false;
        if (end.get_punit() != null && !end.get_punit().out_of_actions) {
            return swap_places(end);
        } else {
            if (attack_set)
                slot.c.attack_queuer.get_player_queue().remove_attack(attack_id, slot.c.line_drawer);
            move(end);
        }
        return true;
    }

    // Accounts for additional attribute damage.
    public override int get_attack_dmg() {
        int sum_dmg = attack_dmg;
        sum_dmg += get_bonus_att_dmg();
        return sum_dmg;
    }

    // Accounts for additional attribute defense.
    public override int get_defense() {
        int sum_def = defense;
        sum_def += get_bonus_def();
        return sum_def;
    }

    public override int calc_dmg_taken(int dmg, bool piercing=false) {
        if (defending && !piercing)
            dmg -= get_defense();
        return dmg > 0 ? dmg : 0;
    }

    // Passed damage should have already accounted for possible defense reduction.
    public override float calc_hp_remaining(int dmg) {
        return Mathf.Max(health - dmg, 0);
    }

    // Passed damage should have already accounted for possible defense reduction.
    public override int get_post_dmg_state(int dmg_after_def) {
        float damaged_resilience = calc_hp_remaining(dmg_after_def);
        if (damaged_resilience <= 0) {
            return DEAD;
        } else if (damaged_resilience < ((float)health / 2f)) {
            return INJURED;
        }
        return ALIVE;
    }

    // ---ATTRIBUTES---

    protected void apply_surrounding_effect(int boost_type, int amount, List<Pos> coords) {
        Formation f = slot.c.formation;
        Group g;
        Pos low = coords[0];
        Pos high = coords[1];
        for (int col = low.x; col <= high.x; col++) {
            for (int row = low.y; row <= high.y; row++) {
                g = f.get_group(col, row);
                if (!g) 
                    continue;

                foreach (Slot s in g.slots) {
                    if (!s.has_punit) // Skip empty or enemies.
                        continue;
                        
                    if (s.get_punit().boosted) {
                        s.get_punit().remove_boost();
                    } else {
                        s.get_punit().boost(boost_type, amount);
                    }
                }
            }
        }
    }

    // ---BOOSTS---
    protected void boost(int boost_type, int amount) {
        if (boosted)
            return;
        affect_boosted_stat(boost_type, amount);
        boosted = true;
    }

    public override void remove_boost() {
        if (!boosted)
            return;
        affect_boosted_stat(active_boost_type, -active_boost_amount);
        boosted = false;
    }

}

public class Warrior : PlayerUnit {
    public Warrior() {
        init("Warrior", 1, 1, 3, MELEE, GROUPING_1);
        ID = WARRIOR;
        attribute_requires_action = true;
    }
}

public class Spearman : PlayerUnit {
    public Spearman() {
        init("Spearman", 1, 2, 2, MELEE, COUNTER_CHARGE);
        ID = SPEARMAN;
        passive_attribute = true;
    }
}

public class Archer : PlayerUnit {
    public Archer() {
        init("Archer", 2, 0, 1, RANGE);
        ID = ARCHER;
        passive_attribute = true;
    }
}

public class Miner : PlayerUnit {
    public Miner() {
        init("Miner", 1, 0, 3, MELEE, HARVEST);
        ID = MINER;
        passive_attribute = true;
    }
}

public class Inspirator : PlayerUnit {
    public Inspirator() {
        init("Inspirator", 0, 0, 1, MELEE, INSPIRE);
        ID = INSPIRATOR;
    }

    public override bool set_attribute_active(bool state) {
        if (attribute_active == state)
            return false; // prevent double application/depplication
        bool active = base.set_attribute_active(state);
        if (active) {
            apply_surrounding_effect(HEALTH_BOOST, 1, get_forward3x1_coords());
        } else {
            apply_surrounding_effect(HEALTH_BOOST, -1, get_forward3x1_coords());
        }
        return active;
    }
}

public class Seeker : PlayerUnit {
    public Seeker() {
        init("Seeker", 1, 1, 2, MELEE, TRUE_SIGHT);
        ID = SEEKER;
    }
}

public class Guardian : PlayerUnit {
    public Guardian() {
        init("Guardian", 2, 3, 5, MELEE, PARRY);
        ID = GUARDIAN;
    }
}

public class Arbalest : PlayerUnit {
    public Arbalest() {
        init("Arbalest", 3, 0, 2, RANGE, PIERCING);
        ID = ARBALEST;
        passive_attribute = true;
    }
}

public class Mender : PlayerUnit {
    public Mender() {
        init("Mender", 0, 3, 4, MELEE, HEAL_1);
        ID = MENDER;
        max_num_actions = 1;
        _num_actions = max_num_actions;
    }

    public override bool set_attribute_active(bool state) {
        bool active = base.set_attribute_active(state);
        if (active) {
            slot.c.bat_loader.selecting_for_heal = active;
            slot.c.bat_loader.healing_unit = this;
        }
        return active;
    }
}

public class Skirmisher : PlayerUnit {
    public Skirmisher() {
        init("Skirmisher", 2, 2, 2, RANGE, STUN, GROUPING_2); 
        ID = SKIRMISHER;
        // "range or melee" achieved by range
    
    }
}

public class Scout : PlayerUnit {
    public Scout() {
        init("Scout", 3, 0, 2, RANGE, PIERCING);
        ID = SCOUT;
        passive_attribute = true;
        // Double strike
        max_num_actions = 3;
        _num_actions = max_num_actions;
    }
}

public class Carter : PlayerUnit {
    public Carter() {
        init("Carter", 2, 2, 4, MELEE);
        ID = CARTER;
        passive_attribute = true;
        // inv increase by 6
    }
}

public class Dragoon : PlayerUnit {
    public Dragoon() {
        init("Dragoon", 4, 1, 3, MELEE, GROUPING_2, PIERCING);
        ID = DRAGOON;
        movement_range = 2;
    }
}

public class Paladin : PlayerUnit {
    public Paladin() {
        init("Paladin", 2, 2, 4, MELEE, GROUPING_2);
        ID = PALADIN;
    }
}

public class Drummer : PlayerUnit {
    public Drummer() {
        init("Drummer", 1, 1, 2, MELEE, BOLSTER);
        ID = DRUMMER;
        // grouping 2 - does this still apply?
    }

    public override bool set_attribute_active(bool state) {
        if (attribute_active == state)
            return false; // prevent double application/depplication
        bool active = base.set_attribute_active(state);
        if (active) {
            apply_surrounding_effect(DEFENSE_BOOST, 1, get_forward3x1_coords());
        } else {
            apply_surrounding_effect(DEFENSE_BOOST, -1, get_forward3x1_coords());
        }
        return active;
    }
}

public class ShieldMaiden : PlayerUnit {
    public ShieldMaiden() {
        init("Shield Maiden", 3, 4, 4, MELEE, GROUPING_1, COMBINED_EFFORT);
        ID = SHIELD_MAIDEN;
    }
}

public class Pikeman : PlayerUnit {
    public Pikeman() {
        init("Pikeman", 3, 1, 3, MELEE, REACH, PIERCING, COUNTER_CHARGE);
        ID = PIKEMAN;
        passive_attribute = true;
    }
}