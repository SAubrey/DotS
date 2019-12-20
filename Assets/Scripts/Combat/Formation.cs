using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/* There is a single formation containing groups of 3 slots. Only the first slot performs game actions.
 */
public class Formation : MonoBehaviour {
    public const int FRONT1 = 3;
    public const int FRONT = 2;
    public const int MID = 1;
    public const int REAR = 0;
    public const int REAR1 = -1;
    public const int LEFT = 0;
    public const int RIGHT = 2;

    public GameObject slot_panel;
    private Controller c;

    public Sprite archer;
    public Sprite warrior;
    public Sprite spearman;
    public Sprite inspiritor;
    public Sprite miner;
    public Sprite empty;

    private Group front_left;
    private Group front_mid;
    private Group front_right;
    private Group mid_left;
    private Group mid_mid;
    private Group mid_right;
    private Group rear_mid;
    
    // Enemy spawn groups
    private Group rear1_mid;
    private Group front1_left;
    private Group front1_mid;
    private Group front1_right;


    // Button images in battle scene for highlighting selections.
    public Dictionary<int, Sprite> images = new Dictionary<int, Sprite>();
    public Image archer_img;
    public Image warrior_img;
    public Image spearman_img;
    public Image inspiritor_img;
    public Image miner_img;

    private IDictionary<int, Image> unit_buttons = new Dictionary<int, Image>();

    // organized by row, column
    private Dictionary<int, Dictionary<int, Group>> groups = new Dictionary<int, Dictionary<int, Group>>() {
        {FRONT1, new Dictionary<int, Group>() },
        {FRONT, new Dictionary<int, Group>() },
        {MID, new Dictionary<int, Group>() },
        {REAR, new Dictionary<int, Group>() },
        {REAR1, new Dictionary<int, Group>() },
    };


    private Dictionary<Location, Unit> astra_board = new Dictionary<Location, Unit>();
    private Dictionary<Location, Unit> endura_board = new Dictionary<Location, Unit>();
    private Dictionary<Location, Unit> martial_board = new Dictionary<Location, Unit>();
    private Dictionary<string, Dictionary<Location, Unit>> culture_boards = new Dictionary<string, Dictionary<Location, Unit>>();

    void Awake() {
        c = GameObject.Find("Controller").GetComponent<Controller>();

        culture_boards.Add(Controller.ASTRA, astra_board);
        culture_boards.Add(Controller.ENDURA, endura_board);
        culture_boards.Add(Controller.MARTIAL, martial_board);
        
        front1_left = new Group();
        front1_mid = new Group();
        front1_right = new Group();
        front_left = new Group();
        front_mid = new Group();
        front_right = new Group();
        mid_left = new Group();
        mid_mid = new Group();
        mid_right = new Group();
        rear_mid = new Group();
        rear1_mid = new Group();

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
        groups[REAR].Add(MID, rear_mid);

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

        add_slots_to_groups();
    }

    private void add_slots_to_groups() {
        Component[] slots = slot_panel.GetComponentsInChildren<Slot>();
        foreach (Slot s in slots) {
            //s.add_to_group();
            s.set_group(groups[s.row][s.col]);
            //add_slot_to_group(s);
        }
    }

    public Group get_group(int row, int col) {
        if (groups.ContainsKey(row)) {
            if (groups[row].ContainsKey(col))
                return groups[row][col];
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
        foreach (int row in groups.Keys) {
            foreach (Group group in groups[row].Values) {
                if (unit_type == Unit.ENEMY && group.has_enemy()) {
                    units.Add(group.get_highest_enemy_slot());
                } else if (unit_type == Unit.PLAYER && group.has_punit()) {
                    units.Add(group.get_highest_player_slot());
                }
            }
        }
        return units;
    }

    public List<Slot> get_all_full_slots(int unit_type) {
        List<Slot> units = new List<Slot>();
        foreach (int row in groups.Keys) {
            foreach (Group group in groups[row].Values) {
                if (unit_type == Unit.ENEMY && group.has_enemy()) {
                    units.AddRange(group.get_full_slots());
                } else if (unit_type == Unit.PLAYER && group.has_punit()) {
                    units.AddRange(group.get_full_slots());
                }
            }
        }
        return units;
    }

    public void reset() {
        Debug.Log("resetting slots in Formation");
        foreach (int row in groups.Keys) {
            foreach (int col in groups[row].Keys) {
                foreach (Slot s in groups[row][col].slots)
                    s.empty_without_validation();
            }
        }
        c.selector.selected_slot = null;
        clear_placement_selection();
    }

    public void save_board(string culture) {
        Dictionary <Location, Unit> d = culture_boards[culture];

        foreach (int row in groups.Keys) {
            foreach (int col in groups[row].Keys) {
                foreach (Slot slot in groups[row][col].slots) {
                    if (!slot.is_empty()) {
                        d.Add(new Location(row, col, slot.num), slot.get_unit());
                    }
                }
            }
        }
    }

    public void load_board(string culture) {
        Dictionary <Location, Unit> cb = culture_boards[culture];
        foreach (Location loc in cb.Keys) {
            groups[loc.row][loc.col].get(loc.slot_num).fill(cb[loc]);
        }
    }
}

public class Location {
    public int col, row, slot_num;
    public Location(int row, int col, int slot_num) {
        this.row = row;
        this.col = col;
        this.slot_num = slot_num;
    }
}