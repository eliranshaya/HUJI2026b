#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HUJI
{
    [CustomEditor(typeof(HUJIMonoBehaviour), true)]
    public class HUJIMonoBehaviourEditor : Editor
    {
        // Persisted foldout state, keyed by "<instanceID>:<groupName>"
        private static readonly Dictionary<string, bool> _foldouts = new();

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawScriptField();
            DrawFields();
            DrawButtons();

            serializedObject.ApplyModifiedProperties();
        }

        // ─────────────────────────────────────────
        // DRAW MAIN
        // ─────────────────────────────────────────

        private void DrawScriptField()
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            GUI.enabled = true;
        }

        private void DrawFields()
        {
            var fields = GetFields();

            // Partition fields into: ungrouped, box groups, foldout groups.
            // Order of first appearance is preserved so the inspector layout
            // follows the source-code declaration order.
            var ungrouped = new List<FieldInfo>();
            var boxGroups = new List<string>();
            var foldoutGroups = new List<string>();
            var boxGroupFields = new Dictionary<string, List<FieldInfo>>();
            var foldoutGroupFields = new Dictionary<string, List<FieldInfo>>();

            foreach (var field in fields)
            {
                if (ShouldHide(field)) continue;

                var box = field.GetCustomAttribute<HUJIBoxGroupAttribute>();
                var foldout = field.GetCustomAttribute<HUJIFoldoutGroupAttribute>();

                if (foldout != null)
                {
                    if (!foldoutGroupFields.ContainsKey(foldout.Name))
                    {
                        foldoutGroupFields[foldout.Name] = new List<FieldInfo>();
                        foldoutGroups.Add(foldout.Name);
                    }

                    foldoutGroupFields[foldout.Name].Add(field);
                }
                else if (box != null)
                {
                    if (!boxGroupFields.ContainsKey(box.Name))
                    {
                        boxGroupFields[box.Name] = new List<FieldInfo>();
                        boxGroups.Add(box.Name);
                    }

                    boxGroupFields[box.Name].Add(field);
                }
                else
                {
                    ungrouped.Add(field);
                }
            }

            foreach (var field in ungrouped)
                DrawField(field);

            foreach (var name in boxGroups)
                DrawBoxGroup(name, boxGroupFields[name]);

            foreach (var name in foldoutGroups)
                DrawFoldoutGroup(name, foldoutGroupFields[name]);
        }

        private void DrawBoxGroup(string name, List<FieldInfo> fields)
        {
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (!string.IsNullOrEmpty(name))
                EditorGUILayout.LabelField(name, EditorStyles.boldLabel);

            foreach (var field in fields)
                DrawField(field);

            EditorGUILayout.EndVertical();
        }

        private void DrawFoldoutGroup(string name, List<FieldInfo> fields)
        {
            string key = $"{target.GetInstanceID()}:{name}";
            if (!_foldouts.TryGetValue(key, out bool open))
                open = true;

            GUILayout.Space(5);
            open = EditorGUILayout.Foldout(open, name, true, EditorStyles.foldoutHeader);
            _foldouts[key] = open;

            if (!open) return;

            EditorGUI.indentLevel++;
            foreach (var field in fields)
                DrawField(field);
            EditorGUI.indentLevel--;
        }

        private void DrawField(FieldInfo field)
        {
            var property = serializedObject.FindProperty(field.Name);
            if (property == null) return;

            DrawInfoBoxes(field);
            DrawHeader(field);

            bool readOnly = field.GetCustomAttribute<HUJIReadOnlyAttribute>() != null;
            if (readOnly) GUI.enabled = false;

            EditorGUI.BeginChangeCheck();

            // Progress bar takes over the field display entirely.
            var progress = field.GetCustomAttribute<HUJIProgressBarAttribute>();
            if (progress != null && IsNumeric(property))
            {
                DrawProgressBar(field, property, progress);
            }
            else
            {
                DrawProperty(field, property);
            }

            bool changed = EditorGUI.EndChangeCheck();

            if (readOnly) GUI.enabled = true;

            ApplyValidation(field, property);

            if (changed)
                HandleOnChanged(field);
        }

        // ─────────────────────────────────────────
        // DRAW HELPERS
        // ─────────────────────────────────────────

        private void DrawProperty(FieldInfo field, SerializedProperty property)
        {
            if (field.GetCustomAttribute<HUJIHideLabelAttribute>() != null)
            {
                EditorGUILayout.PropertyField(property, GUIContent.none, true);
                return;
            }

            var range = field.GetCustomAttribute<HUJIRangeAttribute>();
            if (range != null)
            {
                if (property.propertyType == SerializedPropertyType.Float)
                    property.floatValue = EditorGUILayout.Slider(GetLabel(field), property.floatValue, range.Min, range.Max);
                else if (property.propertyType == SerializedPropertyType.Integer)
                    property.intValue = EditorGUILayout.IntSlider(GetLabel(field), property.intValue, (int)range.Min, (int)range.Max);

                return;
            }

            EditorGUILayout.PropertyField(property, new GUIContent(GetLabel(field)), true);
        }

        private void DrawProgressBar(FieldInfo field, SerializedProperty property, HUJIProgressBarAttribute attr)
        {
            float value = property.propertyType == SerializedPropertyType.Float
                ? property.floatValue
                : property.intValue;

            float pct = Mathf.InverseLerp(attr.Min, attr.Max, value);

            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);

            // Save and tint the background so the bar uses the requested colour.
            Color prev = GUI.color;
            if (ColorUtility.TryParseHtmlString(attr.ColorHex, out var color))
                GUI.color = color;

            EditorGUI.ProgressBar(rect, pct, $"{GetLabel(field)}: {value:0.##} / {attr.Max}");
            GUI.color = prev;
        }

        private void DrawHeader(FieldInfo field)
        {
            var header = field.GetCustomAttribute<HUJIHeaderAttribute>();
            if (header != null)
            {
                GUILayout.Space(5);
                EditorGUILayout.LabelField(header.Name, EditorStyles.boldLabel);
            }
        }

        private void DrawInfoBoxes(FieldInfo field)
        {
            foreach (var box in field.GetCustomAttributes<HUJIInfoBoxAttribute>())
            {
                MessageType type = box.Type switch
                {
                    HUJIInfoBoxType.Warning => MessageType.Warning,
                    HUJIInfoBoxType.Error => MessageType.Error,
                    _ => MessageType.Info
                };

                EditorGUILayout.HelpBox(box.Message, type);
            }
        }

        // ─────────────────────────────────────────
        // LOGIC
        // ─────────────────────────────────────────

        private bool ShouldHide(FieldInfo field)
        {
            if (field.GetCustomAttribute<HUJIHideInInspectorAttribute>() != null)
                return true;

            var hideIf = field.GetCustomAttribute<HUJIHideIfAttribute>();
            if (hideIf != null && GetCondition(hideIf.Name))
                return true;

            var showIf = field.GetCustomAttribute<HUJIShowIfAttribute>();
            if (showIf != null && !GetCondition(showIf.Name))
                return true;

            return false;
        }

        private void ApplyValidation(FieldInfo field, SerializedProperty property)
        {
            var min = field.GetCustomAttribute<HUJIMinValueAttribute>();
            if (min != null)
            {
                if (property.propertyType == SerializedPropertyType.Float)
                    property.floatValue = Mathf.Max(property.floatValue, min.Min);
                else if (property.propertyType == SerializedPropertyType.Integer)
                    property.intValue = Mathf.Max(property.intValue, (int)min.Min);
            }

            var max = field.GetCustomAttribute<HUJIMaxValueAttribute>();
            if (max != null)
            {
                if (property.propertyType == SerializedPropertyType.Float)
                    property.floatValue = Mathf.Min(property.floatValue, max.Max);
                else if (property.propertyType == SerializedPropertyType.Integer)
                    property.intValue = Mathf.Min(property.intValue, (int)max.Max);
            }

            var required = field.GetCustomAttribute<HUJIRequiredAttribute>();
            if (required != null && property.propertyType == SerializedPropertyType.ObjectReference
                                 && property.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox(required.Message, MessageType.Error);
            }

            var validate = field.GetCustomAttribute<HUJIValidateInputAttribute>();
            if (validate != null && !RunValidator(validate.MethodName, property))
            {
                EditorGUILayout.HelpBox(validate.Message, MessageType.Error);
            }
        }

        // Invokes the user-supplied validator. Supports two signatures:
        //   bool MethodName()
        //   bool MethodName(T value)   where T matches the field type
        private bool RunValidator(string methodName, SerializedProperty property)
        {
            // Walk the type chain so validators on base classes are found too.
            var type = target.GetType();
            while (type != null)
            {
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.Name == methodName && m.ReturnType == typeof(bool));

                foreach (var m in methods)
                {
                    var parameters = m.GetParameters();
                    try
                    {
                        if (parameters.Length == 0)
                            return (bool)m.Invoke(target, null);

                        if (parameters.Length == 1)
                        {
                            object value = GetPropertyValue(property);
                            if (value != null && parameters[0].ParameterType.IsAssignableFrom(value.GetType()))
                                return (bool)m.Invoke(target, new[] { value });
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }

                type = type.BaseType;
            }

            return true; // No matching validator found — do not flag the field.
        }

        private void HandleOnChanged(FieldInfo field)
        {
            var attr = field.GetCustomAttribute<HUJIOnValueChangedAttribute>();
            if (attr == null) return;

            serializedObject.ApplyModifiedProperties();

            var method = GetMethod(attr.Name);
            method?.Invoke(target, null);

            serializedObject.Update();
        }

        // ─────────────────────────────────────────
        // BUTTONS
        // ─────────────────────────────────────────

        private void DrawButtons()
        {
            var methods = GetButtonMethods();
            if (methods.Count == 0) return;

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

            foreach (var (method, attr) in methods.OrderBy(x => x.attr.Order))
            {
                // Method-level header support.
                var header = method.GetCustomAttribute<HUJIHeaderAttribute>();
                if (header != null)
                {
                    GUILayout.Space(5);
                    EditorGUILayout.LabelField(header.Name, EditorStyles.boldLabel);
                }

                if (GUILayout.Button(attr.ButtonName))
                {
                    method.Invoke(target, null);
                }
            }
        }

        private List<(MethodInfo method, HUJIButtonAttribute attr)> GetButtonMethods()
        {
            var result = new List<(MethodInfo, HUJIButtonAttribute)>();
            var seen = new HashSet<string>();

            var type = target.GetType();
            while (type != null)
            {
                foreach (var m in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    // Honour AllowMultiple = true on HUJIButtonAttribute.
                    var attrs = m.GetCustomAttributes<HUJIButtonAttribute>();
                    foreach (var attr in attrs)
                    {
                        // Avoid duplicate entries for overridden methods.
                        string key = m.Name + "::" + attr.ButtonName;
                        if (seen.Add(key))
                            result.Add((m, attr));
                    }
                }

                type = type.BaseType;
            }

            return result;
        }

        // ─────────────────────────────────────────
        // REFLECTION
        // ─────────────────────────────────────────

        private List<FieldInfo> GetFields()
        {
            var result = new List<FieldInfo>();
            var seen = new HashSet<string>();

            // Walk the type hierarchy from base -> derived
            var types = new Stack<Type>();
            var t = target.GetType();
            while (t != null && t != typeof(MonoBehaviour) && t != typeof(object))
            {
                types.Push(t);
                t = t.BaseType;
            }

            foreach (var type in types)
            {
                var fields = type.GetFields(BindingFlags.Instance |
                                            BindingFlags.Public |
                                            BindingFlags.NonPublic |
                                            BindingFlags.DeclaredOnly); // <-- key: only fields declared on THIS type

                foreach (var f in fields)
                {
                    if (!(f.IsPublic || f.GetCustomAttribute<SerializeField>() != null))
                        continue;

                    // avoid duplicates from `new` shadowing
                    if (!seen.Add(f.Name))
                        continue;

                    result.Add(f);
                }
            }

            return result;
        }

        private bool GetCondition(string name)
        {
            var field = GetField(name);
            if (field != null && field.FieldType == typeof(bool))
                return (bool)field.GetValue(target);

            var method = GetMethod(name);
            if (method != null && method.ReturnType == typeof(bool) && method.GetParameters().Length == 0)
                return (bool)method.Invoke(target, null);

            return false;
        }

        // Walk the inheritance chain so members declared on base classes resolve.
        private FieldInfo GetField(string name)
        {
            var type = target.GetType();
            while (type != null)
            {
                var f = type.GetField(name,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                if (f != null) return f;
                type = type.BaseType;
            }

            return null;
        }

        private MethodInfo GetMethod(string name)
        {
            var type = target.GetType();
            while (type != null)
            {
                var m = type.GetMethod(name,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                if (m != null) return m;
                type = type.BaseType;
            }

            return null;
        }

        private string GetLabel(FieldInfo field)
        {
            var label = field.GetCustomAttribute<HUJILabelTextAttribute>();
            return label != null ? label.Name : ObjectNames.NicifyVariableName(field.Name);
        }

        private static bool IsNumeric(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Float
                   || property.propertyType == SerializedPropertyType.Integer;
        }

        private static object GetPropertyValue(SerializedProperty property)
        {
            return property.propertyType switch
            {
                SerializedPropertyType.Integer => property.intValue,
                SerializedPropertyType.Boolean => property.boolValue,
                SerializedPropertyType.Float => property.floatValue,
                SerializedPropertyType.String => property.stringValue,
                SerializedPropertyType.Color => property.colorValue,
                SerializedPropertyType.ObjectReference => property.objectReferenceValue,
                SerializedPropertyType.Enum => property.enumValueIndex,
                SerializedPropertyType.Vector2 => property.vector2Value,
                SerializedPropertyType.Vector3 => property.vector3Value,
                SerializedPropertyType.Vector4 => property.vector4Value,
                SerializedPropertyType.Rect => property.rectValue,
                SerializedPropertyType.Bounds => property.boundsValue,
                SerializedPropertyType.Quaternion => property.quaternionValue,
                _ => null
            };
        }
    }
}
#endif