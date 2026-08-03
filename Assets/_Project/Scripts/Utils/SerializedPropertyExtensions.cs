using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// SerializedProperty extension methods
    /// </summary>
    public static class SerializedPropertyExtensions
    {
        /// <summary>
        /// Get value
        /// </summary>
        public static T GetValue<T>(this UnityEditor.SerializedProperty property)
        {
            #if UNITY_EDITOR
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
                case UnityEditor.SerializedPropertyType.Enum:
                    return (T)(object)property.enumValueIndex;
                case UnityEditor.SerializedPropertyType.Vector2:
                    return (T)(object)property.vector2Value;
                case UnityEditor.SerializedPropertyType.Vector3:
                    return (T)(object)property.vector3Value;
                case UnityEditor.SerializedPropertyType.Vector4:
                    return (T)(object)property.vector4Value;
                case UnityEditor.SerializedPropertyType.Rect:
                    return (T)(object)property.rectValue;
                case UnityEditor.SerializedPropertyType.AnimationCurve:
                    return (T)(object)property.animationCurveValue;
                case UnityEditor.SerializedPropertyType.Bounds:
                    return (T)(object)property.boundsValue;
                default:
                    return default;
            }
            #else
            return default;
            #endif
        }

        /// <summary>
        /// Set value
        /// </summary>
        public static void SetValue<T>(this UnityEditor.SerializedProperty property, T value)
        {
            #if UNITY_EDITOR
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
                case UnityEditor.SerializedPropertyType.Enum:
                    property.enumValueIndex = (int)(object)value;
                    break;
                case UnityEditor.SerializedPropertyType.Vector2:
                    property.vector2Value = (Vector2)(object)value;
                    break;
                case UnityEditor.SerializedPropertyType.Vector3:
                    property.vector3Value = (Vector3)(object)value;
                    break;
                case UnityEditor.SerializedPropertyType.Vector4:
                    property.vector4Value = (Vector4)(object)value;
                    break;
                case UnityEditor.SerializedPropertyType.Rect:
                    property.rectValue = (Rect)(object)value;
                    break;
                case UnityEditor.SerializedPropertyType.AnimationCurve:
                    property.animationCurveValue = (AnimationCurve)(object)value;
                    break;
                case UnityEditor.SerializedPropertyType.Bounds:
                    property.boundsValue = (Bounds)(object)value;
                    break;
            }
            property.serializedObject.ApplyModifiedProperties();
            #endif
        }
    }
}
