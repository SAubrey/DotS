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
    public const string CITY = "City";
    public const string GUARDIAN_PASS = "Guardian Pass";

    public const int CITY_ID = -1;
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
    public const int GUARDIAN_PASS_ID = 12;

    public static MapCell create_cell(int ID, int tier, Tile tile, Pos pos) {
        MapCell mc = null;
        // Don't extract name from tile if provided.
        if (ID == PLAINS_ID) {
            mc = new Plains(tier, tile, pos);
        } else if (ID == FOREST_ID) {
            mc = new Forest(tier, tile, pos);
        } else if (ID == RUINS_ID) {
            mc = new Ruins(tier, tile, pos);
        } else if (ID == CLIFF_ID) {
            mc = new Cliff(tier, tile, pos);
        } else if (ID == CAVE_ID) {
            mc = new Cave(tier, tile, pos);
        } else if (ID == STAR_ID) {
            mc = new Star(tier, tile, pos);
        } else if (ID == TITRUM_ID) {
            mc = new Titrum(tier, tile, pos);
        } else if (ID == LUSH_LAND_ID) {
            mc = new LushLand(tier, tile, pos);
        //} else if (name == MIRE) {
            //mc = new Mire(tier, tile, pos);
        } else if (ID == MOUNTAIN_ID) {
            mc = new Mountain(tier, tile, pos);
        } else if (ID == SETTLEMENT_ID) {
            mc = new Settlement(tier, tile, pos);
        } else if (ID == RUNE_GATE_ID) {
            mc = new RuneGate(tier, tile, pos);
        } else if (ID == CITY_ID) {
            mc = new CityCell(tier, tile, pos);
        } else if (ID == GUARDIAN_PASS_ID) {
            mc = new GuardianPass(tier, tile, pos);
        } else if (ID == CITY_ID) {
            mc = new CityCell(tier, tile, pos);
        } else
            mc = new MapCell(0, tier, tile, pos);
        //mc.tile_type_ID = tile_type_ID;
        return mc;
    }

    private static Color enemy_color = new Color(1, .5f, .5f, 1f);
    public readonly Tile tile;
    public readonly int tier;
    public readonly Pos pos;
    public readonly int ID;
    public bool entered { get; private set; }
    public bool discovered { get; private set; }
    public string name;
    public int minerals, star_crystals = 0;
    private int dropped_XP = 0;
    public GameObject dropped_XP_obj;
    public bool creates_travelcard = true;
    public bool has_rune_gate = false;
    public bool restored_rune_gate = false;
    private bool _travelcard_complete = false;
    public bool travelcard_complete {
        get { return _travelcard_complete; }
        set {
            _travelcard_complete = value;
            if (travelcard != null) {
                travelcard.complete = value;
            }
        }
    }
    public Battle battle;
    public bool has_seen_combat = false;
    public bool locked = false;
    public bool glows, flickers = false;
    private List<Enemy> enemies = new List<Enemy>();
    public GameObject fog;
    // Travelcards cannot be set to null.  
    private TravelCard _travelcard;
    public TravelCard travelcard { 
        get => _travelcard;
        set {
            if (value == null)
                return;
            _travelcard = value;
            locked = requires_unlock;
        }
     }

    public MapCell(int ID, int tier, Tile tile, Pos pos) {
        this.tile = tile;
        this.tier = tier;
        this.pos = pos;
        this.ID = ID;
        locked = requires_unlock;
    }

    public void enter() {
        if (creates_travelcard && !travelcard_complete) {
            MapUI.I.display_travelcard(travelcard);
        }
        if (!entered) {
            entered = true;
            discover();
        } 
        if (dropped_XP > 0) {
            pickup_XP(TurnPhaser.I.active_disc);
        }
    }
    
    public void discover() {
        if (discovered)
            return;
        discovered = true;
        Map.I.tm.SetTile(new Vector3Int((int)pos.x, (int)pos.y, 0), tile);
        MapUI.I.place_cell_light(this);
        //MapUI.I.remove_fog(this);
        GameObject.Destroy(fog);
        if (ID == STAR_ID) {
            MapUI.I.place_sparkle_ps(this);
        }
    }

    public void post_battle() {
        clear_dead();
        foreach (Enemy e in enemies)
            e.get_slot().update_text_UI();
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

    public void set_tile_color() {
        if (enemies.Count > 0) {
            Vector3Int vec = new Vector3Int(pos.x, pos.y, 0);
            Map.I.tm.SetTileFlags(vec, TileFlags.None);
            Map.I.tm.SetColor(vec, enemy_color); // Dark red
        } else {
            tile.color = Color.white;
        }
    }

    public bool should_activate_travelcard_without_showing { get =>
        creates_travelcard && !travelcard_complete && entered && !has_battle;
    }

    public void complete_travelcard() {
        travelcard_complete = true;
    }

    public void assign_group_leader() {
        battle = new Battle(Map.I, this, TurnPhaser.I.active_disc, true);
        begin_color_oscillation();
    }

    public void clear_battle() {
        if (battle != null) {
            foreach (Discipline d in battle.participants) {
                d.bat.pending_group_battle_cell = null;
            }
        }
        post_phase();
        battle = null;
        end_color_oscillation();
        set_tile_color();
    }

    private void begin_color_oscillation() {
        Map.I.add_oscillating_cell(this);
    }

    private void end_color_oscillation() {
        if (Map.I.remove_oscillating_cell(this))
            Map.I.tm.SetColor(new Vector3Int(pos.x, pos.y, 0), Color.white);
    }
    
    public void oscillate_color() {
        float y = 0.75f + (Mathf.Sin(Time.time) / 4f);
        Tile t = (Tile)Map.I.tm.GetTile(new Vector3Int(pos.x, pos.y, 0));
        Map.I.tm.SetColor(new Vector3Int(pos.x, pos.y, 0), new Color(1, y, y, 1));
    }

    // Can currently only group battle if the player has retreated/scouted the tile
    // and a group has not already been formed on this cell.
    public bool can_setup_group_battle() {
        return has_enemies && Map.check_adjacent(TurnPhaser.I.active_disc.pos, pos.to_vec3);
    }

    public void add_enemy(Enemy e) {
        if (e != null)
            enemies.Add(e);
        set_tile_color();
    }

    public List<Enemy> get_enemies() {
        return enemies;
    }

    public bool has_enemies { 
        get { return (get_enemies().Count > 0); } 
    }

    public bool requires_unlock {
        get {
            if (has_rune_gate && !restored_rune_gate) {
                return true;
            } else if (has_travelcard) {
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

    public bool has_travelcard { get => travelcard != null; }
    public bool has_battle { get => battle != null; }
    public bool has_group_pending { 
        get {
            if (!has_battle)
                return false;
            return battle.group_pending;
        }
    }

    public Dictionary<string, int> get_travelcard_consequence() {
        return travelcard.consequence;
    }

    public bool can_mine(Battalion b) {
        return b.has_miner && !b.disc.has_mined_in_turn && 
            b.disc.cell == this &&
            (minerals > 0 || star_crystals > 0);
    }

    public void drop_XP(int xp) {
        dropped_XP = xp;
        //show
        dropped_XP_obj = GameObject.Instantiate(MapUI.I.dropped_XP_prefab);
        dropped_XP_obj.transform.SetParent(MapUI.I.map_canvas.transform);
        // Move to center of cell from corner.
        Vector3 p = dropped_XP_obj.transform.position;
        dropped_XP_obj.transform.position = pos.to_vec3;
        dropped_XP_obj.transform.position = new Vector3(p.x + 0.5f, p.y + 0.5f, 0);
    }

    public int pickup_XP(Discipline d) {
        //d.change_var(Storeable.EXPERIENCE, dropped_XP, true);
        d.resources[Discipline.EXPERIENCE] += dropped_XP;
        d.show_adjustment("Experience", dropped_XP);
        dropped_XP = 0;
        GameObject.Destroy(dropped_XP_obj);
        return dropped_XP;
    }
}

public class CityCell : MapCell {
    public CityCell(int tier, Tile tile, Pos pos) : base(CITY_ID, tier, tile, pos) {
        name = "City";
        creates_travelcard = false;
        //pos = new
    }
}

public class Plains : MapCell {
    public Plains(int tier, Tile tile, Pos pos) : base(PLAINS_ID, tier, tile, pos) {
        name = PLAINS;
    }
}

public class Forest : MapCell {
    public Forest(int tier, Tile tile, Pos pos) : base(FOREST_ID, tier, tile, pos) {
        name = FOREST;
    }
}

public class Ruins : MapCell {
    public Ruins(int tier, Tile tile, Pos pos) : base(RUINS_ID, tier, tile, pos) {
        name = RUINS;
    }
}

public class Cliff : MapCell {
    public Cliff(int tier, Tile tile, Pos pos) : base(CLIFF_ID, tier, tile, pos) {
        name = CLIFF;
    }
}

public class Cave : MapCell {
    public Cave(int tier, Tile tile, Pos pos) : base(CAVE_ID, tier, tile, pos) {
        name = CAVE;
    }
}

public class Star : MapCell {
    public Star(int tier, Tile tile, Pos pos) : base(STAR_ID, tier, tile, pos) {
        name = STAR;
        star_crystals = 18;
        creates_travelcard = false;
        travelcard_complete = true;
        glows = true;
    }
}

public class Titrum : MapCell {
    public Titrum(int tier, Tile tile, Pos pos) : base(TITRUM_ID, tier, tile, pos) {
        name = TITRUM;
        minerals = 24;
        glows = true;
    }
}
public class LushLand : MapCell {
    public LushLand(int tier, Tile tile, Pos pos) : base(LUSH_LAND_ID, tier, tile, pos) {
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
    public Mountain(int tier, Tile tile, Pos pos) : base(MOUNTAIN_ID, tier, tile, pos) {
        name = MOUNTAIN;
        minerals = 21;
    }
}
public class Settlement : MapCell {
    public Settlement(int tier, Tile tile, Pos pos) : base(SETTLEMENT_ID, tier, tile, pos) {
        name = SETTLEMENT;
        creates_travelcard = false;
    }
}
public class RuneGate : MapCell {
    public RuneGate(int tier, Tile tile, Pos pos) : base(RUNE_GATE_ID, tier, tile, pos) {
        name = RUNE_GATE;
        has_rune_gate = true;
        glows = true;
    }
}

public class GuardianPass : MapCell {
    public GuardianPass(int tier, Tile tile, Pos pos) : base(GUARDIAN_PASS_ID, tier, tile, pos) {
        name = GUARDIAN_PASS;
    }
}
