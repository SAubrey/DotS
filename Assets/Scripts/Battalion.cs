using System.Collections.Generic;
using UnityEngine;

public class Battalion : MonoBehaviour {

    public string player;
    // Unit selected for placement in battle view
    private int selected_unit_type;
    public int mine_qty;
    private Controller c;
    private List<PlayerUnit> dead_units = new List<PlayerUnit>();
    private List<PlayerUnit> injured_units = new List<PlayerUnit>();
    public bool in_battle = false;
    
    public IDictionary<int, List<PlayerUnit>> units = new Dictionary<int, List<PlayerUnit>>() {
        {PlayerUnit.WARRIOR, new List<PlayerUnit>()},
        {PlayerUnit.SPEARMAN, new List<PlayerUnit>()},
        {PlayerUnit.ARCHER, new List<PlayerUnit>()},
        {PlayerUnit.MINER, new List<PlayerUnit>()},
        {PlayerUnit.INSPIRITOR, new List<PlayerUnit>()},
    };
    
    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        add_units(PlayerUnit.ARCHER, 3);
        add_units(PlayerUnit.WARRIOR, 2);
        add_units(PlayerUnit.SPEARMAN, 2);
        add_units(PlayerUnit.INSPIRITOR, 1);
        add_units(PlayerUnit.MINER, 1);

        if (player == Controller.ENDURA) {
            mine_qty = 4;
        } else {
            mine_qty = 3;
        }
    }

    public void reset_all_actions() {
        foreach (int type in units.Keys) {
            for (int i = 0; i < units[type].Count; i++) {
                units[type][i].reset_actions();
            }
        }
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

    public int count_placeable(int type) {
        int i = 0;
        foreach (PlayerUnit u in units[type]) {
            if (!u.is_placed() && !u.injured) 
                i++;      
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
        // Remove duplicates. Don't injure a dead unit.
        foreach (PlayerUnit du in dead_units) {
            foreach (PlayerUnit iu in injured_units) {
                if (du == iu)
                    injured_units.Remove(iu);
            }
        }
        remove_dead_units();
        remove_injured_units();
    }

    private void remove_dead_units() {
        foreach (PlayerUnit du in dead_units) {
            du.get_slot().empty();
            units[du.get_ID()].Remove(du);
        }
        dead_units.Clear();
    }

    private void remove_injured_units() {
        foreach (PlayerUnit du in injured_units) {
            //if ()
            du.get_slot().empty();
        }
        injured_units.Clear();
    }

    public void add_units(int type, int count) {
        for (int i = 0; i < count; i++) {
            units[type].Add(PlayerUnit.create_punit(type));
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
