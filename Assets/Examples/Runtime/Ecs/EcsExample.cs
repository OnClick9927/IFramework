/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2020-02-10
 *Description:    Description
 *History:        2020-02-10--
*********************************************************************************/
using IFramework;
using IFramework.Modules;
using IFramework.Modules.ECS;
using UnityEngine;

namespace IFramework_Demo
{

    public class EcsExample:Game
	{
        private class SimpleEntity : Entity { }
        private class PlayerComponent : IComponent { }
        private class PCComponent : IComponent { }

        private class RotaComponet:IComponent
        {
            public UnityEngine.GameObject go;
        }
        private struct SpeedComponent: IComponent
        {
            public float speed;
        }
        private class PlayerSystem : ExcuteSystem<SimpleEntity>
        {
            public PlayerSystem(IECSModule module) : base(module) { }
            protected override bool Fitter(SimpleEntity entity)
            {
                return entity.ContainsComponent<PlayerComponent>() && entity.ContainsComponent<RotaComponet>();
            }
            protected override void Excute(SimpleEntity entity)
            {
                RotaComponet rc = entity.GetComponent<RotaComponet>();
                rc.go.transform.Rotate(UnityEngine.Vector3.up, 1);
            }
        }
        private class PCSystem : ExcuteSystem<SimpleEntity>
        {
            public PCSystem(IECSModule module) : base(module) { }
            protected override bool Fitter(SimpleEntity entity)
            {
                return entity.ContainsComponent<PCComponent>() && entity.ContainsComponent<RotaComponet>()&& entity.ContainsComponent<SpeedComponent>();
            }
            protected override void Excute(SimpleEntity entity)
            {
                SpeedComponent sp = entity.GetComponent<SpeedComponent>();
                RotaComponet rc = entity.GetComponent<RotaComponet>();
                rc.go.transform.Rotate(UnityEngine.Vector3.forward,sp.speed);
                sp.speed += 0.01f;
                entity.ReFreshComponent(sp);
            }
        }




        public GameObject pc,pr;

        public override void Startup()
        {
        var    module = Framework.env1.modules.ECS;
            module.SubscribeSystem(new PlayerSystem(module));
            module.SubscribeSystem(new PCSystem(module));

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    for (int k = 0; k < 10; k++)
                    {
                        var player = module.CreateEntity<SimpleEntity>();
                        player.AddComponent<PlayerComponent>();
                        var playerRO = player.AddComponent<RotaComponet>();
                        playerRO.go = GameObject.Instantiate(this.pr);
                        playerRO.go.name = "Player";
                        playerRO.go.transform.position = new UnityEngine.Vector3(i, j, k);
                    }
                }

            }

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    for (int k = 0; k < 10; k++)
                    {
                        var pc = module.CreateEntity<SimpleEntity>();
                        pc.AddComponent<SpeedComponent>();
                        pc.AddComponent<PCComponent>();
                        var pcRO = pc.AddComponent<RotaComponet>();
                        pcRO.go = GameObject.Instantiate(this.pc);

                        pcRO.go.name = "Pc";
                        pcRO.go.transform.position = new UnityEngine.Vector3(-i, j, k);
                    }
                }

            }

        }

        public override void Init()
        {
           
        }
    }
}
