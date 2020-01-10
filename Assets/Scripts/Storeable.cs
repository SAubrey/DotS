using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storeable : MonoBehaviour {
    public const string LIGHT = "light";
    public const string UNITY = "unity";
    public const string EXPERIENCE = "experience";
    public const string STAR_CRYSTALS = "star_crystals";
    public const string MINERALS = "minerals";
    public const string ARELICS = "arelics";
    public const string MRELICS = "mrelics";
    public const string ERELICS = "erelics";
    public const string EQUIMARES = "equimares";

    public string self;
    protected Controller c;
    protected MapUI map_ui;

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        map_ui = c.map_ui;
        
        if (self == "city") {
            _light = 8;
        }
    }

    public virtual void register_turn() {
        decrement_light();
    }

    public void decrement_light() {
        if (light > 0) {
            light--;
        }
        if (light <= 0) {
            decrement_sc();
        }
    }
    
    private void decrement_sc() {
        if (star_crystals > 0) {
            star_crystals--;
            light = 4;
        } else {
            decrement_unity();
        }
    }

    private void decrement_unity() {
        if (unity > 0) {
            unity -= (2 - (unity % 2));
        } else {
            Debug.Log("No more unity remaining for " + self + "!");
        }
    }

    private bool verify_change(int value) {
        if (value < 0) {
            return false;
        }
        return true;
    }

    public void change_var(string var, int val) {
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
                map_ui.update_stat_text(LIGHT, self, _light);
            }
        }
    }
    
    protected int _unity = 0;
    public int unity {
        get { return _unity; }
        set { 
            if (verify_change(_unity + value)) {
                _unity = value; 
                map_ui.update_stat_text(UNITY, self, _unity);
            }
        }
    }

    protected int _experience = 0;
    public int experience {
        get { return _experience; }
        set { 
            if (verify_change(_experience + value)) {
                _experience = value; 
                map_ui.update_stat_text(EXPERIENCE, self, _experience);
            }
        }
    }

    protected int _star_crystals = 0;
    public int star_crystals {
        get { return _star_crystals; }
        set { 
            if (verify_change(_star_crystals + value)) {
                _star_crystals = value; 
                map_ui.update_stat_text(STAR_CRYSTALS, self, _star_crystals);
            } 
        }
    }

    protected int _minerals = 0;
    public int minerals {
        get { return _minerals; }
        set { 
            if (verify_change(_minerals + value)) {
                _minerals = value; 
                map_ui.update_stat_text(MINERALS, self, _minerals);
            } 
        }
    }

    protected int _arelics = 0;
    public int arelics {
        get { return _arelics; }
        set { 
            if (verify_change(_arelics + value)) {
                _arelics = value; 
                map_ui.update_stat_text(ARELICS, self, _arelics);
            } 
        }
    }

    protected int _mrelics = 0;
    public int mrelics {
        get { return _mrelics; }
        set { 
            if (verify_change(_mrelics + value)) {
                _mrelics = value; 
                map_ui.update_stat_text(MRELICS, self, _mrelics);
            }
        }
    }

    protected int _erelics = 0;
    public int erelics {
        get { return _erelics; }
        set { 
            if (verify_change(_erelics + value)) {
                _erelics = value; 
                map_ui.update_stat_text(ERELICS, self, _erelics);
            } 
        }
    }

    protected int _equimares = 0;
    public int equimares {
        get { return _equimares; }
        set { 
            if (verify_change(_equimares + value)) {
                _equimares = value; 
                map_ui.update_stat_text(EQUIMARES, self, _equimares);
            }
        }
    }
}