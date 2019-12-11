using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TravelCard {
    public const int COMBAT = 1;
    public const int BLESSING = 2;
    public const int CHANCE = 3;
    public const int CAVE = 4;
    public const int EVENT = 5;
    public const int LOCATION = 6;
    public const int RUIN = 7;

    public int enemies;
    public Dictionary<string, int> rewards = new Dictionary<string, int>();
    public int ID;
    public Sprite sprite;
    public bool reward = false;
    public int type;
    // reward.Add(Storeable.MRELICS, 2);


    public TravelCard(int id, Sprite sprite) {
        ID = id;
        this.sprite = sprite;
    }
}

public class Att1_1 : TravelCard {
    public Att1_1(int id, Sprite sprite) : base(id, sprite) {
        this.enemies = 7;
        this.type = COMBAT;
    }
}

public class Att2_1 : TravelCard {
    public Att2_1(int id, Sprite sprite) : base(id, sprite) {
        this.enemies = 8;
        this.type = COMBAT;
    }
}

public class Att3_1 : TravelCard {
    public Att3_1(int id, Sprite sprite) : base(id, sprite) {
        this.enemies = 7;
        this.type = COMBAT;
    }
}

public class Att4_1 : TravelCard {
    public Att4_1(int id, Sprite sprite) : base(id, sprite) {
        this.enemies = 7;
        this.type = COMBAT;
    }
}

public class Att5_1 : TravelCard {
    public Att5_1(int id, Sprite sprite) : base(id, sprite) {
        this.enemies = 5;
        this.type = COMBAT;
    }
}

public class Att6_1 : TravelCard {
    public Att6_1(int id, Sprite sprite) : base(id, sprite) {
        this.enemies = 6;
        this.type = COMBAT;
    }
}

public class Att7_1 : TravelCard {
    public Att7_1(int id, Sprite sprite) : base(id, sprite) {
        this.enemies = 5;
        this.type = COMBAT;
    }
}


