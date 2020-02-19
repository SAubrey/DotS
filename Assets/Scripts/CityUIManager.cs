using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CityUIManager : MonoBehaviour {
    
    public const int FORGE = 1;
    public const int TEMPLE = 2;
    public const int WORKSHOP = 3;
    public const int FORGE2 = 4;
    public const int BARRACKS = 5;
    public const int TEMPLE2 = 6;
    public const int MEDITATION = 7;
    public const int STOREHOUSE = 8;
    public const int WORKSHOP2 = 9;
    public const int ENCAMPMENTS = 10;
    public const int BARRACKS2 = 11;
    public const int HALLOFADEPT = 12;
    public const int TEMPLE3 = 13;
    public const int GARRISON = 14;
    public const int STABLE = 15;


    // City inventory texts
    public IDictionary<string, Text> city_inv = new Dictionary<string, Text>();
    public Text c_star_crystals, c_minerals, c_arelics, c_mrelics, c_erelics, c_equimares;
    // Discipline inventory texts
    public IDictionary<string, Text> disc_inv = new Dictionary<string, Text>();
    public Text d_star_crystals, d_minerals, d_arelics, d_mrelics, d_erelics, d_equimares;

    // Unit quantity texts - Hiring
    public IDictionary<int, Text> unit_counts = new Dictionary<int, Text>();
    public Text warriorT, spearmanT, archerT, minerT, inspiratorT, seekerT, guardianT,
        arbalestT, skirmisherT, paladinT, menderT, carterT, dragoonT, scoutT,
        drummerT, shield_maidenT, pikemanT;

    // City Upgrades 
    // Arrange buttons in inspector left to right, top to bottom.
    public List<Button> upgrade_buttons = new List<Button>();

    public Dictionary<int, Button> hire_buttons = new Dictionary<int, Button>();
    public Button warriorB, spearmanB, archerB, inspiratorB, minerB, seekerB, guardianB,
        arbalestB, skirmisherB, paladinB, menderB, carterB, dragoonB, scoutB,
        drummerB, shield_maidenB, pikemanB;
    public Dictionary<int, Upgrade> upgrades = new Dictionary<int, Upgrade>();
    private Controller c;
    public GameObject cityP;
    public GameObject upgradesP;
    public bool visible = false;
    public Text infoT;

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();    

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

        //                                     sc, M, A, MR, E
        upgrades.Add(0, new Upgrade(0, 0, 0, 0, 0));
        upgrades[0].purchased = true;
        upgrades.Add(FORGE, new Upgrade(FORGE, 1, 1, 0, 1, 0, 0, 0, FORGE2, BARRACKS));
        upgrades.Add(TEMPLE, new Upgrade(TEMPLE, 1, 1, 1, 0, 0, 0, 0, TEMPLE2, MEDITATION));
        upgrades.Add(WORKSHOP, new Upgrade(WORKSHOP, 1, 1, 0, 0, 1, 0, 0, STOREHOUSE, WORKSHOP2));
        upgrades.Add(FORGE2, new Upgrade(FORGE2, 6, 6, 0, 6, 2, FORGE, 0, ENCAMPMENTS, 0));
        upgrades.Add(BARRACKS, new Upgrade(BARRACKS, 2, 4, 0, 2, 0, FORGE, 0, BARRACKS, 0));
        upgrades.Add(TEMPLE2, new Upgrade(TEMPLE2, 3, 3, 12, 0, 0, TEMPLE, 0, HALLOFADEPT, 0));
        upgrades.Add(MEDITATION, new Upgrade(MEDITATION, 6, 0, 9, 4, 0, TEMPLE, 0, TEMPLE3, 0));
        upgrades.Add(STOREHOUSE, new Upgrade(STOREHOUSE, 5, 10, 0, 0, 3, WORKSHOP, 0, GARRISON, 0));
        upgrades.Add(WORKSHOP2, new Upgrade(WORKSHOP2, 0, 9, 1, 2, 6, WORKSHOP, 0, STABLE, 0));

        foreach (Button b in hire_buttons.Values)
            b.interactable = false;
        
        unlock_unit_purchase(PlayerUnit.WARRIOR);
        unlock_unit_purchase(PlayerUnit.SPEARMAN);
        unlock_unit_purchase(PlayerUnit.ARCHER);
        unlock_unit_purchase(PlayerUnit.MINER);
        unlock_unit_purchase(PlayerUnit.INSPIRATOR);

        upgradesP.SetActive(false);
    }


    // ---Hire units UI---
    public void update_stat_text(string field, int caller, int val) {
        Text t = null;
        if (caller == Controller.CITY) {
            city_inv.TryGetValue(field, out t);
        } else if (caller == c.active_disc_ID) {
            disc_inv.TryGetValue(field, out t);
        }
        if (t != null) {
            t.text = val.ToString();
        }
    }

    public void load_unit_counts() {
        foreach (int type in unit_counts.Keys) 
            unit_counts[type].text = 
                c.get_active_bat().units[type].Count.ToString();
    }

    public void try_hire_unit(string args_str) {
        string[] args = args_str.Split(',');
        int type = Int32.Parse(args[0]);
        int sc_cost = Int32.Parse(args[1]);
        int mineral_cost = Int32.Parse(args[2]);

        if (verify_avail_unit_resources(sc_cost, mineral_cost)) {
            c.get_active_bat().add_units(type, 1);
            c.get_disc().change_var(Storeable.STAR_CRYSTALS, -sc_cost);
            c.get_disc().change_var(Storeable.MINERALS, -mineral_cost);
            // Update text in city ui and map ui
            unit_counts[type].text = c.get_active_bat().units[type].Count.ToString();
            c.map_ui.unit_countsT[type].text = unit_counts[type].text;
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

    private void unlock_unit_purchase(int player_unit) {
        if (hire_buttons.ContainsKey(player_unit))
            hire_buttons[player_unit].interactable = true;
    }


    // ---Upgrades UI---
    public void purchase_upgrade(int upgrade_ID) {
        if (!can_purchase_upgrade(upgrade_ID))
            return;

        toggle_upgrade_B(upgrade_ID, false);
        upgrades[upgrade_ID].purchased = true;
        upgrade(upgrade_ID);

        check_upstream_unlocks(upgrade_ID);
    }

    private void check_upstream_unlocks(int upgrade_ID) {
        // Make available any unlocked upgrades.
        foreach (int req in upgrades[upgrade_ID].required_to_unlock) {
            Upgrade u = upgrades[req];
            if (requirements_met(u.ID)) {
                toggle_upgrade_B(u.ID, true);
            }
        }
    }

    private void upgrade(int ID) {
        if (is_purchased(FORGE2)) {
            if (is_purchased(STABLE, BARRACKS2))
                unlock_unit_purchase(PlayerUnit.DRAGOON);
            if (is_purchased(WORKSHOP, BARRACKS2))
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
            if (is_purchased(WORKSHOP2, TEMPLE)) {
                unlock_unit_purchase(PlayerUnit.DRUMMER);
            }
        }
        if (ID == TEMPLE) {
            unlock_unit_purchase(PlayerUnit.SEEKER);
        }
    }

    private bool can_purchase_upgrade(int upgrade_ID) {
        return can_afford_upgrade(upgrade_ID) && requirements_met(upgrade_ID);
    }

    private bool can_afford_upgrade(int upgrade_ID) {
        Discipline b = c.get_disc();
        Upgrade u = upgrades[upgrade_ID];
        if (b.get_var(Storeable.STAR_CRYSTALS) < u.star_crystals ||
            b.get_var(Storeable.MINERALS) < u.minerals ||
            b.get_var(Storeable.ARELICS) < u.arelics ||
            b.get_var(Storeable.MRELICS) < u.mrelics ||
            b.get_var(Storeable.ERELICS) < u.erelics)
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

    public void toggle_upgrade_B(int upgrade_ID, bool active) {
        int ID = upgrade_ID == 0 ? 0 : upgrade_ID - 1;
        upgrade_buttons[ID].interactable = active;
    }

    public void toggle_city_panel() {
        visible = !visible;
        load_unit_counts();
        cityP.SetActive(visible);
    }

    public void toggle_upgrades_panel() {
        upgradesP.SetActive(!upgradesP.activeSelf);
    }

    private bool is_purchased(int upgrade_ID, int upgrade_ID2=-1) {
        if (upgrade_ID2 > -1)
            return upgrades[upgrade_ID].purchased && upgrades[upgrade_ID2].purchased;
        return upgrades[upgrade_ID].purchased;
    }

    public void update_info_text(int punit_ID) {
        AttributeWriter.write_attribute_text(infoT, PlayerUnit.create_punit(punit_ID));
    }
}


public class Upgrade {
    public bool purchased = false;
    public int star_crystals, minerals, arelics, 
        mrelics, erelics = 0;
    public int ID = -1;
    public List<int> required_unlocks = new List<int>();
    public List<int> required_to_unlock = new List<int>();
    public Upgrade(int ID, int sc=0, int m=0, 
        int ar=0, int mr=0, int er=0, 
        int R1=0, int R2=0, int RT1=0, int RT2=0) {
        this.ID = ID;
        star_crystals = sc;
        minerals = m;
        arelics = ar;
        mrelics = mr;
        erelics = er;
        required_unlocks.Add(R1);
        required_unlocks.Add(R2);
        required_to_unlock.Add(RT1);
        required_to_unlock.Add(RT2);
    }
}
