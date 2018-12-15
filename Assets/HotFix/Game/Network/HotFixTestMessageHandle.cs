using GT = GameFramework.Taurus;

namespace HotFix.Taurus
{
    [MessageHandler(typeof(HotFixProtoTest))]
    public class HotFixTestMessageHandle: MessageHandlerBase
    {
        public override void Handle(object message)
        {
            HotFixProtoTest test = message as HotFixProtoTest;
        }
    }
}