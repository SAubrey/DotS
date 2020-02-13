using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Slot : MonoBehaviour {
    private Unit unit;
    public Image img;
    public Image unit_img;
    public Controller c;
    private Formation f;
    private Camera cam;
    private BatLoader bl;

    // --VISUAL-- 
    public Text namefg_T;
    public Text namebg_T;
    public Color unselected_color;
    public Color dead;
    public Color injured;
    private Color TRANSPARENT = new Color(0, 0, 0, 0);
    private Color healthbar_fill_color = new Color(1, 1, 1, .8f);
    public Color healthbar_bg_color;
    public Slider healthbar;
    public Canvas info_canv;
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
    public bool has_active_bonus = false;
    
    void Awake() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        cam = GameObject.Find("BattleCamera").GetComponent<Camera>();
        col = group.col;
        row = group.row;
        button = GetComponent<Button>();

        face_text_to_cam();
        f = c.formation;
    }

    void Start() {
        bl = c.bat_loader;
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
        set_nameT("");
        show_selection(false);
        toggle_healthbar(false);
        if (validate)
            group.validate_unit_order();
        return removed_unit;
    }

    private void set_unit(Unit u) {
        if (u == null) 
            return;
        if (u.is_playerunit()) {
            unit = u as PlayerUnit;
        } else if (u.is_enemy()) {
            unit = u as Enemy;
        }
        toggle_healthbar(true);
        update_UI();
        set_nameT(unit.get_name());
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
            //img.sprite = bl.images[image_ID]; 
            if (bl.unit_images.ContainsKey(image_ID)) {
                unit_img.color = Color.white;
                unit_img.sprite = bl.unit_images[image_ID];
            }
        } else if (has_enemy) {
            img.sprite = c.enemy_loader.images[image_ID];
            //unit_img.sprite = bl.unit_images[image_ID];
            //unit_img.color = Color.white;
        } else {
            //Debug.Log(bl.images);
            //img.sprite = c.bat_loader.images[PlayerUnit.EMPTY]; // empty
            img.sprite = c.bat_loader.empty; // empty
            unit_img.color = TRANSPARENT;
            //unit_img.sprite = bl.unit_images[image_ID];
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

    public void size_healthbar(float max_hp) {
        // Adjust width based on max hp.
        RectTransform t = healthbar.transform as RectTransform;
        t.sizeDelta = new Vector2(healthbar_inc_width * max_hp, t.sizeDelta.y);
    }

    // Updated when a boost is removed or applied,
    // or an attribute is activated or deactivated.
    public void update_UI() {
        update_healthbar();
        update_attack();
        update_defense();

        update_images();
    }

    public void update_healthbar() {
        float hp = get_unit().health; // This will already include the boost but not the bonus.
        float hp_boost = get_unit().get_stat_boost(Unit.HEALTH_BOOST) 
            + get_unit().get_bonus_health();

        healthbar.maxValue = get_unit().max_health + hp_boost;
        healthbar.value = hp;
        size_healthbar(healthbar.maxValue);

        if (unit.is_playerunit()) {
            if (healthbar.value < healthbar.maxValue / 2)
                healthbar.fillRect.GetComponent<Image>().color = injured;
        } else {
            float red = ((float)get_enemy().health / (float)get_enemy().max_health);
            healthbar.fillRect.GetComponent<Image>().color = new Color(1, red, red, 1);
        }
        
        hpfgT.text = build_health_string();
        hpbgT.text = hpfgT.text;
    }

    public string build_health_string() {
        float hp = get_unit().health; // This will already include the boost but not the bonus.
        float hp_boost = get_unit().get_stat_boost(Unit.HEALTH_BOOST)
            + get_unit().get_bonus_health();

        string str = hp + " / " + get_unit().max_health.ToString();
        if (hp_boost > 0) 
            str += "+" + hp_boost.ToString();
        return str;
    }

    public void update_attack() {
        attfgT.text = build_att_string();
        attbgT.text = attfgT.text;
    }

    public string build_att_string() {
        float att_boost = get_unit().get_stat_boost(Unit.ATTACK_BOOST)
            + get_unit().get_bonus_att_dmg();

        string str = get_unit().get_raw_attack_dmg().ToString();
        if (att_boost > 0) 
            str += "+" + att_boost.ToString();
        return str;
    }

    public void update_defense() {
        deffgT.text = build_def_string();
        defbgT.text = deffgT.text;
    }

    public string build_def_string() {
        float def_boost = get_unit().get_stat_boost(Unit.DEFENSE_BOOST)
            + get_unit().get_bonus_def();

        string str = get_unit().get_raw_defense().ToString();
        if (def_boost > 0) 
            str += "+" + def_boost.ToString();
        return str;
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
            healthbar_bg.color = TRANSPARENT;
            healthbar_fill.color = TRANSPARENT;
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

    private void set_nameT(string txt) {
        namefg_T.text = txt;
        namebg_T.text = txt;
    }

    public Sprite get_sprite() { return img.sprite; }

    public Group get_group() {
        return group;
    }

    public void face_text_to_cam() {
        info_canv.transform.LookAt(cam.transform); 
        info_canv.transform.forward *= -1; 
        unit_img.transform.LookAt(cam.transform);
        unit_img.transform.forward *= -1;
    }
}
