using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatLoader : MonoBehaviour {
    private Controller c;
    public Sprite white_fade_img, dark_fade_img;
    public Sprite empty; // UIMask image for a slot button image.
    // Unit quantity text fields in the unit selection scrollbar.
    public Dictionary<int, Text> texts = new Dictionary<int, Text>();
    public Text warrior_t, spearman_t, archer_t, miner_t, inspirator_t, seeker_t,
        guardian_t, arbalest_t, skirmisher_t, paladin_t, mender_t, carter_t, dragoon_t,
        scout_t, drummer_t, shield_maiden_t, pikeman_t;


    // Button images in battle scene for highlighting selections.
    public IDictionary<int, Button> unit_buttons = new Dictionary<int, Button>();
    public Button archer_B, warrior_B, spearman_B, inspirator_B, miner_B, 
        seeker_B, guardian_B, arbalest_B, skirmisher_B, paladin_B, mender_B, carter_B, dragoon_B,
        scout_B, drummer_B, shield_maiden_B, pikeman_B;


    // Drawn unit images.
    public Dictionary<int, Sprite> unit_images_back = new Dictionary<int, Sprite>();
    public Sprite warrior_back, spearman_I, archer_back, miner_I, inspirator_I, seeker_I,
        guardian_I, arbalest_I, skirmisher_I, paladin_I, mender_I, carter_I, dragoon_I,
        scout_I, drummer_I, shield_maiden_I, pikeman_I;
    public Dictionary<int, Sprite> unit_images_front = new Dictionary<int, Sprite>();
    public Sprite warrior_front, archer_front;


    // Generic unit sprites as placeholders until art is finalized.
    public Dictionary<int, Sprite> generic_punit_sprites = new Dictionary<int, Sprite>();
    public Sprite punit_up, punit_down, punit_left, punit_right;

    public Dictionary<int, Sprite> generic_enemy_sprites = new Dictionary<int, Sprite>();
    public Sprite enemy_up, enemy_down, enemy_left, enemy_right;

    public bool selecting_for_heal = false;
    public PlayerUnit healing_unit;
    private int selected_unit_type = 0;

    public Sprite get_unit_img(Unit unit, int direction) {
        if (unit.is_playerunit) {
            if (unit.is_melee) {
                return get_unit_direction_img(0, direction);
            }
            else if (unit.is_range) {
                return get_unit_direction_img(2, direction);
            }
            else {
                return generic_punit_sprites[direction];
            }
        } else 
            return generic_enemy_sprites[direction];
    }

    private Sprite get_unit_direction_img(int unit_type, int direction) {
        if (direction == Group.UP || direction == Group.RIGHT)
            return unit_images_back[unit_type];
        return unit_images_front[unit_type];
    }

    void Awake() {
        generic_punit_sprites.Add(Group.UP, punit_up);
        generic_punit_sprites.Add(Group.DOWN, punit_down);
        generic_punit_sprites.Add(Group.LEFT, punit_left);
        generic_punit_sprites.Add(Group.RIGHT, punit_right);

        generic_enemy_sprites.Add(Group.UP, enemy_up);
        generic_enemy_sprites.Add(Group.DOWN, enemy_down);
        generic_enemy_sprites.Add(Group.LEFT, enemy_left);
        generic_enemy_sprites.Add(Group.RIGHT, enemy_right);

        texts.Add(PlayerUnit.WARRIOR, warrior_t);
        texts.Add(PlayerUnit.SPEARMAN, spearman_t);
        texts.Add(PlayerUnit.ARCHER, archer_t);
        texts.Add(PlayerUnit.MINER, miner_t);
        texts.Add(PlayerUnit.INSPIRATOR, inspirator_t);
        texts.Add(PlayerUnit.SEEKER, seeker_t);
        texts.Add(PlayerUnit.GUARDIAN, guardian_t);
        texts.Add(PlayerUnit.ARBALEST, arbalest_t);
        texts.Add(PlayerUnit.SKIRMISHER, skirmisher_t);
        texts.Add(PlayerUnit.PALADIN, paladin_t);
        texts.Add(PlayerUnit.MENDER, mender_t);
        texts.Add(PlayerUnit.CARTER, carter_t);
        texts.Add(PlayerUnit.DRAGOON, dragoon_t);
        texts.Add(PlayerUnit.SCOUT, scout_t);
        texts.Add(PlayerUnit.DRUMMER, drummer_t);
        texts.Add(PlayerUnit.SHIELD_MAIDEN, shield_maiden_t);
        texts.Add(PlayerUnit.PIKEMAN, pikeman_t);

        // Populate unit placement button images dictionary.
        unit_buttons.Add(PlayerUnit.WARRIOR, warrior_B);
        unit_buttons.Add(PlayerUnit.SPEARMAN, spearman_B);
        unit_buttons.Add(PlayerUnit.ARCHER, archer_B);
        unit_buttons.Add(PlayerUnit.MINER, miner_B);
        unit_buttons.Add(PlayerUnit.INSPIRATOR, inspirator_B);
        unit_buttons.Add(PlayerUnit.SEEKER, seeker_B);
        unit_buttons.Add(PlayerUnit.GUARDIAN, guardian_B);
        unit_buttons.Add(PlayerUnit.ARBALEST, arbalest_B);
        unit_buttons.Add(PlayerUnit.SKIRMISHER, skirmisher_B);
        unit_buttons.Add(PlayerUnit.PALADIN, paladin_B);
        unit_buttons.Add(PlayerUnit.MENDER, mender_B);
        unit_buttons.Add(PlayerUnit.CARTER, carter_B);
        unit_buttons.Add(PlayerUnit.DRAGOON, dragoon_B);
        unit_buttons.Add(PlayerUnit.SCOUT, scout_B);
        unit_buttons.Add(PlayerUnit.DRUMMER, drummer_B);
        unit_buttons.Add(PlayerUnit.SHIELD_MAIDEN, shield_maiden_B);
        unit_buttons.Add(PlayerUnit.PIKEMAN, pikeman_B);

        // Unit images to be loaded into slots.
        unit_images_back.Add(PlayerUnit.WARRIOR, warrior_back);
        unit_images_back.Add(PlayerUnit.SPEARMAN, spearman_I);
        unit_images_back.Add(PlayerUnit.ARCHER, archer_back);
        unit_images_back.Add(PlayerUnit.MINER, miner_I);
        unit_images_back.Add(PlayerUnit.INSPIRATOR, inspirator_I);
        unit_images_back.Add(PlayerUnit.SEEKER, seeker_I);
        unit_images_back.Add(PlayerUnit.SHIELD_MAIDEN, shield_maiden_I);
        unit_images_back.Add(PlayerUnit.ARBALEST, arbalest_I);
        unit_images_back.Add(PlayerUnit.SKIRMISHER, skirmisher_I);
        unit_images_back.Add(PlayerUnit.PALADIN, paladin_I);
        unit_images_back.Add(PlayerUnit.MENDER, mender_I);
        unit_images_back.Add(PlayerUnit.CARTER, carter_I);
        unit_images_back.Add(PlayerUnit.DRAGOON, dragoon_I);
        unit_images_back.Add(PlayerUnit.SCOUT, scout_I);
        unit_images_back.Add(PlayerUnit.DRUMMER, drummer_I);
        unit_images_back.Add(PlayerUnit.GUARDIAN, guardian_I);
        unit_images_back.Add(PlayerUnit.PIKEMAN, pikeman_I);

        unit_images_front.Add(PlayerUnit.WARRIOR, warrior_front);
        unit_images_front.Add(PlayerUnit.ARCHER, archer_front);
    }

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
    }

    // This loads the player's battalion composition into the static
    // slots in the battle scene. 
    public void load_text() {
        Battalion b = c.get_active_bat();
        foreach (int type in b.units.Keys) 
            load_text(b, type);
    }

    /*
    Load unit counts in unit placement sidebar.
    */
    public void load_text(Battalion b, int ID) {
        MapUI map_ui = c.map_ui;
        if (!texts.ContainsKey(ID) || !map_ui.unit_countsT.ContainsKey(ID))
            return;

        string num = b.count_placeable(ID).ToString();
        int total_num = b.units[ID].Count;
        int num_injured = b.count_injured(ID);

        texts[ID].text = num + " / " + total_num.ToString() + "    " + num_injured;

        map_ui.unit_countsT[ID].text = (total_num - num_injured) + 
            "         " + num_injured;
        unit_buttons[ID].interactable = total_num > 0;
    }

    // Called by battalion selection buttons
    public void set_selected_unit(int ID) {
        // Reset color of currently selected unit if one exists.
        if (unit_buttons.ContainsKey(selected_unit_type)) {
            unit_buttons[selected_unit_type].image.color = Color.white;
        }
        
        if (selecting_for_heal) { // Heal attribute
            if (c.get_active_bat().heal_injured_unit(ID)) {
                load_text(c.get_active_bat(), ID);
                selected_unit_type = ID;
                unit_buttons[ID].image.color = Controller.GREY;
            } else {
                cancel_heal();
            }
        } else {
            // Select and change color of new selection
            selected_unit_type = ID;
            unit_buttons[ID].image.color = Controller.GREY;
        }
    }

    public void cancel_heal() {
        if (healing_unit != null)
            healing_unit.set_attribute_active(false);
        healing_unit = null;
        selecting_for_heal = false;
        clear_placement_selection();
    }

    public void complete_heal() {
        healing_unit.num_actions--;
        cancel_heal();
    }

    public void clear_placement_selection() {
        if (selected_unit_type != PlayerUnit.EMPTY) 
            unit_buttons[selected_unit_type].image.color = Color.white;
        selected_unit_type = PlayerUnit.EMPTY;
    }

    public int get_selected_unit_ID() {
        if (selected_unit_type >= 0 && selected_unit_type < PlayerUnit.EMPTY) {
            return selected_unit_type;
        }
        return PlayerUnit.EMPTY;
    }
}
