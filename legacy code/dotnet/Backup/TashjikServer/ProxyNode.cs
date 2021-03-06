﻿/************************************************************
* File Name: ProxyNode.cs
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
using System.Runtime.CompilerServices;
using Tashjik.Common;

[assembly:InternalsVisibleTo("Narada")]
[assembly:InternalsVisibleTo("Pastry")]

namespace Tashjik
{
	/// <summary>
	/// Description of ProxyNode.
	/// </summary>
	public abstract class ProxyNode : Node
	{
		protected Tashjik.Common.NodeBasic selfNodeBasic; 
		protected Tier0.TransportLayerCommunicator transportLayerCommunicator = Tier0.TransportLayerCommunicator.getRefTransportLayerCommunicator();

		public ProxyNode(IPAddress ip)
		{
			selfNodeBasic = new NodeBasic(ip);	
		}
		
		public byte[] getHashedIP()
		{
			return selfNodeBasic.getHashedIP();
		}

		public IPAddress getIP()
		{
			return selfNodeBasic.getIP();
		}

		public void setIP(IPAddress ip)
		{
			selfNodeBasic.setIP(ip);
		}
		
		public abstract void notifyOneWayMsg(IPAddress fromIP, byte[] buffer, int offset, int size);
		public abstract Tashjik.Tier0.TransportLayerCommunicator.Data notifyTwoWayMsg(IPAddress fromIP, byte[] buffer, int offset, int size);
		public abstract Tashjik.Tier0.TransportLayerCommunicator.Data notifyTwoWayRelayMsg(IPAddress fromIP, IPAddress originalFromIP, byte[] buffer, int offset, int size, Guid relayTicket);
		public abstract void notifyTwoWayReplyReceived(IPAddress fromIP, byte[] buffer, int offset, int size, AsyncCallback originalRequestCallBack, Object originalAppState);
		
	//	
		//public abstract void setProxyController(ProxyController c);
	}


}
