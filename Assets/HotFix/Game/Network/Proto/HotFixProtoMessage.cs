using HotFix.Taurus;

[Message(1000)]
public partial class HotFixProtoTest
{
}

[Message(1001)]
public partial class C2S_TestInfo : IRequest
{
}

[Message(1002)]
public partial class S2C_TestInfo : IResponse
{
}