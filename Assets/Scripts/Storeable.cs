﻿using System.Collections;
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
    public int ID;
    public GameObject rising_info_prefab;
    public GameObject origin_of_rise_obj;

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        map_ui = c.map_ui;
        city_ui = c.city_ui;
    }

    public virtual GameData save() { return null; }
    public virtual void load(GameData generic) { }

    public virtual void register_turn() {
        light_decay_cascade();
    }

    public void light_decay_cascade() {
        Dictionary<string, int> d = new Dictionary<string, int>();
        d.Add(LIGHT, -1);
        if (light <= 0) {
            if (star_crystals > 0) {
                d.Add(STAR_CRYSTALS, -1);
                d.Add(LIGHT, 4);
            } else {
                if (unity >= 2)
                    d.Add(UNITY, -2);
                else if (unity == 1)
                    d.Add(UNITY, -1);
            }
        }
        StartCoroutine(adjust_resources_visibly(d));
    }

    public IEnumerator adjust_resources_visibly(Dictionary<string, int> adjustments) {
        foreach (KeyValuePair<string, int> r in adjustments) {
            if (r.Value != 0) {
                change_var(r.Key, r.Value, true);
                yield return new WaitForSecondsRealtime(0.5f);
            }
        }
    }

    private bool verify_change(int value) {
        if (value < 0) {
            return false;
        }
        return true;
    }

    public bool verify_change(string type, int value) {
        if (get_var(type) + value < 0) {
            return false;
        }
        return true;
    }

    public virtual void update_text_fields(string type, int value) {
        map_ui.update_stat_text(type, ID, value);
        city_ui.update_stat_text(type, ID, value);
    }

    public void create_rising_info(string type, int value) {
        GameObject ri = GameObject.Instantiate(rising_info_prefab);
        ri.transform.SetParent(origin_of_rise_obj.transform, false); 
        ri.transform.position = origin_of_rise_obj.transform.position;
        RisingInfo ri_script = ri.GetComponent<RisingInfo>();
        ri_script.init(type, value);
    }

    public void change_var(string var, int val, bool show=false) {
        if (var == LIGHT)
            light += val;
        else if (var == UNITY)
            unity += val;
        else if (var == EXPERIENCE)
            experience += val;
        else if (var == STAR_CRYSTALS)
            star_crystals += val;
        else if (var == MINERALS)
            minerals += val;
        else if (var == ARELICS)
            arelics += val;
        else if (var == MRELICS)
            mrelics += val;
        else if (var == ERELICS)
            erelics += val;
        else if (var == EQUIMARES)
            equimares += val;
        
        if (show)
            create_rising_info(var, val);
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
    public new int light {
        get { return _light; }
        set { 
            if (verify_change(_light + value)) {
                _light = value; 
                update_text_fields(LIGHT, _light);
            }
        }
    }
    
    protected int _unity = 0;
    public int unity {
        get { return _unity; }
        set { 
            if (verify_change(_unity + value)) {
                _unity = value; 
                update_text_fields(UNITY, _unity);
            }
        }
    }

    protected int _experience = 0;
    public int experience {
        get { return _experience; }
        set { 
            if (verify_change(_experience + value)) {
                _experience = value; 
                update_text_fields(EXPERIENCE, _experience);
            }
        }
    }

    protected int _star_crystals = 0;
    public int star_crystals {
        get { return _star_crystals; }
        set { 
            if (verify_change(_star_crystals + value)) {
                _star_crystals = value; 
                update_text_fields(STAR_CRYSTALS, _star_crystals);
            } 
        }
    }

    protected int _minerals = 0;
    public int minerals {
        get { return _minerals; }
        set { 
            if (verify_change(_minerals + value)) {
                _minerals = value; 
                update_text_fields(MINERALS, _minerals);
            } 
        }
    }

    protected int _arelics = 0;
    public int arelics {
        get { return _arelics; }
        set { 
            if (verify_change(_arelics + value)) {
                _arelics = value; 
                update_text_fields(ARELICS, _arelics);
            } 
        }
    }

    protected int _mrelics = 0;
    public int mrelics {
        get { return _mrelics; }
        set { 
            if (verify_change(_mrelics + value)) {
                _mrelics = value; 
                update_text_fields(MRELICS, _mrelics);
            }
        }
    }

    protected int _erelics = 0;
    public int erelics {
        get { return _erelics; }
        set { 
            if (verify_change(_erelics + value)) {
                _erelics = value; 
                update_text_fields(ERELICS, _erelics);
            } 
        }
    }

    protected int _equimares = 0;
    public int equimares {
        get { return _equimares; }
        set { 
            if (verify_change(_equimares + value)) {
                _equimares = value; 
                update_text_fields(EQUIMARES, _equimares);
            }
        }
    }
}
 