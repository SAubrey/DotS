using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class City : Storeable {
    public const int CITY = 3;

    protected override void Start() {
        base.Start();
        resources[LIGHT] = 8;
        resources[UNITY] = 3;
        ID = CITY;
    }

    public override void light_decay_cascade() {
        Dictionary<string, int> d = new Dictionary<string, int>();
        d.Add(LIGHT, -1);
        if (resources[LIGHT] <= 0) {
            if (resources[STAR_CRYSTALS] > 0) {
                d.Add(STAR_CRYSTALS, -1);
                d[LIGHT] = light_refresh_amount;
            } else if (resources[UNITY] > 0) {
                d.Add(UNITY, -1);
                d[LIGHT] = light_refresh_amount;
            } else if (resources[UNITY] <= 0) {
                // Game over
                // Raise game over window, game over accept button calls game over in Controller.
                MapUI.I.set_active_game_overP(true);
            }
        }
        show_adjustments(d);
    }
    public override void init(bool from_save) {
        base.init(from_save);
        light_refresh_amount = 8;
    }

    public override GameData save() { 
        return new CityData(this, name);
    }

    public override void load(GameData generic) {
        CityData data = generic as CityData;
        resources[LIGHT] = data.sresources.light;
        resources[UNITY] = data.sresources.unity;
        resources[STAR_CRYSTALS] = data.sresources.star_crystals;
        resources[MINERALS] = data.sresources.minerals;
        resources[ARELICS] = data.sresources.arelics;
        resources[ERELICS] = data.sresources.erelics;
        resources[MRELICS] = data.sresources.mrelics;

        CityUI cui = CityUI.I;
        for (int i = 0; i < cui.upgrades.Count; i++) {
            cui.selected_upgrade_ID = cui.upgrades[i].ID;
            cui.purchase_upgrade();
        }
    }
}  