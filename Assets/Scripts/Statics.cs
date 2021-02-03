using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Fields that are not particular to a class and do not need to be instantiated more than once.
*/
public class Statics : MonoBehaviour {
    //public static GameObject rising_info_prefab, hitsplat_prefab;
    // UI CONSTANTS
    public static readonly Color DISABLED_C = new Color(.78125f, .78125f, .78125f, 1);
    public static readonly Color ASTRA_COLOR = new Color(.6f, .6f, 1, 1);
    public static readonly Color ENDURA_COLOR = new Color(1, 1, .6f, 1);
    public static readonly Color MARTIAL_COLOR = new Color(1, .6f, .6f, 1);
    public static readonly Color[] disc_colors = {ASTRA_COLOR, ENDURA_COLOR, MARTIAL_COLOR, Color.white};
    public static Color BLUE = new Color(.1f, .1f, 1, 1);
    public static Color RED = new Color(1, .1f, .1f, 1);
    public static Color ORANGE = new Color(1, .6f, 0, 1);

    public static readonly Vector3 CITY_POS = new Vector3(10.5f, 10.5f, 0);

    public static Color get_unit_state_color(int state) {
        if (state == Unit.ALIVE)
            return BLUE;
        else if (state == Unit.DEAD)
            return RED;
        else if (state == Unit.INJURED)
            return ORANGE;
        return Color.white;
    }

    public static int calc_distance(Slot start, Slot end) {
        int dx = Mathf.Abs(start.col - end.col);
        int dy = Mathf.Abs(start.row - end.row);
        return dx + dy;
    }

    public static int calc_map_distance(Pos pos1, Pos pos2) {
        int dx = Mathf.Abs(pos1.x - pos2.x);
        int dy = Mathf.Abs(pos1.y - pos2.y);
        return dx + dy;
    }

    public static int valid_nonnegative_change(int starting_amount, int change) {
        if (starting_amount + change < 0) {
            return starting_amount;
        }
        return change;
    }

    
    public static RisingInfo create_rising_info(string text, Color color, Transform origin, Transform parent, GameObject rising_info_prefab) {
        GameObject ri = GameObject.Instantiate(rising_info_prefab, parent.transform);
        RisingInfo ri_script = ri.GetComponent<RisingInfo>();
        ri_script.init(origin, text, color);
        return ri_script;
    }

    public static void create_rising_info_map(string text, Color color, Transform origin, GameObject prefab) {
        RisingInfo ri_script = create_rising_info(text, color, origin, CamSwitcher.I.mapUI_canvas.transform, prefab);
        ri_script.translation_distance = 0.01f;
        ri_script.show();
    }

    public static void create_rising_info_battle(string text, Color color, Transform origin, GameObject prefab) {
        RisingInfo ri_script = create_rising_info(text, color, origin, CamSwitcher.I.battle_canvas.transform, prefab);
        ri_script.translation_distance = 1f;
        ri_script.show();
    }
}