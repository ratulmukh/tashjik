echo off
rem cd I:\ratul\code\tashjik\ratul-Narada\Tier2\Common
rem C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc  /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\Common.dll  /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikServer.dll /target:library INode.cs  IProxyController.cs Node.cs NodeProxy.cs ProxyController.cs Server.cs   


rem cd I:\ratul\code\tashjik\ratul-Narada
rem C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikSrver.dll /target:library Controller.cs IOverlay.cs TashjikServer.cs 


rem cd I:\ratul\code\tashjik\ratul-Narada\TashjikClient
rem C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikClient.dll  /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\Narada.dll  /target:library Client.cs





echo on
cd I:\ratul\code\tashjik\ratul-Narada\Common
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikCommon.dll /target:library AsyncCallback_Object.cs Bool_Object.cs Bool_ObjectAsyncResult.cs ByteArray_AsyncCallback_Object.cs ByteArray_Data_AsyncCallback_Object.cs Data.cs Data_AsyncCallback_Object.cs Data_Object.cs Data_ObjectAsyncResult.cs DataAsyncResult.cs NodeBasic.cs ObjectAsyncResult.cs UtilityMethod.cs Exception\BytesLengthsNotMatchingException.cs Exception\LocalHostIPNotFoundException.cs

cd I:\ratul\code\tashjik\ratul-Narada\Tier2\Streaming
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\Narada.dll /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikCommon.dll  /target:library NaradaNode.cs NaradaBootStrapper.cs INaradaNode.cs





echo off
rem cd I:\ratul\code\tashjik\ratul-Narada\Tier2Test\StreamingTest\NaradaTest
rem C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\NaradaTest.dll /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\Narada.dll  /target:library NaradaNodeTest.cs NaradaBootStrapperTest.cs NaradaTestExerciser.cs

rem cd I:\ratul\code\tashjik\ratul-Narada\
rem C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikServerTest.exe /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\NaradaTest.dll  /target:exe TashjikServerTest.cs



rem cd I:\ratul\code\tashjik\ratul-Narada\bin\debug
rem TashjikServerTest.exe

cd I:\ratul\code\tashjik\ratul-Narada

