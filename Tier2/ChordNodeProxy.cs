﻿/************************************************************
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
using System.Collections;
using System.Collections.Generic;

namespace Tashjik.Tier2
{
	/*********************************************
	* SEMANTICS: call methods on a remote machine
	*********************************************/
	internal class ChordNodeProxy : Tier2.Common.NodeProxy, IChordNode, Tier2.Common.ProxyController.IProxy
	{
		internal static ChordNode thisNode;
	
		private Tashjik.Common.NodeBasic selfNodeBasic;
		private Base.LowLevelComm lowLevelComm;

		private readonly int iPortNo = System.Convert.ToInt16 ("2334");
		private readonly Socket sock = null;

		//not necessary; NodeProxy will be added to the registry in ProxyController by ProxyController itself
		//BUT TO SEND MSGS, U NEED THE INTERFACE
		private Tier2.Common.ProxyController proxyController;

		//needs to be synchronized
		private Dictionary<byte[], Tashjik.Common.AsyncCallback_Object > findSuccessorRegistry = new Dictionary<byte[], Tashjik.Common.AsyncCallback_Object >();
		private List<Tashjik.Common.AsyncCallback_Object> getPredecessorRegistry = new List<Tashjik.Common.AsyncCallback_Object>();
		private Dictionary<byte[], Tashjik.Common.AsyncCallback_Object > getDataRegistry = new Dictionary<byte[], Tashjik.Common.AsyncCallback_Object >();

		public override void setProxyController(Tier2.Common.ProxyController c)
		{
			//need to handle synchronised calls here
			if(proxyController!=null)
			proxyController = c;
		}	


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

		class IChordNode_INode_Msg
		{
			public IChordNode node1;
			public IChordNode node2;
			public Msg msg;
		}

		void processGetDataForNotifyMsgRec(IAsyncResult result)
		{
			Tashjik.Common.Data_Object data_Object = (Tashjik.Common.Data_Object)(result.AsyncState);
			Tashjik.Common.Data data = data_Object.data;

			IChordNode_INode_Msg thisAppState = (IChordNode_INode_Msg)(data_Object.obj);
			ChordNodeProxy iNode = (ChordNodeProxy)(thisAppState.node2);

			Msg msg = thisAppState.msg;
			msg.setReturnValue((Object)data);
			List<Msg> returnMsgList = new List<Msg>();
			returnMsgList.Add(msg);
			proxyController.sendMsg((Object)returnMsgList, iNode);
		}

		void processGetPredecessorForNotifyMsgRec(IAsyncResult result)
		{
			ChordCommon.ByteKey_IChordNode byteKey_INode = (ChordCommon.ByteKey_IChordNode) (result.AsyncState);
			IChordNode predecessor = byteKey_INode.node;
			IChordNode_INode_Msg thisAppState = (IChordNode_INode_Msg)(byteKey_INode.obj);
			IChordNode thisNode = thisAppState.node1;
			ChordNodeProxy iNode = (ChordNodeProxy)(thisAppState.node2);
			if(predecessor==thisNode)
			{
				//no need to add successorProxyForm to registry
				//since it is actually the 'thisNode'
				//we are creating a NodeProxy of thisNode
				//because it needs to be transferred back
				IPAddress predecessorIP = predecessor.getIP();
				predecessor = new ChordNodeProxy(predecessorIP, proxyController);
			}
			Msg msg = thisAppState.msg;
			msg.setReturnValue((Object)predecessor);
			List<Msg> returnMsgList = new List<Msg>();
			returnMsgList.Add(msg);
			proxyController.sendMsg((Object)returnMsgList, iNode);
		}

		void processFindSuccessorForNotifyMsgRec(IAsyncResult result)
		{
			ChordCommon.IChordNode_Object iNode_Object = (ChordCommon.IChordNode_Object) (result.AsyncState);
			IChordNode successor = iNode_Object.node;
			IChordNode_INode_Msg thisAppState = (IChordNode_INode_Msg)(iNode_Object.obj);
		
			IChordNode thisNode = thisAppState.node1;
			ChordNodeProxy iNode = (ChordNodeProxy)(thisAppState.node2);

			if(successor==thisNode)
			{
				//no need to add successorProxyForm to registry
				//since it is actually the 'thisNode'
				//we are creating a NodeProxy of thisNode
				//because it needs to be transferred back
				IPAddress successorIP = successor.getIP();
				successor = new ChordNodeProxy(successorIP, proxyController);
			}

			Msg msg = thisAppState.msg;
			msg.setReturnValue((Object)successor);
			List<Msg> returnMsgList = new List<Msg>();
			returnMsgList.Add(msg);
			proxyController.sendMsg((Object)returnMsgList, iNode);
	
		}	

		//need to make this asynchronous
		public override void beginNotifyMsgRec(IPAddress fromIP, Object data, AsyncCallback notifyMsgRecCallBack, Object appState)
		{
			List<Msg> dataList = (List<Msg>)data;
			List<Msg> returnMsgList = new List<Msg>();
			foreach(Msg msg in dataList)
			{
		
				if(msg.getMode()==Msg.TransmitModeEnum.SEND)
				{
					if(msg.getType()==Msg.TypeEnum.FIND_SUCCESSOR)
					{
						byte[] queryHashedKey = System.Text.Encoding.ASCII.GetBytes((String)msg.getParameter1());
						IChordNode queryingNode = (IChordNode)msg.getParameter2();
						//INode successor = thisNode.findSuccessor(queryHashedKey, queryingNode);

						IChordNode_INode_Msg iNode_Msg = new IChordNode_INode_Msg();
						iNode_Msg.node1 = thisNode;
						iNode_Msg.node2 = this;
						iNode_Msg.msg = msg;

						AsyncCallback findSuccessorCallBack = new AsyncCallback(processFindSuccessorForNotifyMsgRec);
						thisNode.beginFindSuccessor(queryHashedKey, queryingNode, findSuccessorCallBack, iNode_Msg);
						/* if(successor==thisNode)
						{
							//no need to add successorProxyForm to registry
							//since it is actually the 'thisNode'
							//we are creating a NodeProxy of thisNode
							//because it needs to be transferred back
							IPAddress successorIP = successor.getIP();
							successor = new NodeProxy(successorIP);
						}
						msg.setReturnValue((Object)successor);
						returnMsgList.Add(msg);
						*/
					}
					else if(msg.getType()==Msg.TypeEnum.GET_PREDECESSOR)
					{
						IChordNode_INode_Msg iNode_Msg = new IChordNode_INode_Msg();
						iNode_Msg.node1 = thisNode;
						iNode_Msg.node2 = this;
						iNode_Msg.msg = msg;

						AsyncCallback getPredecessorCallBack = new AsyncCallback(processGetPredecessorForNotifyMsgRec);
						thisNode.beginGetPredecessor(getPredecessorCallBack, iNode_Msg);
						//INode predecessor = thisNode.getPredecessor();
		
						/*if(predecessor==thisNode)
						{
							//no need to add successorProxyForm to registry
							//since it is actually the 'thisNode'
							//we are creating a NodeProxy of thisNode
							//because it needs to be transferred back
							IPAddress predecessorIP = predecessor.getIP();
							predecessor = new NodeProxy(predecessorIP);
						}
						msg.setReturnValue((Object)predecessor);
						returnMsgList.Add(msg);
						*/
					}
					else if(msg.getType()==Msg.TypeEnum.NOTIFY)
						//thisNode.notify((INode)msg.getParameter1());
						thisNode.beginNotify((IChordNode)msg.getParameter1(), null, null);
					else if(msg.getType()==Msg.TypeEnum.GET_DATA)
					{
						IChordNode_INode_Msg iNode_Msg = new IChordNode_INode_Msg();
						iNode_Msg.node1 = null;
						iNode_Msg.node2 = this;
						iNode_Msg.msg = msg;

						byte[] byteKey = System.Text.Encoding.ASCII.GetBytes((String)msg.getParameter1());
						AsyncCallback getDataCallBack = new AsyncCallback(processGetDataForNotifyMsgRec);
						thisNode.beginGetData(byteKey, getDataCallBack, iNode_Msg);
						//msg.setReturnValue((Object)data1);
						//returnMsgList.Add(msg)
					}
					else if(msg.getType()==Msg.TypeEnum.PUT_DATA)
					{
						byte[] byteKey = System.Text.Encoding.ASCII.GetBytes((String)msg.getParameter1());
						Tashjik.Common.Data data1 = (Tashjik.Common.Data)msg.getParameter1();
						thisNode.beginPutData(byteKey, data1, null, null);
					}
					//COMMENTING THIS CALL, SINCE IT ISMADE IN AsyncCallBacks
					//need to forward returnMsgList back to fromIP
					//on the same path as forwarding msgs in SEND mode
					//proxyController.sendMsg((Object)returnMsgList, this);
				}
				else if(msg.getMode()==Msg.TransmitModeEnum.REPLY)
				{
					//firsr tough nut cracked :P
					if(msg.getType()==Msg.TypeEnum.FIND_SUCCESSOR)
					{
						byte[] queryHashedKey = (byte[])msg.getParameter1();
						IChordNode successor = (IChordNode)msg.getReturnValue();

						//AsyncCallback findSuccessorCallBack;
						Tashjik.Common.AsyncCallback_Object asyncCallback_Object;
						if(findSuccessorRegistry.TryGetValue(queryHashedKey, out asyncCallback_Object))
						{
							ChordCommon.IChordNode_Object iNode_Object = new ChordCommon.IChordNode_Object();
							iNode_Object.node = successor;
							iNode_Object.obj = asyncCallback_Object.obj;

							IAsyncResult findSuccessorResult = new ChordCommon.IChordNode_ObjectAsyncResult(iNode_Object, false, true);
							findSuccessorRegistry.Remove(queryHashedKey);
							asyncCallback_Object.callBack(findSuccessorResult);
						}
					}
					else if(msg.getType()==Msg.TypeEnum.GET_PREDECESSOR)
					{
						/*INode predecessor = (INode)msg.getParameter1();
						IAsyncResult getPredecessorResult = new Chord.Common.INodeAsyncResult(predecessor, false, true);
						foreach(AsyncCallback getPredecessorCallBack in getPredecessorRegistry)
						getPredecessorCallBack(getPredecessorResult);
						*/

						IChordNode predecessor = (IChordNode)msg.getParameter1();
						foreach(Tashjik.Common.AsyncCallback_Object asyncCallback_Object in getPredecessorRegistry)
						{
							ChordCommon.IChordNode_Object iNode_Object = new ChordCommon.IChordNode_Object();
							iNode_Object.node = predecessor;
							iNode_Object.obj = asyncCallback_Object.obj;
							IAsyncResult getPredecessorResult = new ChordCommon.IChordNode_ObjectAsyncResult(iNode_Object, false, true);
							asyncCallback_Object.callBack(getPredecessorResult);
						}
	
					}
					else if(msg.getType()==Msg.TypeEnum.GET_DATA)
					{
						byte[] byteKey = (byte[])msg.getParameter1();
						Tashjik.Common.Data data1 = (Tashjik.Common.Data)msg.getReturnValue();
						Tashjik.Common.AsyncCallback_Object asyncCallback_Object;
						if(getDataRegistry.TryGetValue(byteKey, out asyncCallback_Object))
						{
	
							Tashjik.Common.Data_Object data_Object = new Tashjik.Common.Data_Object();
							data_Object.data = data1;
							data_Object.obj = asyncCallback_Object.obj;

							IAsyncResult getDataResult = new Tashjik.Common.Data_ObjectAsyncResult(data_Object, false, true);
							getDataRegistry.Remove(byteKey);
							asyncCallback_Object.callBack(getDataResult);
						}	
	
					}


				}
			}
			if(!(notifyMsgRecCallBack==null))
			{
				if(!(notifyMsgRecCallBack==null))
				{
					IAsyncResult res = new Tashjik.Common.ObjectAsyncResult(appState, true, true);
					notifyMsgRecCallBack(res);
				}
			}	
	

		}







		public override byte[] getHashedIP()
		{
			return selfNodeBasic.getHashedIP();
		}

		public override IPAddress getIP()
		{
			return selfNodeBasic.getIP();
		}

		public override void setIP(IPAddress ip)
		{
			selfNodeBasic.setIP(ip);
		}
		/*
		public INode findSuccessor(INode queryNode, INode queryingNode)
		{
			return findSuccessor(queryNode.getHashedIP(), queryingNode);
		}

		public INode findSuccessor(byte[] queryHashedKey, INode queryingNode)
		{
			Msg msg = new Msg(Msg.TypeEnum.FIND_SUCCESSOR, (Object)queryHashedKey, (Object)queryingNode);
			List<Msg> msgList = new List<Msg>();
			msgList.Add(msg);
			proxyController.sendMsg((Object)msgList, this);
			//box byte[] to String
	
			// if(queryingNode==
			// Msg msg = new Msg(Msg.TypeEnum.FIND_SUCCESSOR, queryHashedKey, queryingNode);
			return new Node();
			//relay this call to the actual node via forwarder
		}
		*/
		public void beginFindSuccessor(IChordNode queryNode, IChordNode queryingNode, AsyncCallback findSuccessorCallBack, Object appState)
		{
			beginFindSuccessor(queryNode.getHashedIP(), queryingNode, findSuccessorCallBack, appState);
		}

		public void beginFindSuccessor(byte[] queryHashedKey, IChordNode queryingNode, AsyncCallback findSuccessorCallBack, Object appState)
		{
			Tashjik.Common.AsyncCallback_Object thisAppState = new Tashjik.Common.AsyncCallback_Object();
			thisAppState.callBack = findSuccessorCallBack;
			thisAppState.obj = appState;
			findSuccessorRegistry.Add(queryHashedKey, thisAppState);
			Msg msg = new Msg(Msg.TypeEnum.FIND_SUCCESSOR, (Object)queryHashedKey, (Object)queryingNode);
			List<Msg> msgList = new List<Msg>();
			msgList.Add(msg);
			proxyController.sendMsg((Object)msgList, this);
		}


		/* public INode getPredecessor()
		{
			return new Node();
			//relay this call to the actual node via forwarder
		}
		*/
		public void beginGetPredecessor(AsyncCallback getPredecessorCallBack, Object appState)
		{
			Tashjik.Common.AsyncCallback_Object thisAppState = new Tashjik.Common.AsyncCallback_Object();
			thisAppState.callBack = getPredecessorCallBack;
			thisAppState.obj = appState;
			getPredecessorRegistry.Add(thisAppState);
			Msg msg = new Msg(Msg.TypeEnum.GET_PREDECESSOR, null, null);
			List<Msg> msgList = new List<Msg>();
			msgList.Add(msg);
			proxyController.sendMsg((Object)msgList, this);
		}

		public void beginNotify(IChordNode possiblePred, AsyncCallback notifyCallBack, Object appState)
		{
			Msg msg = new Msg(Msg.TypeEnum.NOTIFY, (Object)possiblePred, null);
			List<Msg> msgList = new List<Msg>();
			msgList.Add(msg);
			proxyController.sendMsg((Object)msgList, this);
		
			if(!(notifyCallBack==null))
			{
				if(!(notifyCallBack==null))
				{
					//should not do this; rather should wait till node really has been notified
					IAsyncResult res = new Tashjik.Common.ObjectAsyncResult(appState, true, true);
					notifyCallBack(res);
				}
			}
	
		}	

		public ChordNodeProxy(IPAddress ip, Tier2.Common.ProxyController proxyController)
		{
			lowLevelComm = Base.LowLevelComm.getRefLowLevelComm();
			selfNodeBasic = new Tashjik.Common.NodeBasic(ip);
			setProxyController(proxyController);
		}

		public void beginPing(AsyncCallback pingCallBack, Object appState)
		{
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
			catch(SocketException se)
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
			Tashjik.Common.AsyncCallback_Object asyncCallback_Object = new Tashjik.Common.AsyncCallback_Object();
			asyncCallback_Object.callBack = getDataCallBack;
			asyncCallback_Object.obj = appState;
			getDataRegistry.Add(byteKey, asyncCallback_Object);
			Msg msg = new Msg(Msg.TypeEnum.GET_DATA, (Object)byteKey, null);
			List<Msg> msgList = new List<Msg>();
			msgList.Add(msg);
			proxyController.sendMsg((Object)msgList, this);
		}
		/*
		public Tashjik.Common.Data getData(byte[] byteKey)
		{
			//need to change this
			return new Tashjik.Common.Data();
		}
		*/


		/* public void putData(byte[] byteKey, Tashjik.Common.Data data)
		{
			Msg msg = new Msg(Msg.TypeEnum.PUT_DATA, (Object)byteKey, (Object)data);
			List<Msg> msgList = new List<Msg>();
			msgList.Add(msg);
			proxyController.sendMsg((Object)msgList, this);
		}
		*/

		public void beginPutData(byte[] byteKey, Tashjik.Common.Data data, AsyncCallback putDataCallBack, Object appState)
		{
			Msg msg = new Msg(Msg.TypeEnum.PUT_DATA, (Object)byteKey, (Object)data);
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
		}

	}
}