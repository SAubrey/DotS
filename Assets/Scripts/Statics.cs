using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Fields that are not particular to a class and do not need to be instantiated more than once.
*/
public class Statics : MonoBehaviour{
    // UI CONSTANTS
    public static readonly Color DISABLED_C = new Color(.78125f, .78125f, .78125f, 1);
    public static readonly Color ASTRA_COLOR = new Color(.6f, .6f, 1, 1);
    public static readonly Color ENDURA_COLOR = new Color(1, 1, .6f, 1);
    public static readonly Color MARTIAL_COLOR = new Color(1, .6f, .6f, 1);
    public static readonly Color[] disc_colors = {ASTRA_COLOR, ENDURA_COLOR, MARTIAL_COLOR, Color.white};

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

}