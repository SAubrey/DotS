using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatLoader : MonoBehaviour {
    public Battalion astra_battalion, endura_battalion, martial_battalion;

    public Sprite white_fade_img, dark_fade_img;
    public Sprite empty; // UIMask image for a slot button image.
    // Unit quantity text fields in the unit selection scrollbar.
    public Dictionary<int, Text> texts = new Dictionary<int, Text>();
    public Text warrior_t, spearman_t, archer_t, miner_t, inspirator_t, seeker_t,
        vanguard_t, arbalest_t, skirmisher_t, paladin_t, mender_t, carter_t, dragoon_t,
        scout_t, drummer_t, guardian_t, pikeman_t;


    // Button images in battle scene for highlighting selections.
    public IDictionary<int, Image> unit_button_imgs = new Dictionary<int, Image>();
    public Image archer_img, warrior_img, spearman_img, inspirator_img, miner_img, 
        seeker_img, vanguard_img, arbalest_img, skirmisher_img, paladin_img, mender_img, carter_img, dragoon_img,
        scout_img, drummer_img, guardian_img, pikeman_img;

    // Drawn unit images.
    public Dictionary<int, Sprite> unit_images = new Dictionary<int, Sprite>();
    public Sprite warrior_I, spearman_I, archer_I, miner_I, inspirator_I, seeker_I,
        vanguard_I, arbalest_I, skirmisher_I, paladin_I, mender_I, carter_I, dragoon_I,
        scout_I, drummer_I, guardian_I, pikeman_I;

    void Start() {
        texts.Add(PlayerUnit.WARRIOR, warrior_t);
        texts.Add(PlayerUnit.SPEARMAN, spearman_t);
        texts.Add(PlayerUnit.ARCHER, archer_t);
        texts.Add(PlayerUnit.MINER, miner_t);
        texts.Add(PlayerUnit.INSPIRATOR, inspirator_t);
        texts.Add(PlayerUnit.SEEKER, seeker_t);
        texts.Add(PlayerUnit.VANGUARD, vanguard_t);
        texts.Add(PlayerUnit.ARBALEST, arbalest_t);
        texts.Add(PlayerUnit.SKIRMISHER, skirmisher_t);
        texts.Add(PlayerUnit.PALADIN, paladin_t);
        texts.Add(PlayerUnit.MENDER, mender_t);
        texts.Add(PlayerUnit.CARTER, carter_t);
        texts.Add(PlayerUnit.DRAGOON, dragoon_t);
        texts.Add(PlayerUnit.SCOUT, scout_t);
        texts.Add(PlayerUnit.DRUMMER, drummer_t);
        texts.Add(PlayerUnit.GUARDIAN, guardian_t);
        texts.Add(PlayerUnit.PIKEMAN, pikeman_t);

        // Populate unit placement button images dictionary.
        unit_button_imgs.Add(PlayerUnit.ARCHER, archer_img);
        unit_button_imgs.Add(PlayerUnit.WARRIOR, warrior_img);
        unit_button_imgs.Add(PlayerUnit.SPEARMAN, spearman_img);
        unit_button_imgs.Add(PlayerUnit.INSPIRATOR, inspirator_img);
        unit_button_imgs.Add(PlayerUnit.SEEKER, seeker_img);
        unit_button_imgs.Add(PlayerUnit.VANGUARD, vanguard_img);
        unit_button_imgs.Add(PlayerUnit.ARBALEST, arbalest_img);
        unit_button_imgs.Add(PlayerUnit.SKIRMISHER, skirmisher_img);
        unit_button_imgs.Add(PlayerUnit.PALADIN, paladin_img);
        unit_button_imgs.Add(PlayerUnit.MENDER, mender_img);
        unit_button_imgs.Add(PlayerUnit.CARTER, carter_img);
        unit_button_imgs.Add(PlayerUnit.DRAGOON, dragoon_img);
        unit_button_imgs.Add(PlayerUnit.SCOUT, scout_img);
        unit_button_imgs.Add(PlayerUnit.DRUMMER, drummer_img);
        unit_button_imgs.Add(PlayerUnit.GUARDIAN, guardian_img);
        unit_button_imgs.Add(PlayerUnit.PIKEMAN, pikeman_img);

        // Images to be loaded into slots.
        unit_images.Add(PlayerUnit.WARRIOR, warrior_I);
        unit_images.Add(PlayerUnit.SPEARMAN, spearman_I);
        unit_images.Add(PlayerUnit.ARCHER, archer_I);
        unit_images.Add(PlayerUnit.MINER, miner_I);
        unit_images.Add(PlayerUnit.INSPIRATOR, inspirator_I);
        unit_images.Add(PlayerUnit.SEEKER, seeker_I);
        unit_images.Add(PlayerUnit.VANGUARD, vanguard_I);
        unit_images.Add(PlayerUnit.ARBALEST, arbalest_I);
        unit_images.Add(PlayerUnit.SKIRMISHER, skirmisher_I);
        unit_images.Add(PlayerUnit.PALADIN, paladin_I);
        unit_images.Add(PlayerUnit.MENDER, mender_I);
        unit_images.Add(PlayerUnit.CARTER, carter_I);
        unit_images.Add(PlayerUnit.DRAGOON, dragoon_I);
        unit_images.Add(PlayerUnit.SCOUT, scout_I);
        unit_images.Add(PlayerUnit.DRUMMER, drummer_I);
        unit_images.Add(PlayerUnit.GUARDIAN, guardian_I);
        unit_images.Add(PlayerUnit.PIKEMAN, pikeman_I);
    }

    // This loads the player's battalion composition into the static
    // slots in the battle scene. 
    public void load_text(Battalion b) {
        MapUI map_ui = b.c.map_ui;
        foreach (int type in b.units.Keys) {
            if (!texts.ContainsKey(type) || !map_ui.unit_countsT.ContainsKey(type))
                continue;

            string num = b.count_placeable(type).ToString();
            int total_num = b.units[type].Count;
            int num_injured = b.count_injured(type);

            texts[type].text = num + " / " + total_num.ToString() + "    " + num_injured;

            map_ui.unit_countsT[type].text = (total_num - num_injured) + 
                "         " + num_injured;
        }
    }
}
