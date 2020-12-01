using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundLoader : MonoBehaviour {
    public static BackgroundLoader I { get; private set; }
    public Image field_panel_img;
    public Dictionary<int, Sprite> background_imgs = new Dictionary<int, Sprite>();
    public Sprite plains, forest, titrum;

    void Awake() {
        if (I == null) {
            I = this;
        } else {
            Destroy(gameObject);
        }
    }
    void Start() {
        field_panel_img = GameObject.Find("FieldPanel").GetComponent<Image>();
        background_imgs.Add(MapCell.PLAINS_ID, plains);
        background_imgs.Add(MapCell.FOREST_ID, forest);
        background_imgs.Add(MapCell.TITRUM_ID, titrum);
        
    }

    public void load(int biome_ID) {
        if (background_imgs.ContainsKey(biome_ID))
            field_panel_img.sprite = background_imgs[biome_ID];
        else
            field_panel_img.sprite = plains;
    }
}
