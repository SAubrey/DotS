﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class Slot : EventTrigger {
    public Image img, unit_img;
    private Unit unit;
    private Camera cam;

    // --VISUAL-- 
    public TextMeshProUGUI name_T;
    public Color dead, injured;
    
    public static readonly Color selected_color = new Color(1, 1, 1, .8f);
    public static readonly Color unselected_color = new Color(1, 1, 1, .1f);
    private static readonly Color healthbar_fill_color = new Color(.8f, .1f, .1f, .45f);
    private static readonly Color healthbar_injured_fill_color = new Color(1f, .5f, 0f, .45f);
    private static readonly Color staminabar_fill_color = new Color(.1f, .8f, .1f, .45f);
    private static readonly Color defensebar_fill_color = new Color(.1f, .1f, .8f, .45f);
    private static readonly Color statbar_bg_color = new Color(.4f, .4f, .4f, .3f);
    private static readonly Color equipment_text_color = new Color(1f, .67f, .32f, 1f);
    public Slider healthbar, staminabar, defensebar;
    public Canvas info_canv;
    public Image healthbar_bg, healthbar_fill;
    public Image staminabar_bg, staminabar_fill;
    public Image defensebar_bg, defensebar_fill;
    public GameObject staminabar_obj, healthbar_obj, defensebar_obj;
    public UnityEngine.Experimental.Rendering.Universal.Light2D light2d;
    public Sprite range_icon, melee_icon, shield_icon, armor_icon;

    public TextMeshProUGUI attT, defT, hpT, stamT, defbarT;
    public Image attfgI, attbgI;
    public Image deffgI, defbgI;
    public Color attfgI_c, deffgI_c;

    [HideInInspector]
    public int col, row;
    public int num; // Hierarchy in group. 0, 1, 2
    public Group group;
    public Button button;
    private bool _disabled = false;
    public AnimationPlayer animation_player;
    public GameObject animation_obj;
    
    void Awake() {
        cam = GameObject.Find("BattleCamera").GetComponent<Camera>();
        light2d.enabled = false;

        col = group.col;
        row = group.row;

        face_text_to_cam();
    }

    void Start() {
        healthbar_fill.color = healthbar_fill_color;
        staminabar_fill.color = staminabar_fill_color;
        staminabar_bg.color = statbar_bg_color;
        healthbar_bg.color = statbar_bg_color;
        defensebar_fill.color = defensebar_fill_color;
        defensebar_bg.color = statbar_bg_color;
    }

    public void click() {
        if (disabled)
            return;
        Selector.I.handle_slot(this);
    }

    public bool fill(Unit u) {
        if (u == null)
            return false;
        set_unit(u);
        init_UI(u);
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

        update_unit_img(group.direction);
        set_active_UI(false);
        set_nameT("");
        //show_selection(false);
        set_button_color();
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
        return unit;
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
    /* Boosts from equipment are included in the base number and colored orange
    while boosts from attributes and attribute buffs are appended to the base number.  
    */
    public override void OnPointerEnter(PointerEventData eventData) {
        if (!has_enemy)
            return;
        if (Selector.I.selecting_target && 
                (group.get_highest_enemy_slot() == this || 
                Selector.I.selected_slot.unit.combat_style == Unit.RANGE)) {
            Selector.I.hovered_slot = this;
            update_healthbar(Selector.I.selected_slot.get_unit());
        }
    }

    public override void OnPointerExit(PointerEventData eventData) {
        if (!has_enemy)
            return;
        // Only remove preview damage if selector is actively targeting,
        if (Selector.I.selecting_target) {
            update_healthbar();
            Selector.I.hovered_slot = null;
        }
    }
    

    public void init_UI(Unit u) {
        set_attack_icon(u);
        set_defense_icon(u);
        set_button_color();
        update_healthbar_color();
        update_defensebar_color();
        show_equipment_boosts();
        set_nameT(unit.get_name());
        update_unit_img(group.direction);
        set_active_UI(true);
        update_text_UI();
    }

    private void set_attack_icon(Unit u) {
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
    }

    private void set_defense_icon(Unit u) {
        if (u.is_playerunit) {
            deffgI.sprite = shield_icon;
            defbgI.sprite = shield_icon;
        } else {
            defbgI.sprite = armor_icon;
            deffgI.sprite = armor_icon;
        }
    }

    // Updated when a boost is removed or applied,
    // or an attribute is activated or deactivated.
    public void update_text_UI() {
        update_healthbar();
        update_staminabar(get_unit().num_actions);
        update_defensebar();
        update_attack();
        //update_defense();
        if (UnitPanelManager.I.player_panel.slot == this) {
            UnitPanelManager.I.update_text(this); 
        }
    }

    public void update_healthbar(Unit preview_unit=null) {
        healthbar.maxValue = get_unit().get_dynamic_max_health();
        
        int prev_dmg = determine_damage_with_preview(preview_unit);
        int final_hp = get_unit().calc_hp_remaining(prev_dmg);
        healthbar.value = final_hp;
        update_healthbar_color();
        
        hpT.text = build_health_string(final_hp, prev_dmg);
    }

    private int determine_damage_with_preview(Unit preview_unit=null) {
        List <Attack> atts = get_unit().is_playerunit ? 
            AttackQueuer.I.get_enemy_queue().get_incoming_attacks(get_unit()) :
            AttackQueuer.I.get_player_queue().get_incoming_attacks(get_unit());
        return AttackQueuer.calc_final_group_dmg_taken(atts, preview_unit);
    }

    private int determine_defense_dmg_with_preview(Unit preview_unit=null) {
        List <Attack> atts = get_unit().is_playerunit ? 
            AttackQueuer.I.get_enemy_queue().get_incoming_attacks(get_unit()) :
            AttackQueuer.I.get_player_queue().get_incoming_attacks(get_unit());
        return AttackQueuer.calc_final_group_defense_reduction(atts, preview_unit);
    }
 
    public string build_health_string(float hp, float preview_damage) {
        //float hp = get_unit().health; // This will already include the boost but not the bonus.
        float hp_boost = get_unit().get_stat_buff(Unit.HEALTH)
            + get_unit().get_bonus_health();

        string str = (hp + get_unit().get_bonus_from_equipment(Unit.HEALTH)).ToString();
        if (preview_damage > 0)
            str += " (-" + preview_damage.ToString() + ")";
        if (hp_boost > 0) 
            str += " (+" + hp_boost.ToString() + ")";
        return str;
    }

    private void update_healthbar_color() {
        if (unit.is_playerunit) {
            if (healthbar.value < healthbar.maxValue / 2f)
                healthbar.fillRect.GetComponent<Image>().color = healthbar_injured_fill_color;
        } else {
            //float red = ((float)get_enemy().health / (float)get_enemy().max_health);
            //healthbar.fillRect.GetComponent<Image>().color = new Color(1, red, red, 1);
            healthbar.fillRect.GetComponent<Image>().color = healthbar_fill_color;
        }
    }

    public void update_defensebar(Unit preview_unit=null) {
        update_defensebar_color();
        // The defense bar includes but does not show stat bonus.
        defensebar.maxValue = get_unit().get_defense();
        int prev_dmg = determine_defense_dmg_with_preview(preview_unit);
        defensebar.value = get_unit().get_defense() - prev_dmg;
        defbarT.text = build_defense_string(defensebar.value, prev_dmg);
    }

    private void update_defensebar_color() {
        defensebar_obj.SetActive(get_unit().get_defense() != 0);
        if (!get_unit().defending) {
            defensebar_fill.color = Color.clear;
        } else {
            defensebar_fill.color = defensebar_fill_color;
        }
    }

    public string build_defense_string(float def, float preview_damage) {
        float def_boost = get_unit().get_stat_buff(Unit.DEFENSE)
            + get_unit().get_bonus_def();
        string str = def.ToString();
        if (preview_damage > 0)
            str += " (-" + preview_damage.ToString() + ")";
        if (def_boost > 0) 
            str += " (+" + def_boost.ToString() + ")";
        return str;
    }

    public void update_staminabar(int stam) {
        staminabar.maxValue = get_unit().max_num_actions;
        staminabar.value = stam;
        
        stamT.text = build_stamina_string(stam);
    }

    public string build_stamina_string(float stam) {
        string str = stam.ToString();
        return str;
    }

    public void update_attack() {
        attT.text = build_att_string();
        toggle_attack(get_unit().get_attack_dmg() > 0);
        show_attacking(get_unit());
    }

    private void show_attacking(Unit u) {
        u.get_slot().attfgI.color = verify_show_attacking(u) ? attfgI_c : Color.white;
    }

    private bool verify_show_attacking(Unit u) {
        if (u == null)
            return false;
        if (u.get_slot() == null)
            return false;
        if (u.get_slot().get_group().is_empty)
            return false;

        Unit highest_unit = get_group().get_highest_full_slot().get_unit();
        int num_grouped = highest_unit.count_grouped_units(); // accounts for grouping attr level.

        bool same_ID = u.get_ID() == highest_unit.get_ID();
        // If 2 other in group, both non-head units are grouped. 
        // If 1 other in group, unit is grouped if not head and same unit type.
        bool grouping = u.has_grouping && highest_unit.is_actively_grouping && same_ID &&
        ((num_grouped == 2 && (u != highest_unit)) || num_grouped == 3);

        return u.attack_set || grouping;
    }

    private void toggle_attack(bool showing) {
        attbgI.gameObject.SetActive(showing);
    }

    public string build_att_string() {
        float att_boost = get_unit().get_stat_buff(Unit.ATTACK)
            + get_unit().get_bonus_att_dmg();

        string str = (get_unit().get_raw_attack_dmg() +
            get_unit().get_bonus_from_equipment(Unit.ATTACK)).ToString();
        if (att_boost > 0 && get_unit().attack_set) 
            str += "+" + att_boost.ToString();
        return str;
    }

    public void update_defense() {
        defT.text = build_def_string();
        toggle_defense(get_unit().get_defense() > 0);
        show_defending(get_unit());
    }

    private void show_defending(Unit u) {
       u.get_slot().deffgI.color = verify_show_defending(u) ? deffgI_c : Color.white;
    }

    // Determines all possible circumstances to color the defense icon blue.
    private bool verify_show_defending(Unit u) {
        if (u == null)
            return false;
        if (u.get_slot() == null)
            return false;
        if (u.get_slot().get_group().is_empty)
            return false;

        Unit highest_unit = get_group().get_highest_full_slot().get_unit();
        int num_grouped = highest_unit.count_grouped_units(); // accounts for grouping attr level.

        bool same_ID = u.get_ID() == highest_unit.get_ID();
        // If 2 other in group, both non-head units are grouped. 
        // If 1 other in group, unit is grouped if not head and same unit type.
        bool grouping = u.has_grouping && highest_unit.is_actively_grouping && same_ID &&
        ((num_grouped == 2 && (u != highest_unit)) || num_grouped == 3);

         // Always show an armored enemy as defending.
        bool defensive_enemy = has_enemy && u.get_defense() > 0;

        return u.defending || grouping || defensive_enemy;
    }

    private void toggle_defense(bool showing) {
        defbgI.gameObject.SetActive(showing);
    }

    public string build_def_string() {
        int def_boost = get_unit().get_stat_buff(Unit.DEFENSE)
            + get_unit().get_bonus_def();

        string str = (get_unit().get_raw_defense() + 
            get_unit().get_bonus_from_equipment(Unit.DEFENSE)).ToString();
        if (def_boost > 0 && get_unit().defending) 
            str += "+" + def_boost.ToString();
        return str;
    }

    public void set_active_UI(bool state) {
        staminabar_obj.SetActive(state);
        healthbar_obj.SetActive(state);
        defensebar_obj.SetActive(state);
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

    // Set the color of the square slot button's image.
    public void set_button_color() {
        if (img == null)
            return;
        
        if (!has_unit) {
            img.color = unselected_color;
            return;
        }

        // Show which discipline owns the unit.
        img.color = get_unit().is_playerunit ? 
            Statics.disc_colors[get_punit().owner_ID] : Color.white;
        show_selection(Selector.I.selected_slot == this);
    }

    private void show_equipment_boosts() {
        if (unit.is_enemy)
            return;

        EquipmentInventory ei = Controller.I.get_disc(get_punit().owner_ID).equipment_inventory;
        hpT.color = ei.get_stat_boost_amount(unit.get_ID(), Unit.HEALTH) > 0 ? 
            equipment_text_color : Color.white;
        defT.color = ei.get_stat_boost_amount(unit.get_ID(), Unit.DEFENSE) > 0 ? 
            equipment_text_color : Color.white;
        attT.color = ei.get_stat_boost_amount(unit.get_ID(), Unit.ATTACK) > 0 ? 
            equipment_text_color : Color.white;
    }

    public void show_dead() {
        if (img != null)
            img.color = dead;
    }

    public void show_injured() {
        if (img != null)
            img.color = injured;
    }

    public void play_animation(string anim) {
        animation_player.play(anim);
    }

    private void update_unit_img(int dir) {
        unit_img.color = Color.white;
        unit_img.sprite = BatLoader.I.get_unit_img(unit, dir);
        if (unit_img.sprite == null)
            unit_img.color = Color.clear;
        rotate_to_direction(dir); 
    }

    public void rotate_to_direction(int direction) {
        unit_img.sprite = BatLoader.I.get_unit_img(unit, direction);
        unit_img.transform.LookAt(cam.transform);
        set_animation_depth(direction);
        //unit_img.transform.forward *= -1;
         // Used if only using forward/back images to mirror them for right/left.
        if (direction == 0 || direction == 180) {
            unit_img.transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
        } else 
            unit_img.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
        face_text_to_cam();
    }

    public void set_animation_depth(int direction) {
        if (direction == Group.UP || direction == Group.RIGHT) {
            animation_obj.transform.SetAsFirstSibling();
        } else {
            animation_obj.transform.SetAsLastSibling();
        }
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
