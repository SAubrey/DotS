using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour {
    private Color astra_color = new Color(.8f, .8f, 1, 1); // Astra blue
    private Color endura_color = new Vector4(1, 1f, .8f, 1); // Endura orange
    private Color martial_color = new Color(1, .8f, .8f, 1); // Martial red
    
    public Text turn_number_t;
    // City UI
    public GameObject cityP;
    private bool city_panel_active = true;
    public Text c_light;
    public Text c_star_crystals;
    public Text c_minerals;
    public Text c_arelics;
    public Text c_mrelics;
    public Text c_erelics;
    public Text c_equimares;
    public IDictionary<string, Text> city_ui = new Dictionary<string, Text>();

    // Battalion UI
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

    public Text bat_text;
    public IDictionary<string, Text> bat_ui = new Dictionary<string, Text>();

    public static List<string> fields = new List<string>() {
        "light", "unity", "experience", 
        "star_crystals", "minerals", "arelics", 
        "mrelics", "erelics", "equimares"
    };

    Controller c;
    void Awake() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        
        // Populate city dictionary
        city_ui.Add("light", c_light);
        city_ui.Add("star_crystals", c_star_crystals);
        city_ui.Add("minerals", c_minerals);
        city_ui.Add("arelics", c_arelics);
        city_ui.Add("mrelics", c_mrelics);
        city_ui.Add("erelics", c_erelics);
        city_ui.Add("equimares", c_equimares);

        // Populate batallion dictionary
        bat_ui.Add("light", b_light);
        bat_ui.Add("unity", b_unity);
        bat_ui.Add("experience", b_experience);
        bat_ui.Add("star_crystals", b_star_crystals);
        bat_ui.Add("minerals", b_minerals);
        bat_ui.Add("arelics", b_arelics);
        bat_ui.Add("mrelics", b_mrelics);
        bat_ui.Add("erelics", b_erelics);
        bat_ui.Add("equimares", b_equimares);
    }

    public void load_stats(string storeable) {
        // Update each field from culture's storeable dictionary

        foreach (string key in fields) {
            update_stat_text(key, storeable, c.stores[storeable].get_var(key));
        }
        highlight_culture(c.get_player());
    }

    public void update_stat_text(string field, string calling_class, int val) {
        if (calling_class == "city") {
            if (field != Storeable.UNITY || field != Storeable.EXPERIENCE)
                city_ui[field].text = val.ToString();
        } else if (calling_class == c.active_player) {
            bat_ui[field].text = val.ToString();
        }
    }

    private void highlight_culture(string culture) {
        if (culture == Controller.ASTRA) {
            bat_text.text = "Astra";
            bat_text.color = astra_color;
        } else if (culture == Controller.MARTIAL) {
            bat_text.text = "Martial";
            bat_text.color = martial_color;
        } else if (culture == Controller.ENDURA) {
            bat_text.text = "Endura";
            bat_text.color = endura_color;
        }
    }

    public void toggle_city_panel() {
        city_panel_active = !city_panel_active;
        cityP.SetActive(city_panel_active);
    }

    public void toggle_inv_panel() {
        inv_panel_active = !inv_panel_active;
        invP.SetActive(inv_panel_active);
    }
}