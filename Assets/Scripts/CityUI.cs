using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class CityUI : MonoBehaviour {
    public static CityUI I { get; private set; }
    public static Color LOCKED_COLOR = new Color(.4f, .4f, .4f, 1);
    public static Color UNLOCKED_COLOR = new Color(.7f, .7f, .7f, 1);
    public static Color PURCHASED_COLOR = new Color(1f, 1f, 1f, 1);
    
    // Astra
    public const int TEMPLE = 1;
    public const int TEMPLE2 = 2;
    public const int CITADEL = 3;
    public const int SHARED_WISDOM = 4;
    public const int MEDITATION = 5;
    public const int SANCTUARY = 6;
    public const int TEMPLE3 = 7;
    public const int HALLOFADEPT = 8;
    public const int FAITHFUL = 9;
    public const int RUNE_PORT = 10;
    public const int CITADEL2 = 11;

    // Endura
    public const int CRAFT_SHOP = 12;
    public const int CRAFT_SHOP2 = 13;
    public const int STOREHOUSE = 14;
    public const int REFINED_STARDUST = 15;
    public const int ENCAMPMENTS = 16;
    public const int STABLE = 17;
    public const int CRAFT_SHOP3 = 18;
    public const int MASTERS_GUILD = 19;
    public const int RESILIENT = 20;
    public const int RESTORE_GREAT_TORCH = 21;
    public const int STOREHOUSE2 = 22;

    // Martial
    public const int FORGE = 23;
    public const int FORGE2 = 24;
    public const int BARRACKS = 25;
    public const int MARTIAL_ORDER = 26;
    public const int STEADY_MARCH = 27;
    public const int GARRISON = 28;
    public const int FORGE3 = 29;
    public const int DOJO_CHOSEN = 30;
    public const int REFINED = 31;
    public const int BOW_ILUHATAR = 32;
    public const int BARRACKS2 = 33;


    // City inventory TextMeshProUGUIs
    public TextMeshProUGUI city_capacityT, bat_capacityT;
    public IDictionary<string, TextMeshProUGUI> city_inv = new Dictionary<string, TextMeshProUGUI>();
    public TextMeshProUGUI c_star_crystals, c_minerals, c_arelics, c_mrelics, c_erelics, c_equimares;
    // Discipline inventory TextMeshProUGUIs
    public IDictionary<string, TextMeshProUGUI> disc_inv = new Dictionary<string, TextMeshProUGUI>();
    public TextMeshProUGUI d_star_crystals, d_minerals, d_arelics, d_mrelics, d_erelics, d_equimares;

    // Unit quantity TextMeshProUGUIs - Hiring
    public IDictionary<int, TextMeshProUGUI> unit_counts = new Dictionary<int, TextMeshProUGUI>();
    public TextMeshProUGUI warriorT, spearmanT, archerT, minerT, inspiratorT, seekerT, guardianT,
        arbalestT, skirmisherT, paladinT, menderT, carterT, dragoonT, scoutT,
        drummerT, shield_maidenT, pikemanT;

    public Dictionary<int, Button> hire_buttons = new Dictionary<int, Button>();
    public Button warriorB, spearmanB, archerB, inspiratorB, minerB, seekerB, guardianB,
        arbalestB, skirmisherB, paladinB, menderB, carterB, dragoonB, scoutB,
        drummerB, shield_maidenB, pikemanB;


    // City Upgrades 
    // Arrange buttons in inspector left to right, top to bottom.
    public GameObject upgradesP;
    public GameObject astra_upgradeP, martial_upgradeP, endura_upgradeP;
    public Button astra_upgradeB, martial_upgradeB, endura_upgradeB;
    public TextMeshProUGUI upgrade_infoT;
    public Dictionary<int, Button> upgrade_buttons = new Dictionary<int, Button>();
    public Button templeB, temple2B, citadelB, shared_wisdomB, meditationB, sanctuaryB,
        temple3B, hallofadeptB, faithfulB, rune_portB, citadel2B;
    public Button craft_shopB, craft_shop2B, storehouseB, refined_stardustB, encampmentsB,
        stableB, craft_shop3B, masters_guildB, resilientB, restore_great_torchB, storehouse2B;
    public Button forgeB, forge2B, barracksB, martial_orderB, steady_marchB, garrisonB, 
        forge3B, dojo_chosenB, refinedB, bow_iluhatarB, barracks2B;

    public Dictionary<int, Upgrade> upgrades = new Dictionary<int, Upgrade>();
    public Button purchaseB;
    public Button equimare_transferB, equimare_transferB2;

    public GameObject cityP;
    public bool visible = false;
    public TextMeshProUGUI infoT;

    public int selected_upgrade_ID;
    void Awake() {
        if (I == null) {
            I = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        city_inv.Add(Storeable.STAR_CRYSTALS, c_star_crystals);
        city_inv.Add(Storeable.MINERALS, c_minerals);
        city_inv.Add(Storeable.ARELICS, c_arelics);
        city_inv.Add(Storeable.MRELICS, c_mrelics);
        city_inv.Add(Storeable.ERELICS, c_erelics);
        city_inv.Add(Storeable.EQUIMARES, c_equimares);

        disc_inv.Add(Storeable.STAR_CRYSTALS, d_star_crystals);
        disc_inv.Add(Storeable.MINERALS, d_minerals);
        disc_inv.Add(Storeable.ARELICS, d_arelics);
        disc_inv.Add(Storeable.MRELICS, d_mrelics);
        disc_inv.Add(Storeable.ERELICS, d_erelics);
        disc_inv.Add(Storeable.EQUIMARES, d_equimares);

        unit_counts.Add(PlayerUnit.WARRIOR, warriorT);
        unit_counts.Add(PlayerUnit.SPEARMAN, spearmanT);
        unit_counts.Add(PlayerUnit.ARCHER, archerT);
        unit_counts.Add(PlayerUnit.MINER, minerT);
        unit_counts.Add(PlayerUnit.INSPIRATOR, inspiratorT);
        unit_counts.Add(PlayerUnit.SEEKER, seekerT);
        unit_counts.Add(PlayerUnit.GUARDIAN, guardianT);
        unit_counts.Add(PlayerUnit.ARBALEST, arbalestT);
        unit_counts.Add(PlayerUnit.SKIRMISHER, skirmisherT);
        unit_counts.Add(PlayerUnit.PALADIN, paladinT);
        unit_counts.Add(PlayerUnit.MENDER, menderT);
        unit_counts.Add(PlayerUnit.CARTER, carterT);
        unit_counts.Add(PlayerUnit.DRAGOON, dragoonT);
        unit_counts.Add(PlayerUnit.SCOUT, scoutT);
        unit_counts.Add(PlayerUnit.DRUMMER, drummerT);
        unit_counts.Add(PlayerUnit.SHIELD_MAIDEN, shield_maidenT);
        unit_counts.Add(PlayerUnit.PIKEMAN, pikemanT);
        cityP.SetActive(visible);

        hire_buttons.Add(PlayerUnit.WARRIOR, warriorB);
        hire_buttons.Add(PlayerUnit.SPEARMAN, spearmanB);
        hire_buttons.Add(PlayerUnit.ARCHER, archerB);
        hire_buttons.Add(PlayerUnit.MINER, minerB);
        hire_buttons.Add(PlayerUnit.INSPIRATOR, inspiratorB);
        hire_buttons.Add(PlayerUnit.SEEKER, seekerB);
        hire_buttons.Add(PlayerUnit.GUARDIAN, guardianB);
        hire_buttons.Add(PlayerUnit.ARBALEST, arbalestB);
        hire_buttons.Add(PlayerUnit.SKIRMISHER, skirmisherB);
        hire_buttons.Add(PlayerUnit.PALADIN, paladinB);
        hire_buttons.Add(PlayerUnit.MENDER, menderB);
        hire_buttons.Add(PlayerUnit.CARTER, carterB);
        hire_buttons.Add(PlayerUnit.DRAGOON, dragoonB);
        hire_buttons.Add(PlayerUnit.SCOUT, scoutB);
        hire_buttons.Add(PlayerUnit.DRUMMER, drummerB);
        hire_buttons.Add(PlayerUnit.SHIELD_MAIDEN, shield_maidenB);
        hire_buttons.Add(PlayerUnit.PIKEMAN, pikemanB);

        //                            SC, M, AR, MR, ER
        upgrades.Add(0, new Upgrade(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
        upgrades[0].purchased = true;
        // Astra
        upgrades.Add(TEMPLE, new Upgrade(TEMPLE, 1, 1, 1, 0, 0, 0, 0, 0, TEMPLE2, CITADEL, 0));
        upgrades.Add(TEMPLE2, new Upgrade(TEMPLE2, 3, 3, 12, 0, 0, TEMPLE, 0, 0, SHARED_WISDOM, MEDITATION, 0));
        upgrades.Add(CITADEL, new Upgrade(CITADEL, 0, 0, 0, 0, 0, TEMPLE, 0, 0, MEDITATION, SANCTUARY, 0));
        upgrades.Add(SHARED_WISDOM, new Upgrade(SHARED_WISDOM, 3, 0, 9, 0, 0, TEMPLE2, 0, 0, TEMPLE3, HALLOFADEPT, FAITHFUL));
        upgrades.Add(MEDITATION, new Upgrade(MEDITATION, 6, 0, 9, 4, 0, TEMPLE2, CITADEL, 0, FAITHFUL, 0, 0));
        upgrades.Add(SANCTUARY, new Upgrade(SANCTUARY, 0, 0, 0, 0, 0, CITADEL, 0, 0, FAITHFUL, RUNE_PORT, CITADEL2));
        upgrades.Add(TEMPLE3, new Upgrade(TEMPLE3, 8, 4, 2, 0, 0, SHARED_WISDOM, 0, 0, 0, 0, 0));
        upgrades.Add(HALLOFADEPT, new Upgrade(HALLOFADEPT, 0, 7, 7, 7, 0, SHARED_WISDOM, 0, 0, 0, 0, 0));
        upgrades.Add(FAITHFUL, new Upgrade(FAITHFUL, 0, 0, 0, 0, 0, SHARED_WISDOM, MEDITATION, SANCTUARY, 0, 0, 0));
        upgrades.Add(RUNE_PORT, new Upgrade(RUNE_PORT, 9, 18, 9, 0, 0, SANCTUARY, 0, 0, 0, 0, 0));
        upgrades.Add(CITADEL2, new Upgrade(CITADEL2, 0, 0, 0, 0, 0, SANCTUARY, 0, 0, 0, 0, 0));

        // Endura
        upgrades.Add(CRAFT_SHOP, new Upgrade(CRAFT_SHOP, 1, 1, 0, 0, 1, 0, 0, 0, CRAFT_SHOP2, STOREHOUSE, 0));
        upgrades.Add(CRAFT_SHOP2, new Upgrade(CRAFT_SHOP2, 0, 6, 1, 2, 6, CRAFT_SHOP, 0, 0, REFINED_STARDUST, ENCAMPMENTS, 0));
        upgrades.Add(STOREHOUSE, new Upgrade(STOREHOUSE, 5, 10, 0, 0, 3, CRAFT_SHOP, 0, 0, ENCAMPMENTS, STABLE, 0));
        upgrades.Add(REFINED_STARDUST, new Upgrade(REFINED_STARDUST, 0, 0, 0, 0, 0, CRAFT_SHOP2, 0, 0, CRAFT_SHOP3, MASTERS_GUILD, RESILIENT));
        upgrades.Add(ENCAMPMENTS, new Upgrade(ENCAMPMENTS, 0, 6, 4, 0, 9, CRAFT_SHOP2, STOREHOUSE, 0, RESILIENT, 0, 0));
        upgrades.Add(STABLE, new Upgrade(STABLE, 3, 3, 0, 3, 6, STOREHOUSE, 0, 0, RESILIENT, RESTORE_GREAT_TORCH, STOREHOUSE2));
        upgrades.Add(CRAFT_SHOP3, new Upgrade(CRAFT_SHOP3, 6, 12, 2, 4, 8, REFINED_STARDUST, 0, 0, 0, 0, 0));
        upgrades.Add(MASTERS_GUILD, new Upgrade(MASTERS_GUILD, 0, 0, 0, 0, 0, REFINED_STARDUST, 0, 0, 0, 0, 0));
        upgrades.Add(RESILIENT, new Upgrade(RESILIENT, 20, 5, 0, 0, 0, REFINED_STARDUST, STABLE, 0, 0, 0, 0));
        upgrades.Add(RESTORE_GREAT_TORCH, new Upgrade(RESTORE_GREAT_TORCH, 14, 20, 7, 0, 7, STABLE, 0, 0, 0, 0, 0));
        upgrades.Add(STOREHOUSE2, new Upgrade(STOREHOUSE2, 5, 20, 0, 0, 8, STABLE, 0, 0, 0, 0, 0));

        // Martial
        upgrades.Add(FORGE, new Upgrade(FORGE, 1, 1, 0, 1, 0, 0, 0, 0, FORGE2, BARRACKS, 0));
        upgrades.Add(FORGE2, new Upgrade(FORGE2, 6, 6, 0, 6, 2, FORGE, 0, 0, MARTIAL_ORDER, STEADY_MARCH, 0));
        upgrades.Add(BARRACKS, new Upgrade(BARRACKS, 0, 0, 0, 0, 0, FORGE, 0, 0, STEADY_MARCH, GARRISON, 0));
        upgrades.Add(MARTIAL_ORDER, new Upgrade(MARTIAL_ORDER, 8, 8, 0, 8, 0, FORGE2, 0, 0, FORGE3, DOJO_CHOSEN, REFINED));
        upgrades.Add(STEADY_MARCH, new Upgrade(STEADY_MARCH, 6, 0, 0, 9, 4, FORGE2, BARRACKS, 0, REFINED, 0, 0));
        upgrades.Add(GARRISON, new Upgrade(GARRISON, 0, 0, 0, 0, 0, BARRACKS, 0, 0, REFINED, BOW_ILUHATAR, BARRACKS2));
        upgrades.Add(FORGE3, new Upgrade(FORGE3, 5, 5, 5, 5, 5, MARTIAL_ORDER, 0, 0, 0, 0, 0));
        upgrades.Add(DOJO_CHOSEN, new Upgrade(DOJO_CHOSEN, 0, 0, 0, 0, 0, MARTIAL_ORDER, 0, 0, 0, 0, 0));
        upgrades.Add(REFINED, new Upgrade(REFINED, 0, 0, 0, 0, 0, MARTIAL_ORDER, STEADY_MARCH, GARRISON, 0, 0, 0));
        upgrades.Add(BOW_ILUHATAR, new Upgrade(BOW_ILUHATAR, 0, 0, 0, 0, 0, GARRISON, 0, 0, 0, 0, 0));
        upgrades.Add(BARRACKS2, new Upgrade(BARRACKS2, 0, 0, 0, 0, 0, GARRISON, 0, 0, 0, 0, 0));

        upgrade_buttons.Add(TEMPLE, templeB);
        upgrade_buttons.Add(TEMPLE2, temple2B);
        upgrade_buttons.Add(CITADEL, citadelB);
        upgrade_buttons.Add(SHARED_WISDOM, shared_wisdomB);
        upgrade_buttons.Add(MEDITATION, meditationB);
        upgrade_buttons.Add(SANCTUARY, sanctuaryB);
        upgrade_buttons.Add(TEMPLE3, temple3B);
        upgrade_buttons.Add(HALLOFADEPT, hallofadeptB);
        upgrade_buttons.Add(FAITHFUL, faithfulB);
        upgrade_buttons.Add(RUNE_PORT, rune_portB);
        upgrade_buttons.Add(CITADEL2, citadel2B);

        upgrade_buttons.Add(CRAFT_SHOP, craft_shopB);
        upgrade_buttons.Add(CRAFT_SHOP2, craft_shop2B);
        upgrade_buttons.Add(STOREHOUSE, storehouseB);
        upgrade_buttons.Add(REFINED_STARDUST, refined_stardustB);
        upgrade_buttons.Add(ENCAMPMENTS, encampmentsB);
        upgrade_buttons.Add(STABLE, stableB);
        upgrade_buttons.Add(CRAFT_SHOP3, craft_shop3B);
        upgrade_buttons.Add(MASTERS_GUILD, masters_guildB);
        upgrade_buttons.Add(RESILIENT, resilientB);
        upgrade_buttons.Add(RESTORE_GREAT_TORCH, restore_great_torchB);
        upgrade_buttons.Add(STOREHOUSE2, storehouse2B);

        upgrade_buttons.Add(FORGE, forgeB);
        upgrade_buttons.Add(FORGE2, forge2B);
        upgrade_buttons.Add(BARRACKS, barracksB);
        upgrade_buttons.Add(MARTIAL_ORDER, martial_orderB);
        upgrade_buttons.Add(STEADY_MARCH, steady_marchB);
        upgrade_buttons.Add(GARRISON, garrisonB);
        upgrade_buttons.Add(FORGE3, forge3B);
        upgrade_buttons.Add(DOJO_CHOSEN, dojo_chosenB);
        upgrade_buttons.Add(REFINED, refinedB);
        upgrade_buttons.Add(BOW_ILUHATAR, bow_iluhatarB);
        upgrade_buttons.Add(BARRACKS2, barracks2B);

        foreach (Discipline d in Controller.I.discs.Values) {
            d.on_resource_change += update_stat_text;
            d.on_capacity_change += register_capacity_change;
        }
        Controller.I.city.on_resource_change += update_stat_text;

        new_game();
    }

    public void new_game() {
        // Reset hire buttons
        foreach (Button b in hire_buttons.Values)
            b.interactable = false;
        unlock_unit_purchase(PlayerUnit.WARRIOR);
        unlock_unit_purchase(PlayerUnit.SPEARMAN);
        unlock_unit_purchase(PlayerUnit.ARCHER);
        unlock_unit_purchase(PlayerUnit.MINER);
        unlock_unit_purchase(PlayerUnit.INSPIRATOR);
        
        // Reset upgrades
        foreach (Button b in upgrade_buttons.Values) {
            set_color(b, false, false);
        }
        set_color(TEMPLE, false, true);
        set_color(CRAFT_SHOP, false, true);
        set_color(FORGE, false, true);

        switch_upgradeP(TurnPhaser.I.active_disc_ID);
        upgradesP.SetActive(false);
    }


    // ---Hire units UI---
    public void update_stat_text(int calling_class, string field, int val, int sum, int capacity) {
        TextMeshProUGUI t = null;
        if (calling_class == City.CITY) {
            city_inv.TryGetValue(field, out t);
            MapUI.update_capacity_text(city_capacityT, sum, capacity);
        } else if (calling_class == TurnPhaser.I.active_disc_ID) {
            disc_inv.TryGetValue(field, out t);
            MapUI.update_capacity_text(bat_capacityT, sum, capacity);
        }
        if (t != null) {
            t.text = val.ToString();
        }
    }

    public void load_unit_counts() {
        foreach (int type in unit_counts.Keys) 
            unit_counts[type].text = 
                TurnPhaser.I.active_disc.bat.units[type].Count.ToString();
    }

    public void try_hire_unit(string args_str) {
        string[] args = args_str.Split(',');
        int type = Int32.Parse(args[0]);
        int sc_cost = Int32.Parse(args[1]);
        int mineral_cost = Int32.Parse(args[2]);

        if (verify_avail_unit_resources(sc_cost, mineral_cost)) {
            TurnPhaser.I.active_disc.bat.add_units(type, 1);
            TurnPhaser.I.active_disc.show_adjustments(
                new Dictionary<string, int>() {
                    {Storeable.STAR_CRYSTALS, -sc_cost},
                    {Storeable.MINERALS, -mineral_cost}
            });
            // Update TextMeshProUGUI in city ui and map ui
            unit_counts[type].text = TurnPhaser.I.active_disc.bat.units[type].Count.ToString();
            MapUI.I.unit_countsT[type].text = unit_counts[type].text;
        }
    }

    public void move_resource_to_city(string type) {
        if (type == Storeable.EQUIMARES && 
            Controller.I.city.get_res(Storeable.EQUIMARES) >= 10) {
            return;
        }
        if (Controller.I.city.get_valid_change_amount(type, 1) != 0 && 
                TurnPhaser.I.active_disc.get_valid_change_amount(type, -1) != 0) {
            Controller.I.city.change_var(type, 1);
            TurnPhaser.I.active_disc.change_var(type, -1);
        }
    }

    public void move_resource_to_disc(string type) {
        if (Controller.I.city.get_valid_change_amount(type, -1) != 0 && 
                TurnPhaser.I.active_disc.get_valid_change_amount(type, 1) != 0) {
            Controller.I.city.change_var(type, -1);
            TurnPhaser.I.active_disc.change_var(type, 1);
        }
    }

    private bool verify_avail_unit_resources(int sc_cost, int mineral_cost) {
        Discipline disc = TurnPhaser.I.active_disc;

        if (disc.get_res(Storeable.STAR_CRYSTALS) >= sc_cost && 
            disc.get_res(Storeable.MINERALS) >= mineral_cost) {
                return true;
        }
        return false;
    }

    private void unlock_unit_purchase(int player_unit) {
        if (hire_buttons.ContainsKey(player_unit)) {
            hire_buttons[player_unit].interactable = true;
        }
    }

    public void register_capacity_change(int ID, int sum, int capacity) {
        if (TurnPhaser.I.active_disc_ID != ID)
            return;
        MapUI.update_capacity_text(city_capacityT, sum, capacity);
    }



    // ---Upgrades UI---
    public void press_upgrade_button(int upgrade_ID) {
        selected_upgrade_ID = upgrade_ID;
        bool purchased = upgrades[upgrade_ID].purchased;
        set_color(upgrade_ID, purchased, requirements_met(upgrade_ID));
        set_active_purchaseB(!purchased && can_purchase_upgrade(upgrade_ID));
        fill_infoT(upgrade_ID);
    }

    private void set_color(int upgrade_ID, bool purchased, bool unlocked=false) {
        Button b = upgrade_buttons[upgrade_ID];
        if (purchased)
            b.image.color = PURCHASED_COLOR;
        else
            b.image.color = unlocked ? UNLOCKED_COLOR : LOCKED_COLOR;
    }

    private void set_color(Button b, bool purchased, bool unlocked=false) {
        if (purchased)
            b.image.color = PURCHASED_COLOR;
        else
            b.image.color = unlocked ? UNLOCKED_COLOR : LOCKED_COLOR;
    }

    private bool can_purchase_upgrade(int upgrade_ID) {
        return can_afford_upgrade(upgrade_ID) && requirements_met(upgrade_ID);
    }

    private bool can_afford_upgrade(int upgrade_ID) {
        Discipline b = TurnPhaser.I.active_disc;
        Upgrade u = upgrades[upgrade_ID];
        if (b.get_res(Storeable.STAR_CRYSTALS) < u.star_crystals ||
            b.get_res(Storeable.MINERALS) < u.minerals ||
            b.get_res(Storeable.ARELICS) < u.arelics ||
            b.get_res(Storeable.MRELICS) < u.mrelics ||
            b.get_res(Storeable.ERELICS) < u.erelics)
            return false;
        return true;
    }

    private bool requirements_met(int upgrade_ID) {
        foreach (int req in upgrades[upgrade_ID].required_unlocks) {
            if (!upgrades.ContainsKey(req))
                continue;
            if (!upgrades[req].purchased)
                return false;
        }
        return true;
    }

    public void purchase_upgrade() {
        int ID = selected_upgrade_ID;
        if (ID <= 0)
            return;
        if (!can_purchase_upgrade(ID)) {
            
            return;
        }
        Upgrade u = upgrades[ID];
        TurnPhaser.I.active_disc.show_adjustments(
            new Dictionary<string, int>() {
                {Storeable.STAR_CRYSTALS, -u.star_crystals},
                {Storeable.MINERALS, -u.minerals},
                {Storeable.ARELICS, -u.arelics},
                {Storeable.MRELICS, -u.mrelics},
                {Storeable.ERELICS, -u.erelics},
            });
        set_color(ID, true);
        upgrades[ID].purchased = true;
        upgrade(ID);
        set_active_purchaseB(false);

        check_upstream_unlocks(ID);
    }

    private void check_upstream_unlocks(int upgrade_ID) {
        // Make available any unlocked upgrades.
        foreach (int req in upgrades[upgrade_ID].required_to_unlock) {
            Upgrade u = upgrades[req];
            if (u.ID != 0 && requirements_met(u.ID)) {
                set_color(u.ID, false, true);
            }
        }
    }

    private void upgrade(int ID) {
        if (is_purchased(FORGE2)) {
            if (is_purchased(STABLE, BARRACKS2))
                unlock_unit_purchase(PlayerUnit.DRAGOON);
            if (is_purchased(CRAFT_SHOP, BARRACKS2))
                unlock_unit_purchase(PlayerUnit.GUARDIAN);
            if (ID == BARRACKS2)
                unlock_unit_purchase(PlayerUnit.SCOUT);
            
        }
        else if (is_purchased(FORGE)) {
            if (ID == BARRACKS) {
                unlock_unit_purchase(PlayerUnit.GUARDIAN);
                unlock_unit_purchase(PlayerUnit.ARBALEST);
                unlock_unit_purchase(PlayerUnit.SKIRMISHER);
            } else if (ID == BARRACKS2) 
                unlock_unit_purchase(PlayerUnit.PALADIN);
            else if (ID == TEMPLE2)
                unlock_unit_purchase(PlayerUnit.MENDER);
            else if (ID == STABLE)
                unlock_unit_purchase(PlayerUnit.CARTER);
            if (is_purchased(CRAFT_SHOP2, TEMPLE)) {
                unlock_unit_purchase(PlayerUnit.DRUMMER);
            }
        }

        if (ID == TEMPLE) {
            unlock_unit_purchase(PlayerUnit.SEEKER);
        }
        if (ID == RUNE_PORT) {
            Map.I.city_cell.has_rune_gate = true;
            Map.I.city_cell.restored_rune_gate = true;
        }
        else if (ID == SHARED_WISDOM) {
            Controller.I.astra.change_var(Storeable.UNITY, 10);
            Controller.I.endura.change_var(Storeable.UNITY, 10);
            Controller.I.martial.change_var(Storeable.UNITY, 10);
        }
        else if (ID == STOREHOUSE) {
            Controller.I.city.capacity += 36;
            MapUI.update_capacity_text(city_capacityT, 
                Controller.I.city.get_sum_storeable_resources(), 108);
            MapUI.update_capacity_text(MapUI.I.city_capacityT,
                Controller.I.city.get_sum_storeable_resources(), 108);
        } 
        else if (ID == STOREHOUSE2) {
            Controller.I.city.capacity += 36;
            MapUI.update_capacity_text(city_capacityT, 
                Controller.I.city.get_sum_storeable_resources(), 144);
            MapUI.update_capacity_text(MapUI.I.city_capacityT,
                Controller.I.city.get_sum_storeable_resources(), 144);
        }
        else if (ID == FAITHFUL) {
            Controller.I.astra.change_var(Storeable.UNITY, 10);
            Controller.I.endura.change_var(Storeable.UNITY, 10);
            Controller.I.martial.change_var(Storeable.UNITY, 10);
        }
        else if (ID == REFINED_STARDUST) {
            Controller.I.astra.light_refresh_amount = 5;
            Controller.I.endura.light_refresh_amount = 5;
            Controller.I.martial.light_refresh_amount = 5;
        }
        else if (ID == STABLE) {
            equimare_transferB.interactable = true;
            equimare_transferB2.interactable = true;
        }
        else if (ID == RESTORE_GREAT_TORCH) {
            Controller.I.city.light_refresh_amount = 11;
        }
        else if (ID == REFINED) {
            
        }
    }

    private void fill_infoT(int upgrade_ID) {
        upgrade_infoT.text = upgrades[upgrade_ID].build_cost_str();
        UpgradeWriter.write_attribute_text(upgrade_infoT, upgrade_ID);
    }

    private void clear_selection() {
        upgrade_infoT.text = "";
        purchaseB.interactable = false;
    }

    private void set_active_purchaseB(bool state) {
        purchaseB.interactable = state;
    }

    private bool is_purchased(int upgrade_ID, int upgrade_ID2=-1) {
        if (upgrade_ID2 > -1)
            return upgrades[upgrade_ID].purchased && upgrades[upgrade_ID2].purchased;
        return upgrades[upgrade_ID].purchased;
    }

    public void switch_upgradeP(int disc) {
        astra_upgradeP.SetActive(false);
        endura_upgradeP.SetActive(false);
        martial_upgradeP.SetActive(false);
        astra_upgradeB.image.color = UNLOCKED_COLOR;
        endura_upgradeB.image.color = UNLOCKED_COLOR;
        martial_upgradeB.image.color = UNLOCKED_COLOR;
        if (disc == Discipline.ASTRA) {
            astra_upgradeP.SetActive(true);
            astra_upgradeB.image.color = PURCHASED_COLOR;
        } else if (disc == Discipline.ENDURA) {
            endura_upgradeP.SetActive(true);
            endura_upgradeB.image.color = PURCHASED_COLOR;
        } else if (disc == Discipline.MARTIAL) {
            martial_upgradeP.SetActive(true);
            martial_upgradeB.image.color = PURCHASED_COLOR;
        }
    }

    public void update_info_text(int punit_ID) {
        AttributeWriter.write_attribute_text(infoT, PlayerUnit.create_punit(punit_ID, -1));
    }

    public void toggle_city_panel() {
        visible = !visible;
        load_unit_counts();
        clear_selection();
        toggle_upgrades_panel();
        cityP.SetActive(visible);
    }

    public void toggle_upgrades_panel() {
        clear_selection();
        upgradesP.SetActive(!upgradesP.activeSelf);
    }
}


public class Upgrade {
    public bool purchased = false;
    public int star_crystals, minerals, arelics, 
        mrelics, erelics = 0;
    public int ID = -1;
    public List<int> required_unlocks = new List<int>();
    public List<int> required_to_unlock = new List<int>();
    public Upgrade(int ID, int sc, int m, 
        int ar, int mr, int er, 
        int needs1, int needs2, int needs3,
        int helps_unlock1, int helps_unlock2, int helps_unlock3) {
        this.ID = ID;
        star_crystals = sc;
        minerals = m;
        arelics = ar;
        mrelics = mr;
        erelics = er;
        required_unlocks.Add(needs1);
        required_unlocks.Add(needs2);
        required_unlocks.Add(needs3);
        required_to_unlock.Add(helps_unlock1);
        required_to_unlock.Add(helps_unlock2);
        required_to_unlock.Add(helps_unlock3);
    }

    public string build_cost_str() {
        string cost = "";
        if (star_crystals > 0)
            cost += "Star Crystal " + star_crystals + ", ";
        if (minerals > 0)
            cost += "Mineral " + minerals + ", ";
        if (arelics > 0)
            cost += "Astra Relic " + arelics + ", ";
        if (mrelics > 0)
            cost += "Martial Relic " + mrelics + ", ";
        if (erelics > 0)
            cost += "Endura Relic " + erelics + ", ";
        if (cost.Length >= 2)
            cost = cost.Remove(cost.Length - 2, 2);
        cost += ". ";
        return cost;
    }
}
