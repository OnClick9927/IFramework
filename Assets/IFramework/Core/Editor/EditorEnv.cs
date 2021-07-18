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
using UnityEngine;
using IFramework.Modules.Message;

namespace IFramework
{
    public class EditorEnv
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
            protected override List<string> directorys
            {
                get
                {
                    return new List<string>()
                    {
                        "Assets/Project",
                        "Assets/Project",
                        "Assets/Project/Shaders",
                        "Assets/Project/Textures",
                        "Assets/Project/Images",
                        "Assets/Project/Scripts",
                        "Assets/Project/Scenes",
                        "Assets/Project/Prefabs",
                        "Assets/Project/Resources",
                        "Assets/StreamingAssets",
                    };
                }
            }

            protected override List<string> files { get { return null; } }
        }
        private struct ScriptEnvCheckCommand : ICommand
        {
            public void Excute()
            {
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
            }
        }
        private struct FileInitializeCommand : ICommand
        {
            public void Excute()
            {
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
        public static string memoryPath
        {
            get
            {
                string path = Path.Combine(Application.persistentDataPath + "/../", frameworkName + "Memory");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    File.SetAttributes(path, FileAttributes.Hidden);
                }
                return path;
            }
        }
        public static string formatScriptsPath
        {
            get
            {
                string path = Path.Combine(memoryPath, "FormatScripts");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }
        public static event EditorApplication.CallbackFunction delayCall { add { EditorApplication.delayCall += value; } remove { EditorApplication.delayCall -= value; } }

        [InitializeOnLoadMethod]
        static void EditorEnvInit()
        {
            Debug.Log("IFramework: EditorEnv Init?   " + frameworkPath);
            Framework.CreateEnv(envType).InitWithAttribute();
            CompilationPipeline.assemblyCompilationStarted += Dispose;
            EditorApplication.update += Framework.env0.Update;
            env.modules.Message.fitSubType = true;
            env.modules.Message.processesPerFrame = 20;
            env.modules.Message.Subscribe<ICommand>(Listen);
            SendCommand(new ScriptEnvCheckCommand());
            SendCommand(new FileInitializeCommand());
        }
        private static string GetFilePath([CallerFilePath] string path = "")
        {
            return path;
        }
        private static void Dispose(string obj)
        {
            Framework.env0.Dispose();
            UnityEngine.Debug.Log("IFramework: EditorEnv Dispose");
        }
        private static void Listen(IMessage message)
        {
            if (message.args.Is<ICommand>())
            {
                message.args.As<ICommand>().Excute();
            }
        }


        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="command"></param>
        public static void SendCommand(ICommand command)
        {
            env.modules.Message.Publish<ICommand>(command);
        }
        /// <summary>
        /// 注册消息
        /// </summary>
        /// <typeparam name="type"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        public static void SubscribeRootMessage<type>(IMessageListener listener)
        {
            env.modules.Message.Subscribe<type>(listener);
        }
        /// <summary>
        /// 取消监听
        /// </summary>
        /// <typeparam name="type"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        public static void UnSubscribeRootMessage<type>(IMessageListener listener)
        {
            env.modules.Message.UnSubscribe<type>(listener);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static IMessage PublishRootMessage<Type>(IEventArgs args, MessageUrgencyType priority = MessageUrgencyType.Common)
        {
            return env.modules.Message.Publish<Type>(args, priority);
        }
    }

}
