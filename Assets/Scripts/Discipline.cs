using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Discipline : Storeable, ISaveLoad {
    public const string EXPERIENCE = "experience";
    public const int ASTRA = 0, ENDURA = 1, MARTIAL = 2;
    public GameObject piece;
    public Battalion bat { get; private set; }
    public bool active = false;
    public bool restart_battle_from_drawn_card = false;
    private int mine_qty_multiplier = 3;
    public int mine_qty {
        get => mine_qty_multiplier *= bat.count_placeable(PlayerUnit.MINER);
    }
    public bool has_mined_in_turn, has_moved_in_turn, has_scouted_in_turn = false;
    public bool has_acted_in_turn { get => has_moved_in_turn || has_scouted_in_turn; }
    public bool dead { get; private set; } = false;
    private MapCell _cell;
    public MapCell cell {
        get => _cell;
        set {
            if (value == null)
                return;
            previous_cell = cell;
            _cell = value;
            pos = cell.pos.to_vec3;
        }
    }
    public MapCell previous_cell { get; private set; }
    private Vector3 _pos;
    public Vector3 pos {
        get { return _pos; }
        set {
            _pos = value;
            piece.transform.position = new Vector3(value.x, value.y, 0);
        }
    }
    
    public EquipmentInventory equipment_inventory;
    public event Action on_unit_count_change;
    public void trigger_unit_count_change() { on_unit_count_change(); }

    protected override void Start() {
        base.Start();
        resources.Add(EXPERIENCE, 0);
        mine_qty_multiplier = ID == ENDURA ? 4 : 3;
    }

    public override void init(bool from_save) {
        base.init(from_save);
        bat = new Battalion(this);
        equipment_inventory = new EquipmentInventory(this);

        resources[LIGHT] = 4;
        resources[UNITY] = 4;
        resources[STAR_CRYSTALS] = 1;
        resources[EXPERIENCE] = 500;
        cell = Map.I.city_cell;
    }

    public override void register_turn() {
        if (!dead) {
            base.register_turn();
            check_insanity(get_res(UNITY));
        }
        has_mined_in_turn = false;
        has_moved_in_turn = false;
        has_scouted_in_turn = false;
    }

    public void move(MapCell cell) {
        // Offset position on cell to simulate human placement and prevent perfect overlap.
        this.cell = cell;
        float randx = UnityEngine.Random.Range(-0.2f, 0.2f); 
        float randy = UnityEngine.Random.Range(-0.2f, 0.2f);
        pos = new Vector3(cell.pos.x + 0.5f + randx, cell.pos.y + 0.5f + randy, 0);
        has_moved_in_turn = true;
        MapUI.I.update_cell_text(cell.name);
        cell.enter();
    }

    public void move_to_previous_cell() {
        move(previous_cell);
    }

    /*
    Upon death, move the player piece to the city,
    reset troop composition to default, 
    lose all resources,
    drop equipment and experience on the cell of death to be retrieved.
    */
    public void die() {
        if (!cell.battle.is_group) {
            cell.battle.end();
        }
        bat.kill_injured_units();
        remove_resources_lost_on_death();
        Map.I.get_cell(pos).drop_XP(resources[EXPERIENCE]);
        Debug.Log(pos);
        pos = new Vector3(-100, -100, 0);
        dead = true;
    }

    public void respawn() {
        bat.add_default_troops();
        move(Map.I.city_cell);
        has_moved_in_turn = false;
        dead = false;
    }

    private void check_insanity(int unity) {
        // 5 == "wavering"
        // 4 == unable to build
        if (unity == 3) {
            roll_for_insanity(1, 20);
        } else if (unity == 2) {
            roll_for_insanity(2, 50);
        } else if (unity < 2) {
            roll_for_insanity(2, 80);
        }
    }

    private void roll_for_insanity(int quantity, int chance) {
        int roll = UnityEngine.Random.Range(1, 100);
        if (roll <= chance) {
            // Units flee into the darkness.
            for (int i = 0; i < quantity; i++) {
                bat.lose_random_unit("Your ranks crumble without the light of the stars.");
            }
        }
    }

    public void receive_travelcard_consequence() {
        if (get_travelcard() == null)
            return;
        show_adjustments(cell.get_travelcard_consequence());
        if (get_travelcard().equipment_reward_amount > 0) {
            string name = equipment_inventory.add_random_equipment(cell.tier);
            Statics.create_rising_info_map(
                RisingInfo.build_resource_text(name, 1),
                Statics.disc_colors[ID],
                origin_of_rise_obj.transform,
                rising_info_prefab);
        }
        cell.complete_travelcard();
    }

    public void add_xp_in_battle(int xp, Enemy enemy) {
        // Show xp rising over enemy
        change_var(Discipline.EXPERIENCE, xp, false);
        StartCoroutine(_add_xp_in_battle(xp, enemy));
    }

    private IEnumerator _add_xp_in_battle(int xp, Enemy enemy) {
        yield return new WaitForSeconds(1f);
        Statics.create_rising_info_battle(
            RisingInfo.build_resource_text("XP", xp),
            Statics.disc_colors[ID],
            enemy.get_slot().transform, 
            rising_info_prefab);
    }

    public TravelCard get_travelcard() {
        return cell.travelcard;
    }

    public void mine(MapCell cell) {
        int mined = 0;
        if (cell.ID == MapCell.TITRUM_ID || cell.ID == MapCell.MOUNTAIN_ID) {
            mined = Statics.valid_nonnegative_change(cell.minerals, mine_qty);
            show_adjustment(Storeable.MINERALS, mined);
            cell.minerals -= mined;
        } else if (cell.ID == MapCell.STAR_ID) {
            mined = Statics.valid_nonnegative_change(cell.star_crystals, mine_qty);
            show_adjustment(Storeable.STAR_CRYSTALS, mined);
            cell.star_crystals -= mined;
        }
        has_mined_in_turn = true;
        if (MapUI.I.cell_UI_is_open)
            MapUI.I.open_cell_UI_script.update_mineable_text();
    }

    public void reset() {
        restart_battle_from_drawn_card = false;
        foreach (int type in PlayerUnit.unit_types) {  
            bat.units[type].Clear();
        }
    }

    public override GameData save() {
        return new DisciplineData(this, name);
    }

    public override void load(GameData generic) {
        DisciplineData data = generic as DisciplineData;
        reset();
        resources[LIGHT] = data.sresources.light;
        resources[UNITY] = data.sresources.unity;
        resources[STAR_CRYSTALS] = data.sresources.star_crystals;
        resources[MINERALS] = data.sresources.minerals;
        resources[ARELICS] = data.sresources.arelics;
        resources[ERELICS] = data.sresources.erelics;
        resources[MRELICS] = data.sresources.mrelics;

        pos = new Vector3(data.col, data.row);
        cell.travelcard = TravelDeck.I.make_card(data.redrawn_travel_card_ID);
        restart_battle_from_drawn_card = cell.travelcard != null;

        // Create healthy units.
        Debug.Log("count:" + PlayerUnit.unit_types.Count);
        Debug.Log("count:" + data.sbat.healthy_types.Count);
        foreach (int type in PlayerUnit.unit_types) {  
            bat.add_units(type, data.sbat.healthy_types[type]);
        }
        // Create injured units.
        foreach (int type in PlayerUnit.unit_types) {
            for (int i = 0; i < data.sbat.injured_types[type]; i++) {
                PlayerUnit pu = PlayerUnit.create_punit(type, ID);
                if (pu == null)
                    continue;
                pu.injured = true;
                bat.units[type].Add(pu);
            }
        }

        
    }
}
