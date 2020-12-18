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

    public static readonly List<int> unit_types = new List<int> { WARRIOR, SPEARMAN, ARCHER, 
        MINER, INSPIRATOR, SEEKER, GUARDIAN, ARBALEST, SKIRMISHER, PALADIN,
        MENDER, CARTER, DRAGOON, SCOUT, DRUMMER, SHIELD_MAIDEN, PIKEMAN };

    public const int EMPTY = 100; // Graphical lookup usage.
    public bool injured = false;
    public int owner_ID { get; private set; }

    public static PlayerUnit create_punit(int ID, int owner_ID) {
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
        pu.owner_ID = owner_ID;
        return pu;
    }

    public PlayerUnit(string name, int ID, int att, int def, int hp, int style, 
            int atr1=0, int atr2=0, int atr3=0) : 
            base(name, ID, att, def, hp, style, atr1, atr2, atr3) {
        type = PLAYER;
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
        slot.update_healthbar();

        if (state == INJURED) {
            injured = true;
            //slot.show_injured();
            Controller.I.get_bat_from_ID(owner_ID).add_injured_unit(this);
            BatLoader.I.load_unit_text(Controller.I.get_bat_from_ID(owner_ID), ID);
        } else if (state == DEAD) {
            dead = true;
            //slot.show_dead(); 
            BatLoader.I.load_unit_text(Controller.I.get_bat_from_ID(owner_ID), ID);
            Controller.I.get_bat_from_ID(owner_ID).add_dead_unit(this);
        }
        if (defending) {
            num_actions--;
            defending = false; 
            if (final_dmg > 0) {
                //defense = 0;
                slot.update_defense();
            }
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
                AttackQueuer.I.get_player_queue().remove_attack(attack_id);
            move(end);
        }
        return true;
    }

    public override int get_attack_dmg() {
        return attack_dmg + get_bonus_att_dmg() + get_bonus_from_equipment(Unit.ATTACK_BOOST);
    }

    public override int get_defense() {
        return defense + get_bonus_def() + get_bonus_from_equipment(Unit.DEFENSE_BOOST);
    }

    public override int get_boosted_max_health() {
        return max_health + get_bonus_health() + get_stat_boost(HEALTH_BOOST) +
            get_bonus_from_equipment(Unit.HEALTH_BOOST);
    }

    public int get_bonus_from_equipment(int stat_ID) {
        Discipline d = Controller.I.get_bat_from_ID(owner_ID).disc;
        return d.equipment_inventory.get_stat_boost_amount(ID, stat_ID);
    }

    public override int calc_dmg_taken(int dmg, bool piercing=false) {
        if (defending && !piercing) {
            dmg -= get_defense();
        }
        return dmg > 0 ? dmg : 0;
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
        Formation f = Formation.I;
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
    public Warrior() : base("Warrior", WARRIOR, 1, 1, 3, MELEE, GROUPING_1) {
        attribute_requires_action = true;
    }
}

public class Spearman : PlayerUnit {
    public Spearman() : base("Spearman", SPEARMAN, 1, 2, 2, MELEE, COUNTER_CHARGE) {
        passive_attribute = true;
    }
}

public class Archer : PlayerUnit {
    public Archer() : base("Archer", ARCHER, 2, 0, 1, RANGE) {
        passive_attribute = true;
    }
}

public class Miner : PlayerUnit {
    public Miner() : base("Miner", MINER, 1, 0, 3, MELEE, HARVEST) {
        passive_attribute = true;
    }
}

public class Inspirator : PlayerUnit {
    public Inspirator() : base("Inspirator", INSPIRATOR, 0, 0, 1, MELEE, INSPIRE) {}

    public override bool set_attribute_active(bool state) {
        if (attribute_active == state)
            return false; // prevent double application/depplication
        bool active = base.set_attribute_active(state);
        if (active) {
            apply_surrounding_effect(HEALTH_BOOST, 1, get_forward3x1_coords());
            // affecting num actions directly bypasses one-way street to allow
            // the user to undo and redo the buff. This means has_acted_in_stage
            // does not get enabled for inspirator, which only matters if units
            // are cycling after having acted.
            _num_actions--;
        } else {
            apply_surrounding_effect(HEALTH_BOOST, -1, get_forward3x1_coords());
            _num_actions++;
        }
        slot.update_staminabar(num_actions);
        return active;
    }
}

public class Seeker : PlayerUnit {
    public Seeker() : base("Seeker", SEEKER, 1, 1, 2, MELEE, TRUE_SIGHT) {}
}

public class Guardian : PlayerUnit {
    public Guardian() : base("Guardian", GUARDIAN, 2, 3, 5, MELEE, PARRY) {}
}

public class Arbalest : PlayerUnit {
    public Arbalest() : base("Arbalest", ARBALEST, 3, 0, 2, RANGE, PIERCING) {
        passive_attribute = true;
    }
}

public class Mender : PlayerUnit {
    public Mender() : base("Mender", MENDER, 0, 3, 4, MELEE, HEAL_1) {
        max_num_actions = 1;
        _num_actions = max_num_actions;
    }

    public override bool set_attribute_active(bool state) {
        bool active = base.set_attribute_active(state);
        if (active) {
            BatLoader.I.selecting_for_heal = active;
            BatLoader.I.healing_unit = this;
        }
        return active;
    }
}

public class Skirmisher : PlayerUnit {
    public Skirmisher() : base("Skirmisher", SKIRMISHER, 2, 2, 2, RANGE, STUN, GROUPING_2) {
        attribute_requires_action = true;
    }
}

public class Scout : PlayerUnit {
    public Scout() : base("Scout", SCOUT, 3, 0, 2, RANGE, PIERCING) {
        passive_attribute = true;
        // Double strike
        max_num_actions = 3;
        _num_actions = max_num_actions;
    }
}

public class Carter : PlayerUnit {
    public Carter() : base("Carter", CARTER, 2, 2, 4, MELEE) {
        passive_attribute = true;
        // inv increase by 6
    }
}

public class Dragoon : PlayerUnit {
    public Dragoon() : base("Dragoon", DRAGOON, 4, 1, 3, MELEE, GROUPING_2, PIERCING) {}
}

public class Paladin : PlayerUnit {
    public Paladin() : base("Paladin", PALADIN, 2, 2, 4, MELEE, GROUPING_2) {
        attribute_requires_action = true;
    }
}

public class Drummer : PlayerUnit {
    public Drummer() : base("Drummer", DRUMMER, 1, 1, 2, MELEE, BOLSTER) {}

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
    public ShieldMaiden() : base("Shield Maiden", SHIELD_MAIDEN, 3, 4, 4, MELEE, GROUPING_1, COMBINED_EFFORT) {
        attribute_requires_action = true;
    }
}

public class Pikeman : PlayerUnit {
    public Pikeman() : base("Pikeman", PIKEMAN, 3, 1, 3, MELEE, REACH, PIERCING, COUNTER_CHARGE) {
        passive_attribute = true;
    }
}