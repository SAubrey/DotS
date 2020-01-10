using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class TileMapper : MonoBehaviour {
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

    public const int PLAINS_2 = 12;
    public const int FOREST_2 = 13;
    public const int RUINS_2 = 14;
    public const int CLIFF_2 = 15;
    public const int CAVE_2 = 16;
    public const int STAR_2 = 17;
    public const int TITRUM_2 = 18;
    public const int LUSH_LAND_2 = 19;
    public const int MOUNTAIN_2 = 20;

    public TurnPhaser tp;
    public CamSwitcher cs;
    public Tilemap tm;
    
    public Tile plains_1;
    public Tile plains_2;
    public Tile forest_1;
    public Tile forest_2;
    public Tile ruins_1;
    public Tile ruins_2;
    public Tile cliff_1;
    public Tile cliff_2;
    public Tile cave_1;
    public Tile cave_2;
    public Tile star_1;
    public Tile star_2;
    public Tile titrum_1;
    public Tile titrum_2;
    public Tile mire;
    //public Tile mire_1;
    public Tile lush_land_2;
    public Tile mountain_2;
    
    public Tile settlement;
    public Tile rune_gate;

    public Tile city;
    public Tile shadow;
    private System.Random rand;
    public Controller c;

    private static Dictionary<int, Tile> tiles = new Dictionary<int, Tile>();

    public Dictionary<int, Dictionary<int, int>> bag_counters
     = new Dictionary<int, Dictionary<int, int>>();
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

    public Dictionary<int, List<int>> bags = new Dictionary<int, List<int>>();
    public List<int> t1_bag = new List<int>();
    public List<int> t2_bag = new List<int>();
    public List<int> t3_bag = new List<int>();
    
    public Dictionary<Pos, MapCell> map = new Dictionary<Pos, MapCell>();
    public Vector3 center_point;

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        tm = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        cs = c.cam_switcher;
        tp = c.turn_phaser;
        rand = new System.Random();

        bags.Add(1, t1_bag);
        bags.Add(2, t2_bag);
        bags.Add(3, t3_bag);
        bag_counters.Add(1, t1_bag_count);
        bag_counters.Add(2, t2_bag_count);
        bag_counters.Add(3, t3_bag_count);
        // Tier 1
        tiles.Add(PLAINS_1, plains_1);
        tiles.Add(FOREST_1, forest_1);
        tiles.Add(RUINS_1, ruins_1);
        tiles.Add(CLIFF_1, cliff_1);
        tiles.Add(CAVE_1, cave_1);
        tiles.Add(STAR_1, star_1);
        tiles.Add(TITRUM_1, titrum_1);
        tiles.Add(MIRE, mire);
        tiles.Add(SETTLEMENT, settlement);
        tiles.Add(RUNE_GATE, rune_gate);
        // Tier 2
        tiles.Add(PLAINS_2, plains_2);
        tiles.Add(FOREST_2, forest_2);
        tiles.Add(RUINS_2, ruins_2);
        tiles.Add(CLIFF_2, cliff_2);
        tiles.Add(CAVE_2, cave_2);
        tiles.Add(STAR_2, star_2);
        tiles.Add(TITRUM_2, titrum_2);
        tiles.Add(MOUNTAIN_2, mountain_2);
        tiles.Add(LUSH_LAND_2, lush_land_2);
        
        populate_decks();
        generate_t1(tm);
        generate_t2(tm);
        generate_t3(tm);
    }

    void Update() {
        if (cs.current_cam == CamSwitcher.MAP) {
            if (Input.GetMouseButtonDown(0)) {
                handle_left_click();
            }
        }
    }

    private void handle_left_click() {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (tp.moving) {
            if (move_player(pos)) {
                tp.advance_stage(); // Movement stage to action
            }
        }
    }

    public MapCell get_cell(int x, int y) {
        return map[new Pos(x, y)];
    }

    public MapCell get_cell(Vector3 pos) {
        return map[new Pos((int)pos.x, (int)pos.y)];
    }

    public List<Enemy> get_enemies(Vector3 pos) {
        return get_cell(pos).get_enemies();
    }
    
    public bool move_player(Vector3 pos) {
        int x = (int)pos.x;
        int y = (int)pos.y;
        if (get_tile(pos.x, pos.y) == null || 
                !check_adjacent(x, y, 
                (int)c.get_disc().pos.x, (int)c.get_disc().pos.y)) {
            return false;
        }
        
        MapCell cell = map[new Pos(x, y)];
        if (cell.discovered) {
            Debug.Log("was discovered");
        } else { // Not discovered, draw tile
            tm.SetTile(new Vector3Int(x, y, 0), null); // clear shadow
            tm.SetTile(new Vector3Int(x, y, 0), cell.tile);
        }
                 
        c.get_disc().pos = pos;
        return true;
    }

    public static bool check_adjacent(int x, int y, int x1, int y1) {
        int dx = Mathf.Abs(x - x1);
        int dy = Mathf.Abs(y - y1);
        return (dx + dy == 1) ? true : false;
    }

    // Randomly pick tiles from grab bags. 
    private Tile grab_tile(int tier) {
        if (bags[tier].Count > 0) {
            int index = rand.Next(bags[tier].Count);
            int tile_ID = bags[tier][index];

            bags[tier].RemoveAt(index);
            bag_counters[tier][tile_ID]--;
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

    public void set_tile_color(Tilemap tmap, int x, int y, Color color) {
        Vector3Int vec = new Vector3Int(x, y, 0);
        tmap.SetTileFlags(vec, TileFlags.None);
        tmap.SetColor(vec, color);
    }
/*
    public int check_tile_tier(int x, int y) {
        x = Math.Abs(x);
        y = Math.Abs(y);
        if (x <= 5 && y <= 5) {
            return 1;
        } else if (x <= 8 && y <= 8) {
            return 2;
        } else if (x > 8 && y > 8) {
            return 3;
        }
        return 0;
    }
 */
    public TileBase get_tile(float x, float y) {
        if (x >= 0 && y >= 0) 
            return tm.GetTile(new Vector3Int((int)x, (int)y, 0));
        return null;  
    }

    public void place_tile(Tilemap tmap, Tile type, int x, int y) {
        tmap.SetTile(new Vector3Int(x, y, 0), type);
    }

    void generate_t1(Tilemap tm) {
        for (int x = 8; x < 13; x++) {
            for (int y = 8; y < 13; y++) {
                if (x == 10 && y == 10) {
                    Pos pos = new Pos(x, y);
                    map.Add(pos, MapCell.create_cell(1, city, pos));
                    place_tile(tm, city, x, y);
                } else {
                    create_tile(1, x, y);
                }
            } 
        }
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

    void create_tile(int tier, int x, int y) {
        Pos pos = new Pos(x, y);
        Tile tile = grab_tile(tier);
        map.Add(pos, new MapCell(tier, tile, pos));
        place_tile(tm, shadow, pos.x, pos.y);
    }
}

