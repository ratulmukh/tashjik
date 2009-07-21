using System;
using Tashjik;
using Tashjik.Tier2;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
//using log4net;
//using log4net.Config;

namespace TashjikClient
{
	public class Client
	{
		//private static readonly ILog log = LogManager.GetLogger(typeof(Client));
		
		public Client()
		{
			
		}
				
		public static void Main()
		{
			Guid g = Guid.NewGuid();
			Console.Write(g.ToString());
			
			ChordServer chord = (ChordServer)(TashjikServer.createNew("Chord")); //new Guid("0c400880-0722-420e-a792-0a764d6539ee")));
			String key = "key";
			MemoryStream data = new MemoryStream(Marshal.SizeOf(chord));
			
			AsyncCallback putDataCallBack = new AsyncCallback(processPutDataCallBack);
			chord.beginPutData(key, data, (UInt64)(data.Length), putDataCallBack, chord);
/*			//ChordServer chord = (ChordServer)(TashjikServer.createNew(String.Chord));
			ArrayList arr = TashjikServer.getList(String.Chord);
			ChordServer chord = (ChordServer)(arr[0]);
			
			String key = "key";
			Tashjik.Common.Data data = new Tashjik.Common.Data();
			AsyncCallback putDataCallBack = new AsyncCallback(processPutDataCallBack);
			//chord.beginGetData(key, data, putDataCallBack, null);
*/
//Tashjik.Server.Node node = new Tashjik.Server.Node();
			
		}
		
		static void processPutDataCallBack(IAsyncResult result)
		{
			ChordServer chord = (ChordServer)(result.AsyncState);
			String key = "key";
			chord.beginGetData(key, new AsyncCallback(processGetDataCallBack), null);
		}
		
		static void processGetDataCallBack(IAsyncResult result)
		{
			Tashjik.Common.Data_Object data_Object = (Tashjik.Common.Data_Object)(result.AsyncState);
			Stream data = data_Object.data;
		//	log.Info(data);
							
		}
		
		
	}
}
