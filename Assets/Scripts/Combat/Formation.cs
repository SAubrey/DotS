using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/* There is a single formation containing groups of 3 slots. Only the first slot performs game actions.
 */
public class Formation : MonoBehaviour {
    public static Formation I { get; private set; }
    // Organized by column, row
    private Dictionary<int, Dictionary<int, Group>> groups = 
        new Dictionary<int, Dictionary<int, Group>>();
    public List<Group> c0;// = new List<Group>();
    public List<Group> c1;// = new List<Group>();
    public List<Group> c2;// = new List<Group>();
    public List<Group> c3;// = new List<Group>();
    public List<Group> c4;// = new List<Group>();
    public List<Group> c5;// = new List<Group>();
    public List<Group> c6;// = new List<Group>();
    public List<Group> c7;// = new List<Group>();
    public List<Group> c8;// = new List<Group>();
    public List<Group> c9;// = new List<Group>();
    public List<Group> c10;// = new List<Group>();
    void Awake() {
        if (I == null) {
            I = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void add_groups() {
        List<List<Group>> gs = new List<List<Group>>() {
            c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10 };
        foreach (List<Group> col in gs) {
            foreach (Group g in col) {
                //Debug.Log("adding group at" + g.col + "," + g.row);
                add_group(g);
            }
        }
    }

    void Start() {
        add_groups();
        build_t1_battlefield();
    }

    // For groups to add themselves. 
    public void add_group(Group g) {
        //Debug.Log(g.col + "," + g.row);
        if (!groups.ContainsKey(g.col)) {
            groups.Add(g.col, new Dictionary<int, Group>());
        }
        if (!groups[g.col].ContainsKey(g.row)) {
            groups[g.col].Add(g.row, g);
        }
    }

    public Group get_group(int col, int row) {
        if (groups.ContainsKey(col)) {
            if (groups[col].ContainsKey(row))
                return groups[col][row];
        }
        Group g = find_group_by_coordinates(col, row);
        Debug.Log("Had to find group by coordinates" + col + "," + row);
        if (g == null) {
            Debug.Log("Group is null at " + col + "," + row);
            return null;
        }
        g.init();
        return g;
    }

    public List<Slot> get_highest_full_slots(int unit_type) {
        List<Slot> units = new List<Slot>();
        foreach (int col in groups.Keys) {
            foreach (Group group in groups[col].Values) {
                if (unit_type == Unit.ENEMY && group.has_enemy) {
                    units.Add(group.get_highest_enemy_slot());
                } else if (unit_type == Unit.PLAYER && group.has_punit) {
                    units.Add(group.get_highest_player_slot());
                }
            }
        }
        return units;
    }

    public List<Slot> get_all_full_slots(int unit_type) {
        List<Slot> units = new List<Slot>();
        foreach (int col in groups.Keys) {
            foreach (Group group in groups[col].Values) {
                if (unit_type == Unit.ENEMY && group.has_enemy) {
                    units.AddRange(group.get_full_slots());
                } else if (unit_type == Unit.PLAYER && group.has_punit) {
                    units.AddRange(group.get_full_slots());
                }
            }
        }
        return units;
    }

    public List<Group> get_all_nonempty_groups(int unit_type) {
        List<Group> gs = new List<Group>();
        foreach (int col in groups.Keys) {
            foreach (Group group in groups[col].Values) {
                if (unit_type == Unit.ENEMY && group.has_enemy) {
                    gs.Add(group);
                } else if (unit_type == Unit.PLAYER && group.has_punit) {
                    gs.Add(group);
                }
            }
        }
        return gs;
    }

    public void reset() {
        clear_battlefield();
    }

    public void clear_battlefield() {
        foreach (int col in groups.Keys) {
            foreach (int row in groups[col].Keys) {
                groups[col][row].empty();
            }
        }
        Selector.I.deselect();
        BatLoader.I.clear_placement_selection();
    }

    public void reset_groups_dir() {
        foreach (int col in groups.Keys) {
            foreach (int row in groups[col].Keys) {
                groups[col][row].reset_dir();
            }
        }
    }

    // Dynamic save - not to file.
    public void save_board(Battle b) {
        foreach (int col in groups.Keys) {
            foreach (int row in groups[col].Keys) {
                foreach (Slot slot in groups[col][row].slots) {
                    if (!slot.is_empty)
                        b.board.Add(new Location(col, row, slot.num), slot.get_unit());
                }
            }
        }
    }

    public void load_board(Battle b) {
        foreach (Location loc in b.board.Keys) {
            groups[loc.col][loc.row].get(loc.slot_num).fill(b.board[loc]);
        }
    }

    /*
    Rotate units within a group if the first unit has
    taken action in the battle phase.
    */
    public void rotate_actioned_player_groups() {
        foreach (int row in groups.Keys) {
            foreach (Group group in groups[row].Values) {
                if (group.is_empty)
                    continue;
                //if (group.get(0).get_unit().out_of_actions) {
                    //group.rotate_units();
                //}
            }
        }
    }

    private void build_t1_battlefield() {
        set_groups(4, 6, 5, 6, Group.PLAYER);
        set_groups(5, 5, 4, 4, Group.PLAYER);

        set_groups(2, 3, 4, 6, Group.ENEMY);
        set_groups(7, 8, 4, 6, Group.ENEMY);
        set_groups(4, 6, 2, 3, Group.ENEMY);
        set_groups(4, 6, 7, 8, Group.ENEMY);

        // set neutral
        set_groups(4, 4, 4, 4, Group.NEUTRAL);
        set_groups(6, 6, 4, 4, Group.NEUTRAL);
        // lower half
        set_groups(0, 3, 3, 3, Group.NEUTRAL);
        set_groups(7, 10, 3, 3, Group.NEUTRAL);
        set_groups(2, 3, 2, 2, Group.NEUTRAL);
        set_groups(7, 8, 2, 2, Group.NEUTRAL);
        set_groups(3, 3, 0, 1, Group.NEUTRAL);
        set_groups(7, 7, 0, 1, Group.NEUTRAL);
        // upper half        
        set_groups(0, 3, 7, 7, Group.NEUTRAL);
        set_groups(7, 10, 7, 7, Group.NEUTRAL);
        set_groups(2, 3, 8, 8, Group.NEUTRAL);
        set_groups(7, 8, 8, 8, Group.NEUTRAL);
        set_groups(3, 3, 9, 10, Group.NEUTRAL);
        set_groups(6, 6, 9, 10, Group.NEUTRAL);

        // set PERIPHERY
        set_groups(4, 6, 0, 1, Group.PERIPHERY);
        set_groups(4, 6, 9, 10, Group.PERIPHERY);
        set_groups(0, 1, 4, 6, Group.PERIPHERY);
        set_groups(9, 10, 4, 6, Group.PERIPHERY);
        // corners
        set_groups(0, 1, 2, 2, Group.PERIPHERY); //SW
        set_groups(1, 1, 1, 1, Group.PERIPHERY);
        set_groups(2, 2, 0, 1, Group.PERIPHERY);
        set_groups(9, 10, 2, 2, Group.PERIPHERY); // SE
        set_groups(9, 9, 1, 1, Group.PERIPHERY);
        set_groups(8, 8, 0, 1, Group.PERIPHERY);
        set_groups(0, 1, 8, 8, Group.PERIPHERY); // NW
        set_groups(1, 1, 9, 9, Group.PERIPHERY);
        set_groups(2, 2, 9, 10, Group.PERIPHERY);
        set_groups(8, 8, 9, 10, Group.PERIPHERY); // NE
        set_groups(9, 9, 9, 9, Group.PERIPHERY);
        set_groups(9, 10, 8, 8, Group.PERIPHERY);
    }

    private void build_t2_battlefield() {
        build_t1_battlefield();
        set_groups(4, 4, 4, 4, Group.PLAYER);
        set_groups(6, 6, 4, 4, Group.PLAYER);
    }

    private void build_t3_battlefield() {

    }

    // Set the tile zone type.
    public void set_groups(int colmin, int colmax, int rowmin, int rowmax, int type) {
        for (int c = colmin; c <= colmax; c++) {
            for (int r = rowmin; r <= rowmax; r++) {
                //Debug.Log(c + "," + r);
                get_group(c, r).set_type(type);

            }
        }
    }

    public Group find_group_by_coordinates(int c, int r) {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Group");
        foreach (GameObject g in gos) {
            Group group = g.GetComponentInChildren<Group>();
            if (group.col == c && group.row == r) {
                group.init();
                return group;
            }
        }
        return null;
    }
}

public class Location {
    public int col, row, slot_num;
    public Location(int col, int row, int slot_num) {
        this.col = col;
        this.row = row;
        this.slot_num = slot_num;
    }
}