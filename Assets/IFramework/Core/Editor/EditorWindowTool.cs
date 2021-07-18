/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-09-08
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using UnityEngine;
using System.Reflection;

namespace IFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EditorWindowCacheAttribute : Attribute
    {
        public string searchName { get; private set; }
        public EditorWindowCacheAttribute() { }
        public EditorWindowCacheAttribute(string searchName)
        {
            this.searchName = searchName;
        }

    }
    [OnEnvironmentInit(EnvironmentType.Ev0)]
    public static class EditorWindowTool
    {
        public enum DockType
        {
            Left,
            Top,
            Right,
            Bottom
        }

        public class Entity
        {
            public string searchName { get; private set; }
            public Type type { get; private set; }
            public Rect position
            {
                get
                {
                    EditorWindow window = Find();
                    if (window == null)
                        return new Rect(0, 0, 0, 0);
                    return window.position;
                }
                set
                {
                    EditorWindow window = FindOrCreate();
                    if (window != null)
                        window.position = value;
                }
            }
            public bool isOpen
            {
                get
                {
                    return FindAll().Length != 0;
                }
                set
                {
                    if (value)
                        FindOrCreate();
                    else
                        foreach (EditorWindow window in FindAll())
                            window.Close();
                }
            }

            public Entity(Type type, string searchName = "")
            {
                this.type = type;
                this.searchName = string.IsNullOrEmpty(searchName) ? type.FullName : searchName;
            }

            public EditorWindow Find()
            {
                foreach (EditorWindow window in FindAll())
                {
                    return window;
                }
                return null;
            }
            public EditorWindow Create()
            {
                EditorWindow window = EditorWindow.GetWindow(type);
                return window;
            }
            public EditorWindow[] FindAll()
            {
                if (type == null)
                    return new EditorWindow[0];
                return (EditorWindow[])(Resources.FindObjectsOfTypeAll(type));
            }
            public EditorWindow FindOrCreate()
            {
                EditorWindow window = Find();
                if (window != null) return window;
                if (type == null) return null;
                window = Create();
                return window;
            }
        }

        private static List<Entity> _windows;
        public static List<Entity> windows { get { return _windows; } }
        static FieldInfo m_Parent;

        static EditorWindowTool()
        {
            Type type = typeof(EditorWindow);
            m_Parent = type.GetField("m_Parent", BindingFlags.Instance | BindingFlags.NonPublic);
            _windows = new List<Entity>();
            var em = type.GetSubTypesInAssemblys().GetEnumerator();
            while (em.MoveNext())
            {
                var _type = em.Current;
                if (_type.IsAbstract || !_type.IsDefined(typeof(EditorWindowCacheAttribute), false)) continue;
                EditorWindowCacheAttribute attr = _type.GetCustomAttributes(typeof(EditorWindowCacheAttribute), false).First() as EditorWindowCacheAttribute;
                windows.Add(new Entity(_type, attr.searchName));
            }
            windows.Sort((a, b) => { return a.searchName.CompareTo(b.searchName); });
            CollectUnityWndows();
        }
        private static void CollectUnityWndows()
        {
            typeof(EditorWindow).GetSubTypesInAssemblys().ForEach((type) =>
            {
                if (type.Namespace != null && type.Namespace.Contains("UnityEditor") && !type.IsAbstract)
                {
                    windows.Add(new Entity(type, type.Name));
                }
            });
        }
        public static bool Exist(string name)
        {
            return FindEntity(name) != null;
        }
        public static Entity FindEntity(string name)
        {
            return windows.Find((info) => { return info.searchName == name; });
        }
        public static EditorWindow Find(string name)
        {
            Entity item = FindEntity(name);
            return item == null ? null : item.Find();
        }
        public static EditorWindow Create(string name)
        {
            Entity item = FindEntity(name);
            return item == null ? null : item.Create();
        }
        public static EditorWindow[] FindAll(string name)
        {
            Entity item = FindEntity(name);
            return item == null ? null : item.FindAll();
        }
        public static EditorWindow FindOrCreate(string name)
        {
            Entity item = FindEntity(name);
            return item == null ? null : item.FindOrCreate();
        }



        public static Rect LocalPosition(this EditorWindow self)
        {
            return new Rect(Vector2.zero, self.position.size);
        }
        public static void Dock(this EditorWindow self, EditorWindow child, DockType type)
        {
            var anchorParent = GetParentOf(self);
            SetDragSource(anchorParent, GetParentOf(child));
            PerformDrop(GetWindowOf(anchorParent), child, GetFakeMousePosition(self, type));
        }



        private static object GetParentOf(object target)
        {
            return m_Parent.GetValue(target);
        }
        private static object GetWindowOf(object target)
        {
            var property = target.GetType().GetProperty("window", BindingFlags.Instance | BindingFlags.Public);
            return property.GetValue(target, null);
        }
        private static void SetDragSource(object target, object source)
        {
            var field = target.GetType().GetField("s_OriginalDragSource", BindingFlags.Static | BindingFlags.NonPublic);
            field.SetValue(null, source);
        }
        private static void PerformDrop(object window, EditorWindow child, Vector2 screenPoint)
        {
            var rootSplitViewProperty = window.GetType().GetProperty("rootSplitView", BindingFlags.Instance | BindingFlags.Public);
            object rootSplitView = rootSplitViewProperty.GetValue(window, null);

            var dragMethod = rootSplitView.GetType().GetMethod("DragOver", BindingFlags.Instance | BindingFlags.Public);
            var dropMethod = rootSplitView.GetType().GetMethod("PerformDrop", BindingFlags.Instance | BindingFlags.Public);
            var dropInfo = dragMethod.Invoke(rootSplitView, new object[] { child, screenPoint });
            if (dropInfo == null) return;
            FieldInfo fi = dropInfo.GetType().GetField("dropArea");
            if (fi != null && fi.GetValue(dropInfo) == null) return;

            dropMethod.Invoke(rootSplitView, new object[] { child, dropInfo, screenPoint });
        }
        private static Vector2 GetFakeMousePosition(EditorWindow wnd, DockType position)
        {
            Vector2 mousePosition = Vector2.zero;
            switch (position)
            {
                case DockType.Left: mousePosition = new Vector2(20, wnd.position.size.y / 2); break;
                case DockType.Top: mousePosition = new Vector2(wnd.position.size.x / 2, 20); break;
                case DockType.Right: mousePosition = new Vector2(wnd.position.size.x - 20, wnd.position.size.y / 2); break;
                case DockType.Bottom: mousePosition = new Vector2(wnd.position.size.x / 2, wnd.position.size.y - 20); break;
            }
            return new Vector2(wnd.position.x + mousePosition.x, wnd.position.y + mousePosition.y);
        }
    }
}
