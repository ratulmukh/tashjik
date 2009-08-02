﻿using System;
using Tashjik;
using Tashjik.Tier2;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
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
			
			Console.WriteLine("Creating new Chord overlay");
			ChordServer chord = (ChordServer)(TashjikServer.createNew("Chord")); //new Guid("0c400880-0722-420e-a792-0a764d6539ee")));
			String strKey = "key";
			String strData = "data";
			Console.WriteLine(strKey);
			Console.WriteLine(strData);
			
			byte[] key = System.Text.Encoding.ASCII.GetBytes(strKey);
			byte[] data = System.Text.Encoding.ASCII.GetBytes(strData);
			Console.WriteLine(key);
			Console.WriteLine(data);
			
			Console.WriteLine(Encoding.ASCII.GetString(key));
			Console.WriteLine(Encoding.ASCII.GetString(data));
			
			Console.WriteLine(key.ToString());
			Console.WriteLine(data.ToString());
			
			Console.WriteLine("Putting data to new Chord ");
			chord.beginPutData(key, data, 0, strData.Length, new AsyncCallback(processPutDataCallBack), chord);
			Console.WriteLine("After Putting data to new Chord ");
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
			String strKey = "key";
			byte[] key = System.Text.Encoding.ASCII.GetBytes(strKey);
			chord.beginGetData(key, new AsyncCallback(processGetDataCallBack), null);
		}
		
		static void processGetDataCallBack(IAsyncResult result)
		{
			Tashjik.Common.Data_Object data_Object = (Tashjik.Common.Data_Object)(result.AsyncState);
			byte[] data = data_Object.data;
			Console.Write("DATA FOUND: IT IS ");
			Console.WriteLine(Encoding.ASCII.GetString(data));
			
		//	log.Info(data);
							
		}
		
		
	}
}
