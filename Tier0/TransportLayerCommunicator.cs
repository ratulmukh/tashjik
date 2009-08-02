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
using System.Runtime.Remoting.Messaging;
using Tashjik;
	
[assembly:InternalsVisibleTo("TransportLayerCommunicatorTest")]
namespace Tashjik.Tier0
{

		
	public class TransportLayerCommunicator
	{

		
		private class Msg
		{
			public readonly byte[] buffer;
			public readonly int offset;
			public readonly int size;
			public readonly Guid overlayGuid;
			public readonly AsyncCallback callBack;
			public readonly Object appState;
			
			public Msg(byte[] buffer, int offset, int size, Guid overlayGuid, AsyncCallback callBack, Object appState)
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
			public enum ConnectionState
			{
				NOT_CONNECTED,
				WAITING_TO_CONNECT,
				CONNECTED,
				CONNECTION_FAILED
			}
			
			private readonly Object msgQueueLock = new Object();
			private readonly IPAddress IP;
			private readonly Socket sock;
			private readonly Queue<Msg> msgQueue = new Queue<Msg>();
			private ConnectionState connectionState;
			private static readonly TransportLayerCommunicator transportLayerCommunicator = TransportLayerCommunicator.getRefTransportLayerCommunicator();
			
			public SockMsgQueue(IPAddress IP)
			{
				this.IP = IP;
				sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				connectionState = ConnectionState.NOT_CONNECTED;
			}
			
			public SockMsgQueue(Socket sock)
			{
				this.sock = sock;
				this.IP = ((IPEndPoint)(sock.RemoteEndPoint)).Address;
				if(sock.Connected)
					connectionState = ConnectionState.CONNECTED;
				else
					connectionState = ConnectionState.NOT_CONNECTED;
					
			}
			
			public ConnectionState getConnectionState()
			{
				return connectionState;
			}
			
			static private void beginConnectCallBackFor_establishRemoteConnection(IAsyncResult result)
			{
				SockMsgQueue sockMsgQueue = ((SockMsgQueue)(result.AsyncState));
				
				try
				{
					sockMsgQueue.sock.EndConnect(result);
				}
				catch(SocketException e)
				{
					Console.WriteLine("TransportLayerCommunicator::beginConnectCallBackFor_establishRemoteConnection SocketException");
					lock(sockMsgQueue.msgQueueLock)
					{
						sockMsgQueue.connectionState = ConnectionState.CONNECTION_FAILED;
						Msg msg;
						Tashjik.Common.TashjikAsyncResult asyncResult;
						Console.WriteLine(" inside connectionState = ConnectionState.CONNECTION_FAILED");
						Console.WriteLine(sockMsgQueue.msgQueue.Count);
						for(int i=0; i<sockMsgQueue.msgQueue.Count; i++)
						{
							msg = sockMsgQueue.msgQueue.Dequeue();
							Console.WriteLine("msg.deque");
							asyncResult = new Tashjik.Common.TashjikAsyncResult(msg.appState, false, false);
							if(msg.callBack != null)
							{
								Console.WriteLine("msg.callBack");
								msg.callBack(asyncResult);
							}
							else
								throw e;
							
						}
					}
					Console.WriteLine("TransportLayerCommunicator::beginConnectCallBackFor_establishRemoteConnection SocketException END of catch");
					
				}
			
			}
			
			
			private void establishRemoteConnection()
			{
				
				Console.WriteLine("TransportLayerCommunicator::SockMsgQueue::establishRemoteConnection ENTER");
				connectionState = ConnectionState.WAITING_TO_CONNECT;
				int iPortNo = System.Convert.ToInt16 ("2334");
				//IPEndPoint ipEnd = new IPEndPoint (IP,iPortNo);
				IPEndPoint ipEnd = new IPEndPoint (IP,iPortNo);
				Console.WriteLine("TransportLayerCommunicator::SockMsgQueue::establishRemoteConnection endPoint created");
				/*
				SocketState socketState = new SocketState();
				socketState.sock = sock;
				
				Stack<Object> thisAppState = new Stack<Object>();
				thisAppState.Push(this);
				thisAppState.Push();
				thisAppState.Push(appState);
				
				socketState.transportLayerCommunicator = transportLayerCommunicator;
			*/	AsyncCallback beginConnectCallBack = new AsyncCallback(beginConnectCallBackFor_establishRemoteConnection);
				
				Console.WriteLine("TransportLayerCommunicator::SockMsgQueue::establishRemoteConnection before calling beginConnect");					
				sock.BeginConnect(ipEnd, beginConnectCallBack, this);
				Console.WriteLine("TransportLayerCommunicator::SockMsgQueue::establishRemoteConnection EXIT");
				
			}
			
			public void enqueue(Msg msg)
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
//				Console.WriteLine("TransportLayerCommunicator::SockMsgQueue::dispatchMsg ENTER");
				
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
				else if(connectionState == ConnectionState.CONNECTION_FAILED)
				{
					return;
				}
				//I don't think it is required to lock 'count'
				//even if count turns out to be false, it shouldn't matter
				//the next loop will handle data if it is inside
				if(msgQueue.Count == 0)
					return;
				
				int msgQueueCount = msgQueue.Count;
				Msg tempMsg;
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
					concatenatedMsg.Append('\0', 1);
					concatenatedMsg.Append(Encoding.ASCII.GetString(tempMsg.buffer, tempMsg.offset, tempMsg.size));
					concatenatedMsg.Append('\0', 1);
					
				}
				concatenatedMsg.Append('\n', 0);

				String strCompositeMsg = concatenatedMsg.ToString();
				int compositeMsgLen    = strCompositeMsg.Length;
				byte[] compositeMsg    = System.Text.Encoding.ASCII.GetBytes(strCompositeMsg);
				
				SocketFlags f = new SocketFlags();  // :O
			    SocketState so2 = new SocketState();
			    so2.sock = sock;
			    sock.BeginSend(compositeMsg, 0, compositeMsgLen, f, new AsyncCallback(beginSendCallBackFor_DispatchMsg), so2);
			    	    
					
        		Console.WriteLine("TransportLayerCommunicator::SockMsgQueue::dispatchMsg EEXIT");
			}

			static private void beginSendCallBackFor_DispatchMsg(IAsyncResult result)
			{
				Console.WriteLine("TransportLayerCommunicator::SockMsgQueue::beginSendCallBackFor_DispatchMsg ENTER");
				
				SocketState so2   = ((SocketState)(result.AsyncState));
				so2.sock.EndSend(result);
				Console.WriteLine("TransportLayerCommunicator::SockMsgQueue::beginSendCallBackFor_DispatchMsg EXIT");
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

		public void EndTransportLayerSend(IPAddress IP)
		{
			SockMsgQueue sockMsgQueue;
			if(commRegistry.TryGetValue(IP, out sockMsgQueue))
				if(sockMsgQueue.getConnectionState() == SockMsgQueue.ConnectionState.CONNECTION_FAILED)
					throw new SocketException();
		}
		
		public void BeginTransportLayerSend(IPAddress IP, byte[] buffer, int offset, int size, Guid overlayGuid, AsyncCallback callBack, Object appState)
		{
			//Console.WriteLine("TransportLayerCommunicator::beginTransportLayerSend ENTER");
	
			Msg msg = new Msg(buffer, offset, size, overlayGuid, callBack, appState);
			
			SockMsgQueue sockMsgQueue;
			if(commRegistry.TryGetValue(IP, out sockMsgQueue))
				sockMsgQueue.enqueue(msg);
			else
			{
				sockMsgQueue = new SockMsgQueue(IP);
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
						BeginTransportLayerSend(IP, buffer, offset, size, overlayGuid, callBack, appState);
				}
			}
			Console.WriteLine("TransportLayerCommunicator::beginTransportLayerSend EXIT");
		}
				
		
		

		                                                  
		internal void receive(IPAddress fromIP, Guid overlayGuid, byte[] buffer, int offset, int size)
		{
			String s = Encoding.ASCII.GetString(buffer);
			Console.WriteLine(s);
			
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
			//IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
        	//	IPAddress ipAddress = ipHostInfo.AddressList[0];
        	byte[] byteIP = {127, 0, 0, 1};
			IPAddress ipAddress = new IPAddress(byteIP);

        	int iPortNo = System.Convert.ToInt16 ("2334");
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
			Console.WriteLine("TransportLayerCommunicator::beginAcceptCallback_forStartListening ENTER");
			// Signal the main thread to continue.
        	allDone.Set();

	        // Get the socket that handles the client request.
    	    SocketState socketState = (SocketState) result.AsyncState;
    	    Socket listener = socketState.sock;
        	Socket handler = listener.EndAccept(result);
			TransportLayerCommunicator transportLayerCommunicator = socketState.transportLayerCommunicator;
			
        	SockMsgQueue sockMsgQueue = new SockMsgQueue(handler);
        	IPAddress IP = ((IPEndPoint)(handler.RemoteEndPoint)).Address;
        	try
			{
				transportLayerCommunicator.commRegistry.Add(IP, sockMsgQueue);
				socketState.sock = handler;
		        handler.BeginReceive( socketState.buffer, 0, socketState.buffer.Length, new SocketFlags(), new AsyncCallback(beginReceiveCallBack), socketState);	
				Console.WriteLine("TransportLayerCommunicator::beginAcceptCallback_forStartListening ENTER");
        	}
			catch(System.ArgumentException)
			{
				socketState.sock = handler;
		        handler.BeginReceive( socketState.buffer, 0, socketState.buffer.Length, new SocketFlags(), new AsyncCallback(beginReceiveCallBack), socketState);	
		        Console.WriteLine("TransportLayerCommunicator::beginAcceptCallback_forStartListening ENTER");
			}
			Console.WriteLine("TransportLayerCommunicator::beginAcceptCallback_forStartListening ENTER");
	
		}
		
		static private void beginReceiveCallBack(IAsyncResult result)
		{
			Console.WriteLine("TransportLayerCommunicator::beginReceiveCallBack ENTER");
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
			content = socketState.concatenatedString.ToString();
			Console.WriteLine(content);
			notifyUpperLayer(content, sock, socketState.transportLayerCommunicator );
			Console.WriteLine("TransportLayerCommunicator::beginReceiveCallBack EXIT");
		}
		
		static private void notifyUpperLayer(String content, Socket fromSock, TransportLayerCommunicator transportLayerCommunicator)
		{
			Console.WriteLine("TransportLayerCommunicator::notifyUpperLayer ENTER");
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
					if(s.Length == 0)
						break;
					strOverlayGuid = s;
					Console.WriteLine("haha 1");
					Console.WriteLine(s);
					Console.WriteLine(s.Length);
					byteOverlayGuid = System.Text.Encoding.ASCII.GetBytes(strOverlayGuid);
					overlayGuid = new Guid(s);
					readytoNotify = true;
				}
				else if(readytoNotify == true)
				{
					strBuffer = s;
					Console.WriteLine("haha 2");
					Console.WriteLine(s);
					Console.WriteLine(s.Length);
					byteBuffer = System.Text.Encoding.ASCII.GetBytes(strBuffer);
					
					IPAddress fromIP = ((IPEndPoint)(fromSock.RemoteEndPoint)).Address;
					transportLayerCommunicator.receive(fromIP, overlayGuid, byteBuffer, 0, byteBuffer.Length);
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
	//		  		Console.WriteLine("TransportLayerCommunicator::messageDispatch inside while loop");								
					enumerator.Reset();
					for(int count=0; count<commRegistry.Count; count++)
					{
	//					Console.WriteLine("TransportLayerCommunicator::messageDispatch inside for loop");								
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
		private static readonly Object transportLayerCommunicatorLock = new Object();
		
		public static TransportLayerCommunicator getRefTransportLayerCommunicator()
		{
			lock(transportLayerCommunicatorLock)
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
}


