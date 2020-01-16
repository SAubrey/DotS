using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CityUIManager : MonoBehaviour {

    // City inventory texts
    public Text c_star_crystals;
    public Text c_minerals;
    public Text c_arelics;
    public Text c_mrelics;
    public Text c_erelics;
    public Text c_equimares;
    // Discipline inventory texts
    public Text d_star_crystals;
    public Text d_minerals;
    public Text d_arelics;
    public Text d_mrelics;
    public Text d_erelics;
    public Text d_equimares;

    // Unit quantity texts
    public Text warriorT;
    public Text spearmanT;
    public Text archerT;
    public Text minerT;
    public Text inspiratorT;

    public IDictionary<string, Text> city_inv = new Dictionary<string, Text>();
    public IDictionary<string, Text> disc_inv = new Dictionary<string, Text>();
    public IDictionary<int, Text> units_count = new Dictionary<int, Text>();
    private Controller c;
    public GameObject cityP;
    public bool visible = false;

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();    

        //city_inv.Add(Storeable.LIGHT, c_light);
        city_inv.Add(Storeable.STAR_CRYSTALS, c_star_crystals);
        city_inv.Add(Storeable.MINERALS, c_minerals);
        city_inv.Add(Storeable.ARELICS, c_arelics);
        city_inv.Add(Storeable.MRELICS, c_mrelics);
        city_inv.Add(Storeable.ERELICS, c_erelics);
        city_inv.Add(Storeable.EQUIMARES, c_equimares);

        //disc_inv.Add(Storeable.LIGHT, d_light);
        disc_inv.Add(Storeable.STAR_CRYSTALS, d_star_crystals);
        disc_inv.Add(Storeable.MINERALS, d_minerals);
        disc_inv.Add(Storeable.ARELICS, d_arelics);
        disc_inv.Add(Storeable.MRELICS, d_mrelics);
        disc_inv.Add(Storeable.ERELICS, d_erelics);
        disc_inv.Add(Storeable.EQUIMARES, d_equimares);

        units_count.Add(PlayerUnit.WARRIOR, warriorT);
        units_count.Add(PlayerUnit.SPEARMAN, spearmanT);
        units_count.Add(PlayerUnit.ARCHER, archerT);
        units_count.Add(PlayerUnit.MINER, minerT);
        units_count.Add(PlayerUnit.INSPIRATOR, inspiratorT);
        cityP.SetActive(visible);
    }

    public void update_stat_text(string field, string calling_class, int val) {
        Text t = null;
        if (calling_class == "city") {
            city_inv.TryGetValue(field, out t);
        } else if (calling_class == c.active_disc) {
            disc_inv.TryGetValue(field, out t);
        }
        if (t != null) {
            t.text = val.ToString();
        }
    }

    public void load_unit_counts() {
        foreach (int type in units_count.Keys) {
            units_count[type].text = c.get_active_bat().units[type].Count.ToString();
        }
    }

    public void try_hire_unit(string args_str) {
        string[] args = args_str.Split(',');
        int type = Int32.Parse(args[0]);
        int sc_cost = Int32.Parse(args[1]);;
        int mineral_cost = Int32.Parse(args[2]);;

        if (verify_avail_unit_resources(sc_cost, mineral_cost)) {
            c.get_active_bat().add_units(type, 1);
            c.get_disc().change_var(Storeable.STAR_CRYSTALS, -sc_cost);
            c.get_disc().change_var(Storeable.MINERALS, -mineral_cost);
            // update text in disc
            units_count[type].text = c.get_active_bat().units[type].Count.ToString();
        }
    }

    public void move_resource_to_city(string type) {
        if (c.city.verify_change(type, 1) && 
                c.get_disc().verify_change(type, -1)) {
            c.city.change_var(type, 1);
            c.get_disc().change_var(type, -1);
        }
    }

    public void move_resource_to_disc(string type) {
        if (c.city.verify_change(type, -1) && 
                c.get_disc().verify_change(type, 1)) {
            c.city.change_var(type, -1);
            c.get_disc().change_var(type, 1);
        }
    }

    private bool verify_avail_unit_resources(int sc_cost, int mineral_cost) {
        Discipline disc = c.get_disc();

        if (disc.get_var(Storeable.STAR_CRYSTALS) >= sc_cost && 
            disc.get_var(Storeable.MINERALS) >= mineral_cost) {
                return true;
        }
        return false;
    }

    public void toggle_city_panel() {
        visible = !visible;
        load_unit_counts();
        cityP.SetActive(visible);
    }
}
