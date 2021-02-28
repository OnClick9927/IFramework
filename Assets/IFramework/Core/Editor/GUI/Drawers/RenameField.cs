/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.214
 *UnityVersion:   2018.4.24f1
 *Date:           2020-12-11
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
using UnityEditor;
using System;

namespace IFramework.GUITool
{
    public class RenameField : GUIBase
    {
        public string value;
        private bool _label = true;
        public bool outline = true;
        public Color outlineColor = Color.white;
        public event Action<string> onValueChange;
        private string _id = Guid.NewGuid().ToString();

        private string textName { get { return string.Format("RenameFieldtext{0}", _id); } }

        public override void OnGUI(Rect position)
        {
            base.OnGUI(position);
            if (outline)
            {
                position.DrawOutLine(2, outlineColor);
            }
            if (_label)
            {
                EditorGUI.LabelField(position, value);
            }
            else
            {
                GUI.SetNextControlName(textName);
                string _value = value;
                _value = EditorGUI.TextField(position, "", _value);
                if (_value != value)
                {
                    value = _value;
                    if (onValueChange != null)
                    {
                        onValueChange.Invoke(value);
                    }
                }
            }
            Event e = Event.current;
            if (e.keyCode == KeyCode.F2 && position.Contains(e.mousePosition))
            {
                _label = false;
                GUI.FocusControl(textName);
                if (e.type != EventType.Layout)
                {
                    e.Use();
                }
            }
            else if (e.keyCode == KeyCode.Return || e.character == '\n')
            {
                if (GUI.GetNameOfFocusedControl() == textName)
                {
                    _label = true;
                    GUI.FocusControl("");
                    if (e.type != EventType.Layout)
                    {
                        e.Use();
                    }
                }
            }
            else if (e.clickCount == 1)
            {
                if (!position.Contains(e.mousePosition))
                {
                    _label = true;
                    if (GUI.GetNameOfFocusedControl() == textName)
                    {
                        if (e.type != EventType.Layout)
                        {
                            GUI.FocusControl("");
                            e.Use();
                        }
                    }
                }
                else if (e.button == 1 && position.Contains(e.mousePosition) && _label)
                {
                    Vector2 pos = e.mousePosition;
                    EditorUtility.DisplayCustomMenu(position, new GUIContent[] {
                        new GUIContent("Copy"),
                        new GUIContent("Paste"),
                        new GUIContent("Clear"),
                        new GUIContent("Rename"),
                    },
                    (index) => {
                        switch (index)
                        {
                            case 0: return !string.IsNullOrEmpty(value);
                            case 1: return !string.IsNullOrEmpty(GUIUtility.systemCopyBuffer);
                            case 2: return !string.IsNullOrEmpty(value);
                        }
                        return true;
                    },
                    -1,
                    (object userData, string[] options, int selected) => {
                        switch (selected)
                        {
                            case 0: GUIUtility.systemCopyBuffer = value; break;
                            case 1: value = GUIUtility.systemCopyBuffer; break;
                            case 2: value = ""; break;
                            case 3:
                                _label = false;
                                GUI.FocusControl(textName);
                                if (e.type != EventType.Layout && e.type != EventType.Repaint)
                                {
                                    e.Use();
                                }
                                break;
                        }
                    }, null);
                }
            }
        }
        protected override void OnDispose()
        {
            onValueChange = null;
        }
    }
}
