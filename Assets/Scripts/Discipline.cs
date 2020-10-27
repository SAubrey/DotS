using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Discipline : Storeable, ISaveLoad {
    public const int ASTRA = 0, ENDURA = 1, MARTIAL = 2;
    public GameObject piece;
    public Battalion bat;
    private TravelCard travelcard;
    public bool restart_battle_from_drawn_card = false;
    public int base_unity = 10;
    public int mine_qty;
    public bool has_mined_in_turn = false;
    public bool has_moved_in_turn = false;
    


    protected override void Start() {
        base.Start();
        bat = new Battalion(c, this);

        _light = 4;
        _unity = base_unity;
        /*
        _star_crystals = capacity;
        _minerals = capacity;
        _arelics = capacity;
        _mrelics = capacity;
        _erelics = capacity;
        */
        pos = new Vector3(10.5f, 10.5f);
        mine_qty = ID == ENDURA ? 4 : 3;
    }

    public override void new_game() {
        base.new_game();
        pos = new Vector3(10.5f, 10.5f);
    }

    public override void register_turn() {
        base.register_turn();
        check_insanity();
        has_mined_in_turn = false;
        has_moved_in_turn = false;
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

    public void complete_travelcard() {
        if (travelcard == null)
            return;
        MapCell mc = c.map.get_cell(pos);
        if (bat.in_battle) {
            bat.in_battle = false;
            StartCoroutine(adjust_resources_visibly(travelcard.consequence));
        }
        mc.complete_travelcard();
        travelcard = null;
    }

    public void set_travelcard(TravelCard tc) {
        travelcard = tc;
        MapCell mc = c.map.get_cell(pos);
        mc.travelcard = tc;
    }

    public TravelCard get_travelcard() {
        return travelcard;
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
        c.map.open_cell_UI_script.update_star_crystal_text();
    }

    public void reset() {
        restart_battle_from_drawn_card = false;
        foreach (int type in PlayerUnit.unit_types) {  
            bat.units[type].Clear();
        }
    }
    
    private Vector3 _pos;
    public Vector3 pos {
        get { return _pos; }
        set {
            prev_pos = _pos;
            _pos = value;
            piece.transform.position = new Vector3(value.x, value.y, 0);
        }
    }
    public Vector3 prev_pos;

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
        travelcard = c.travel_deck.get_card(data.redrawn_travel_card_ID);
        restart_battle_from_drawn_card = travelcard != null;

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
