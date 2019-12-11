using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    public const string ASTRA = "astra"; // must be strings
    public const string ENDURA = "endura";
    public const string MARTIAL = "martial";

    // UI CONSTANTS
    public static Color GREY = new Color(.78125f, .78125f, .78125f, 1);

    // Singleton gameobjects
    public Formation formation;
    public TurnPhaser turn_phaser;
    public MapUI map_ui;
    public Selector selector;
    public AttackQueuer attack_queuer;
    public UnitPanelManager unit_panel_man;
    public BattlePhaser battle_phaser;
    public CamSwitcher cam_switcher;
    public BatLoader bat_loader;
    public TravelDeck travel_deck;
    public TileMapper tile_mapper;
    public EnemyLoader enemy_loader;
    public LineDrawer line_drawer;
    public EnemyBrain enemy_brain;

    public Storeable astra;
    public Storeable martial;
    public Storeable endura;
    public Storeable city;
    private int turn_number = 1;
    public int num_players = 3;

    public string active_player;

    public IDictionary<string, Storeable> stores = new Dictionary<string, Storeable>();

    void Awake() {
        // Public classes that all other classes will grab from initially.
        formation = GameObject.Find("Formation").GetComponent<Formation>();
        turn_phaser = GameObject.Find("TurnPhaser").GetComponent<TurnPhaser>();
        selector = GameObject.Find("Selector").GetComponent<Selector>();
        attack_queuer = GameObject.Find("AttackQueuer").GetComponent<AttackQueuer>();
        unit_panel_man = GameObject.Find("UnitPanelManager").GetComponent<UnitPanelManager>();
        battle_phaser = GameObject.Find("BattlePhaser").GetComponent<BattlePhaser>();
        cam_switcher = GameObject.Find("CamSwitcher").GetComponent<CamSwitcher>();
        bat_loader = GameObject.Find("BatLoader").GetComponent<BatLoader>();
        travel_deck = GameObject.Find("TravelDeck").GetComponent<TravelDeck>();
        tile_mapper = GameObject.Find("TileMapper").GetComponent<TileMapper>();
        enemy_loader = GameObject.Find("EnemyLoader").GetComponent<EnemyLoader>();
        map_ui = GameObject.Find("MapUI").GetComponent<MapUI>();
        unit_panel_man = GameObject.Find("UnitPanelManager").GetComponent<UnitPanelManager>();
        line_drawer = GameObject.Find("LineDrawer").GetComponent<LineDrawer>();
        enemy_brain = GameObject.Find("EnemyBrain").GetComponent<EnemyBrain>();
    }

    void Start() {
        stores.Add(ASTRA, astra);
        stores.Add(MARTIAL, martial);
        stores.Add(ENDURA, endura);
        stores.Add("city", city);
        
        set_player(ASTRA);
    }

    public void advance_turn() {
        // Update turn number
        turn_number += 1;

        city.decrement_light();
        astra.decrement_light();
        martial.decrement_light();
        endura.decrement_light();

        map_ui.turn_number_t.text = turn_number.ToString();
        map_ui.load_stats(active_player);
    }
     
    public void set_player(string type) {
        active_player = type;
        map_ui.load_stats(active_player);
        formation.reset();

        if (get_active_bat().in_battle) {
            Debug.Log("Loading " + active_player);
            formation.load_board(active_player);
        }
    }

    public void rotate_player() {
        if (get_player() == ASTRA) set_player(ENDURA);
        else if (get_player() == ENDURA) set_player(MARTIAL);
        else if (get_player() == MARTIAL) set_player(ASTRA);
        Debug.Log("Rotated player to " + get_player());
    }       

    public string get_player() {
        return active_player;
    }

    public Storeable get_player_obj() {
        return stores[get_player()];
    }

    public Battalion get_active_bat() {
        return stores[get_player()].bat;
    }

    // BUTTON HANDLES
    public void inc_stat(string field) {
        stores[active_player].change_var(field, 1);
    }

    public void dec_stat(string field) {
        stores[active_player].change_var(field, -1);
    }

    public void inc_city_stat(string field) {
        city.change_var(field, 1);
    }

    public void dec_city_stat(string field) {
        city.change_var(field, -1);
    }

    public void quit_game() {
        Application.Quit();
    }
}
//GameObject.Find("HP Image").GetComponent<Image>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;

public struct Pos {
    public int x, y;
    public Pos(int x, int y) {
        this.x = x;
        this.y = y;
    }
}