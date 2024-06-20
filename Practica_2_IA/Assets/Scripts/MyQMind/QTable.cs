using NavigationDJIA.World;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class QTable
{
    #region Variables

    private Dictionary<QTableState, QTableReward> qTable; // possible actions : goto { n, s, e, w }
    
    private string filePath;
    private string fileName;

    private static char[] csvSeparators = { ',', ';' };

    #endregion

    #region Constructors

    public QTable(string filename = "QTable.csv")
    {
        this.qTable = new Dictionary<QTableState, QTableReward>();
        this.filePath = $"{Application.dataPath}";
        //this.filePath = $"{Application.persistentDataPath}";
        this.fileName = filename;
    }

    #endregion

    #region PublicMethods

    public string GetFileName()
    {
        return $"{filePath}/{fileName}";
    }

    public void LoadTable()
    {
        Debug.Log($"Loaded QTable from path \"{GetFileName()}\"");
        ReadFile();
    }

    public void SaveTable()
    {
        Debug.Log($"Saved QTable to path \"{GetFileName()}\"");
        WriteFile();
    }

    public void Add(QTableState state, QTableReward reward)
    {
        if(qTable.ContainsKey(state))
            qTable[state] = reward;
        else
            qTable.Add(state, reward);
    }

    public void Add(QTableState state)
    {
        QTableReward reward = new QTableReward();
        Add(state, reward);
    }

    // Q(s,a)' = lerp(Q(s,a), reward + gamma * maxQ(s',a'), alpha);
    public void UpdateQ(QTableState state, QTableState nextState, QTableAction action, float reward, float alpha, float gamma)
    {
        if (!qTable.ContainsKey(state))
            Add(state);

        if (!qTable.ContainsKey(nextState))
            Add(nextState);

        float newQ = Mathf.Lerp(GetQ(state, action), reward + gamma * GetMaxQ(nextState), alpha);
        SetQ(state, action, newQ);
    }

    public float GetQ(QTableState state, QTableAction action)
    {
        switch (action)
        {
            case QTableAction.GoNorth:
                return qTable[state].rewardNorth;
            case QTableAction.GoEast:
                return qTable[state].rewardEast;
            case QTableAction.GoSouth:
                return qTable[state].rewardSouth;
            case QTableAction.GoWest:
                return qTable[state].rewardWest;
            default:
                return -1;
        }
    }

    public float GetMaxQ(QTableState state)
    {
        float f1 = qTable[state].rewardNorth;
        float f2 = qTable[state].rewardEast;
        float f3 = qTable[state].rewardSouth;
        float f4 = qTable[state].rewardWest;
        return Mathf.Max(f1, f2, f3, f3);
    }

    public void SetQ(QTableState state, QTableAction action, float value)
    {
        QTableReward reward = qTable[state];
        switch (action)
        {
            case QTableAction.GoNorth:
                reward.rewardNorth = value;
                break;
            case QTableAction.GoEast:
                reward.rewardEast = value;
                break;
            case QTableAction.GoSouth:
                reward.rewardSouth = value;
                break;
            case QTableAction.GoWest:
                reward.rewardWest = value;
                break;
        }
    }

    #endregion

    #region PrivateMethods

    private void ReadFile()
    {
        string filename = GetFileName();
        StreamReader reader = new StreamReader(filename);

        string csvLine;

        while ((csvLine = reader.ReadLine()) != null)
        {
            string[] dataStrings = csvLine.Split(csvSeparators);

            QTableState state = new QTableState();
            state.CSVSetData(dataStrings, 0);

            QTableReward reward = new QTableReward();
            reward.CSVSetData(dataStrings, state.CSVGetNumElements());

            qTable.Add(state, reward);
        }

        reader.Close();
    }

    private void WriteFile()
    {
        string filename = GetFileName();
        StreamWriter writer = new StreamWriter(filename, false);

        foreach (var entry in qTable)
        {
            string stateString = entry.Key.CSVGetData();
            string valueString = entry.Value.CSVGetData();
            writer.WriteLine($"{stateString} {valueString}");
        }

        writer.Close();
    }

    #endregion

    #region PublicDebugMethods

    public void DebugPrintTableInfo()
    {
        foreach (var entry in qTable)
        {
            string stateString = entry.Key.CSVGetData();
            string valueString = entry.Value.CSVGetData();
            Debug.Log($"{stateString} {valueString}");
        }
    }

    #endregion
}
