using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/* There is a single formation containing groups of 3 slots. Only the first slot performs game actions.
 */
public class Formation : MonoBehaviour {

    public GameObject slot_panel;
    private Controller c;

    public Sprite archer;
    public Sprite warrior;
    public Sprite spearman;
    public Sprite inspiritor;
    public Sprite miner;
    public Sprite empty;

    // Button images in battle scene for highlighting selections.
    public Dictionary<int, Sprite> images = new Dictionary<int, Sprite>();
    public Image archer_img;
    public Image warrior_img;
    public Image spearman_img;
    public Image inspiritor_img;
    public Image miner_img;

    private IDictionary<int, Image> unit_buttons = new Dictionary<int, Image>();

    // Organized by column, row
    private Dictionary<int, Dictionary<int, Group>> groups = new Dictionary<int, Dictionary<int, Group>>();

    // For saving/loading. Maintains which units are where.
    private Dictionary<Location, Unit> astra_board = new Dictionary<Location, Unit>();
    private Dictionary<Location, Unit> endura_board = new Dictionary<Location, Unit>();
    private Dictionary<Location, Unit> martial_board = new Dictionary<Location, Unit>();
    private Dictionary<string, Dictionary<Location, Unit>> discipline_boards = new Dictionary<string, Dictionary<Location, Unit>>();

    void Awake() {
        c = GameObject.Find("Controller").GetComponent<Controller>();

        discipline_boards.Add(Controller.ASTRA, astra_board);
        discipline_boards.Add(Controller.ENDURA, endura_board);
        discipline_boards.Add(Controller.MARTIAL, martial_board);

        images.Add(PlayerUnit.ARCHER, archer);
        images.Add(PlayerUnit.WARRIOR, warrior);
        images.Add(PlayerUnit.SPEARMAN, spearman);
        images.Add(PlayerUnit.INSPIRATOR, inspiritor);
        images.Add(PlayerUnit.MINER, miner);
        images.Add(PlayerUnit.EMPTY, empty);

        // Populate unit placement button images dictionary.
        unit_buttons.Add(PlayerUnit.ARCHER, archer_img);
        unit_buttons.Add(PlayerUnit.WARRIOR, warrior_img);
        unit_buttons.Add(PlayerUnit.SPEARMAN, spearman_img);
        unit_buttons.Add(PlayerUnit.INSPIRATOR, inspiritor_img);
        unit_buttons.Add(PlayerUnit.MINER, miner_img);
    }

    void Start() {
        build_t1_battlefield();
    }

    // For groups to add themselves. 
    public void add_group(Group g) {
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
        return null;
    }

    // Called by battalion selection buttons
    public void set_selected_unit(int ID) {
        
        // Reset color of currently selected unit if one exists.
        int su = c.get_active_bat().get_selected_unit_type(); 
        if (su >= 0 && su < PlayerUnit.EMPTY) {
            unit_buttons[su].color = Color.white;
        }
        
        // Select and change color of new selection
        c.get_active_bat().set_selected_unit_type(ID);
        unit_buttons[ID].color = new Color(.7f, .7f, .7f, 1);
    }

    public void clear_placement_selection() {
        foreach (int unit_type in unit_buttons.Keys) {
            unit_buttons[unit_type].color = Color.white;
        }
        c.get_active_bat().clear_selected_unit_type();
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


    public void clear_battlefield() {
        foreach (int col in groups.Keys) {
            foreach (int row in groups[col].Keys) {
                groups[col][row].empty();
            }
        }
        c.selector.selected_slot = null;
        clear_placement_selection();
    }

    public void reset_groups_dir() {
        foreach (int col in groups.Keys) {
            foreach (int row in groups[col].Keys) {
                groups[col][row].reset_dir();
            }
        }
    }

    public void save_board(string discipline) {
        Dictionary <Location, Unit> d = discipline_boards[discipline];

        foreach (int col in groups.Keys) {
            foreach (int row in groups[col].Keys) {
                foreach (Slot slot in groups[col][row].slots) {
                    if (!slot.is_empty) {
                        d.Add(new Location(col, row, slot.num), slot.get_unit());
                    }
                }
            }
        }
        if (d.Count > 0) {
            MapCell mc = c.tile_mapper.get_cell(c.get_disc().pos);
            mc.has_enemies = true;
        }
    }

    public void load_board(string discipline) {
        Dictionary <Location, Unit> cb = discipline_boards[discipline];
        foreach (Location loc in cb.Keys) {
            groups[loc.col][loc.row].get(loc.slot_num).fill(cb[loc]);
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
                    group.rotate_units();
                //}
            }
        }
    }

    public void build_t1_battlefield() {
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

    public void build_t2_battlefield() {
        build_t1_battlefield();
        set_groups(4, 4, 4, 4, Group.PLAYER);
        set_groups(6, 6, 4, 4, Group.PLAYER);
    }

    public void build_t3_battlefield() {

    }

    public void set_groups(int colmin, int colmax, int rowmin, int rowmax, int type) {
        for (int c = colmin; c <= colmax; c++) {
            for (int r = rowmin; r <= rowmax; r++) {
                get_group(c, r).set_type(type);
            }
        }
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