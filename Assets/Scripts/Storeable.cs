using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Storeable : MonoBehaviour, ISaveLoad {
    public const string LIGHT = "light";
    public const string UNITY = "unity";
    
    public const string STAR_CRYSTALS = "star crystals";
    public const string MINERALS = "minerals";
    public const string ARELICS = "Astra relics";
    public const string MRELICS = "Martial relics";
    public const string ERELICS = "Endura relics";
    public const string EQUIMARES = "equimares";

    public static string[] FIELDS = { LIGHT, UNITY, STAR_CRYSTALS,
                            MINERALS, ARELICS, MRELICS, ERELICS, EQUIMARES };
    public Dictionary<string, int> resources = new Dictionary<string, int>() {
        {LIGHT, 4},
        {UNITY, 10},
        {STAR_CRYSTALS, 0},
        {MINERALS, 0},
        {ARELICS, 0},
        {MRELICS, 0},
        {ERELICS, 0},
        {EQUIMARES, 0},
    };
    public GameObject rising_info_prefab;
    public GameObject origin_of_rise_obj;
    public GameObject map_UI_canvas;
    public int ID;
    public const int INITIAL_CAPACITY = 72;
    private int _capacity = INITIAL_CAPACITY;
    public int capacity {
        get { return _capacity; } 
        set {
            _capacity = value;
            if (on_capacity_change != null)
                on_capacity_change(ID, get_sum_storeable_resources(), value);
        }
    }
    public const int INITIAL_LIGHT_REFRESH_AMOUNT = 4;
    public int light_refresh_amount = INITIAL_LIGHT_REFRESH_AMOUNT;

    public event Action<int, string, int, int, int> on_resource_change; // Disc ID, res type, new amount
    public event Action<int, int, int> on_capacity_change; 
    protected List<Adjustment> adjustments = new List<Adjustment>();

    protected virtual void Start() {
        map_UI_canvas = GameObject.Find("MapUICanvas");
        TurnPhaser.I.on_turn_change += register_turn;

        Controller.I.init += init;
    }

    float time_elapsed = 0f;
    float time_interval = 0.5f;

    void Update() {
        /* Display adjusted resources in a timely sequence. */
        if (adjustments.Count == 0) {
            return;
        }

        time_elapsed += Time.deltaTime;
        if (time_elapsed >= time_interval) {
            time_elapsed = 0;

            Adjustment a = adjustments[0];
            adjustments.Remove(a);
            if (resources.ContainsKey(a.resource)) {
                change_var(a.resource, a.amount, true);
            } else {
                Statics.create_rising_info_map(
                    RisingInfo.build_resource_text(a.resource, a.amount),
                    Statics.disc_colors[ID],
                    origin_of_rise_obj.transform, 
                    rising_info_prefab);
            }
            UIPlayer.I.play(UIPlayer.INV_IN);
            // Allow next adjustment to happen instantly.
            if (adjustments.Count == 0) {
                time_elapsed = time_interval;
            }
        }
    }

    public virtual GameData save() { return null; }
    public virtual void load(GameData generic) { }

    public virtual void init(bool from_save) {
        capacity = INITIAL_CAPACITY;
        light_refresh_amount = INITIAL_LIGHT_REFRESH_AMOUNT;
    }

    public virtual void register_turn() {
        light_decay_cascade();
    }

    public virtual void light_decay_cascade() {
        Dictionary<string, int> d = new Dictionary<string, int>();
        d.Add(LIGHT, -1);
        if (resources[LIGHT] <= 0) {
            if (resources[STAR_CRYSTALS] > 0) {
                d.Add(STAR_CRYSTALS, -1);
                d[LIGHT] = light_refresh_amount;
            } else {
                if (resources[UNITY] >= 2)
                    d.Add(UNITY, -2);
                else if (resources[UNITY] == 1) // Can't have negative Unity.
                    d.Add(UNITY, -1);
            }
        }
        show_adjustments(d);
    }

    public void show_adjustments(Dictionary<string, int> adjs) {
        foreach (KeyValuePair<string, int> a in adjs) {
            if (a.Value != 0) {
                adjustments.Add(new Adjustment(a.Key, a.Value));
            }
        }
    }

    public void show_adjustment(string var, int val) {
        Dictionary<string, int> d = new Dictionary<string, int>();
        d.Add(var, val);
        show_adjustments(d);
    }

    protected void remove_resources_lost_on_death() {
        Dictionary<string, int> adjs = new Dictionary<string, int>();
        adjs.Add(STAR_CRYSTALS, -resources[STAR_CRYSTALS]);
        adjs.Add(MINERALS, -resources[MINERALS]);
        adjs.Add(ARELICS, -resources[ARELICS]);
        adjs.Add(MRELICS, -resources[MRELICS]);
        adjs.Add(ERELICS, -resources[ERELICS]);
        adjs.Add(EQUIMARES, -resources[EQUIMARES]);

        resources[LIGHT] = 4;
        resources[UNITY] = 10;
        show_adjustments(adjs);
    }

    // Use != 0 with result to use as boolean.
    public int get_valid_change_amount(string type, int change) {
        // Return change without going lower than 0.
        //Debug.Log(type + " " + get_res(type) + " + " + change);
        int amount = Statics.valid_nonnegative_change(get_res(type), change);

            //return change - (get_var(type) - change);
        // Return change without going higher than cap.
        if (get_sum_storeable_resources() + change > capacity)
            return capacity - get_sum_storeable_resources();
        return amount;
    }

    public int get_sum_storeable_resources() {
        int sum = 0;
        foreach (string res in resources.Keys) {
            if (res == Discipline.EXPERIENCE ||
                res == Storeable.UNITY ||
                res == Storeable.LIGHT)
                continue;
            sum += resources[res];
        }
        return sum;
    }

    public int change_var(string var, int val, bool show=false) {
        val = get_valid_change_amount(var, val);
        resources[var] += val;
        
        Debug.Log(ID + " " + var  + " " + get_res(var) + " " + 
            get_sum_storeable_resources() + " " + capacity);
        if (on_resource_change != null) {
            on_resource_change(ID, var, get_res(var), 
                get_sum_storeable_resources(), capacity);
        }
        if (show && val != 0) {
            Statics.create_rising_info_map(
                RisingInfo.build_resource_text(var, val), 
                Statics.disc_colors[ID],
                origin_of_rise_obj.transform,
                rising_info_prefab);
        }
        return val;
    }

    public void add_var_without_check(string var, int val) {
        resources[var] += val;
    }

    public void set_res(string res, int amount) {
        if (!resources.ContainsKey(res))
            return;
        resources[res] = amount;
    }

    public int get_res(string var) {
        int r;
        if (resources.TryGetValue(var, out r)) {
            return r;
        }
        Debug.Log("RESOURCE NOT FOUND IN DICTIONARY");
        return 0;
    }
}

public class Adjustment {
    public string resource;
    public int amount;
    public Adjustment(string resource, int amount) {
        this.resource = resource;
        this.amount = amount;
    }
}
 