﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Object exists for duration of the visual hitsplat.
public class HitSplat : RisingInfo { 
    public static Color BLUE = new Color(.1f, .1f, 1, 1);
    public static Color RED = new Color(1, .1f, .1f, 1);
    public static Color ORANGE = new Color(1, .6f, 0, 1);
    
    public void init(int dmg, int state, Slot end_slot) {
        set_text(dmg.ToString());
        set_color(state);
        transform.position = end_slot.transform.position;
    }

    private void set_color(int state) {
        if (state == Unit.ALIVE)
            fg_T.color = BLUE;
        else if (state == Unit.DEAD)
            fg_T.color = RED;
        else if (state == Unit.INJURED)
            fg_T.color = ORANGE;
    }
}
