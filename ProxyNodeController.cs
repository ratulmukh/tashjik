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
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace Tashjik
{
	
	public class ProxyNodeController : IProxyNodeController , Tier0.TransportLayerCommunicator.ISink
	{
	
		private readonly ProxyNodeRegistry proxyNodeRegistry = new ProxyNodeRegistry();
		
		/*
		public enum TransmitModeEnum
		{
			SEND,
			REPLY
		}
		
		public struct MsgHandlerInfo
		{
			TransmitModeEnum mode;
			int msgType;
			
		}

		
		public void addMessageHandler(TransmitModeEnum mode)
		{
			
		}
*/
		class ProxyNodeRegistry
		{
			List<ProxyNodeData> proxyDataList;
	
			class ProxyNodeData
			{
				public ProxyNode proxyNode;
				public Queue<Object> msgQueue;
	
				public ProxyNodeData(ProxyNode n)
				{
					proxyNode = n;
					msgQueue = new Queue<Object>();
				}

				public void AddDataToQueue(Object data)
				{	
					msgQueue.Enqueue(data);
				}
			}	


			public void AddData(ProxyNode n, Object data)
			{
				foreach(ProxyNodeData proxyNodeData in proxyDataList)
				{
					if(n==proxyNodeData.proxyNode)
					{
						proxyNodeData.AddDataToQueue(data);
						return;
					}
				}
				//proxyNode not existing; so create 1
				ProxyNodeData npd = new ProxyNodeData(n);
				npd.AddDataToQueue(data);
				proxyDataList.Add(npd);
			}

			public void AddNewEntry(ProxyNode n)
			{
				ProxyNodeData npd = new ProxyNodeData(n);
				proxyDataList.Add(npd);
			}


			public ProxyNodeRegistry()
			{
				proxyDataList = new List<ProxyNodeData>();
			}

			public ProxyNode findProxyNode(IPAddress ip)
			{
				foreach(ProxyNodeData proxyNodeData in proxyDataList)
				if(proxyNodeData.proxyNode.getIP()==ip)
					return proxyNodeData.proxyNode;
					//RELAX: yes i know this is wrong; have to change it
					throw new Tashjik.Common.Exception.LocalHostIPNotFoundException();
				
			}
		}

/*		public interface IProxy
		{
			//void notifyMsgRec(IPAddress fromIP, Object data, AsyncCallback notifyMsgRecCallBack, Object appState);
				void beginNotifyMsgRec(IPAddress fromIP, Object data, AsyncCallback notifyMsgRecCallBack, Object appState);
		}
*/
			
			
		public void notifyMsg(IPAddress fromIP, byte[] buffer, int offset, int size)
		{
			
		}
	
		public void notifyMsg(IPAddress fromIP, Object data)
		{
			try
			{
				ProxyNode proxyFound = proxyNodeRegistry.findProxyNode(fromIP);
				proxyFound.beginNotifyMsgRec(fromIP, data, null, null);
			}
			//RELAX: yes i know this is wrong; have to change it
			catch (Tashjik.Common.Exception.LocalHostIPNotFoundException)
			{
				ProxyNode n = createProxyNodeDelegate(fromIP);
				proxyNodeRegistry.AddNewEntry(n);
				n.beginNotifyMsgRec(fromIP, data, null, null);
			}
		
		}
		
		public ProxyNode getProxyNode(IPAddress IP)
		{
			try
			{
				ProxyNode proxyFound = proxyNodeRegistry.findProxyNode(IP);
				return proxyFound;
			}
			//RELAX: yes i know this is wrong; have to change it
			catch (Tashjik.Common.Exception.LocalHostIPNotFoundException)
			{
				ProxyNode n = createProxyNodeDelegate(IP);
				proxyNodeRegistry.AddNewEntry(n);
				return n;
			}
		}

		private CreateProxyNodeDelegate createProxyNodeDelegate;
		
/*		internal void setCreateProxyNodeDelegate(OverlayServer.CreateProxyNodeDelegate createProxyNodeDelegate)
		{
			this.createProxyNodeDelegate = createProxyNodeDelegate;
		}
*/

		//private OverlayServerFactory overlayServerFactory;
/*
		internal ProxyController(OverlayServerFactory overlayServerFactory, String ovType)
		{
			this.overlayServerFactory = overlayServerFactory;
			proxyNodeRegistry = new ProxyNodeRegistry();
			strOverlayType = ovType;
		}
		*/
		public delegate ProxyNode CreateProxyNodeDelegate(IPAddress IP);
		
		public ProxyNodeController(CreateProxyNodeDelegate createProxyNodeDelegate)
		{
			this.createProxyNodeDelegate = createProxyNodeDelegate;
		}
		
		//private String strOverlayType;
		
		/*internal ProxyNode createProxyNode(IPAddress IP)
		{
			return overlayServerFactory.createProxyNode(strOverlayType, IP, this);
						                   
		}*/
		
		public void register(ProxyNode proxyNode)
		{
			proxyNodeRegistry.AddNewEntry(proxyNode);
		}

		public void sendMsg(Object data, ProxyNode sender)
		{
			proxyNodeRegistry.AddData(sender, data);
		}

	}

}
