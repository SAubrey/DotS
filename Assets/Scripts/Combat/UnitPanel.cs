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
    public GameObject infoP;

    public virtual void update_panel(Slot slot) {}

    void Awake() {
        selector = GameObject.Find("Selector").GetComponent<Selector>();
        bp = GameObject.Find("BattlePhaser").GetComponent<BattlePhaser>();
        cam = GameObject.Find("BattleCamera").GetComponent<Camera>();
    }

    public void reposition(Slot slot) {
        Vector3 slot_pos = cam.WorldToScreenPoint(slot.transform.position);
        panel.transform.position = new Vector3(slot_pos.x, slot_pos.y - 200f, 0);
    }
    
    public void set_name(string s) {
        unit_name.text = s;
    }

    public virtual void close() {
        if (panel.activeSelf) {
            infoP.SetActive(false);
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
        if (slot == null)
            return;
        this.slot = slot;
        if (slot.has_punit) {
            this.punit = slot.get_punit();
        }
        update_panel(slot);
        update_info_panel();
        reposition(slot);
        panel.SetActive(true); 
    }

    public void toggle_info_panel() {
        infoP.SetActive(!infoP.activeSelf);
        if (infoP.activeSelf)
            update_info_panel();
    }

    public void update_info_panel() {
        if (slot == null)
            return;
        Text t = infoP.GetComponentInChildren<Text>();
        AttributeWriter.write_attribute_text(t, slot.get_unit());
    }
}