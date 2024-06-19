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
        this.filePath = $"{Application.dataPath}/QLearningData";
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
        ReadFile();
    }

    public void SaveTable()
    {
        WriteFile();
    }

    #endregion

    #region PrivateMethods

    private void ReadFile()
    {
        string filename = GetFileName();
        StreamReader reader = new StreamReader(filePath);

        string csvLine = reader.ReadLine();
        string[] dataStrings = csvLine.Split(csvSeparators);

        QTableState state = new QTableState();
        state.CSVSetData(dataStrings, 0);

        QTableReward reward = new QTableReward();
        reward.CSVSetData(dataStrings, state.CSVGetNumElements());

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
}
