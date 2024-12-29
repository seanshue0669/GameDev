using System.Collections.Generic;
using UnityEngine;

namespace EventSO
{
    /// <summary>
    /// Manages a list of categorized events for use in the event system.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/EventList", fileName = "EventList")]
    public class EventListSO : ScriptableObject
    {
        [System.Serializable]
        public class EventCategory
        {
            public string CategoryName; // Name of the event category
            public List<GeneralEventSO> Events = new List<GeneralEventSO>(); // List of events under this category
        }

        [Header("Event Categories")]
        public List<EventCategory> Categories = new List<EventCategory>();

        // Provides mappings for the event system
        public Dictionary<int, GeneralEventSO> idToEvent = new Dictionary<int, GeneralEventSO>(); // ID -> Event mapping
        public Dictionary<string, int> tagToID = new Dictionary<string, int>(); // Tag -> ID mapping
        private System.Random random = new System.Random();
        #region Initialization

        /// <summary>
        /// Initializes the lookup dictionaries (idToEvent and tagToID) based on the current categories and events.
        /// </summary>
        public void InitializeLookup()
        {
            idToEvent.Clear();
            tagToID.Clear();

            foreach (var category in Categories)
            {
                foreach (var eventSO in category.Events)
                {
                    if (eventSO != null)
                    {
                        // Add to idToEvent
                        if (!idToEvent.ContainsKey(eventSO.EventID))
                        {
                            idToEvent[eventSO.EventID] = eventSO;
                        }
                        else
                        {
                            Debug.LogWarning($"Duplicate EventID '{eventSO.EventID}' detected.");
                        }

                        // Add to tagToID
                        if (!tagToID.ContainsKey(eventSO.EventTag))
                        {
                            tagToID[eventSO.EventTag] = eventSO.EventID;
                        }
                        else
                        {
                            Debug.LogWarning($"Duplicate EventTag '{eventSO.EventTag}' detected.");
                        }
                    }
                }
            }

            Debug.Log("EventListSO lookup initialized successfully.");
        }

        #endregion

        #region Category and Event Management

        /// <summary>
        /// Adds a new category with the given name.
        /// </summary>
        /// <param name="categoryName">The name of the category to add.</param>
        public void AddCategory(string categoryName)
        {
            if (Categories.Exists(c => c.CategoryName == categoryName))
            {
                Debug.LogWarning($"Category '{categoryName}' already exists.");
                return;
            }

            Categories.Add(new EventCategory { CategoryName = categoryName });
            Debug.Log($"Category '{categoryName}' added successfully.");
        }

        /// <summary>
        /// Removes the category with the given name and all associated events.
        /// </summary>
        /// <param name="categoryName">The name of the category to remove.</param>
        public void RemoveCategory(string categoryName)
        {
            var category = Categories.Find(c => c.CategoryName == categoryName);
            if (category == null)
            {
                Debug.LogWarning($"Category '{categoryName}' not found.");
                return;
            }

            // Remove events in the category
            foreach (var eventSO in category.Events)
            {
                if (eventSO != null)
                {
                    idToEvent.Remove(eventSO.EventID);
                    tagToID.Remove(eventSO.EventTag);
                }
            }

            Categories.Remove(category);
            Debug.Log($"Category '{categoryName}' removed successfully.");
        }

        /// <summary>
        /// Adds an event to the specified category, automatically assigning a unique ID.
        /// </summary>
        /// <param name="categoryName">The name of the category to add the event to.</param>
        /// <param name="eventSO">The event to add.</param>
        public void AddEvent(string categoryName, GeneralEventSO eventSO)
        {
            if (eventSO == null)
            {
                Debug.LogWarning("Cannot add a null event.");
                return;
            }

            var category = Categories.Find(c => c.CategoryName == categoryName);
            if (category == null)
            {
                Debug.LogWarning($"Category '{categoryName}' not found.");
                return;
            }

            // Check for duplicate events
            if (idToEvent.ContainsKey(eventSO.EventID))
            {
                Debug.LogWarning($"Event with ID '{eventSO.EventID}' already exists.");
                return;
            }

            // Assign a unique ID
            eventSO.EventID = GenerateUniqueID();

            // Add event
            category.Events.Add(eventSO);
            idToEvent[eventSO.EventID] = eventSO;

            if (!tagToID.ContainsKey(eventSO.EventTag))
            {
                tagToID[eventSO.EventTag] = eventSO.EventID;
            }
            else
            {
                Debug.LogWarning($"Duplicate Tag '{eventSO.EventTag}' detected for another event.");
            }

            Debug.Log($"Event '{eventSO.name}' added to category '{categoryName}' with ID '{eventSO.EventID}'.");
        }

        /// <summary>
        /// Removes an event from the specified category.
        /// </summary>
        /// <param name="categoryName">The name of the category to remove the event from.</param>
        /// <param name="eventID">The ID of the event to remove.</param>
        public void RemoveEvent(string categoryName, int eventID)
        {
            var category = Categories.Find(c => c.CategoryName == categoryName);
            if (category == null)
            {
                Debug.LogWarning($"Category '{categoryName}' not found.");
                return;
            }

            var eventSO = idToEvent.GetValueOrDefault(eventID);
            if (eventSO == null)
            {
                Debug.LogWarning($"Event with ID '{eventID}' not found.");
                return;
            }

            if (category.Events.Remove(eventSO))
            {
                idToEvent.Remove(eventID);
                tagToID.Remove(eventSO.EventTag);

                Debug.Log($"Event with ID '{eventID}' removed from category '{categoryName}'.");
            }
            else
            {
                Debug.LogWarning($"Event with ID '{eventID}' not found in category '{categoryName}'.");
            }
        }

        #endregion

        #region EventSystem

        /// <summary>
        /// Finds an event by its tag.
        /// </summary>
        /// <param name="tag">The tag of the event to find.</param>
        /// <returns>The event associated with the tag, or null if not found.</returns>
        public GeneralEventSO FindEventByTag(string tag)
        {
            if (tagToID.TryGetValue(tag, out var eventID))
            {
                idToEvent.TryGetValue(eventID, out var eventSO);
                return eventSO;
            }
            return null;
        }

        #endregion
        /// <summary>
        /// Generates a unique ID based on the existing keys in idToEvent.
        /// </summary>
        /// <returns>A unique ID.</returns>
        private int GenerateUniqueID()
        {
            int id;
            do
            {
                id = random.Next(); // Generate a random integer
            }
            while (idToEvent.ContainsKey(id)); // Check if the ID already exists in idToEvent

            return id;
        }
    }
}
