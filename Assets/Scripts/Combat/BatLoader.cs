using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatLoader : MonoBehaviour {
    public Battalion astra_battalion;
    public Battalion endura_battalion;
    public Battalion martial_battalion;

    // Text fields in the unit selection scrollbar.
    public Dictionary<int, Text> texts = new Dictionary<int, Text>();
    public Text warrior_t;
    public Text spearman_t;
    public Text archer_t;
    public Text miner_t;
    public Text inspirator_t;
    public Text seeker_t;

    public Sprite empty; // UIMask image for a slot button image.

    // Button images in battle scene for highlighting selections.
    public IDictionary<int, Image> unit_button_imgs = new Dictionary<int, Image>();
    public Image archer_img;
    public Image warrior_img;
    public Image spearman_img;
    public Image inspiritor_img;
    public Image miner_img;

    // Drawn unit images.
    public Dictionary<int, Sprite> unit_images = new Dictionary<int, Sprite>();
    public Sprite warrior_I;
    public Sprite spearman_I;
    public Sprite archer_I;
    public Sprite miner_I;
    public Sprite inspirator_I;
    public Sprite seeker_I;

    void Start() {
        texts.Add(PlayerUnit.WARRIOR, warrior_t);
        texts.Add(PlayerUnit.SPEARMAN, spearman_t);
        texts.Add(PlayerUnit.ARCHER, archer_t);
        texts.Add(PlayerUnit.MINER, miner_t);
        texts.Add(PlayerUnit.INSPIRATOR, inspirator_t);
        texts.Add(PlayerUnit.SEEKER, seeker_t);

        // Populate unit placement button images dictionary.
        unit_button_imgs.Add(PlayerUnit.ARCHER, archer_img);
        unit_button_imgs.Add(PlayerUnit.WARRIOR, warrior_img);
        unit_button_imgs.Add(PlayerUnit.SPEARMAN, spearman_img);
        unit_button_imgs.Add(PlayerUnit.INSPIRATOR, inspiritor_img);
        unit_button_imgs.Add(PlayerUnit.MINER, miner_img);

        // Images to be loaded into slots.
        unit_images.Add(PlayerUnit.WARRIOR, warrior_I);
        //unit_images.Add(PlayerUnit.SPEARMAN, spearman_I);
        unit_images.Add(PlayerUnit.ARCHER, archer_I);
        unit_images.Add(PlayerUnit.MINER, miner_I);
        unit_images.Add(PlayerUnit.INSPIRATOR, inspirator_I);
        unit_images.Add(PlayerUnit.SEEKER, seeker_I);
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
