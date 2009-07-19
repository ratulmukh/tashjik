using System;
//using Tashjik;
//using Tashjik.Tier2;
using System.Collections;
using TashjikTest.Tier2Test.StreamingTest.NaradaTest;

namespace TashjikTest
{
	/// <summary>
	/// Description of TashjikServerTest_cs.
	/// </summary>
	public class TashjikServerTest
	{
		public TashjikServerTest()
		{
			
		}
		
		public void exerciseTests()
		{
			NaradaTestExerciser naradaTestExerciser = new NaradaTestExerciser();
		}
		
		public static void Main()
		{
		/*	Guid g = Guid.NewGuid();
			Console.Write(g.ToString());
			
			//ChordServer chord = (ChordServer)(TashjikServer.createNew(OverlayTypeEnum.Chord));
			ArrayList arr = TashjikServer.getList(OverlayTypeEnum.Chord);
			ChordServer chord = (ChordServer)(arr[0]);
			
			String key = "key";
			Tashjik.Common.Data data = new Tashjik.Common.Data();
			AsyncCallback putDataCallBack = new AsyncCallback(processPutDataCallBack);
			//chord.beginGetData(key, data, putDataCallBack, null);
		*/	
		TashjikServerTest tashjikServerTest = new TashjikServerTest();
		tashjikServerTest.exerciseTests();
			
		}
		
		static void processPutDataCallBack(IAsyncResult result)
		{
			
		}
		
		
	}
}
