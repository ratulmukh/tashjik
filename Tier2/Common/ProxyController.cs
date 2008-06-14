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

namespace Tashjik.Tier2.Common
{
	/*
	class ProxyController : IProxyController, Controller.ISink
	{
	
		private readonly NodeProxyRegistry nodeProxyRegistry;

		class NodeProxyRegistry
		{
			List<NodeProxyData> proxyDataList;
	
			class NodeProxyData
			{
				public NodeProxy nodeProxy;
				public Queue<Object> msgQueue;
	
				public NodeProxyData(NodeProxy n)
				{
					nodeProxy = n;
					msgQueue = new Queue<Object>();
				}

				public void AddDataToQueue(Object data)
				{	
					msgQueue.Enqueue(data);
				}
			}	


			public void AddData(NodeProxy n, Object data)
			{
				foreach(NodeProxyData nodeProxyData in proxyDataList)
				{
					if(n==nodeProxyData.nodeProxy)
					{
						nodeProxyData.AddDataToQueue(data);
						return;
					}
				}
				//nodeProxy not existing; so create 1
				NodeProxyData npd = new NodeProxyData(n);
				npd.AddDataToQueue(data);
				proxyDataList.Add(npd);
			}

			public void AddNewEntry(NodeProxy n)
			{
				NodeProxyData npd = new NodeProxyData(n);
				proxyDataList.Add(npd);
			}


			public NodeProxyRegistry()
			{
				proxyDataList = new List<NodeProxyData>();
			}

			public NodeProxy getNodeProxy(IPAddress ip)
			{
				foreach(NodeProxyData nodeProxyData in proxyDataList)
				if(nodeProxyData.nodeProxy.getIP()==ip)
					return nodeProxyData.nodeProxy;
					//RELAX: yes i know this is wrong; have to change it
					throw new Tashjik.Common.Exception.LocalHostIPNotFoundException();
				
			}
		}

		public interface IProxy
		{
			//void notifyMsgRec(IPAddress fromIP, Object data, AsyncCallback notifyMsgRecCallBack, Object appState);
				void beginNotifyMsgRec(IPAddress fromIP, Object data, AsyncCallback notifyMsgRecCallBack, Object appState);
		}

		public void notifyMsgRec(IPAddress fromIP, Object data)
		{
			try
			{
				NodeProxy proxyFound = nodeProxyRegistry.getNodeProxy(fromIP);
				proxyFound.beginNotifyMsgRec(fromIP, data, null, null);
			}
			//RELAX: yes i know this is wrong; have to change it
			catch (Tashjik.Common.Exception.LocalHostIPNotFoundException e)
			{
				NodeProxy n = new NodeProxy(fromIP);
				nodeProxyRegistry.AddNewEntry(n);
				n.beginNotifyMsgRec(fromIP, data, null, null);
			}
		
		}



		public ProxyController()
		{
			nodeProxyRegistry = new NodeProxyRegistry();
			NodeProxy.setProxyController(this);
			
		}

		public void register(NodeProxy nodeProxy)
		{
			nodeProxyRegistry.AddNewEntry(nodeProxy);
		}

		public void sendMsg(Object data, NodeProxy sender)
		{
			nodeProxyRegistry.AddData(sender, data);
		}

	}
	*/
}
