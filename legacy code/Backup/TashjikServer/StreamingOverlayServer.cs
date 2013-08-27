/* File Name: StreamingOverlayServer.cs
*
* Author: Ratul Mukhopadhyay
* ratuldotmukhATgmaildotcom
*
* This software is licensed under the terms and conditions of
* the MIT license, as given below.
*
* Copyright (c) <2008-2010> <Ratul Mukhopadhyay>
*
* Permission is hereby granted, free of charge, to any person
* obtaining a copy of this software and associated documentation
* files (the "Software"), to deal in the Software without
* restriction, including without limitation the rights to use,
* copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the
* Software is furnished to do so, subject to the following
* conditions:
*
* The above copyright notice and this permission notice shall be
* included in all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
* OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
* NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
* HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
* WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
* OTHER DEALINGS IN THE SOFTWARE.
*
*
* Description:
* This class is only to circumvent cicrcular referencing between
* TashjikServer.dll and Tier2Common.dll. It is the same as 
* ProxyController.cs, but with all its TashjikServer.dll
* references removed.
*
* Supporting Documentation:
*
* Portability: .NET VES
* Status: Experimental
* Reuse Reviews:
* Date Name Comment
*
* Modification History:
*
************************************************************/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Tashjik
{
	/// <summary>
	/// Description of StreamingServer.
	/// </summary>
	public abstract class StreamingOverlayServer 
	{
		public abstract Guid getGuid();
		
//		public abstract List<TashjikDataStream> search(String key);
//		public abstract void beginGetStream(TashjikDataStream stream, AsyncCallback getStreamCallBack, Object appState); 
//		public abstract void addRepository(String directoryPath);
		
		public abstract void shutdown();
		
		private ProxyNodeController proxyNodeController;
		
		public StreamingOverlayServer(ProxyNodeController.CreateProxyNodeDelegate createProxyNodeDelegate, Guid overlayInstanceGuid)
		{
			proxyNodeController = new ProxyNodeController(createProxyNodeDelegate, overlayInstanceGuid);
		}
		
		internal  ProxyNode getProxyNode(IPAddress IP)
		{
			return proxyNodeController.getProxyNode(IP);
		}
		
		internal ProxyNodeController getProxyNodeController()
		{
			return proxyNodeController;
		}
		
		//internal delegate ProxyNode CreateProxyNodeDelegate(IPAddress IP);
		
		/*internal void setCreateProxyNodeDelegate(ProxyNodeController.CreateProxyNodeDelegate createProxyNodeDelegate)
		{
			proxyNodeController.setCreateProxyNodeDelegate(createProxyNodeDelegate);
		}
		*/
	}
}
