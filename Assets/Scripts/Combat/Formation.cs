using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/* There is a single formation containing groups of 3 slots. Only the first slot performs game actions.
 */
public class Formation : MonoBehaviour {
    /*
    public const int FRONT1 = 3;
    public const int FRONT = 2;
    public const int MID = 1;
    public const int REAR = 0;
    public const int REAR1 = -1;
    public const int LEFT = 0;
    public const int RIGHT = 2;*/

    public GameObject slot_panel;
    private Controller c;

    public Sprite archer;
    public Sprite warrior;
    public Sprite spearman;
    public Sprite inspiritor;
    public Sprite miner;
    public Sprite empty;

    public Group front_left;
    public Group front_mid;
    public Group front_right;
    public Group mid_left;
    public Group mid_mid;
    public Group mid_right;
    public Group rear_mid;
    
    // Enemy spawn groups
    public Group rear1_mid;
    public Group front1_left;
    public Group front1_mid;
    public Group front1_right;


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
   /*     {FRONT1, new Dictionary<int, Group>() },
        {FRONT, new Dictionary<int, Group>() },
        {MID, new Dictionary<int, Group>() },
        {REAR, new Dictionary<int, Group>() },
        {REAR1, new Dictionary<int, Group>() },
    };*/

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
/*
        groups[FRONT1].Add(LEFT, front1_left);
        groups[FRONT1].Add(MID, front1_mid);
        groups[FRONT1].Add(RIGHT, front1_right);
        groups[REAR1].Add(MID, rear1_mid);

        groups[FRONT].Add(LEFT, front_left);
        groups[FRONT].Add(MID, front_mid);
        groups[FRONT].Add(RIGHT, front_right);
        groups[MID].Add(LEFT, mid_left);
        groups[MID].Add(MID, mid_mid);
        groups[MID].Add(RIGHT, mid_right);
        groups[REAR].Add(MID, rear_mid);*/

        images.Add(PlayerUnit.ARCHER, archer);
        images.Add(PlayerUnit.WARRIOR, warrior);
        images.Add(PlayerUnit.SPEARMAN, spearman);
        images.Add(PlayerUnit.INSPIRITOR, inspiritor);
        images.Add(PlayerUnit.MINER, miner);
        images.Add(PlayerUnit.EMPTY, empty);

        // Populate unit placement button images dictionary.
        unit_buttons.Add(PlayerUnit.ARCHER, archer_img);
        unit_buttons.Add(PlayerUnit.WARRIOR, warrior_img);
        unit_buttons.Add(PlayerUnit.SPEARMAN, spearman_img);
        unit_buttons.Add(PlayerUnit.INSPIRITOR, inspiritor_img);
        unit_buttons.Add(PlayerUnit.MINER, miner_img);
    }

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
                if (group.get(0).get_unit().has_acted) {
                    group.rotate_units();
                }
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