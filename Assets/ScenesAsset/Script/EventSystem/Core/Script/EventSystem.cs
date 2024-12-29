using EventSO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EventSystem
{
    private static EventSystem _instance;

    /// <summary>
    /// Singleton instance of EventSystem.
    /// </summary>
    public static EventSystem Instance => _instance ??= new EventSystem();

    private readonly Dictionary<int, Dictionary<string, List<Delegate>>> eventHandlers = new(); // Key: EventID
    private readonly Dictionary<string, int> tagToEventID = new(); // Key: EventTag, Value: EventID
    private readonly Dictionary<string, TaskCompletionSource<object>> pendingTasks = new();

    private EventSystem() { }

    // ---------- Event Registration ----------

    /// <summary>
    /// Registers a synchronous event with no return value (Action<T>).
    /// </summary>
    public void RegisterEvent<T>(string eventTag, string identifier, Action<T> action)
    {
        if (tagToEventID.TryGetValue(eventTag, out var eventID))
        {
            AddHandler(eventID, identifier, action);
        }
        else
        {
            Debug.LogWarning($"EventTag '{eventTag}' does not exist in the current system.");
        }
    }

    /// <summary>
    /// Registers an asynchronous event with a return value (Func<T, Task<TResult>>).
    /// </summary>
    public void RegisterCallBack<T, TResult>(string eventTag, string identifier, Func<T, Task<TResult>> callback)
    {
        if (tagToEventID.TryGetValue(eventTag, out var eventID))
        {
            AddHandler(eventID, identifier, callback);
        }
        else
        {
            Debug.LogWarning($"EventTag '{eventTag}' does not exist in the current system.");
        }
    }

    /// <summary>
    /// Waits for a callback to complete and returns its TaskCompletionSource.
    /// </summary>
    public Task<object> WaitForCallBack(string eventTag, string identifier)
    {
        if (!tagToEventID.TryGetValue(eventTag, out var eventID))
        {
            Debug.LogWarning($"EventTag '{eventTag}' does not exist in the current system.");
            return Task.FromException<object>(new ArgumentException($"EventTag '{eventTag}' not found"));
        }
        var key = $"{eventID}:{identifier}";
        if (pendingTasks.TryGetValue(key, out var tcs_source))
        {
            return tcs_source.Task;
        }
        var tcs = new TaskCompletionSource<object>();
        pendingTasks[key] = tcs;
        return tcs.Task;
    }

    // ---------- Event Triggering ----------

    /// <summary>
    /// Triggers all registered Func<T, Task<TResult>> handlers for the given event.
    /// </summary>
    public async Task TriggerCallBack<T, TResult>(string eventTag, string identifier, T data)
    {
        if (tagToEventID.TryGetValue(eventTag, out var eventID) &&
            eventHandlers.TryGetValue(eventID, out var identifierHandlers) &&
            identifierHandlers.TryGetValue(identifier, out var handlers))
        {
            foreach (var handler in handlers)
            {
                try
                {
                    if (handler is Func<T, Task<TResult>> asyncFunc)
                    {
                        var result = await asyncFunc(data);
                        if (pendingTasks.TryGetValue($"{eventID}:{identifier}", out var tcs))
                        {
                            tcs.SetResult(result);
                            pendingTasks.Remove($"{eventID}:{identifier}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Unknown handler type for EventID '{eventID}' and Identifier '{identifier}'.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error executing handler for '{eventTag}:{identifier}': {ex.Message}");
                }
            }
        }
        else
        {
            Debug.LogWarning($"No handlers registered for EventTag '{eventTag}' with Identifier '{identifier}'.");
        }
    }
    /// <summary>
    /// Triggers all registered Action<T> handlers for the given event.
    /// </summary>
    public void TriggerEvent<T>(string eventTag, string identifier, T data)
    {
        if (tagToEventID.TryGetValue(eventTag, out var eventID) &&
            eventHandlers.TryGetValue(eventID, out var identifierHandlers) &&
            identifierHandlers.TryGetValue(identifier, out var handlers))
        {
            foreach (var handler in handlers)
            {
                try
                {
                    if (handler is Action<T> action)
                    {
                        action.Invoke(data);
                    }
                    else
                    {
                        Debug.LogWarning($"Unknown handler type for EventID '{eventID}' and Identifier '{identifier}'.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error executing event handler for '{eventTag}:{identifier}': {ex.Message}");
                }
            }
        }
        else
        {
            Debug.LogWarning($"No event handlers registered for EventTag '{eventTag}' with Identifier '{identifier}'.");
        }
    }
    // ---------- Initialization ----------

    /// <summary>
    /// Initializes the EventSystem with a list of event tags and their identifiers.
    /// </summary>
    public void Initialize(EventListSO eventListSO)
    {
        if (eventListSO == null)
        {
            Debug.LogError("EventListSO is null. Initialization failed.");
            return;
        }

        // Clear any previous handlers
        eventHandlers.Clear();
        tagToEventID.Clear();

        // Populate tagToEventID and eventHandlers based on EventListSO
        foreach (var category in eventListSO.Categories)
        {
            foreach (var eventSO in category.Events)
            {
                if (eventSO != null)
                {
                    var eventID = eventSO.EventID;
                    var eventTag = eventSO.EventTag;

                    // Map tag to EventID
                    tagToEventID[eventTag] = eventID;

                    // Initialize handlers for EventID
                    if (!eventHandlers.ContainsKey(eventID))
                    {
                        eventHandlers[eventID] = new Dictionary<string, List<Delegate>>();
                    }

                    foreach (var identifier in eventSO.Identifiers)
                    {
                        if (!eventHandlers[eventID].ContainsKey(identifier))
                        {
                            eventHandlers[eventID][identifier] = new List<Delegate>();
                        }
                    }
                }
            }
        }

        Debug.Log("EventSystem initialized successfully with EventListSO.");
    }

    // ---------- Internal Methods for Registration ----------

    private void AddHandler(int eventID, string identifier, Delegate callback)
    {
        if (!eventHandlers.ContainsKey(eventID))
        {
            Debug.LogWarning($"EventID '{eventID}' does not exist in the current system.");
            return;
        }

        if (!eventHandlers[eventID].ContainsKey(identifier))
        {
            Debug.LogWarning($"Identifier '{identifier}' does not exist for EventID '{eventID}'.");
            return;
        }

        eventHandlers[eventID][identifier].Add(callback);
    }

    // ---------- Event Unregistration ----------

    public void UnregisterEvent<T>(string eventTag, string identifier, Action<T> callback)
    {
        if (tagToEventID.TryGetValue(eventTag, out var eventID))
        {
            RemoveHandler(eventID, identifier, callback);
        }
        else
        {
            Debug.LogWarning($"EventTag '{eventTag}' does not exist in the current system.");
        }
    }

    public void UnregisterCallBack<T, TResult>(string eventTag, string identifier, Func<T, Task<TResult>> callback)
    {
        if (tagToEventID.TryGetValue(eventTag, out var eventID))
        {
            RemoveHandler(eventID, identifier, callback);
        }
        else
        {
            Debug.LogWarning($"EventTag '{eventTag}' does not exist in the current system.");
        }
    }

    private void RemoveHandler(int eventID, string identifier, Delegate callback)
    {
        if (eventHandlers.TryGetValue(eventID, out var identifierHandlers) &&
            identifierHandlers.TryGetValue(identifier, out var handlers))
        {
            handlers.Remove(callback);

            if (handlers.Count == 0)
            {
                identifierHandlers.Remove(identifier);
            }

            if (identifierHandlers.Count == 0)
            {
                eventHandlers.Remove(eventID);
            }
        }
    }
}
