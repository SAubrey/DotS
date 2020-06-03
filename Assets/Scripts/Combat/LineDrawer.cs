﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour {
    public GameObject LinePrefab;
    public PreviewLine preview_line;
    public Dictionary<int, Line> lines = new Dictionary<int, Line>();

    void Start() {
        preview_line = GetComponentInChildren<PreviewLine>();
        preview_line.init();
    }

    public void draw_line(Unit start_u, Vector3 start_pos, Vector3 end_pos, int id) {
        if (start_u == null) {
            return;
        }
        GameObject L = GameObject.Instantiate(LinePrefab);
        Line line = L.GetComponent<Line>();
        line.init(start_u, id, start_pos, end_pos);

        // manage ID
        lines.Add(id, line);
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

    public void clear() {
        lines.Clear();
        preview_line.erase();
    }
}