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
			private readonly Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
			private readonly Queue<Msg> msgQueue = new Queue<Msg>();
			private ConnectionState connectionState;
			
			public SockMsgQueue(IPAddress IP)
			{
				this.IP = IP;
				connectionState = ConnectionState.NOT_CONNECTED;
			}
			
			static private void beginConnectCallBackFor_establishRemoteConnection(IAsyncResult result)
			{
				Socket sock   = ((Socket)(result.AsyncState));
				sock.EndConnect(result);
			}
			

			
			private void establishRemoteConnection()
			{
				connectionState = ConnectionState.WAITING_TO_CONNECT;
				int iPortNo = System.Convert.ToInt16 ("2334");
				IPEndPoint ipEnd = new IPEndPoint (IP,iPortNo);
				
				AsyncCallback beginConnectCallBack = new AsyncCallback(beginConnectCallBackFor_establishRemoteConnection);
				sock.BeginConnect(ipEnd, beginConnectCallBack, null);
				
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
				MemoryStream memStream, sizeMemStream;
				byte[] byteArray, sizeArray;
				Int64 memStreamSize = new Int64();
								
				memStream = new MemoryStream(Marshal.SizeOf(msgQueue));
				memStreamSize = (long)(memStream.Length);
				sizeMemStream = new MemoryStream(Marshal.SizeOf(memStreamSize));
				try 
	    	    {
					formatter.Serialize(memStream, msgQueue);
					formatter.Serialize(sizeMemStream, memStreamSize);

			        sizeArray = new byte[sizeMemStream.Length];
			        sizeMemStream.Read(sizeArray, 0, (int)(sizeMemStream.Length));
			        SocketFlags f = new SocketFlags();  // :O
			        sock.BeginSend(sizeArray, 0, (int)(sizeMemStream.Length), f, new AsyncCallback(beginSendCallBackFor_DispatchMsg), null);
				
		    	    byteArray = new byte[memStream.Length];
    				memStream.Read(byteArray, 0, (int)(memStream.Length));
    			    msgQueue.Clear();
    			    //NOTE: I am assuming tht the previous BeginSend will reach before
    			    //the BeginSend below. This is because the firsst BeginSend contains
    			    //the size of the data coming in after tht. Hope I am right
    				sock.BeginSend(byteArray, 0, (int)(memStream.Length), f, new AsyncCallback(beginSendCallBackFor_DispatchMsg), null);
			    	//I think thr is a corresponding CloseSend to be called in the callback
				        
					memStream.Close();
					sizeMemStream.Close();
				
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
				sockMsgQueue = new SockMsgQueue(IP);
				sockMsgQueue.enqueue(msg);
				commRegistry.Add(IP, sockMsgQueue);
			}
		}
		
		
		

		                                                  
		private void receive(IPAddress fromIP, Msg msg)
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
		    ThreadStart job = new ThreadStart(this.messageDispatch);
			Thread messageDispatcher = new Thread(job);
			messageDispatcher.Start();	
			
			//initialise connection acceptor thread, etc,. 
			
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


