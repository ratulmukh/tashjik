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


namespace Tashjik.Base
{
	public class LowLevelComm
	{
		[Serializable]
		public class Msg
		{
			private readonly Guid overlayGuid;
			private Object data;


			public Msg(Guid guid)
			{
				overlayGuid = guid;
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
			private readonly Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
			private readonly Queue<Msg> msgQueue = new Queue<Msg>();

			public SockMsgQueue(IPAddress IP)
			{
					int iPortNo = System.Convert.ToInt16 ("2334");
					IPEndPoint ipEnd = new IPEndPoint (IP,iPortNo);
				
//					AsyncCallback beginConnectCallBack = new AsyncCallback(beginConnectCallBackForDispatchMsg);
//					Sock_Msg sock_msg = new Sock_Msg();
//					sock_msg.msg = msg;
//					sock_msg.sock = sock;
					sock.BeginConnect(ipEnd, null, null);
		
			}
				
			
			public void enqueue(Msg msg)
			{
				msgQueue.Enqueue(msg);
			}
			public void dispatchMsg()
			{
				BinaryFormatter formatter = new BinaryFormatter();
				MemoryStream memStream;
				byte[] byteArray;
					
				for(int i=0; i<msgQueue.Count; i++)
				{
					Msg msg = msgQueue.Dequeue();
					
					memStream = new MemoryStream(Marshal.SizeOf(msg));
								
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
    	    	
	    		    byteArray = new byte[memStream.Length];
    			    int count = memStream.Read(byteArray, 0, (int)(memStream.Length));
						
    			    SocketFlags f = new SocketFlags();  // :O
    			    sock.BeginSend(byteArray, 0, (int)(memStream.Length), f, null, null);
			        //I think thr is a corresponding CloseSend to be called in the callback
			        
					memStream.Close();
					
				}
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

		public void forward(IPAddress IP, Msg msg)
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
		public LowLevelComm()
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
		private static LowLevelComm lowLevelComm = null;
		
		//need to take care of threading issues
		public static LowLevelComm getRefLowLevelComm()
		{
			if(lowLevelComm!=null)
				return lowLevelComm;
			else
			{
				lowLevelComm = new LowLevelComm();
				return lowLevelComm;
			}
		}

	}
}


