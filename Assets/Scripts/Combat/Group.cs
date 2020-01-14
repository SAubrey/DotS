﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

// Slot group
public class Group : MonoBehaviour {
    public const int UP = 0;
    public const int DOWN = 180;
    public const int LEFT = 90;
    public const int RIGHT = 270;
    public const int MAX = 3;
    
    // Group types. Used to limit unit placement.
    public const int NEUTRAL = 0; // Cannot place here.
    public const int PLAYER = 1; // player can only place here initially.
    public const int ENEMY = 2; // Only enemy can place here.
    public const int PERIPHERY = 3; // Player can place here phase 2+.
    
    public Color neutral_color;
    public Color player_color;
    public Color enemy_color;
    public Color periphery_color;
    public Color disabled_color;
    public int type;

    public int default_direction;
    private int direction;
    public int col, row;
    public Slot[] slots = new Slot[MAX];
    Controller c;
    private bool _disabled = false;
    Image img;

    void Awake() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        c.formation.add_group(this);
        img = GetComponent<Image>();
    }
    void Start() {
        direction = default_direction;
        c = GameObject.Find("Controller").GetComponent<Controller>();
        set_color(type);
    }

    // Moves units up within their group upon vacancies from unit death/movement.
    public void validate_unit_order() {
        if (is_empty)
            return;

        for (int i = 0; i < MAX; i++) {
            if (!slots[i].is_empty) 
                continue;
            for (int j = i + 1; j < MAX; j++) {
                if (slots[j].is_empty)
                    continue;
                // Move unit to higher slot
                slots[i].fill(slots[j].empty(false));
                break;
            }
        }
    }

    /*
    Rotate units clockwise at the end of a battle phase 
    if the first unit has expired their action. 
    
    A copy of the group is used to mutate the original list such that
    each non-empty slot will move to the first filled slot ahead of it,
     clockwise. 
    */
    public void rotate_units() {
        // Nothing to rotate with.
        if (is_empty || (get(1).is_empty && get(2).is_empty))
            return;

        // immutable units copy
        Unit[] us = new Unit[MAX];
        for (int i = 0; i < MAX; i++) 
            us[i] = get(i).get_unit();
        
        for (int i = 0; i < MAX; i++) {
            if (get(i).is_empty)
                continue;

            int place = (i + 1) % 3;
            if (get(i).is_empty) {
                place = (i + 2) % 3; 
            }
            set(i, us[place]);
        }
    }

    public void rotate(int direction) {
        this.direction = direction;
        transform.localEulerAngles = new Vector3(0, 0, direction);

        foreach (Slot s in slots)
            s.face_text_to_cam();
    }

    public void rotate_towards_target(Group target) {
        int dx = target.col - col;
        int dy = target.row - row;
        
        // Point in relation to the greater difference
        if (Math.Abs(dy) >= Math.Abs(dx)) {
            if (dy > 0)
                rotate(UP);
            else if (dy < 0)
                rotate(DOWN);
        } else {
            if (dx > 0)
                rotate(RIGHT);
            else if (dx < 0)
                rotate(LEFT);
        }
    }

    public void set_type(int type) {
        set_color(type);
        this.type = type;
    }

    // Toggles the group color if no parameter is set. 
    private void set_color(int type) {
        if (type == NEUTRAL)
            img.color = neutral_color;
        else if(type == PLAYER) {
            img.color = player_color;
        } else if (type == ENEMY)
            img.color = enemy_color;
        else if (type == PERIPHERY) 
            img.color = periphery_color;
    }

    public bool disabled {
        get { return _disabled; }
        set { 
            _disabled = value;
            img.enabled = !_disabled;
            foreach (Slot s in slots)
                s.disabled = _disabled;

        }
    }

    public void reset_dir() {
        rotate(default_direction);
    }

    public void add_slot(Slot slot) {
        slots[slot.num] = slot;
    }

    public void set(int i, Unit u) {
        slots[i].fill(u);
    }
    public Slot get(int i) {
        if (slots[i] != null)
            return slots[i];
        return null;
    }

    public Slot get_highest_empty_slot() {
        for (int i = 0; i < MAX; i++) 
            if (slots[i].is_empty)
                return slots[i];
        return null;
    }

    public Slot get_highest_enemy_slot() {
        for (int i = 0; i < MAX; i++) 
            if (slots[i].has_enemy)
                return slots[i];  
        return null;
    }

    public Slot get_highest_player_slot() {
        foreach (Slot s in slots) 
            if (s.has_punit)
                return s;  
        return null;
    }

    public List<Slot> get_full_slots() {
        List<Slot> full_slots = new List<Slot>();
        foreach (Slot s in slots) 
            if (s.has_unit)
                full_slots.Add(s);  
        return full_slots;
    }

    public bool has_punit {
        get {
            foreach (Slot slot in slots)
                if (slot.get_punit() != null)
                    return true;
            return false;
        }
    }

    public bool has_enemy {
        get {
            foreach (Slot slot in slots)
                if (slot.get_enemy() != null)
                    return true;
            return false;
        }
    }

    public void empty() {
        foreach (Slot s in slots) 
            s.empty(false);
    }

    public bool is_empty {
        get {
            foreach (Slot slot in slots)
                if (!slot.is_empty)
                    return false;
            return true;
        }
    }

    public bool faces(int direction) {
        return (direction == this.direction) ? true : false;
    }

    public int get_dir() {
        return direction;
    }
}