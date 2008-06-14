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

namespace Tashjik.Tier2.Chord
{
	internal class Server : IOverlay
	{
		private readonly Guid guid;
		internal readonly Node thisNode;
	
		public Guid getGuid()
		{
			return guid;
		}

		public void shutdown()
		{
			
		}
		public Server()
		{
			guid = System.Guid.NewGuid();
			thisNode = new Node();
			NodeProxy.thisNode = thisNode;
		}


		/*
		public Tashjik.Common.Data getData(String key)
		{
			byte[] byteKey = System.Text.Encoding.ASCII.GetBytes(key);
			INode successor = thisNode.findSuccessor(byteKey, thisNode);
			return successor.getData(byteKey);
		}



		public void putData(String key, Tashjik.Common.Data data)
		{
			byte[] byteKey = System.Text.Encoding.ASCII.GetBytes(key);
			INode successor = thisNode.findSuccessor(byteKey, thisNode);
			successor.putData(byteKey, data);
		}
		*/
		public void beginGetData(String key, AsyncCallback getDataCallBack, Object appState)
		{
			byte[] byteKey = System.Text.Encoding.ASCII.GetBytes(key);

			Tashjik.Common.ByteArray_AsyncCallback_Object thisAppState = new Tashjik.Common.ByteArray_AsyncCallback_Object();
			thisAppState.byteArray = byteKey;
			thisAppState.callBack = getDataCallBack;
			thisAppState.obj = appState;

			AsyncCallback findSuccessorCallBack = new AsyncCallback(processFindSuccessorForGetData);
			thisNode.beginFindSuccessor(byteKey, thisNode, findSuccessorCallBack, thisAppState);

		}

		static void processFindSuccessorForGetData(IAsyncResult result)
		{
			Common.INode_Object iNode_Object = (Common.INode_Object)(result.AsyncState);
			INode successor = iNode_Object.node;
			Tashjik.Common.ByteArray_AsyncCallback_Object recAppState = (Tashjik.Common.ByteArray_AsyncCallback_Object)(iNode_Object.obj);
		
			byte[] queryByteKey = recAppState.byteArray;
			Object origAppState = recAppState.obj;
			AsyncCallback callBack = recAppState.callBack;
	
			successor.beginGetData(queryByteKey, callBack, origAppState);
	
		}
	


		public void beginPutData(String key, Tashjik.Common.Data data, AsyncCallback putDataCallBack, Object appState)
		{
			byte[] byteKey = System.Text.Encoding.ASCII.GetBytes(key);
		
			Tashjik.Common.ByteArray_Data_AsyncCallback_Object thisAppState = new Tashjik.Common.ByteArray_Data_AsyncCallback_Object();

			thisAppState.byteArray = byteKey;
			thisAppState.callBack = putDataCallBack;
			thisAppState.data = data;
			thisAppState.obj = appState;

			AsyncCallback findSuccessorCallBack = new AsyncCallback(processFindSuccessorForPutData);
			thisNode.beginFindSuccessor(byteKey, thisNode, findSuccessorCallBack, thisAppState);
		}
	
		static void processFindSuccessorForPutData(IAsyncResult result)
		{
			Common.INode_Object iNode_Object = (Common.INode_Object)(result.AsyncState);
			INode successor = iNode_Object.node;
			Tashjik.Common.ByteArray_Data_AsyncCallback_Object recAppState = (Tashjik.Common.ByteArray_Data_AsyncCallback_Object)(iNode_Object.obj);

			byte[] queryByteKey = recAppState.byteArray;
			Object origAppState = recAppState.obj;
			AsyncCallback callBack = recAppState.callBack;
			Tashjik.Common.Data data = recAppState.data;
	
			successor.beginPutData(queryByteKey, data, callBack, origAppState);
		

		}

		public void joinOther(IPAddress IP, Guid giud)
		{
			//thisNode.initiateJoin(IP, giud);
		}
	}

}
