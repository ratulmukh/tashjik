/************************************************************
* File Name: TransportLayerCommunicator.cs
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

#define SIM
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

using Tashjik.Common;

[assembly:InternalsVisibleTo("TransportLayerCommunicatorTest")]
namespace Tashjik.Tier0
{

		
	public class TransportLayerCommunicator
	{

		public enum CallType
		{
			ONE_WAY,
			TWO_WAY_SEND,
			TWO_WAY_REPLY,
			TWO_WAY_RELAY_SEND,
			TWO_WAY_RELAY_REPLY
		}
		private class Msg
		{
			public readonly byte[] buffer;
			public readonly int offset;
			public readonly int size;
			public readonly Guid overlayGuid;
			public readonly AsyncCallback callBack;
			public readonly Object appState;
			public readonly CallType callType;
			
			public Msg(byte[] buffer, int offset, int size, Guid overlayGuid, AsyncCallback callBack, Object appState, CallType callType)
			{
				this.buffer = buffer;
				this.offset = offset;
				this.size = size;
				this.overlayGuid = overlayGuid;
				this.callBack = callBack;
				this.appState = appState;
				this.callType = callType;
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
			private readonly TransportLayerCommunicator transportLayerCommunicator; // = TransportLayerCommunicator.getRefTransportLayerCommunicator();
			
			public SockMsgQueue(IPAddress IP, TransportLayerCommunicator transportLayerCommunicator)
			{
				this.transportLayerCommunicator = transportLayerCommunicator;
				this.IP = IP;
				sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
				Console.Write("TransportLayerCommunicator::DEEPCHK overlayRegistry count = ");
				Console.WriteLine(transportLayerCommunicator.overlayRegistry.Count);
				connectionState = ConnectionState.WAITING_TO_CONNECT;
#if SIM
				int iPortNo = System.Convert.ToInt16("2335"); //Boxit port
				byte[] byteIP = {127, 0, 0, 1};
				IPAddress ipAddress = new IPAddress(byteIP);
				IPEndPoint ipEnd = new IPEndPoint (ipAddress,iPortNo);
#else				
				int iPortNo = System.Convert.ToInt16 ("2334");
				IPEndPoint ipEnd = new IPEndPoint (IP,iPortNo);
#endif
				
				
				Console.WriteLine("TransportLayerCommunicator::SockMsgQueue::establishRemoteConnection endPoint created");
				Console.Write("TransportLayerCommunicator::DEEPCHK overlayRegistry count = ");
				Console.WriteLine(transportLayerCommunicator.overlayRegistry.Count);
				
				AsyncCallback beginConnectCallBack = new AsyncCallback(beginConnectCallBackFor_establishRemoteConnection);
				Console.Write("TransportLayerCommunicator::DEEPCHK overlayRegistry count = ");
				Console.WriteLine(transportLayerCommunicator.overlayRegistry.Count);
				Console.WriteLine("TransportLayerCommunicator::SockMsgQueue::establishRemoteConnection before calling beginConnect");					
				sock.BeginConnect(ipEnd, beginConnectCallBack, this);
				Console.Write("TransportLayerCommunicator::DEEPCHK overlayRegistry count = ");
				Console.WriteLine(transportLayerCommunicator.overlayRegistry.Count);
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
			//	Console.WriteLine("TransportLayerCommunicator::SockMsgQueue::dispatchMsg ENTER");

				if(connectionState == ConnectionState.NOT_CONNECTED)
				{
					Console.WriteLine("TransportLayerCommunicator::SockMsgQueue::dispatchMsg connectionState == NOT_CONNECTED");
					establishRemoteConnection();
					return;
				}
				else if(connectionState == ConnectionState.WAITING_TO_CONNECT)
				{
					Console.WriteLine("TransportLayerCommunicator::SockMsgQueue::dispatchMsg connectionState == WAITING_TO_CONNECT");
					if(sock.Connected)
						connectionState = ConnectionState.CONNECTED;
					else
					    return;
				}
				else if(connectionState == ConnectionState.CONNECTION_FAILED)
				{
					Console.WriteLine("TransportLayerCommunicator::SockMsgQueue::dispatchMsg connectionState == CONNECTION_FAILED");
					return;
				}
				//I don't think it is required to lock 'count'
				//even if count turns out to be false, it shouldn't matter
				//the next loop will handle data if it is inside
				if(msgQueue.Count == 0)
					return;
				Console.Write("TransportLayerCommunicator::SockMsgQueue::dispatchMsg msgQueue.Count =");
				Console.WriteLine(msgQueue.Count);
				Console.Write("TransportLayerCommunicator::DEEPCHK overlayRegistry count = ");
				Console.WriteLine(transportLayerCommunicator.overlayRegistry.Count);
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
#if SIM
					concatenatedMsg.Append(UtilityMethod.GetLocalHostIP().ToString());
					concatenatedMsg.Append('\0', 1);
					concatenatedMsg.Append(IP.ToString());
					concatenatedMsg.Append('\0', 1);
#endif				
					concatenatedMsg.Append(tempMsg.callType.ToString());
                    concatenatedMsg.Append('\0', 1);
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
			    Console.Write("TransportLayerCommunicator::SockMsgQueue::dispatchMsg msg to be sent = ");
			    Console.WriteLine(strCompositeMsg);
			    Console.Write("TransportLayerCommunicator::DEEPCHK overlayRegistry count = ");
				Console.WriteLine(transportLayerCommunicator.overlayRegistry.Count);
			    Console.WriteLine("TransportLayerCommunicator::SockMsgQueue::dispatchMsg finally calling beginSend on the socket");
			    sock.BeginSend(compositeMsg, 0, compositeMsgLen, f, new AsyncCallback(beginSendCallBackFor_DispatchMsg), so2);
			    Console.Write("TransportLayerCommunicator::DEEPCHK overlayRegistry count = ");
				Console.WriteLine(transportLayerCommunicator.overlayRegistry.Count);	    
					
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
			Data notifyTwoWayMsg(IPAddress fromIP, byte[] buffer, int offset, int size);
			Data notifyTwoWayRelayMsg(IPAddress fromIP, IPAddress originalFromIP, byte[] buffer, int offset, int size, Guid relayTicket);
			void notifyTwoWayReplyReceived(IPAddress fromIP, byte[] buffer, int offset, int size, AsyncCallback originalRequestCallBack, Object originalAppState);
		}
		

		public void register(Guid guid, ISink sink)
		{
			Console.WriteLine("TransportLayerCommunicator::register ADD to overlayRegistry");
			overlayRegistry.Add(guid, sink);
			Console.Write("TransportLayerCommunicator::register overlayRegistry count = ");
			Console.WriteLine(overlayRegistry.Count);
		}
			


		public void EndTransportLayerSend(IPAddress IP)
		{
			EndSend(IP);
		}
		
		public void EndTransportLayerSendTwoWay(IPAddress IP)
		{
			EndSend(IP);
		}
		
		public void EndTransportLayerSendTwoWayRelay(IPAddress IP)
		{
			EndSend(IP);
		}
		
		private void EndSend(IPAddress IP)
		{
			SockMsgQueue sockMsgQueue;
			if(commRegistry.TryGetValue(IP, out sockMsgQueue))
				if(sockMsgQueue.getConnectionState() == SockMsgQueue.ConnectionState.CONNECTION_FAILED)
					throw new SocketException();
		}
		
		void TransportLayerSend(IPAddress IP, byte[] buffer, int offset, int size, Guid overlayGuid, AsyncCallback callBack, Object appState)
		{
		
		}
		
		private void addToSockMsgQueue(IPAddress IP, byte[] buffer, int offset, int size, Guid overlayGuid, AsyncCallback callBack, Object appState, CallType callType)
		{
			Console.WriteLine("TransportLayerCommunicator::addToSockMsgQueue ENTER");
			Console.Write("TransportLayerCommunicator::DEEPCHK overlayRegistry count = ");
			Console.WriteLine(overlayRegistry.Count);
			Msg msg = new Msg(buffer, offset, size, overlayGuid, callBack, appState, callType);
			
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
						addToSockMsgQueue(IP, buffer, offset, size, overlayGuid, callBack, appState, callType);
				}
			}
			Console.Write("TransportLayerCommunicator::DEEPCHK overlayRegistry count = ");
			Console.WriteLine(overlayRegistry.Count);
			Console.WriteLine("TransportLayerCommunicator::addToSockMsgQueue EXIT");
		}
				
		class TwoWayCallBackData
		{
			public AsyncCallback callBack;
			public Object appState;
		}
		
		Dictionary<String, TwoWayCallBackData> twoWayCallBackRegistry = new Dictionary<String, TwoWayCallBackData>();
		
		public void BeginTransportLayerSend(IPAddress IP, byte[] buffer, int offset, int size, Guid overlayGuid, AsyncCallback callBack, Object appState)
		{
			addToSockMsgQueue(IP, buffer, offset, size, overlayGuid, callBack, appState, CallType.ONE_WAY);
		
		}
		
		public void BeginTransportLayerSendTwoWay(IPAddress IP, byte[] buffer, int offset, int size, Guid overlayGuid, AsyncCallback callBack, Object appState)
		{
			Console.WriteLine("TransportLayerComm::BeginTransportLayerSendTwoWay ENTER");
			
			Guid twoWayTicket   = addToTwoWayCallBackRegistry(callBack, appState);
			String strBuffer    = Encoding.ASCII.GetString(buffer, offset, size);
			byte[] compositeMsg = UtilityMethod.convertToTabSeparatedByteArray(false, twoWayTicket.ToString(), strBuffer);
			
			addToSockMsgQueue(IP, compositeMsg, 0, compositeMsg.Length, overlayGuid, null, null, CallType.TWO_WAY_SEND);
		
		}
		
		public void BeginTransportLayerSendTwoWayRelay(IPAddress IP, byte[] buffer, int offset, int size, Guid overlayGuid, AsyncCallback callBack, Object appState, Guid relayTicket)
		{
			Console.WriteLine("TransportLayerCommunicator::BeginTransportLayerSendTwoWayRelay ENTER");
			
			Guid twoWayTicket        = processRelayGuid(relayTicket, callBack, appState);
			String strOriginalFromIP = extractOriginalFromIP(relayTicket);
			String strBuffer         = Encoding.ASCII.GetString(buffer, offset, size);
			
			composeAndSendTwoWayRelay(IP, overlayGuid, twoWayTicket, strOriginalFromIP, strBuffer);
		}
		
		Dictionary<String, String> relayTicketRegistry = new Dictionary<String, String>();
		
		private TwoWayCallBackData createTwoWayCallBackData(AsyncCallback callBack, Object appState)
		{
			TwoWayCallBackData twoWayCallBackData = new TwoWayCallBackData();
			twoWayCallBackData.callBack = callBack;
			twoWayCallBackData.appState = appState;
			return twoWayCallBackData;
		}
		
		private Guid addToTwoWayCallBackRegistry(AsyncCallback callBack, Object appState)
		{
			Guid twoWayTicket = Guid.NewGuid();
			TwoWayCallBackData twoWayCallBackData = createTwoWayCallBackData(callBack, appState);
			twoWayCallBackRegistry.Add(twoWayTicket.ToString(), twoWayCallBackData);
			return twoWayTicket;
		}
		
		private Guid processRelayGuid(Guid relayTicket, AsyncCallback callBack, Object appState)
		{
			Guid twoWayTicket;
			if(relayTicket == new Guid("00000000-0000-0000-0000-000000000000"))
			{
				Console.WriteLine("TransportLayerCommunicator::BeginTransportLayerSendTwoWayRelay new relay request");
				twoWayTicket = addToTwoWayCallBackRegistry(callBack, appState);
			}
			else
			{
				Console.WriteLine("TransportLayerCommunicator::BeginTransportLayerSendTwoWayRelay old relay request");
				twoWayTicket = relayTicket;
			}
			return twoWayTicket;
		}
		
		private String extractOriginalFromIP(Guid relayTicket)
		{
			String strOriginalFromIP;
			if(relayTicket == new Guid("00000000-0000-0000-0000-000000000000"))
				strOriginalFromIP = UtilityMethod.GetLocalHostIP().ToString();
			else
			{
				if(relayTicketRegistry.TryGetValue(relayTicket.ToString(), out strOriginalFromIP))
					relayTicketRegistry.Remove(relayTicket.ToString());
				else
					throw new Exception();
			}
			return strOriginalFromIP;
		}
			
        private void composeAndSendTwoWayRelay(IPAddress IP, Guid overlayGuid, Guid twoWayTicket, String strOriginalFromIP, String strBuffer)
		{
			byte[] compositeMsg;
			//no need to send originalIP if the relay is going back to originalIP itself!
			if(String.Compare(IP.ToString(), strOriginalFromIP) != 0)
			{
				compositeMsg = UtilityMethod.convertToTabSeparatedByteArray(false, twoWayTicket.ToString(), strOriginalFromIP, strBuffer);
				addToSockMsgQueue(IP, compositeMsg, 0, compositeMsg.Length, overlayGuid, null, null, CallType.TWO_WAY_RELAY_SEND);
			}
			else
			{
				compositeMsg = UtilityMethod.convertToTabSeparatedByteArray(false, twoWayTicket.ToString(), strBuffer);
				addToSockMsgQueue(IP, compositeMsg, 0, compositeMsg.Length, overlayGuid, null, null, CallType.TWO_WAY_RELAY_REPLY);
			}
		}
		
	/*	public void registerTwoWay(Guid guid, List<TwoWayCallbackData> twoWayCallbackDataList)
		{
			
		}
*/		        
		public delegate Data TwoWayCallbackDelegate(IPAddress fromIP, byte[] buffer, int offset, int size);
		public delegate Data TwoWayRelayCallbackDelegate(IPAddress fromIP, IPAddress originalFromIP, Guid relayTicket, byte[] buffer, int offset, int size);

		private void receiveTwoWay_ReplyOrRelayReply(IPAddress fromIP, Guid overlayGuid, String strTwoWayTicket, String strExtractedData)
		{
			Console.WriteLine("TransportLayerCommunicator::receiveTwoWay CallType =TWO_WAY_REPLY || TWO_WAY_RELAY_REPLY");
			
			ISink sink;
			if(overlayRegistry.TryGetValue(overlayGuid, out sink))
			{
				
				TwoWayCallBackData twoWayCallbackData;
				if(twoWayCallBackRegistry.TryGetValue(strTwoWayTicket, out twoWayCallbackData))
				{
					Console.WriteLine("TransportLayerCommunicator::receiveTwoWay TwoWayTicket found in registry");
					if(twoWayCallbackData.callBack != null)
						sink.notifyTwoWayReplyReceived(fromIP, System.Text.Encoding.ASCII.GetBytes(strExtractedData), 0, strExtractedData.Length, twoWayCallbackData.callBack, twoWayCallbackData.appState);
						twoWayCallBackRegistry.Remove(strTwoWayTicket);
			
				}
				else
				{
					Console.WriteLine("TransportLayerCommunicator::receiveTwoWay TwoWayTicket not found");
					throw new Exception();
				}
			}
			else
			{
				Console.WriteLine("TransportLayerCommunicator::receiveTwoWay overlayGuid not found");
				throw new Exception();
			}
		}
	
		private void receiveTwoWay_Send(IPAddress fromIP, Guid overlayGuid, String strTwoWayTicket, String strExtractedData)
		{
			Console.WriteLine("TransportLayerCommunicator::receiveTwoWay CallType =TWO_WAY_SEND");
			ISink sink;
			if(overlayRegistry.TryGetValue(overlayGuid, out sink))
			{
				Data data;
				data = sink.notifyTwoWayMsg(fromIP, System.Text.Encoding.ASCII.GetBytes(strExtractedData), 0, strExtractedData.Length);
				if(data == null)
  					data = constructNullBakraData();
				
				byte[] compositeMsg = UtilityMethod.convertToTabSeparatedByteArray(false, strTwoWayTicket, Encoding.ASCII.GetString(data.buffer));  //WARNING: data.offset and data.size have not been considered here
				addToSockMsgQueue(fromIP, compositeMsg, 0, compositeMsg.Length, overlayGuid, null, null, CallType.TWO_WAY_REPLY);
			}
			else
			{
				Console.WriteLine("TransportLayerCommunicator::receiveTwoWay overlayGuid not found");
				throw new Exception();
			}
				
		}
		
		private Data constructNullBakraData()
		{
			Console.WriteLine("TransportLayerCommunicator::receiveTwoWay return data from notifyTwoWayMsg is null");
			String strNull = "NULLBAKRA";
			Data data = new Data();
			data.buffer = System.Text.Encoding.ASCII.GetBytes(strNull);
			data.offset = 0;
			data.size = strNull.Length;
			
			return data;
		}
		
		private void receiveTwoWay_RelaySend(IPAddress fromIP, Guid overlayGuid, String strTwoWayTicket, String strExtractedData)
		{
			Console.WriteLine("TransportLayerCommunicator::receiveTwoWay CallType =TWO_WAY_RELAY_SEND");
			ISink sink;
			if(overlayRegistry.TryGetValue(overlayGuid, out sink))
			{
				int endOfOriginalFromIP  = strExtractedData.IndexOf('\r');
				String strOriginalFromIP = strExtractedData.Substring(0, endOfOriginalFromIP);
				Console.Write("TransportLayerCommunicator::receiveTwoWay strOriginalFromIP = ");
				Console.WriteLine(strOriginalFromIP);
				
				IPAddress originalFromIP = UtilityMethod.convertStrToIP(strOriginalFromIP);
				relayTicketRegistry.Add(strTwoWayTicket, strOriginalFromIP);
				
				String realExtractedData = strExtractedData.Substring(endOfOriginalFromIP + 1, strExtractedData.Length - endOfOriginalFromIP -1);
				Console.Write("TransportLayerCommunicator::receiveTwoWay realExtractedData = ");  
				Console.WriteLine(realExtractedData);
				
				Data data;
				data = sink.notifyTwoWayRelayMsg(fromIP, originalFromIP, System.Text.Encoding.ASCII.GetBytes(realExtractedData), 0, realExtractedData.Length, new Guid(strTwoWayTicket));
				if(data != null)
				{
							byte[] compositeMsg = UtilityMethod.convertToTabSeparatedByteArray(false, strTwoWayTicket, Encoding.ASCII.GetString(data.buffer));  //WARNING: data.offset and data.size have not been considered here
					addToSockMsgQueue(originalFromIP, compositeMsg, 0, compositeMsg.Length, overlayGuid, null, null, CallType.TWO_WAY_REPLY);
				}
				else
				{
					//?? what should be done over here?
				}
			}
			else
			{
				Console.WriteLine("TransportLayerCommunicator::receiveTwoWay overlayGuid not found");
				throw new Exception();
			}
		}
		
		internal void receiveTwoWay(IPAddress fromIP, Guid overlayGuid, byte[] buffer, int offset, int size, String strCallType)
		{
			Console.WriteLine("TransportLayerCommunicator::receiveTwoWay ENTER");
			
			String strReceivedData = Encoding.ASCII.GetString(buffer, offset, size);
			Console.Write("TransportLayerCommunicator::receiveTwoWay strReceivedData.Length = ");
			Console.WriteLine(strReceivedData.Length);
			
			String strExtractedData = strReceivedData.Substring(37, strReceivedData.Length - 37);
			Console.Write("TransportLayerCommunicator::receiveTwoWay strExtractedData = ");
			Console.WriteLine(strExtractedData);
		
			String strTwoWayTicket = strReceivedData.Substring(0, 36);
			Console.Write("TransportLayerCommunicator::receiveTwoWay strTwoWayTicket = ");
			Console.WriteLine(strTwoWayTicket);
				
			if(String.Compare(strCallType, CallType.TWO_WAY_REPLY.ToString()) == 0
			|| String.Compare(strCallType, CallType.TWO_WAY_RELAY_REPLY.ToString()) == 0)
				receiveTwoWay_ReplyOrRelayReply(fromIP, overlayGuid, strTwoWayTicket, strExtractedData);
			else if(String.Compare(strCallType, CallType.TWO_WAY_SEND.ToString()) == 0)
				receiveTwoWay_Send(fromIP, overlayGuid, strTwoWayTicket, strExtractedData);
			else if(String.Compare(strCallType, CallType.TWO_WAY_RELAY_SEND.ToString()) == 0)
				receiveTwoWay_RelaySend(fromIP, overlayGuid, strTwoWayTicket, strExtractedData);
			else
				Console.WriteLine("TransportLayerCommunicator::receiveTwoWay unknown calltype");
		}

				
		internal void receive(IPAddress fromIP, Guid overlayGuid, byte[] buffer, int offset, int size)
		{
			Console.WriteLine("TransportLayerCommuicator::Receive ENTER");
			String s = Encoding.ASCII.GetString(buffer);
			Console.Write("TransportLayerCommuicator::Receive overlayGuid = ");
			Console.WriteLine(overlayGuid.ToString());
			Console.Write("TransportLayerCommuicator::Receive data received = ");
			Console.WriteLine(s);
			
			Console.Write("TransportLayerCommuicator::Receive overlayRegistry count = ");
			Console.WriteLine(overlayRegistry.Count);
			ISink sink;
			if(overlayRegistry.TryGetValue(overlayGuid, out sink))
				sink.notifyMsg(fromIP, buffer, offset, size);
			else
			{
				Console.WriteLine("TransportLayerCommuicator::Receive - could not find overlayGuid in overlayRegistry");
				throw new Exception();
			}

		}

		public class Forwarder
		{
	
		}
		
		public class Receiver
		{
			
		}

		
		//never call this directly
		private TransportLayerCommunicator()
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
			Console.Write("TransportLayerCommunicator::DEEPCHK overlayRegistry count = ");
			Console.WriteLine(overlayRegistry.Count);
			//IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
        	//	IPAddress ipAddress = ipHostInfo.AddressList[0];
#if SIM
			int iPortNo = System.Convert.ToInt16 (UtilityMethod.GetPort());
#else        	
        	int iPortNo = System.Convert.ToInt16 ("2334");
#endif
        	byte[] byteIP = {127, 0, 0, 1};
        	Console.Write("StartListening : IP=");
        	Console.Write(byteIP);
        	Console.Write(" Port=");
        	Console.WriteLine(iPortNo);
        	Console.Write("TransportLayerCommunicator::DEEPCHK overlayRegistry count = ");
			Console.WriteLine(overlayRegistry.Count);
			IPAddress ipAddress = new IPAddress(byteIP);

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
    	            Console.Write("TrnsportLayerComm::Waiting for a connection at port ");
    	            Console.WriteLine(iPortNo);
        	        Console.Write("TransportLayerCommunicator::DEEPCHK overlayRegistry count = ");
			Console.WriteLine(overlayRegistry.Count);
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
			
        	SockMsgQueue sockMsgQueue = new SockMsgQueue(handler, transportLayerCommunicator );
        	IPAddress IP = ((IPEndPoint)(handler.RemoteEndPoint)).Address;
        	try
			{
				transportLayerCommunicator.commRegistry.Add(IP, sockMsgQueue);
				socketState.sock = handler;
		        handler.BeginReceive( socketState.buffer, 0, socketState.buffer.Length, new SocketFlags(), new AsyncCallback(beginReceiveCallBack), socketState);	
				Console.WriteLine("TransportLayerCommunicator::beginAcceptCallback_forStartListening EXIT");
        	}
			catch(System.ArgumentException)
			{
				socketState.sock = handler;
		        handler.BeginReceive( socketState.buffer, 0, socketState.buffer.Length, new SocketFlags(), new AsyncCallback(beginReceiveCallBack), socketState);	
		        Console.WriteLine("TransportLayerCommunicator::beginAcceptCallback_forStartListening EXIT");
			}
			Console.WriteLine("TransportLayerCommunicator::beginAcceptCallback_forStartListening EXIT");
	
		}
		
		static private void beginReceiveCallBack(IAsyncResult result)
		{
			Console.WriteLine("TransportLayerCommunicator::beginReceiveCallBack ENTER");
			String content = String.Empty;
			SocketState socketState = ((SocketState)(result.AsyncState));
			Socket sock = socketState.sock;
		try
		{
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
		catch(Exception e)
		{
		/*	if(bytesRead > 0)
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
*/
		}
		
		
		
		}
		
#if SIM		
		enum MsgExtractionStatus
		{
			NOTHING_EXTRACTED,
			FROM_IP_EXTRACTED,
			TO_IP_EXTRACTED,
			CALLTYPE_EXTRACTED,
			OVERLAYGUID_EXTRACTED,
			MESSAGE_EXTRACTED
		}
		static private void notifyUpperLayer(String content, Socket fromSock, TransportLayerCommunicator transportLayerCommunicator)
		{
			Console.WriteLine("TransportLayerCommunicator::notifyUpperLayer ENTER");
			String[] split = content.Split(new char[] {'\0'});
			
			String strFromIP = null;
			String strToIP;
			String strCallType = null;
			String strOverlayGuid;
			byte[] byteOverlayGuid; 
			Guid overlayGuid = new Guid();;
			String strBuffer;
			byte[] byteBuffer;
			MsgExtractionStatus msgExtractionStatus = MsgExtractionStatus.NOTHING_EXTRACTED;  
			foreach (String s in split)
			{
				if(msgExtractionStatus == MsgExtractionStatus.NOTHING_EXTRACTED || msgExtractionStatus == MsgExtractionStatus.MESSAGE_EXTRACTED)
				{
					if(s.Length == 0)
						break;
					strFromIP = s;  	
					Console.Write("FromIP received: ");
					Console.WriteLine(s);
					msgExtractionStatus = MsgExtractionStatus.FROM_IP_EXTRACTED;
					
				}
				else if(msgExtractionStatus == MsgExtractionStatus.FROM_IP_EXTRACTED)
				{
					strToIP = s;	
					Console.WriteLine("ToIP received: ");
					Console.WriteLine(s);
					msgExtractionStatus = MsgExtractionStatus.TO_IP_EXTRACTED;
						
					
				}
				else if(msgExtractionStatus == MsgExtractionStatus.TO_IP_EXTRACTED)
				{
					strCallType = s;
					Console.WriteLine("haha 1");
					Console.WriteLine(s);
					Console.WriteLine(s.Length);
					//byteOverlayGuid = System.Text.Encoding.ASCII.GetBytes(strOverlayGuid);
					//overlayGuid = new Guid(s);
					msgExtractionStatus = MsgExtractionStatus.CALLTYPE_EXTRACTED;
				}
				else if(msgExtractionStatus == MsgExtractionStatus.CALLTYPE_EXTRACTED)
				{
					strOverlayGuid = s;
					Console.WriteLine("haha 1.5");
					Console.WriteLine(s);
					Console.WriteLine(s.Length);
					byteOverlayGuid = System.Text.Encoding.ASCII.GetBytes(strOverlayGuid);
					overlayGuid = new Guid(s);
					msgExtractionStatus = MsgExtractionStatus.OVERLAYGUID_EXTRACTED;
				}
				else if(msgExtractionStatus == MsgExtractionStatus.OVERLAYGUID_EXTRACTED)
				{
					strBuffer = s;
					Console.WriteLine("haha 2");
					Console.WriteLine(s);
					Console.WriteLine(s.Length);
					byteBuffer = System.Text.Encoding.ASCII.GetBytes(strBuffer);
					
					IPAddress fromIP = UtilityMethod.convertStrToIP(strFromIP);

					if(String.Compare(strCallType, CallType.ONE_WAY.ToString()) == 0)
						transportLayerCommunicator.receive(fromIP, overlayGuid, byteBuffer, 0, byteBuffer.Length);
					else
					{
						Console.WriteLine("TransportLayerCommunicator::notifyUpperLayer CallType IS NOT ONE_WAY");
						transportLayerCommunicator.receiveTwoWay(fromIP, overlayGuid, byteBuffer, 0, byteBuffer.Length, strCallType);
					}
					
					msgExtractionStatus = MsgExtractionStatus.MESSAGE_EXTRACTED;
				}
			}
		}
#else
        static private void notifyUpperLayer(String content, Socket fromSock, TransportLayerCommunicator transportLayerCommunicator)
        {
	        Console.WriteLine("TransportLayerCommunicator::notifyUpperLayer ENTER");
            Console.Write("TransportLayerCommunicator::DEEPCHK overlayRegistry count = ");
			Console.WriteLine(overlayRegistry.Count);
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
#endif

			internal class SocketState
			{
				public Socket sock;
				public byte[] buffer = new byte[5024];
				public StringBuilder concatenatedString = new StringBuilder();
				public TransportLayerCommunicator transportLayerCommunicator;
			}
		
		private void messageDispatch()
		{
			Console.WriteLine("TransportLayerCommunicator::messageDispatch ENTER");
			Console.Write("TransportLayerCommunicator::DEEPCHK overlayRegistry count = ");
			Console.WriteLine(overlayRegistry.Count);
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
#if SIM
		private static Dictionary<String, TransportLayerCommunicator> transportLayerCommunicatorRegistry = new Dictionary<String, TransportLayerCommunicator>();
		private static Object transportLayerCommunicatorRegistryLock = new Object();
#else
		//singleton
		private static TransportLayerCommunicator transportLayerCommunicator = null;
#endif		
		//need to take care of threading issues
		private static readonly Object transportLayerCommunicatorLock = new Object();
		
		public static TransportLayerCommunicator getRefTransportLayerCommunicator()
		{
			Console.WriteLine("TransportLayerCommunicator::getRefTransportLayerCommunicator ENTER");
#if SIM
			Console.Write("IPAddress=");
			Console.Write(UtilityMethod.GetLocalHostIP().ToString());
			Console.Write("    Port=");
			Console.WriteLine(UtilityMethod.GetPort());
			
			TransportLayerCommunicator transportLayerCommunicator;
			lock(transportLayerCommunicatorRegistryLock)
			{
				if(!(transportLayerCommunicatorRegistry.TryGetValue(UtilityMethod.GetLocalHostIP().ToString(), out transportLayerCommunicator)))
				{
					transportLayerCommunicator = new TransportLayerCommunicator();
					transportLayerCommunicatorRegistry.Add(UtilityMethod.GetLocalHostIP().ToString(), transportLayerCommunicator);
				}
			}
			return transportLayerCommunicator;
			
			
#else
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
#endif
		}

/*		public class TwoWayCallbackData
		{
			public int twoWayId;
			public TwoWayCallbackDelegate twoWayCallbackDelegate;
			public TwoWayRelayCallbackDelegate twoWayRelayCallbackDelegate;
			public Object appState;
			
		}
*/
		public class Data
		{
			public byte[] buffer;
			public int offset;
			public int size;
			
			public Data()
			{
				
			}
			
			public Data(byte[] buffer, int offset, int size)
			{
				this.buffer = buffer;
				this.offset = offset;
				this.size   = size;
			}
		
		}
		
		public class CallBackReturnData
		{
			public byte[] buffer;
			public int offset;
			public int size;
			public Object AppState;
		
		}

		
	}
}


