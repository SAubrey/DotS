using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


// This draws enemies and places them into combat slots. Enemy drawing
// is percentage based with replacement.
public class EnemyLoader : MonoBehaviour {
    public static EnemyLoader I { get; private set; }
    public static int COMMON_THRESH = 0;
    public static int UNCOMMON_THRESH = 60;
    public static int RARE_THRESH = 90;
    public static int MAX_ROLL = 100;

    // Spawning zones - 2 x,y coordinates from low to high.
    private Zone front_first_zone = new Zone(4, 7, 6, 7);
    private Zone front_second_zone = new Zone(4, 8, 6, 8);
    private Zone rear_first_zone = new Zone(4, 3, 6, 3);
    private Zone rear_second_zone = new Zone(4, 4, 6, 4);
    private Zone right_first_zone = new Zone(7, 4, 7, 6);
    private Zone right_second_zone = new Zone(8, 4, 8, 6);
    private Zone left_first_zone = new Zone(3, 4, 3, 6);
    private Zone left_second_zone = new Zone(2, 4, 2, 6);

    public const int T1 = 1;
    public const int T2 = 2;
    public const int T3 = 3;

    private Controller c;
    private Formation f;
    public System.Random rand;
    public Dictionary<int, List<List<List<int>>>> biomes = 
        new Dictionary<int, List<List<List<int>>>>();
    

    // Assign possible enemy spawns (by ID) per biome, per enemy tier, per spawn rate.
    //biomes[PLAINS][1][Enemy.UNCOMMON]
    public List<List<List<int>>> plains_tiers = new List<List<List<int>>>();
    public List<List<List<int>>> forest_tiers = new List<List<List<int>>>();
    public List<List<List<int>>> titrum_tiers = new List<List<List<int>>>();
    public List<List<List<int>>> cliff_tiers = new List<List<List<int>>>();
    public List<List<List<int>>> mountain_tiers = new List<List<List<int>>>();
    public List<List<List<int>>> cave_tiers = new List<List<List<int>>>();
    public List<List<List<int>>> meld_tiers = new List<List<List<int>>>();
    void Awake() {
        if (I == null) {
            I = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        f = GameObject.Find("Formation").GetComponent<Formation>();
        rand = new System.Random();

        make_biome(plains_tiers);
        make_biome(forest_tiers);
        make_biome(titrum_tiers);
        make_biome(cliff_tiers);
        make_biome(mountain_tiers);
        make_biome(cave_tiers);
        make_biome(meld_tiers);

        biomes.Add(MapCell.PLAINS_ID, plains_tiers);
        biomes.Add(MapCell.FOREST_ID, forest_tiers);
        biomes.Add(MapCell.TITRUM_ID, titrum_tiers);
        biomes.Add(MapCell.CLIFF_ID, cliff_tiers);
        biomes.Add(MapCell.MOUNTAIN_ID, mountain_tiers);
        biomes.Add(MapCell.CAVE_ID, cave_tiers);
        biomes.Add(MapCell.MELD_ID, meld_tiers);

        populate_biomes();
    }

    private void make_biome(List<List<List<int>>> biome_tiers) {
        for (int i = 0; i <= T3; i++) { // for each tier
            List<List<int>> rarities = new List<List<int>>();
            for (int j = 0; j <= 2; j++) { // for each rarity
                rarities.Add(new List<int>());
            }
            biome_tiers.Insert(i, rarities);
        }
    }

    private void reset() {
        front_first_zone.reset();
        front_second_zone.reset();
        rear_first_zone.reset();
        rear_second_zone.reset();
        right_first_zone.reset();
        right_second_zone.reset();
        left_first_zone.reset();
        left_second_zone.reset();
    }

    public void generate_new_enemies(MapCell cell, int quantity) {
        for (int i = 0; i < quantity; i++) {
            int rarity = roll_rarity(); 
            int enemyID = -1;
            if (cell.biome_ID == MapCell.RUINS_ID) {
                enemyID = pick_enemy(cell.travelcard.enemy_biome_ID, cell.tier, rarity);
            } else {
                enemyID = pick_enemy(cell.biome_ID, cell.tier, rarity);
            }
            cell.add_enemy(Enemy.create_enemy(enemyID));
        }
        reset();
    }

    public void load_enemies(List<Enemy> enemies) {
        foreach (Enemy e in enemies) {
            slot_enemy(e);
        }
        reset();
    }

    private int roll_rarity() {
        int rarity = rand.Next(0, MAX_ROLL);
        if (rarity < UNCOMMON_THRESH) 
            return Enemy.COMMON;
        else if (rarity < RARE_THRESH)
            return Enemy.UNCOMMON;
        else
            return Enemy.RARE;
    }

    private int pick_enemy(int biome, int tier, int rarity) {
        List<int> candidates = biomes[biome][tier][rarity];

        // Try lower rarities if one is missing. (There should always be a common)
        for (int i = 0; i < 3; i++) {
            candidates = biomes[biome][tier][rarity];
            if (candidates.Count > 0)
                break;
            rarity--;
        }
        int r = rand.Next(0, candidates.Count);
        //Debug.Log("candidates: " + candidates.Count + ". " +
             //biome + ", " + tier + ", " + rarity + ", " + r);
        return biomes[biome][tier][rarity][r];
    }

    private void slot_enemy(Enemy enemy) {
        Zone zone = get_appropriate_zone(enemy);
        if (fill_slot(enemy, zone.get_spawn_pos()))
            zone.increment_pos();
    }

    private bool spawn_left = false;
    private Zone get_appropriate_zone(Enemy enemy) {
        Zone zone = front_first_zone;
        if (enemy.has_attribute(Enemy.FLANKING)) {
            spawn_left = !spawn_left;
            zone = spawn_left ? left_first_zone : right_first_zone;
        }
        else if (enemy.has_attribute(Enemy.STALK)) {
            zone = rear_first_zone.full ? rear_second_zone : rear_first_zone;
        }
        else if (enemy.is_range) {
            if (front_second_zone.full) {
                spawn_left = !spawn_left;
                zone = spawn_left ? left_second_zone : right_second_zone;
            } else
                zone = front_second_zone;
        } else {
            if (front_first_zone.full) {
                spawn_left = !spawn_left;
                zone = spawn_left ? right_first_zone : left_first_zone;   
            }
        }
        return zone;
    }

    private bool fill_slot(Enemy enemy, Pos pos) {
        Slot s = f.get_group(pos.x, pos.y).get_highest_empty_slot();
        if (s != null) {
            s.fill(enemy);
            return true;
        }
        return false;
    }

    private void populate_biomes() {
        biomes[MapCell.PLAINS_ID][T1][Enemy.COMMON].Add(Enemy.GALTSA);
        biomes[MapCell.PLAINS_ID][T1][Enemy.COMMON].Add(Enemy.GREM);
        biomes[MapCell.PLAINS_ID][T1][Enemy.UNCOMMON].Add(Enemy.ENDU);
        biomes[MapCell.PLAINS_ID][T1][Enemy.COMMON].Add(Enemy.KOROTE);
        biomes[MapCell.PLAINS_ID][T2][Enemy.COMMON].Add(Enemy.MOLNER);
        biomes[MapCell.PLAINS_ID][T2][Enemy.COMMON].Add(Enemy.ETUENA);
        biomes[MapCell.PLAINS_ID][T2][Enemy.UNCOMMON].Add(Enemy.CLYPTE);
        biomes[MapCell.PLAINS_ID][T2][Enemy.RARE].Add(Enemy.GOLIATH);
        biomes[MapCell.PLAINS_ID][T3][Enemy.COMMON].Add(Enemy.GALTSA); //false

        biomes[MapCell.FOREST_ID][T1][Enemy.COMMON].Add(Enemy.KVERM);
        biomes[MapCell.FOREST_ID][T1][Enemy.UNCOMMON].Add(Enemy.LATU);
        biomes[MapCell.FOREST_ID][T1][Enemy.COMMON].Add(Enemy.EKE_TU);
        biomes[MapCell.FOREST_ID][T1][Enemy.COMMON].Add(Enemy.OETEM);
        biomes[MapCell.FOREST_ID][T2][Enemy.COMMON].Add(Enemy.EKE_FU);
        biomes[MapCell.FOREST_ID][T2][Enemy.UNCOMMON].Add(Enemy.EKE_SHI_AMI);
        biomes[MapCell.FOREST_ID][T2][Enemy.RARE].Add(Enemy.EKE_LORD);
        biomes[MapCell.FOREST_ID][T2][Enemy.UNCOMMON].Add(Enemy.KETEMCOL);
        biomes[MapCell.FOREST_ID][T3][Enemy.COMMON].Add(Enemy.KVERM);//false

        biomes[MapCell.TITRUM_ID][T1][Enemy.COMMON].Add(Enemy.MAHUKIN);
        biomes[MapCell.TITRUM_ID][T1][Enemy.UNCOMMON].Add(Enemy.DRONGO);
        biomes[MapCell.TITRUM_ID][T2][Enemy.COMMON].Add(Enemy.MAHEKET);
        biomes[MapCell.TITRUM_ID][T2][Enemy.UNCOMMON].Add(Enemy.CALUTE);
        biomes[MapCell.TITRUM_ID][T2][Enemy.UNCOMMON].Add(Enemy.ETALKET);
        biomes[MapCell.TITRUM_ID][T2][Enemy.RARE].Add(Enemy.MUATEM);
        biomes[MapCell.TITRUM_ID][T3][Enemy.COMMON].Add(Enemy.MAHUKIN);//false

        biomes[MapCell.MOUNTAIN_ID][T2][Enemy.COMMON].Add(Enemy.DRAK);
        biomes[MapCell.MOUNTAIN_ID][T2][Enemy.COMMON].Add(Enemy.ZERRKU);
        biomes[MapCell.MOUNTAIN_ID][T2][Enemy.COMMON].Add(Enemy.GOKIN);
        biomes[MapCell.MOUNTAIN_ID][T3][Enemy.COMMON].Add(Enemy.DRAK);// false?
        biomes[MapCell.MOUNTAIN_ID][T3][Enemy.COMMON].Add(Enemy.ZERRKU);
        biomes[MapCell.MOUNTAIN_ID][T3][Enemy.COMMON].Add(Enemy.GOKIN);

        biomes[MapCell.CLIFF_ID][T1][Enemy.COMMON].Add(Enemy.DRAK);
        biomes[MapCell.CLIFF_ID][T1][Enemy.COMMON].Add(Enemy.ZERRKU);
        biomes[MapCell.CLIFF_ID][T1][Enemy.COMMON].Add(Enemy.GOKIN);
        biomes[MapCell.CLIFF_ID][T2][Enemy.COMMON].Add(Enemy.DRAK);
        biomes[MapCell.CLIFF_ID][T2][Enemy.COMMON].Add(Enemy.ZERRKU);
        biomes[MapCell.CLIFF_ID][T2][Enemy.COMMON].Add(Enemy.GOKIN);
        biomes[MapCell.CLIFF_ID][T3][Enemy.COMMON].Add(Enemy.DRAK);//false
        biomes[MapCell.CLIFF_ID][T3][Enemy.COMMON].Add(Enemy.ZERRKU);
        biomes[MapCell.CLIFF_ID][T3][Enemy.COMMON].Add(Enemy.GOKIN);

        biomes[MapCell.CAVE_ID][T1][Enemy.COMMON].Add(Enemy.TAJAQAR);
        biomes[MapCell.CAVE_ID][T1][Enemy.COMMON].Add(Enemy.TAJAERO);
        biomes[MapCell.CAVE_ID][T1][Enemy.RARE].Add(Enemy.TERRA_QUAL);
        biomes[MapCell.CAVE_ID][T1][Enemy.UNCOMMON].Add(Enemy.DUALE);
        biomes[MapCell.CAVE_ID][T2][Enemy.COMMON].Add(Enemy.TAJAQAR);
        biomes[MapCell.CAVE_ID][T2][Enemy.COMMON].Add(Enemy.TAJAERO);
        biomes[MapCell.CAVE_ID][T2][Enemy.RARE].Add(Enemy.TERRA_QUAL);
        biomes[MapCell.CAVE_ID][T2][Enemy.UNCOMMON].Add(Enemy.DUALE);
        biomes[MapCell.CAVE_ID][T3][Enemy.COMMON].Add(Enemy.TAJAQAR);//false
        biomes[MapCell.CAVE_ID][T3][Enemy.COMMON].Add(Enemy.TAJAERO);
        biomes[MapCell.CAVE_ID][T3][Enemy.RARE].Add(Enemy.TERRA_QUAL);
        biomes[MapCell.CAVE_ID][T3][Enemy.UNCOMMON].Add(Enemy.DUALE);

        biomes[MapCell.MELD_ID][T1][Enemy.COMMON].Add(Enemy.MELD_WARRIOR);
        biomes[MapCell.MELD_ID][T1][Enemy.COMMON].Add(Enemy.MELD_SPEARMAN);
        biomes[MapCell.MELD_ID][T2][Enemy.COMMON].Add(Enemy.MELD_WARRIOR);
        biomes[MapCell.MELD_ID][T2][Enemy.COMMON].Add(Enemy.MELD_SPEARMAN);
        biomes[MapCell.MELD_ID][T3][Enemy.COMMON].Add(Enemy.MELD_WARRIOR);
        biomes[MapCell.MELD_ID][T3][Enemy.COMMON].Add(Enemy.MELD_SPEARMAN);
    }
}

public class Zone {
    public Pos low;
    public Pos high;
    //public Pos current_pos;
    public bool increments_horizontally = false;
    int _col = 0;
    int col {
        get { return _col; }
        set { _col = value % 3; }
    }
    int _row = 0;
    int row {
        get { return _row; }
        set { _row = value % 3; }
    }
    int enemies_slotted = 0;
    public bool full {
        get { return (enemies_slotted >= 9); } 
    }

    public Zone(int low_col, int low_row, int high_col, int high_row) {
        low = new Pos(low_col, low_row);
        high = new Pos(high_col, high_row);
        if (high_row - low_row == 0) {
            increments_horizontally = true;
            col = 1;
        } else
            row = 1;
    }

    public Pos get_spawn_pos() {
        return new Pos(low.x + col, low.y + row);
    }

    public void reset() {
        col = 0;
        row = 0;
        if (increments_horizontally)
            col = 1;
        else
            row = 1;
    }

    public void increment_pos() {
        enemies_slotted++;
        if (increments_horizontally) {
            col++;
        } else {
            row++;
        }
    }
}
