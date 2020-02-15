using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.EventSystems;

public class TileMapper : MonoBehaviour, ISaveLoad {
    public const int PLAINS_1 = 0;
    public const int FOREST_1 = 1;
    public const int RUINS_1 = 2;
    public const int CLIFF_1 = 3;
    public const int CAVE_1 = 4;
    public const int STAR_1 = 5;
    public const int TITRUM_1 = 6;
    public const int MIRE = 7;
    public const int SETTLEMENT = 8;
    public const int RUNE_GATE = 9;
    public const int CITY = 10;

    public const int PLAINS_2 = 11;
    public const int FOREST_2 = 12;
    public const int RUINS_2 = 13;
    public const int CLIFF_2 = 14;
    public const int CAVE_2 = 15;
    public const int STAR_2 = 16;
    public const int TITRUM_2 = 17;
    public const int LUSH_LAND_2 = 18;
    public const int MOUNTAIN_2 = 19;

    public TurnPhaser tp;
    public CamSwitcher cs;
    public Tilemap tm;
    
    public Tile plains_1, plains_2;
    public Tile forest_1, forest_2;
    public Tile ruins_1, ruins_2;
    public Tile cliff_1, cliff_2;
    public Tile cave_1, cave_2;
    public Tile star_1, star_2;
    public Tile titrum_1, titrum_2;
    public Tile mire;
    //public Tile mire_1;
    public Tile lush_land_2;
    public Tile mountain_2;
    
    public Tile settlement;
    public Tile rune_gate;

    public Tile city;
    public Tile shadow;
    private System.Random rand;
    private Controller c;

    private static IDictionary<int, Tile> tiles = new Dictionary<int, Tile>();
    private static IDictionary<Tile, int> tile_to_tileID = new Dictionary<Tile, int>();

    private Dictionary<int, Dictionary<int, int>> bag_counters
     = new Dictionary<int, Dictionary<int, int>>();
    public Dictionary<int, List<int>> bags = new Dictionary<int, List<int>>();
    private Dictionary<int, int> t1_bag_count = new Dictionary<int, int>() {
        {PLAINS_1, 6},
        {FOREST_1, 6},
        {RUINS_1, 4},
        {CLIFF_1, 1},
        {CAVE_1, 1},
        {STAR_1, 4},
        {TITRUM_1, 2},
    };
    private Dictionary<int, int> t2_bag_count = new Dictionary<int, int>() {
        {PLAINS_2, 14},
        {FOREST_2, 14},
        {RUINS_2, 12},
        {CLIFF_2, 2},
        {CAVE_2, 6},
        {STAR_2, 7},
        {TITRUM_2, 8},
        {MIRE, 10},
        {SETTLEMENT, 2},
        {LUSH_LAND_2, 5},
        {MOUNTAIN_2, 14},
        {RUNE_GATE, 2},
    };  
    private Dictionary<int, int> t3_bag_count = new Dictionary<int, int>() {
        {PLAINS_2, 212},
    };
    
    public Dictionary<Pos, MapCell> map = new Dictionary<Pos, MapCell>();
    public bool waiting_for_second_gate { get; set; } = false;
    public bool scouting { get; set; } = false;

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        tm = GameObject.Find("MapTilemap").GetComponent<Tilemap>();
        cs = c.cam_switcher;
        tp = c.turn_phaser;
        rand = new System.Random();

        bags.Add(1, new List<int>() );
        bags.Add(2, new List<int>() );
        bags.Add(3, new List<int>() );
        bag_counters.Add(1, t1_bag_count);
        bag_counters.Add(2, t2_bag_count);
        bag_counters.Add(3, t3_bag_count);
        // Tier 1
        tiles.Add(CITY, city);
        tiles.Add(PLAINS_1, plains_1);
        tiles.Add(FOREST_1, forest_1);
        tiles.Add(RUINS_1, ruins_1);
        tiles.Add(CLIFF_1, cliff_1);
        tiles.Add(CAVE_1, cave_1);
        tiles.Add(STAR_1, star_1);
        tiles.Add(TITRUM_1, titrum_1);
        tiles.Add(SETTLEMENT, settlement);
        // Tier 2
        tiles.Add(RUNE_GATE, rune_gate);
        tiles.Add(MIRE, mire);
        tiles.Add(PLAINS_2, plains_2);
        tiles.Add(FOREST_2, forest_2);
        tiles.Add(RUINS_2, ruins_2);
        tiles.Add(CLIFF_2, cliff_2);
        tiles.Add(CAVE_2, cave_2);
        tiles.Add(STAR_2, star_2);
        tiles.Add(TITRUM_2, titrum_2);
        tiles.Add(MOUNTAIN_2, mountain_2);
        tiles.Add(LUSH_LAND_2, lush_land_2);

        // Populate inverse dictionary.
        foreach (KeyValuePair<int, Tile> pair in tiles) {
            if (!tile_to_tileID.ContainsKey(pair.Value))
                tile_to_tileID.Add(pair.Value, pair.Key);
        }
    }

    public void init(bool from_save) {
        if (!from_save) {
            new_game();
        }
    }

    void Update() {
        if (cs.current_cam == CamSwitcher.MAP) {
            if (Input.GetMouseButtonDown(0)) {
                if (!EventSystem.current.IsPointerOverGameObject()) 
                    handle_left_click();
            }
        }
    }

    private void new_game() {
        populate_decks();
        generate_t1(tm);
        generate_t2(tm);
        generate_t3(tm);
    }

    private void clear_data() {
        bags[1].Clear();
        bags[2].Clear();
        bags[3].Clear();
        map.Clear();
    }

    private void handle_left_click() {
        if (tp.moving) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (get_tile(pos.x, pos.y) == null)
                return;
            if (scouting) {
                if (scout(pos)) {
                    scouting = false;
                    c.map_ui.set_active_scoutB(false);
                    c.turn_phaser.advance_stage();
                }
            }
            else if (move_player(pos)) {
                tp.advance_stage(); // Movement stage to action
            }
        }
    }

    public bool move_player(Vector3 pos) {
        Discipline d = c.get_disc();
        if (get_tile(pos.x, pos.y) == null || 
            !(check_adjacent(pos, d.pos) || waiting_for_second_gate)) 
            return false;

        int x = (int)pos.x;
        int y = (int)pos.y;
        MapCell cell = map[new Pos(x, y)];
        if (cell.discovered) {
            if (cell.has_rune_gate) {
                if (waiting_for_second_gate) {
                    // move to gate
                    waiting_for_second_gate = false;
                    c.map_ui.set_active_rune_gateB(false);
                } else {
                    c.map_ui.set_active_rune_gateB(true);
                }
            }
        } else { // Not discovered, draw tile
            tm.SetTile(new Vector3Int(x, y, 0), cell.tile);
        }
        c.get_disc().pos = pos;
        c.map_ui.update_cell_text(cell.name);
        c.get_active_bat().in_battle = cell.has_enemies; // Only if enemies have already spawned.

        Debug.Log("setting in_battle to " + c.get_active_bat().in_battle + 
        "for " + c.active_disc_ID
            + " at " + x + ", " + y);
        return true;
    }

    private bool scout(Vector3 pos) {
        if (get_tile(pos.x, pos.y) == null || !check_adjacent(pos, c.get_disc().pos)) 
            return false;
        MapCell cell = map[new Pos((int)pos.x, (int)pos.y)];
        tm.SetTile(new Vector3Int((int)pos.x, (int)pos.y, 0), cell.tile);
        return true;
    }

    public static bool check_adjacent(int x, int y, int x1, int y1) {
        int dx = Mathf.Abs(x - x1);
        int dy = Mathf.Abs(y - y1);
        return dx + dy == 1;
    }

    public static bool check_adjacent(Vector3 pos1, Vector3 pos2) {
        int dx = Mathf.Abs((int)pos1.x - (int)pos2.x);
        int dy = Mathf.Abs((int)pos1.y - (int)pos2.y);
        return dx + dy == 1;
    }

    // Randomly pick tiles from grab bags. 
    private Tile grab_tile(int tier) {
        if (bags[tier].Count > 0) {
            int index = rand.Next(bags[tier].Count);
            int tile_ID = bags[tier][index];

            bags[tier].RemoveAt(index);
            return tiles[tile_ID];
        }
        return null;
    }

    private void populate_decks() {
        for (int tier = 1; tier <= 3; tier++) { // For each tier
            foreach (int tile_type_ID in bag_counters[tier].Keys) {
                for (int i = 0; i < bag_counters[tier][tile_type_ID]; i++) {
                    bags[tier].Add(tile_type_ID);
                }
            }
        }
    }

    public void create_tile(int tier, int x, int y) {
        Pos pos = new Pos(x, y);
        Tile tile = grab_tile(tier);
        map.Add(pos, MapCell.create_cell(
            tier, tile_to_tileID[tile], tile, pos));
        place_tile(shadow, pos.x, pos.y);
    }

    public TileBase get_tile(float x, float y) {
        if (x >= 0 && y >= 0) 
            return tm.GetTile(new Vector3Int((int)x, (int)y, 0));
        return null;  
    }

    public void place_tile(Tile tile, int x, int y) {
        tm.SetTile(new Vector3Int(x, y, 0), tile);
    }

    public bool is_at_city(Discipline disc) {
        return ((int)disc.pos.x == 10 && (int)disc.pos.y == 10);
    }

    public GameData save() {
        return new MapData(this, Controller.TILE_MAPPER);
    }
 
    public void load(GameData generic) {
        MapData data = generic as MapData;
        clear_data();

        foreach (int num in data.t1_bag)
            bags[1].Add(num);
        foreach (int num in data.t2_bag)
            bags[2].Add(num);
        foreach (int num in data.t3_bag)
            bags[3].Add(num);
        
        // Recreate map.
        foreach (SMapCell mcs in data.cells) {
            Pos pos = new Pos(mcs.x, mcs.y);
            MapCell cell = MapCell.create_cell(
                mcs.tier, mcs.tile_type, tiles[mcs.tile_type], pos);
            cell.minerals = mcs.minerals;
            cell.star_crystals = mcs.star_crystals;
            cell.discovered = mcs.discovered;

            if (cell.discovered)
                place_tile(tiles[mcs.tile_type], mcs.x, mcs.y);
            else
                place_tile(shadow, pos.x, pos.y);
            map.Add(pos, cell);
        }
    }

    public MapCell get_cell(Vector3 pos) {
        return map[new Pos((int)pos.x, (int)pos.y)];
    }

    public List<Enemy> get_enemies(Vector3 pos) {
        return get_cell(pos).get_enemies();
    }
    
    public void build_rune_gate(Pos pos) {
        MapCell mc = map[pos];
        mc.has_rune_gate = true;
        c.map_ui.set_active_rune_gateB(true);
    }

    void generate_t1(Tilemap tm) {
        for (int x = 8; x < 13; x++) {
            for (int y = 8; y < 13; y++) {
                if (x == 10 && y == 10) {
                    Pos pos = new Pos(x, y);
                    MapCell mc = MapCell.create_cell(1, CITY, city, pos);
                    mc.name = "City";
                    map.Add(pos, mc);
                    place_tile(city, x, y);
                } else {
                    create_tile(1, x, y);
                }
            } 
        }
        map[new Pos(10, 10)].discover(); // discover the city.
    }

    void generate_t2(Tilemap tm) {
        // t2 origin is 5, 5
        // Horizontal bars
        for (int x = 5; x < 16; x++) {
            for (int y = 5; y < 8; y++) {
                create_tile(2, x, y);
                create_tile(2, x, y + 8);
            }
        }

        // Vertical bars
        for (int x = 5; x < 8; x++) {
            for (int y = 8; y < 13; y++) {
                create_tile(2, x, y);
                create_tile(2, x + 8, y);
            }
        }
    }

    void generate_t3(Tilemap tm) {
        // (9 wide 3 deep over 2 wide band)
        // Horizontal protrusion
        for (int x = 6; x < 15; x++) {
            for (int y = 0; y < 3; y++) {
                create_tile(3, x, y);
                create_tile(3, x, y + 18);
            }
        }
        // Vertical protrusion
        for (int x = 0; x < 3; x++) {
            for (int y = 6; y < 15; y++) {
                create_tile(3, x, y);
                create_tile(3, x + 18, y);
            }
        }
        // Horizontal bar
        for (int x = 3; x < 18; x++) {
            for (int y = 3; y < 5; y++) {
                create_tile(3, x, y);
                create_tile(3, x, y + 13);
            }
        }
        // Vertical bar
        for (int x = 3; x < 5; x++) {
            for (int y = 5; y < 16; y++) {
                create_tile(3, x, y);
                create_tile(3, x + 13, y);
            }
        }
    }

    public void toggle_waiting_for_second_gate() {
        waiting_for_second_gate = !waiting_for_second_gate;
    }

    public void toggle_scouting() {
        scouting = !scouting;
    }
}