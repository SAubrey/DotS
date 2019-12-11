using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


// This draws enemies and places them into combat slots. Enemy drawing
// is percentage based with replacement.
public class EnemyLoader : MonoBehaviour {
    public static int COMMON_THRESH = 0;
    public static int UNCOMMON_THRESH = 60;
    public static int RARE_THRESH = 90;
    public static int MAX_ROLL = 100;

    public const int PLAINS = 0;
    public const int FOREST = 1;
    public const int TITRUM = 2;
    public const int CLIFF = 3;
    public const int MOUNTAIN = 4;
    public const int CAVE = 5;

    public const int T1 = 0;
    public const int T2 = 1;
    public const int T3 = 2;

    private Formation f;
    public System.Random rand;
    private int enemy_row = 3;

    public Sprite galtsa, grem, endu, korote, molner, etuena, clypte, goliath, kverm,
        latu, eke_tu, oetem, eke_fu, eke_shi_ami, eke_lord, ketemcol, mahukin, drongo, maheket,
        calute, etalket, muatem, drak, zerrku, gokin, tajaqar, tajaero, terra_qual, duale;
    public Dictionary<int, Sprite> images = new Dictionary<int, Sprite>();
   
    public Dictionary<int, List<List<List<int>>>> biomes = 
        new Dictionary<int, List<List<List<int>>>>();

    //biomes[PLAINS][1][Enemy.UNCOMMON]
    public List<List<List<int>>> plains_tiers = new List<List<List<int>>>();
    public List<List<List<int>>> forest_tiers = new List<List<List<int>>>();
    public List<List<List<int>>> titrum_tiers = new List<List<List<int>>>();
    public List<List<List<int>>> cliff_tiers = new List<List<List<int>>>();
    public List<List<List<int>>> mountain_tiers = new List<List<List<int>>>();
    public List<List<List<int>>> cave_tiers = new List<List<List<int>>>();

    void Start() {
        f = GameObject.Find("Formation").GetComponent<Formation>();
        rand = new System.Random();
        images.Add(Enemy.GALTSA, galtsa);
        images.Add(Enemy.GREM, grem);
        images.Add(Enemy.ENDU, endu);
        images.Add(Enemy.KOROTE, korote);
        images.Add(Enemy.MOLNER, molner);
        images.Add(Enemy.ETUENA, etuena);
        images.Add(Enemy.CLYPTE, clypte);
        images.Add(Enemy.GOLIATH, goliath);
        images.Add(Enemy.KVERM, kverm);
        images.Add(Enemy.LATU, latu);
        images.Add(Enemy.EKE_TU, eke_tu);
        images.Add(Enemy.OETEM, oetem);
        images.Add(Enemy.EKE_FU, eke_fu);
        images.Add(Enemy.EKE_SHI_AMI, eke_shi_ami);
        images.Add(Enemy.EKE_LORD, eke_lord);
        images.Add(Enemy.KETEMCOL, ketemcol);
        images.Add(Enemy.MAHUKIN, mahukin);
        images.Add(Enemy.DRONGO, drongo);
        images.Add(Enemy.MAHEKET, maheket);
        images.Add(Enemy.CALUTE, calute);
        images.Add(Enemy.ETALKET, etalket);
        images.Add(Enemy.MUATEM, muatem);
        images.Add(Enemy.DRAK, drak);
        images.Add(Enemy.ZERRKU, zerrku);
        images.Add(Enemy.GOKIN, gokin);
        images.Add(Enemy.TAJAQAR, tajaqar);
        images.Add(Enemy.TAJAERO, tajaero);
        images.Add(Enemy.TERRA_QUAL, terra_qual);
        images.Add(Enemy.DUALE, duale);

        make_biome(plains_tiers);
        make_biome(forest_tiers);
        make_biome(titrum_tiers);
        make_biome(cliff_tiers);
        make_biome(mountain_tiers);
        make_biome(cave_tiers);

        biomes.Add(PLAINS, plains_tiers);
        biomes.Add(FOREST, forest_tiers);
        biomes.Add(TITRUM, titrum_tiers);
        biomes.Add(CLIFF, cliff_tiers);
        biomes.Add(MOUNTAIN, mountain_tiers);
        biomes.Add(CAVE, cave_tiers);

        populate_biomes();
    }

    private void make_biome(List<List<List<int>>> biome_tiers) {
        for (int i = 0; i <= 2; i++) { // for each tier
            List<List<int>> rarities = new List<List<int>>();
            for (int j = 0; j <= 2; j++) { // for each rarity
                rarities.Add(new List<int>());
            }
            biome_tiers.Insert(i, rarities);
        }
    }

    public void load(int biome, int tier, int quantity) {

        for (int i = 0; i < quantity; i++) {
            int rarity = roll_rarity(); 
            int enemyID = pick_enemy(biome, tier, rarity);
            Enemy e = Enemy.create_enemy(enemyID);
            slot_enemy(e);
        }
    }

    private int roll_rarity() {
        int rarity = rand.Next(0, MAX_ROLL + 1);
        if (rarity < UNCOMMON_THRESH) 
            return Enemy.COMMON;
        else if (rarity < RARE_THRESH)
            return Enemy.UNCOMMON;
        else
            return Enemy.RARE;
    }

    private int pick_enemy(int biome, int tier, int rarity) {
        List<int> candidates = biomes[biome][tier][rarity];
        int r = rand.Next(0, candidates.Count);
        return biomes[biome][tier][rarity][r];
    }

    private bool slot_enemy(Enemy enemy) {
        bool success = false;
        if (enemy.attributes[Enemy.FLANKING]) { // If flanking, place in rear.
            if (fill_slot(enemy, -1, 1)) {
                success = true;
            } else {
                success = fill_slot(enemy, enemy_row, spawn_column);
            }
        } else {
            success = fill_slot(enemy, enemy_row, spawn_column);
        }
        spawn_column++;
        return success;
    }

    private bool fill_slot(Enemy enemy, int row, int col) {
        Slot s = f.get_group(row, col).get_highest_empty_slot();
        if (s != null) {
            s.fill(enemy);
            return true;
        }
        return false;
    }

    private void populate_biomes() {
        biomes[PLAINS][T1][Enemy.COMMON].Add(Enemy.GALTSA);
        biomes[PLAINS][T1][Enemy.COMMON].Add(Enemy.GREM);
        biomes[PLAINS][T1][Enemy.UNCOMMON].Add(Enemy.ENDU);
        biomes[PLAINS][T1][Enemy.COMMON].Add(Enemy.KOROTE);
        biomes[PLAINS][T2][Enemy.COMMON].Add(Enemy.MOLNER);
        biomes[PLAINS][T2][Enemy.COMMON].Add(Enemy.ETUENA);
        biomes[PLAINS][T2][Enemy.UNCOMMON].Add(Enemy.CLYPTE);
        biomes[PLAINS][T2][Enemy.RARE].Add(Enemy.GOLIATH);

        biomes[FOREST][T1][Enemy.COMMON].Add(Enemy.KVERM);
        biomes[FOREST][T1][Enemy.UNCOMMON].Add(Enemy.LATU);
        biomes[FOREST][T1][Enemy.COMMON].Add(Enemy.EKE_TU);
        biomes[FOREST][T1][Enemy.COMMON].Add(Enemy.OETEM);
        biomes[FOREST][T2][Enemy.COMMON].Add(Enemy.EKE_FU);
        biomes[FOREST][T2][Enemy.UNCOMMON].Add(Enemy.EKE_SHI_AMI);
        biomes[FOREST][T2][Enemy.RARE].Add(Enemy.EKE_LORD);
        biomes[FOREST][T2][Enemy.UNCOMMON].Add(Enemy.KETEMCOL);

        biomes[TITRUM][T1][Enemy.COMMON].Add(Enemy.MAHUKIN);
        biomes[TITRUM][T1][Enemy.UNCOMMON].Add(Enemy.DRONGO);
        biomes[TITRUM][T2][Enemy.COMMON].Add(Enemy.MAHEKET);
        biomes[TITRUM][T2][Enemy.UNCOMMON].Add(Enemy.CALUTE);
        biomes[TITRUM][T2][Enemy.UNCOMMON].Add(Enemy.ETALKET);
        biomes[TITRUM][T2][Enemy.RARE].Add(Enemy.MUATEM);

        biomes[MOUNTAIN][T2][Enemy.COMMON].Add(Enemy.DRAK);
        biomes[MOUNTAIN][T2][Enemy.COMMON].Add(Enemy.ZERRKU);
        biomes[MOUNTAIN][T2][Enemy.COMMON].Add(Enemy.GOKIN);
        biomes[CLIFF][T1][Enemy.COMMON].Add(Enemy.DRAK);
        biomes[CLIFF][T1][Enemy.COMMON].Add(Enemy.ZERRKU);
        biomes[CLIFF][T1][Enemy.COMMON].Add(Enemy.GOKIN);

        biomes[CAVE][0][Enemy.COMMON].Add(Enemy.TAJAQAR);
        biomes[CAVE][0][Enemy.COMMON].Add(Enemy.TAJAERO);
        biomes[CAVE][0][Enemy.RARE].Add(Enemy.TERRA_QUAL);
        biomes[CAVE][0][Enemy.UNCOMMON].Add(Enemy.DUALE);
    }

    private int _spawn_column = 1;
    public int spawn_column {
        get { return _spawn_column % 3; }
        set {
            _spawn_column = value;
        }
    }
}
