using UnityEngine;

public class PlayerUnit : Unit {
    public const int WARRIOR = 0;
    public const int SPEARMAN = 1;
    public const int ARCHER = 2;
    public const int MINER = 3;
    public const int INSPIRITOR = 4;
    public const int EMPTY = 5; // Graphical lookup usage.

    public int defense;
    public float resilience;
    private float injury_thresh;
    public bool injured = false;

    protected void init(string name, int att, int def, int res, int mvmt_range, int style,
        int atr1=-1, int atr2=-1, int atr3=-1) {
        create_attribute_list(num_attributes);
        type = PLAYER;
        this.name = name;
        attack = att;
        defense = def;
        resilience = res;
        movement_range = mvmt_range;
        combat_style = style;
        attack_range = style == MELEE ? 1 : 9;

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
        else if (ID == INSPIRITOR) pu = new Inspiritor();
        return pu;
    }

    /* A player unit is injured if the damage taken is equal or greater
       than half its resilience rounded down. Returns true if dead.
       Returns 1 if dead, 2 if injured, 0 if no effect. */ 
    public override int take_damage(int dmg) {
        int state = get_post_dmg_state(calc_dmg_taken(dmg));
        if (state == INJURED) {
            injured = true;
            // show injured
            Debug.Log(this.ToString() + " has been injured.");
        } else if (state == DEAD) {
            dead = true;
            slot.show_dead();
            slot.c.bat_loader.load_text(slot.c.get_active_bat());
            slot.c.get_active_bat().add_dead_unit(this);
        }
        return state;
    }

    public bool attempt_move(Slot end) {
        if (has_acted || (end.get_group().has_enemy() || 
            !in_range(slot.get_punit().movement_range, 
                slot.col, slot.row,
                end.col, end.row))) 
            return false;
        move(end);
        end.selector.deselect();
        return true;
    }

    public override int calc_dmg_taken(int dmg) {
        int final_dmg = dmg - defense;
        return final_dmg > 0 ? final_dmg : 0;
    }

    // Passed damage should have already accounted for defense reduction.
    public int get_post_dmg_state(int dmg) {
        float damaged_resilience = resilience - dmg;
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

public class Inspiritor : PlayerUnit {
    public Inspiritor() {
        init("Inspiritor", 0, 0, 1, 1, MELEE, INSPIRE);
        ID = INSPIRITOR;
    }
}