using System.Collections.Generic;
using UnityEngine;

namespace EventSO
{
    /// <summary>
    /// Represents a general event with unique ID, tag, and identifiers.
    /// </summary>
    public class GeneralEventSO : ScriptableObject
    {
        [Header("Event Info")]
        public int EventID; // Unique identifier for the event
        public string EventTag; // Tag associated with the event

        [Header("Event Identifiers")]
        public List<string> Identifiers = new List<string>(); // Additional identifiers for the event
    }
}
