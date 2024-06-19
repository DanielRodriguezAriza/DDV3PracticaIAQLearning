using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICSVConvertible
{
    public void CSVSetData(string csvLine, char[] csvSeparators, int offset = 0);
    public void CSVSetData(string[] csvElements, int offset = 0);
    public string CSVGetData(char separator = ';');
    public int CSVGetNumElements();
}
