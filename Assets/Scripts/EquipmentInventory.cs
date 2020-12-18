using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory {
    public Dictionary<int, List<Equipment>> equipment = new Dictionary<int, List<Equipment>>();
    public EquipmentInventory() {
        equip(Equipment.SHARPENED_BLADES, 1);
    }
    
    public string add_random_equipment(int tier) {
        int choice_ID = Random.Range(0, Equipment.equipment[tier].Length);
        return (equip(choice_ID));
    }

    public string equip(int ID, int amount=1) {
        Equipment e = Equipment.make_equipment(ID);
        if (!equipment.ContainsKey(ID))
            equipment.Add(ID, new List<Equipment>());
        equipment[ID].Add(e);
        e.activate();
        return e.name;
    }

    public int get_stat_boost_amount(int unit_ID, int stat_ID) {
        int sum = 0;
        foreach (int eID in equipment.Keys) {
            Equipment e = equipment[eID][0];
            if (!check_compatible_unit(unit_ID, e) || !check_compatible_stat(stat_ID, e))
                continue;

            sum += e.affect_amount * equipment[eID].Count;
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

    public void remove_equipment(int ID) {
        if (!equipment.ContainsKey(ID)) {
            return;
        }
        equipment[ID].RemoveAt(0);
        if (equipment[ID].Count <= 0) {
            equipment.Remove(ID);
        }
    }

    public int get_equipment_amount(int ID) {
        if (!equipment.ContainsKey(ID)) 
            return 0;
        return equipment[ID].Count;
    }

    public void remove_all_equipment() {
        equipment.Clear();
    }
}
