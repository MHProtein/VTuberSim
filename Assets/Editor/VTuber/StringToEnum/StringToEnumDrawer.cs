using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using VTuber.Core.StringToEnum;

namespace Editor.VTuber.StringToEnum
{
    [CustomPropertyDrawer(typeof(StringToEnumAttribute))]
    public class StringToEnumDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            if (property.propertyType != SerializedPropertyType.String)
            {
                base.OnGUI(position, property, label);
                return;
            }

            void SetValue(string str)
            {
                property.stringValue = str;
                property.serializedObject.ApplyModifiedProperties();
            }

            position = EditorGUI.PrefixLabel(position, UnityEngine.GUIUtility.GetControlID(FocusType.Passive), label);

            string str = property.stringValue;
            var e1Rect = new Rect(position.x, position.y, position.width * 0.5f - 5, 
                position.height);
            var e2Rect = new Rect(position.x + position.width * 0.5f + 5, position.y, position.width * 0.5f - 5,
                position.height);
            string labelContent = string.IsNullOrEmpty(str) ? "Select Enum" : str;

            if (EditorGUI.DropdownButton(position, new GUIContent(labelContent), FocusType.Keyboard))
            {
                GeneralSearchWindow.GeneralSearchWindow searchWindow = ScriptableObject.CreateInstance<GeneralSearchWindow.GeneralSearchWindow>();
                if (EnumDatabase.Instance == null)
                {
                    Debug.Log("⚠️ EnumDataBase.Instance is null");
                    return;
                }
                
                var enumData = EnumDatabase.Instance.GetEnumData(((StringToEnumAttribute)attribute).Key);
                
                searchWindow.Init(enumData, (index) => SetValue(enumData[index]));
                
                Rect center = position;
                float width = 120;
                float height = 26;
                center.position += Vector2.right * width;
                center.position += Vector2.up * (position.height / 2 + height);
                Vector2 screenPosition = UnityEngine.GUIUtility.GUIToScreenPoint(new Vector2(center.x, center.y));

                SearchWindow.Open(new SearchWindowContext(screenPosition), searchWindow);
            }
            EditorGUI.EndProperty();
        }
    }
}