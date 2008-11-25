/*
 * Created by SharpDevelop.
 * User: ratul
 * Date: 11/21/2008
 * Time: 11:06 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace Tashjik.Tier2.Common
{
	/// <summary>
	/// Description of Server.
	/// </summary>
	public abstract class Server : IOverlay
	{
	
		public abstract Guid getGuid();

		//Common.Data getData(String key);
		//void putData(String key, Common.Data data);

		public abstract void beginGetData(String key, AsyncCallback getDataCallBack, Object appState);
		public abstract void beginPutData(String key, Tashjik.Common.Data data, AsyncCallback putDataCallBack, Object appState);
		
		public abstract void shutdown();
	}
}
