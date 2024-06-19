using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct QTableState
{
    public bool NorthIsWalkable;
    public bool EastIsWalkable;
    public bool SouthIsWalkable;
    public bool WestIsWalkable;

    public bool EnemyIsNorth;
    public bool EnemyIsEast;
    public bool EnemyIsSouth;
    public bool EnemyIsWest;
    
    public float EnemyDistance;

    public QTableState(bool northWalkable = true, bool eastWalkable = true, bool southWalkable = true, bool westWalkable = true, bool enemyIsNorth = true, bool enemyIsEast = true, bool enemyIsSouth = true, bool enemyIsWest = true, float distance = 0.0f)
    {
        this.NorthIsWalkable = northWalkable;
        this.EastIsWalkable = eastWalkable;
        this.SouthIsWalkable = southWalkable;
        this.WestIsWalkable = westWalkable;

        this.EnemyIsNorth = enemyIsNorth;
        this.EnemyIsEast = enemyIsEast;
        this.EnemyIsSouth = enemyIsSouth;
        this.EnemyIsWest = enemyIsWest;

        this.EnemyDistance = distance;
    }

    public string GetStringCSV()
    {
        string str = $"{NorthIsWalkable}; {EastIsWalkable}; {SouthIsWalkable}; {WestIsWalkable}; {EnemyIsNorth}; {EnemyIsEast}; {EnemyIsSouth}; {EnemyIsWest}; {EnemyDistance}";
        return str;
    }
}

public enum QTableAction
{
    MoveNorth,
    MoveEast,
    MoveSouth,
    MoveWest
}
