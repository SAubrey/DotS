﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData {
    public string name;
}

interface ISaveLoad {
    GameData save();
    void load(GameData generic);
}

[System.Serializable]
public class ControllerData : GameData {
    public int turn_number;
    public int active_disc;
}

[System.Serializable]
public class MapData : GameData {
    public List<int> t1_bag = new List<int>();
    public List<int> t2_bag = new List<int>();
    public List<int> t3_bag = new List<int>();
    public List<SMapCell> cells = new List<SMapCell>();
    public MapData(TileMapper tm, string name) {
        this.name = name;

        foreach (int num in tm.bags[1]) 
            t1_bag.Add(num);
        foreach (int num in tm.bags[2]) 
            t2_bag.Add(num);
        foreach (int num in tm.bags[3]) 
            t3_bag.Add(num);

        foreach (MapCell mc in tm.map.Values) {
            SMapCell mcs = 
                new SMapCell(mc.tile_ID,
                        mc.pos.x, mc.pos.y, 
                        mc.tier, mc.discovered,
                        mc.minerals, mc.star_crystals);
            cells.Add(mcs);
        }
    }
}

[System.Serializable]
public struct SMapCell {
    public int x, y;
    public int tier;
    public bool discovered;
    public int minerals, star_crystals;
    public int tile_type;
    public SMapCell(int tile_type, int x, int y, 
            int tier, bool discovered,
            int minerals, int star_crystals) {
        this.tile_type = tile_type;
        this.x = x;
        this.y = y;
        this.tier = tier;
        this.discovered = discovered;
        this.minerals = minerals;
        this.star_crystals = star_crystals;
    }
}

[System.Serializable]
public class DisciplineData : GameData {
    public SBattalion sbat;
    public SStoreableResources sresources;
    public float col, row;
    public int redrawn_travel_card_ID;

    public DisciplineData(Discipline disc, string name) {
        this.name = name;
        col = disc.pos.x;
        row = disc.pos.y;
        if (disc.bat.in_battle)
            redrawn_travel_card_ID = disc.get_travelcard().ID;
        sbat = new SBattalion(disc.bat);
        sresources = new SStoreableResources(disc);
    }
}

[System.Serializable]
public struct SStoreableResources {
    public int light, unity, star_crystals, minerals, arelics, erelics, mrelics;
    public SStoreableResources(Storeable s) {
        light = s.light;
        unity = s.unity;
        star_crystals = s.star_crystals;
        minerals = s.minerals;
        arelics = s.arelics;
        erelics = s.erelics;
        mrelics = s.mrelics;
    }
}

[System.Serializable]
public struct SBattalion {
    // Indices refer to the unit type, values refer to the amount.
    public List<int> healthy_types, injured_types;
    public SBattalion(Battalion bat) {
        healthy_types = new List<int>(PlayerUnit.unit_types.Count);
        injured_types = new List<int>(PlayerUnit.unit_types.Count);
        foreach (int type in PlayerUnit.unit_types) {
            int injured = bat.count_injured(type);
            int healthy = bat.units[type].Count - injured;
            healthy_types.Add(healthy);
            injured_types.Add(injured);
        }
    }
}

[System.Serializable]
public class CityData : GameData {
    public SStoreableResources sresources;
    public bool[] purchases;

    public CityData(City city, string name) {
        this.name = name;
        sresources = new SStoreableResources(city);

        CityUIManager cui = city.c.city_ui;
        purchases = new bool[cui.upgrades.Count];
        for (int i = 0; i < cui.upgrades.Count; i++) {
            purchases[i] = cui.upgrades[i].purchased;
        }   
    }
}

[System.Serializable]
public class TravelDeckData : GameData {

    public List<int> t1_deck = new List<int>();
    public List<int> t2_deck = new List<int>();
    public List<int> t3_deck = new List<int>();
    public TravelDeckData(TravelDeck td, string name) {
        this.name = name;
        foreach (int card_type in td.decks[1]) 
            t1_deck.Add(card_type);
        foreach (int card_type in td.decks[2]) 
            t2_deck.Add(card_type);
        foreach (int card_type in td.decks[3]) 
            t3_deck.Add(card_type);
        
        
    }
}