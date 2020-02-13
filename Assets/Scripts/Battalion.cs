﻿using System.Collections.Generic;
using UnityEngine;

public class Battalion {

    public int ID;
    public Controller c;
    public IDictionary<int, List<PlayerUnit>> units = 
        new Dictionary<int, List<PlayerUnit>>() { };
    private List<PlayerUnit> dead_units = new List<PlayerUnit>();
    private List<PlayerUnit> injured_units = new List<PlayerUnit>();
    public int mine_qty;
    // Unit selected for placement in battle view
    private int selected_unit_type;
    public bool in_battle = false;
    public bool mini_retreating = false;
    
    public Battalion(Controller c, int ID) {
        this.c = c;
        this.ID = ID;

        foreach (int unit_type in PlayerUnit.unit_types) {
            units.Add(unit_type, new List<PlayerUnit>());
        }

        add_units(PlayerUnit.ARCHER, 3);
        add_units(PlayerUnit.WARRIOR, 2);
        add_units(PlayerUnit.SPEARMAN, 2);
        add_units(PlayerUnit.INSPIRATOR, 1);
        add_units(PlayerUnit.MINER, 1);
        
        if (ID == Controller.ENDURA) {
            mine_qty = 4;
        } else {
            mine_qty = 3;
        }
    }

    public void add_units(int type, int count) {
        //Debug.Log(type);
        foreach (int k in units.Keys) {
          //  Debug.Log(k);
           // Debug.Log(units[k]);
        }
        for (int i = 0; i < count; i++) {
            if (units.ContainsKey(type)) {
                units[type].Add(PlayerUnit.create_punit(type));
            }
        }
    }

    public void lose_random_unit() {
        int roll = Random.Range(0, units.Count);
        units[roll].RemoveAt(0);
    }

    public void reset_all_stage_actions() {
        foreach (int type in units.Keys) {
            for (int i = 0; i < units[type].Count; i++) {
                units[type][i].has_acted_in_stage = false;
            }
        }
    }

    public PlayerUnit get_unit(int ID) {
        for (int i = 0; i < units[ID].Count; i++) {
            if (units[ID][i] != null) 
                return units[ID][i];     
        }
        return null;
    }

    public PlayerUnit get_placeable_unit(int ID) {
        for (int i = 0; i < units[ID].Count; i++) {
            if (units[ID][i] != null && !units[ID][i].is_placed()) 
                return units[ID][i];    
        }
        return null;
    }

    public int count_placeable(int type=-1) {
        int i = 0;
        if (type >= 0) {
            foreach (PlayerUnit u in units[type]) {
                if (!u.is_placed() && !u.injured) 
                    i++;      
            }
        } else { // Count all units.
            for (int t = 0; t < units.Count; t++) {
                foreach (PlayerUnit u in units[t]) {
                    if (!u.is_placed() && !u.injured)
                        i++;
                }
            }
        }
        return i;
    }

    public int count_injured(int type=-1) {
        int i = 0;
        if (type >= 0) {
            foreach (PlayerUnit u in units[type]) {
            if (u.injured) 
                i++;      
            }
        } else { // Count all units.
            for (int t = 0; t < units.Count; t++) {
                foreach (PlayerUnit u in units[t]) {
                    if (u.injured)
                        i++;
                }
            }
        }
        return i;
    }

    
    public int count_healthy(int type=-1) {
        int i = 0;
        if (type >= 0) {
            foreach (PlayerUnit u in units[type]) {
                if (!u.injured) 
                    i++;      
            }
        } else { // Count all units.
            for (int t = 0; t < units.Count; t++) {
                foreach (PlayerUnit u in units[t]) {
                    if (!u.is_placed() && !u.injured)
                        i++;
                }
            }
        }
        return i;
    }

    public void add_dead_unit(PlayerUnit du) {
        if (!dead_units.Contains(du)) 
            dead_units.Add(du);
    }

    public void add_injured_unit(PlayerUnit iu) {
        if (!injured_units.Contains(iu)) 
            injured_units.Add(iu);
    }

    public void post_battle() {
        remove_expired_units();
        foreach (Slot s in c.formation.get_all_full_slots(Unit.PLAYER)) {
            s.get_unit().health = s.get_unit().max_health;
            s.update_healthbar();
            if (s.get_punit().defending)
                s.get_punit().defending = false;
        }
    }

    public void post_phase() {
        foreach (int type in units.Keys) {
            for (int i = 0; i < units[type].Count; i++) {
                units[type][i].post_phase();
            }
        }
    }
 
    private void remove_expired_units() {
        // Remove duplicates. Don't injure a dead unit.
        foreach (PlayerUnit du in dead_units) {
            foreach (PlayerUnit iu in injured_units) {
                if (du == iu)
                    injured_units.Remove(iu);
            }
        }
        remove_dead_units();
        remove_injured_units();
        validate_all_punits(); // validate *after* removing units.
    }

    private void remove_dead_units() {
        foreach (PlayerUnit du in dead_units) {
            du.get_slot().empty(false);
            units[du.get_ID()].Remove(du);
        }
        dead_units.Clear();
    }

    private void remove_injured_units() {
        foreach (PlayerUnit du in injured_units) {
            du.get_slot().empty(false);
        }
        injured_units.Clear();
    }

    
    private void validate_all_punits() {
        List<Group> punit_groups = c.formation.get_all_nonempty_groups(Unit.PLAYER);
        foreach (Group g in punit_groups) {
            g.validate_unit_order();
        }
    }

    public PlayerUnit get_selected_unit() {
        if (selected_unit_type >= 0 && selected_unit_type < PlayerUnit.EMPTY) {
            return get_placeable_unit(selected_unit_type);
        }
        return null;
    }

    public void set_selected_unit_type(int type) { 
        selected_unit_type = type;
    }

    public void clear_selected_unit_type() {
        selected_unit_type = PlayerUnit.EMPTY;
    }

    public int get_selected_unit_type() {
        return selected_unit_type;
    }

    public bool has_miner() {
        foreach (int punit in units.Keys) {
            if (punit == PlayerUnit.MINER) {
                return true;
            }
        }
        return false;
    }
}
