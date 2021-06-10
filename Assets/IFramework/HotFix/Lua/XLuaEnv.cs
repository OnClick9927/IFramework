/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using XLua;
using UnityEngine;
using System.Collections.Generic;
using System;
using IFramework.Singleton;
using IFramework.AAEX;

namespace IFramework.Hotfix.Lua
{
    public interface IXLuaLoader
    {
        byte[] load(ref string filepath);
    }

    public class AddressableLoader : IXLuaLoader
    {
        public static string projectScriptsPath
        {
            get { return Application.dataPath.CombinePath("Project/Lua").ToRegularPath(); }
        }
        public byte[] load(ref string filepath)
        {
            filepath = filepath.Replace(".", "/");
            var textAsset = Resources.Load<TextAsset>(filepath);
            if (textAsset != null)
                return textAsset.bytes;
            filepath = projectScriptsPath.CombinePath(filepath+ ".lua").ToAssetsPath();

            var handle = Assets.LoadPreparedAsset<TextAsset>(filepath);
            textAsset = handle;
            if (textAsset == null) return null;
            var bytes = textAsset.bytes;
            Assets.Release(handle);
            return bytes;
        }
    }
    public class XLuaEnv :SingletonPropertyClass<XLuaEnv>
	{
        private XLuaEnv() { }
        private LuaEnv _luaenv;
        private List<LuaTable> _tables;
        private static float _lastGCTime;

        public static LuaTable gtable { get { return instance._luaenv.Global; } }
        public static float gcInterval=1f;
        public static bool disposed { get; private set; }
        public static Action onDispose;

        public static LuaTable NewTable()
        {
            LuaTable table = instance._luaenv.NewTable();
            instance._tables.Add(table);
            return table;
        }
        public static void AddLoader(IXLuaLoader loader)
        {
            instance. _luaenv.AddLoader(loader.load);
        }

        public static LuaFunction LoadString(string chunk, string chunkName = "chunk", LuaTable env = null)
        {
            return instance._luaenv.LoadString(chunk, chunkName, env);
        }
        public static T LoadString<T>(string chunk, string chunkName = "chunk", LuaTable env = null)
        {
            return instance._luaenv.LoadString<T>(chunk, chunkName, env);
        }
        public static T LoadString<T>(byte[] chunk, string chunkName = "chunk", LuaTable env = null)
        {
            return instance._luaenv.LoadString<T>(chunk, chunkName, env);
        }



        public static object[] DoString(byte[] chunk, string chunkName = "chunk", LuaTable env = null)
        {
            return instance._luaenv.DoString(chunk, chunkName, env);
        }
        public static object[] DoString(string chunk, string chunkName = "chunk", LuaTable env = null)
        {
            return instance._luaenv.DoString(chunk, chunkName, env);
        }



        public static LuaTable GetTable(TextAsset luaScript, string chunkName = "chunk")
        {
            //// 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
            LuaTable meta = XLuaEnv.NewTable();
            meta.Set("__index", XLuaEnv.gtable);
            LuaTable scriptEnv = XLuaEnv.NewTable();
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();
            DoString(luaScript.text, chunkName, scriptEnv);
            return scriptEnv;
        }
        

        public static void FullGc()
        {
            instance._luaenv.FullGc();
        }

        protected override void OnSingletonInit()
        {
            disposed = false;
            _luaenv = new LuaEnv();
            _tables = new List<LuaTable>();
            Framework.BindEnvUpdate(Update, EnvironmentType.Ev1);
            Framework.BindEnvDispose(()=> { SingletonCollection.Dispose<XLuaEnv>(); }, EnvironmentType.Ev1);
        }
        protected override void OnDispose()
        {

            if (onDispose != null) onDispose();

            _tables.ForEach((table) =>
            {
                table.Dispose();
            });
            // DoString(@"
            //         local util = require 'xlua.util'
            //        -- util.print_func_ref_by_csharp()
            // ");
            _luaenv.Dispose();
            _luaenv = null;
            disposed = true;
        }

        private void Update()
        {
            if (_luaenv == null) return;
            if (Time.time - _lastGCTime > gcInterval)
            {
                _luaenv.Tick();
                _lastGCTime = Time.time;
            }
        }

    }
}
