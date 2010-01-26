/************************************************************
* File Name: ProxyNodeController.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Tashjik.Tier0;
using System.Text;

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
		private Tier0.TransportLayerCommunicator transportLayerCommunicator;
			
		class ProxyNodeRegistry
		{
			List<ProxyNodeData> proxyDataList;
			//Tier0.TransportLayerCommunicator transportLayerCommunicator;
			public ProxyNodeRegistry()
			{
				proxyDataList = new List<ProxyNodeData>();
				//transportLayerCommunicator = Tier0.TransportLayerCommunicator.getRefTransportLayerCommunicator();
			}
			
			class ProxyNodeData
			{
				public ProxyNode proxyNode;
				//queue is defunct for the moment
				//we will be sending data directly to transportLayerCommunicator
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
				Console.WriteLine("ProxyNodeController::ProxyNodeData::AddData ENTER");
				foreach(ProxyNodeData proxyNodeData in proxyDataList)
				{
					if(n==proxyNodeData.proxyNode)
					{
						Console.WriteLine("ProxyNodeController::ProxyNodeData::AddData proxyNode found");
						proxyNodeData.AddDataToQueue(data);
						return;
					}
				}
				//proxyNode not existing; so create 1
				Console.WriteLine("ProxyNodeController::ProxyNodeData::AddData proxyNode not existing; so creating 1");
				ProxyNodeData npd = new ProxyNodeData(n);
				npd.AddDataToQueue(data);
				proxyDataList.Add(npd);
			}

			public void AddNewEntry(ProxyNode n)
			{
				ProxyNodeData npd = new ProxyNodeData(n);
				proxyDataList.Add(npd);
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
			
			
	
	
		public Tashjik.Tier0.TransportLayerCommunicator.Data notifyTwoWayMsg(IPAddress fromIP, byte[] buffer, int offset, int size)
		{
			Console.WriteLine("ProxyNodeController::notifyTwoWayMsg ENTER");
			Tashjik.Tier0.TransportLayerCommunicator.Data data;
			ProxyNode proxyNode = getProxyNode(fromIP);
			data = proxyNode.notifyTwoWayMsg(fromIP, buffer, offset, size);
			return data;
		}
		
		public Tashjik.Tier0.TransportLayerCommunicator.Data notifyTwoWayRelayMsg(IPAddress fromIP, IPAddress originalFromIP, byte[] buffer, int offset, int size, Guid relayTicket)
		{
			Console.WriteLine("ProxyNodeController::notifyTwoWayRelayMsg ENTER");
			
			Tashjik.Tier0.TransportLayerCommunicator.Data data;
			ProxyNode proxyNode = getProxyNode(fromIP);
			data = proxyNode.notifyTwoWayRelayMsg(fromIP, originalFromIP, buffer, offset, size, relayTicket);
			return data;
		}
		
		public void notifyOneWayMsg(IPAddress fromIP, byte[] buffer, int offset, int size) 
		{
			Console.WriteLine("ProxyNodeController::notify ENTER");
			
			ProxyNode proxyNode = getProxyNode(fromIP);
			proxyNode.notifyOneWayMsg(fromIP, buffer, offset, size);
		}
		
		
		public void notifyTwoWayReplyReceived(IPAddress fromIP, byte[] buffer, int offset, int size, AsyncCallback originalRequestCallBack, Object originalAppState)
		{
			Console.WriteLine("ProxyNodeController::notify ENTER");
			
			ProxyNode proxyNode = getProxyNode(fromIP);
			proxyNode.notifyTwoWayReplyReceived(fromIP, buffer, offset, size, originalRequestCallBack, originalAppState);
	
		}
		
		public ProxyNode getProxyNode(IPAddress IP)
		{
			Console.WriteLine("ProxyNodeController::getProxyNode ENTER");
			try
			{
				ProxyNode proxyFound = proxyNodeRegistry.findProxyNode(IP);
				Console.WriteLine("ProxyNodeController::getProxyNode proxyFound");
				return proxyFound;
			}
			//RELAX: yes i know this is wrong; have to change it
			catch (Exception)
			{
				Console.WriteLine("ProxyNodeController::getProxyNode proxy NOT Found: creating a new 1 and adding to registry");
				ProxyNode n = createProxyNodeDelegate(IP, this);
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
		public delegate ProxyNode CreateProxyNodeDelegate(IPAddress IP, ProxyNodeController proxyNodeController);
		
		private Guid overlayInstanceGuid;
		
		public ProxyNodeController(CreateProxyNodeDelegate createProxyNodeDelegate, Guid overlayInstanceGuid)
		{
			Console.WriteLine("ProxyNodeController::ProxyNodeController ENTER");
			this.overlayInstanceGuid = overlayInstanceGuid;
			this.createProxyNodeDelegate = createProxyNodeDelegate;
			transportLayerCommunicator = Tier0.TransportLayerCommunicator.getRefTransportLayerCommunicator();
			transportLayerCommunicator.register(overlayInstanceGuid, this);
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
		
		public void sendMsg(Object data, ProxyNode n)
		{
			
		}
		
		public void sendMsg(ProxyNode sender, byte[] buffer, int offset, int size, AsyncCallback callBack, Object appState)
		{
			Console.WriteLine("ProxyNodeController::sendMsg sending data to transportLayerCommunicator");
			//proxyNodeRegistry.AddData(sender, data);
			transportLayerCommunicator.BeginTransportLayerSendOneWay(sender.getIP(), buffer, offset, size, overlayInstanceGuid, callBack, appState);
		}

		public void sendMsgTwoWay(ProxyNode sender, byte[] buffer, int offset, int size, AsyncCallback callBack, Object appState)
		{
			Console.WriteLine("ProxyNodeController::sendMsgTwoWay sending data to transportLayerCommunicator");
			//proxyNodeRegistry.AddData(sender, data);
			transportLayerCommunicator.BeginTransportLayerSendTwoWay(sender.getIP(), buffer, offset, size, overlayInstanceGuid, callBack, appState);
		
		}
		
		public void sendMsgTwoWayRelay(ProxyNode sender, byte[] buffer, int offset, int size, AsyncCallback callBack, Object appState, Guid relayTicket)
		{
			Console.WriteLine("ProxyNodeController::sendMsgTwoWayRelay sending data to transportLayerCommunicator");
			//proxyNodeRegistry.AddData(sender, data);
			transportLayerCommunicator.BeginTransportLayerSendTwoWayRelay(sender.getIP(), buffer, offset, size, overlayInstanceGuid, callBack, appState, relayTicket);
		
		}
		
	}

}
