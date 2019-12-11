using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Slot group
public class Group {
    public Slot[] slots;
    public int max = 3;

    public Group() {
        slots = new Slot[max];
    }

       // Moves units up within their group upon vacancies from unit death/movement.
    public void validate_unit_order() {
        if (is_empty())
            return;

        for (int i = 0; i < slots.Length; i++) {
            if (!slots[i].is_empty()) 
                continue;
            for (int j = i + 1; j < slots.Length; j++) {
                if (slots[j].is_empty())
                    continue;
                // Move unit to higher slot
                Unit u = slots[j].get_unit();
                slots[j].empty_without_validation();
                slots[i].fill(u);
                break;
            }
        }
    }

    public void add_slot(Slot slot) {
        slots[slot.num] = slot;
    }

    public void clear_slots() {
        slots = new Slot[max];
    }

    public void set(int i, Slot unit) {
        slots[i] = unit;
    }
    public Slot get(int i) {
        if (slots[i] != null) {
            return slots[i];
        }
        return null;
    }

    public Slot get_highest_empty_slot() {
        for (int i = 0; i < max; i++) {
            if (slots[i].is_empty()) {
                return slots[i];
            }
        }
        return null;
    }

    public Slot get_highest_enemy_slot() {
        for (int i = 0; i < max; i++) {
            if (slots[i].has_enemy()) {
                return slots[i];
            }
        }
        return null;
    }

    public Slot get_highest_player_slot() {
        foreach (Slot s in slots) {
            if (s.has_punit())
                return s;
        }
        return null;
    }

    public List<Slot> get_full_slots() {
        List<Slot> full_slots = new List<Slot>();
        foreach (Slot s in slots) {
            if (s.has_unit())
                full_slots.Add(s);
        }   
        return full_slots;
    }

    public bool has_punit() {
        foreach (Slot slot in slots) {
            if (slot.get_punit() != null)
                return true;
        }
        return false;
    }

    public bool has_enemy() {
        foreach (Slot slot in slots) {
            if (slot.get_enemy() != null)
                return true;
        }
        return false;
    }

    public bool is_empty() {
        foreach (Slot slot in slots) {
            if (!slot.is_empty())
                return false;
        }
        return true;
    }

}
