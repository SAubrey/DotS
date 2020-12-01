using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Discipline : Storeable, ISaveLoad {
    public const int ASTRA = 0, ENDURA = 1, MARTIAL = 2;
    public GameObject piece;
    public Battalion bat;
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

    protected override void Start() {
        base.Start();
        bat = new Battalion(c, this);

        _light = 4;
        _unity = 10;
  
        pos = new Vector3(10.5f, 10.5f);
        mine_qty_multiplier = ID == ENDURA ? 4 : 3;
    }

    public override void new_game() {
        base.new_game();
        cell = Map.I.city_cell;
    }

    public override void register_turn() {
        if (!dead) {
            base.register_turn();
            check_insanity();
        }
        has_mined_in_turn = false;
        has_moved_in_turn = false;
        has_scouted_in_turn = false;
    }

    public void move(MapCell cell) {
        // Offset position on cell to simulate human placement and prevent perfect overlap.
        this.cell = cell;
        float randx = Random.Range(-0.2f, 0.2f); 
        float randy = Random.Range(-0.2f, 0.2f);
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
        cell.battle.end();
        bat.kill_injured_units();
        remove_resources_lost_on_death();
        Map.I.get_cell(pos).drop_XP(experience);
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

    private void check_insanity() {
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
        int roll = Random.Range(1, 100);
        if (roll <= chance) {
            // Units flee into the darkness.
            for (int i = 0; i < quantity; i++) {
                bat.lose_random_unit();
            }
        }
    }

    public Pos get_Pos() {
        return new Pos((int)pos.x, (int)pos.y);
    }

    public TravelCard get_travelcard() {
        return cell.travelcard;
    }

    public void mine(MapCell cell) {
        int sc_mined = 0;
        if (cell.biome_ID == MapCell.TITRUM_ID || cell.biome_ID == MapCell.MOUNTAIN_ID) {
            if (cell.minerals >= mine_qty) {
                sc_mined = change_var(Storeable.MINERALS, mine_qty, true);
            } else {
                sc_mined = change_var(Storeable.MINERALS, cell.minerals, true);
            }
        } else if (cell.biome_ID == MapCell.STAR_ID) {
            if (cell.star_crystals >= mine_qty) {
                sc_mined = change_var(Storeable.STAR_CRYSTALS, mine_qty, true);
            } else {
                sc_mined = change_var(Storeable.STAR_CRYSTALS, cell.star_crystals, true);
            }
        }
        has_mined_in_turn = true;
        cell.star_crystals -= sc_mined;
        if (MapUI.I.cell_UI_is_open)
            MapUI.I.open_cell_UI_script.update_star_crystal_text();
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
        _light = data.sresources.light;
        _unity = data.sresources.unity;
        _star_crystals = data.sresources.star_crystals;
        _minerals = data.sresources.minerals;
        _arelics = data.sresources.arelics;
        _erelics = data.sresources.erelics;
        _mrelics = data.sresources.mrelics;

        pos = new Vector3(data.col, data.row);
        cell.travelcard = TravelDeck.I.get_card(data.redrawn_travel_card_ID);
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
