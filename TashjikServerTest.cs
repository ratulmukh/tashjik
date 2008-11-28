/*
 * Created by SharpDevelop.
 * User: ratul
 * Date: 11/23/2008
 * Time: 10:49 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using Tashjik;
using Tashjik.Tier2;
using System.Collections;

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
		
		public static void Main()
		{
			Guid g = Guid.NewGuid();
			Console.Write(g.ToString());
			
			//ChordServer chord = (ChordServer)(TashjikServer.createNew(OverlayTypeEnum.Chord));
			ArrayList arr = TashjikServer.getList(OverlayTypeEnum.Chord);
			ChordServer chord = (ChordServer)(arr[0]);
			
			String key = "key";
			Tashjik.Common.Data data = new Tashjik.Common.Data();
			AsyncCallback putDataCallBack = new AsyncCallback(processPutDataCallBack);
			//chord.beginGetData(key, data, putDataCallBack, null);
			
			
		}
		
		static void processPutDataCallBack(IAsyncResult result)
		{
			
		}
		
		
	}
}
