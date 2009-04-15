cd I:\ratul\code\tashjik\ratul-Narada\Tier2\Streaming
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\Narada.dll  /target:library NaradaNode.cs NaradaBootStrapper.cs INaradaNode.cs
cd I:\ratul\code\tashjik\ratul-Narada\Tier2Test\StreamingTest\NaradaTest
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /out:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\NaradaTest.dll  /r:I:\ratul\code\tashjik\Ratul-Narada\bin\debug\Narada.dll  /target:library NaradaNodeTest.cs NaradaBootStrapperTest.cs NaradaTestExerciser.cs
cd I:\ratul\code\tashjik\ratul-Narada\bin\debug
TashjikServerTest.exe
cd I:\ratul\code\tashjik\ratul-Narada

