using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace EventSO
{
    [CustomEditor(typeof(EventListSO))]
    public class EventListSOEditor : Editor
    {
        private string newCategoryName = "";
        private string newEventName = "";
        private string newIdentifier = "";
        private int selectedCategoryIndex = 0;
        private int selectedEventIndex = 0;
        private int selectedIdentifierRemoveIndex = 0;
        private bool[] showIdentifiers;
        private bool showIdentifiersFoldout = false;

        private List<bool> categoryFoldouts = new List<bool>();
        public override void OnInspectorGUI()
        {
            EventListSO eventList = (EventListSO)target;

            InitializeShowIdentifiers(eventList);
            InitializeCategoryFoldouts(eventList);
            serializedObject.Update();
            DrawCategories();
            DrawCategoryManagement(eventList);
            if (eventList.Categories.Count > 0)
            {
                DrawEventManagement(eventList);
            }
            if (eventList.Categories.Count > 0 && eventList.Categories[selectedCategoryIndex].Events.Count > 0)
            {
                DrawIdentifierManagement(eventList);
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void InitializeShowIdentifiers(EventListSO eventList)
        {
            if (showIdentifiers == null || showIdentifiers.Length != eventList.Categories.Count)
            {
                showIdentifiers = new bool[eventList.Categories.Count];
            }
        }
        private void InitializeCategoryFoldouts(EventListSO eventList)
        {
            if (categoryFoldouts.Count != eventList.Categories.Count)
            {
                categoryFoldouts = new List<bool>(new bool[eventList.Categories.Count]);
            }
        }
        private void DrawCategories()
        {
            SerializedProperty categories = serializedObject.FindProperty("Categories");
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Event Categories", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            for (int i = 0; i < categories.arraySize; i++)
            {
                SerializedProperty category = categories.GetArrayElementAtIndex(i);
                SerializedProperty categoryName = category.FindPropertyRelative("CategoryName");
                SerializedProperty events = category.FindPropertyRelative("Events");

                EditorGUILayout.BeginVertical("box");
                categoryFoldouts[i] = EditorGUILayout.Foldout(categoryFoldouts[i], categoryName.stringValue, true);

                if (categoryFoldouts[i])
                {
                    DrawEvents(events);
                }

                EditorGUILayout.EndVertical();
            }
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }

        private void DrawEvents(SerializedProperty events)
        {
            EditorGUI.indentLevel++;
            for (int j = 0; j < events.arraySize; j++)
            {
                SerializedProperty eventSO = events.GetArrayElementAtIndex(j);
                GeneralEventSO eventObject = (GeneralEventSO)eventSO.objectReferenceValue;

                EditorGUILayout.BeginHorizontal();
                if (eventObject != null)
                {
                    EditorGUILayout.LabelField($"Event: {eventObject.EventTag}", GUILayout.Width(200));
                    if (GUILayout.Button("Detial", GUILayout.Width(60)))
                    {
                        Selection.activeObject = eventObject;
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Event: Missing Reference");
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }

        private void DrawCategoryManagement(EventListSO eventList)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Category Management", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            DrawAddCategory(eventList);
            DrawRemoveCategory(eventList);
            EditorGUI.indentLevel--;
        }

        private void DrawAddCategory(EventListSO eventList)
        {
            EditorGUILayout.LabelField("Add Category", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            newCategoryName = EditorGUILayout.TextField("New Category Name", newCategoryName);
            if (GUILayout.Button("Add Category",GUILayout.Width(60)))
            {
                if (!string.IsNullOrEmpty(newCategoryName))
                {
                    eventList.AddCategory(newCategoryName);
                    categoryFoldouts.Add(false);
                    newCategoryName = "";
                    EditorUtility.SetDirty(eventList);
                }
                else
                {
                    Debug.LogWarning("Category name cannot be empty.");
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawRemoveCategory(EventListSO eventList)
        {
            if (eventList.Categories.Count > 0)
            {
                EditorGUILayout.LabelField("Remove Category", EditorStyles.boldLabel);
                string[] categoryNames = eventList.Categories.ConvertAll(c => c.CategoryName).ToArray();
                selectedCategoryIndex = EditorGUILayout.Popup("Select Category to Remove", selectedCategoryIndex, categoryNames);

                if (GUILayout.Button("Remove Selected Category"))
                {
                    var category = eventList.Categories[selectedCategoryIndex];
                    foreach (var eventSO in category.Events)
                    {
                        if (eventSO != null)
                        {
                            string path = AssetDatabase.GetAssetPath(eventSO);
                            AssetDatabase.DeleteAsset(path);
                        }
                    }

                    eventList.RemoveCategory(categoryNames[selectedCategoryIndex]);
                    categoryFoldouts.RemoveAt(selectedCategoryIndex);
                    selectedCategoryIndex = 0;
                    EditorUtility.SetDirty(eventList);
                }
            }
        }

        private void DrawEventManagement(EventListSO eventList)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Event Management", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            DrawAddEvent(eventList);
            DrawRemoveEvent(eventList);
            EditorGUI.indentLevel--;
        }

        private void DrawAddEvent(EventListSO eventList)
        {
            EditorGUILayout.LabelField("Add Event", EditorStyles.boldLabel);

            string[] categoryNames = eventList.Categories.ConvertAll(c => c.CategoryName).ToArray();
            selectedCategoryIndex = EditorGUILayout.Popup("Select Category to Add Event", selectedCategoryIndex, categoryNames);
            newEventName = EditorGUILayout.TextField("New Event Name", newEventName);

            if (GUILayout.Button("Add Event"))
            {
                if (!string.IsNullOrEmpty(newEventName))
                {
                    GeneralEventSO newEvent = ScriptableObject.CreateInstance<GeneralEventSO>();
                    newEvent.EventTag = newEventName;

                    string path = $"Assets/ScenesAsset/Script/EventSystem/EventPool/{newEventName}.asset";
                    AssetDatabase.CreateAsset(newEvent, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    eventList.AddEvent(categoryNames[selectedCategoryIndex], newEvent);
                    newEventName = "";
                    EditorUtility.SetDirty(eventList);
                }
                else
                {
                    Debug.LogWarning("Event name cannot be empty.");
                }
            }
        }

        private void DrawRemoveEvent(EventListSO eventList)
        {
            EditorGUILayout.LabelField("Remove Event", EditorStyles.boldLabel);

            if (eventList.Categories.Count > 0)
            {
                string[] categoryNames = eventList.Categories.ConvertAll(c => c.CategoryName).ToArray();
                selectedCategoryIndex = EditorGUILayout.Popup("Select Category", selectedCategoryIndex, categoryNames);

                var selectedCategory = eventList.Categories[selectedCategoryIndex];

                if (selectedCategory.Events.Count > 0)
                {
                    string[] eventTags = selectedCategory.Events.ConvertAll(e => e.EventTag).ToArray();
                    selectedEventIndex = EditorGUILayout.Popup("Select Event", selectedEventIndex, eventTags);

                    if (GUILayout.Button("Remove Selected Event"))
                    {
                        var eventSO = selectedCategory.Events[selectedEventIndex];
                        string path = AssetDatabase.GetAssetPath(eventSO);
                        AssetDatabase.DeleteAsset(path);
                        selectedCategory.Events.RemoveAt(selectedEventIndex);
                        selectedEventIndex = 0;
                        EditorUtility.SetDirty(eventList);

                        Debug.Log($"Removed event '{eventSO.EventTag}' from category '{selectedCategory.CategoryName}'.");
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("No events available in the selected category.");
                }
            }
            else
            {
                EditorGUILayout.LabelField("No categories available.");
            }
        }
        private void DrawIdentifierManagement(EventListSO eventList)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Identifiers Management", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            string[] categoryNames = eventList.Categories.ConvertAll(c => c.CategoryName).ToArray();
            selectedCategoryIndex = EditorGUILayout.Popup("Select Category", selectedCategoryIndex, categoryNames);
            var selectedCategory = eventList.Categories[selectedCategoryIndex];
            if (selectedCategory.Events.Count > 0)
            {
                string[] eventTags = selectedCategory.Events.ConvertAll(e => e.EventTag).ToArray();
                selectedEventIndex = EditorGUILayout.Popup("Select Event", selectedEventIndex, eventTags);
                var selectedEvent = selectedCategory.Events[selectedEventIndex];
                showIdentifiersFoldout = EditorGUILayout.Foldout(showIdentifiersFoldout, "Show Identifiers");
                if (showIdentifiersFoldout)
                {
                    DrawDisplayIdentifiers(selectedEvent);
                }
                EditorGUILayout.LabelField("Add/Delete Identifiers", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                DrawAddIdentifier(selectedEvent);
                DrawRemoveIdentifier(selectedEvent);
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.LabelField("No events available in the selected category.");
            }
            EditorGUI.indentLevel--;
        }
        private void DrawDisplayIdentifiers(GeneralEventSO eventSO)
        {
            if (eventSO.Identifiers.Count > 0)
            {
                EditorGUILayout.LabelField("Identifiers List:");
                EditorGUI.indentLevel++;
                foreach (var identifier in eventSO.Identifiers)
                {
                    EditorGUILayout.LabelField(identifier);
                }
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.LabelField("No Identifiers found.");
            }
        }

        private void DrawAddIdentifier(GeneralEventSO eventSO)
        {
            newIdentifier = EditorGUILayout.TextField("New Identifier", newIdentifier);
            if (GUILayout.Button("Add Identifier"))
            {
                if (!string.IsNullOrEmpty(newIdentifier) && !eventSO.Identifiers.Contains(newIdentifier))
                {
                    eventSO.Identifiers.Add(newIdentifier);
                    newIdentifier = "";
                    EditorUtility.SetDirty(eventSO);
                }
                else
                {
                    Debug.LogWarning("Identifier is empty or already exists.");
                }
            }
        }

        private void DrawRemoveIdentifier(GeneralEventSO eventSO)
        {
            if (eventSO.Identifiers.Count > 0)
            {
                EditorGUILayout.LabelField("Remove Identifiers:");
                string[] identifiers = eventSO.Identifiers.ToArray();
                selectedIdentifierRemoveIndex = EditorGUILayout.Popup("Select Identifier to Remove", selectedIdentifierRemoveIndex, identifiers);

                if (GUILayout.Button("Remove Selected Identifier"))
                {
                    eventSO.Identifiers.RemoveAt(selectedIdentifierRemoveIndex);
                    selectedIdentifierRemoveIndex = Mathf.Clamp(selectedIdentifierRemoveIndex - 1, 0, eventSO.Identifiers.Count - 1);
                    EditorUtility.SetDirty(eventSO);
                }
            }
        }
    }
}
