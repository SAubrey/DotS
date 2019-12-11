using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatLoader : MonoBehaviour {
    public Battalion astra_battalion;
    public Battalion endura_battalion;
    public Battalion martial_battalion;

    public Text warrior_t;
    public Text spearman_t;
    public Text archer_t;
    public Text miner_t;
    public Text inspiritor_t;
    public Dictionary<int, Text> texts = new Dictionary<int, Text>();
    
    void Start() {
        texts.Add(PlayerUnit.WARRIOR, warrior_t);
        texts.Add(PlayerUnit.SPEARMAN, spearman_t);
        texts.Add(PlayerUnit.ARCHER, archer_t);
        texts.Add(PlayerUnit.MINER, miner_t);
        texts.Add(PlayerUnit.INSPIRITOR, inspiritor_t);
    }

    // This loads the player's battalion composition into the static
    // slots in the battle scene. 
    public void load_text(Battalion b) {
        foreach (int type in b.units.Keys) {
            string num = b.count_placeable(type).ToString();
            texts[type].text = num + " / " + b.units[type].Count.ToString();
        }
    }
}
