using GT=GameFramework.Taurus;

namespace HotFix.Taurus
{
    [GT.MessageHandler(typeof(C2S_TestInfo))]
    public class C2S_TestInfo_Handle:MessageHandlerBase
    {
        public override void Handle(object message)
        {
            C2S_TestInfo c2S_TestInfo = message as C2S_TestInfo;
            
            HotFixMode.Network.SendMessage(new S2C_TestInfo(){ Message = "C2S_TestInfo_Handle -- S2C" },new System.Net.IPEndPoint(System.Net.IPAddress.Parse("255.255.255.255"),35120));
        }
    }
}