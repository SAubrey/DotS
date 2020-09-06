using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class Map : MonoBehaviour, ISaveLoad {
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
    public Controller c;
    public GameObject cell_UI_prefab;
    public MapCellUI open_cell_UI_script;

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
    private Dictionary<int, int> t2_bag_count = new Dictionary<int, int>() { // 96
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
    private Dictionary<int, int> t3_bag_count = new Dictionary<int, int>() { // 212
        {PLAINS_2, 38},
        {FOREST_2, 38},
        {RUINS_2, 24},
        {CLIFF_2, 4},
        {CAVE_2, 12},
        {STAR_2, 14},
        {TITRUM_2, 16},
        {MIRE, 20},
        {SETTLEMENT, 4},
        {LUSH_LAND_2, 10},
        {MOUNTAIN_2, 28},
        {RUNE_GATE, 4},
    };
    
    public Dictionary<Pos, MapCell> map = new Dictionary<Pos, MapCell>();
    public bool waiting_for_second_gate { get; set; } = false;
    public bool scouting { get; set; } = false;

    public GraphicRaycaster graphic_raycaster;

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
                PointerEventData m_PointerEventData = new PointerEventData(EventSystem.current);
                m_PointerEventData.position = Input.mousePosition;
                List<RaycastResult> objects = new List<RaycastResult>();

                graphic_raycaster.Raycast(m_PointerEventData, objects);
                Debug.Log(objects.Count);
                // Close the open cell window if clicking anywhere other than on the window.
                // (The tilemap does not register as a game object)
                bool hit_cell_window = false;
                foreach (RaycastResult o in objects) {
                    if (o.gameObject.tag == "Cell Window") {
                        hit_cell_window = true;
                        continue;
                    }
                }
                Debug.Log(hit_cell_window);
                if (!hit_cell_window) {
                    if (open_cell_UI_script)
                        open_cell_UI_script.close();
                    if (objects.Count <= 0)
                        handle_left_click();
                }
            }
        }
    }

    private void new_game() {
        clear_data();
        scouting = false;
        waiting_for_second_gate = false;
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
        if (open_cell_UI_script)
            open_cell_UI_script.close();
        open_cell_UI_script = null;
    }

    private void handle_left_click() {
        if (tp.moving) {
            Vector3 pos = c.cam_switcher.mapCam.ScreenToWorldPoint(Input.mousePosition);

            if (get_tile(pos.x, pos.y) == null)
                return;

            generate_cell_UI(get_cell(pos));
        }
    }

    private void generate_cell_UI(MapCell cell) {
        GameObject cell_UI = Instantiate(cell_UI_prefab, GameObject.Find("MapUICanvas").transform);
        MapCellUI cell_UI_script = cell_UI.GetComponentInChildren<MapCellUI>();
        cell_UI_script.init(this, cell);
        open_cell_UI_script = cell_UI_script;
    }

    public void move_player(Vector3 pos, bool advance_stage) {
        MapCell cell = get_cell(pos);
        c.get_disc().pos = pos;
        c.map_ui.update_cell_text(cell.name);

        if (!cell.discovered) {
            tm.SetTile(new Vector3Int((int)pos.x, (int)pos.y, 0), cell.tile);
        }
        
        if ((cell.biome_ID == MapCell.CAVE_ID || cell.biome_ID == MapCell.RUINS_ID) 
                && !cell.travelcard_complete) {
            c.map_ui.set_active_ask_to_enterP(true);
            return;        
        }
        if (advance_stage)
            tp.advance_stage();
    }

    public bool can_move(Vector3 destination) {
        Vector3 current_pos = new Vector3(get_current_cell().pos.x, get_current_cell().pos.y, 0);
        return check_adjacent(destination, current_pos);
    }

    public void scout(Vector3 pos) {
        MapCell cell = map[new Pos((int)pos.x, (int)pos.y)];
        tm.SetTile(new Vector3Int((int)pos.x, (int)pos.y, 0), cell.tile);
        tp.advance_stage();
    }

    public bool can_scout(Vector3 pos) {
        return get_tile(pos.x, pos.y) != null && check_adjacent(pos, c.get_disc().pos) && 
            c.get_active_bat().get_unit(PlayerUnit.SCOUT) != null &&
            get_current_cell() != get_cell(pos) && get_cell(pos) != get_city_cell();
    }

    public bool can_teleport(Vector3 pos) {
        MapCell cell = get_cell(pos);
        return cell.restored_rune_gate && get_current_cell().restored_rune_gate &&
            get_current_cell() != cell && cell != get_city_cell();
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

    public void create_cell(int tier, int x, int y) {
        Pos pos = new Pos(x, y);
        Tile tile = grab_tile(tier);
        map.Add(pos, MapCell.create_cell(this,
            tier, tile_to_tileID[tile], tile, pos));
        tm.SetTileFlags(new Vector3Int(y, x, 0), TileFlags.None);
        tile.color = Color.white;
        place_tile(shadow, pos.x, pos.y);
    }

    public Tile get_tile(float x, float y) {
        if (x >= 0 && y >= 0) 
            return tm.GetTile<Tile>(new Vector3Int((int)x, (int)y, 0));
        return null;  
    }

    public MapCell get_current_cell(Discipline disc=null) {
        return disc == null ? get_cell(c.get_disc().pos) : get_cell(disc.pos);
    }

    public void place_tile(Tile tile, int x, int y) {
        tm.SetTile(new Vector3Int(x, y, 0), tile);
    }

    public bool is_at_city(Discipline disc) {
        return ((int)disc.pos.x == 10 && (int)disc.pos.y == 10);
    }

    public MapCell get_city_cell() {
        return map[new Pos(10, 10)];
    }

    public GameData save() {
        return new MapData(this, Controller.MAP);
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
            MapCell cell = MapCell.create_cell(this,
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

    public List<Enemy> get_enemies_here() {
        return get_cell(c.get_disc().pos).get_enemies();
    }
    
    public void build_rune_gate(Pos pos) {
        map[pos].restored_rune_gate = true;
    }

    void generate_t1(Tilemap tm) {
        for (int x = 8; x < 13; x++) {
            for (int y = 8; y < 13; y++) {
                if (x == 10 && y == 10) {
                    Pos pos = new Pos(x, y);
                    MapCell mc = MapCell.create_cell(this, 1, CITY, city, pos);
                    mc.name = "City";
                    map.Add(pos, mc);
                    place_tile(city, x, y);
                } else {
                    create_cell(1, x, y);
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
                create_cell(2, x, y);
                create_cell(2, x, y + 8);
            }
        }

        // Vertical bars
        for (int x = 5; x < 8; x++) {
            for (int y = 8; y < 13; y++) {
                create_cell(2, x, y);
                create_cell(2, x + 8, y);
            }
        }
    }

    void generate_t3(Tilemap tm) {
        // (9 wide 3 deep over 2 wide band)
        // Horizontal protrusion
        for (int x = 6; x < 15; x++) {
            for (int y = 0; y < 3; y++) {
                create_cell(3, x, y);
                create_cell(3, x, y + 18);
            }
        }
        // Vertical protrusion
        for (int x = 0; x < 3; x++) {
            for (int y = 6; y < 15; y++) {
                create_cell(3, x, y);
                create_cell(3, x + 18, y);
            }
        }
        // Horizontal bar
        for (int x = 3; x < 18; x++) {
            for (int y = 3; y < 5; y++) {
                create_cell(3, x, y);
                create_cell(3, x, y + 13);
            }
        }
        // Vertical bar
        for (int x = 3; x < 5; x++) {
            for (int y = 5; y < 16; y++) {
                create_cell(3, x, y);
                create_cell(3, x + 13, y);
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