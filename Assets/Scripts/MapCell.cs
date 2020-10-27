using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MapCell {
    public const string PLAINS = "Plains";
    public const string FOREST = "Forest";
    public const string RUINS = "Ruins";
    public const string CLIFF = "Cliff";
    public const string CAVE = "Cave";
    public const string STAR = "Star";
    public const string TITRUM = "Titrum";
    public const string LUSH_LAND = "Lush Land";
    public const string MIRE = "Mire";
    public const string MOUNTAIN = "Mountain";
    public const string SETTLEMENT = "Settlement";
    public const string RUNE_GATE = "Rune Gate";

    public const int PLAINS_ID = 0;
    public const int FOREST_ID = 1;
    public const int RUINS_ID = 2;
    public const int CLIFF_ID = 3;
    public const int CAVE_ID = 4;
    public const int STAR_ID = 5;
    public const int TITRUM_ID = 6;
    public const int LUSH_LAND_ID = 7;

    // Not an actual tile type, just enemy spawn type as determined by travelcards
    public const int MELD_ID = 8;
    public const int MOUNTAIN_ID = 9;
    public const int SETTLEMENT_ID = 10;
    public const int RUNE_GATE_ID = 11;

    public static MapCell create_cell(Map map, int tier, int tile_type_ID, Tile tile, Pos pos) {
        MapCell mc = null;
        string[] splits = tile.ToString().Split('_');

        string name = splits[0];
        if (name == PLAINS) {
            mc = new Plains(tier, tile, pos);
        } else if (name == FOREST) {
            mc = new Forest(tier, tile, pos);
        } else if (name == RUINS) {
            mc = new Ruins(tier, tile, pos);
        } else if (name == CLIFF) {
            mc = new Cliff(tier, tile, pos);
        } else if (name == CAVE) {
            mc = new Cave(tier, tile, pos);
        } else if (name == STAR) {
            mc = new Star(tier, tile, pos);
        } else if (name == TITRUM) {
            mc = new Titrum(tier, tile, pos);
        } else if (name == LUSH_LAND) {
            mc = new LushLand(tier, tile, pos);
        //} else if (name == MIRE) {
            //mc = new Mire(tier, tile, pos);
        } else if (name == MOUNTAIN) {
            mc = new Mountain(tier, tile, pos);
        } else if (name == SETTLEMENT) {
            mc = new Settlement(tier, tile, pos);
        } else if (name == RUNE_GATE) {
            mc = new RuneGate(tier, tile, pos);
        } else {
            mc = new MapCell(tier, tile, pos, 0);
        }
        mc.tile_type_ID = tile_type_ID;
        mc.map = map;
        return mc;
    }

    private static Color enemy_color = new Color(1, .5f, .5f, 1f);
    public readonly Tile tile;
    public readonly int tier;
    public readonly Pos pos;
    public readonly int biome_ID;
    public bool discovered = false;
    public string name;
    public int minerals, star_crystals = 0;
    public int tile_type_ID;
    public bool creates_travelcard = true;
    public bool has_rune_gate = false;
    public bool restored_rune_gate = false;
    public bool travelcard_complete = false;
    public Battle battle;
    private List<Enemy> enemies = new List<Enemy>();
    public Map map;
    private TravelCard _travelcard;
    public TravelCard travelcard { 
        get => _travelcard;
        set {
            if (value == null)
                return;
            travelcard_complete = false;
            _travelcard = value;
        }
     }

    public MapCell(int tier, Tile tile, Pos pos, int biome_ID) {
        this.tile = tile;
        this.tier = tier;
        this.pos = pos;
        this.biome_ID = biome_ID;
    }

    public void post_battle() {
        clear_dead();
        foreach (Enemy e in enemies)
            e.get_slot().update_UI();
    }

    public void post_phase() {
        foreach (Enemy e in enemies)
            e.post_phase();
    }

    private void clear_dead() {
        // Don't modify enemies while iterating.
        List<Enemy> dead = new List<Enemy>();
        foreach (Enemy e in enemies) {
            if (e.is_dead)
                dead.Add(e);
        }
        foreach (Enemy e in dead) 
            kill_enemy(e);

        set_tile_color();
    }
 
    private void kill_enemy(Enemy enemy) {
        if (enemy.get_slot() != null)
            enemy.get_slot().empty(); // validate as you go?
        enemies.Remove(enemy);
    }

    private void set_tile_color() {
        if (enemies.Count > 0) {
            Map map = GameObject.Find("Controller").GetComponent<Controller>().map;
            Vector3Int vec = new Vector3Int(pos.x, pos.y, 0);
            map.tm.SetTileFlags(vec, TileFlags.None);
            map.tm.SetColor(vec, enemy_color); // Dark red
        } else {
            tile.color = Color.white;
        }
    }

    public void discover() {
        discovered = true;
        map.adjust_light_size(this);
    }

    public void complete_travelcard() {
        travelcard = null;
        travelcard_complete = true;
    }

    public void assign_group_leader() {
        battle = new Battle(map, this, map.c.get_disc(), true);
        begin_color_oscillation();
    }

    public void clear_battle() {
        foreach (Discipline d in battle.participants) {
            d.bat.pending_group_battle_cell = null;
        }
        battle = null;
        end_color_oscillation();
    }

    public void begin_color_oscillation() {
        map.add_oscillating_cell(this);
    }

    public void end_color_oscillation() {
        if (map.remove_oscillating_cell(this))
            map.tm.SetColor(new Vector3Int(pos.x, pos.y, 0), Color.white);
    }
    
    public void oscillate_color() {
        float y = 0.75f + (Mathf.Sin(Time.time) / 4f);
        Tile t = (Tile)map.tm.GetTile(new Vector3Int(pos.x, pos.y, 0));
        map.tm.SetColor(new Vector3Int(pos.x, pos.y, 0), new Color(1, y, y, 1));
    }

    // Can currently only group battle if the player has retreated/scouted the tile
    // and a group has not already been formed on this cell.
    public bool can_setup_group_battle() {
        return has_enemies;
    }

    public void add_enemy(Enemy e) {
        if (e != null)
            enemies.Add(e);
        set_tile_color();
    }

    public List<Enemy> get_enemies() {
        //Debug.Log("Retrieving " + enemies.Count + " enemies from " + pos.x + ", " + pos.y);
        return enemies;
    }

    public bool has_enemies { 
        get { return (get_enemies().Count > 0); } 
    }

    public bool requires_unlock {
        get {
            if (has_rune_gate && !restored_rune_gate) {
                return true;
            } else if (travelcard != null) {
                if (travelcard.unlockable != null)
                    return true;
            }
            return false;
        }
    }

    public TravelCardUnlockable get_unlockable() {
        return travelcard.unlockable;
    }

    public int get_unlock_cost() {
        if (has_rune_gate) 
            return 10;
        else if (travelcard.unlockable != null)
            return travelcard.unlockable.resource_cost;
        return 0;
    }

    public string get_unlock_type() {
        if (has_rune_gate)
            return Storeable.STAR_CRYSTALS;
        else    
            return travelcard.unlockable.resource_type;
    }

    public bool has_travelcard {
        get { return travelcard != null; }
    }

    public Dictionary<string, int> get_travelcard_consequence() {
        return travelcard.consequence;
    }

    public bool can_mine(Battalion b) {
        return b.has_miner && !b.disc.has_mined_in_turn && 
            map.get_cell(b.disc.pos) == this &&
            (minerals > 0 || star_crystals > 0);
    }
}

public class Plains : MapCell {
    public Plains(int tier, Tile tile, Pos pos) : base(tier, tile, pos, PLAINS_ID) {
        name = PLAINS;
    }
}

public class Forest : MapCell {
    public Forest(int tier, Tile tile, Pos pos) : base(tier, tile, pos, FOREST_ID) {
        name = FOREST;
    }
}

public class Ruins : MapCell {
    public Ruins(int tier, Tile tile, Pos pos) : base(tier, tile, pos, RUINS_ID) {
        name = RUINS;
    }
}

public class Cliff : MapCell {
    public Cliff(int tier, Tile tile, Pos pos) : base(tier, tile, pos, CLIFF_ID) {
        name = CLIFF;
    }
}

public class Cave : MapCell {
    public Cave(int tier, Tile tile, Pos pos) : base(tier, tile, pos, CAVE_ID) {
        name = CAVE;
    }
}

public class Star : MapCell {
    public Star(int tier, Tile tile, Pos pos) : base(tier, tile, pos, STAR_ID) {
        name = STAR;
        star_crystals = 18;
        creates_travelcard = false;
        travelcard_complete = true;
    }
}

public class Titrum : MapCell {
    public Titrum(int tier, Tile tile, Pos pos) : base(tier, tile, pos, TITRUM_ID) {
        name = TITRUM;
        minerals = 24;
    }
}
public class LushLand : MapCell {
    public LushLand(int tier, Tile tile, Pos pos) : base(tier, tile, pos, LUSH_LAND_ID) {
        name = LUSH_LAND;
        creates_travelcard = false;
        travelcard_complete = true;
    }
}
/*
public class Mire : MapCell {
    public Mire(int tier, Tile tile, Pos pos) : base(tier, tile, pos, MIRE_ID) {
        name = MIRE;
    }
}*/
public class Mountain : MapCell {
    public Mountain(int tier, Tile tile, Pos pos) : base(tier, tile, pos, MOUNTAIN_ID) {
        name = MOUNTAIN;
        minerals = 21;
    }
}
public class Settlement : MapCell {
    public Settlement(int tier, Tile tile, Pos pos) : base(tier, tile, pos, SETTLEMENT_ID) {
        name = SETTLEMENT;
        creates_travelcard = false;
        travelcard_complete = true;
    }
}
public class RuneGate : MapCell {
    public RuneGate(int tier, Tile tile, Pos pos) : base(tier, tile, pos, RUNE_GATE_ID) {
        name = RUNE_GATE;
        has_rune_gate = true;
    }
}
