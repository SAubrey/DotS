using UnityEngine;

public class UnitPanelManager : MonoBehaviour {
    public static UnitPanelManager I { get; private set; }
    public EnemyPanel enemy_panel;
    public PlayerPanel player_panel;
    void Awake() {
        if (I == null) {
            I = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        enemy_panel.close();
        player_panel.close();
    }

    public void update(Slot slot) {
        Debug.Log("updating");
        if (slot.has_punit) {
            player_panel.update_panel(slot);
        } else if (slot.has_enemy) {
            enemy_panel.update_panel(slot);
        }
    }

    public void update_text(Slot slot) {
        if (slot.has_punit) {
            player_panel.update_text(slot.get_punit());
        } else if (slot.has_punit) {
            //enemy_panel.update_text(slot);
        }
    }

    public void close(int unit_type=-1) {
        if (unit_type == -1) {
            player_panel.close();
            enemy_panel.close();
        } else if (unit_type == Unit.PLAYER) {
            player_panel.close();
        } else {
            enemy_panel.close();
        }
    }

    public void show(Slot slot) {
        if (slot.has_punit) {
            player_panel.show_panel(slot);
            enemy_panel.panel.SetActive(false);
        } else if (slot.has_enemy) {
            enemy_panel.show_panel(slot);
            player_panel.panel.SetActive(false);
        }
    }
}

