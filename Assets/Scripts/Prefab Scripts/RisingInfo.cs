using UnityEngine;
using TMPro;

public class RisingInfo : MonoBehaviour {
    protected float timeout = AttackQueuer.WAIT_TIME;
    protected float time_alive = 0;
    //public Text T;
    public TextMeshProUGUI T;
    protected float translation_distance = 0.01f;

    public void init(string resource, int value, Color color) {
        string readout = "";
        if (value > 0) {
            readout = "+ ";
        }
        readout += value.ToString() + " " + resource;
        set_text(readout);
        T.color = color;
    }

    protected virtual void Update() {
        time_alive += Time.deltaTime;
        translate_up(translation_distance);
        fade();
        if (time_alive > timeout)
            die(); 
    }

    protected void translate_up(float dy) {
        this.transform.Translate(0, dy, 0);
    }

    protected void fade() {
        /* Dilation experiment
        if (time_alive < AttackQueuer.WAIT_TIME / 2)
            dilation = time_alive / (AttackQueuer.WAIT_TIME / 2);
        else 
            dilation = (AttackQueuer.WAIT_TIME - time_alive) / (AttackQueuer.WAIT_TIME / 2);
        T.material.SetFloat(ShaderUtilities.ID_FaceDilate, dilation);
        Debug.Log(T.material.GetFloat(ShaderUtilities.ID_FaceDilate)); */
        T.color = new Color (T.color[0], T.color[1], T.color[2], 1 - (time_alive / timeout));
    }

    public void show() {
        T.enabled = true;
    }

    protected void set_text(string text) {
        T.text = text;
    }

    protected void die() {
        Destroy(gameObject);
    }
}
