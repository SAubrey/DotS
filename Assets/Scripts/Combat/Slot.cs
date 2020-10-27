using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class Slot : EventTrigger {
    public Image img, unit_img;
    private Unit unit;
    public Controller c;
    private Formation f;
    private Camera cam;
    private BatLoader bl;

    // --VISUAL-- 
    public TextMeshProUGUI name_T;
    public Color dead, injured;
    
    public static readonly Color selected_color = new Color(1, 1, 1, .8f);
    public static readonly Color unselected_color = new Color(1, 1, 1, .1f);
    private static readonly Color healthbar_fill_color = new Color(.8f, .1f, .1f, .3f);
    private static readonly Color staminabar_fill_color = new Color(.1f, .8f, .1f, .3f);
    private static readonly Color statbar_bg_color = new Color(.4f, .4f, .4f, .3f);
    public Slider healthbar, staminabar;
    public Canvas info_canv;
    public Image healthbar_bg, healthbar_fill;
    public Image staminabar_bg, staminabar_fill;
    public GameObject staminabar_obj, healthbar_obj;
    public UnityEngine.Experimental.Rendering.Universal.Light2D light2d;
    public Sprite range_icon, melee_icon;

    public TextMeshProUGUI attT, defT, hpT, stamT;
    public Image attfgI, attbgI;
    public Image deffgI, defbgI;
    public Color attfgI_c, deffgI_c;

    [HideInInspector]
    public int col, row;
    public int num; // Hierarchy in group. 0, 1, 2
    public Group group;
    public Button button;
    private bool _disabled = false;
    
    void Awake() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        cam = GameObject.Find("BattleCamera").GetComponent<Camera>();
        light2d.enabled = false;

        col = group.col;
        row = group.row;
        //button = GetComponent<Button>();

        face_text_to_cam();
        f = c.formation;
    }

    void Start() {
        bl = c.bat_loader;
        healthbar_fill.color = healthbar_fill_color;
        staminabar_fill.color = staminabar_fill_color;
        staminabar_bg.color = statbar_bg_color;
        healthbar_bg.color = statbar_bg_color;
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
        init_UI(c.battle_phaser.active_bat.disc.ID);
        set_nameT(unit.get_name());
        update_UI(group.get_dir());
        set_active_UI(true);
        if (u.is_playerunit)
            toggle_light(true);
        return true;
    }

    // Full slots below the removed slot will be moved up if validated.
    public Unit empty(bool validate=true) {
        Unit removed_unit = unit;
        if (unit != null) {
            unit.set_slot(null);
            unit = null;
        }

        update_UI(group.get_dir());
        set_active_UI(false);
        set_nameT("");
        //show_selection(false);
        set_color();
        toggle_light(false);
        if (validate)
            group.validate_unit_order();
        return removed_unit;
    }

    private void set_unit(Unit u) {
        if (u == null) 
            return;
        if (u.is_playerunit) {
            unit = u as PlayerUnit;
        } else if (u.is_enemy) {
            unit = u as Enemy;
        }
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

    public bool disabled {
        get { return _disabled; }
        set { 
            _disabled = value;
            set_active_UI(_disabled);
            button.interactable = !_disabled;
        }
    }


    // ----- GRAPHICAL -----
    public bool showing_preview_damage = false;

    public bool is_showing_damage() {
        return showing_preview_damage;
    }
    
    public override void OnPointerEnter(PointerEventData eventData) {
        if (c.selector.selecting_target && has_enemy && 
                (group.get_highest_enemy_slot() == this || 
                c.selector.selected_slot.unit.combat_style == Unit.RANGE)) {
            showing_preview_damage = true;
            show_preview_damage(true, c.selector.selected_slot.get_unit().get_attack_dmg());
        }
    }

    public override void OnPointerExit(PointerEventData eventData) {
        if (showing_preview_damage) {
            showing_preview_damage = false;
            show_preview_damage(false);
        }
    }

    public void show_preview_damage(bool showing, int dmg=0) {
        if (showing) {
            update_healthbar(unit.calc_hp_remaining(dmg), dmg);
        } else {
            if (!c.selector.selected_slot.unit.attack_set)
                update_healthbar(get_unit().health);
        }
    }

    public void init_UI(int disc_ID) {
        // Change attack icon based on attack type.
        if (get_unit().is_range) {
            attfgI.sprite = range_icon;
            attbgI.sprite = range_icon;
        } else {
            attfgI.sprite = melee_icon;
            attbgI.sprite = melee_icon;
        }
        attfgI.color = Color.white;
        deffgI.color = Color.white;

        if (get_unit().is_playerunit) {
            // Assign slot button image color to show discipline.
            set_color();
        }
        update_UI();
    }

    // Updated when a boost is removed or applied,
    // or an attribute is activated or deactivated.
    public void update_UI() {
        update_healthbar(get_unit().health);
        update_staminabar(get_unit().num_actions);
        update_attack();
        update_defense();
        if (c.unit_panel_man.player_panel.slot == this) {
            c.unit_panel_man.update_text(this); 
        }
    }

    public void update_healthbar(float hp, float preview_damage=0) {
        healthbar.maxValue = get_unit().get_boosted_max_health();
        healthbar.value = hp;
        //update_healthbar_color();
        
        hpT.text = build_health_string(hp, preview_damage);
    }

    public void update_staminabar(int stam) {
        staminabar.maxValue = get_unit().max_num_actions;
        staminabar.value = stam;
        
        stamT.text = build_stamina_string(stam);
    }

    private void update_healthbar_color() {
        if (unit.is_playerunit) {
            if (healthbar.value < healthbar.maxValue / 2)
                healthbar.fillRect.GetComponent<Image>().color = injured;
        } else {
            float red = ((float)get_enemy().health / (float)get_enemy().max_health);
            healthbar.fillRect.GetComponent<Image>().color = new Color(1, red, red, 1);
        }
    }

    public string build_health_string(float hp, float preview_damage=0) {
        //float hp = get_unit().health; // This will already include the boost but not the bonus.
        float hp_boost = get_unit().get_stat_boost(Unit.HEALTH_BOOST)
            + get_unit().get_bonus_health();

        string str = hp.ToString();
        if (preview_damage > 0)
            str += "(-" + preview_damage.ToString() + ")";
        //str += "/" + get_unit().max_health.ToString();
        if (hp_boost > 0) 
            str += "(+" + hp_boost.ToString() + ")";
            //str += "+" + hp_boost.ToString();
        return str;
    }

    public string build_stamina_string(float stam) {
        string str = stam.ToString();
        return str;
    }

    public void update_attack() {
        attT.text = build_att_string();
        Unit u = get_unit();
        if (u.has_grouping) {
            int num_grouped = u.has_attribute(PlayerUnit.GROUPING_1) ? 2 : 3;
            show_group_attacking(u.is_attribute_active && u.attack_set, num_grouped);
        }
        show_attacking(u.attack_set);
        toggle_attack(u.get_attack_dmg() > 0);
    }

    public string build_att_string() {
        float att_boost = get_unit().get_stat_boost(Unit.ATTACK_BOOST)
            + get_unit().get_bonus_att_dmg();

        string str = get_unit().get_raw_attack_dmg().ToString();
        if (att_boost > 0 && get_unit().attack_set) 
            str += "+" + att_boost.ToString();
        return str;
    }

    public void update_defense() {
        defT.text = build_def_string();
        Unit u = get_unit();
        if (u.has_grouping) {
            int num_grouped = u.has_attribute(PlayerUnit.GROUPING_1) ? 2 : 3;       
            show_group_defending(u.is_attribute_active && u.defending, num_grouped);
        }
        toggle_defense(u.get_defense() > 0);
        if (u.is_enemy)
            show_defending(u.get_defense() > 0);
        else   
            show_defending(u.defending);

    }

    private void show_defending(bool showing) {
        deffgI.color = showing ? deffgI_c : Color.white;
    }

    private void toggle_defense(bool showing) {
        defbgI.gameObject.SetActive(showing);
    }

    private void toggle_attack(bool showing) {
        attbgI.gameObject.SetActive(showing);
    }

    public string build_def_string() {
        float def_boost = get_unit().get_stat_boost(Unit.DEFENSE_BOOST)
            + get_unit().get_bonus_def();

        string str = get_unit().get_raw_defense().ToString();
        if (def_boost > 0 && get_unit().defending) 
            str += "+" + def_boost.ToString();
        return str;
    }

    public void set_active_UI(bool state) {
        staminabar_obj.SetActive(state);
        healthbar_obj.SetActive(state);
    }

    // Selection color is determined by opacity.
    public void show_selection(bool showing) {
        if (img != null) {
            Color color = img.color;
            //color.a = showing ? selected_color.a : unselected_color.a;
            if (has_punit) {
                color.a = showing ? selected_color.a + 0.2f : unselected_color.a + 0.7f;
            } else
                color.a = showing ? selected_color.a : unselected_color.a;
            img.color = color;
        }
    }

    public void set_color() {
        if (img == null)
            return;
        Color color; 
        
        if (!has_unit) {
            color = Color.white;
        }
        // Show which discipline owns the unit.
        else if (get_unit().is_playerunit) {
            color = Statics.disc_colors[get_punit().owner_ID];
        } else {
            color = Color.white;
        }
        /*
        Debug.Log("setting transparency?");
        // Show if selected.
        if (c.selector.selected_slot == this) {
            color.a = selected_color.a;
            Debug.Log(color.a);
        } else {
            color.a = unselected_color.a;
            Debug.Log(color.a);
        }*/
        img.color = color;
        show_selection(c.selector.selected_slot == this);
    }

    public void show_dead() {
        if (img != null)
            img.color = dead;
    }

    public void show_injured() {
        if (img != null)
            img.color = injured;
    }

    private void show_attacking(bool attacking) {
        attfgI.color = attacking ? attfgI_c : Color.white;
    }

    private void show_group_defending(bool defending, int num) {
        Slot s;
        for (int i = 0; i < num; i++) {
            s = group.slots[i];
            s.deffgI.color = defending ? deffgI_c : Color.white;
        }
    }

    private void show_group_attacking(bool attacking, int num) {
        Slot s;
        for (int i = 0; i < num; i++) {
            s = group.slots[i];
            s.attfgI.color = attacking ? attfgI_c : Color.white;
        }
    }

    // Update slot button image and slot unit image.
    public void update_UI(int dir) { 
        unit_img.color = Color.white;
        if (bl == null) {
            Debug.Log("Reassigning BL");
            bl = GameObject.Find("BatLoader").GetComponent<BatLoader>();
            Debug.Log("Bl: " + bl);
        }
        unit_img.sprite = bl.get_unit_img(unit, dir);
        if (unit_img.sprite == null)
            unit_img.color = Color.clear;

        
        rotate_unit_img_to_direction(dir); 
        face_text_to_cam();
    }

    public void rotate_unit_img_to_direction(int direction) {
        unit_img.transform.LookAt(cam.transform);
        //unit_img.transform.forward *= -1;
         // Used if only using forward/back images to mirror them for right/left.
        if (direction == 0 || direction == 180) {
            unit_img.transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
        } else 
            unit_img.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
        
    }

    public void face_text_to_cam() {
        info_canv.transform.LookAt(cam.transform); 
        info_canv.transform.forward *= -1; 
    }

    private void toggle_light(bool state) {
        light2d.enabled = state;
    }
    // ---End GRAPHICAL--- 
    
    public bool is_type(int type) {
        return group.type == type;
    }

    public bool has_punit {
        get {
            if (unit == null) return false;
            if (unit.is_playerunit) return true;
            return false;
        }
    }

    public bool has_enemy {
        get {
            if (unit == null) return false;
            if (unit.is_enemy) return true;
            return false;
        }
    }

    public bool has_unit {
        get { return unit != null ? true : false; }
    }

    public bool is_empty {
        get { return unit == null ? true : false; }
    }

    public PlayerUnit get_punit() {
        return has_punit ? unit as PlayerUnit : null;
    }

    public Enemy get_enemy() {
        return has_enemy ? unit as Enemy : null;
    }

    private void set_nameT(string txt) {
        name_T.text = txt;
    }

    public Group get_group() {
        return group;
    }
}
