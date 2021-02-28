/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-09-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;

namespace IFramework.GUITool
{
    public class SearchField :GUIBase
    {
        //private Guid uuid = Guid.NewGuid();
        public string value = "";
        public event Action<string> onEndEdit;
        public event Action<string> onValueChange;
        public event Action<int> onModeChange;
        public string[] modes;
        public int mode;
        private MethodInfo info;
        private int controlID;
        public SearchField(string value ,string[] modes,int mode)
        {
            this.mode = mode;
            this.value = value;
            this.modes = modes;
            info = typeof(EditorGUI).GetMethod("ToolbarSearchField",
                BindingFlags.NonPublic | BindingFlags.Static,
                null,
                new System.Type[]
                { typeof(int),typeof(Rect), typeof(string[]) ,typeof(int).MakeByRefType(),typeof(string)},
                null);
        }
        protected override void OnDispose()
        {
            onValueChange = null;
            onEndEdit = null;
        }
        public override void OnGUI(Rect position)
        {
            base.OnGUI(position);
            if (info != null)
            {
                controlID = GUIUtility.GetControlID(("EditorSearchField" /*+ uuid.ToString()*/).GetHashCode(), FocusType.Keyboard, position);

                int _mode = mode;
                object[] args = new object[] { controlID,position, modes, _mode, value };
                string tmp = (string)info.Invoke(null, args);
                if ((int)args[3]!=mode)
                {
                    mode = (int)args[3];
                    if (onModeChange != null)
                        onModeChange(mode);
                }
                if (tmp!=value)
                {
                    value = tmp;
                    if (onValueChange!=null)
                        onValueChange(value);
                }
                Event e = Event.current;
                if ((e.keyCode == KeyCode.Return || e.keyCode == KeyCode.Escape || e.character == '\n'))
                {
                    if (GUIUtility.keyboardControl == controlID)
                    {
                        GUIUtility.keyboardControl = -1;
                        if (e.type != EventType.Repaint && e.type != EventType.Layout)
                            Event.current.Use();
                        if (onEndEdit != null) onEndEdit(value);
                    }
      
                }
            }
           
        }

    }

}
