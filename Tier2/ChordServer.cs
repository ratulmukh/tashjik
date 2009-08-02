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
using System.Collections.Generic;

namespace Tashjik.Tier2
{
	public class ChordServer : OverlayServer
	{
		private readonly Guid guid;
		internal readonly ChordRealNode thisNode;
		internal readonly static ProxyNodeController chordproxyNodeController = new ProxyNodeController(new  ProxyNodeController.CreateProxyNodeDelegate(createChordProxyNode));
	
		public override Guid getGuid()
		{
			return new Guid(guid.ToByteArray());
		}

		public override void shutdown()
		{
			
		}
		public ChordServer() : base(new  ProxyNodeController.CreateProxyNodeDelegate(createChordProxyNode))
		{
			guid = System.Guid.NewGuid();
			thisNode = new ChordRealNode();
			ChordProxyNode.thisNode = thisNode;
		}
		
		public ChordServer(IPAddress joinOtherIP, Guid joinOtherGuid, ProxyNodeController proxyController) : base(new  ProxyNodeController.CreateProxyNodeDelegate(createChordProxyNode))
		{
/*			guid = joinOtherGuid;
			Node joinOtherNode = new ProxyNode(joinOtherIP, proxyController);
			thisNode = new Node(joinOtherNode);
*/
		}

		private static ProxyNode createChordProxyNode(IPAddress IP)
		{
			return new ChordProxyNode(IP, chordproxyNodeController);
		}
		
		internal IChordNode getChordProxyNode(IPAddress IP)
		{      
			return (IChordNode)(base.getProxyNode(IP));
		}

		/*
		public Tashjik.Common.Data getData(String key)
		{
			byte[] byteKey = System.Text.Encoding.ASCII.GetBytes(key);
			Node successor = thisNode.findSuccessor(byteKey, thisNode);
			return successor.getData(byteKey);
		}



		public void putData(String key, Tashjik.Common.Data data)
		{
			byte[] byteKey = System.Text.Encoding.ASCII.GetBytes(key);
			Node successor = thisNode.findSuccessor(byteKey, thisNode);
			successor.putData(byteKey, data);
		}
		*/
		public override void beginGetData(byte[] key, AsyncCallback getDataCallBack, Object appState)
		{
			Console.WriteLine("ChordServer::beginGetData ENTER");
			
			Stack<Object> thisAppState = new Stack<Object>();
			thisAppState.Push(key);
			thisAppState.Push(getDataCallBack);
			thisAppState.Push(appState);
			
			AsyncCallback findSuccessorCallBack = new AsyncCallback(processFindSuccessorForGetData);
			thisNode.beginFindSuccessor(key, thisNode, findSuccessorCallBack, thisAppState);

		}

		static void processFindSuccessorForGetData(IAsyncResult result)
		{
			Console.WriteLine("ChordServer::processFindSuccessorForGetData ENTER");
			ChordCommon.IChordNode_Object iNode_Object = (ChordCommon.IChordNode_Object)(result.AsyncState);
			IChordNode successor = iNode_Object.node;
			Stack<Object> recAppState = (Stack<Object>)(iNode_Object.obj);	
			
			Object origAppState = recAppState.Pop();
			AsyncCallback getDataCallBack = (AsyncCallback)(recAppState.Pop());
			byte[] queryKey = (byte[])(recAppState.Pop());
			
			successor.beginGetData(queryKey, getDataCallBack, origAppState);
			
			Console.WriteLine("ChordServer::processFindSuccessorForGetData EXIT");
		}
	


		public override void beginPutData(byte[] key, byte[] data, int offset, int size, AsyncCallback putDataCallBack, Object appState)
		{
			Console.WriteLine("Chord::BeginPutData ENTER");
			Stack<Object> thisAppState = new Stack<Object>();
			thisAppState.Push(key);
			thisAppState.Push(data);
			thisAppState.Push(offset);
			thisAppState.Push(size);
			thisAppState.Push(putDataCallBack);
			thisAppState.Push(appState);
			
			
			thisNode.beginFindSuccessor(key, thisNode, new AsyncCallback(processFindSuccessorForPutData), thisAppState);
            Console.WriteLine("Chord::BeginPutData aync EXIT");
		}
		

		static void processFindSuccessorForPutData(IAsyncResult result)
		{
			Console.WriteLine("ChordServer::processFindSuccessorForPutData ENTER");
			ChordCommon.IChordNode_Object iNode_Object = (ChordCommon.IChordNode_Object)(result.AsyncState);
			IChordNode successor = iNode_Object.node;
			Stack<Object> recAppState = (Stack<Object>)(iNode_Object.obj);
			
			Object origAppState = recAppState.Pop();
			AsyncCallback putDataCallBack = (AsyncCallback)(recAppState.Pop());
			int size = (int)(recAppState.Pop());
			int offset = (int)(recAppState.Pop());
			byte[] data = (byte[])(recAppState.Pop());
			byte[] key = (byte[])(recAppState.Pop());
			
			Console.WriteLine("ChordServer::processFindSuccessorForPutData calling beginPutData");
			successor.beginPutData(key, data, offset, size, putDataCallBack, origAppState);
			
			Console.WriteLine("ChordServer::processFindSuccessorForPutData EXIT");
		}

		public void joinOther(IPAddress IP, Guid giud)
		{
			//thisNode.initiateJoin(IP, giud);
		}
	}

}
