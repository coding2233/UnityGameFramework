@echo off
protoc.exe --csharp_out="../HotFix/HotFix/Game/Network/Proto/" --proto_path="./Proto" HotFixProto.proto
pause