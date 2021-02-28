/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Modules;
using UnityEngine;

namespace IFramework
{
    [RequireComponent(typeof(Launcher))]
	public abstract class Game:MonoBehaviour
	{
        public EnvironmentType envType { get { return Launcher.envType; } }
        public FrameworkEnvironment env { get { return Launcher.env; } }
        public IFrameworkModules modules { get { return Launcher.modules; } }

       

        public abstract void Init();
        public abstract void Startup();
	}
}
