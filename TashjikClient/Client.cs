using System;
using Tashjik;
//using Tashjik.Tier2.Streaming;
using System.Collections;


namespace TashjikClient
{
	public class Client
	{
		public Client()
		{
			
		}
				
		public static void Main()
		{
			Guid g = Guid.NewGuid();
			Console.Write(g.ToString());
			
/*			//ChordServer chord = (ChordServer)(TashjikServer.createNew(String.Chord));
			ArrayList arr = TashjikServer.getList(String.Chord);
			ChordServer chord = (ChordServer)(arr[0]);
			
			String key = "key";
			Tashjik.Common.Data data = new Tashjik.Common.Data();
			AsyncCallback putDataCallBack = new AsyncCallback(processPutDataCallBack);
			//chord.beginGetData(key, data, putDataCallBack, null);
*/
Tashjik.Server.Node node = new Tashjik.Server.Node();
			
		}
		
		
		
	}
}
