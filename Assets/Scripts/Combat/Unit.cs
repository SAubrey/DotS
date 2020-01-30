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


    // Attributes
    public const int FLANKING = 1;
    public const int FLYING = 2;
    public const int GROUPING_1 = 3;
    public const int GROUPING_2 = 4;
    public const int STALK = 5;
    public const int PIERCING = 6;
    public const int ARCING_STRIKE = 7;
    public const int AGGRESSIVE = 8;
    public const int ARMOR_1 = 9;
    public const int ARMOR_2 = 10;
    public const int ARMOR_3 = 11;
    public const int ARMOR_4 = 12;
    public const int ARMOR_5 = 13;
    public const int TERROR_1 = 14;
    public const int TERROR_2 = 15;
    public const int TERROR_3 = 16;
    public const int TARGET_RANGE = 17;
    public const int TARGET_HEAVY = 18;
    public const int STUN = 19;
    public const int WEAKNESS_POLEARM = 20;
    public const int DEVASTATING_BLOW = 21;
    public const int TARGET_CENTERFOLD = 22;
    public const int REACH = 23;
    public const int CHARGE = 24;

    // Allied attributes
    public const int INSPIRE = 25;
    public const int HARVEST = 26;
    public const int COUNTER_CHARGE = 27;
    public const int BOLSTER = 28;

    // Attribute fields
    protected int num_attributes = 28;
    public List<bool> attributes = new List<bool>();
    //public bool grouping_active = false;
    protected bool attribute_active = false;
    public bool attribute_requires_action = false; // alters button behavior.
    public bool passive_attribute = false;

    // Combat fields
    public const int MELEE = 1;
    public const int RANGE = 2;
    protected int attack_dmg;
    protected int defense;
    public int health;
    public int max_health;
    public int combat_style;
    public int movement_range = 1;
    public int attack_range = 1;
    public int max_num_actions = 2;
    public int _num_actions;
    public int num_actions {
        get { return _num_actions; }
        set {
            if (value < _num_actions) {
                slot.update_images();
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
    public bool attack_set = false;

    protected int type; // Player or Enemy
    protected int ID; // Unique code for the particular unit type.
    protected string name;
    // Unique identifier for looking up drawn attack lines and aggregating attacks.
    public int attack_id;

    protected Slot slot = null;
    protected bool dead = false; // Used to determine what to remove in the Battalion.
    private bool placed = false;

    public virtual int take_damage(int dmg) { return 0; }
    public virtual int calc_dmg_taken(int dmg, bool piercing=false) { return 0; }
    public virtual float calc_hp_remaining(int dmg) { return 0; }
    public virtual int get_post_dmg_state(int dmg_after_def) { return 0; }
    public virtual int get_attack_dmg() { return attack_dmg; }
    public virtual int get_defense() { return defense; }
    public virtual bool set_attribute_active(bool state) {
        if (state && can_activate_attribute()) {
            attribute_active = true;
            Debug.Log("turning attr on");
        } else {
            attribute_active = false;
            Debug.Log("turning attr off");
        }
        if (slot != null)
            slot.update_UI();
        return attribute_active;
    }

    public virtual void remove_boost() { }

    protected void init(string name, int att, int hp, int style, int atr1, int atr2, int atr3) {
        create_attribute_list(num_attributes);
        this.name = name;
        attack_dmg = att;
        combat_style = style;
        attack_range = style == MELEE ? 1 : 8;
        num_actions = max_num_actions;
        health = hp;
        max_health = hp;

        if (atr1 >= 0)
            attributes[atr1] = true;
        if (atr2 >= 0)
            attributes[atr2] = true;
        if (atr3 >= 0)
            attributes[atr3] = true;
    }

    protected void create_attribute_list(int num_attributes) {
        for (int i = 0; i < num_attributes; i++) {
            attributes.Insert(i, false);
        }
    }

    public bool can_move(Slot dest) {
        bool out_of_range = !in_range(slot.get_punit().movement_range, 
                slot.col, slot.row,
                dest.col, dest.row);
        bool opposite_unit = false;
        if (dest.has_unit)
            opposite_unit = dest.get_unit().type != type;

        if (out_of_actions || has_acted_in_stage || opposite_unit || out_of_range)
            return false;
        return true;
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
        Unit u = s.empty();
        slot.fill(u);
        s.fill(this);
        return true;
    }

    // Check against game-rule limitations.
    public bool can_hit(Slot end) {
        bool attacking_self = slot.get_unit().get_type() == end.get_unit().get_type();
        bool melee_vs_flying = slot.get_unit().combat_style == Unit.MELEE && 
                                end.get_unit().attributes[Unit.FLYING];
        bool out_of_range =
            !in_range(slot.get_unit().attack_range,
                     slot.col, slot.row, end.col, end.row);

        return (attacking_self || melee_vs_flying 
                || out_of_range || out_of_actions) ? false : true;
    }

    public void attack() {
        attack_set = false;
        num_actions--;
        if ((attributes[GROUPING_1] || attributes[GROUPING_2]) && is_attribute_active()){
            foreach (Slot s in slot.get_group().get_full_slots()) {
                s.get_unit().num_actions--;
            }
        }
    }

    public bool can_attack() {
        return (attack_dmg > 0 && !out_of_actions);
    }

    public bool can_defend() {
        return (defense > 0 && !out_of_actions);
    }

    public bool in_range(int range, int x, int y, int x1, int y1) {
        int dx = Mathf.Abs(x - x1);
        int dy = Mathf.Abs(y - y1);
        return (dx + dy <= range) ? true : false;
    }

    // Called at the end of a battle phase.
    public void post_phase() {
        num_actions = max_num_actions;
        has_acted_in_stage = false;
        attack_set = false;
        
        set_attribute_active(false);
        remove_boost();
        if (slot != null) 
            slot.update_UI();
    }
    
    public int get_bonus_health() {
        int sum_hp = 0;
        return sum_hp;
    }

    public int get_bonus_att_dmg() {
        int sum_dmg = 0;
        if (attribute_active && 
            (attributes[Unit.GROUPING_1] || attributes[Unit.GROUPING_2])) {
            sum_dmg += ((1 + attack_dmg) * (get_grouped_units()));
        }
        return sum_dmg;
    }
    
    public int get_bonus_def() {
        int sum_def = 0;
         if (attribute_active && 
            (attributes[Unit.GROUPING_1] || attributes[Unit.GROUPING_2])) {
            sum_def += ((1 + defense) * (get_grouped_units()));
        }
        return sum_def;
    }

    public int get_stat_boost(int type) {
        if (type != active_boost_type)
            return 0;
        return active_boost_amount;
    }

    protected int get_grouped_units() {
        int grouped_units = slot.get_group().get_num_of_same_active_units_in_group(ID);
        if (attributes[Unit.GROUPING_1] && grouped_units > 1) {
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

    public bool is_attribute_active() {
        return attribute_active;
    }

    public int get_raw_attack_dmg() {
        return attack_dmg;
    }
    
    public int get_raw_defense() {
        return defense;
    }

    public bool is_melee() {
        return (combat_style == MELEE) ? true : false;
    }

    public bool is_range() {
        return (combat_style == RANGE) ? true : false;
    }

    public bool is_enemy() {
        return (type == ENEMY) ? true : false;
    }

    public bool is_playerunit() {
        return (type == PLAYER) ? true : false;
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
        placed = slot != null ? true : false;    
    }

    public bool is_dead() {
        return dead;
    }

    public bool is_placed() {
        return placed;
    }
}