/************************************************************
* File Name: DataSender.cs
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
using Tashjik.Tier0;
using System.IO;
using System.Runtime.InteropServices;
using Tashjik;
using System.Threading;

namespace Tashjik.Test.Tier0Test
{
	/// <summary>
	/// Description of DataSender.
	/// </summary>
	public class DataSender : TransportLayerCommunicator.ISink
	{
	
		static TransportLayerCommunicator transportLayerCommunicator = TransportLayerCommunicator.getRefTransportLayerCommunicator();
		
		public static void Main()
		{
			DataSender dataSender = new DataSender();
			dataSender.SendData();
			while(true)
			{
				;
			}
		}
		
		private void SendData()
		{
			Console.WriteLine("DataSender::SendData ENTER");
			Guid Tier0TestGuid = new Guid("e04ffdc1-637d-4640-bfe6-55fa5a38a178");
			
			transportLayerCommunicator.register(Tier0TestGuid, this);
			//String strIP = "127.0.0.1";
			//byte[] byteIP = System.Text.Encoding.ASCII.GetBytes(strIP);
			String strMsg = "Hi da";
			byte[] msg = System.Text.Encoding.ASCII.GetBytes(strMsg);
			//byte[] byteIP = {127, 0, 0, 1};
			//byte[] byteIP = {10, 200, 76, 45};
			byte[] byteIP = {192, 168, 1, 100};
			IPAddress IP = new IPAddress(byteIP);
			
			IPAddress IP1 = Tashjik.Common.UtilityMethod.GetLocalHostIP();
			Console.WriteLine(IP1.ToString());
				
			
			//TransportLayerCommunicator.Msg msg = new TransportLayerCommunicator.Msg(Tier0TestGuid);
						
			//MemoryStream data = new MemoryStream(Marshal.SizeOf(Tier0TestGuid));
			//msg.setData(data);
			
			Thread.Sleep(1000);
			
			transportLayerCommunicator.BeginTransportLayerSendOneWay(IP, msg, 0, strMsg.Length, Tier0TestGuid, new AsyncCallback(sendDataCallBack), IP);
			
			//sendSameDataToSameIP_MultipleTimes(IP, msg, 0, strMsg.Length, Tier0TestGuid, null, null);
			
			
			Console.WriteLine("DataSender::SendData EXIT");
		}
		
		private void sendSameDataToSameIP_MultipleTimes(IPAddress IP, byte[] buffer, int offset, int size, Guid overlayGuid, AsyncCallback callBack, Object appState)
		{
			while(true)
			{
				transportLayerCommunicator.BeginTransportLayerSendOneWay(IP, buffer, offset, size, overlayGuid, callBack, appState);
			}
		}
		
		private void sendDataCallBack(IAsyncResult result)
		{
			IPAddress IP = (IPAddress)(result.AsyncState);
			try
			{
				transportLayerCommunicator.EndTransportLayerSend(IP);
			}
			catch(SocketException)
			{
				Console.WriteLine("DataSender caught SocketException");
			}
		}
		
 		public void notifyOneWayMsg(IPAddress fromIP, byte[] buffer, int offset, int size)
		{
			
		}
	}
}
