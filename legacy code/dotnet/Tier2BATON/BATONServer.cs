﻿/************************************************************
* File Name: BATONServer.cs
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


//using System;
//using System.Net;
//using System.Net.Sockets;
//
//namespace Tashjik.Tier2
//{
//
//	public class BATONServer : Tier2.Common.Server, IOverlay
//	{
//		private  readonly Guid guid;
//		internal readonly BATONNode thisNode;
//		
//		public BATONServer()
//		{
//			guid = System.Guid.NewGuid();
//			thisNode = new BATONNode();
//			//ProxyNode.thisNode = thisNode;
//		}
//		
//		public BATONServer(IPAddress joinOtherIP, Guid joinOtherGuid, Tier2.Common.ProxyController proxyController)
//		{
//			guid = joinOtherGuid;
//			IBATONNode joinOtherNode = new BATONProxyNode(joinOtherIP, proxyController);
//			thisNode = new BATONNode(joinOtherNode);
//		}
//			
//		public override Guid getGuid()
//		{
//			return new Guid(guid.ToByteArray());
//		}
//
//		//Common.Data getData(String key);
//		//void putData(String key, Common.Data data);
//
//		public override void beginGetData(String key, AsyncCallback getDataCallBack, Object appState)
//		{
//			
//		}
//		public override void beginPutData(String key, Tashjik.Common.Data data, AsyncCallback putDataCallBack, Object appState)
//		{
//			
//		}
//		
//		public override void shutdown()
//		{
//			
//		}
//		
//		
//	}
//}
