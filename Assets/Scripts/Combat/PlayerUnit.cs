using UnityEngine;

public class PlayerUnit : Unit {
    public const int WARRIOR = 0;
    public const int SPEARMAN = 1;
    public const int ARCHER = 2;
    public const int MINER = 3;
    public const int INSPIRATOR = 4;
    public const int EMPTY = 5; // Graphical lookup usage.

    public float resilience;
    private float injury_thresh;
    public bool injured = false;
    private bool _defending = false;
    public bool defending {
        get { return _defending; }
        set {
            if (value && !has_acted) {
                _defending = true;
                slot.show_defensive(true);
            } else {
                _defending = false;
                slot.show_defensive(false);
            }
        }
    }

    protected void init(string name, int att, int def, int res, 
            int mvmt_range, int style,
            int atr1=-1, int atr2=-1, int atr3=-1) {
        create_attribute_list(num_attributes);
        type = PLAYER;
        this.name = name;
        attack_dmg = att;
        defense = def;
        resilience = res;

        num_actions = max_num_actions;
        movement_range = mvmt_range;
        combat_style = style;
        attack_range = style == MELEE ? 1 : 9;

        //this.sc_cost = sc_cost;
        //mineral_cost = m_cost;

        if (atr1 >= 0)
            attributes[atr1] = true;
        if (atr2 >= 0)
            attributes[atr2] = true;
        if (atr3 >= 0)
            attributes[atr3] = true;
    }

    public static PlayerUnit create_punit(int ID) {
        PlayerUnit pu = null;
        if (ID == WARRIOR) pu = new Warrior(); 
        else if (ID == SPEARMAN) pu = new Spearman();
        else if (ID == ARCHER) pu = new Archer();
        else if (ID == MINER) pu = new Miner();
        else if (ID == INSPIRATOR) pu = new Inspirator();
        return pu;
    }

    /* A player unit is injured if the damage taken is equal or greater
       than half its resilience rounded down. Returns true if dead.
       Returns 1 if dead, 2 if injured, 0 if no effect. 
       ! Do not remove a slot's unit directly! Cleanup happens after the
       battle sequence in Battalion
       final damage = (attack power + flank damage) - defense
       */ 
    public override int take_damage(int raw_dmg) {
        int dmg_after_def = calc_dmg_taken(raw_dmg);
        int state = get_post_dmg_state(dmg_after_def);
        slot.update_healthbar(calc_hp_remaining(dmg_after_def));

        if (state == INJURED) {
            injured = true;
            slot.show_injured();
            slot.c.get_active_bat().add_injured_unit(this);
        } else if (state == DEAD) {
            dead = true;
            slot.show_dead(); 
            slot.c.bat_loader.load_text(slot.c.get_active_bat());
            slot.c.get_active_bat().add_dead_unit(this);
        }
        if (defending) {
            has_acted = true;
            defending = false; // Defense only works on first attack this way.
        }
        return state;
    }

    public bool attempt_move(Slot end) {
        if (!can_move(end))
            return false;
        if (end.get_punit() != null) {
            if (!end.get_punit().has_moved) {
                return swap_places(end);
            }
        } else {
            move(end);
        }
        return true;
    }

    public override int calc_dmg_taken(int dmg) {
        if (defending)
            dmg -= defense;
        return dmg > 0 ? dmg : 0;
    }

    public override float calc_hp_remaining(int dmg) {
        float damaged_hp = resilience - dmg;
        return damaged_hp;
    }

    // Passed damage should have already accounted for possible defense reduction.
    public override int get_post_dmg_state(int dmg_after_def) {
        float damaged_resilience = resilience - dmg_after_def;
        if (damaged_resilience <= 0) {
            return DEAD;
        } else if (damaged_resilience < (resilience / 2f)) {
            return INJURED;
        }
        return ALIVE;
    }
}

public class Warrior : PlayerUnit {
    public Warrior() {
        init("Warrior", 1, 1, 3, 1, MELEE, GROUPING_1);
        ID = WARRIOR;
    }
}

public class Spearman : PlayerUnit {
    public Spearman() {
        init("Spearman", 1, 2, 2, 1, MELEE, COUNTER_CHARGE);
        ID = SPEARMAN;
    }
}

public class Archer : PlayerUnit {
    public Archer() {
        init("Archer", 2, 0, 1, 1, RANGE);
        ID = ARCHER;
    }
}

public class Miner : PlayerUnit {
    public Miner() {
        init("Miner", 1, 0, 3, 1, MELEE, HARVEST);
        ID = MINER;
    }
}

public class Inspirator : PlayerUnit {
    public Inspirator() {
        init("Inspirator", 0, 0, 1, 1, MELEE, INSPIRE);
        ID = INSPIRATOR;
    }
}