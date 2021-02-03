using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLog : MonoBehaviour {
    public List<string> log = new List<string>();

    void Start() {
        
    }

    void Update() {
        
    }

    public void write(string text) {
        log.Add(text);
    }
}
