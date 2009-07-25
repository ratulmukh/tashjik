echo on

cd I:\ratul\code\tashjik\trunk\Common
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\trunk\bin\debug\TashjikCommon.dll /target:library AsyncCallback_Object.cs Bool_Object.cs Bool_ObjectAsyncResult.cs   Data.cs Data_AsyncCallback_Object.cs Data_Object.cs Data_ObjectAsyncResult.cs DataAsyncResult.cs NodeBasic.cs ObjectAsyncResult.cs TashjikAsyncResult.cs UtilityMethod.cs Exception\BytesLengthsNotMatchingException.cs Exception\LocalHostIPNotFoundException.cs


cd I:\ratul\code\tashjik\trunk\Tier0 
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\trunk\bin\debug\TashjikTier0.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikCommon.dll /target:library ITransportLayerCommunicator.cs TransportLayerCommunicator.cs





cd I:\ratul\code\tashjik\trunk
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\trunk\bin\debug\TashjikServer.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikCommon.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikTier0.dll  /target:library TashjikDataStream.cs OverlayServerFactory.cs OverlayController.cs OverlayServer.cs StreamingOverlayServer.cs TashjikServer.cs Node.cs IProxyNodeController.cs RealNode.cs ProxyNode.cs ProxyNodeController.cs 

cd I:\ratul\code\tashjik\trunk\Tier2
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\trunk\bin\debug\Tier2Common.dll /target:library DataNotFoundInStoreException.cs


cd I:\ratul\code\tashjik\trunk\Tier2
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\trunk\bin\debug\Pastry.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikServer.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikTier0.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\Tier2Common.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikCommon.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikBase.dll /target:library IPastryNode.cs PastryDataStore.cs PastryRealNode.cs PastryProxyNode.cs PastryServer.cs

cd I:\ratul\code\tashjik\trunk\Tier2
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\trunk\bin\debug\Chord.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikServer.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikTier0.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\Tier2Common.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikCommon.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikBase.dll  /target:library IChordNode.cs ChordDataStore.cs ChordRealNode.cs ChordProxyNode.cs ChordServer.cs ChordCommon\IChordNode_Object.cs chordCommon\IChordNode_ObjectAsyncResult.cs chordCommon\ByteKey_IChordNode.cs


 

rem cd I:\ratul\code\tashjik\trunk\Tier2\Streaming
rem C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\trunk\bin\debug\Narada.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikServer.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikCommon.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikBase.dll /target:library INaradaNode.cs NaradaRealNode.cs NaradaProxyNode.cs NaradaServer.cs

cd I:\ratul\code\tashjik\trunk\TashjikClient
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\trunk\bin\debug\TashjikClient.exe /r:I:\Progra~1\log4net\log4net-1.2.10\bin\net\2.0\release\log4net.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikCommon.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikServer.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\Chord.dll /target:exe Client.cs 

cd I:\ratul\code\tashjik\trunk\Tier0Test
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\trunk\bin\debug\Tier0TestSender.exe /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikCommon.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikTier0.dll DataSender.cs 

cd I:\ratul\code\tashjik\trunk\Tier0Test
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\trunk\bin\debug\Tier0TestReceiver.exe /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikCommon.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikTier0.dll DataReceiver.cs 



echo off
rem cd I:\ratul\code\tashjik\trunk\Tier2\Common
rem C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\trunk\bin\debug\Tier2Common.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikServer.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikCommon.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikBase.dll /target:library INode.cs IProxyController.cs Node.cs NodeProxy.cs ProxyController.cs Server.cs StreamingServer.cs

rem cd I:\ratul\code\tashjik\trunk
rem C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\trunk\bin\debug\TashjikServer.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikCommon.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikBase.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\Tier2Common.dll /target:library Controller.cs IOverlay.cs TashjikServer.cs 

rem cd I:\ratul\code\tashjik\trunk\Tier2\Streaming
rem C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc rem /out:I:\ratul\code\tashjik\trunk\bin\debug\Narada.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikCommon.dll  /target:library NaradaNode.cs NaradaBootStrapper.cs INaradaNode.cs

rem cd I:\ratul\code\tashjik\trunk\Tier2Test\StreamingTest\NaradaTest
rem C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\trunk\bin\debug\NaradaTest.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\Narada.dll  /target:library NaradaNodeTest.cs NaradaBootStrapperTest.cs NaradaTestExerciser.cs

rem cd I:\ratul\code\tashjik\trunk\
rem C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\trunk\bin\debug\TashjikServerTest.exe /r:I:\ratul\code\tashjik\trunk\bin\debug\NaradaTest.dll  /target:exe TashjikServerTest.cs



rem cd I:\ratul\code\tashjik\trunk\bin\debug
rem TashjikServerTest.exe

cd I:\ratul\code\tashjik\trunk

