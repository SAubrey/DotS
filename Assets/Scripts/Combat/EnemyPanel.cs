using UnityEngine;
using UnityEngine.UI;

public class EnemyPanel : UnitPanel {
    public Text HpT;
    public Text XpT;
    
    public override void update_panel(Slot slot) {
        if (slot.get_punit() != null)
            return;

        Enemy enemy = slot.get_enemy();
        set_name(enemy.get_name());
        AttT.text = enemy.get_raw_attack_dmg().ToString();
        XpT.text = enemy.xp.ToString();
        HpT.text = (enemy.health + " / " + enemy.max_health);
    }
}