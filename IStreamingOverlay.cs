/*
 * Created by SharpDevelop.
 * User: ratul
 * Date: 4/18/2009
 * Time: 11:59 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Tashjik
{

	public interface IStreamingOverlay
	{
		Guid getGuid();
		
		List<TashjikDataStream> search(String key);
		void beginGetStream(TashjikDataStream stream, AsyncCallback getStreamCallBack, Object appState); 
		void addRepository(String directoryPath);
		
		void shutdown();
	}


}
