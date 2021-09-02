using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory {
    // <equipment ID, List of equipment of that type>
    public Dictionary<string, List<Equipment>> equipment = new Dictionary<string, List<Equipment>>();
    public List<EquipmentSlot> equipment_slots = new List<EquipmentSlot>();
    public int[] slot_experience_requirements = new int[4] {
        50, 150, 300, 1000
    };
    public Discipline disc;

    public EquipmentInventory(Discipline disc) {
        this.disc = disc;
        for (int i = 0; i < slot_experience_requirements.Length; i++) {
            equipment_slots.Add(new EquipmentSlot(i));
        }
        add_to_inventory(Equipment.SHARPENED_BLADES, 2);
        add_to_inventory(Equipment.BOLSTERED_SHIELDS, 1);
    }

    public EquipmentSlot get_slot(int i) {
        return equipment_slots[i];
    }

    public int get_highest_unlocked_slot() {
        for (int i = slot_experience_requirements.Length - 1; i >= 0; i--) {
            if (disc.get_res(Discipline.EXPERIENCE) >= slot_experience_requirements[i]) {
                return i;
            }
        }
        return -1;
    }
    
    public string add_random_equipment(int tier) {
        int choice_index = Random.Range(0, Equipment.equipment[tier].Length);
        return (add_to_inventory(Equipment.equipment[tier][choice_index]));
    }

    public string add_to_inventory(string name, int amount=1) {
        Equipment e = Equipment.make_equipment(name);
        if (!equipment.ContainsKey(name))
            equipment.Add(name, new List<Equipment>());
        equipment[name].Add(e);
        disc.show_adjustment(name, amount);
        EquipmentUI.I.fill_dropdowns(this);
        return e == null ? "" : e.name;
    }

    public void equip(string name, int slot_num) {
        if (name == null || !equipment.ContainsKey(name))
            return;
        
        equipment_slots[slot_num].fill(equipment[name][0]);
    }

    public void unequip(int slot_num) {
        if (slot_num < 0 || slot_num > equipment_slots.Count)
            return;
        equipment_slots[slot_num].clear();
    }

    public int get_stat_boost_amount(int unit_ID, int stat_ID) {
        int sum = 0;
        foreach (EquipmentSlot es in equipment_slots) {
            if (!es.full)
                continue;
            if (!check_compatible_unit(unit_ID, es.equipment) 
                || !check_compatible_stat(stat_ID, es.equipment))
                continue;
            sum += es.equipment.affect_amount;
        }
        return sum;
    }

    private bool check_compatible_unit(int unit_ID, Equipment e) {
        foreach (int type in e.affected_unit_types) {
            if (unit_ID == type) {
                return true;
            }
        }
        return false;
    }

    private bool check_compatible_stat(int stat_ID, Equipment e) {
        foreach (int type in e.affected_stats) {
            if (stat_ID == type) {
                return true;
            }
        }
        return false;
    }

    public void remove_equipment(string name) {
        if (!equipment.ContainsKey(name)) {
            return;
        }
        equipment[name].RemoveAt(0);
        if (equipment[name].Count <= 0) {
            equipment.Remove(name);
        }
    }

    public int get_equipment_amount(string name) {
        if (!equipment.ContainsKey(name)) 
            return 0;
        return equipment[name].Count;
    }

    public bool has_equipped(string name) {
        foreach (EquipmentSlot es in equipment_slots) {
            if (!es.full)
                continue;
            if (es.equipment.name == name)
                return true;
        }
        return false;
    }

    public bool has(string name) {
        return equipment.ContainsKey(name);
    }

    public void remove_all_equipment() {
        equipment.Clear();
    }
}

public class EquipmentSlot {
    public Equipment equipment;
   // public bool locked { get; private set; } = false;
    public int num = 0;
    public EquipmentSlot(int num) {
        this.num = num;
    }

    public void fill(Equipment e) {
        equipment = e;
        e.equipped = true;
        e.activate();
    }

    public void clear() {
        if (full) {
            equipment.deactivate();
        }
        equipment.equipped = false;
        equipment = null;
    }

    public bool full { get => equipment != null; }
}
