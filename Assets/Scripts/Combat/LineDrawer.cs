﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour {
    public GameObject Line;
    public GameObject FieldPanel;
    public Dictionary<int, Line> lines = new Dictionary<int, Line>();
    private const float TIMEOUT = AttackQueuer.WAIT_TIME;
    int id = 0; // unique identifier for each line.
  
    public void draw_line(Unit u, Vector3 start_pos, Vector3 end_pos) {
        if (u == null) {
            Debug.Log("not drawing null line");
            return;
        }
        GameObject L = GameObject.Instantiate(Line);
        Line line = L.GetComponent<Line>();
        line.init(FieldPanel, u, id, start_pos, end_pos);

        // manage ID
        u.line_id = id;
        lines.Add(id, line);
        id++;
    }

    public Line get_line(int id) {
        return lines[id];
    }

    public void remove(int id) {
        if (lines.ContainsKey(id)) {
            lines[id].remove();
            lines.Remove(id);
        }
    }
}