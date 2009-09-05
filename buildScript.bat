echo on
rem csc=E:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc

cd Common
csc /out:..\bin\debug\TashjikCommon.dll /target:library AsyncCallback_Object.cs Bool_Object.cs Bool_ObjectAsyncResult.cs   Data.cs Data_AsyncCallback_Object.cs Data_Object.cs Data_ObjectAsyncResult.cs DataAsyncResult.cs NodeBasic.cs ObjectAsyncResult.cs TashjikAsyncResult.cs UtilityMethod.cs Exception\BytesLengthsNotMatchingException.cs Exception\LocalHostIPNotFoundException.cs


cd ../Tier0 
csc /out:..\bin\debug\TashjikTier0.dll /r:..\bin\debug\TashjikCommon.dll /target:library ITransportLayerCommunicator.cs TransportLayerCommunicator.cs





cd ..
csc /out:bin\debug\TashjikServer.dll /r:bin\debug\TashjikCommon.dll /r:bin\debug\TashjikTier0.dll  /target:library TashjikDataStream.cs OverlayServerFactory.cs OverlayController.cs OverlayServer.cs StreamingOverlayServer.cs TashjikServer.cs Node.cs IProxyNodeController.cs RealNode.cs ProxyNode.cs ProxyNodeController.cs 

cd Tier2
csc /out:..\bin\debug\Tier2Common.dll /target:library DataNotFoundInStoreException.cs



rem csc /out:..\bin\debug\Pastry.dll /r:..\bin\debug\TashjikServer.dll /r:..\bin\debug\TashjikTier0.dll /r:..\bin\debug\Tier2Common.dll /r:..\bin\debug\TashjikCommon.dll /r:..\bin\debug\TashjikBase.dll /target:library IPastryNode.cs PastryDataStore.cs PastryRealNode.cs PastryProxyNode.cs PastryServer.cs


csc /out:..\bin\debug\Chord.dll /r:..\bin\debug\TashjikServer.dll /r:..\bin\debug\TashjikTier0.dll /r:..\bin\debug\Tier2Common.dll /r:..\bin\debug\TashjikCommon.dll /r:..\bin\debug\TashjikBase.dll  /target:library IChordNode.cs ChordDataStore.cs ChordRealNode.cs ChordProxyNode.cs ChordServer.cs ChordCommon\IChordNode_Object.cs chordCommon\IChordNode_ObjectAsyncResult.cs chordCommon\ByteKey_IChordNode.cs


 
echo off
rem cd I:\ratul\code\tashjik\trunk\Tier2\Streaming
rem csc /out:I:\ratul\code\tashjik\trunk\bin\debug\Narada.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikServer.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikCommon.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikBase.dll /target:library INaradaNode.cs NaradaRealNode.cs NaradaProxyNode.cs NaradaServer.cs
echo on

cd ..\TashjikClient
csc /out:..\bin\debug\TashjikClient.exe  /r:..\bin\debug\TashjikCommon.dll /r:..\bin\debug\TashjikTier0.dll /r:..\bin\debug\TashjikServer.dll /r:..\bin\debug\Chord.dll /target:exe Client.cs 

cd ..\boxit
csc /r:..\bin\debug\TashjikCommon.dll /r:..\bin\debug\TashjikTier0.dll /out:..\bin\debug\Boxit.exe /target:exe Boxit.cs 

echo off
rem cd ..\Tier0Test
rem csc /out:..\bin\debug\Tier0TestSender.exe /r:..\bin\debug\TashjikCommon.dll /r:..\bin\debug\TashjikTier0.dll DataSender.cs 


rem csc /out:..\bin\debug\Tier0TestReceiver.exe /r:..\bin\debug\TashjikCommon.dll /r:..\bin\debug\TashjikTier0.dll DataReceiver.cs 


cd ..


rem echo off
rem cd I:\ratul\code\tashjik\trunk\Tier2\Common
rem csc /out:I:\ratul\code\tashjik\trunk\bin\debug\Tier2Common.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikServer.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikCommon.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikBase.dll /target:library INode.cs IProxyController.cs Node.cs NodeProxy.cs ProxyController.cs Server.cs StreamingServer.cs

rem cd I:\ratul\code\tashjik\trunk
rem csc /out:I:\ratul\code\tashjik\trunk\bin\debug\TashjikServer.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikCommon.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikBase.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\Tier2Common.dll /target:library Controller.cs IOverlay.cs TashjikServer.cs 

rem cd I:\ratul\code\tashjik\trunk\Tier2\Streaming
rem csc rem /out:I:\ratul\code\tashjik\trunk\bin\debug\Narada.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\TashjikCommon.dll  /target:library NaradaNode.cs NaradaBootStrapper.cs INaradaNode.cs

rem cd I:\ratul\code\tashjik\trunk\Tier2Test\StreamingTest\NaradaTest
rem csc /out:I:\ratul\code\tashjik\trunk\bin\debug\NaradaTest.dll /r:I:\ratul\code\tashjik\trunk\bin\debug\Narada.dll  /target:library NaradaNodeTest.cs NaradaBootStrapperTest.cs NaradaTestExerciser.cs

rem cd I:\ratul\code\tashjik\trunk\
rem csc /out:I:\ratul\code\tashjik\trunk\bin\debug\TashjikServerTest.exe /r:I:\ratul\code\tashjik\trunk\bin\debug\NaradaTest.dll  /target:exe TashjikServerTest.cs



rem cd I:\ratul\code\tashjik\trunk\bin\debug
rem TashjikServerTest.exe

rem cd I:\ratul\code\tashjik\trunk

