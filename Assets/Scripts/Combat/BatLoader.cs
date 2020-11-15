using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
Battalion loader.
Pertains to the player unit placement selection bar and placed player/enemy unit images for slots.
*/
public class BatLoader : MonoBehaviour {
    public static BatLoader I { get; private set; }
    //public Sprite white_fade_img, dark_fade_img;
    public Sprite empty; // UIMask image for a slot button image.
    // Unit quantity text fields in the unit selection scrollbar.
    public Dictionary<int, TextMeshProUGUI> texts = new Dictionary<int, TextMeshProUGUI>();
    public TextMeshProUGUI warrior_t, spearman_t, archer_t, miner_t, inspirator_t, seeker_t,
        guardian_t, arbalest_t, skirmisher_t, paladin_t, mender_t, carter_t, dragoon_t,
        scout_t, drummer_t, shield_maiden_t, pikeman_t;


    // Button images in battle scene for highlighting selections.
    public IDictionary<int, Button> unit_buttons = new Dictionary<int, Button>();
    public Button archer_B, warrior_B, spearman_B, inspirator_B, miner_B, 
        seeker_B, guardian_B, arbalest_B, skirmisher_B, paladin_B, mender_B, carter_B, dragoon_B,
        scout_B, drummer_B, shield_maiden_B, pikeman_B;


    // Drawn unit images.
    // Player units
    public Dictionary<int, Sprite> unit_images_back = new Dictionary<int, Sprite>();
    public Sprite warrior_b, spearman_b, archer_b, miner_b, inspirator_b, seeker_b,
        guardian_b, arbalest_b, skirmisher_b, paladin_b, mender_b, carter_b, dragoon_b,
        scout_b, drummer_b, shield_maiden_b, pikeman_b;
    public Dictionary<int, Sprite> unit_images_front = new Dictionary<int, Sprite>();
    public Sprite warrior_f, spearman_f, archer_f, miner_f, inspirator_f, seeker_f,
        guardian_f, arbalest_f, skirmisher_f, paladin_f, mender_f, carter_f, dragoon_f,
        scout_f, drummer_f, shield_maiden_f, pikeman_f;

    // Enemy units
    public Dictionary<int, Sprite> enemy_images_back = new Dictionary<int, Sprite>();
    public Dictionary<int, Sprite> enemy_images_front = new Dictionary<int, Sprite>();
    // Plains
    public Sprite galtsa_b, grem_b, endu_b, korote_b, molner_b, etuena_b, clypte_b, goliath_b;
    public Sprite galtsa_f, grem_f, endu_f, korote_f, molner_f, etuena_f, clypte_f, goliath_f;
    // Forest
    public Sprite kverm_b, latu_b, eke_tu_b, oetem_b, eke_fu_b, eke_shi_ami_b, eke_lord_b, ketemcol_b;
    public Sprite kverm_f, latu_f, eke_tu_f, oetem_f, eke_fu_f, eke_shi_ami_f, eke_lord_f, ketemcol_f;
    // Titrum
    public Sprite mahukin_b, drongo_b, maheket_b, calute_b, etalket_b, muatem_b;
    public Sprite mahukin_f, drongo_f, maheket_f, calute_f, etalket_f, muatem_f;
    // Mountain/Cliff
    public Sprite drak_b, zerrku_b, gokin_b;
    public Sprite drak_f, zerrku_f, gokin_f;
    // Cave
    public Sprite tajaqar_b, tajaero_b, terra_qual_b, duale_b;
    public Sprite tajaqar_f, tajaero_f, terra_qual_f, duale_f;

    public Sprite meld_warrior_b, meld_spearman_b;
    public Sprite meld_warrior_f, meld_spearman_f;

    public bool selecting_for_heal = false;
    public TextMeshProUGUI discT;
    public PlayerUnit healing_unit;
    private int selected_unit_type = 0;


    public Sprite get_unit_img(Unit unit, int direction) {
        if (unit != null)
            return get_unit_direction_img(unit.get_type(), unit.get_ID(), direction);
        return null;
    }

    private Sprite get_unit_direction_img(int unit_type, int unit_ID, int direction) {
        if (unit_type == Unit.PLAYER) {
            if (direction == Group.UP || direction == Group.RIGHT)
                return unit_images_back[unit_ID];
            return unit_images_front[unit_ID];
        } else {
            if (direction == Group.UP || direction == Group.RIGHT)
                return enemy_images_back[unit_ID];
            return enemy_images_front[unit_ID];
        }
    }

    void Awake() {
        if (I == null) {
            I = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

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
        unit_images_back.Add(PlayerUnit.WARRIOR, warrior_b);
        unit_images_back.Add(PlayerUnit.SPEARMAN, spearman_b);
        unit_images_back.Add(PlayerUnit.ARCHER, archer_b);
        unit_images_back.Add(PlayerUnit.MINER, miner_b);
        unit_images_back.Add(PlayerUnit.INSPIRATOR, inspirator_b);
        unit_images_back.Add(PlayerUnit.SEEKER, seeker_b);
        unit_images_back.Add(PlayerUnit.SHIELD_MAIDEN, shield_maiden_b);
        unit_images_back.Add(PlayerUnit.ARBALEST, arbalest_b);
        unit_images_back.Add(PlayerUnit.SKIRMISHER, skirmisher_b);
        unit_images_back.Add(PlayerUnit.PALADIN, paladin_b);
        unit_images_back.Add(PlayerUnit.MENDER, mender_b);
        unit_images_back.Add(PlayerUnit.CARTER, carter_b);
        unit_images_back.Add(PlayerUnit.DRAGOON, dragoon_b);
        unit_images_back.Add(PlayerUnit.SCOUT, scout_b);
        unit_images_back.Add(PlayerUnit.DRUMMER, drummer_b);
        unit_images_back.Add(PlayerUnit.GUARDIAN, guardian_b);
        unit_images_back.Add(PlayerUnit.PIKEMAN, pikeman_b);

        unit_images_front.Add(PlayerUnit.WARRIOR, warrior_f);
        unit_images_front.Add(PlayerUnit.SPEARMAN, spearman_f);
        unit_images_front.Add(PlayerUnit.ARCHER, archer_f);
        unit_images_front.Add(PlayerUnit.MINER, miner_f);
        unit_images_front.Add(PlayerUnit.INSPIRATOR, inspirator_f);
        unit_images_front.Add(PlayerUnit.SEEKER, seeker_f);
        unit_images_front.Add(PlayerUnit.SHIELD_MAIDEN, shield_maiden_f);
        unit_images_front.Add(PlayerUnit.ARBALEST, arbalest_f);
        unit_images_front.Add(PlayerUnit.SKIRMISHER, skirmisher_f);
        unit_images_front.Add(PlayerUnit.PALADIN, paladin_f);
        unit_images_front.Add(PlayerUnit.MENDER, mender_f);
        unit_images_front.Add(PlayerUnit.CARTER, carter_f);
        unit_images_front.Add(PlayerUnit.DRAGOON, dragoon_f);
        unit_images_front.Add(PlayerUnit.SCOUT, scout_f);
        unit_images_front.Add(PlayerUnit.DRUMMER, drummer_f);
        unit_images_front.Add(PlayerUnit.GUARDIAN, guardian_f);
        unit_images_front.Add(PlayerUnit.PIKEMAN, pikeman_f);

        // Enemies
        // Plains
        enemy_images_back.Add(Enemy.GALTSA, galtsa_b);
        enemy_images_front.Add(Enemy.GALTSA, galtsa_f);
        enemy_images_back.Add(Enemy.GREM, grem_b);
        enemy_images_front.Add(Enemy.GREM, grem_f);
        enemy_images_back.Add(Enemy.ENDU, endu_b);
        enemy_images_front.Add(Enemy.ENDU, endu_f);
        enemy_images_back.Add(Enemy.KOROTE, korote_b);
        enemy_images_front.Add(Enemy.KOROTE, korote_f);
        enemy_images_back.Add(Enemy.MOLNER, molner_b);
        enemy_images_front.Add(Enemy.MOLNER, molner_f);
        enemy_images_back.Add(Enemy.ETUENA, etuena_b);
        enemy_images_front.Add(Enemy.ETUENA, etuena_f);
        enemy_images_back.Add(Enemy.CLYPTE, clypte_b);
        enemy_images_front.Add(Enemy.CLYPTE, clypte_f);
        enemy_images_back.Add(Enemy.GOLIATH, goliath_b);
        enemy_images_front.Add(Enemy.GOLIATH, goliath_f);
        // Forest
        enemy_images_back.Add(Enemy.KVERM, kverm_b);
        enemy_images_front.Add(Enemy.KVERM, kverm_f);
        enemy_images_back.Add(Enemy.LATU, latu_b);
        enemy_images_front.Add(Enemy.LATU, latu_f);
        enemy_images_back.Add(Enemy.EKE_TU, eke_tu_b);
        enemy_images_front.Add(Enemy.EKE_TU, eke_tu_f);
        enemy_images_back.Add(Enemy.OETEM, oetem_b);
        enemy_images_front.Add(Enemy.OETEM, oetem_f);
        enemy_images_back.Add(Enemy.EKE_FU, eke_fu_b);
        enemy_images_front.Add(Enemy.EKE_FU, eke_fu_f);
        enemy_images_back.Add(Enemy.EKE_SHI_AMI, eke_shi_ami_b);
        enemy_images_front.Add(Enemy.EKE_SHI_AMI, eke_shi_ami_f);
        enemy_images_back.Add(Enemy.EKE_LORD, eke_lord_b);
        enemy_images_front.Add(Enemy.EKE_LORD, eke_lord_f);
        enemy_images_back.Add(Enemy.KETEMCOL, ketemcol_b);
        enemy_images_front.Add(Enemy.KETEMCOL, ketemcol_f);
        // Titrum
        enemy_images_back.Add(Enemy.MAHUKIN, mahukin_b);
        enemy_images_front.Add(Enemy.MAHUKIN, mahukin_f);
        enemy_images_back.Add(Enemy.DRONGO, drongo_b);
        enemy_images_front.Add(Enemy.DRONGO, drongo_f);
        enemy_images_back.Add(Enemy.MAHEKET, maheket_b);
        enemy_images_front.Add(Enemy.MAHEKET, maheket_f);
        enemy_images_back.Add(Enemy.CALUTE, calute_b);
        enemy_images_front.Add(Enemy.CALUTE, calute_f);
        enemy_images_back.Add(Enemy.ETALKET, etalket_b);
        enemy_images_front.Add(Enemy.ETALKET, etalket_f);
        enemy_images_back.Add(Enemy.MUATEM, muatem_b);
        enemy_images_front.Add(Enemy.MUATEM, muatem_f);

        // Mountains/Cliff
        enemy_images_back.Add(Enemy.DRAK, drak_b);
        enemy_images_front.Add(Enemy.DRAK, drak_f);
        enemy_images_back.Add(Enemy.ZERRKU, zerrku_b);
        enemy_images_front.Add(Enemy.ZERRKU, zerrku_f);
        enemy_images_back.Add(Enemy.GOKIN, gokin_b);
        enemy_images_front.Add(Enemy.GOKIN, gokin_f);
        // Cave
        enemy_images_back.Add(Enemy.TAJAQAR, tajaqar_b);
        enemy_images_front.Add(Enemy.TAJAQAR, tajaqar_f);
        enemy_images_back.Add(Enemy.TAJAERO, tajaero_b);
        enemy_images_front.Add(Enemy.TAJAERO, tajaero_f);
        enemy_images_back.Add(Enemy.TERRA_QUAL, terra_qual_b);
        enemy_images_front.Add(Enemy.TERRA_QUAL, terra_qual_f);
        enemy_images_back.Add(Enemy.DUALE, duale_b);
        enemy_images_front.Add(Enemy.DUALE, duale_f);

        // Meld
        enemy_images_back.Add(Enemy.MELD_WARRIOR, meld_warrior_b);
        enemy_images_front.Add(Enemy.MELD_WARRIOR, meld_warrior_f);
        
        enemy_images_back.Add(Enemy.MELD_SPEARMAN, meld_spearman_b);
        enemy_images_front.Add(Enemy.MELD_SPEARMAN, meld_spearman_f);
    }

    // This loads the player's battalion composition into the static
    // slots in the battle scene. 
    public void load_bat(Battalion b) {
        foreach (int type_ID in b.units.Keys) {
            load_unit_text(b, type_ID);
            unit_buttons[type_ID].interactable = b.units[type_ID].Count > 0;
        }
        Debug.Log("loading bat in battlefield: " + b.disc.name);
        MapUI.I.highlight_discipline(discT, b.disc.ID);
    }

    public void load_unit_text(Battalion b, int ID) {
        texts[ID].text = build_unit_text(b, ID);
    }

    /*
    Load unit counts in unit placement sidebar.
    */
    private string build_unit_text(Battalion b, int ID) {
        if (!texts.ContainsKey(ID))
            return "";
        string num = b.count_placeable(ID).ToString();
        int total_num = b.units[ID].Count;
        int num_injured = b.count_injured(ID);
        return num + " / " + total_num.ToString() + "    " + num_injured;
    }

    // Called by battalion selection buttons
    public void set_selected_unit(int ID) {
        // Reset color of currently selected unit if one exists.
        if (unit_buttons.ContainsKey(selected_unit_type)) {
            unit_buttons[selected_unit_type].image.color = Color.white;
        }
        
        if (selecting_for_heal) { // Heal attribute
            if (BattlePhaser.I.active_bat.heal_injured_unit(ID)) {
                load_unit_text(BattlePhaser.I.active_bat, ID);
                selected_unit_type = ID;
                unit_buttons[ID].image.color = Statics.DISABLED_C;
            } else {
                cancel_heal();
            }
        } else {
            // Select and change color of new selection
            selected_unit_type = ID;
            unit_buttons[ID].image.color = Statics.DISABLED_C;
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
