﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class City : Storeable {

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        map_ui = c.map_ui;
        city_ui = c.city_ui;
        
        _light = 8;
        ID = Controller.CITY;
    }

    public override void new_game() {
        base.new_game();
        light_refresh_amount = 8;
    }

    public override GameData save() { 
        return new CityData(this, name);
    }

    public override void load(GameData generic) {
        CityData data = generic as CityData;
        _light = data.sresources.light;
        _unity = data.sresources.unity;
        _star_crystals = data.sresources.star_crystals;
        _minerals = data.sresources.minerals;
        _arelics = data.sresources.arelics;
        _erelics = data.sresources.erelics;
        _mrelics = data.sresources.mrelics;

        CityUIManager cui = c.city_ui;
        for (int i = 0; i < cui.upgrades.Count; i++) {
            cui.selected_upgrade_ID = cui.upgrades[i].ID;
            cui.purchase_upgrade();
        }
    }
}  