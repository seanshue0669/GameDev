using System;
using System.Collections.Generic;
using System.Diagnostics;
using Stage;
using UnityEngine;
public static class GameDataPool
{
    private static readonly Dictionary<string, StageCollections> _stageCollections = new Dictionary<string, StageCollections>();   
    private static readonly Dictionary<string, UIComponentCollectionSO> _uiCollections = new Dictionary<string, UIComponentCollectionSO>();
    private static readonly Dictionary<string, SharedDataSO> _dataCollections = new Dictionary<string, SharedDataSO>();
    public static void InitializePool(List<SharedDataSO> p_dataList, List<UIComponentCollectionSO> p_uiList)
    {
        UnityEngine.Debug.Log("GameDataPool initialized successfully");
        #region InitStage

        _stageCollections["DiceGame"] = new DiceGameStageCollection();
        // Manually adding more stage collection here
        _stageCollections["PokerGame"] = new PokerGameStageCollection();
        _stageCollections["WheelGame"] = new RouletteGameStageCollection();


        //Dont touch!
        foreach (var dataSO in p_dataList)
        {
            _dataCollections[dataSO.m_gameName]=dataSO;
        }
        foreach(var uiSO in p_uiList)
        {
            _uiCollections[uiSO.m_gameName]= uiSO;
        }
        #endregion
    }
    #region Tool Function
    public static StageCollections GetStageCollection(string p_gameName)
    {
        if (_stageCollections.ContainsKey(p_gameName))
        {
            return _stageCollections[p_gameName];
        }
        else
        {
            return null;
            throw new ArgumentException($"StageCollection for game {p_gameName} not found.");
        }    
    }
    public static UIComponentCollectionSO GetUICollection(string p_gameName)
    {
        if (_uiCollections.ContainsKey(p_gameName))
        {
            return _uiCollections[p_gameName]; ;
        }
        else
        {
            return null;
            throw new ArgumentException($"UIComponentCollection for game {p_gameName} not found.");
        }
    }
    public static SharedDataSO GetSharedData(string p_gameName)
    {
        if (_dataCollections.ContainsKey(p_gameName))
        {
            return _dataCollections[p_gameName]; ;
        }
        else
        {
            return null;
            throw new ArgumentException($"UIComponentCollection for game {p_gameName} not found.");
        }      
    }
    #endregion
}

