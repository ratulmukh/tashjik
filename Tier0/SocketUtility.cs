/************************************************************
* File Name: SocketUtility.cs
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
* 
* 
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
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Tashjik.Tier0
{
	public class SocketUtility
	{
		public SocketUtility()
		{
		}
		
		public class SocketListener
		{
			private readonly IPAddress ipAddress;
			private readonly int iPortNo;
			private AsyncCallback callbackListener;
			private Object appState;
			private ManualResetEvent allDone = new ManualResetEvent(false);
			
			public SocketListener(IPAddress ipAddress, int iPortNo, AsyncCallback callbackListener, Object appState)
			{
				this.appState = appState;
				this.callbackListener = callbackListener;
				this.ipAddress = ipAddress;
				this.iPortNo = iPortNo;
			}
			
			public void SocketStartListening()
			{
				IPEndPoint localEndPoint = new IPEndPoint(ipAddress, iPortNo);
           		Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
			              	
           		try
        		{
           		  	listener.Bind(localEndPoint);
            		listener.Listen(100);
            		//listener.Listen(SocketOptionName.MaxConnections );

	            	while (true) 
	            	{
	    	            // Set the event to nonsignaled state.
    	    	        allDone.Reset();
		
    	    	        // Start an asynchronous socket to listen for connections.
        		        Console.WriteLine("SocketUtility::Waiting for a connection at port {0}", iPortNo);
    	                	            
        	    	    SocketListenState socketListenState = new SocketListenState(listener, allDone, appState);
						listener.BeginAccept(callbackListener,socketListenState);
						
	    	            // Wait until a connection is made before continuing.
    	    	        allDone.WaitOne();
        	    	}
	
		        } 
        		catch (Exception e) 
	        	{
    		        Console.WriteLine(e.ToString());
        		}	
			}
		
			public class SocketStartListeningParams
			{
				public IPAddress ipAddress; 
				public int iPortNo;
				public AsyncCallback callback;
				public Object appState;
			}
		
			public class SocketListenState
			{
				public Socket sock;
				public ManualResetEvent allDone;
				public Object obj;
				public SocketListenState(Socket sock, ManualResetEvent allDone, Object obj)
				{
					this.sock = sock;
					this.allDone = allDone;
					this.obj = obj; 
				}
			}
		}	
	}
}
