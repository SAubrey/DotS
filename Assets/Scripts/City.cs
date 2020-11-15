using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class City : Storeable {
    public const int CITY = 3;

    protected override void Start() {
        base.Start();
        _light = 8;
        _unity = 3;
        ID = CITY;
    }

    public override void light_decay_cascade() {
        Dictionary<string, int> d = new Dictionary<string, int>();
        d.Add(LIGHT, -1);
        if (light <= 0) {
            if (star_crystals > 0) {
                d.Add(STAR_CRYSTALS, -1);
                d[LIGHT] = light_refresh_amount;
            } else if (unity > 0) {
                d.Add(UNITY, -1);
                d[LIGHT] = light_refresh_amount;
            } else if (unity <= 0) {
                // Game over
                // Raise game over window, game over accept button calls game over in Controller.
                MapUI.I.set_active_game_overP(true);
            }
        }
        adjust_resources_visibly(d);
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

        CityUI cui = CityUI.I;
        for (int i = 0; i < cui.upgrades.Count; i++) {
            cui.selected_upgrade_ID = cui.upgrades[i].ID;
            cui.purchase_upgrade();
        }
    }
}  