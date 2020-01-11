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

    public static MapCell create_cell(int tier, Tile tile, Pos pos) {
        MapCell mc = null;
        //Debug.Log(tile.ToString());
        string[] splits = tile.ToString().Split('_');

        //Debug.Log(splits.Length);
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
        } else if (name == MIRE) {
            mc = new Mire(tier, tile, pos);
        } else if (name == MOUNTAIN) {
            mc = new Mountain(tier, tile, pos);
        } else if (name == SETTLEMENT) {
            mc = new Settlement(tier, tile, pos);
        } else if (name == RUNE_GATE) {
            mc = new RuneGate(tier, tile, pos);
        } else {
            mc = new MapCell(tier, tile, pos);
        }
        return mc;
    }

    public Tile tile;
    public Pos pos;
    public int tier;
    public bool discovered = false;
    public string name;
    public int minerals = 0;
    public int star_crystals = 0;
    private List<Enemy> enemies = new List<Enemy>();

    public MapCell(int tier, Tile tile, Pos pos) {
        this.tile = tile;
        this.tier = tier;
        this.pos = pos;
    }

    public void discover() {
        discovered = true;
    }

    public void save_enemies(List<Slot> enemy_slots) {
        enemies.Clear();
        foreach (Slot es in enemy_slots) {
            enemies.Add(es.get_enemy());
        }
        Debug.Log("Saved " + enemies.Count + " enemies.");
    }

    public List<Enemy> get_enemies() {
        return enemies;
    }

    public bool has_enemies { 
        get { return get_enemies().Count > 0; }
    }

    public void clear_enemies() {
        enemies.Clear();
    }
}

public class Plains : MapCell {
    public Plains(int tier, Tile tile, Pos pos) : base(tier, tile, pos) {
        name = PLAINS;
    }
}

public class Forest : MapCell {
    public Forest(int tier, Tile tile, Pos pos) : base(tier, tile, pos) {
        name = FOREST;
    }
}

public class Ruins : MapCell {
    public Ruins(int tier, Tile tile, Pos pos) : base(tier, tile, pos) {
        name = RUINS;
    }
}

public class Cliff : MapCell {
    public Cliff(int tier, Tile tile, Pos pos) : base(tier, tile, pos) {
        name = CLIFF;
    }
}

public class Cave : MapCell {
    public Cave(int tier, Tile tile, Pos pos) : base(tier, tile, pos) {
        name = CAVE;
    }
}

public class Star : MapCell {
    public Star(int tier, Tile tile, Pos pos) : base(tier, tile, pos) {
        name = STAR;
        star_crystals = 18;
    }
}

public class Titrum : MapCell {
    public Titrum(int tier, Tile tile, Pos pos) : base(tier, tile, pos) {
        name = TITRUM;
        minerals = 24;
    }
}
public class LushLand : MapCell {
    public LushLand(int tier, Tile tile, Pos pos) : base(tier, tile, pos) {
        name = LUSH_LAND;
    }
}
public class Mire : MapCell {
    public Mire(int tier, Tile tile, Pos pos) : base(tier, tile, pos) {
        name = MIRE;
    }
}
public class Mountain : MapCell {
    public Mountain(int tier, Tile tile, Pos pos) : base(tier, tile, pos) {
        name = MOUNTAIN;
        minerals = 21;
    }
}
public class Settlement : MapCell {
    public Settlement(int tier, Tile tile, Pos pos) : base(tier, tile, pos) {
        name = SETTLEMENT;
    }
}
public class RuneGate : MapCell {
    public RuneGate(int tier, Tile tile, Pos pos) : base(tier, tile, pos) {
        name = RUNE_GATE;
    }
}
