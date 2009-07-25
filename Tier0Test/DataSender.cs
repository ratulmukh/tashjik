/*
 * Created by SharpDevelop.
 * User: ratul
 * Date: 7/21/2009
 * Time: 5:20 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

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
			byte[] byteIP = {10, 200, 76, 45};
			IPAddress IP = new IPAddress(byteIP);
			
			//IPAddress IP = Tashjik.Common.UtilityMethod.GetLocalHostIP();
				
			
			//TransportLayerCommunicator.Msg msg = new TransportLayerCommunicator.Msg(Tier0TestGuid);
						
			//MemoryStream data = new MemoryStream(Marshal.SizeOf(Tier0TestGuid));
			//msg.setData(data);
			
			Thread.Sleep(1000);
			
			transportLayerCommunicator.BeginTransportLayerSend(IP, msg, 0, strMsg.Length, Tier0TestGuid, new AsyncCallback(sendDataCallBack), IP);
			
			//sendSameDataToSameIP_MultipleTimes(IP, msg, 0, strMsg.Length, Tier0TestGuid, null, null);
			
			
			Console.WriteLine("DataSender::SendData EXIT");
		}
		
		private void sendSameDataToSameIP_MultipleTimes(IPAddress IP, byte[] buffer, int offset, int size, Guid overlayGuid, AsyncCallback callBack, Object appState)
		{
			while(true)
			{
				transportLayerCommunicator.BeginTransportLayerSend(IP, buffer, offset, size, overlayGuid, callBack, appState);
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
		
 		public void notifyMsg(IPAddress fromIP, byte[] buffer, int offset, int size)
		{
			
		}
	}
}
