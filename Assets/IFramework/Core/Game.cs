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
	public abstract class Game:MonoBehaviour
	{
        public EnvironmentType envType = EnvironmentType.Ev1;
        public IEnvironment env { get { return Launcher.env; } }
        public IFrameworkModules modules { get { return Launcher.modules; } }
        private void Awake()
        {
            Launcher.envType = envType;
            Launcher.Instance.game = this;
            transform.parent = Launcher.Instance.transform;
        }
        public abstract void Init();
        public abstract void Startup();
	}
}
