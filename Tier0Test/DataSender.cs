﻿/*
 * Created by SharpDevelop.
 * User: ratul
 * Date: 7/21/2009
 * Time: 5:20 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Net;
using Tashjik.Tier0;
using System.IO;
using System.Runtime.InteropServices;
using Tashjik;

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
			String strIP = "127.0.0.1";
			byte[] byteIP = System.Text.Encoding.ASCII.GetBytes(strIP);
			//IPAddress IP = new IPAddress(byteIP);
			
			IPAddress IP = Tashjik.Common.UtilityMethod.GetLocalHostIP();
				
			
			TransportLayerCommunicator.Msg msg = new TransportLayerCommunicator.Msg(Tier0TestGuid);
						
			MemoryStream data = new MemoryStream(Marshal.SizeOf(Tier0TestGuid));
			msg.setData(data);
			
			sendSameDataToSameIP_MultipleTimes(IP, msg);
			Console.WriteLine("DataSender::SendData EXIT");
		}
		
		private void sendSameDataToSameIP_MultipleTimes(IPAddress IP, TransportLayerCommunicator.Msg msg)
		{
			while(true)
			{
				transportLayerCommunicator.forwardMsgToRemoteHost(IP, msg);
			}
		}
		
		public void notifyMsg(IPAddress fromIP, Object data)
		{
			
		}
	}
}
