﻿/************************************************************
* File Name: ITransportLayerCommunicator.cs
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

namespace Tashjik.Tier0
{
	public interface ITransportLayerCommunicator
	{
		
	/*	class TwoWayCallbackData
		{
			public int twoWayId;
			public TwoWayCallbackDelegate twoWayCallbackDelegate;
			public TwoWayRelayCallbackDelegate twoWayRelayCallbackDelegate;
			public Object appState;
			
		}

		class Data
		{
			public byte[] buffer;
			public int offset;
			public int size;
		
		}
*/	
		void TransportLayerSendOneWay(IPAddress IP, byte[] buffer, int offset, int size, Guid overlayGuid, AsyncCallback callBack, Object appState);
		void BeginTransportLayerSendOneWay(IPAddress IP, byte[] buffer, int offset, int size, Guid overlayGuid, AsyncCallback callBack, Object appState);
		void BeginTransportLayerSendTwoWay(IPAddress IP, byte[] buffer, int offset, int size, Guid overlayGuid, AsyncCallback callBack, Object appState);
		void BeginTransportLayerSendTwoWayRelay(IPAddress IP, byte[] buffer, int offset, int size, Guid overlayGuid, AsyncCallback callBack, Object appState, Guid relayTicket);
		
		void EndTransportLayerSend(IPAddress IP);
		void register(Guid guid, TransportLayerCommunicator.ISink sink);

//		void registerTwoWay(Guid guid, List<TwoWayCallbackData> twoWayCallbackDataList);
		        
//		public delegate ProxyNode TwoWayCallbackDelegate(Guid relayTicket, Object appState);
//		public delegate Data TwoWayCallbackDelegate(IPAddress fromIP, byte[] buffer, int offset, int size);
//		public delegate Data TwoWayRelayCallbackDelegate(IPAddress fromIP, IPAddress originalFromIP, Guid relayTicket, byte[] buffer, int offset, int size);

		


	}
}
	
