/************************************************************
* File Name: ChordProxyNode.cs
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Tashjik.Tier0;

using Tashjik.Common;

namespace Tashjik.Tier2
{
	/*********************************************
	* SEMANTICS: call methods on a remote machine
	*********************************************/
	internal class ChordProxyNode : ProxyNode, IChordNode
	{
		internal static ChordRealNode thisNode;
	
		//private Tashjik.Common.NodeBasic selfNodeBasic;
		//private Base.LowLevelComm lowLevelComm;

		private readonly int iPortNo = System.Convert.ToInt16 ("2334");
		private readonly Socket sock = null;

		//not necessary; ProxyNode will be added to the registry in ProxyController by ProxyController itself
		//BUT TO SEND MSGS, U NEED THE INTERFACE
		private ProxyNodeController proxyController;

		//needs to be synchronized
		private Dictionary <byte[], Tashjik.Common.AsyncCallback_Object > findSuccessorRegistry = new Dictionary<byte[], Tashjik.Common.AsyncCallback_Object >();
		private List<Tashjik.Common.AsyncCallback_Object> getPredecessorRegistry = new List<Tashjik.Common.AsyncCallback_Object>();
		private Dictionary <byte[], Tashjik.Common.AsyncCallback_Object > getDataRegistry = new Dictionary<byte[], Tashjik.Common.AsyncCallback_Object >();

		private void setProxyController(ProxyNodeController c)
		{
			//need to handle synchronised calls here
			//if(proxyController!=null) WTF is this? :O
			if(proxyController == null)
				proxyController = c;
		}	

/*
		private class Msg
		{
	
			public enum TransmitModeEnum
			{
				SEND,
				REPLY
			}
			public enum TypeEnum 
			{
				FIND_SUCCESSOR,
				GET_PREDECESSOR,
				NOTIFY,
				GET_DATA,
				PUT_DATA

			}

			private TypeEnum type;
			private Object parameter1;
			private Object parameter2;
			private Object returnValue;
			private TransmitModeEnum mode;

			public Msg(TypeEnum t, Object obj1, Object obj2)
			{
				type = t;
				parameter1 = obj1;
				parameter2 = obj2;
				returnValue = null;
				mode = TransmitModeEnum.SEND;
			}

			public TypeEnum getType()
			{	
				return type;
			}

			public Object getParameter1()
			{
				return parameter1;
			}

			public Object getParameter2()
			{
				return parameter2;
			}

			public TransmitModeEnum getMode()
			{
				return mode;
			}
	
			public Object getReturnValue()
			{
				return returnValue;
			}

	
			public void setReturnValue(Object obj)
			{
				returnValue = obj;
				//nulling obj1 and obj2 so that they dont hav to
				//travel back through the network unnecessarily
		
				//commenting out the nulling part for now
				//parameter1 = null;
				//parameter2 = null;
				mode = TransmitModeEnum.REPLY;
			}
		}
*/

		public override void notifyMsg(IPAddress fromIP, byte[] buffer, int offset, int size)
		{
			Console.WriteLine("ChordProxyNode::notifyMsg ENTER");
			String strReceivedData = Encoding.ASCII.GetString(buffer, offset, size);
			String[] split = strReceivedData.Split(new char[] {'\r'});
			if(String.Compare(split[0], "PREDECESSOR_NOTIFY") == 0)
			{
				thisNode.predecessorNotify((IChordNode)(proxyController.getProxyNode(UtilityMethod.convertStrToIP(split[1]))));
			}
		}
		
		public override Tashjik.Tier0.TransportLayerCommunicator.Data notifyTwoWayMsg(IPAddress fromIP, byte[] buffer, int offset, int size)
		{
			Console.WriteLine("ChordProxyNode::notifyTwoWayMsg ENTER");
			String strReceivedData = Encoding.ASCII.GetString(buffer, offset, size);
			String[] split = strReceivedData.Split(new char[] {'\r'});
			if(String.Compare(split[0], "GET_PREDECESSOR") == 0)
			{


				Tashjik.Tier0.TransportLayerCommunicator.Data data = new Tashjik.Tier0.TransportLayerCommunicator.Data();
				data.offset = 0;
				
				byte[] compositeMsg;
				IChordNode predecessor = thisNode.getPredecessor();
				if(predecessor == null)
				{
					Console.WriteLine("ChordProxyNode::notifyTwoWayMsg  predecessor is unknown");
					compositeMsg = UtilityMethod.convertToTabSeparatedByteArray(true, "GET_PREDECESSOR_REPLY", "UNKNOWN_PREDECESSOR");
				}
				else
				{
					Console.Write("ChordProxyNode::notifyTwoWayMsg predecessor =");
					Console.WriteLine(predecessor.getIP().ToString());
					compositeMsg = UtilityMethod.convertToTabSeparatedByteArray(true, "GET_PREDECESSOR_REPLY", predecessor.getIP().ToString());
				}
				
				data.buffer = compositeMsg;
				data.size = compositeMsg.Length;
				return data;
			
			}
			return null;
		}
		
		internal class IP_ChordProxyNode
		{
			public IPAddress IP;
			public ChordProxyNode chordProxyNode;
			public Guid ticket;
		}
		public override Tashjik.Tier0.TransportLayerCommunicator.Data notifyTwoWayRelayMsg(IPAddress fromIP, IPAddress originalFromIP, byte[] buffer, int offset, int size, Guid relayTicket)
		{
			Console.WriteLine("ChordProxyNode::notifyTwoWayRelayMsg ENTER");
			String strReceivedData = Encoding.ASCII.GetString(buffer, offset, size);
			String[] split = strReceivedData.Split(new char[] {'\r'});
			if(String.Compare(split[0], "FIND_SUCCESSOR") == 0)
			{
				Console.WriteLine("ChordProxyNode::notifyTwoWayRelayMsg message = FIND_SUCCESSOR");
				IP_ChordProxyNode iIP_ChordProxyNode = new IP_ChordProxyNode();
				iIP_ChordProxyNode.IP = originalFromIP;
				iIP_ChordProxyNode.chordProxyNode = this;
				iIP_ChordProxyNode.ticket = relayTicket;
				thisNode.beginFindSuccessor(System.Text.Encoding.ASCII.GetBytes(split[1]), (IChordNode)(proxyController.getProxyNode(originalFromIP)), new AsyncCallback(processFindSuccessor_notifyTwoWayRelayMsg_callback), iIP_ChordProxyNode, relayTicket);
			}
			return null;
		}
		
		static void processFindSuccessor_notifyTwoWayRelayMsg_callback(IAsyncResult ayncResult)
		{
			ChordCommon.IChordNode_Object iNode_Object = (ChordCommon.IChordNode_Object)(ayncResult.AsyncState);
			IChordNode successor = (iNode_Object.node);
			ChordProxyNode chordProxyNode = ((IP_ChordProxyNode)(iNode_Object.obj)).chordProxyNode;
			IPAddress originalFromIP = ((IP_ChordProxyNode)(iNode_Object.obj)).IP;
			Guid relayTicket = ((IP_ChordProxyNode)(iNode_Object.obj)).ticket;
			
			byte[] compositeMsg = UtilityMethod.convertToTabSeparatedByteArray(true, "FIND_SUCCESSOR_REPLY", successor.getIP().ToString());
			chordProxyNode.proxyController.sendMsgTwoWayRelay(chordProxyNode.proxyController.getProxyNode(originalFromIP), compositeMsg, 0, compositeMsg.Length, null, null, relayTicket);
		}
			
		public override void notifyTwoWayReplyReceived(IPAddress fromIP, byte[] buffer, int offset, int size, AsyncCallback originalRequestCallBack, Object originalAppState)
		{
			Console.WriteLine("ChordProxyNode::notifyTwoWayReplyReceived ENTER");
			String receivedData = Encoding.ASCII.GetString(buffer, offset, size);
			String[] split = receivedData.Split(new char[] {'\r'});
			if(String.Compare(split[0], "FIND_SUCCESSOR_REPLY") == 0)
			{
				Console.WriteLine("ChordProxyNode::notifyTwoWayReplyReceived FIND_SUCCESSOR_RECEIVED");
				String strSuccessorIP = split[1];
				Console.Write("ChordProxyNode::notifyTwoWayReplyReceived SuccessorIP = ");
				Console.WriteLine(strSuccessorIP);
				
				IPAddress successorIP = UtilityMethod.convertStrToIP(strSuccessorIP);

				ChordCommon.IChordNode_Object iNode_Object = new ChordCommon.IChordNode_Object();
				iNode_Object.node = (IChordNode)(proxyController.getProxyNode(successorIP));
				iNode_Object.obj = originalAppState;
				
				Tashjik.Common.ObjectAsyncResult objectAsyncResult = new Tashjik.Common.ObjectAsyncResult(iNode_Object, false, false);
				if(originalRequestCallBack != null)
					originalRequestCallBack(objectAsyncResult);
			}
			else if(String.Compare(split[0], "GET_PREDECESSOR_REPLY")==0)
			{
				Console.WriteLine("ChordProxyNode::notifyTwoWayReplyReceived GET_PREDECESSOR_RECEIVED");
				String strPredecessorIP = split[1];
				ChordCommon.IChordNode_Object iNode_Object = new ChordCommon.IChordNode_Object();
				
				if(String.Compare(strPredecessorIP, "UNKNOWN_PREDECESSOR")==0)
				{
					Console.Write("ChordProxyNode::notifyTwoWayReplyReceived PREDECESSOR_UNKNOWN");
					iNode_Object.node = null;
		
				}
				else
				{
					Console.Write("ChordProxyNode::notifyTwoWayReplyReceived PredecessorIP = ");
					Console.WriteLine(strPredecessorIP);
				
					IPAddress predecessorIP = UtilityMethod.convertStrToIP(strPredecessorIP);

					iNode_Object.node = (IChordNode)(proxyController.getProxyNode(predecessorIP));
				}
				iNode_Object.obj = originalAppState;
				
				Tashjik.Common.ObjectAsyncResult objectAsyncResult = new Tashjik.Common.ObjectAsyncResult(iNode_Object, false, false);
				if(originalRequestCallBack != null)
					originalRequestCallBack(objectAsyncResult);
			}
		}

		//need to make this asynchronous
		public /*override*/ void beginNotifyMsgRec(IPAddress fromIP, Object data, AsyncCallback notifyMsgRecCallBack, Object appState)
		{

		}





		public void beginFindSuccessor(IChordNode queryNode, IChordNode queryingNode, AsyncCallback findSuccessorCallBack, Object appState, Guid relayTicket)
		{
			beginFindSuccessor(queryNode.getHashedIP(), queryingNode, findSuccessorCallBack, appState, relayTicket);
		}

		public void beginFindSuccessor(byte[] queryHashedKey, IChordNode queryingNode, AsyncCallback findSuccessorCallBack, Object appState, Guid relayTicket)
		{
			Console.WriteLine("ChordProxyNode::beginFindSuccessor ENTER");

			byte[] compositeMsg = UtilityMethod.convertToTabSeparatedByteArray(true, "FIND_SUCCESSOR", Encoding.ASCII.GetString(queryHashedKey));
			Console.WriteLine("ChordProxyNode::beginFindSuccessor before sendMsg to proxyController");
			proxyController.sendMsgTwoWayRelay(this, compositeMsg, 0, compositeMsg.Length, findSuccessorCallBack, appState, relayTicket);
		}
		

		public void beginGetPredecessor(AsyncCallback getPredecessorCallBack, Object appState)
		{
			Console.WriteLine("ChordProxyNode::beginGetPredecessor ENTER");

			byte[] compositeMsg = UtilityMethod.convertToTabSeparatedByteArray(true, "GET_PREDECESSOR");

			Console.WriteLine("ChordProxyNode::beginGetPredecessor before sendMsg to proxyController");
			proxyController.sendMsgTwoWay(this, compositeMsg, 0, compositeMsg.Length, getPredecessorCallBack, appState);

		}

		public void beginPredecessorNotify(IChordNode possiblePred, AsyncCallback notifyCallBack, Object appState)
		{
			Console.WriteLine("ChordProxyNode::beginNotify ENTER");

			byte[] compositeMsg = UtilityMethod.convertToTabSeparatedByteArray(true, "PREDECESSOR_NOTIFY", possiblePred.getIP().ToString());

			Console.WriteLine("ChordProxyNode::beginGetPredecessor before sendMsg to proxyController");
			proxyController.sendMsg(this, compositeMsg, 0, compositeMsg.Length, notifyCallBack, appState);

			
	
		}
		
		//to be implemented
		public void predecessorNotify(IChordNode possiblePred)
		{
			
		}

		public ChordProxyNode(IPAddress ip, ProxyNodeController proxyController) : base(ip)
		{
		//	lowLevelComm = Base.LowLevelComm.getRefLowLevelComm();
//			selfNodeBasic = new Tashjik.Common.NodeBasic(ip);
			setProxyController(proxyController);
		}

		public void beginPing(AsyncCallback pingCallBack, Object appState)
		{
			Console.WriteLine("Chord::ChordProxyNode::Engine  beginPing ENTER");
			Tashjik.Common.Bool_Object bool_object = new Tashjik.Common.Bool_Object();

			IAsyncResult res;
			try
			{
				Socket sock = Tashjik.Common.UtilityMethod.CreateSocketConnection(selfNodeBasic.getIP());
				sock.Close();
				bool_object.b = true;
				bool_object.obj = appState;
				if(!(pingCallBack==null))
				{
					res = new Tashjik.Common.Bool_ObjectAsyncResult(bool_object, true, true);
					pingCallBack(res);
				}
			}
			catch(SocketException)
			{
	
				bool_object.b = false;
				bool_object.obj = appState;
				if(!(pingCallBack==null))
				{
					res = new Tashjik.Common.Bool_ObjectAsyncResult(bool_object, true, true);
					pingCallBack(res);
				}
			}

		}

		/* public void ping()
		{
			try
			{
				Socket sock = Common.UtilityMethod.CreateSocketConnection(selfNodeBasic.getIP());
				sock.Close();

				return true;
			}
			catch(SocketException se)
			{
				return false;
			}

		}
		*/

		public void beginGetData(byte[] byteKey, AsyncCallback getDataCallBack, Object appState)
		{
	/*		Tashjik.Common.AsyncCallback_Object asyncCallback_Object = new Tashjik.Common.AsyncCallback_Object();
			asyncCallback_Object.callBack = getDataCallBack;
			asyncCallback_Object.obj = appState;
			getDataRegistry.Add(byteKey, asyncCallback_Object);
			Msg msg = new Msg(Msg.TypeEnum.GET_DATA, (Object)byteKey, null);
			List<Msg> msgList = new List<Msg>();
			msgList.Add(msg);
			proxyController.sendMsg((Object)msgList, this);
	*/	}
		

		public void beginPutData(byte[] key, byte[] data, int offset, int size, AsyncCallback putDataCallBack, Object appState)
		{
			
		}
		
		public void beginPutData(byte[] byteKey, Stream data, UInt64 dataLength, AsyncCallback putDataCallBack, Object appState)
		{
	/*		Msg msg = new Msg(Msg.TypeEnum.PUT_DATA, (Object)byteKey, (Object)data);
			List<Msg> msgList = new List<Msg>();
			msgList.Add(msg);
			proxyController.sendMsg((Object)msgList, this);
			//Tashjik.Chord.Common.ByteKey_Data byteKey_Data = new Tashjik.Chord.Common.ByteKey_Data();
			//byteKey_Data.byteKey = byteKey;
			//byteKey_Data.data = null;
			//IAsyncResult res = new Tashjik.Chord.Common.ByteKey_DataAsyncResult(byteKey_Data, true, true);
	
			//need to wait till reply comes back
			//or maybe not; have to sort this out
			if(!(putDataCallBack==null))
			{
				IAsyncResult res = new Tashjik.Common.ObjectAsyncResult(appState, true, true);
				putDataCallBack(res);
			}
	*/	}

	}
}
