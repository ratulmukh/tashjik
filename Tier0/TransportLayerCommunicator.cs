/************************************************************
* File Name: 
*
* Author: Ratul Mukhopadhyay
* ratuldotmukhATgmaildotcom
*
* This software is licensed under the terms and conditions of
* the MIT license, as given below.
*
* Copyright (c) <2008> <Ratul Mukhopadhyay>
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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;


namespace Tashjik.Tier0
{

		
	public class TransportLayerCommunicator
	{

		[Serializable]
		public class Msg
		{
			private readonly Guid overlayGuid;
			private Object data;

			public Msg(Guid overlayGuid)
			{
				this.overlayGuid = overlayGuid;
			}
	
			public Guid getGuid()
			{
				return new Guid(overlayGuid.ToByteArray());
			}

			public void setData(Object obj)
			{
				data = obj;
			}	
		
			public Object getData()
			{
				return data;
			}
		}
		
		private class SockMsgQueue
		{
			private enum ConnectionState
			{
				NOT_CONNECTED,
				WAITING_TO_CONNECT,
				CONNECTED
			}
			
			private IPAddress IP;
			private readonly Socket sock;
			private readonly Queue<Msg> msgQueue = new Queue<Msg>();
			private ConnectionState connectionState;
			private TransportLayerCommunicator transportLayerCommunicator;
			
			public SockMsgQueue(IPAddress IP, TransportLayerCommunicator transportLayerCommunicator)
			{
				this.transportLayerCommunicator = transportLayerCommunicator;
				this.IP = IP;
				sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
				connectionState = ConnectionState.NOT_CONNECTED;
			}
			
			public SockMsgQueue(Socket sock, TransportLayerCommunicator transportLayerCommunicator)
			{
				this.transportLayerCommunicator = transportLayerCommunicator;
				this.sock = sock;
				this.IP = ((IPEndPoint)(sock.RemoteEndPoint)).Address;
				if(sock.Connected)
					connectionState = ConnectionState.CONNECTED;
				else
					connectionState = ConnectionState.NOT_CONNECTED;
					
			}
			
			static private void beginConnectCallBackFor_establishRemoteConnection(IAsyncResult result)
			{
				SocketState socketState  = ((SocketState)(result.AsyncState));
				socketState.sock.EndConnect(result);
				
				socketState.sock.BeginReceive(socketState.buffer, 0, socketState.buffer.Length, new SocketFlags(), new AsyncCallback(beginReceiveCallBack), socketState);
			}
			
			
			private void establishRemoteConnection()
			{
				connectionState = ConnectionState.WAITING_TO_CONNECT;
				int iPortNo = System.Convert.ToInt16 ("2334");
				IPEndPoint ipEnd = new IPEndPoint (IP,iPortNo);
				
				SocketState socketState = new SocketState();
				socketState.sock = sock;
				socketState.transportLayerCommunicator = transportLayerCommunicator;
				AsyncCallback beginConnectCallBack = new AsyncCallback(beginConnectCallBackFor_establishRemoteConnection);
				sock.BeginConnect(ipEnd, beginConnectCallBack, socketState);
				
			}
			
			public void enqueue(Msg msg)
			{
				msgQueue.Enqueue(msg);
			}
			public void dispatchMsg()
			{
				if(connectionState == ConnectionState.NOT_CONNECTED)
				{
					establishRemoteConnection();
					return;
				}
				else if(connectionState == ConnectionState.WAITING_TO_CONNECT)
				{
					if(sock.Connected)
						connectionState = ConnectionState.CONNECTED;
					else
					    return;
				}
				
				if(msgQueue.Count ==0)
					return;
				
				BinaryFormatter formatter = new BinaryFormatter();
				
				byte[] byteArray;
											
				MemoryStream memStream = new MemoryStream(Marshal.SizeOf(msgQueue));
				try 
	    	    {
					formatter.Serialize(memStream, msgQueue);
					
			        SocketFlags f = new SocketFlags();  // :O
			        SocketState so2 = new SocketState();
			        so2.sock = sock;
			        byteArray = new byte[memStream.Length+1];
    				memStream.Read(byteArray, 0, (int)(memStream.Length));
    				byteArray[memStream.Length] = (byte)('\n');
    			    msgQueue.Clear();
    			    sock.BeginSend(byteArray, 0, (int)(memStream.Length), f, new AsyncCallback(beginSendCallBackFor_DispatchMsg), so2);
			    	    
					memStream.Close();
					
				}
		        catch (SerializationException e) 
			    {
    			    Console.WriteLine("Failed to serialize. Reason: " + e.Message);
        		    throw;
			    }
        
			}

			static private void beginSendCallBackFor_DispatchMsg(IAsyncResult result)
			{
				Socket sock   = ((Socket)(result.AsyncState));
				sock.EndSend(result);
			}
			
			static private void beginConnectCallBackForDispatchMsg(IAsyncResult result)
			{
				Msg msg     = ((Sock_Msg)(result.AsyncState)).msg;
				Socket sock = ((Sock_Msg)(result.AsyncState)).sock;
		            
				MemoryStream memStream = new MemoryStream(Marshal.SizeOf(msg));
			
				BinaryFormatter formatter = new BinaryFormatter();
				try 
	        	{
	    	        formatter.Serialize(memStream, msg);
    		    }
	        	catch (SerializationException e) 
		        {
    		        Console.WriteLine("Failed to serialize. Reason: " + e.Message);
        		    throw;
		        }
    		    /*finally 
        		{
	        	    memStream.Close();
	    	    }*/
    	    
    		    byte[] byteArray = new byte[memStream.Length];
    		    int count = memStream.Read(byteArray, 0, (int)(memStream.Length));
				memStream.Close();

				//sock.BeginSend(
		                                                  	
			}
		}
		
		[Serializable]
		class Sock_Msg
		{
			public Socket sock;
			public Msg msg;
		}
		
		//dictionary containing IPs and their corresponding queues
		//for every IP to whom we would like to maintain a connection,
		//there exists a queue of objects tht need to be dispatched
		private Dictionary<IPAddress, SockMsgQueue> commRegistry = new Dictionary<IPAddress, SockMsgQueue>();
		
		
		private Dictionary<Guid, ISink> overlayRegistry = new Dictionary<Guid, ISink>();

		public interface ISink
		{
			void notifyMsg(IPAddress fromIP, Object data);
		}

		public void register(Guid guid, ISink sink)
		{
			overlayRegistry.Add(guid, sink);

		}

		public void forwardMsgToRemoteHost(IPAddress IP, Msg msg)
		{
			SockMsgQueue sockMsgQueue;
											
			if(commRegistry.TryGetValue(IP, out sockMsgQueue))
				sockMsgQueue.enqueue(msg);
			else
			{
				sockMsgQueue = new SockMsgQueue(IP, this);
				sockMsgQueue.enqueue(msg);
				commRegistry.Add(IP, sockMsgQueue);
			}
		}
		
		
		

		                                                  
		internal void receive(IPAddress fromIP, Msg msg)
		{
			ISink sink;
			if(overlayRegistry.TryGetValue(msg.getGuid(), out sink))
				sink.notifyMsg(fromIP, msg.getData());
			else
				throw new Exception();

		}

		public class Forwarder
		{
		}

		public class Receiver
		{

		}

		//never call this directly
		public TransportLayerCommunicator()
		{
			init();
		}
		
		private void init()
		{
			//start thread to constantly traverse through 
			//commRegistry and dispatch msgs
		    ThreadStart messageDispatchJob = new ThreadStart(this.messageDispatch);
			Thread messageDispatcher = new Thread(messageDispatchJob);
			messageDispatcher.Start();	
			
			//initialise connection acceptor thread, etc,. 
		    ThreadStart listenJob = new ThreadStart(this.StartListening);
			Thread listener = new Thread(listenJob);
			listener.Start();	
		}
			
		static ManualResetEvent allDone = new ManualResetEvent(false);
		
		private void StartListening()
		{
			IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
        	IPAddress ipAddress = ipHostInfo.AddressList[0];
        	IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
        	
        	Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
        	try 
        	{
            	listener.Bind(localEndPoint);
            	listener.Listen(100);

	            while (true) 
	            {
    	            // Set the event to nonsignaled state.
        	        allDone.Reset();
	
    	            // Start an asynchronous socket to listen for connections.
        	        Console.WriteLine("Waiting for a connection...");
        	        
        	        SocketState socketState = new SocketState();
					socketState.sock = listener;
					socketState.transportLayerCommunicator = this;
            	    listener.BeginAccept( 
                	    new AsyncCallback(beginAcceptCallback_forStartListening),
                    	socketState );

	                // Wait until a connection is made before continuing.
    	            allDone.WaitOne();
        	    }

	        } 
        	catch (Exception e) 
        	{
    	        Console.WriteLine(e.ToString());
        	}	

		}
		
		static private void beginAcceptCallback_forStartListening(IAsyncResult result)
		{
			// Signal the main thread to continue.
        	allDone.Set();

	        // Get the socket that handles the client request.
    	    SocketState socketState = (SocketState) result.AsyncState;
    	    Socket listener = socketState.sock;
        	Socket handler = listener.EndAccept(result);
			TransportLayerCommunicator transportLayerCommunicator = socketState.transportLayerCommunicator;
			
        	SockMsgQueue sockMsgQueue = new SockMsgQueue(handler, transportLayerCommunicator);
        	IPAddress IP = ((IPEndPoint)(handler.RemoteEndPoint)).Address;
			transportLayerCommunicator.commRegistry.Add(IP, sockMsgQueue);
        	
			
			socketState.sock = handler;
	
	        handler.BeginReceive( socketState.buffer, 0, socketState.buffer.Length, new SocketFlags(), new AsyncCallback(beginReceiveCallBack), socketState);

		}
		
		static private void beginReceiveCallBack(IAsyncResult result)
		{
			String content = String.Empty;
			SocketState socketState = ((SocketState)(result.AsyncState));
			Socket sock = socketState.sock;
			
			int bytesRead = sock.EndReceive(result);
			if(bytesRead > 0)
			{
				socketState.concatenatedString.Append(Encoding.ASCII.GetString(socketState.buffer, 0, bytesRead));
				
				if(content.IndexOf("\n") > -1)
				{
					content = socketState.concatenatedString.ToString();
					notifyUpperLayer(content, sock, socketState.transportLayerCommunicator );
				}
				else 
					sock.BeginReceive(socketState.buffer, 0, socketState.buffer.Length, new SocketFlags(), new AsyncCallback(beginReceiveCallBack), socketState);
			}
		}
		
		static private void notifyUpperLayer(String content, Socket fromSock, TransportLayerCommunicator transportLayerCommunicator)
		{
			String serialisedObjectStr = content.Substring(0, content.Length - 1);
			MemoryStream memStream = new MemoryStream(serialisedObjectStr.Length);
			byte[] serialisedObjectByteArray = System.Text.Encoding.ASCII.GetBytes(serialisedObjectStr.ToString());
			memStream.Write(serialisedObjectByteArray, 0, serialisedObjectByteArray.Length);
			BinaryFormatter formatter = new BinaryFormatter();
			Queue<Msg> msgQueue = (Queue<Msg>)((formatter.Deserialize(memStream)));
				
			IPAddress fromIP = ((IPEndPoint)(fromSock.RemoteEndPoint)).Address;
			foreach( Msg msg in msgQueue )
			transportLayerCommunicator.receive(fromIP, msg);

		}
		
					internal class SocketState
			{
				public Socket sock;
				public byte[] buffer = new byte[1024];
				public StringBuilder concatenatedString = new StringBuilder();
				public TransportLayerCommunicator transportLayerCommunicator;
			}
		
		private void messageDispatch()
		{
			Dictionary<IPAddress, SockMsgQueue>.Enumerator enumerator = commRegistry.GetEnumerator();
			while(true)
			{
				SockMsgQueue sockMsgQueue = enumerator.Current.Value;
				sockMsgQueue.dispatchMsg();
				enumerator.MoveNext();
				//if enumerator reaches the end, does it circle around??
			}
		}

		//singleton
		private static TransportLayerCommunicator transportLayerCommunicator = null;
		
		//need to take care of threading issues
		public static TransportLayerCommunicator getRefTransportLayerCommunicator()
		{
			if(transportLayerCommunicator!=null)
				return transportLayerCommunicator;
			else
			{
				transportLayerCommunicator = new TransportLayerCommunicator();
				return transportLayerCommunicator;
			}
		}

	}
}


