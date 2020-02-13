using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class City : Storeable {

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        map_ui = c.map_ui;
        city_ui = c.city_ui;
        _light = 8;
    }

    public override GameData save() { 
        return new CityData(this, name);
    }

    public override void load(GameData generic) {
        CityData data = generic as CityData;
        light = data.sresources.light;
        unity = data.sresources.unity;
        star_crystals = data.sresources.star_crystals;
        minerals = data.sresources.minerals;
        arelics = data.sresources.arelics;
        erelics = data.sresources.erelics;
        mrelics = data.sresources.mrelics;

        CityUIManager cui = c.city_ui;
        for (int i = 0; i < cui.upgrades.Count; i++) {
            cui.purchase_upgrade(cui.upgrades[i].ID);
        }
    }
}  