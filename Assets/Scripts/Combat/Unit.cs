using System.Collections.Generic;
using UnityEngine;

public class Unit {
    public const int PLAYER = 0;
    public const int ENEMY = 1;

    // Used to determine post-damage decision making. 
    public const int ALIVE = 0;
    public const int DEAD = 1;
    public const int INJURED = 2;

    // Boost IDs
    public const int HEALTH_BOOST = 1;
    public const int DEFENSE_BOOST = 2;
    public const int ATTACK_BOOST = 3;


    // Attributes (0 is null atr)
    public const int FLANKING = 1;
    public const int FLYING = 2;
    public const int GROUPING_1 = 3;
    public const int GROUPING_2 = 4;
    public const int STALK = 5;
    public const int PIERCING = 6;
    public const int ARCING_STRIKE = 7;
    public const int AGGRESSIVE = 8;
    public const int TARGET_RANGE = 9;
    public const int TARGET_HEAVY = 10;
    public const int STUN = 11;
    public const int CHARGE = 12;
    public const int PARRY = 13;
    public const int PENETRATING_BLOW = 14;
    public const int CRUSHING_BLOW = 15;

    // Enemy only attributes
    public const int TERROR_1 = 16;
    public const int TERROR_2 = 17;
    public const int TERROR_3 = 18;
    public const int WEAKNESS_POLEARM = 19;

    // Allied only attributes
    public const int REACH = 21;
    public const int INSPIRE = 22;
    public const int HARVEST = 23;
    public const int COUNTER_CHARGE = 24;
    public const int BOLSTER = 25;
    public const int TRUE_SIGHT = 26;
    public const int HEAL_1 = 27;
    public const int COMBINED_EFFORT = 28;

    // Attribute fields
    public int attribute1, attribute2, attribute3 = 0;
    protected bool attribute_active = false;
    public bool attribute_requires_action = false; // alters button behavior.
    public bool passive_attribute = false;

    // Combat fields
    public const int MELEE = 1;
    public const int RANGE = 2;
    protected int attack_dmg, defense;
    public int health, max_health;
    public int combat_style;
    public int movement_range = 1;
    public int attack_range = 1;
    public int max_num_actions = 2;
    protected int _num_actions;
    public int num_actions {
        get { return _num_actions; }
        set {
            if (value < _num_actions) {
                if (slot != null)
                    slot.update_num_actions(value);
                has_acted_in_stage = true;
            }

            _num_actions = value;
            if (_num_actions >= max_num_actions) {
                has_acted_in_stage = false;
            }
        }
    }
    public bool out_of_actions { get { return num_actions <= 0; } }
    public bool has_acted_in_stage = false;
    private bool _attack_set = false;
    public bool attack_set {
        get { return _attack_set; }
        set {
            if (out_of_actions)
                value = false;
            _attack_set = value;
            if (slot != null)
                slot.update_attack();
        }
    }
    private bool _defending = false;
    public bool defending {
        get { return _defending; }
        set {
            if (out_of_actions)
                value = false;
            _defending = value;
            if (slot != null)
                slot.update_defense();
        }
    }

    protected int type; // Player or Enemy
    protected int ID; // Code for the particular unit type. (not unique to unit)
    protected string name;
    // Unique identifier for looking up drawn attack lines and aggregating attacks.
    public int attack_id;

    protected Slot slot = null;
    protected bool dead = false; // Used to determine what to remove in the Battalion.

    private static int static_unique_ID = 0;
    private int unique_ID = static_unique_ID;

    public virtual int take_damage(int dmg) { return 0; }
    public virtual int calc_dmg_taken(int dmg, bool piercing=false) { return 0; }
    public virtual float calc_hp_remaining(int dmg) { return 0; }
    public virtual int get_post_dmg_state(int dmg_after_def) { return 0; }
    public virtual int get_attack_dmg() { return attack_dmg; }
    public virtual int get_defense() { return defense; }
    public virtual bool set_attribute_active(bool state) {
        attribute_active = state && can_activate_attribute();
        //Debug.Log("turning attr " + attribute_active);
        if (is_placed)
            slot.update_UI();
        return attribute_active;
    }

    public virtual void remove_boost() { }

    protected void init(string name, int att, int hp, int style, 
            int atr1=0, int atr2=0, int atr3=0) {
        this.name = name;
        attack_dmg = att;
        combat_style = style;
        attack_range = style == MELEE ? 1 : 8;
        num_actions = max_num_actions;
        health = hp;
        max_health = hp;

        unique_ID = static_unique_ID;
        static_unique_ID++;

        attribute1 = atr1;
        attribute2 = atr2;
        attribute3 = atr3;
    }

    public bool has_attribute(int atr_ID) {
        return (attribute1 == atr_ID || 
                attribute2 == atr_ID || 
                attribute3 == atr_ID);
    }

    public bool can_move(Slot dest) {
        bool out_of_range = !in_range(movement_range, 
                slot.col, slot.row,
                dest.col, dest.row);
        bool opposite_unit = false;
        if (dest.has_unit)
            opposite_unit = dest.get_unit().type != type;

        return !(out_of_actions || has_acted_in_stage || opposite_unit || out_of_range);
    }
    
    protected void move(Slot end) {
        slot.empty();
        end.fill(this);
        num_actions--;
        end.get_group().validate_unit_order();
    }

    protected bool swap_places(Slot s) {
        if (!can_move(s) || !s.get_unit().can_move(slot))
            return false;

        s.get_unit().num_actions--;
        num_actions--;
        Unit u = s.empty(false); 
        slot.fill(u);
        s.fill(this);
        return true;
    }

    public bool attempt_set_up_attack(Slot target_slot) {
        if (!target_slot)
            return false;
        if (!target_slot.has_unit)
            return false;
        if (!can_hit(target_slot))
            return false;

        slot.c.attack_queuer.add_attack(slot, target_slot);
        slot.update_attack();
        return true;
    }

    // Check against game-rule limitations.
    public bool can_hit(Slot end) {
        bool attacking_self = type == end.get_unit().get_type();
        bool melee_vs_flying = slot.get_unit().combat_style == Unit.MELEE && 
                                end.get_unit().has_attribute(Unit.FLYING);
       
        bool out_of_range = has_attribute(REACH) ? 
            !in_range_of_reach(attack_range,
                     slot.col, slot.row, end.col, end.row) :
            !in_range(attack_range,
                     slot.col, slot.row, end.col, end.row);

        return !attacking_self && !melee_vs_flying 
                && !out_of_range && !out_of_actions;
    }

    public void attack() {
        attack_set = false;
        if (is_actively_grouping) {
            foreach (Slot s in slot.get_group().get_full_slots()) {
                s.get_unit().num_actions--;
            }
        } else {
            num_actions--;
        }
    }

    // Called at the end of a battle phase.
    public void post_phase() {
        num_actions = max_num_actions;
        has_acted_in_stage = false;
        attack_set = false;
        
        set_attribute_active(false);
        //remove_boost();
        if (slot != null) { // Not all units are placed.
            slot.update_UI();
        }
    }

    public bool can_attack() {
        return attack_dmg > 0 && !out_of_actions;
    }

    public bool can_defend() {
        return defense > 0 && !out_of_actions;
    }

    // Checks range for each direction additively, forming a diamond.
    public static bool in_range(int range, int x, int y, int x1, int y1) {
        int dx = Mathf.Abs(x - x1);
        int dy = Mathf.Abs(y - y1);
        return dx + dy <= range;
    }

    // Checks range for each direction separately, forming a square.
    public static bool in_range_of_reach(int range, int x, int y, int x1, int y1) {
        int dx = Mathf.Abs(x - x1);
        int dy = Mathf.Abs(y - y1);
        return dx <= range && dy <= range;
    }

    public int get_boosted_max_health( ) {
        return max_health + get_bonus_health() + get_stat_boost(HEALTH_BOOST);
    }
    
    // "Bonus" refers to any stat increases not from boost-type attributes.
    public int get_bonus_health() {
        int sum_hp = 0;
        // hp from non-boost attr?
        return sum_hp;
    }

    public int get_bonus_att_dmg() {
        int sum_dmg = 0;
        if (is_actively_grouping) {
            sum_dmg += ((1 + attack_dmg) * (count_grouped_units()));
        }
        return sum_dmg;
    }
    
    public int get_bonus_def() {
        int sum_def = 0;
         if (is_actively_grouping) {
            sum_def += ((1 + defense) * (count_grouped_units()));
        }
        return sum_def;
    }

    public int get_stat_boost(int type) {
        if (type != active_boost_type)
            return 0;
        return active_boost_amount;
    }

    // Returns number of same units in group with Grouping that have actions remaining.
    protected int count_grouped_units() {
        int grouped_units = slot.get_group().get_num_of_same_active_units_in_group(ID);
        if (has_attribute(Unit.GROUPING_1) && grouped_units > 1) {
            grouped_units = 1;
        }
        return grouped_units;
    }
    
    public int active_boost_type = -1;
    public int active_boost_amount = 0;
    protected void affect_boosted_stat(int boost_type, int amount) {
        active_boost_type = boost_type;
        active_boost_amount = amount;

        if (boost_type == HEALTH_BOOST)
            health += amount;
        else if (boost_type == ATTACK_BOOST) {
            attack_dmg += amount;
        }
        if (boost_type == DEFENSE_BOOST) {
            defense += amount;
        }
    }

    private bool _boosted = false;
    public bool boosted {
        get { return _boosted; }
        set { 
            _boosted = value;
            if (!value) {
                active_boost_type = -1;
                active_boost_amount = 0;
            }
            if (slot != null)
                slot.update_UI();
        }
    }

    /*
    This parent class version does boolean checks for aspects
    that apply to all player units.
    */
    public virtual bool can_activate_attribute() {
        if (passive_attribute || out_of_actions || has_acted_in_stage)
            return false;
        return true;
    }
    
    protected List<Pos> get_forward3x1_coords() {
        int direction = slot.get_group().get_dir();
        Pos low = new Pos(slot.col, slot.row);
        Pos high = new Pos(slot.col, slot.row);
        if (direction == Group.UP) {
            low.x--;
            low.y++;
            high.x++;
            high.y++;
        } else if (direction == Group.DOWN) {
            low.x--;
            low.y--;
            high.x++;
            high.y--;
        } else if (direction == Group.LEFT) {
            low.x--;
            low.y--;
            high.x--;
            high.y++;
        } else if (direction == Group.RIGHT) {
            low.x++;
            low.y--;
            high.x++;
            high.y++;
        }
        //Debug.Log(low.x + ",  " + low.y + ". High: " + high.x + ", " + high.y);
        return new List<Pos>() {low, high};
    }


    public int get_raw_attack_dmg() {
        return attack_dmg;
    }
    
    public int get_raw_defense() {
        return defense;
    }

    public int get_type() {
        return type;
    }

    public string get_name() {
        return name;
    }

    public int get_ID() {
        return ID;
    }

    public Slot get_slot() {
        return slot;
    }

    public void set_slot(Slot s) {
        slot = s;
    }

    public bool is_actively_grouping { 
        get { return attribute_active && has_grouping; }
    }
    public bool has_grouping { 
        get { return has_attribute(GROUPING_1) || has_attribute(GROUPING_2); }
    }
    
    public bool is_attribute_active { get { return attribute_active; } }
    public bool is_melee { get { return combat_style == MELEE; } }
    public bool is_range { get { return combat_style == RANGE; } }
    public bool is_enemy { get { return type == ENEMY; } }
    public bool is_playerunit { get { return type == PLAYER; } }
    public bool is_dead { get { return dead; } }
    public bool is_placed { get { return slot != null; } }
}