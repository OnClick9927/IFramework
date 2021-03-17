/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-22
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using System;
using UnityEditor.Compilation;
using System.IO;
using System.Collections.Generic;
using IFramework.Modules;
using System.Runtime.CompilerServices;

namespace IFramework
{
    class EditorEnv
    {
        public interface IFileInitializer
        {
            void Create();
        }
        public abstract class FileInitializer : IFileInitializer
        {
            protected abstract List<string> directorys { get; }
            protected abstract List<string> files { get; }

            protected virtual bool CreateFile(int index, string path) { return true; }
            protected static bool ExistFile(string path)
            {
                return File.Exists(path);
            }
            protected static bool ExistDirectory(string path)
            {
                return Directory.Exists(path);
            }
            public virtual void Create()
            {
                if (directorys != null)
                {
                    directorys.ForEach((path) =>
                    {
                        if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                    });
                }
                if (files != null)
                {
                    files.ForEach((index, path) =>
                    {
                        bool bo = CreateFile(index, path);
                        if (!bo)
                        {
                            if (!string.IsNullOrEmpty(path) && !File.Exists(path))
                            {
                                File.Create(path);
                            }
                        }
                    });
                }
            }
        }
        class ProjectFloderInitializer : FileInitializer
        {
            protected override List<string> directorys { get {
                    return new List<string>()
                    {
                        "Assets/Project",
                        "Assets/Project/Sources",
                        "Assets/Project/Sources/Shaders",
                        "Assets/Project/Sources/Textures",
                        "Assets/Project/Sources/Images",
                        "Assets/Project/Scripts",
                        "Assets/Project/Sences",
                        "Assets/Project/Resources",
                        "Assets/StreamingAssets",
                    };
                } }

            protected override List<string> files { get { return null; } }
        }

        private const string _relativeCorePath = "Core";
        private static string _fpath;


        public const EnvironmentType envType = EnvironmentType.Ev0;
        public static IEnvironment env { get { return Framework.env0; } }
        public static IFrameworkModules moudules { get { return env.modules; } }



        public static string frameworkName { get { return Framework.FrameworkName; } }
        public static string author { get { return Framework.Author; } }
        public static string version { get { return Framework.Version; } }
        public static string description { get { return Framework.Description; } }
        public static string frameworkPath
        {
            get
            {
                if (string.IsNullOrEmpty(_fpath))
                {
                    var path = GetFilePath().ToAssetsPath();
                    int index = path.IndexOf(_relativeCorePath);
                    path = path.Substring(0, index);

                    UnityEngine.Debug.Log(path);
                    _fpath = path;
                }
                return _fpath;
            }
        }
        private static string GetFilePath([CallerFilePath] string path = "")
        {
            return path;
        }
        public static string corePath { get { return frameworkPath.CombinePath(_relativeCorePath); } }
        public static string memoryPath
        {
            get
            {
                string path = "Assets/../" + frameworkName+"EditorMemory";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }
        public static string formatScriptsPath
        {
            get
            {
                string path = Path.Combine(memoryPath,"Scripts");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }



        public static event EditorApplication.CallbackFunction update { add { EditorApplication.update += value; } remove { EditorApplication.update -= value; } }
        public static event EditorApplication.CallbackFunction delayCall { add { EditorApplication.delayCall += value; } remove { EditorApplication.delayCall -= value; } }
        public static event EditorApplication.HierarchyWindowItemCallback hierarchyWindowItemOnGUI { add { EditorApplication.hierarchyWindowItemOnGUI += value; } remove { EditorApplication.hierarchyWindowItemOnGUI -= value; } }
        public static event Action<string> assemblyCompilationStarted { add { CompilationPipeline.assemblyCompilationStarted += value; } remove { CompilationPipeline.assemblyCompilationStarted -= value; } }

        [InitializeOnLoadMethod]
        static void EditorEnvInit()
        {
            UnityEngine.Debug.Log("IFramework: EditorEnv Init?   " + frameworkPath);
            Framework.CreateEnv(envType).InitWithAttribute();
            assemblyCompilationStarted += (str) => {
                Framework.env0.Dispose();
                UnityEngine.Debug.Log("IFramework: EditorEnv Dispose"); 
            };

            update += Framework.env0.Update;
#if UNITY_2018_1_OR_NEWER
            PlayerSettings.allowUnsafeCode = true;
#else
          string  path = UnityEngine.Application.dataPath.CombinePath("mcs.rsp");
            string content = "-unsafe";
            if (File.Exists(path) && path.ReadText(System.Text.Encoding.Default) == content) return;
                path.WriteText(content, System.Text.Encoding.Default); 
            AssetDatabase.Refresh();
            EditorTools.Quit();
#endif
            typeof(IFileInitializer).GetSubTypesInAssemblys().ForEach((type) =>
            {
                if (!type.IsAbstract)
                {
                    (Activator.CreateInstance(type) as IFileInitializer).Create();
                }
            });
            delayCall += () => { AssetDatabase.Refresh(); };
              

        }
    }

}
