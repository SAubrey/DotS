using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCellLight : MonoBehaviour {
    public UnityEngine.Experimental.Rendering.Universal.Light2D cell_light;
    public LightFlicker light_flicker;
    public LightGlow light_glow;

    public void init(MapCell cell) {
        if (light_flicker == null && light_glow == null)
            return;
        light_flicker.light2d = cell_light;
        light_glow.light2d = cell_light;
        
        if (MapUI.I.cell_light_colors.ContainsKey(cell.ID))
            set_color(MapUI.I.cell_light_colors[cell.ID]);
        set_glow(cell.glows);
        set_flicker(cell.flickers);
    }

    public void set_glow(bool active) {
        light_glow.active = active;
    }

    public void set_flicker(bool active) {
        light_flicker.active = active;
    }

    public void set_color(Color c) {
        cell_light.color = c;
    }
}
