using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MapUI : MonoBehaviour {
    public static MapUI I { get; private set; }
    public TextMeshProUGUI turn_number_t;

    // ---City UI---
    public GameObject cityP;
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
    public GameObject ask_to_enterP, game_overP, travel_cardP;
    public TextMeshProUGUI travelcard_descriptionT, travelcard_typeT, 
        travelcard_subtextT, travelcard_consequenceT;
    public Image discI;
    public Sprite astraI, martialI, enduraI;
    public Button travelcard_continueB, travelcard_rollB, travelcard_exitB;

    public GameObject cell_UI_prefab, dropped_XP_prefab;
    public MapCellUI open_cell_UI_script;
    public MapCell last_open_cell;
    public GraphicRaycaster graphic_raycaster;
    public Canvas canvas;
    public GameObject map_canvas;
    public GameObject map_cell_light_prefab, map_cell_sparkles_prefab,
        map_cell_fog_prefab;
    private int max_discovered_tile_distance = 0;
    public UnityEngine.Experimental.Rendering.Universal.Light2D city_light;
    public Color star_light_color, titrum_light_color, rune_gate_light_color,
        forest_light_color, lush_land_color, cave_color, mountain_color, plains_color,
        ruins_color;
    public Dictionary<int, Color> cell_light_colors;

    void Awake() {
        if (I == null) {
            I = this;
        } else {
            Destroy(gameObject);
        }

        cell_light_colors = new Dictionary<int, Color>() {
            {MapCell.STAR_ID, star_light_color},
            {MapCell.TITRUM_ID, titrum_light_color},
            {MapCell.RUNE_GATE_ID, rune_gate_light_color},
            {MapCell.FOREST_ID, forest_light_color},
            {MapCell.LUSH_LAND_ID, lush_land_color},
            {MapCell.CAVE_ID, cave_color},
            {MapCell.MOUNTAIN_ID, mountain_color},
            {MapCell.PLAINS_ID, plains_color},
            //{MapCell.SETTLEMENT_ID, settlement_color},
            {MapCell.RUINS_ID, ruins_color},
        };
        
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
        disc_inv.Add(Discipline.EXPERIENCE, b_experience);
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
        set_active_travelcard_continueB(true);
        set_active_travelcard_rollB(false);
        TurnPhaser.I.on_disc_change += register_disc_change;
        TurnPhaser.I.on_turn_change += register_turn;
        

        foreach (Discipline d in Controller.I.discs.Values) {
            d.on_resource_change += update_stat_text;
            d.on_capacity_change += register_capacity_change;
            d.on_unit_count_change += load_battalion_count;
        }
        Controller.I.city.on_resource_change += update_stat_text;
    }

    void Update() {
        if (CamSwitcher.I.current_cam != CamSwitcher.MAP)
            return;

        // Oscillate tile colors
        foreach (MapCell mc in Map.I.oscillating_cells) {
            mc.oscillate_color();
        }
        
        if (!Input.anyKeyDown)
            return;

        // Process left mouse click
        if (Input.GetMouseButtonDown(0)) {
            handle_left_click();
            return;
        }

        // Decide whether to advance or close windows.
        bool advance = Input.GetKeyDown(KeyCode.Space);
        bool exit = Input.GetKeyDown(KeyCode.X);
        bool UIopen = CityUI.I.cityP.activeSelf || cell_UI_is_open || 
            game_overP.activeSelf || travel_cardP.activeSelf;
        if (advance) {
            if (next_stageB.IsActive() && !UIopen) {
                TurnPhaser.I.end_disciplines_turn();
            }

        } else if (exit && UIopen) {
            if (cell_UI_is_open)
                close_cell_UI();
            else if (CityUI.I.upgradesP.activeSelf) {
                CityUI.I.toggle_upgrades_panel();
            } else if (CityUI.I.cityP.activeSelf) {
                CityUI.I.toggle_city_panel();
            }
        }
    }

    public void register_disc_change(Discipline d) {
        load_battalion_count();
        CityUI.I.load_unit_counts();
        highlight_discipline(discT, discI, d.ID);
        set_next_stageB_text(d.ID);

        update_storeable_resource_UI(Controller.I.city);
        update_storeable_resource_UI(d);
    }

    public void register_turn() {
        turn_number_t.text = TurnPhaser.I.turn.ToString();
    }

    public void register_capacity_change(int ID, int sum, int capacity) {
        if (TurnPhaser.I.active_disc_ID != ID)
            return;
        update_capacity_text(bat_capacityT, sum, capacity);
    }
    
    public void adjust_light_size(MapCell cell) {
        int dist = Statics.calc_map_distance(cell.pos, new Pos(10, 10));
        Debug.Log("distance from city: " + dist);
        if (dist > max_discovered_tile_distance) {
            max_discovered_tile_distance = dist;
            city_light.pointLightInnerRadius = dist;
            city_light.pointLightOuterRadius = dist + 3;
        }
    }

    public void place_cell_light(MapCell cell) {
        if (cell == Map.I.city_cell || cell == null)
            return;
        GameObject light = Instantiate(map_cell_light_prefab);
        light.transform.SetParent(map_canvas.transform);
        Vector3 p = cell.pos.to_vec3;
        light.transform.position = new Vector3(p.x + 0.5f, p.y + 0.5f, 0);
        light.GetComponent<MapCellLight>().init(cell);
    }

    public void place_sparkle_ps(MapCell cell) {
        if (cell == null)
            return;
        GameObject ps = Instantiate(map_cell_sparkles_prefab);
        ps.transform.SetParent(map_canvas.transform);
        Vector3 p = cell.pos.to_vec3;
        ps.transform.position = new Vector3(p.x + 0.5f, p.y + 0.5f, 0);
    }

    public GameObject place_fog_ps(MapCell cell) {
        if (cell == null)
            return null;
        GameObject fog = Instantiate(map_cell_fog_prefab);
        fog.transform.SetParent(map_canvas.transform);
        Vector3 p = cell.pos.to_vec3;
        fog.transform.position = new Vector3(p.x + 0.5f, p.y + 0.5f, 0);
        return fog;
    }

    public void update_storeable_resource_UI(Storeable s) {
        // Trigger resource property UI updates by setting values to themselves.
        foreach (string resource in s.resources.Keys) {
            update_stat_text(s.ID, resource, s.get_res(resource), s.get_sum_storeable_resources(), s.capacity);
        }
    }

    
    private void handle_left_click() {
        PointerEventData m_PointerEventData = new PointerEventData(EventSystem.current);
        m_PointerEventData.position = Input.mousePosition;
        List<RaycastResult> objects = new List<RaycastResult>();

        graphic_raycaster.Raycast(m_PointerEventData, objects);
        // Close the open cell window if clicking anywhere other than on the window.
        // (The tilemap does not register as a game object)
        bool hit_cell_window = false;
        bool hit_window = false;
        foreach (RaycastResult o in objects) {
            if (o.gameObject.tag == "Cell Window") {
                hit_cell_window = true;
                hit_window = true;
                continue;
            }
            else if (o.gameObject.tag == "MapWindow") {
                hit_window = true;
                continue;
            }
        }
        if (EquipmentUI.I.selecting) {
            EquipmentUI.I.selecting = false;
            hit_window = true;
        }

        Vector3 pos = CamSwitcher.I.map_cam.ScreenToWorldPoint(Input.mousePosition);
        bool over_tile = Map.I.get_tile(pos.x, pos.y) != null;
        bool forced_tc_interaction = (travel_cardP.activeSelf && !travelcard_exitB.interactable) 
            || ask_to_enterP.activeSelf;

        if (forced_tc_interaction)
            return;
        
        if (!hit_cell_window) {
            close_cell_UI();

            if (objects.Count <= 0 && !hit_window) {
                close_travelcardP();

                if (over_tile) {
                    Map.I.get_cell(pos).discover(); // DEBUG
                    generate_cell_UI(Map.I.get_cell(pos));
                }

            }
        }
    }

    private void generate_cell_UI(MapCell cell) {
        GameObject cell_UI = Instantiate(cell_UI_prefab, canvas.transform);
        open_cell_UI_script = cell_UI.GetComponentInChildren<MapCellUI>();
        last_open_cell = cell;
        open_cell_UI_script.init(cell);
    }
    
    public void close_cell_UI() {
        if (!cell_UI_is_open) 
            return;
        open_cell_UI_script.close();
        open_cell_UI_script = null;
    }

    public bool cell_UI_is_open { get => open_cell_UI_script != null; }

    public void load_battalion_count() {
        Battalion b = TurnPhaser.I.active_disc.bat;
        if (b == null)
            return;
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
            update_capacity_text(city_capacityT, sum, capacity);
        } else if (disc_ID == TurnPhaser.I.active_disc_ID) {
            disc_inv.TryGetValue(field, out t);
            update_capacity_text(bat_capacityT, sum, capacity);
        }
        if (t != null)
            t.text = val.ToString();
    }

    public void highlight_discipline(TextMeshProUGUI txt, Image img, int disc_ID) {
        Sprite s = null;
        string t = "";
        if (disc_ID == Discipline.ASTRA) {
            t = "Astra";
            s = astraI;
        } else if (disc_ID == Discipline.MARTIAL) {
            t = "Martial";
            s = martialI;
        } else if (disc_ID == Discipline.ENDURA) {
            t = "Endura";
            s = enduraI;
        }
        txt.text = t;
        if (img != null)
            img.sprite = s;
        txt.color = Statics.disc_colors[disc_ID];
    }

    public void toggle_city_panel() {
        // If the battalion is at the city, toggle the main city panel.
        if (Map.I.is_at_city(TurnPhaser.I.active_disc)) {
            CityUI.I.toggle_city_panel();
        } else {
            cityP.SetActive(!cityP.activeSelf);
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

    private void set_next_stageB_text(int disc_ID) {
        string s = "End ";
        if (disc_ID == Discipline.ASTRA)
            s += "Astra's Turn";
        if (disc_ID == Discipline.MARTIAL)
            s += "Martial's Turn";
        if (disc_ID == Discipline.ENDURA)
            s += "Endura's Turn";
        next_stageB.GetComponentInChildren<TextMeshProUGUI>().text = s;
    }

    public void display_travelcard(TravelCard tc) {
        if (tc == null || travel_cardP.activeSelf)
            return;
        bool active_disc_at_selected_cell = 
            TurnPhaser.I.active_disc.cell == open_cell_UI_script.cell;
        bool interactable = !tc.complete && active_disc_at_selected_cell;
        set_active_travelcard_continueB(interactable);
        set_active_travelcard_rollB(interactable && tc.die_num_sides > 0 && !tc.rolled);
        set_active_travelcard_exitB(!interactable);
        set_active_next_stageB(false);

        tc.on_open(TravelCardManager.I);

        // Update text
        travelcard_typeT.text = tc.type_text;
        travelcard_descriptionT.text = tc.description;
        travelcard_subtextT.text = tc.subtext;
        travelcard_consequenceT.text = tc.consequence_text;
        travel_cardP.SetActive(true);
    }

    public void close_travelcardP() {
        set_active_ask_to_enterP(false);
        set_active_travelcard_continueB(false);
        set_active_travelcard_rollB(false);
        travel_cardP.SetActive(false);
        set_active_next_stageB(true);
    }

    public void toggle_travelcard(TravelCard tc) {
        if (travel_cardP.activeSelf) {
            close_travelcardP();
        } else {
            display_travelcard(tc);
        }
    }
    
    public void set_active_travelcard_rollB(bool state) {
        travelcard_rollB.interactable = state;
        TextMeshProUGUI t = travelcard_rollB.GetComponentInChildren<TextMeshProUGUI>();
        t.text = state ? "Roll" : "";
    }

    public void set_active_travelcard_exitB(bool state) {
        travelcard_exitB.interactable = state;
    }
    
    public void set_active_travelcard_continueB(bool state) {
        travelcard_continueB.interactable = state;
    }

    public void set_active_ask_to_enterP(bool state) {
        ask_to_enterP.SetActive(state);
    }

    public void set_active_game_overP(bool state) {
        game_overP.SetActive(state);
    }

    // Button driven
    public void toggle_units_panel() {
        unitsP_active = !unitsP_active;
        unitsP.SetActive(unitsP_active);
    }

    public void set_active_next_stageB(bool state) {
        next_stageB.interactable = state;
    }
}