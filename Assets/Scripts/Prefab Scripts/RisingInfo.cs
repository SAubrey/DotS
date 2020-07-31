using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RisingInfo : MonoBehaviour {
    protected float timeout = AttackQueuer.WAIT_TIME;
    protected float time_alive = 0;
    public Text fg_T;
    public Text bg_T;

    public void init(string resource, int value, Color color) {
        string readout = "";
        if (value > 0) {
            readout += "+ ";
        }
        readout += value.ToString() + " " + resource;
        set_text(readout);
        fg_T.color = color;
    }

    protected virtual void Update() {
        time_alive += Time.deltaTime;
        translate_up(.01f);
        fade();
        if (time_alive > timeout)
            die(); 
    }

    protected void translate_up(float dy) {
        this.transform.Translate(0, dy, 0);
    }

    protected void fade() {
        fg_T.color = new Color (fg_T.color[0], fg_T.color[1], fg_T.color[2], 1 - (time_alive / timeout));
        bg_T.color = new Color(.1f, .1f, .1f, 1 - (time_alive / timeout));
    }

    public void show() {
        fg_T.enabled = true;
        bg_T.enabled = true;
    }

    protected void set_text(string text) {
        fg_T.text = text;
        bg_T.text = text;
    }

    protected void die() {
        Destroy(gameObject);
    }
}
