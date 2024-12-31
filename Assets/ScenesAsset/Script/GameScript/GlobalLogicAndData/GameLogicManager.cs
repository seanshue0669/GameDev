using Stage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameLogicManager : MonoBehaviour
{
    #region Fields and Properties
    // Game control
    private IStage m_activeStage;
    public bool IsGameRestarting { get; private set; }
    public bool IsGameContinuing { get; private set; }

    private string m_currentGameName;

    // Game data
    private StageCollections m_stageCollection;
    private SharedDataSO m_sharedData;
    private UIComponentCollectionSO m_uiComponents;
    #endregion

    private async void Update()
    {
        if (m_activeStage != null)
        {
            await m_activeStage.InputAsync();
        }
    }

    public async void StartGame(string p_gameName)
    {
        IsGameRestarting = true;
        IsGameContinuing = true;
        m_activeStage = null;
        m_currentGameName = p_gameName;

        Debug.Log($"StartGame: {p_gameName}");

        InitializeGameData();
        await RunStagesAsync();
    }

    #region Game Execution Utility
    private void InitializeGameData()
    {
        // Fetching data from the data pool
        m_stageCollection = GameDataPool.GetStageCollection(m_currentGameName);
        m_sharedData = GameDataPool.GetSharedData(m_currentGameName);
        m_uiComponents = GameDataPool.GetUICollection(m_currentGameName);
    }
    private async Task RunStagesAsync()
    {
        while (IsGameRestarting)
        {
            foreach (var stage in m_stageCollection.stages)
            {
                m_activeStage = stage;
                Debug.Log($"Executing stage: {m_activeStage.GetType().Name} for game: {m_currentGameName}");
                await m_activeStage.ExecuteAsync(m_sharedData, m_uiComponents);
                if (!IsGameContinuing)
                    break;
            }
            Debug.Log("All game stages completed!");
        }
    }   
    #endregion
}
