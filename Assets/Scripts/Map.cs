using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour, ISaveLoad {
    public static Map I { get; private set; }

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
    public Tile impasse;
    
    public Tile settlement;
    public Tile rune_gate;

    public Tile city, shadow;
    public CityCell city_cell;

    //public Dictionary<int, Tile> tiles = new Dictionary<int, Tile>();
    public Dictionary<int, List<Tile>> tiles = new Dictionary<int, List<Tile>>();
    public Dictionary<Tile, int> tile_to_tileID = new Dictionary<Tile, int>();

    // Filled and then removed from as cells are drawn.
    public Dictionary<int, List<int>> bags = new Dictionary<int, List<int>>();
    private static readonly Dictionary<int, Dictionary<int, int>> bag_counters
     = new Dictionary<int, Dictionary<int, int>>();
    private readonly Dictionary<int, int> t1_bag_count = new Dictionary<int, int>() {
        {MapCell.PLAINS_ID, 6},
        {MapCell.FOREST_ID, 6},
        {MapCell.RUINS_ID, 4},
        {MapCell.CLIFF_ID, 1},
        {MapCell.CAVE_ID, 1},
        {MapCell.STAR_ID, 4},
        {MapCell.TITRUM_ID, 2},
    };
    private readonly Dictionary<int, int> t2_bag_count = new Dictionary<int, int>() { // 96 + 4 * 4
        {MapCell.PLAINS_ID, 42}, //32
        {MapCell.FOREST_ID, 22},
        {MapCell.RUINS_ID, 12},
        {MapCell.CLIFF_ID, 2},
        {MapCell.CAVE_ID, 6},
        {MapCell.STAR_ID, 7},
        {MapCell.TITRUM_ID, 8},
        //{MIRE_ID, 0}, // 10
        {MapCell.SETTLEMENT_ID, 2},
        {MapCell.LUSH_LAND_ID, 5},
        {MapCell.MOUNTAIN_ID, 14},
        {MapCell.RUNE_GATE_ID, 2},
    };  
    private readonly Dictionary<int, int> t3_bag_count = new Dictionary<int, int>() { // 212 + 8 x 4
        {MapCell.PLAINS_ID, 82},
        {MapCell.FOREST_ID, 54},
        {MapCell.RUINS_ID, 24},
        {MapCell.CLIFF_ID, 4},
        {MapCell.CAVE_ID, 12},
        {MapCell.STAR_ID, 14},
        {MapCell.TITRUM_ID, 16},
        //{MIRE_ID, 0},
        {MapCell.SETTLEMENT_ID, 4},
        {MapCell.LUSH_LAND_ID, 10},
        {MapCell.MOUNTAIN_ID, 28},
        {MapCell.RUNE_GATE_ID, 4},
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
        tiles.Add(MapCell.CITY_ID, new List<Tile> {city, city, city});
        tiles.Add(MapCell.PLAINS_ID, new List<Tile> {plains_1, plains_2, plains_2});
        tiles.Add(MapCell.FOREST_ID, new List<Tile> {forest_1, forest_2, forest_2});
        tiles.Add(MapCell.RUINS_ID, new List<Tile> {ruins_1, ruins_2, ruins_2});
        tiles.Add(MapCell.CLIFF_ID, new List<Tile> {cliff_1, cliff_2, cliff_2});
        tiles.Add(MapCell.CAVE_ID, new List<Tile> {cave_1, cave_2, cave_2});
        tiles.Add(MapCell.STAR_ID, new List<Tile> {star_1, star_2, star_2});
        tiles.Add(MapCell.TITRUM_ID, new List<Tile> {titrum_1, titrum_2, titrum_2});
        tiles.Add(MapCell.SETTLEMENT_ID, new List<Tile> {settlement, settlement, settlement});
        tiles.Add(MapCell.RUNE_GATE_ID, new List<Tile> {rune_gate, rune_gate, rune_gate});
        tiles.Add(MapCell.MOUNTAIN_ID, new List<Tile> {mountain_2, mountain_2, mountain_2});
        tiles.Add(MapCell.LUSH_LAND_ID, new List<Tile> {lush_land_2, lush_land_2, lush_land_2});
        tiles.Add(MapCell.GUARDIAN_PASS_ID, new List<Tile> {mountain_2, mountain_2, mountain_2});

        create_city();
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

    private void populate_decks() {
        for (int tier = 1; tier <= 3; tier++) { // For each tier
            foreach (int cell_ID in bag_counters[tier].Keys) {
                for (int i = 0; i < bag_counters[tier][cell_ID]; i++) {
                    bags[tier].Add(cell_ID);
                }
            }
        }
    }

    private void create_city() {
        city_cell = (CityCell)MapCell.create_cell(MapCell.CITY_ID, 1, city, new Pos(12, 12));
        city_cell.discover();
        map.Add(city_cell.pos, city_cell);
    }

    private void clear_data() {
        bags[1].Clear();
        bags[2].Clear();
        bags[3].Clear();
        map.Clear();
        MapUI.I.close_cell_UI();
        MapUI.I.close_travelcardP();
    }

    public bool can_move(Vector3 destination) {
        Vector3 current_pos = get_current_cell().pos.to_vec3;
        return check_adjacent(destination, current_pos) && 
            !TurnPhaser.I.active_disc.has_acted_in_turn &&
            !get_cell(destination).has_battle &&
            !get_cell(destination).has_group_pending;
    }

    public void scout(Vector3 pos) {
        MapCell cell = map[new Pos((int)pos.x, (int)pos.y)];
        cell.discover();
        TurnPhaser.I.active_disc.has_scouted_in_turn = true;
        
        // Draw card in advance to reveal enemy count if applicable.
        cell.travelcard = TravelDeck.I.draw_card(cell.tier, cell.ID);
        if (!cell.has_travelcard)
            return;
        if (cell.travelcard.enemy_count > 0) {
            EnemyLoader.I.generate_new_enemies(cell, cell.travelcard.enemy_count);
            cell.has_seen_combat = true; // Will bypass enemy generation when cell is entered.
        }
    }

    public bool can_scout(Vector3 pos) {
        return get_tile(pos.x, pos.y) != null && check_adjacent(pos, TurnPhaser.I.active_disc.pos) && 
            TurnPhaser.I.active_disc.bat.get_unit(PlayerUnit.SCOUT) != null &&
            get_current_cell() != get_cell(pos) && get_cell(pos) != city_cell &&
            !get_cell(pos).discovered &&
            !TurnPhaser.I.active_disc.has_acted_in_turn;
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
    private int grab_cell(int tier) {
        if (bags[tier].Count <= 0)
            throw new System.Exception("Out of cells to draw for this tier.");

        int index = rand.Next(bags[tier].Count);
        int cell_ID = bags[tier][index];

        bags[tier].RemoveAt(index);
        return cell_ID;
    }

    public void create_cell(int tier, int x, int y, int ID=-1) {
        Pos pos = new Pos(x, y);
        if (ID == -1) {
            ID = grab_cell(tier);
        }
        Tile tile = tiles[ID][tier - 1];
        MapCell cell = MapCell.create_cell(
            ID, tier, tile, pos);
        
        map.Add(pos, cell);
        place_tile(shadow, pos.x, pos.y);
        cell.fog = MapUI.I.place_fog_ps(cell);
        tile.color = Color.white;

        if (cell.creates_travelcard) {
            cell.travelcard = TravelDeck.I.draw_card(cell.tier, cell.ID);
        }
        //get_cell(pos.to_vec3).discover(); // debug
    }

    public Tile get_tile(float x, float y) {
        if (x >= 0 && y >= 0) 
            return tm.GetTile<Tile>(new Vector3Int((int)x, (int)y, 0));
        return null;  
    }

    public MapCell get_current_cell(Discipline disc=null) {
        //return disc == null ? get_cell(TurnPhaser.I.active_disc.pos) : get_cell(disc.pos);
        return disc == null ? TurnPhaser.I.active_disc.cell : disc.cell;
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
                mcs.ID, mcs.tier, tiles[mcs.ID][mcs.tier], pos);
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
        Debug.Log("Contains cell? " + oscillating_cells.Contains(cell));
        if (oscillating_cells.Contains(cell)) {
            Debug.Log("removing osc cell");
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
        return TurnPhaser.I.active_disc.cell.get_enemies();
    }

    public void retreat_battalion() {
        TurnPhaser.I.active_disc.move_to_previous_cell();
    }
    
    public void build_rune_gate(Pos pos) {
        map[pos].restored_rune_gate = true;
    }


    void generate_t1(Tilemap tm) {
        for (int x = 10; x < 15; x++) {
            for (int y = 10; y < 15; y++) {
                if (x == 12 && y == 12) {
                    place_tile(city, x, y);
                } else {
                    create_cell(1, x, y);
                }
            } 
        }
        create_cell(1, 12, 9, MapCell.GUARDIAN_PASS_ID);
        create_cell(1, 12, 15, MapCell.GUARDIAN_PASS_ID);
    }

    void generate_t2(Tilemap tm) {
        // t2 origin is 5, 5
        // Horizontal bars
        for (int x = 6; x < 19; x++) {
            for (int y = 6; y < 9; y++) {
                create_cell(2, x, y);
                create_cell(2, x, y + 10);
            }
        }

        // Vertical bars
        for (int x = 6; x < 9; x++) {
            for (int y = 9; y < 16; y++) {
                create_cell(2, x, y);
                create_cell(2, x + 10, y);
            }
        }
        create_cell(2, 5, 12, MapCell.GUARDIAN_PASS_ID);
        create_cell(2, 19, 12, MapCell.GUARDIAN_PASS_ID);
        create_cell(2, 12, 5, MapCell.GUARDIAN_PASS_ID);
        create_cell(2, 12, 19, MapCell.GUARDIAN_PASS_ID);
    }

    void generate_t3(Tilemap tm) {
        // (9 wide 3 deep over 2 wide band)
        // Horizontal protrusion
        for (int x = 8; x < 17; x++) {
            for (int y = 0; y < 3; y++) {
                create_cell(3, x, y);
                create_cell(3, x, y + 22);
            }
        }
        // Vertical protrusion
        for (int x = 0; x < 3; x++) {
            for (int y = 8; y < 17; y++) {
                create_cell(3, x, y);
                create_cell(3, x + 22, y);
            }
        }
        // Horizontal bar
        for (int x = 3; x < 22; x++) {
            for (int y = 3; y < 5; y++) {
                create_cell(3, x, y);
                create_cell(3, x, y + 17);
            }
        }
        // Vertical bar
        for (int x = 3; x < 5; x++) {
            for (int y = 5; y < 20; y++) {
                create_cell(3, x, y);
                create_cell(3, x + 17, y);
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