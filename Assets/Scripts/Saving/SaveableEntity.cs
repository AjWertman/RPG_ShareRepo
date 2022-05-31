using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPGProject.Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] string uniqueIdentifier = "";
        static Dictionary<string, SaveableEntity> globalIDLookup = new Dictionary<string, SaveableEntity>();

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("uniqueIdentifier");

            if (string.IsNullOrEmpty(serializedProperty.stringValue) || !IsUnique(serializedProperty.stringValue))
            {
                serializedProperty.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            globalIDLookup[serializedProperty.stringValue] = this;
        }
#endif

        private bool IsUnique(string _stringToTest)
        {
            if (!globalIDLookup.ContainsKey(_stringToTest))
            {
                return true;
            }

            if (globalIDLookup[_stringToTest] == this)
            {
                return true;
            }

            if (globalIDLookup[_stringToTest] == null)
            {
                globalIDLookup.Remove(_stringToTest);
                return true;
            }

            if (globalIDLookup[_stringToTest].GetUniqueIdentifier() != _stringToTest)
            {
                globalIDLookup.Remove(_stringToTest);
                return true;
            }

            return false;
        }

        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();

            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }

            return state;
        }

        public void RestoreState(object _state)
        {
            Dictionary<string, object> stateDictionary = (Dictionary<string, object>)_state;

            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                string typeString = saveable.GetType().ToString();

                if (stateDictionary.ContainsKey(typeString))
                {
                    saveable.RestoreState(stateDictionary[typeString]);
                }
            }
        }
    }
}