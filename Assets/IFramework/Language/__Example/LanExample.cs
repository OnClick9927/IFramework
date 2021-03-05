/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-03
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework;
using IFramework.Language;
using System.Collections.Generic;
using UnityEngine;
namespace IFramework_Demo
{
    public class LanExample : Game
    {
        [LanguageKey]
        public string key = "77";
        IDelegateLanguageObserver observer;
        ILanguageModule mou;
        public List<LanPair> Load()
        {
            return new List<LanPair>()
                {
                    new LanPair()
                    {
                        lan= SystemLanguage.Chinese,
                        value="哈哈",
                        key="77"
                    },
                    new LanPair()
                    {
                        lan= SystemLanguage.English,
                        value="haha",
                        key="77"
                    }
                };
        }

        public override void Init()
        {
            mou = modules.CreateModule<LanguageModule>();

        }

        public override void Startup()
        {
            mou.Load(Load());
            observer = mou.CreateDelegateObserver(key, SystemLanguage.English)
                .Listen((val) => { Log.E(val); });
        }
        int index;
        private void Update()
        {
            index = ++index % 40;
            mou.language = (SystemLanguage)index;
        }

    }
}
