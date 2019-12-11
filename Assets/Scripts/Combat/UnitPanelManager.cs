using UnityEngine;

public class UnitPanelManager : MonoBehaviour {
    public EnemyPanel enemy_panel;
    public PlayerPanel player_panel;

    void Start() {
        enemy_panel.close();
        player_panel.close();
    }

    public void update(Slot slot) {
        if (slot.get_punit() != null) {
            player_panel.update_panel(slot);
        } else if (slot.get_enemy() != null) {
            enemy_panel.update_panel(slot);
        }
    }

    public void close(int unit_type) {
        if (unit_type == Unit.PLAYER) {
            player_panel.close();
        } else {
            enemy_panel.close();
        }
    }

    public void show(Slot slot) {
        if (slot.get_punit() != null) {
            player_panel.show_panel(slot);
            enemy_panel.panel.SetActive(false);
        } else if (slot.get_enemy() != null) {
            enemy_panel.show_panel(slot);
            player_panel.panel.SetActive(false);
        }
    }
}

