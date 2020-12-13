using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour, ISaveLoad {
    public static Map I { get; private set; }

    // Tile IDs
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

    public Tilemap tm;
    public Tile plains_1, plains_2;
    public Tile forest_1, forest_2;
    public Tile ruins_1, ruins_2;
    public Tile cliff_1, cliff_2;
    public Tile cave_1, cave_2;
    public Tile star_1, star_2;
    public Tile titrum_1, titrum_2;
    public Tile mire;
    public Tile lush_land_2;
    public Tile mountain_2;
    
    public Tile settlement;
    public Tile rune_gate;

    public Tile city, shadow;
    public CityCell city_cell;

    public Dictionary<int, Tile> tiles = new Dictionary<int, Tile>();
    public Dictionary<Tile, int> tile_to_tileID = new Dictionary<Tile, int>();

    private static readonly Dictionary<int, Dictionary<int, int>> bag_counters
     = new Dictionary<int, Dictionary<int, int>>();
    public Dictionary<int, List<int>> bags = new Dictionary<int, List<int>>();
    private readonly Dictionary<int, int> t1_bag_count = new Dictionary<int, int>() {
        {PLAINS_1, 6},
        {FOREST_1, 6},
        {RUINS_1, 4},
        {CLIFF_1, 1},
        {CAVE_1, 1},
        {STAR_1, 4},
        {TITRUM_1, 2},
    };
    private readonly Dictionary<int, int> t2_bag_count = new Dictionary<int, int>() { // 96
        {PLAINS_2, 20},
        {FOREST_2, 20},
        {RUINS_2, 12},
        {CLIFF_2, 2},
        {CAVE_2, 6},
        {STAR_2, 7},
        {TITRUM_2, 8},
        //{MIRE, 0}, // 10
        {SETTLEMENT, 2},
        {LUSH_LAND_2, 5},
        {MOUNTAIN_2, 14},
        {RUNE_GATE, 2},
    };  
    private readonly Dictionary<int, int> t3_bag_count = new Dictionary<int, int>() { // 212
        {PLAINS_2, 50},
        {FOREST_2, 50},
        {RUINS_2, 24},
        {CLIFF_2, 4},
        {CAVE_2, 12},
        {STAR_2, 14},
        {TITRUM_2, 16},
        //{MIRE, 0},
        {SETTLEMENT, 4},
        {LUSH_LAND_2, 10},
        {MOUNTAIN_2, 28},
        {RUNE_GATE, 4},
    };
    
    public Dictionary<Pos, MapCell> map = new Dictionary<Pos, MapCell>();
    public bool waiting_for_second_gate { get; set; } = false;
    public bool scouting { get; set; } = false;

    public List<MapCell> oscillating_cells = new List<MapCell>();
    private System.Random rand;
    void Awake() {
        if (I == null) {
            I = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        tm = GameObject.Find("MapTilemap").GetComponent<Tilemap>();
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
        //tiles.Add(MIRE, mire);
        tiles.Add(PLAINS_2, plains_2);
        tiles.Add(FOREST_2, forest_2);
        tiles.Add(RUINS_2, ruins_2);
        tiles.Add(CLIFF_2, cliff_2);
        tiles.Add(CAVE_2, cave_2);
        tiles.Add(STAR_2, star_2);
        tiles.Add(TITRUM_2, titrum_2);
        tiles.Add(MOUNTAIN_2, mountain_2);
        tiles.Add(LUSH_LAND_2, lush_land_2);

        create_city();

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

    private void new_game() {
        clear_data();
        scouting = false;
        waiting_for_second_gate = false;
        populate_decks();
        // Recreate city only if game has loaded the first time.
        if (Controller.I.game_has_begun) {
            create_city();
        } else {
            map.Add(city_cell.pos, city_cell);
        }
        generate_t1(tm);
        generate_t2(tm);
        generate_t3(tm);
    }

    private void create_city() {
        city_cell = (CityCell)MapCell.create_cell(1, CITY, city, new Pos(10, 10), MapCell.CITY);
        city_cell.discover();
        map.Add(city_cell.pos, city_cell);
    }

    private void clear_data() {
        bags[1].Clear();
        bags[2].Clear();
        bags[3].Clear();
        map.Clear();
        MapUI.I.close_cell_UI();
    }

    public bool can_move(Vector3 destination) {
        Vector3 current_pos = get_current_cell().pos.to_vec3;
        return check_adjacent(destination, current_pos) && 
            !Controller.I.get_disc().has_acted_in_turn &&
            !get_cell(destination).has_battle &&
            !get_cell(destination).has_group_pending;
    }

    public void scout(Vector3 pos) {
        MapCell cell = map[new Pos((int)pos.x, (int)pos.y)];
        cell.discover();
        Controller.I.get_disc().has_scouted_in_turn = true;
        
        // Draw card in advance to reveal enemy count if applicable.
        cell.travelcard = TravelDeck.I.draw_card(cell.tier, cell.biome_ID);
        if (!cell.has_travelcard)
            return;
        if (cell.travelcard.enemy_count > 0) {
            EnemyLoader.I.generate_new_enemies(cell, cell.travelcard.enemy_count);
            cell.has_seen_combat = true; // Will bypass enemy generation when cell is entered.
        }
    }

    public bool can_scout(Vector3 pos) {
        return get_tile(pos.x, pos.y) != null && check_adjacent(pos, Controller.I.get_disc().pos) && 
            Controller.I.get_disc().bat.get_unit(PlayerUnit.SCOUT) != null &&
            get_current_cell() != get_cell(pos) && get_cell(pos) != city_cell &&
            !get_cell(pos).discovered &&
            !Controller.I.get_disc().has_acted_in_turn;
    }

    public bool can_teleport(Vector3 pos) {
        MapCell cell = get_cell(pos);
        return cell.restored_rune_gate && get_current_cell().restored_rune_gate &&
            get_current_cell() != cell && cell != city_cell;
    }

    public static bool check_adjacent(Vector3 pos1, Vector3 pos2) {
        int dx = Mathf.Abs((int)pos1.x - (int)pos2.x);
        int dy = Mathf.Abs((int)pos1.y - (int)pos2.y);
        return dx + dy == 1;
    }

    // Randomly pick tiles from grab bags. 
    private Tile grab_tile(int tier) {
        if (bags[tier].Count <= 0)
            return null;

        int index = rand.Next(bags[tier].Count);
        int tile_ID = bags[tier][index];

        bags[tier].RemoveAt(index);
        return tiles[tile_ID];
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
        MapCell cell = MapCell.create_cell(
            tier, tile_to_tileID[tile], tile, pos);
        map.Add(pos, cell);
        place_tile(shadow, pos.x, pos.y);
        tile.color = Color.white;

        if (cell.creates_travelcard) {
            cell.travelcard = TravelDeck.I.draw_card(cell.tier, cell.biome_ID);
            Debug.Log(cell.travelcard);
        }
        get_cell(pos.to_vec3).discover(); // debug
    }

    public Tile get_tile(float x, float y) {
        if (x >= 0 && y >= 0) 
            return tm.GetTile<Tile>(new Vector3Int((int)x, (int)y, 0));
        return null;  
    }

    public MapCell get_current_cell(Discipline disc=null) {
        //return disc == null ? get_cell(Controller.I.get_disc().pos) : get_cell(disc.pos);
        return disc == null ? Controller.I.get_disc().cell : disc.cell;
    }

    public void place_tile(Tile tile, int x, int y) {
        tm.SetTile(new Vector3Int(x, y, 0), tile);
        tm.SetTileFlags(new Vector3Int(y, x, 0), TileFlags.None); // Allow color change
    }

    public bool is_at_city(Discipline disc) {
        return disc.cell == city_cell;
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
            MapCell cell = MapCell.create_cell(
                mcs.tier, mcs.tile_type, tiles[mcs.tile_type], pos);
            cell.minerals = mcs.minerals;
            cell.star_crystals = mcs.star_crystals;

            if (mcs.discovered) {
                cell.discover();
            } else {
                place_tile(shadow, pos.x, pos.y);
            }
            map.Add(pos, cell);
        }
    }

    public void add_oscillating_cell(MapCell cell) {
        oscillating_cells.Add(cell);
    }

    public bool remove_oscillating_cell(MapCell cell) {
        if (oscillating_cells.Contains(cell)) {
            oscillating_cells.Remove(cell);
            return true;
        }
        return false;
    }

    public MapCell get_cell(Vector3 pos) {
        Pos p = new Pos((int)pos.x, (int)pos.y);
        if (!map.ContainsKey(p)) {
           return null;
        }
        return map[p];
    }

    public List<Enemy> get_enemies_here() {
        return Controller.I.get_disc().cell.get_enemies();
    }

    public void retreat_battalion() {
        Controller.I.get_disc().move_to_previous_cell();
    }
    
    public void build_rune_gate(Pos pos) {
        map[pos].restored_rune_gate = true;
    }

    void generate_t1(Tilemap tm) {
        for (int x = 8; x < 13; x++) {
            for (int y = 8; y < 13; y++) {
                if (x == 10 && y == 10) {
                    place_tile(city, x, y);
                } else {
                    create_cell(1, x, y);
                }
            } 
        }
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