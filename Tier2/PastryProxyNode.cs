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
using System.Collections;
using System.Collections.Generic;
using Tashjik;

namespace Tashjik.Tier2
{
	internal class PastryProxyNode : ProxyNode, IPastryNode
	{
		
		internal static PastryRealNode thisNode;
	
		//private Tashjik.Common.NodeBasic selfNodeBasic;
		//private Tier0.TransportLayerCommunicator transportLayerCommunicator;		
		
		//private ProxyController proxyController;
		
		public PastryProxyNode(IPAddress ip/*, ProxyController proxyController*/) : base(ip)
		{
			//transportLayerCommunicator = Tier0.TransportLayerCommunicator.getRefLowLevelComm();
			//selfNodeBasic = new Tashjik.Common.NodeBasic(ip);
			//setProxyController(proxyController);
		}
		/*
		public override void setProxyController(ProxyController c)
		{
			//need to handle synchronised calls here
			if(proxyController!=null)
			proxyController = c;
		}
		*/
		/*
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
		*/
		public override void beginNotifyMsgRec(IPAddress fromIP, Object data, AsyncCallback notifyMsgRecCallBack, Object appState)
		{
			
		}
		
		public void join(IPastryNode newNode)
		{
			
		}
		
		public void leave()
		{
			
		}
		
		public void route(Object msg, byte[] key)
		{
			
		}
		
						
	}

}
