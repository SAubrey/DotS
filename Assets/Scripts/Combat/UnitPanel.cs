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
    public PlayerUnit punit;
    private Camera cam;

    public virtual void update_panel(Slot slot) {}

    void Awake() {
        selector = GameObject.Find("Selector").GetComponent<Selector>();
        bp = GameObject.Find("BattlePhaser").GetComponent<BattlePhaser>();
        cam = GameObject.Find("BattleCamera").GetComponent<Camera>();
    }

    public void reposition(Slot slot) {
        Vector3 slot_pos = cam.WorldToScreenPoint(slot.transform.position);
        panel.transform.position = new Vector3(slot_pos.x, slot_pos.y - 50f, 0);
    }
    
    public void set_name(string s) {
        unit_name.text = s;
    }

    public virtual void close() {
        if (panel.activeSelf) {
            panel.SetActive(false); 
            selector.deselect();
            slot = null;
            punit = null;
        }
    }

    
    public void rotate(int direction) {
        slot.get_group().rotate(direction);
    }

    public void show_panel(Slot slot) {
        if (slot != null) {
            this.slot = slot;
            if (slot.has_punit) {
                this.punit = slot.get_punit();
            }
            update_panel(slot);
            panel.SetActive(true); 
            reposition(slot);
        }
    }
}