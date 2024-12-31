using EventSO;
using System.Collections.Generic;
using UnityEngine;
public class GameRoot : MonoBehaviour
{
    // Singleton pattern for GameRoot(dont touch)
    private static GameRoot _instance;
    public static GameRoot Instance { get { return _instance; } }

    #region Property that manager need
    [SerializeField]
    public EventListSO m_eventListSO;
    public List<SharedDataSO> m_sharedDatas;
    public List<UIComponentCollectionSO> m_uiComponentCollections;
    #endregion

    /// <summary>
    /// These manager will change by switching scene.
    /// The main goal is to tracing and switing manager.
    /// </summary>
    #region StateManager Singleton pattern
    public static GameLogicManager gameLogicManager;

    #endregion

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        gameLogicManager = gameLogicManager ?? GetComponent<GameLogicManager>();
        //Initialize some stuff
        GameDataPool.InitializePool(m_sharedDatas, m_uiComponentCollections);       
        EventInitializer.InitializeEvent(m_eventListSO);      
        EventSystem.Instance.RegisterEvent<string>("SwitchScene", "StartGame", RunCurrrentGame);      
        //Create Log
        Debug.Log("GameRoot initialized successfully");
    }
    private void RunCurrrentGame(string p_gameName)
    {
        gameLogicManager.StartGame(p_gameName);
    }
} 

