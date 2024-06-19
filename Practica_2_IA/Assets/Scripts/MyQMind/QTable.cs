using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QTable
{
    #region Variables

    private Dictionary<QTableState, QTableAction> qTable;
    private string filename;

    #endregion

    #region GettersAndSetters

    public string FileName { get { return this.filename; } set { this.filename = value; } }

    #endregion

    #region Constructors

    public QTable(string filename = null)
    {
        this.qTable = new Dictionary<QTableState, QTableAction>();
        this.filename = filename;
    }
    
    #endregion

    private void ReadFile()
    {

    }

    private void WriteFile()
    {

    }

}
