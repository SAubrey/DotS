using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour {

    private Color astra_color = new Color(.8f, .8f, 1, 1); // Astra blue
    private Color endura_color = new Vector4(1, 1f, .8f, 1); // Endura orange
    private Color martial_color = new Color(1, .8f, .8f, 1); // Martial red
    
    public Text turn_number_t;

    // ---City UI---
    public GameObject cityP;
    private bool city_panel_active = true;
    public Text c_light;
    public Text c_star_crystals;
    public Text c_minerals;
    public Text c_arelics;
    public Text c_mrelics;
    public Text c_erelics;
    public Text c_equimares;
    public IDictionary<string, Text> city_inv = new Dictionary<string, Text>();

    // Battalion Resource UI
    public GameObject invP;
    private bool inv_panel_active = true;
    public Text b_light;
    public Text b_unity;
    public Text b_experience;
    public Text b_star_crystals;
    public Text b_minerals;
    public Text b_arelics;
    public Text b_mrelics;
    public Text b_erelics;
    public Text b_equimares;

    // Battalion Unit UI
    public GameObject unitsP;
    private bool unitsP_active = true;
    public Text warrior_count;
    public Text spearman_count;
    public Text archer_count;
    public Text miner_count;
    public Text inspirator_count;
    public Dictionary<int, Text> unit_countsT = new Dictionary<int, Text>();

    public Text map_discT;
    public Text battle_discT;
    public Text map_cellT;
    public IDictionary<string, Text> disc_inv = new Dictionary<string, Text>();
    public Button next_stageB;
    public Button rune_gateB;
    

    Controller c;
    void Awake() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        
        // Populate city dictionary
        city_inv.Add(Storeable.LIGHT, c_light);
        city_inv.Add(Storeable.STAR_CRYSTALS, c_star_crystals);
        city_inv.Add(Storeable.MINERALS, c_minerals);
        city_inv.Add(Storeable.ARELICS, c_arelics);
        city_inv.Add(Storeable.MRELICS, c_mrelics);
        city_inv.Add(Storeable.ERELICS, c_erelics);
        city_inv.Add(Storeable.EQUIMARES, c_equimares);

        // Populate batallion dictionary
        disc_inv.Add(Storeable.LIGHT, b_light);
        disc_inv.Add(Storeable.UNITY, b_unity);
        disc_inv.Add(Storeable.EXPERIENCE, b_experience);
        disc_inv.Add(Storeable.STAR_CRYSTALS, b_star_crystals);
        disc_inv.Add(Storeable.MINERALS, b_minerals);
        disc_inv.Add(Storeable.ARELICS, b_arelics);
        disc_inv.Add(Storeable.MRELICS, b_mrelics);
        disc_inv.Add(Storeable.ERELICS, b_erelics);
        disc_inv.Add(Storeable.EQUIMARES, b_equimares);

        unit_countsT.Add(PlayerUnit.WARRIOR, warrior_count);
        unit_countsT.Add(PlayerUnit.SPEARMAN, spearman_count);
        unit_countsT.Add(PlayerUnit.ARCHER, archer_count);
        unit_countsT.Add(PlayerUnit.MINER, miner_count);
        unit_countsT.Add(PlayerUnit.INSPIRATOR, inspirator_count);
    }

    public void load_stats(Storeable s) {
        // Trigger resource property UI updates by non-adjusting values.
        foreach (string resource in Storeable.FIELDS) {
            s.change_var(resource, 0);
        }
        c.city_ui.load_unit_counts();
        highlight_discipline(c.active_disc_ID);
    }

    public void update_stat_text(string field, int calling_class, int val) {
        Text t = null;
        if (calling_class == Controller.CITY) {
            city_inv.TryGetValue(field, out t);
        } else if (calling_class == c.active_disc_ID) {
            disc_inv.TryGetValue(field, out t);
        }
        if (t != null)
            t.text = val.ToString();
    }

    private void highlight_discipline(int discipline) {
        if (discipline == Controller.ASTRA) {
            map_discT.text = "Astra";
            map_discT.color = astra_color;
        } else if (discipline == Controller.MARTIAL) {
            map_discT.text = "Martial";
            map_discT.color = martial_color;
        } else if (discipline == Controller.ENDURA) {
            map_discT.text = "Endura";
            map_discT.color = endura_color;
        }
        battle_discT.text = map_discT.text;
        battle_discT.color = map_discT.color;
    }

    public void toggle_city_panel() {
        if (c.tile_mapper.is_at_city(c.get_disc())) {
            c.city_ui.toggle_city_panel();
        } else {
            city_panel_active = !city_panel_active;
            cityP.SetActive(city_panel_active);
        }
    }

    public void toggle_inv_panel() {
        inv_panel_active = !inv_panel_active;
        invP.SetActive(inv_panel_active);
    }
 
    public void update_cell_text(string tile_name) {
        map_cellT.text = tile_name;
    }

    public void activate_next_stageB(bool state) {
        next_stageB.interactable = state;
    }

    public void activate_rune_gateB(bool state) {
        rune_gateB.interactable = state;
    }

    public void toggle_units_panel() {
        unitsP_active = !unitsP_active;
        unitsP.SetActive(unitsP_active);
    }
}