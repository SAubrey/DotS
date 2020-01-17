﻿using System.Collections.Generic;
using UnityEngine;

public class Unit {
    public const int PLAYER = 0;
    public const int ENEMY = 1;

    // Used to determine post-damage decision making. 
    public const int ALIVE = 0;
    public const int DEAD = 1;
    public const int INJURED = 2;

    // Attributes
    public const int CHARGE = 0;
    public const int FLANKING = 1;
    public const int FLYING = 2;
    public const int GROUPING_1 = 3;
    public const int GROUPING_2 = 4;
    public const int GROUPING_3 = 5;
    public const int GROUPING_4 = 6;
    public const int STALK = 7;
    public const int PIERCING_BOLT = 8;
    public const int ARCING_STRIKE = 9;
    public const int AGGRESSIVE = 10;
    public const int ARMOR_1 = 11;
    public const int ARMOR_2 = 12;
    public const int ARMOR_3 = 13;
    public const int ARMOR_4 = 14;
    public const int ARMOR_5 = 15;
    public const int TERROR_1 = 16;
    public const int TERROR_2 = 17;
    public const int TERROR_3 = 18;
    public const int TARGET_RANGE = 19;
    public const int TARGET_HEAVY = 20;
    public const int STUN = 21;
    public const int WEAKNESS_POLEARM = 22;
    public const int DEVASTATING_BLOW = 23;
    public const int TARGET_CENTERFOLD = 24;

    // Allied attributes
    public const int INSPIRE = 25;
    public const int HARVEST = 26;
    public const int COUNTER_CHARGE = 27;

    protected int num_attributes = 28;

    // Attack styles
    public const int MELEE = 1;
    public const int RANGE = 2;
    public int attack_dmg;
    public int defense;
    public int combat_style;
    public int movement_range = 1;
    public int attack_range = 1;
    public int max_num_actions = 2;
    public int _num_actions;
    public int num_actions {
        get { return _num_actions; }
        set {
            if (value < _num_actions) 
                has_acted_in_stage = true;

            _num_actions = value;
            if (_num_actions >= max_num_actions) {
                has_acted_in_stage = false;
            }
        }
    }
    public bool out_of_actions { get { return num_actions <= 0; } }
    public bool has_acted_in_stage = false;
    public bool attack_set = false;

    public List<bool> attributes = new List<bool>();

    protected int type; // Player or Enemy
    protected int ID; // Unique code for the particular unit type.
    protected string name;
    protected Slot slot = null;
    protected bool dead = false; // Used to determine what to remove in the Battalion.
    private bool placed = false;
    // Unique identifier for looking up drawn attack lines and aggregating attacks.
    public int attack_id;

    public virtual int take_damage(int dmg) { return 0; }
    public virtual int calc_dmg_taken(int dmg) { return 0; }
    public virtual float calc_hp_remaining(int dmg) { return 0; }
    public virtual int get_post_dmg_state(int dmg_after_def) { return 0; }
    protected void init(string name, int att, int style, int atr1, int atr2, int atr3) {
        create_attribute_list(num_attributes);
        this.name = name;
        attack_dmg = att;
        combat_style = style;
        attack_range = style == MELEE ? 1 : 9;
        num_actions = max_num_actions;

        if (atr1 >= 0)
            attributes[atr1] = true;
        if (atr2 >= 0)
            attributes[atr2] = true;
        if (atr3 >= 0)
            attributes[atr3] = true;
    }
    
    protected void move(Slot end) {
        slot.empty();
        end.fill(this);
        //end.fill(slot.empty());
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

    public int attack() {
        attack_set = false;
        num_actions--;
        return attack_dmg;
    }

    protected void create_attribute_list(int num_attributes) {
        for (int i = 0; i < num_attributes; i++) {
            attributes.Insert(i, false);
        }
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
        
        if (get_slot() != null) {
            slot.update_healthbar();
        }
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