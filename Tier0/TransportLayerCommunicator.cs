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
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("TransportLayerCommunicatorTest")]
namespace Tashjik.Tier0
{

		
	public class TransportLayerCommunicator
	{

		
		public class MsgNew
		{
			public readonly byte[] buffer;
			public readonly int offset;
			public readonly int size;
			public readonly Guid overlayGuid;
			public readonly AsyncCallback callBack;
			public readonly Object appState;
			
			public MsgNew(byte[] buffer, int offset, int size, Guid overlayGuid, AsyncCallback callBack, Object appState)
			{
				this.buffer = buffer;
				this.offset = offset;
				this.size = size;
				this.overlayGuid = overlayGuid;
				this.callBack = callBack;
				this.appState = appState;
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
			
			private readonly Object msgQueueLock = new Object();
			private IPAddress IP;
			private readonly Socket sock;
			private readonly Queue<MsgNew> msgQueue = new Queue<MsgNew>();
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
				try
				{
					SocketState socketState  = ((SocketState)(result.AsyncState));
					socketState.sock.EndConnect(result);
					//wtf is this beginReceive for?? :@ :@
					//socketState.sock.BeginReceive(socketState.buffer, 0, socketState.buffer.Length, new SocketFlags(), new AsyncCallback(beginReceiveCallBack), socketState);
				}
				catch(SocketException e)
				{
					Console.WriteLine("TransportLayerCommunicator::beginConnectCallBackFor_establishRemoteConnection SocketException");
					throw e;
				}
			
			}
			
			
			private void establishRemoteConnection()
			{
				Console.WriteLine("TransportLayerCommunicator::SockMsgQueue::establishRemoteConnection ENTER");
				connectionState = ConnectionState.WAITING_TO_CONNECT;
				int iPortNo = System.Convert.ToInt16 ("2334");
				IPEndPoint ipEnd = new IPEndPoint (IP,iPortNo);
				
				SocketState socketState = new SocketState();
				socketState.sock = sock;
				socketState.transportLayerCommunicator = transportLayerCommunicator;
				AsyncCallback beginConnectCallBack = new AsyncCallback(beginConnectCallBackFor_establishRemoteConnection);
				try {
				sock.BeginConnect(ipEnd, beginConnectCallBack, socketState);
				Console.WriteLine("TransportLayerCommunicator::SockMsgQueue::establishRemoteConnection EXIT");
				}
				catch(SocketException )
				{
					Console.WriteLine("TransportLayerCommunicator::beginConnectCallBackFor_establishRemoteConnection SocketException");
					//throw e;
				}
			}
			
			public void enqueue(MsgNew msg)
			{
				//for now, lets lock enque, deque and lock
				//need to explore if locking can be decreased
				//without compromising safety and increasing perf
							
				lock(msgQueueLock)
				{
					msgQueue.Enqueue(msg);
				}
			}
			
			public void dispatchMsg()
			{
				Console.WriteLine("TransportLayerCommunicator::SockMsgQueue::dispatchMsg ENTER");
				
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
				//I don't think it is required to lock 'count'
				//even if count turns out to be false, it shouldn't matter
				//the next loop will handle data if it is inside
				if(msgQueue.Count == 0)
					return;
				
				int msgQueueCount = msgQueue.Count;
				MsgNew tempMsg;
				StringBuilder concatenatedMsg = new StringBuilder();
				
				for(int i=0; i<msgQueueCount;i++)
				{
					lock(msgQueueLock)
					{
						//not sure if it required to apply a lock for this deque
						//thr is only 1 thread tht deques, while multiple 1s enque
						tempMsg = msgQueue.Dequeue();
					}
					
					concatenatedMsg.Append(tempMsg.overlayGuid.ToString());
					concatenatedMsg.Append('\0', 0);
					concatenatedMsg.Append(Encoding.ASCII.GetString(tempMsg.buffer, tempMsg.offset, tempMsg.size));
					concatenatedMsg.Append('\0', 0);
					
				}
				concatenatedMsg.Append('\n', 0);

				String strCompositeMsg = concatenatedMsg.ToString();
				int compositeMsgLen    = strCompositeMsg.Length;
				byte[] compositeMsg      = System.Text.Encoding.ASCII.GetBytes(strCompositeMsg);
				
				SocketFlags f = new SocketFlags();  // :O
			    SocketState so2 = new SocketState();
			    so2.sock = sock;
			    sock.BeginSend(compositeMsg, 0, compositeMsgLen, f, new AsyncCallback(beginSendCallBackFor_DispatchMsg), so2);
			    	    
					
        		Console.WriteLine("TransportLayerCommunicator::SockMsgQueue::dispatchMsg EEXIT");
			}

			static private void beginSendCallBackFor_DispatchMsg(IAsyncResult result)
			{
				Socket sock   = ((Socket)(result.AsyncState));
				sock.EndSend(result);
			}
			
			
		}

		//dictionary containing IPs and their corresponding queues
		//for every IP to whom we would like to maintain a connection,
		//there exists a queue of objects tht need to be dispatched
		private Dictionary<IPAddress, SockMsgQueue> commRegistry = new Dictionary<IPAddress, SockMsgQueue>();
		
		
		private Dictionary<Guid, ISink> overlayRegistry = new Dictionary<Guid, ISink>();

		public interface ISink
		{
			void notifyMsg(IPAddress fromIP, byte[] buffer, int offset, int size);
		}
		

		public void register(Guid guid, ISink sink)
		{
			overlayRegistry.Add(guid, sink);

		}

		public void endTransportLayerSend(IAsyncResult asyncResult
)
		{
			
		}
		
		public void beginTransportLayerSend(IPAddress IP, byte[] buffer, int offset, int size, Guid overlayGuid, AsyncCallback callBack, Object appState)
		{
			Console.WriteLine("TransportLayerCommunicator::beginTransportLayerSend ENTER");
	
			MsgNew msg = new MsgNew(buffer, offset, size, overlayGuid, callBack, appState);
			
			SockMsgQueue sockMsgQueue;
			if(commRegistry.TryGetValue(IP, out sockMsgQueue))
				sockMsgQueue.enqueue(msg);
			else
			{
				sockMsgQueue = new SockMsgQueue(IP, this);
				sockMsgQueue.enqueue(msg);
				try
				{
					commRegistry.Add(IP, sockMsgQueue);
			
				}
				catch(System.ArgumentException)
				{
					//this will take care of the situation where multiple
					//threads try to add a new enrty to the registry with
					//the same IP address
					if(commRegistry.TryGetValue(IP, out sockMsgQueue))
						sockMsgQueue.enqueue(msg);
					else 
						beginTransportLayerSend(IP, buffer, offset, size, overlayGuid, callBack, appState);
				}
			}
			Console.WriteLine("TransportLayerCommunicator::forwardMsgToRemoteHost EXIT");
		}
				
		
		

		                                                  
		internal void receive(IPAddress fromIP, Guid overlayGuid, byte[] buffer, int offset, int size)
		{
			ISink sink;
			if(overlayRegistry.TryGetValue(overlayGuid, out sink))
				sink.notifyMsg(fromIP, buffer, offset, size);
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
            	//listener.Listen(SocketOptionName.MaxConnections );

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
			String[] split = content.Split(new char[] {'\0'});
			String strOverlayGuid;
			byte[] byteOverlayGuid; 
			Guid overlayGuid = new Guid();;
			String strBuffer;
			byte[] byteBuffer;
			bool readytoNotify = false;  
			foreach (String s in split)
			{
				if(readytoNotify == false)
				{
					strOverlayGuid = s;
					byteOverlayGuid = System.Text.Encoding.ASCII.GetBytes(strOverlayGuid);
					overlayGuid = new Guid(byteOverlayGuid);
					readytoNotify = true;
				}
				else if(readytoNotify == true)
				{
					strBuffer = s;
					byteBuffer = System.Text.Encoding.ASCII.GetBytes(strBuffer);
					
					IPAddress fromIP = ((IPEndPoint)(fromSock.RemoteEndPoint)).Address;
					transportLayerCommunicator.receive(fromIP, overlayGuid, byteBuffer, 0, strBuffer.Length);
					readytoNotify = false;
				}
			}
			
			

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
			Console.WriteLine("TransportLayerCommunicator::messageDispatch ENTER");
			
			//Dictionary<IPAddress, SockMsgQueue>.Enumerator enumerator = commRegistry.GetEnumerator();
			IEnumerator enumerator; 
			while(true)
			{
				enumerator = commRegistry.GetEnumerator();
				try {
			  		Console.WriteLine("TransportLayerCommunicator::messageDispatch inside while loop");								
					enumerator.Reset();
					for(int count=0; count<commRegistry.Count; count++)
					{
						Console.WriteLine("TransportLayerCommunicator::messageDispatch inside for loop");								
						enumerator.MoveNext();
						SockMsgQueue sockMsgQueue = ((Dictionary<IPAddress, SockMsgQueue>.Enumerator)(enumerator)).Current.Value;
						sockMsgQueue.dispatchMsg();
						
					}
				}
				catch(InvalidOperationException)
				{
							
				}
			  
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


