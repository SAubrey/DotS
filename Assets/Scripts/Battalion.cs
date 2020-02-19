using System.Collections.Generic;
using UnityEngine;

public class Battalion {

    public Controller c;
    public IDictionary<int, List<PlayerUnit>> units = 
        new Dictionary<int, List<PlayerUnit>>() { };
    private List<PlayerUnit> dead_units = new List<PlayerUnit>();
    private List<PlayerUnit> injured_units = new List<PlayerUnit>();
    public int mine_qty;
    // Unit selected for placement in battle view
    public bool in_battle = false;
    public bool mini_retreating = false;
    public int ID;
    
    public Battalion(Controller c, int ID) {
        this.c = c;
        this.ID = ID;

        foreach (int unit_type in PlayerUnit.unit_types) 
            units.Add(unit_type, new List<PlayerUnit>());

        add_units(PlayerUnit.ARCHER, 3);
        add_units(PlayerUnit.WARRIOR, 2);
        add_units(PlayerUnit.SPEARMAN, 2);
        add_units(PlayerUnit.INSPIRATOR, 1);
        add_units(PlayerUnit.MINER, 1);

        // Units for testing
        add_units(PlayerUnit.MENDER, 1);
        
        mine_qty = ID == Controller.ENDURA ? 4 : 3;
    }

    public void add_units(int type, int count) {
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
        List<PlayerUnit> punits;
        units.TryGetValue(ID, out punits);
        if (punits == null)
            return null;

        for (int i = 0; i < punits.Count; i++) {
            if (punits[i] != null) 
                return punits[i];     
        }
        return null;
    }

    public PlayerUnit get_placeable_unit(int ID) {
        List<PlayerUnit> punits;
        units.TryGetValue(ID, out punits);
        if (punits == null)
            return null;

        for (int i = 0; i < punits.Count; i++) {
            PlayerUnit pu = punits[i];
            if (pu != null && !pu.is_placed && !pu.injured) 
                return pu;    
        }
        return null;
    }

    public int count_placeable(int type=-1) {
        int i = 0;
        if (type >= 0) {
            foreach (PlayerUnit u in units[type]) {
                if (!u.is_placed && !u.injured) 
                    i++;      
            }
        } else { // Count all units.
            for (int t = 0; t < units.Count; t++) {
                foreach (PlayerUnit u in units[t]) {
                    if (!u.is_placed && !u.injured)
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
                    if (!u.is_placed && !u.injured)
                        i++;
                }
            }
        }
        return i;
    }

    public bool heal_injured_unit(int ID) {
        foreach (PlayerUnit pu in units[ID]) {
            if (pu.injured) {
                pu.injured = false;
                return true;
            }
        }
        return false;
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
        foreach (PlayerUnit pu in get_all_placed_units()) {
            Slot s = pu.get_slot();
            s.get_unit().health = s.get_unit().get_boosted_max_health();
            if (s.get_punit().defending)
                s.get_punit().defending = false;
            s.update_UI();
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

    public bool has_miner { 
        get {
            foreach (int punit in units.Keys) {
                if (punit == PlayerUnit.MINER)
                    return true;
            }
            return false;
        }
    }

    public bool has_seeker { 
        get {
            foreach (int punit in units.Keys) {
                if (punit == PlayerUnit.SEEKER) 
                    return true;
            }
            return false;
        }
    }

    public List<PlayerUnit> get_all_placed_units() {
        List<PlayerUnit> punits = new List<PlayerUnit>();
        foreach (List<PlayerUnit> type_list in units.Values) {
            foreach (PlayerUnit pu in type_list) {
                if (pu.is_placed)
                    punits.Add(pu);
            }
        }
        return punits;
    }
}
