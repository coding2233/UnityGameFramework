using UnityEngine;

namespace HotFix.Taurus
{
    [GameState(GameStateType.Start)]
    public class HotFixTestState : GameState
    {
        public override void OnEnter(params object[] parameters)
        {
            base.OnEnter(parameters);

            Debug.Log("HotFixTestState--Start!!");

            HotFixMode.Network.SetPort(35120);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
        }

        public override void OnInit()
        {
            base.OnInit();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (Input.GetKeyDown(KeyCode.T))
            {
                HotFixMode.Network.SendMessage(new HotFixProtoTest() {ID = 522, Commit = "xxxx", Message = "MMMMMMMM"},
                    new System.Net.IPEndPoint(System.Net.IPAddress.Parse("255.255.255.255"), 35120));
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                NetworkMessageCallTest();
            }
        }


        private async void NetworkMessageCallTest()
        {
            S2C_TestInfo s2C_TestInfo = await HotFixMode.Network.Call<S2C_TestInfo>(
                new C2S_TestInfo() {Message = "HotFixTestState -- C2S" },
                new System.Net.IPEndPoint(System.Net.IPAddress.Parse("255.255.255.255"), 35120));
        }
    }
}