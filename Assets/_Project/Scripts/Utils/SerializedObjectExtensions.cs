using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// SerializedObject extension methods
    /// </summary>
    public static class SerializedObjectExtensions
    {
        /// <summary>
        /// Get property
        /// </summary>
        public static UnityEditor.SerializedProperty GetProperty(this UnityEditor.SerializedObject serializedObject, string propertyName)
        {
            return serializedObject.FindProperty(propertyName);
        }

        /// <summary>
        /// Get or create property
        /// </summary>
        public static UnityEditor.SerializedProperty GetOrCreateProperty(this UnityEditor.SerializedObject serializedObject, string propertyName)
        {
            UnityEditor.SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                Debug.LogWarning($"Property {propertyName} not found");
            }
            return property;
        }

        /// <summary>
        /// Set property value
        /// </summary>
        public static void SetPropertyValue<T>(this UnityEditor.SerializedObject serializedObject, string propertyName, T value)
        {
            UnityEditor.SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null)
            {
                switch (property.propertyType)
                {
                    case UnityEditor.SerializedPropertyType.Integer:
                        property.intValue = (int)(object)value;
                        break;
                    case UnityEditor.SerializedPropertyType.Boolean:
                        property.boolValue = (bool)(object)value;
                        break;
                    case UnityEditor.SerializedPropertyType.Float:
                        property.floatValue = (float)(object)value;
                        break;
                    case UnityEditor.SerializedPropertyType.String:
                        property.stringValue = (string)(object)value;
                        break;
                    case UnityEditor.SerializedPropertyType.Color:
                        property.colorValue = (Color)(object)value;
                        break;
                    case UnityEditor.SerializedPropertyType.ObjectReference:
                        property.objectReferenceValue = (UnityEngine.Object)(object)value;
                        break;
                }
                serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// Get property value
        /// </summary>
        public static T GetPropertyValue<T>(this UnityEditor.SerializedObject serializedObject, string propertyName)
        {
            UnityEditor.SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                return default;
            }

            switch (property.propertyType)
            {
                case UnityEditor.SerializedPropertyType.Integer:
                    return (T)(object)property.intValue;
                case UnityEditor.SerializedPropertyType.Boolean:
                    return (T)(object)property.boolValue;
                case UnityEditor.SerializedPropertyType.Float:
                    return (T)(object)property.floatValue;
                case UnityEditor.SerializedPropertyType.String:
                    return (T)(object)property.stringValue;
                case UnityEditor.SerializedPropertyType.Color:
                    return (T)(object)property.colorValue;
                case UnityEditor.SerializedPropertyType.ObjectReference:
                    return (T)(object)property.objectReferenceValue;
                default:
                    return default;
            }
        }
    }
}
