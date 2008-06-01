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

namespace Tashjik.Base
{
	public class LowLevelComm
	{

		public class Msg
		{
			private readonly Guid overlayGuid;
			private Object data;


			public Msg(Guid guid)
			{
				overlayGuid = guid;
			}

			public Guid getGuid()
			{
				return new Guid(overlayGuid.ToByteArray());
			}

			public void setData(Object list)
			{
				data = list;
			}	

			public Object getData()
			{
				return data;
			}

		}

		private class SockMsgQueue
		{
			public Socket sock;
			public Queue<Msg> msgQueue;

			public SockMsgQueue(Socket s, Queue<Msg> m)
			{
				sock = s;
				msgQueue = m;
			}
		}

		private Dictionary<IPAddress, SockMsgQueue> commRegistry = new Dictionary<IPAddress, SockMsgQueue>();
		private Dictionary<Guid, ISink> overlayRegistry = new Dictionary<Guid, ISink>();

		public interface ISink
		{
			void notifyMsg(IPAddress fromIP, Object data);
		}

		public void register(Guid guid, ISink sink)
		{
			overlayRegistry.Add(guid, sink);

		}

		public void forward(IPAddress IP, Msg msg)
		{
			SockMsgQueue sockMsgQueue;
			if(commRegistry.TryGetValue(IP, out sockMsgQueue))
				sockMsgQueue.msgQueue.Enqueue(msg);
			else
			{
				Queue<Msg> msgQueue= new Queue<Msg>();
				msgQueue.Enqueue(msg);

				Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
				int iPortNo = System.Convert.ToInt16 ("2334");
				IPEndPoint ipEnd = new IPEndPoint (IP,iPortNo);
				sock.Connect(ipEnd);

				SockMsgQueue sockMsgQueue1 = new SockMsgQueue(sock, msgQueue);
				commRegistry.Add(IP, sockMsgQueue1);
			}
		}

		private void receive(IPAddress fromIP, Msg msg)
		{
			ISink sink;
			if(overlayRegistry.TryGetValue(msg.getGuid(), out sink))
				sink.notifyMsg(fromIP, msg.getData());
			else
				throw new Exception();

		}

		public class Forwarder
		{
		}

		public class Receiver
		{

		}

		//never call this directy
		public LowLevelComm()
		{

		}

		//singleton
		private static LowLevelComm lowLevelComm = null;

		public static LowLevelComm getRefLowLevelComm()
		{
			if(lowLevelComm!=null)
				return lowLevelComm;
			else
			{
				lowLevelComm = new LowLevelComm();
				return lowLevelComm;
			}
		}

	}
}


