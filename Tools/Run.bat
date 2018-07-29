@echo off
protoc.exe --csharp_out="../Assets/" --proto_path="./Proto" ProtoTest.proto
pause