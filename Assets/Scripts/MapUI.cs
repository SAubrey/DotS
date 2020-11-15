using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapUI : MonoBehaviour {
    public static MapUI I { get; private set; }
    public TextMeshProUGUI turn_number_t;

    // ---City UI---
    public GameObject cityP;
    private bool city_panel_active = true;
    public TextMeshProUGUI city_capacityT;
    public TextMeshProUGUI c_light, c_star_crystals, c_minerals, 
        c_arelics, c_mrelics, c_erelics, c_equimares;
    public IDictionary<string, TextMeshProUGUI> city_inv = new Dictionary<string, TextMeshProUGUI>();


    // Battalion Resource UI
    public GameObject invP;
    private bool inv_panel_active = true;
    public TextMeshProUGUI b_light, b_unity, b_experience, b_star_crystals,
         b_minerals, b_arelics, b_mrelics, b_erelics, b_equimares;


    // Battalion Unit UI
    public GameObject unitsP;
    private bool unitsP_active = true;
    public TextMeshProUGUI bat_capacityT;
    public Dictionary<int, TextMeshProUGUI> unit_countsT = new Dictionary<int, TextMeshProUGUI>();
    public TextMeshProUGUI warrior_count, spearman_count, archer_count, 
        miner_count, inspirator_count, seeker_count,
        guardian_count, arbalest_count, skirmisher_count, 
        paladin_count, mender_count, carter_count, dragoon_count,
        scout_count, drummer_count, shield_maiden_count, pikeman_count;

    public IDictionary<string, TextMeshProUGUI> disc_inv = new Dictionary<string, TextMeshProUGUI>();
    public TextMeshProUGUI discT, map_cellT, battle_cellT;
    public Button next_stageB;
    public GameObject ask_to_enterP, game_overP;

    void Awake() {
        if (I == null) {
            I = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        
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

    void Start() {
        set_next_stageB_text(TurnPhaser.I.active_disc_ID);
    }

    void Update() {
        if (CamSwitcher.I.current_cam != CamSwitcher.MAP)
            return;
        if (Input.GetKeyDown(KeyCode.Space) && next_stageB.IsActive()) {
            TurnPhaser.I.end_disciplines_turn();
        } else if (Input.GetKeyDown(KeyCode.X)) {
            Map.I.close_cell_UI();
        }
    }

    public void register_turn() {
        load_battalion_count(Controller.I.get_disc().bat);
        CityUI.I.load_unit_counts();
        highlight_discipline(discT, TurnPhaser.I.active_disc_ID);
        set_next_stageB_text(TurnPhaser.I.active_disc_ID);

        update_storeable_resource_UI(Controller.I.city);
        update_storeable_resource_UI(Controller.I.get_disc());
    }

    public void update_storeable_resource_UI(Storeable s) {
        // Trigger resource property UI updates by setting values to themselves.
        foreach (string resource in Storeable.FIELDS) {
            s.update_text_fields(resource, s.get_var(resource));
        }
    }

    public void load_battalion_count(Battalion b) {
        foreach (int type_ID in b.units.Keys) {
            unit_countsT[type_ID].text = build_unit_text(b, type_ID);
        }
    }

    public string build_unit_text(Battalion b, int ID) {
        if (!unit_countsT.ContainsKey(ID))
            return "";

        string num = b.count_placeable(ID).ToString();
        int total_num = b.units[ID].Count;
        int num_injured = b.count_injured(ID);
        return unit_countsT[ID].text = (total_num - num_injured) + 
            "         " + num_injured;
    }

    public static void update_capacity_text(TextMeshProUGUI text, int sum_resources, int capacity) {
        if (text == null)
            return;
        text.text = sum_resources + " / " + capacity;
    }

    public void update_stat_text(int disc_ID, string field, int val, int sum, int capacity) {
        TextMeshProUGUI t = null;
        if (disc_ID == City.CITY) {
            city_inv.TryGetValue(field, out t);
            MapUI.update_capacity_text(city_capacityT, sum, capacity);
        } else if (disc_ID == TurnPhaser.I.active_disc_ID) {
            disc_inv.TryGetValue(field, out t);
            MapUI.update_capacity_text(bat_capacityT, sum, capacity);
        }
        if (t != null)
            t.text = val.ToString();
    }

    public void highlight_discipline(TextMeshProUGUI txt, int disc_ID) {
        if (disc_ID == Discipline.ASTRA) {
            txt.text = "Astra";
        } else if (disc_ID == Discipline.MARTIAL) {
            txt.text = "Martial";
        } else if (disc_ID == Discipline.ENDURA) {
            txt.text = "Endura";
        }
        txt.color = Statics.disc_colors[disc_ID];
    }

    public void toggle_city_panel() {
        if (Map.I.is_at_city(Controller.I.get_disc())) {
            CityUI.I.toggle_city_panel();
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
/*
    public void set_active_next_stageB(bool state) {
        next_stageB.interactable = state;
    }*/

    public void set_next_stageB_text(int disc_ID) {
        string s = "End ";
        if (disc_ID == Discipline.ASTRA)
            s += "Astra's Turn";
        if (disc_ID == Discipline.MARTIAL)
            s += "Martial's Turn";
        if (disc_ID == Discipline.ENDURA)
            s += "Endura's Turn";
        next_stageB.GetComponentInChildren<TextMeshProUGUI>().text = s;
    }

    public void set_active_ask_to_enterP(bool state) {
        ask_to_enterP.SetActive(state);
    }

    public void set_active_game_overP(bool state) {
        game_overP.SetActive(state);
    }

    public void toggle_units_panel() {
        unitsP_active = !unitsP_active;
        unitsP.SetActive(unitsP_active);
    }
}