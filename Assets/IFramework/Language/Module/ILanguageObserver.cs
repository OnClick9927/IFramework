/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-09-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework.Language
{
    public interface ILanguageObserver
    {
        ILanguageModule module { get; }
        string languageKey { get; }
        SystemLanguage fallbackLanguage { get; set; }
        void Listen(SystemLanguage languageType, string value);
        void SetValue(ILanguageModule module, string key, SystemLanguage fallback, bool autoStart = true);
    }
}
