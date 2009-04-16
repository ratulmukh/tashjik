echo off
rem cd I:\ratul\code\tashjik\ratul-Narada\Tier2\Common
rem C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc  /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\Common.dll  /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikServer.dll /target:library INode.cs  IProxyController.cs Node.cs NodeProxy.cs ProxyController.cs Server.cs   


rem cd I:\ratul\code\tashjik\ratul-Narada
rem C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikSrver.dll /target:library Controller.cs IOverlay.cs TashjikServer.cs 


rem cd I:\ratul\code\tashjik\ratul-Narada\TashjikClient
rem C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikClient.dll  /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\Narada.dll  /target:library Client.cs

echo on
cd I:\ratul\code\tashjik\ratul-Narada\Tier2\Streaming
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\Narada.dll  /target:library NaradaNode.cs NaradaBootStrapper.cs INaradaNode.cs

cd I:\ratul\code\tashjik\ratul-Narada\Tier2Test\StreamingTest\NaradaTest
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\NaradaTest.dll /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\Narada.dll  /target:library NaradaNodeTest.cs NaradaBootStrapperTest.cs NaradaTestExerciser.cs

cd I:\ratul\code\tashjik\ratul-Narada\
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikServerTest.exe /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\NaradaTest.dll  /target:exe TashjikServerTest.cs



cd I:\ratul\code\tashjik\ratul-Narada\bin\debug
TashjikServerTest.exe

cd I:\ratul\code\tashjik\ratul-Narada

