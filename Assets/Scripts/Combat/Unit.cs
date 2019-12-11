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
    public int attack;
    public int combat_style;
    public int movement_range = 1;
    public int attack_range = 1;
    public bool attack_set = false;

    public List<bool> attributes = new List<bool>();

    protected int type; // Player or Enemy
    protected int ID;
    protected string name;
    protected Slot slot = null;
    protected bool dead = false; // Used to determine what to remove in the Battalion.
    private bool placed = false;
    public int line_id; // unique identifier for looking up drawn attack lines.

    public virtual int calc_dmg_taken(int dmg) { return 0; }
    // Returns true if unit dies.
    public virtual int take_damage(int dmg) { return 0; }
    
    protected void move(Slot end) {
        slot.empty();
        end.fill(this);
        has_moved = true;
        end.get_group().validate_unit_order();
    }
    protected void create_attribute_list(int num_attributes) {
        for (int i = 0; i < num_attributes; i++) {
            attributes.Insert(i, false);
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
    public Unit get_unit() {
        return this;
    }

    // Check against game-rule limitations.
    public bool can_hit(Slot end) {
        //Debug.Log("attacking slot: " + slot);
        //Debug.Log("attacked slot: " + end);
        bool attacking_self = slot.get_unit().get_type() == end.get_unit().get_type();
        bool melee_vs_flying = slot.get_unit().combat_style == Unit.MELEE && 
                                end.get_unit().attributes[Unit.FLYING];
        bool out_of_range = !in_range(slot.get_unit().attack_range, slot.col, slot.row, end.col, end.row);
        
        if (attacking_self || melee_vs_flying || out_of_range || has_attacked) {
            Debug.Log("can't hit");
            return false;
        } 
        return true;
    }

    public bool in_range(int range, int x, int y, int x1, int y1) {
        int dx = Mathf.Abs(x - x1);
        int dy = Mathf.Abs(y - y1);
        return (dx + dy <= range) ? true : false;
    }


    private bool _has_attacked;
    public bool has_attacked {
        get { return _has_attacked; }
        set {
            _has_attacked = value;
            determine_acted();
        }
    }

    private bool _has_moved;
    public bool has_moved {
        get { return _has_moved; }
        set {
            _has_moved = value;
            determine_acted();
        }
    }

    private bool _has_acted;
    public bool has_acted {
        get { return _has_acted; }
        set {
            _has_acted = value;
            if (!_has_acted) {
                _has_attacked = false;
                _has_moved = false;

            }
        }
    }

    private void determine_acted() {
        if (has_attacked || has_moved) {
            _has_acted = true;
        } else 
            _has_acted = false;
    }

    public void reset_action() {
        has_acted = false;
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