/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.115
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Language;
using IFramework.Hotfix.Lua;
using IFramework.UI;
using IFramework.AAEX;

namespace IFramework.Hotfix
{
    public class LuaGame : AAGame
    {
        public class UnityModules
        {
            public IUIModule UI { get { return Launcher.modules.GetModule<UIModule>(""); } }
            public ILanguageModule Lan { get { return Launcher.modules.GetModule<LanguageModule>(""); } }
        }

        public UnityModules unityModules = new UnityModules();
        public override void Init()
        {
           
        }
        public override void Startup()
        {
            onUpdateCompelete += Assets.PrepareDefault;
            onPrepareCompelete += () => {
                if (Assets.currentPrepare==Assets.defaultKey)
                {
                    StartLua();
                }
            };
            Assets.UpdateAssets();
        }
        private void StartLua()
        {
            XLuaEnv.AddLoader(new AddressableLoader());
            new XluaMain();
        }
    }
}
