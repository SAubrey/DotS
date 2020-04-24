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
    public Text city_capacityT;
    public Text c_light, c_star_crystals, c_minerals, 
        c_arelics, c_mrelics, c_erelics, c_equimares;
    public IDictionary<string, Text> city_inv = new Dictionary<string, Text>();


    // Battalion Resource UI
    public GameObject invP;
    private bool inv_panel_active = true;
    public Text b_light, b_unity, b_experience, b_star_crystals,
         b_minerals, b_arelics, b_mrelics, b_erelics, b_equimares;


    // Battalion Unit UI
    public GameObject unitsP;
    private bool unitsP_active = true;
    public Text bat_capacityT;
    public Dictionary<int, Text> unit_countsT = new Dictionary<int, Text>();
    public Text warrior_count, spearman_count, archer_count, 
        miner_count, inspirator_count, seeker_count,
        guardian_count, arbalest_count, skirmisher_count, 
        paladin_count, mender_count, carter_count, dragoon_count,
        scout_count, drummer_count, shield_maiden_count, pikeman_count;

    Controller c;
    public IDictionary<string, Text> disc_inv = new Dictionary<string, Text>();
    public Text map_discT, battle_discT, map_cellT, battle_cellT;
    public Button next_stageB, rune_gateB, scoutB, mineB;
    public GameObject ask_to_enterP;

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
        unit_countsT.Add(PlayerUnit.SEEKER, seeker_count);
        unit_countsT.Add(PlayerUnit.GUARDIAN, guardian_count);
        unit_countsT.Add(PlayerUnit.ARBALEST, arbalest_count);
        unit_countsT.Add(PlayerUnit.SKIRMISHER, skirmisher_count);
        unit_countsT.Add(PlayerUnit.PALADIN, paladin_count);
        unit_countsT.Add(PlayerUnit.MENDER, mender_count);
        unit_countsT.Add(PlayerUnit.CARTER, carter_count);
        unit_countsT.Add(PlayerUnit.DRAGOON, dragoon_count);
        unit_countsT.Add(PlayerUnit.SCOUT, scout_count);
        unit_countsT.Add(PlayerUnit.DRUMMER, drummer_count);
        unit_countsT.Add(PlayerUnit.SHIELD_MAIDEN, shield_maiden_count);
        unit_countsT.Add(PlayerUnit.PIKEMAN, pikeman_count);
    }

    public void load_stats(Storeable s) {
        // Trigger resource property UI updates by non-adjusting values.
        foreach (string resource in Storeable.FIELDS) {
            s.update_text_fields(resource, s.get_var(resource));
        }
        c.city_ui.load_unit_counts();
        highlight_discipline(c.active_disc_ID);
    }

    
    public static void update_capacity_text(Text text, int sum_resources, int capacity) {
        if (text == null)
            return;
        text.text = sum_resources + " / " + capacity;
    }

    public void update_stat_text(int calling_class, string field, int val, int sum, int capacity) {
        Text t = null;
        if (calling_class == Controller.CITY) {
            city_inv.TryGetValue(field, out t);
            MapUI.update_capacity_text(city_capacityT, sum, capacity);
        } else if (calling_class == c.active_disc_ID) {
            disc_inv.TryGetValue(field, out t);
            MapUI.update_capacity_text(bat_capacityT, sum, capacity);
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
        if (c.map.is_at_city(c.get_disc())) {
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
        battle_cellT.text = tile_name;
    }

    public void set_active_next_stageB(bool state) {
        next_stageB.interactable = state;
    }

    public void set_next_stageB_text(string text) {
        next_stageB.GetComponentInChildren<Text>().text = text;
    }

    public void set_active_ask_to_enterP(bool state) {
        ask_to_enterP.SetActive(state);
    }

    public void set_active_game_lossP(bool state) {
        ask_to_enterP.SetActive(state);
    }

    public void set_active_rune_gateB(bool state) {
        rune_gateB.interactable = state;
    }

    public void toggle_units_panel() {
        unitsP_active = !unitsP_active;
        unitsP.SetActive(unitsP_active);
    }
    
    public void enable_mineB() {
        mineB.interactable = true;
    }

    public void disable_mineB() {
        mineB.interactable = false;
    }

    public void disable_scoutB() {
        scoutB.interactable = false;
    }

    public void enable_scoutB() {
        scoutB.interactable = true;
    }

    public void set_active_mineB(bool state) {
        mineB.interactable = state;
    }

    public void set_active_scoutB(bool state) {
        scoutB.interactable = state;
    }
}