using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour {
    private Dictionary<int, GameObject> attack_renderers =
        new Dictionary<int, GameObject>();
    
    public GameObject line;
    public GameObject FieldPanel;
    private bool fading = false;
    private List<Unit> faders;
    private Unit fading_unit;
    private float timeout = AttackQueuer.WAIT_TIME;
    private float time_alive = 0;
    int id = 0; // unique identifier for each line.

/* void Update() {
        if (fading) {
            foreach (Unit u in faders) {
                GameObject l = attack_renderers[u];
                LineRenderer lr = l.GetComponent<LineRenderer>();
                if (u.get_type() == Unit.ENEMY) {
                    Vector4 sc = lr.startColor;
                    Vector4 ec = lr.endColor;
                    lr.startColor = new Color(sc[0], 0, 0, 0.1f);
                    lr.endColor = new Color(1, 0, 0, 1);
                } else {
                    lr.startColor = new Color(1, 1, 1, 0.1f);
                    lr.endColor = new Color(1, 1, 1, 1);
                }
            }
        }
    }*/    

    void Update() {
        if (!fading || fading_unit == null) return;

        time_alive += Time.deltaTime;
        if (time_alive >= timeout) {
            time_alive = 0;
            fading = false;
            remove(fading_unit.line_id);
            fading_unit = null;
        } else {
            GameObject l = attack_renderers[fading_unit.line_id];
            LineRenderer lr = l.GetComponent<LineRenderer>();
            Vector4 sc = lr.startColor;
            Vector4 ec = lr.endColor;
            lr.startColor = new Color(sc[0], sc[1], sc[2], (1 - (time_alive / timeout)) * 0.1f);
            lr.endColor = new Color(ec[0], ec[1], ec[2], 1 - (time_alive / timeout));
        }
    }

    public void draw_line(Unit u, Vector3 start_pos, Vector3 end_pos) {
        if (u == null) {
            Debug.Log("drawing null line");
            return;
        }
        GameObject l = GameObject.Instantiate(line);
        l.transform.SetParent(FieldPanel.transform, false);
        LineRenderer lr = l.GetComponent<LineRenderer>();
        lr.sortingLayerName = "Top";
        lr.positionCount = 2;
        lr.startWidth = .025f;
        lr.endWidth = .001f;
        if (u.get_type() == Unit.ENEMY) {
            lr.startColor = new Color(1, 0, 0, 0.1f);
            lr.endColor = new Color(1, 0, 0, 1);
        } else {
            lr.startColor = new Color(1, 1, 1, 0.1f);
            lr.endColor = new Color(1, 1, 1, 1);
        }
        lr.SetPosition(0, start_pos);
        lr.SetPosition(1, end_pos);

        // manage ID
        u.line_id = id;
        attack_renderers.Add(id, l);
        id++;
    }

    public void remove(int id) {
        if (attack_renderers.ContainsKey(id)) {
            GameObject.Destroy(attack_renderers[id]);
            attack_renderers.Remove(id);
        }
    }

    public void begin_fade(Unit u) {
        fading = true;
        fading_unit = u;
    }
}