/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Object = UnityEngine.Object;
using System.Reflection;
using System.IO;
using UnityEditorInternal;
using System.Text;
using IFramework.GUITool;
using UnityEditor.ProjectWindowCallback;

namespace IFramework
{
    static class CopyAsset
    {
        class CreateAssetAction : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                Object obj = CreateAssetFormTemplate(pathName, resourceFile);
                ProjectWindowUtil.ShowCreatedAsset(obj);
            }

            private static Object CreateAssetFormTemplate(string pathName, string resourceFile)
            {
                string fullName = Path.GetFullPath(pathName);
                StreamReader reader = new StreamReader(resourceFile);
                string content = reader.ReadToEnd();
                reader.Close();
                string fileName = Path.GetFileNameWithoutExtension(pathName);
                content = content.Replace("#NAME", fileName);
                StreamWriter writer = new StreamWriter(fullName, false, System.Text.Encoding.UTF8);
                writer.Write(content);
                writer.Close();
                AssetDatabase.ImportAsset(pathName);
                AssetDatabase.Refresh();
                return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
            }
        }
        public static void Copy(string newFileName, string sourcePath)
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
            ScriptableObject.CreateInstance<CreateAssetAction>(),
           /*Path.Combine(GetSelectedPath(), newFileName)*/ newFileName, null, sourcePath);
        }
    }

    static partial class EditorTools
    {
        [CustomEditor(typeof(Transform)), CanEditMultipleObjects]
        class TransformEditor : Editor
        {
            class CustomFloatField
            {
                private static readonly int Hint = "EditorTextField".GetHashCode();
                private static readonly Type EditorGUIType = typeof(EditorGUI);
                private static readonly Type RecycledTextEditorType = Assembly.GetAssembly(EditorGUIType).GetType("UnityEditor.EditorGUI+RecycledTextEditor");
                private static readonly Type[] ArgumentTypes =
                        {
                    RecycledTextEditorType,
                    typeof (Rect),
                    typeof (Rect),
                    typeof (int),
                    typeof (float),
                    typeof (string),
                    typeof (GUIStyle),
                    typeof (bool)
                    };

                private static readonly MethodInfo DoFloatFieldMethodInfo = EditorGUIType.GetMethod("DoFloatField", BindingFlags.NonPublic | BindingFlags.Static, null, ArgumentTypes, null);
                private static readonly FieldInfo FieldInfo = EditorGUIType.GetField("s_RecycledEditor", BindingFlags.NonPublic | BindingFlags.Static);
                private static readonly object RecycledEditor = FieldInfo.GetValue(null);

                public static float Draw(Rect draw, Rect drag, float value, GUIStyle style)
                {
                    var controlID = GUIUtility.GetControlID(Hint, FocusType.Keyboard, draw);
                    var parameters = new object[] { RecycledEditor, draw, drag, controlID, value, "g7", style, true };

                    return (float)DoFloatFieldMethodInfo.Invoke(null, parameters);
                }
            }
            class TransformRotationGUI
            {
                private object transformRotationGUI;
                private FieldInfo eulerAnglesField;
                private MethodInfo onEnableMethod;
                private MethodInfo rotationFieldMethod;
                private MethodInfo setLocalEulerAnglesMethod;

                private SerializedProperty property;

                public Vector3 eulerAngles { get { return (Vector3)eulerAnglesField.GetValue(transformRotationGUI); } }

                public TransformRotationGUI()
                {
                    if (transformRotationGUI == null)
                    {
                        var transformRotationGUIType = Type.GetType("UnityEditor.TransformRotationGUI,UnityEditor");
                        var transformType = typeof(Transform);
                        eulerAnglesField = transformRotationGUIType.GetField("m_EulerAngles", BindingFlags.Instance | BindingFlags.NonPublic);
                        onEnableMethod = transformRotationGUIType.GetMethod("OnEnable");
                        rotationFieldMethod = transformRotationGUIType.GetMethod("RotationField", new Type[] { });
                        setLocalEulerAnglesMethod = transformType.GetMethod("SetLocalEulerAngles", BindingFlags.Instance | BindingFlags.NonPublic);

                        transformRotationGUI = Activator.CreateInstance(transformRotationGUIType);
                    }
                }

                public void Initialize(SerializedProperty property, GUIContent content)
                {
                    this.property = property;
                    onEnableMethod.Invoke(transformRotationGUI, new object[] { property, content });
                }

                public void Draw()
                {
                    rotationFieldMethod.Invoke(transformRotationGUI, null);
                }

                public void Reset()
                {
                    var targets = property.serializedObject.targetObjects;
                    var parameters = new object[] { Vector3.zero, 0 };

                    Undo.RecordObjects(targets, "Reset Rotation");
                    foreach (var target in targets)
                        setLocalEulerAnglesMethod.Invoke(target, parameters);
                }
            }

            private class Content
            {
                public static readonly GUIContent Position = new GUIContent("Position", "The local position of this GameObject relative to the parent.");
                public static readonly GUIContent Rotation = new GUIContent("Rotation", "The local rotation of this Game Object relative to the parent.");
                public static readonly GUIContent Scale = new GUIContent("Scale", "The local scaling of this GameObject relative to the parent.");
                public static readonly GUIContent ResetPosition = new GUIContent(EditorGUIUtility.IconContent("Refresh").image, "Reset the position.");
                public static readonly GUIContent ResetRotation = new GUIContent(EditorGUIUtility.IconContent("Refresh").image, "Reset the rotation.");
                public static readonly GUIContent ResetScale = new GUIContent(EditorGUIUtility.IconContent("Refresh").image, "Reset the scale.");

                public const string FloatingPointWarning = "Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range.";
            }

            private class Styles
            {
                public static GUIStyle ResetButton;

                static Styles()
                {
                    ResetButton = new GUIStyle()
                    {
                        margin = new RectOffset(0, 0, 2, 0),
                        fixedWidth = 15,
                        fixedHeight = 15
                    };
                }
            }

            private class Properties
            {
                public SerializedProperty Position;
                public SerializedProperty Rotation;
                public SerializedProperty Scale;

                public Properties(SerializedObject obj)
                {
                    Position = obj.FindProperty("m_LocalPosition");
                    Rotation = obj.FindProperty("m_LocalRotation");
                    Scale = obj.FindProperty("m_LocalScale");
                }
            }

            private const int MaxDistanceFromOrigin = 100000;
            private const int ContentWidth = 60;

            private float xyRatio, xzRatio;

            private Properties properties;
            private TransformRotationGUI rotationGUI;

            private void OnEnable()
            {
                properties = new Properties(serializedObject);

                if (rotationGUI == null)
                    rotationGUI = new TransformRotationGUI();
                rotationGUI.Initialize(properties.Rotation, Content.Rotation);
            }

            public override void OnInspectorGUI()
            {
                if (!EditorGUIUtility.wideMode)
                {
                    EditorGUIUtility.wideMode = true;
                    EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 212;
                }

                serializedObject.UpdateIfRequiredOrScript();

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(properties.Position, Content.Position);
                    using (new EditorGUI.DisabledGroupScope(properties.Position.vector3Value == Vector3.zero))
                        if (GUILayout.Button(Content.ResetPosition, Styles.ResetButton))
                            properties.Position.vector3Value = Vector3.zero;
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    rotationGUI.Draw();
                    using (new EditorGUI.DisabledGroupScope(rotationGUI.eulerAngles == Vector3.zero))
                        if (GUILayout.Button(Content.ResetRotation, Styles.ResetButton))
                        {
                            rotationGUI.Reset();
                            if (Tools.current == Tool.Rotate)
                            {
                                if (Tools.pivotRotation == PivotRotation.Global)
                                {
                                    Tools.handleRotation = Quaternion.identity;
                                    SceneView.RepaintAll();
                                }
                            }
                        }
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(properties.Scale, Content.Scale);
                    using (new EditorGUI.DisabledGroupScope(properties.Scale.vector3Value == Vector3.one))
                        if (GUILayout.Button(Content.ResetScale, Styles.ResetButton))
                            properties.Scale.vector3Value = Vector3.one;
                }
                var dragRect = new Rect(16, 105, EditorGUIUtility.labelWidth - 10, 10);

                var e = Event.current;
                if (dragRect.Contains(e.mousePosition) && e.type == EventType.MouseDown && e.button == 0)
                {
                    var currentScale = properties.Scale.vector3Value;
                    xyRatio = currentScale.y / currentScale.x;
                    xzRatio = currentScale.z / currentScale.x;
                }

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var c = GUI.color;
                    GUI.color = Color.clear;
                    var newScaleX = CustomFloatField.Draw(new Rect(), dragRect, properties.Scale.vector3Value.x, EditorStyles.numberField);

                    if (check.changed)
                    {
                        var currentScale = properties.Scale.vector3Value;

                        var delta = newScaleX - properties.Scale.vector3Value.x;

                        currentScale.x += delta;
                        currentScale.y += delta * xyRatio;
                        currentScale.z += delta * xzRatio;

                        properties.Scale.vector3Value = currentScale;
                    }

                    GUI.color = c;
                }

                serializedObject.ApplyModifiedProperties();

                EditorGUIUtility.labelWidth = 0;

                var transform = target as Transform;
                var position = transform.position;

                if
                (
                    Mathf.Abs(position.x) > MaxDistanceFromOrigin ||
                    Mathf.Abs(position.y) > MaxDistanceFromOrigin ||
                    Mathf.Abs(position.z) > MaxDistanceFromOrigin
                )
                    EditorGUILayout.HelpBox(Content.FloatingPointWarning, UnityEditor.MessageType.Warning);
            }

            [MenuItem("CONTEXT/Transform/Set Random Rotation")]
            private static void RandomRotation(MenuCommand command)
            {
                var transform = command.context as Transform;

                Undo.RecordObject(transform, "Set Random Rotation");
                transform.rotation = UnityEngine.Random.rotation;
            }

            [MenuItem("CONTEXT/Transform/Snap to Ground")]
            private static void SnapToGround(MenuCommand command)
            {
                var transform = command.context as Transform;

                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit))
                {
                    Undo.RecordObject(transform, "SnGameed To Ground");
                    transform.position = hit.point;
                }
            }

            [MenuItem("CONTEXT/Transform/Snap to Ground (Physics)", true)]
            private static bool ValidateSnapToGroundPhysics(MenuCommand command)
            {
                return ((Transform)command.context).GetComponent<Collider>() != null;
            }
        }

        [CustomEditor(typeof(DefaultAsset))]
        class DefaultAssetEditorView : Editor
        {
            private string _path;
            private bool _isdir;
            private void OnEnable()
            {
                _path = AssetDatabase.GetAssetPath(this.target);
                _isdir = _path.IsDirectory();
            }
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (_isdir)
                {
                    GUI.enabled = true;
                    if (GUILayout.Button("Open"))
                    {
                        EditorTools.OpenFloder(_path);
                    }
                }
            }
        }

        class WhenDeleteMonoScriptProcessor : UnityEditor.AssetModificationProcessor
        {
            private static AssetDeleteResult OnWillDeleteAsset(string AssetPath, RemoveAssetOptions rao)
            {
                if (!AssetPath.EndsWith(".cs")) return AssetDeleteResult.DidNotDelete;
                MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetPath);
                if (monoScript == null) return AssetDeleteResult.DidNotDelete;
                Type spType = monoScript.GetClass();
                if (spType == null || !spType.IsSubclassOf(typeof(MonoBehaviour))) return AssetDeleteResult.DidNotDelete;

                MonoBehaviour[] monos = Object.FindObjectsOfType(spType) as MonoBehaviour[];
                monos.ForEach((m) =>
                {
                    Object.DestroyImmediate(m);
                });
                string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { @"Assets" });
                if (guids == null || guids.Length <= 0) return AssetDeleteResult.DidNotDelete;
                guids.ToList()
                     .ConvertAll((guid) => { return AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)); })
                     .ForEach((o) => {
                         var cps = o.GetComponentsInChildren(spType, true);
                         if (cps != null && cps.Length > 0)
                         {
                             cps.ForEach((c) =>
                             {
                                 Object.DestroyImmediate(c, true);
                             });
                             AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(o));
                             EditorUtility.SetDirty(o);
                         }

                     });
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                return AssetDeleteResult.DidNotDelete;
            }

        }

        class FormatIFrameworkScript
        {
            const string key = "FormatIFrameworkScript";

            private class FormatIFrameWorkScriptProcessor : UnityEditor.AssetModificationProcessor
            {
                public static void OnWillCreateAsset(string metaPath)
                {
                    if (!EditorPrefs.GetBool(key, false)) return;
                    string filePath = metaPath.Replace(".meta", "");
                    if (!filePath.EndsWith(".cs")) return;
                    string realPath = filePath.ToAbsPath();
                    string txt = File.ReadAllText(realPath);
                    if (!txt.Contains("#FAuthor#")) return;
                    txt = txt.Replace("#FAuthor#", EditorEnv.author)
                             .Replace("#FNameSpace#", EditorEnv.frameworkName)
                             .Replace("#FDescription#", EditorEnv.description)
                             .Replace("#FSCRIPTNAME#", Path.GetFileNameWithoutExtension(filePath))
                             .Replace("#FVERSION#", EditorEnv.version)
                             .Replace("#FUNITYVERSION#", Application.unityVersion)
                             .Replace("#FDATE#", DateTime.Now.ToString("yyyy-MM-dd")).ToUnixLineEndings();
                    File.WriteAllText(realPath, txt, Encoding.UTF8);
                    EditorPrefs.SetBool(key, false);
                }
            }
            private static string newScriptName = "newScript.cs";
            private static string originScriptPath = EditorEnv.formatScriptsPath.CombinePath("AuthorCharpScript.txt");

            [MenuItem("Assets/Create/IFramework/AuthorScript", priority = -1000)]
            public static void Create()
            {
                CreateOriginIfNull();
                CopyAsset.Copy(newScriptName, originScriptPath);
                EditorPrefs.SetBool(key, true);
            }

            private static void CreateOriginIfNull()
            {
                if (File.Exists(originScriptPath)) return;
                using (FileStream fs = new FileStream(originScriptPath, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        fs.Lock(0, fs.Length);
                        sw.WriteLine("/*********************************************************************************");
                        sw.WriteLine(" *Author:         #FAuthor#");
                        sw.WriteLine(" *Version:        #FVERSION#");
                        sw.WriteLine(" *UnityVersion:   #FUNITYVERSION#");
                        sw.WriteLine(" *Date:           #FDATE#");
                        sw.WriteLine(" *Description:    #FDescription#");
                        sw.WriteLine(" *History:        2018.11--");
                        sw.WriteLine("*********************************************************************************/");
                        sw.WriteLine("namespace #FNameSpace#");
                        sw.WriteLine("{");
                        sw.WriteLine("\tpublic class #FSCRIPTNAME#");
                        sw.WriteLine("\t{");
                        sw.WriteLine("\t");
                        sw.WriteLine("\t}");
                        sw.WriteLine("}");
                        fs.Unlock(0, fs.Length);
                        sw.Flush();
                        fs.Flush();
                    }
                }
                AssetDatabase.Refresh();
            }
        }


        [CustomEditor(typeof(ScriptCreater))]
        class ScriptCreaterEditor : Editor
        {
            private ScriptCreater _creater { get { return this.target as ScriptCreater; } }
            private string path { get { return _creater.createDirectory.CombinePath(_creater.scriptName + ".cs"); } }
            private List<string> baseTypes;
            private void OnEnable()
            {
                var list = typeof(MonoBehaviour).GetSubTypesInAssemblys().ToList();
                list.Insert(0, typeof(MonoBehaviour));
                baseTypes = list.ConvertAll((type) => { return type.FullName; });
            }
            private bool DropdownButton(int id, Rect position)
            {
                Event e = Event.current;
                switch (e.type)
                {
                    case EventType.MouseDown:
                        if (position.Contains(e.mousePosition) && e.button == 0)
                        {
                            Event.current.Use();
                            return true;
                        }
                        break;
                    case EventType.KeyDown:
                        if (GUIUtility.keyboardControl == id && e.character == '\n')
                        {
                            Event.current.Use();
                            return true;
                        }
                        break;
                    case EventType.Repaint:
                        //Styles.BoldLabel.Draw(position, content, id, false);
                        break;
                }
                return false;
            }

            public override void OnInspectorGUI()
            {
                GUILayout.Space(10);
                GUILayout.Label("Base Class : " + baseTypes[_creater.searchIndex]);
                Rect pos = GUILayoutUtility.GetLastRect();
                int ctrlId = GUIUtility.GetControlID(GetHashCode(), FocusType.Keyboard, pos);
                {
                    if (DropdownButton(ctrlId, pos))
                    {

                        SearchablePopup.Show(pos, baseTypes.ToArray(), _creater.searchIndex, (i, str) =>
                        {
                            _creater.searchIndex = i;
                        });
                        GUIUtility.ExitGUI();
                    }
                }

                _creater.scriptName = EditorGUILayout.TextField("Script Name", _creater.scriptName);
                if (!_creater.scriptName.IsLegalFieldName())
                    _creater.scriptName = _creater.name.Replace(" ", "").Replace("(", "").Replace(")", "");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("marks"), true);
                GUILayout.Label("Description");
                _creater.description = EditorGUILayout.TextArea(_creater.description, GUILayout.Height(40));
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(new GUIContent("Create Path:", "Drag Floder To Box"));
                    Rect rect = EditorGUILayout.GetControlRect();
                    rect.DrawOutLine(2, Color.black);
                    EditorGUI.LabelField(rect, _creater.createDirectory);
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        var info = EditorTools.DragAndDropTool.Drag(Event.current, rect);
                        if (info.paths.Length > 0 && info.compelete && info.enterArera && info.paths[0].IsDirectory())
                            _creater.createDirectory = info.paths[0];
                    }
                    GUILayout.EndHorizontal();
                }
                if (serializedObject != null)
                {
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                }

                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Build", GUILayout.Height(25)))
                    {
                        BuildScript();
                    }
                    if (GUILayout.Button("Bind ", GUILayout.Height(25)))
                    {
                        if (EditorApplication.isCompiling)
                        {
                            EditorUtility.DisplayDialog("Warnning", "Please Wait  Editor Compiling", "ok");
                            return;
                        }
                        SetFields();
                    }
                    if (GUILayout.Button("Remove", GUILayout.Height(25)))
                    {
                        _creater.GetComponentsInChildren<ScriptMark>(true).ToList().ForEach((sm) => {
                            DestroyImmediate(sm);
                        });
                        DestroyImmediate(_creater);
                    }
                    GUILayout.EndHorizontal();
                }

            }

            private void BuildScript()
            {
                if (EditorApplication.isCompiling) return;
                if (_creater.marks == null || _creater.marks.Length == 0)
                {
                    _creater.marks = _creater.GetComponentsInChildren<ScriptMark>(true);

                }

                if (BuildCheck())
                {
                    string txt = formatScript;

                    if (!txt.Contains("#SCAuthor#")) return;
                    txt = txt.Replace("#SCAuthor#", ProjectConfig.UserName)
                             .Replace("#SCVERSION#", ProjectConfig.Version)
                             .Replace("#SCUNITYVERSION#", Application.unityVersion)
                             .Replace("#SCDATE#", DateTime.Now.ToString("yyyy-MM-dd"))
                             .Replace("#SCNameSpace#", ProjectConfig.NameSpace)
                             .Replace("#SCSCRIPTNAME#", _creater.scriptName)
                             .Replace("#BaseClass#", baseTypes[_creater.searchIndex])
                             .Replace("#SCDescription#", DescriptionString())
                             .Replace("#SCField#", FieldString());
                    File.WriteAllText(path, txt, Encoding.UTF8);
                    AssetDatabase.Refresh();
                }
            }
            private string DescriptionString()
            {
                string res = string.IsNullOrEmpty(_creater.description) ? ProjectConfig.Description : _creater.description;
                if (!res.Contains("\n"))
                    return res;
                else
                {
                    string s = string.Empty;
                    var strs = res.Split('\n');
                    for (int i = 0; i < strs.Length; i++)
                    {
                        string str = strs[i];
                        if (i == 0)
                        {
                            s = s.Append(str);
                            if (strs.Length > 1)
                                s = s.Append("\n");
                        }
                        else
                        {
                            s = s.Append("                  " + str);
                            if (i < strs.Length - 1)
                                s = s.Append("\n");
                        }
                    }

                    return s;
                }

            }
            private string FieldString()
            {
                string result = string.Empty;
                _creater.marks.ForEach((mark) => {
                    if (!string.IsNullOrEmpty(mark.description))
                    {
                        if (mark.description.Contains("\n"))
                        {
                            mark.description.Split('\n').ToList().ForEach((str) =>
                            {
                                result = result.Append("\t\t//" + str + "\n");
                            });
                        }
                        else
                            result = result.Append("\t\t//" + mark.description + "\n");

                    }
                    var fieldType = mark.fieldType;
                    if (mark.isPublic)
                        result = result.Append("\t\tpublic " + fieldType + " " + mark.fieldName + ";\n");
                    else
                        result = result.Append("\t\t[UnityEngine.SerializeField] private " + fieldType + " " + mark.fieldName + ";\n");

                });
                return result;
            }
            private bool BuildCheck()
            {
                Assembly defaultAssembly =
        AppDomain.CurrentDomain
                 .GetAssemblies()
                 .First(assembly => assembly.GetName().Name == "Assembly-CSharp");
                Type type = defaultAssembly.GetType(ProjectConfig.NameSpace + "." + _creater.scriptName);
                if (type != null)
                {
                    bool bo = true;
                    if (File.Exists(path))
                    {
                        bo = EditorUtility.DisplayDialog("Warnning", "The File Exist \n" + path + "\n Overwrite the original file ?", "yes", "no");
                    }
                    if (bo)
                    {
                        bo = EditorUtility.DisplayDialog("Warnning", "The Tppe Exist\n" + type.FullName + "  Overwrite the original file ?", "yes", "no");
                    }
                    if (!bo) return false;
                }
                for (int i = 0; i < _creater.marks.Length; i++)
                {
                    var mark = _creater.marks[i];
                    if (mark.fieldName == _creater.scriptName)
                    {
                        EditorUtility.DisplayDialog("Err", "Field Name Should be diferent With ScriptName", "ok");
                        return false;
                    }
                    var sameFields = _creater.marks.ToList().FindAll((__sm) => { return mark.fieldName == __sm.fieldName; });
                    if (sameFields.Count > 1)
                    {
                        EditorUtility.DisplayDialog("Err", "Can't Exist Same Name Field", "ok");
                        return false;
                    }

                }

                if (!Directory.Exists(_creater.createDirectory))
                {
                    EditorUtility.DisplayDialog("Err", "Directory Not Exist ", "ok");
                    return false;
                }
                return true;
            }

            private void SetFields()
            {
                if (!File.Exists(path)) return;

                Assembly defaultAssembly =
                    AppDomain.CurrentDomain
                             .GetAssemblies()
                             .First(assembly => assembly.GetName().Name == "Assembly-CSharp");
                Type type = defaultAssembly.GetType(ProjectConfig.NameSpace + "." + _creater.scriptName);

                if (type == null) return;

                ScriptMark[] marks = _creater.marks;
                Component component = _creater.GetComponent(type);
                if (component == null) component = _creater.gameObject.AddComponent(type);
                SerializedObject serialiedScript = new SerializedObject(component);

                foreach (var _mark in marks)
                {
                    var _type = _mark.fieldType;
                    if (_type.StartsWith("UnityEngine.") && _type.LastIndexOf(".") == "UnityEngine".Length)
                    {
                        _type = _type.Replace("UnityEngine.", "");
                    }
                    serialiedScript.FindProperty(_mark.fieldName).objectReferenceValue = _mark.GetComponent(_type);
                }
                serialiedScript.ApplyModifiedPropertiesWithoutUndo();
            }

            private const string formatScript = "/*********************************************************************************\n" +
                " *Author:         #SCAuthor#\n" +
                " *Version:        #SCVERSION#\n" +
                " *UnityVersion:   #SCUNITYVERSION#\n" +
                " *Date:           #SCDATE#\n" +
                " *Description:    #SCDescription#\n" +
                " *History:        #SCDATE#--\n" +
                "*********************************************************************************/\n" +
                "\n" +
                "namespace #SCNameSpace#\n" +
                "{\n" +
                "\tpublic class #SCSCRIPTNAME# : #BaseClass#\n" +
                "\t{\n" +
                "#SCField#\n" +
                "\t}\n" +
                "}";
        }

        [CanEditMultipleObjects, CustomEditor(typeof(ScriptMark))]
        class ScriptMarkEditor : Editor
        {
            public ScriptMark SM { get { return this.target as ScriptMark; } }
            public override void OnInspectorGUI()
            {
                //base.OnInspectorGUI();
                Component[] cps = SM.GetComponents<Component>();
                if (cps == null || cps.Length <= 0) return;
                List<string> names = new List<string>();
                cps.ToList().ForEach((c) =>
                {
                    if (c != null)
                        names.Add(c.GetType().FullName);
                    //else
                    //    DestroyImmediate(c);
                });
                names = names.Distinct().ToList();
                SM.isPublic = EditorGUILayout.Toggle("IsPublic", SM.isPublic);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Type");
                SM.index = EditorGUILayout.Popup(SM.index, names.ToArray());
                SM.fieldType = names[SM.index];
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Name");
                SM.fieldName = EditorGUILayout.TextField(SM.fieldName);
                SM.fieldName = string.IsNullOrEmpty(SM.fieldName) ? SM.name : SM.fieldName;
                if (!SM.fieldName.IsLegalFieldName()) SM.fieldName = SM.name;
                SM.fieldName = SM.fieldName.Replace(" ", "").Replace("(", "").Replace(")", "");

                EditorGUILayout.EndHorizontal();
                GUILayout.Label("Description");
                SM.description = EditorGUILayout.TextArea(SM.description, GUILayout.Height(40));
                serializedObject.Update();
            }
        }
    }
    static partial class EditorTools
    {
        public static class DragAndDropTool
        {
            public class Info
            {
                public bool dragging;
                public bool enterArera;
                public bool compelete;
                public Object[] objectReferences { get { return DragAndDrop.objectReferences; } }
                public string[] paths { get { return DragAndDrop.paths; } }
                public DragAndDropVisualMode visualMode { get { return DragAndDrop.visualMode; } }
                public int activeControlID { get { return DragAndDrop.activeControlID; } }
            }

            private static bool _dragging;
            private static bool _enterArera;
            private static bool _compelete;
            private static Info _info = new Info();
            public static Info Drag(Event eve, Rect Content, DragAndDropVisualMode mode = DragAndDropVisualMode.Generic)
            {
                switch (eve.type)
                {
                    case EventType.DragUpdated:
                        _dragging = true; _compelete = false;
                        _enterArera = Content.Contains(eve.mousePosition);
                        if (_enterArera)
                        {
                            DragAndDrop.visualMode = mode;
                            Event.current.Use();
                        }
                        break;
                    case EventType.DragPerform:
                        DragAndDrop.AcceptDrag();
                        _enterArera = Content.Contains(eve.mousePosition);
                        _compelete = true; _dragging = false;
                        Event.current.Use();

                        break;
                    case EventType.DragExited:
                        _dragging = false; _compelete = true;
                        _enterArera = Content.Contains(eve.mousePosition);
                        break;
                    default:
                        _dragging = false; _compelete = false;
                        _enterArera = Content.Contains(eve.mousePosition);
                        break;
                }
                _info.compelete = _compelete;
                _info.enterArera = _enterArera;
                _info.dragging = _dragging;
                return _info;
            }
        }
        public static class EditorLayoutTool
        {
            private static MethodInfo _miLoadWindowLayout;
            private static MethodInfo _miSaveWindowLayout;
            private static MethodInfo _miReloadWindowLayoutMenu;

            private static bool _available;

            static EditorLayoutTool()
            {
                Type tyWindowLayout = Type.GetType("UnityEditor.WindowLayout,UnityEditor");
                Type tyEditorUtility = Type.GetType("UnityEditor.EditorUtility,UnityEditor");
                Type tyInternalEditorUtility = Type.GetType("UnityEditorInternal.InternalEditorUtility,UnityEditor");

                if (tyWindowLayout != null && tyEditorUtility != null && tyInternalEditorUtility != null)
                {
                    _miLoadWindowLayout = tyWindowLayout.GetMethod("LoadWindowLayout", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(bool) }, null);
                    _miSaveWindowLayout = tyWindowLayout.GetMethod("SaveWindowLayout", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
                    _miReloadWindowLayoutMenu = tyInternalEditorUtility.GetMethod("ReloadWindowLayoutMenu", BindingFlags.Public | BindingFlags.Static);

                    if (_miLoadWindowLayout == null || _miSaveWindowLayout == null || _miReloadWindowLayoutMenu == null)
                        return;

                    _available = true;
                }
            }

            public static bool available
            {
                get { return _available; }
            }

            public static void SaveLayoutToAsset(string assetPath)
            {
                SaveLayout(Path.Combine(Directory.GetCurrentDirectory(), assetPath));
                //EditorUtility.SetDirty( AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object)));
                AssetDatabase.Refresh();
            }

            public static void LoadLayoutFromAsset(string assetPath)
            {
                if (_miLoadWindowLayout != null)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
                    _miLoadWindowLayout.Invoke(null, new object[] { path, false });
                }
            }

            private static void SaveLayout(string path)
            {
                if (_miSaveWindowLayout != null)
                    _miSaveWindowLayout.Invoke(null, new object[] { path });
            }
            //.wlt
        }
        public static class ReorderableListTool
        {
            public static ReorderableList Create(SerializedProperty property, float space = 10f)
            {
                return Create(property, true, true, true, true, null, space);
            }

            public static ReorderableList Create(SerializedProperty property, List<Column> cols, float space = 10f)
            {
                return Create(property, true, true, true, true, cols, space);
            }
            public static ReorderableList Create(SerializedProperty property, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton, float space = 10f)
            {
                return Create(property, draggable, displayHeader, displayAddButton, displayRemoveButton, null, space);
            }
            public static ReorderableList Create(SerializedProperty property, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton, List<Column> cols, float space = 10f)
            {
                var list = new ReorderableList(property.serializedObject, property, draggable, displayHeader, displayAddButton, displayRemoveButton);

                list.drawElementCallback = DrawElement(list, cols, space);
                list.drawHeaderCallback = DrawHeader(list, Calc(list, cols), space);

                return list;
            }
            private static ReorderableList.ElementCallbackDelegate DrawElement(ReorderableList list, List<Column> cols, float space)
            {
                return (rect, index, isActive, isFocused) =>
                {
                    var property = list.serializedProperty;
                    var columns = Calc(list, cols);
                    var layouts = CalcWidth(columns, rect, space);
                    rect.height = EditorGUIUtility.singleLineHeight;
                    for (var j = 0; j < columns.Count; j++)
                    {
                        var c = columns[j];
                        rect.width = layouts[j];

                        //Log.L(c.Width + "  " + layouts[j]);

                        var p = property.GetArrayElementAtIndex(index).FindPropertyRelative(c.PropertyName);
                        //if (p!=null)
                        {
                            EditorGUI.PropertyField(rect, p, GUIContent.none);
                            rect.x += rect.width + space;
                        }

                    }
                };
            }

            private static ReorderableList.HeaderCallbackDelegate DrawHeader(ReorderableList list, List<Column> cols, float space)
            {
                return (rect) =>
                {
                    var columns = cols;

                    if (list.draggable)
                    {
                        rect.width -= 15;
                        rect.x += 15;
                    }

                    var layouts = CalcWidth(columns, rect, space);
                    rect.height = EditorGUIUtility.singleLineHeight;
                    for (var j = 0; j < columns.Count; j++)
                    {
                        var c = columns[j];

                        rect.width = layouts[j];
                        EditorGUI.LabelField(rect, c.DisplayName);
                        rect.x += rect.width + space;
                    }
                };
            }

            private static List<Column> Calc(ReorderableList list, List<Column> cols)
            {
                var property = list.serializedProperty;
                if (cols == null) cols = new List<Column>();
                property = list.serializedProperty;
                if (property.isArray && property.arraySize > 0)
                {
                    SerializedProperty it = property.GetArrayElementAtIndex(0).Copy();
                    var prefix = it.propertyPath;
                    var index = 0;
                    if (it.Next(true))
                    {
                        do
                        {
                            if (it.propertyPath.StartsWith(prefix))
                            {
                                if (index >= cols.Count)
                                {
                                    var c = new Column();
                                    c.PropertyName = it.propertyPath.Substring(prefix.Length + 1);
                                    c.DisplayName = string.IsNullOrEmpty(c.DisplayName) ? c.PropertyName : c.DisplayName;
                                    cols.Add(c);
                                }
                                else
                                {
                                    var c = cols[index];
                                    c.PropertyName = it.propertyPath.Substring(prefix.Length + 1);
                                    c.DisplayName = string.IsNullOrEmpty(c.DisplayName) ? c.PropertyName : c.DisplayName;
                                }
                            }
                            else
                            {
                                break;
                            }
                            index += 1;
                        }
                        while (it.Next(false));
                    }
                }
                return cols;
            }

            private static List<float> CalcWidth(List<Column> columns, Rect rect, float space)
            {
                var autoWidth = rect.width;
                var autoCount = 0;
                foreach (var column in columns)
                {
                    if (column.Width != 0)
                    {
                        autoWidth -= column.Width;
                    }
                    else
                    {
                        autoCount += 1;
                    }
                }

                autoWidth -= (columns.Count - 1) * space;
                autoWidth /= autoCount;

                var widths = new List<float>(columns.Count);
                foreach (var column in columns)
                {
                    if (column.Width != 0)
                    {
                        widths.Add(column.Width);
                    }
                    else
                    {
                        widths.Add(autoWidth);
                        //column.Width = autoWidth;
                    }
                }

                return widths;
            }


            public static bool DrawWithFold(ReorderableList list, string label = null)
            {
                var property = list.serializedProperty;
                property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, label != null ? label : property.displayName);
                if (property.isExpanded)
                {
                    list.DoLayoutList();
                }
                return property.isExpanded;
            }
            public static void Draw(ReorderableList list)
            {
                list.DoLayoutList();
            }

            public class Column
            {
                public string DisplayName;
                internal string PropertyName;
                public float Width=10;
            }

        }
        public class ScriptableObjectTool
        {
            public static T Create<T>(string savePath) where T : ScriptableObject
            {
                ScriptableObject sto = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(sto, savePath);
                EditorUtility.SetDirty(sto);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(savePath);
                AssetDatabase.Refresh();
                return AssetDatabase.LoadAssetAtPath<T>(savePath);
            }
            public static T Load<T>(string path) where T : ScriptableObject
            {
                return AssetDatabase.LoadAssetAtPath<T>(path);
            }
            public static void Update<T>(T t) where T : ScriptableObject
            {
                EditorEnv.delayCall += delegate ()
                {
                    EditorUtility.SetDirty(t);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                };
            }
            public static void Update<T>(T sto, Object[] subAssets) where T : ScriptableObject
            {
                EditorEnv.delayCall += delegate () {
                    string path = AssetDatabase.GetAssetPath(sto);
                    Object[] objs = AssetDatabase.LoadAllAssetsAtPath(path);
                    if (objs != null)
                    {
                        List<Type> typeList = new List<Type>();
                        for (int i = 0; i < subAssets.Length; i++)
                        {
                            typeList.Add(subAssets[i].GetType());
                        }
                        typeList = typeList.Distinct().ToList();
#if UNITY_2017_4_OR_NEWER
                        for (int i = 0; i < objs.Length; i++)
                        {
                            if (objs[i] != null)
                            {
                                if (AssetDatabase.IsMainAsset(objs[i])) continue;

                                Type objType = objs[i].GetType();
                                if (typeList.Contains(objType) && !subAssets.ToList().Contains(objs[i]))
                                    AssetDatabase.RemoveObjectFromAsset(objs[i]);
                            }

                        }
#else
                    for (int i = 0; i < objs.Length; i++)
                    {
                        if (objs[i] != null)
                        {
                            if (AssetDatabase.IsMainAsset(objs[i])) continue;
                           
                            Type objType = objs[i].GetType();
                            if (typeList.Contains(objType) && !subAssets.ToList().Contains(objs[i]))
                                Object.DestroyImmediate(objs[i],true);
                        }
                    }
#endif
                    }
                    AssetDatabase.ImportAsset(path);
                    if (subAssets == null || subAssets.Length == 0) return;
                    for (int i = subAssets.Length - 1; i >= 0; i--)
                    {
                        if (subAssets[i] != null && !AssetDatabase.Contains(subAssets[i]))
                        {
                            Object asset = subAssets[i];
                            AssetDatabase.AddObjectToAsset(asset, sto);
                            asset.hideFlags = HideFlags.HideInHierarchy;
                        }
                    }
                    EditorApplication.RepaintProjectWindow();
                    EditorUtility.SetDirty(sto);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                };
            }
            public static void Delete<T>(string path) where T : ScriptableObject
            {
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.Refresh();
            }
            public static void Delete<T>(T sto) where T : ScriptableObject
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(sto));
                AssetDatabase.Refresh();
            }
        }
        public static class Prefs
        {
            static string GetKey<T>(string key)
            {
                return string.Format("{0}/{1}", typeof(T).FullName, key);
            }
            public static void DeleteAll()
            {
                EditorPrefs.DeleteAll();
            }
            public static void DeleteKey<T>(string key)
            {
                EditorPrefs.DeleteKey(GetKey<T>(key));
            }
            public static bool GetBool<T>(string key, bool defaultValue)
            {
                return EditorPrefs.GetBool(GetKey<T>(key), defaultValue);
            }
            public static bool GetBool<T>(string key)
            {
                return EditorPrefs.GetBool(GetKey<T>(key));
            }
            public static float GetFloat<T>(string key, float defaultValue)
            {
                return EditorPrefs.GetFloat(GetKey<T>(key), defaultValue);
            }
            public static float GetFloat<T>(string key)
            {
                return EditorPrefs.GetFloat(GetKey<T>(key));
            }
            public static int GetInt<T>(string key, int defaultValue)
            {
                return EditorPrefs.GetInt(GetKey<T>(key), defaultValue);
            }
            public static int GetInt<T>(string key)
            {
                return EditorPrefs.GetInt(GetKey<T>(key));
            }
            public static string GetString<T>(string key, string defaultValue)
            {
                return EditorPrefs.GetString(GetKey<T>(key), defaultValue);
            }
            public static string GetString<T>(string key)
            {
                return EditorPrefs.GetString(GetKey<T>(key));
            }
            public static bool HasKey<T>(string key)
            {
                return EditorPrefs.HasKey(GetKey<T>(key));
            }
            public static void SetBool<T>(string key, bool value)
            {
                EditorPrefs.SetBool(GetKey<T>(key), value);
            }
            public static void SetFloat<T>(string key, float value)
            {
                EditorPrefs.SetFloat(GetKey<T>(key), value);
            }
            public static void SetInt<T>(string key, int value)
            {
                EditorPrefs.SetInt(GetKey<T>(key), value);
            }
            public static void SetString<T>(string key, string value)
            {
                EditorPrefs.SetString(GetKey<T>(key), value);
            }

            public static void SetObject<T, V>(string key, V value)
            {
                SetString<T>(key, JsonUtility.ToJson(value));
            }
            public static V GetObject<T, V>(string key)
            {
                if (HasKey<T>(key))
                {
                    var str = GetString<T>(key);
                    return JsonUtility.FromJson<V>(str);
                }
                return default(V);
            }
        }


        [OnEnvironmentInit]
        public static class ProjectConfig
        {
            public static string NameSpace { get { return Info.NameSpace; } }
            public static string UserName { get { return Info.UserName; } }
            public static string Version { get { return Info.Version; } }
            public static string Description { get { return Info.Description; } }
            public static bool enable { get { return Info.enable; } }
            public static bool enable_L { get { return Info.enable_L; } }
            public static bool enable_W { get { return Info.enable_W; } }
            public static bool enable_E { get { return Info.enable_E; } }
            public const string configName = "ProjectConfig";
            [Serializable]
            public class ProjectConfigInfo
            {
                public bool enable = true;
                public bool enable_L = true;
                public bool enable_W = true;
                public bool enable_E = true;



                public string NameSpace;
                public string UserName;
                public string Version;
                public string Description;
                public ProjectConfigInfo()
                {
                    UserName = "OnClick";
                    NameSpace = "IFramework_Demo";
                    Version = "0.0.1";
                    Description = "Description";
                }
            }


            private static ProjectConfigInfo __info;
            public static ProjectConfigInfo Info
            {
                get
                {
                    if (__info == null)
                    {
                        __info = EditorTools.Prefs.GetObject<ProjectConfigInfo, ProjectConfigInfo>(key);
                        if (__info == null)
                        {
                            EditorTools.Prefs.SetObject<ProjectConfigInfo, ProjectConfigInfo>(key, new ProjectConfigInfo());
                            __info = EditorTools.Prefs.GetObject<ProjectConfigInfo, ProjectConfigInfo>(key);
                        }
                    }
                    return __info;
                }
            }
            private const string key = "ProjectConfig";
            public static void Save()
            {
                EditorTools.Prefs.SetObject<ProjectConfigInfo, ProjectConfigInfo>(key, Info);
            }



            static ProjectConfig()
            {
                Log.loger = new UnityLoger();
                Log.enable_L = ProjectConfig.enable_L;
                Log.enable_W = ProjectConfig.enable_W;
                Log.enable_E = ProjectConfig.enable_E;
                Log.enable = ProjectConfig.enable;
            }

            class FormatProjectScript
            {
                const string key = "FormatUserScript";

                private class FormatUserScriptProcessor : UnityEditor.AssetModificationProcessor
                {
                    public static void OnWillCreateAsset(string metaPath)
                    {
                        if (!EditorPrefs.GetBool(key, false)) return;

                        string filePath = metaPath.Replace(".meta", "");
                        if (!filePath.EndsWith(".cs")) return;
                        string realPath = filePath.ToAbsPath();
                        string txt = File.ReadAllText(realPath);

                        if (!txt.Contains("#User#")) return;
                        //这里实现自定义的一些规则
                        txt = txt.Replace("#User#", ProjectConfig.UserName)
                                 .Replace("#UserSCRIPTNAME#", Path.GetFileNameWithoutExtension(filePath))
                                 .Replace("#UserNameSpace#", ProjectConfig.NameSpace)
                                 .Replace("#UserVERSION#", ProjectConfig.Version)
                                .Replace("#UserDescription#", ProjectConfig.Description)

                                 .Replace("#UserUNITYVERSION#", Application.unityVersion)
                                 .Replace("#UserDATE#", DateTime.Now.ToString("yyyy-MM-dd")).ToUnixLineEndings();

                        File.WriteAllText(realPath, txt, Encoding.UTF8);
                        EditorPrefs.SetBool(key, false);
                    }
                }
                private class FormatUserCSScript
                {

                    private static string newScriptName = "newScript.cs";
                    private static string originScriptPathWithNameSpace = EditorEnv.formatScriptsPath.CombinePath("UserCSharpScript.txt");

                    [MenuItem("Assets/Create/IFramework/FormatProjectCSharpScript", priority = -1000)]
                    public static void Create()
                    {
                        CreateOriginIfNull();
                        CopyAsset.Copy(newScriptName, originScriptPathWithNameSpace);
                        EditorPrefs.SetBool(key, true);
                    }
                    private static void CreateOriginIfNull()
                    {
                        if (File.Exists(originScriptPathWithNameSpace)) return;
                        using (FileStream fs = new FileStream(originScriptPathWithNameSpace, FileMode.Create, FileAccess.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(fs))
                            {
                                fs.Lock(0, fs.Length);
                                sw.WriteLine("/*********************************************************************************");
                                sw.WriteLine(" *Author:         #User#");
                                sw.WriteLine(" *Version:        #UserVERSION#");
                                sw.WriteLine(" *UnityVersion:   #UserUNITYVERSION#");
                                sw.WriteLine(" *Date:           #UserDATE#");
                                sw.WriteLine(" *Description:    #UserDescription#");
                                sw.WriteLine(" *History:        #UserDATE#--");
                                sw.WriteLine("*********************************************************************************/");
                                sw.WriteLine("using System;");
                                sw.WriteLine("using System.Collections;");
                                sw.WriteLine("using System.Collections.Generic;");
                                sw.WriteLine("using IFramework;");

                                sw.WriteLine("");
                                sw.WriteLine("namespace #UserNameSpace#");
                                sw.WriteLine("{");
                                sw.WriteLine("\tpublic class #UserSCRIPTNAME#");
                                sw.WriteLine("\t{");
                                sw.WriteLine("\t");
                                sw.WriteLine("\t}");
                                sw.WriteLine("}");
                                fs.Unlock(0, fs.Length);
                                sw.Flush();
                                fs.Flush();
                            }
                        }
                        AssetDatabase.Refresh();
                    }
                }
                private class FormatUserMonoScript
                {
                    private static string newScriptName = "newScript.cs";
                    private static string originScriptPathWithNameSpace = EditorEnv.formatScriptsPath.CombinePath("UserMonoScript.txt");

                    [MenuItem("Assets/Create/IFramework/FormatProjectMonoScript", priority = -1000)]
                    public static void Create()
                    {
                        CreateOriginIfNull();
                        CopyAsset.Copy(newScriptName, originScriptPathWithNameSpace);
                        EditorPrefs.SetBool(key, true);
                    }

                    private static void CreateOriginIfNull()
                    {
                        if (File.Exists(originScriptPathWithNameSpace)) return;
                        using (FileStream fs = new FileStream(originScriptPathWithNameSpace, FileMode.Create, FileAccess.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(fs))
                            {
                                fs.Lock(0, fs.Length);
                                sw.WriteLine("/*********************************************************************************");
                                sw.WriteLine(" *Author:         #User#");
                                sw.WriteLine(" *Version:        #UserVERSION#");
                                sw.WriteLine(" *UnityVersion:   #UserUNITYVERSION#");
                                sw.WriteLine(" *Date:           #UserDATE#");
                                sw.WriteLine(" *Description:    #UserDescription#");
                                sw.WriteLine(" *History:        #UserDATE#--");
                                sw.WriteLine("*********************************************************************************/");
                                sw.WriteLine("using System;");
                                sw.WriteLine("using System.Collections;");
                                sw.WriteLine("using System.Collections.Generic;");
                                sw.WriteLine("using UnityEngine;");
                                sw.WriteLine("using IFramework;");

                                sw.WriteLine("");
                                sw.WriteLine("namespace #UserNameSpace#");
                                sw.WriteLine("{");
                                sw.WriteLine("\tpublic class #UserSCRIPTNAME# : MonoBehaviour");
                                sw.WriteLine("\t{");
                                sw.WriteLine("\t");
                                sw.WriteLine("\t}");
                                sw.WriteLine("}");
                                fs.Unlock(0, fs.Length);
                                sw.Flush();
                                fs.Flush();
                            }
                        }
                        AssetDatabase.Refresh();
                    }
                }
            }

        }
        [EditorWindowCache("ProjectConfig")]
        class ProjectConfigWindow : EditorWindow
        {

            private void OnGUI()
            {

                var Info = ProjectConfig.Info;
                GUILayout.Space(10);
                Info.UserName = EditorGUILayout.TextField(new GUIContent("UserName", "Project Author's Name"), Info.UserName);
                Info.Version = EditorGUILayout.TextField(new GUIContent("Version", "Version of Project"), Info.Version);

                EditorGUILayout.LabelField(new GUIContent("NameSpace", "Script's Namespace"));
                Info.NameSpace = EditorGUILayout.TextArea(Info.NameSpace);
                GUILayout.Label("Description of Scripts");
                Info.Description = EditorGUILayout.TextArea(Info.Description, GUILayout.Height(100));
                GUILayout.Space(10);

                GUILayout.Label("LogSetting in Editor mode", GUIStyles.Get("IN Title"));
                Info.enable = EditorGUILayout.Toggle("Enable", Info.enable);
                GUI.enabled = Info.enable;
                Info.enable_L = EditorGUILayout.Toggle("Log Enable", Info.enable_L);
                Info.enable_W = EditorGUILayout.Toggle("Warning Enable", Info.enable_W);
                Info.enable_E = EditorGUILayout.Toggle("Error Enable", Info.enable_E);

                GUI.enabled = true;

                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Save"))
                    {
                        ProjectConfig.Save();
                    }

                    GUILayout.EndHorizontal();
                }

            }


        }
    }
    static partial class EditorTools
    {
        private const string hierarchyOverridePath = "IFramework/Tool/HierarchyExtension";
        private const string copyAssetPathPath = "IFramework/Tools/Copy Path";
        private const string quitPath = "IFramework/Tools/Editor Quit";

        private const string openDocPathPath = "IFramework/Folder/Open DocPath";
        private const string openStreamingPath = "IFramework/Folder/Open StreamingPath";
        private const string openDataPath = "IFramework/Folder/Open DataPath";
        private const string openConsoleLogPath = "IFramework/Folder/Open ConsoleLogPath";
        private const string openTemporaryCachePath = "IFramework/Folder/Open TemporaryCachePath";
        private const string findScriptPath = "CONTEXT/MonoBehaviour/IFramework.FindScript";



        [MenuItem(findScriptPath)]
        static void FindScript(MenuCommand command)
        {
            Selection.activeObject = MonoScript.FromMonoBehaviour(command.context as MonoBehaviour);
        }

        [MenuItem(copyAssetPathPath)]
        public static void CopyAssetPath()
        {
            if (EditorApplication.isCompiling)
            {
                return;
            }
            string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            GUIUtility.systemCopyBuffer = path;
        }
        [MenuItem(quitPath)]
        public static void Quit()
        {
            //  Environment.Exit(0);
            EditorApplication.Exit(0);
        }


        [MenuItem(openDocPathPath)]
        public static void OpenDoc()
        {
            OpenFloder(Application.persistentDataPath);
        }
        [MenuItem(openStreamingPath)]
        public static void OpenStreaming()
        {
            OpenFloder(Application.streamingAssetsPath);
        }
        [MenuItem(openDataPath)]
        public static void OpenDataPath()
        {
            OpenFloder(Application.dataPath);
        }
        [MenuItem(openTemporaryCachePath)]
        public static void OpenTemporary()
        {
            OpenFloder(Application.temporaryCachePath);
        }
#if UNITY_2018_1_OR_NEWER
        [MenuItem(openConsoleLogPath)]
        public static void OpenConsoleLog()
        {
            OpenFloder(Application.consoleLogPath);
        }
#endif





        public static void OpenFloder(string folder)
        {
            EditorUtility.OpenWithDefaultApp(folder);
        }
        public static EditorBuildSettingsScene[] ScenesInBuildSetting()
        {
            return EditorBuildSettings.scenes;
        }
        public static string[] GetScenesInBuildSetting()
        {
            List<string> levels = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
            {
                if (EditorBuildSettings.scenes[i].enabled)
                    levels.Add(EditorBuildSettings.scenes[i].path);
            }

            return levels.ToArray();
        }

        public static string GetBuildTargetName(BuildTarget target)
        {
            string name = PlayerSettings.productName + "_" + PlayerSettings.bundleVersion;
            if (target == BuildTarget.Android)
            {
                return name + PlayerSettings.Android.bundleVersionCode + ".apk";
            }
            if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
            {
                return name + PlayerSettings.Android.bundleVersionCode + ".exe";
            }
            if
#if UNITY_2017_3_OR_NEWER
            (target == BuildTarget.StandaloneOSX)
#else
            (target == BuildTarget.StandaloneOSXIntel || target == BuildTarget.StandaloneOSXIntel64 || target == BuildTarget.StandaloneOSXUniversal)
#endif
            {
                return name + ".Game";
            }
            if (target == BuildTarget.iOS)
            {
                return "iOS";
            }
            return null;
            //if (target == BuildTarget.WebGL)
            //{
            //    return "/web";
            //}

        }
        public static GameObject CreatePrefab(GameObject source, string savePath)
        {
            GameObject goClone = GameObject.Instantiate(source);
            GameObject prefab;
#if UNITY_2018_1_OR_NEWER
            prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(goClone, savePath, InteractionMode.AutomatedAction);

#else
            prefab = PrefabUtility.CreatePrefab(savePath, goClone);
            prefab = PrefabUtility.ReplacePrefab(goClone, prefab, ReplacePrefabOptions.ConnectToPrefab);
#endif
            AssetDatabase.ImportAsset(savePath);
            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            GameObject.DestroyImmediate(goClone);
            return prefab;
        }
    }
    static partial class EditorTools
    {
        public static void DrawOutLine(this Rect rect, float width, Color color)
        {
            Handles.color = color;

            Handles.DrawAAPolyLine(2, new Vector3(rect.x,
                                         rect.y,
                                         0),
                          new Vector3(rect.x,
                                         rect.yMax,
                                         0),
                          new Vector3(rect.xMax,
                                         rect.yMax,
                                         0),
                          new Vector3(rect.xMax,
                                         rect.y,
                                         0),
                          new Vector3(rect.x,
                                         rect.y,
                                         0)
                            );

            Handles.color = Color.white;
        }

    }
}
