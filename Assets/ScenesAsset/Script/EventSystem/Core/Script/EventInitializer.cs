using EventSO;
using UnityEngine;

public static class EventInitializer 
{
    public static void InitializeEvent(EventListSO p_eventListSO)
    {
        if (p_eventListSO == null)
        {
            Debug.LogError("EventListSO is not assigned in the Inspector.");
            return;
        }
        EventSystem.Instance.Initialize(p_eventListSO);
    }
}
