using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Slot : MonoBehaviour {
    private Unit unit;
    public Image img;
    public Controller c;
    private Formation f;
    //public GameObject unit_panel;
    private Camera cam;

    // --VISUAL-- 
    public Text namefg_T;
    public Text namebg_T;
    public Color unselected_not_front;
    public Color dead;
    public Color injured;
    private Color transparent = new Color(0, 0, 0, 0);
    private Color healthbar_fill_color = new Color(1, 1, 1, .8f);
    public Color healthbar_bg_color;
    public Slider healthbar;
    public Image healthbar_bg;
    public Image healthbar_fill;
    private int healthbar_inc_width = 15;

    public Text attfgT, attbgT;
    public Image attfgI, attbgI;
    public Text deffgT, defbgT;
    public Image deffgI, defbgI;
    public Text hpfgT, hpbgT;
    public Color attfgI_c;
    public Color deffgI_c;


    [HideInInspector]
    public int col;
    [HideInInspector]
    public int row;
    public int num; // Hierarchy in group. 0, 1, 2
    public Group group;
    private bool _disabled = false;
    public Button button;
    
    void Awake() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        cam = GameObject.Find("BattleCamera").GetComponent<Camera>();
        col = group.col;
        row = group.row;
        button = GetComponent<Button>();

        face_text_to_cam();
        f = c.formation;
    }

    public void click() {
        if (disabled)
            return;
        c.selector.handle_slot(this);
    }

    public bool fill(Unit u) {
        if (u == null)
            return false;
        set_unit(u);
        set_sprite(u.get_ID());
        return true;
    }

    // Full slots below the removed slot will be moved up if validated.
    public Unit empty(bool validate=true) {
        Unit removed_unit = unit;
        if (unit != null) {
            unit.set_slot(null);
            unit = null;
        }
        set_sprite(PlayerUnit.EMPTY);
        set_namefg_T("");
        show_selection(false);
        toggle_healthbar(false);
        if (validate)
            group.validate_unit_order();
        return removed_unit;
    }

    private void set_unit(Unit u) {
        if (u == null) {
            Debug.Log("! Not setting null unit");
            return;
        }
        if (u.is_playerunit()) {
            unit = u as PlayerUnit;
            int hp = (int)get_punit().resilience;
            set_up_healthbar(hp, hp);
        } else if (u.is_enemy()) {
            unit = u as Enemy;
            set_up_healthbar(get_enemy().health, get_enemy().max_health);
        }
        toggle_healthbar(true);
        set_namefg_T(unit.get_name());
        unit.set_slot(this);
    }

    public Unit get_unit() {
        if (has_enemy) {
            return get_enemy();
        } else if (has_punit) {
            return get_punit();
        } 
        return null;
    }

    public void set_sprite(int image_ID) { 
        if (has_punit) {
            img.sprite = f.images[image_ID]; 
        } else if (has_enemy) {
            img.sprite = c.enemy_loader.images[image_ID];
        } else {
            img.sprite = f.images[image_ID];
        }
    }

    public bool disabled {
        get { return _disabled; }
        set { 
            _disabled = value;
            toggle_healthbar(_disabled);
            button.interactable = !_disabled;
        }
    }

    public void set_up_healthbar(int hp, int max_hp) {
        healthbar.maxValue = max_hp;
        healthbar.value = hp;

        // Adjust width based on max hp.
        RectTransform t = healthbar.transform as RectTransform;
        t.sizeDelta = new Vector2(healthbar_inc_width * max_hp, t.sizeDelta.y);
        update_healthbar(max_hp);
    }

    public void update_healthbar(float hp=-255) {
        if (hp == -255) {
            if (has_punit) {
                hp = get_punit().resilience;
            } else {
                hp = get_enemy().health;
            }
        }
        
        healthbar.value = hp;
        if (unit.is_playerunit()) {
            if (healthbar.value < healthbar.maxValue / 2)
                healthbar.fillRect.GetComponent<Image>().color = injured;
        } else {
            float red = ((float)get_enemy().health / (float)get_enemy().max_health);
            healthbar.fillRect.GetComponent<Image>().color = new Color(1, red, red, 1);
        }

        update_images();
        hpfgT.text = healthbar.value + " / " + healthbar.maxValue;
        hpbgT.text = hpfgT.text;
        attfgT.text = get_unit().get_raw_attack_dmg().ToString();
        attbgT.text = attfgT.text;
        deffgT.text = get_unit().get_raw_defense().ToString();
        defbgT.text = deffgT.text;
    }

    public void update_images() {
        if (get_unit().out_of_actions) {
            attfgI.color = Color.white;
            deffgI.color = Color.white;
        } else {
            attfgI.color = attfgI_c;
            deffgI.color = deffgI_c;
        }
    }

    public void toggle_healthbar(bool state) {
        if (state) {
            healthbar_bg.color = healthbar_bg_color;
            healthbar_fill.color = healthbar_fill_color;
        } else {
            healthbar_bg.color = transparent;
            healthbar_fill.color = transparent;
        }
        healthbar.enabled = state;
        hpfgT.enabled = state;
        hpbgT.enabled = state;

        attfgT.enabled = state;
        attbgT.enabled = state;
        attfgI.enabled = state;
        attbgI.enabled = state;

        deffgT.enabled = state;
        defbgT.enabled = state;
        deffgI.enabled = state;
        defbgI.enabled = state;
    }

    public bool has_punit {
        get {
            if (unit == null) return false;
            if (unit.is_playerunit()) return true;
            return false;
        }
    }

    public bool has_enemy {
        get {
            if (unit == null) return false;
            if (unit.is_enemy()) return true;
            return false;
        }
    }

    public bool has_unit {
        get { return unit != null ? true : false; }
    }

    public bool is_empty {
        get { return unit == null ? true : false; }
    }

    public bool is_type(int type) {
        return group.type == type;
    }

    public void show_selection(bool showing) {
        if (img != null) {
            img.color = showing? Color.gray : Color.white;
        }
    }

    public void show_dead() {
        if (img != null)
            img.color = dead;
    }

    public void show_injured() {
        if (img != null)
            img.color = injured;
    }

    public void show_offensive() {
        if (img != null)
            img.color = new Color(1, 0.2f, 0.2f, 1);
    }

    public void show_defensive(bool defending) {
        healthbar_fill.color = defending ? Color.blue : Color.white;
    }

    public PlayerUnit get_punit() {
        return has_punit ? unit as PlayerUnit : null;
    }

    public Enemy get_enemy() {
        return has_enemy ? unit as Enemy : null;
    }

    private void set_namefg_T(string txt) {
        namefg_T.text = txt;
        namebg_T.text = txt;
    }

    public Sprite get_sprite() { return img.sprite; }

    public Group get_group() {
        return group;
    }

    public void face_text_to_cam() {
        namebg_T.transform.LookAt(cam.transform);
        namebg_T.transform.forward = namefg_T.transform.forward * -1;
        healthbar.transform.LookAt(cam.transform);
        healthbar.transform.forward = healthbar.transform.forward * -1;
    }
}
