using HotFix.Taurus;
using GT=GameFramework.Taurus;

[GT.Message(1000)]
public partial class HotFixProtoTest
{
}

[GT.Message(1001)]
public partial class C2S_TestInfo : IRequest
{
}

[GT.Message(1002)]
public partial class S2C_TestInfo : IResponse
{
}