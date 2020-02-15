using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Discipline : Storeable, ISaveLoad {
    public GameObject piece;
    public Battalion bat;
    public TravelCard travel_card;
    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        bat = new Battalion(c, ID);
        map_ui = c.map_ui;
        city_ui = c.city_ui;

        _light = 4;
        _unity = 10;
        pos = new Vector3(10.5f, 10.5f);
    }

    public override void register_turn() {
        base.register_turn();
        check_insanity();
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

    public void reset() {
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
        light = data.sresources.light;
        unity = data.sresources.unity;
        star_crystals = data.sresources.star_crystals;
        minerals = data.sresources.minerals;
        arelics = data.sresources.arelics;
        erelics = data.sresources.erelics;
        mrelics = data.sresources.mrelics;

        pos = new Vector3(data.col, data.row);
        
        // Create healthy units.
        Debug.Log("count:" + PlayerUnit.unit_types.Count);
        Debug.Log("count:" + data.sbat.healthy_types.Count);
        //for (int i = 0; i < PlayerUnit.unit_types.Count - 1; i++) {
        foreach (int type in PlayerUnit.unit_types) {  
            bat.units[type].Clear();
            bat.add_units(type, data.sbat.healthy_types[type]);
        }
        // Create injured units.
        //for (int type = 0; type < PlayerUnit.unit_types.Count; type++) {
        foreach (int type in PlayerUnit.unit_types) {
            for (int i = 0; i < data.sbat.injured_types[type]; i++) {
                PlayerUnit pu = PlayerUnit.create_punit(type);
                if (pu == null)
                    continue;
                pu.injured = true;
                bat.units[type].Add(pu);
            }
        }
    }
}
