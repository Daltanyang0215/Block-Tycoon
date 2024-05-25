using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexaUpgradeManager
{
    #region Singleton
    private static HexaUpgradeManager _instance;
    public static HexaUpgradeManager Instance
    {
        get
        {
            if (_instance == null) _instance = new HexaUpgradeManager();
            return _instance;
        }
    } 
    #endregion


    
}

