using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storeable : MonoBehaviour, ISaveLoad {
    public const string LIGHT = "light";
    public const string UNITY = "unity";
    public const string EXPERIENCE = "experience";
    public const string STAR_CRYSTALS = "star crystals";
    public const string MINERALS = "minerals";
    public const string ARELICS = "Astra relics";
    public const string MRELICS = "Martial relics";
    public const string ERELICS = "Endura relics";
    public const string EQUIMARES = "equimares";

    public static string[] FIELDS = { LIGHT, UNITY, EXPERIENCE, STAR_CRYSTALS,
                            MINERALS, ARELICS, MRELICS, ERELICS, EQUIMARES };
    public Controller c;
    protected MapUI map_ui;
    public CityUIManager city_ui;
    public GameObject rising_info_prefab;
    public GameObject origin_of_rise_obj;
    public int ID;
    public const int INITIAL_CAPACITY = 72;
    public int capacity = INITIAL_CAPACITY;
    public const int INITIAL_LIGHT_REFRESH_AMOUNT = 4;
    public int light_refresh_amount = INITIAL_LIGHT_REFRESH_AMOUNT;

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        map_ui = c.map_ui;
        city_ui = c.city_ui;
    }

    public virtual GameData save() { return null; }
    public virtual void load(GameData generic) { }

    public virtual void new_game() {
        capacity = INITIAL_CAPACITY;
        light_refresh_amount = INITIAL_LIGHT_REFRESH_AMOUNT;
    }

    public virtual void register_turn() {
        light_decay_cascade();
    }

    public void light_decay_cascade() {
        Dictionary<string, int> d = new Dictionary<string, int>();
        d.Add(LIGHT, -1);
        if (light <= 0) {
            if (star_crystals > 0) {
                d.Add(STAR_CRYSTALS, -1);
                d.Add(LIGHT, light_refresh_amount);
            } else {
                if (unity >= 2)
                    d.Add(UNITY, -2);
                else if (unity == 1)
                    d.Add(UNITY, -1);
            }
        }
        StartCoroutine(adjust_resources_visibly(d));
    }

    public virtual void update_text_fields(string type, int value) {
        map_ui.update_stat_text(ID, type, value, get_sum_storeable_resources(), capacity);
        city_ui.update_stat_text(ID, type, value, get_sum_storeable_resources(), capacity);
    }

    public IEnumerator adjust_resources_visibly(Dictionary<string, int> adjustments) {
        foreach (KeyValuePair<string, int> r in adjustments) {
            int valid_change_amount = get_valid_change_amount(r.Key, r.Value);
            if (valid_change_amount != 0) {
                change_var(r.Key, valid_change_amount, true);
                yield return new WaitForSecondsRealtime(0.5f);
            }
        }
    }

    public void create_rising_info(string type, int value) {
        GameObject ri = GameObject.Instantiate(rising_info_prefab);
        ri.transform.SetParent(origin_of_rise_obj.transform, false); 
        ri.transform.position = origin_of_rise_obj.transform.position;
        RisingInfo ri_script = ri.GetComponent<RisingInfo>();
        ri_script.init(type, value);
    }

    // Use != 0 with result to use as boolean.
    public int get_valid_change_amount(string type, int change) {
        // Return change without going lower than 0.
        if (get_var(type) + change < 0) {
            Debug.Log(-get_var(type));
            return -get_var(type);
        }
            //return change - (get_var(type) - change);
        // Return change without going higher than cap.
        if (get_sum_storeable_resources() + change > capacity)
            return capacity - get_sum_storeable_resources();
        return change;
    }

    public int get_sum_storeable_resources() {
        return star_crystals + minerals + arelics + 
            mrelics + erelics + equimares;
    }

    public void change_var(string var, int val, bool show=false) {
        val = get_valid_change_amount(var, val);

        if (var == LIGHT)
            _light += val;
        else if (var == UNITY)
            _unity += val;
        else if (var == EXPERIENCE)
            _experience += val;
        else if (var == STAR_CRYSTALS)
            _star_crystals += val;
        else if (var == MINERALS)
            _minerals += val;
        else if (var == ARELICS)
            _arelics += val;
        else if (var == MRELICS)
            _mrelics += val;
        else if (var == ERELICS)
            _erelics += val;
        else if (var == EQUIMARES)
            _equimares += val;
        
        update_text_fields(var, get_var(var));
        if (show)
            create_rising_info(var, val);
    }

    public void set_var_without_check(string var, int val) {
        if (var == LIGHT)
            _light += val;
        else if (var == UNITY)
            _unity += val;
        else if (var == EXPERIENCE)
            _experience += val;
        else if (var == STAR_CRYSTALS)
            _star_crystals += val;
        else if (var == MINERALS)
            _minerals += val;
        else if (var == ARELICS)
            _arelics += val;
        else if (var == MRELICS)
            _mrelics += val;
        else if (var == ERELICS)
            _erelics += val;
        else if (var == EQUIMARES)
            _equimares += val;
    }

    public int get_var(string var) {
        if (var == LIGHT)
            return light;
        else if (var == UNITY)
            return unity;
        else if (var == EXPERIENCE)
            return experience;
        else if (var == STAR_CRYSTALS)
            return star_crystals;
        else if (var == MINERALS)
            return minerals;
        else if (var == ARELICS)
            return arelics;
        else if (var == MRELICS)
            return mrelics;
        else if (var == ERELICS)
            return erelics;
        else if (var == EQUIMARES)
            return equimares;
        return 0;
    }

    protected int _light = 4;
    public new int light { get { return _light; } }
        /*set { 
            _light = get_valid_change_amount(LIGHT, value - _light); 
            update_text_fields(LIGHT, _light);
        }*/
    
    protected int _unity = 0;
    public int unity { get { return _unity; } }

    protected int _experience = 0;
    public int experience { get { return _experience; } }

    protected int _star_crystals = 0;
    public int star_crystals { get { return _star_crystals; } }

    protected int _minerals = 0;
    public int minerals { get { return _minerals; } }

    protected int _arelics = 0;
    public int arelics { get { return _arelics; } }

    protected int _mrelics = 0;
    public int mrelics { get { return _mrelics; } }

    protected int _erelics = 0;
    public int erelics { get { return _erelics; } }

    protected int _equimares = 0;
    public int equimares { get { return _equimares; } }
}
 