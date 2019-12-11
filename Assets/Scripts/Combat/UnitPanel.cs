using UnityEngine;
using UnityEngine.UI;

public class UnitPanel : MonoBehaviour {
    protected Selector selector;
    protected BattlePhaser bp;
    public Button closeB;
    public GameObject panel;
    public Text AttT;
    public Text unit_name;
    public Slot slot;

    void Awake() {
        selector = GameObject.Find("Selector").GetComponent<Selector>();
        bp = GameObject.Find("BattlePhaser").GetComponent<BattlePhaser>();
    }

    public void reposition(Slot slot) {
        Vector3 slot_pos = slot.transform.position;
        panel.transform.position = new Vector3(slot_pos.x, slot_pos.y - .3f, slot_pos.z);
    }
    
    public void set_name(string s) {
        unit_name.text = s;
    }

    public void close() {
        if (panel.activeSelf) {
            panel.SetActive(false); 
            selector.deselect();
        }
    }

    public virtual void update_panel(Slot slot) {}
    
    public void show_panel(Slot slot) {
        if (slot != null) {
            this.slot = slot;
            update_panel(slot);
            panel.SetActive(true); 
            reposition(slot);
        }
    }
}