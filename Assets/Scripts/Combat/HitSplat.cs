using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Object exists for duration of the visual hitsplat.
public class HitSplat : MonoBehaviour { 
    public Color BLUE = new Color(.1f, .1f, 1, 1);
    public Color RED = new Color(1, .1f, .1f, 1);
    public Color ORANGE = new Color(1, .6f, 0, 1);
    private float timeout = AttackQueuer.WAIT_TIME;
    private float time_alive = 0;
    public Text fg_T;
    public Text bg_T;

    public void init(Attack att) {
        int dmg = att.calc_dmg(); // Damage accounting for defense.
        set_text(dmg);

        if (att.direction == AttackQueuer.E_TO_PU) {
            set_color(att.get_punit().get_post_dmg_state(dmg));
        } else {
            if (dmg > 0)
                set_color(Unit.DEAD);
            else
                set_color(Unit.ALIVE);
        }
    }

    void Update() {
        time_alive += Time.deltaTime;
        translate_up();
        fade();
        if (time_alive > timeout)
            die(); 
    }

    private void translate_up() {
        this.transform.Translate(0, 0.0015f, 0);
    }

    private void fade() {
        fg_T.color = new Color (fg_T.color[0], fg_T.color[1], fg_T.color[2], 1 - (time_alive / timeout));
        bg_T.color = new Color(.9f, .9f, .9f, 1 - (time_alive / timeout));
    }

    private void set_color(int state) {
        if (state == Unit.ALIVE)
            fg_T.color = BLUE;
        else if (state == Unit.DEAD)
            fg_T.color = RED;
        else if (state == Unit.INJURED)
            fg_T.color = ORANGE;
    }

    private void set_text(int dmg) {
        fg_T.text = dmg.ToString();
        bg_T.text = dmg.ToString();
    }

    private void die() {
        Destroy(gameObject);
    }
}
