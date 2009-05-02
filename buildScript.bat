echo on

cd I:\ratul\code\tashjik\ratul-Narada\Tier0 
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikTier0.dll /target:library ITransportLayerCommunicator.cs TransportLayerCommunicator.cs

cd I:\ratul\code\tashjik\ratul-Narada\Common
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikCommon.dll /target:library AsyncCallback_Object.cs Bool_Object.cs Bool_ObjectAsyncResult.cs ByteArray_AsyncCallback_Object.cs ByteArray_Data_AsyncCallback_Object.cs Data.cs Data_AsyncCallback_Object.cs Data_Object.cs Data_ObjectAsyncResult.cs DataAsyncResult.cs NodeBasic.cs ObjectAsyncResult.cs UtilityMethod.cs Exception\BytesLengthsNotMatchingException.cs Exception\LocalHostIPNotFoundException.cs

cd I:\ratul\code\tashjik\ratul-Narada
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikServer.dll /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikCommon.dll /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikTier0.dll  /target:library TashjikDataStream.cs OverlayServerFactory.cs OverlayController.cs OverlayServer.cs StreamingOverlayServer.cs TashjikServer.cs Node.cs IProxyNodeController.cs RealNode.cs ProxyNode.cs ProxyNodeController.cs 

cd I:\ratul\code\tashjik\ratul-Narada\Tier2
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\Pastry.dll /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikServer.dll /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikCommon.dll /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikBase.dll /target:library IPastryNode.cs PastryDataStore.cs PastryRealNode.cs PastryProxyNode.cs PastryServer.cs

cd I:\ratul\code\tashjik\ratul-Narada\Tier2\Streaming
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\Narada.dll /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikServer.dll /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikCommon.dll /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikBase.dll /target:library INaradaNode.cs NaradaRealNode.cs NaradaProxyNode.cs NaradaServer.cs

cd I:\ratul\code\tashjik\ratul-Narada\TashjikClient
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikClient.exe /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikServer.dll /target:exe Client.cs 





echo off
rem cd I:\ratul\code\tashjik\ratul-Narada\Tier2\Common
rem C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\Tier2Common.dll /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikServer.dll /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikCommon.dll /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikBase.dll /target:library INode.cs IProxyController.cs Node.cs NodeProxy.cs ProxyController.cs Server.cs StreamingServer.cs

rem cd I:\ratul\code\tashjik\ratul-Narada
rem C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikServer.dll /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikCommon.dll /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikBase.dll /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\Tier2Common.dll /target:library Controller.cs IOverlay.cs TashjikServer.cs 

rem cd I:\ratul\code\tashjik\ratul-Narada\Tier2\Streaming
rem C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc rem /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\Narada.dll /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikCommon.dll  /target:library NaradaNode.cs NaradaBootStrapper.cs INaradaNode.cs

rem cd I:\ratul\code\tashjik\ratul-Narada\Tier2Test\StreamingTest\NaradaTest
rem C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\NaradaTest.dll /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\Narada.dll  /target:library NaradaNodeTest.cs NaradaBootStrapperTest.cs NaradaTestExerciser.cs

rem cd I:\ratul\code\tashjik\ratul-Narada\
rem C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\TashjikServerTest.exe /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\NaradaTest.dll  /target:exe TashjikServerTest.cs



rem cd I:\ratul\code\tashjik\ratul-Narada\bin\debug
rem TashjikServerTest.exe

cd I:\ratul\code\tashjik\ratul-Narada

