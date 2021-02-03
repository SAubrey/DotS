using System.Collections;
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
public class TurnPhaserData : GameData {
    public int turn;
    public int active_disc_ID;
}

[System.Serializable]
public class MapData : GameData {
    public List<int> t1_bag = new List<int>();
    public List<int> t2_bag = new List<int>();
    public List<int> t3_bag = new List<int>();
    public List<SMapCell> cells = new List<SMapCell>();
    public MapData(Map map, string name) {
        this.name = name;

        foreach (int num in map.bags[1]) 
            t1_bag.Add(num);
        foreach (int num in map.bags[2]) 
            t2_bag.Add(num);
        foreach (int num in map.bags[3]) 
            t3_bag.Add(num);

        foreach (MapCell mc in map.map.Values) {
            SMapCell mcs = 
                new SMapCell(mc.ID,
                        mc.pos.x, mc.pos.y, 
                        mc.tier, mc.discovered,
                        mc.minerals, mc.star_crystals);
            cells.Add(mcs);
        }
    }
}

[System.Serializable]
public struct SMapCell {
    public int ID;
    public int x, y;
    public int tier;
    public bool discovered;
    public int minerals, star_crystals;
    public SMapCell(int ID, int x, int y, 
            int tier, bool discovered,
            int minerals, int star_crystals) {
        this.ID = ID;
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
        light = s.get_res(Storeable.LIGHT);
        unity = s.get_res(Storeable.UNITY);
        star_crystals = s.get_res(Storeable.STAR_CRYSTALS);
        minerals = s.get_res(Storeable.MINERALS);
        arelics = s.get_res(Storeable.ARELICS);
        erelics = s.get_res(Storeable.ERELICS);
        mrelics = s.get_res(Storeable.MRELICS);
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

        CityUI cui = CityUI.I;
        purchases = new bool[cui.upgrades.Count];
        for (int i = 0; i < cui.upgrades.Count; i++) {
            purchases[i] = cui.upgrades[i].purchased;
        }   
    }
}