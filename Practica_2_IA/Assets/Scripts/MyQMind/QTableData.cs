using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QTableDistances
{
    Close = 0,
    Middle,
    Far
}

public enum QTableAction
{
    GoNorth = 0,
    GoEast,
    GoSouth,
    GoWest
}

public struct QTableState : ICSVConvertible
{
    public bool NorthIsWalkable;
    public bool EastIsWalkable;
    public bool SouthIsWalkable;
    public bool WestIsWalkable;

    public bool EnemyIsNorth;
    public bool EnemyIsEast;
    public bool EnemyIsSouth;
    public bool EnemyIsWest;
    
    public QTableDistances EnemyDistance;

    public QTableState(bool northWalkable = true, bool eastWalkable = true, bool southWalkable = true, bool westWalkable = true, bool enemyIsNorth = true, bool enemyIsEast = true, bool enemyIsSouth = true, bool enemyIsWest = true, QTableDistances distance = QTableDistances.Close)
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

    public string CSVGetData(char separator = ';')
    {
        return $"{NorthIsWalkable}{separator} {EastIsWalkable}{separator} {SouthIsWalkable}{separator} {WestIsWalkable}{separator} {EnemyIsNorth}{separator} {EnemyIsEast}{separator} {EnemyIsSouth}{separator} {EnemyIsWest}{separator} {(int)EnemyDistance}{separator}";
    }

    public void CSVSetData(string csvLine, char[] csvSeparators, int offset = 0)
    {
        string[] dataStrings = csvLine.Split(csvSeparators);
        CSVSetData(dataStrings, offset);
    }

    public void CSVSetData(string[] dataStrings, int offset = 0)
    {
        this.NorthIsWalkable = bool.Parse(dataStrings[offset + 0]);
        this.EastIsWalkable  = bool.Parse(dataStrings[offset + 1]);
        this.SouthIsWalkable = bool.Parse(dataStrings[offset + 2]);
        this.WestIsWalkable  = bool.Parse(dataStrings[offset + 3]);
        this.EnemyIsNorth    = bool.Parse(dataStrings[offset + 4]);
        this.EnemyIsEast     = bool.Parse(dataStrings[offset + 5]);
        this.EnemyIsSouth    = bool.Parse(dataStrings[offset + 6]);
        this.EnemyIsWest     = bool.Parse(dataStrings[offset + 7]);
        this.EnemyDistance   = (QTableDistances)int.Parse(dataStrings[offset + 8]);
    }

    public int CSVGetNumElements()
    {
        return 9;
    }
}

public struct QTableReward : ICSVConvertible
{
    public float rewardNorth;
    public float rewardEast;
    public float rewardSouth;
    public float rewardWest;

    public QTableReward(float north = 0.0f, float east = 0.0f, float south = 0.0f, float west = 0.0f)
    {
        this.rewardNorth = north;
        this.rewardEast = east;
        this.rewardSouth = south;
        this.rewardWest = west;
    }

    public string CSVGetData(char separator = ';')
    {
        return $"{rewardNorth}{separator} {rewardEast}{separator} {rewardSouth}{separator} {rewardWest}{separator}";
    }

    public void CSVSetData(string csvLine, char[] csvSeparators, int offset = 0)
    {
        string[] dataStrings = csvLine.Split(csvSeparators);
        CSVSetData(dataStrings, offset);
    }

    public void CSVSetData(string[] dataStrings, int offset = 0)
    {
        this.rewardNorth = float.Parse(dataStrings[offset + 0]);
        this.rewardEast  = float.Parse(dataStrings[offset + 1]);
        this.rewardSouth = float.Parse(dataStrings[offset + 2]);
        this.rewardWest  = float.Parse(dataStrings[offset + 3]);
    }

    public int CSVGetNumElements()
    {
        return 4;
    }

}
