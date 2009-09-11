#define SIM

using System;
using System.Net;
using System.Net.Sockets;
using Tashjik;
using Tashjik.Tier2;
using Tashjik.Tier0;
using Tashjik.Common;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

//using log4net;
//using log4net.Config;

namespace TashjikClient
{
	public class Client : TransportLayerCommunicator.ISink
	{
		//private static readonly ILog log = LogManager.GetLogger(typeof(Client));
		
		public Client()
		{
			
		}
		
		static TransportLayerCommunicator transportLayerCommunicator; // = TransportLayerCommunicator.getRefTransportLayerCommunicator();

#if SIM		
		public static void Main(string[] args) 
		{
			Console.WriteLine(args.Length);
		
			if(args.Length >= 1)
			{
				//byte[] byteIP = System.Text.Encoding.ASCII.GetBytes(args[0]);
				String[] IPsplit = args[0].Split(new char[] {'.'});
				Console.WriteLine("Hi there");

				int IP0 = (int)(System.Convert.ToInt32 (IPsplit[0]));
				int IP1 = (int)(System.Convert.ToInt32 (IPsplit[1]));
				int IP2 = (int)(System.Convert.ToInt32 (IPsplit[2]));
				int IP3 = (int)(System.Convert.ToInt32 (IPsplit[3]));
				byte[] byteIP = {(byte)IP0, (byte)IP1, (byte)IP2, (byte)IP3};
				Console.Write(IP0);
				Console.Write(".");
				Console.Write(IP1);
				Console.Write(".");
				Console.Write(IP2);
				Console.Write(".");
				Console.WriteLine(IP3);
/*				Console.Write((int)byteIP[0]);
				Console.Write(" ");
				Console.Write((int)byteIP[1]);
				Console.Write(" ");
				Console.Write((int)byteIP[2]);
				Console.Write(" ");
				Console.WriteLine((int)byteIP[3]);
*/				
				IPAddress ipAddress = new IPAddress(byteIP);

				UtilityMethod.SetLocalHostIP(ipAddress);
				Console.Write("received port=");
				Console.WriteLine(args[1]);
				UtilityMethod.SetPort(args[1]);
				
				transportLayerCommunicator = TransportLayerCommunicator.getRefTransportLayerCommunicator();
				
			}

#else
		public static void Main()
		{
#endif
		
			Guid g = Guid.NewGuid();
			Console.Write(g.ToString());

			Client client = new Client();
	//		client.chkTransPortLayerComm();
			client.chkChord();
			
		}
		private void requestBootStrapNode()
		{
			Console.WriteLine("Entering requestBootStrapNode");
#if SIM
			
			transportLayerCommunicator.register(ClientGuid, this);
			byte[] byteIP = {127, 0, 0, 1};
			IPAddress ipAddress = new IPAddress(byteIP);

			String strMsg = "bootStrap request";
			byte[] msg = System.Text.Encoding.ASCII.GetBytes(strMsg);
			transportLayerCommunicator.BeginTransportLayerSend(ipAddress, msg, 0, strMsg.Length, ClientGuid, new AsyncCallback(sendDataCallBack), ipAddress);
#else
			chordInstanceGuid = new Guid("");
			return null;
#endif			
		}
		
		private void receiveBootStrapNode(IPAddress fromIP, byte[] buffer, int offset, int size)
		{
			Console.WriteLine("Client::receiveBootStrapNode ENTER");
			//Console.WriteLine(Encoding.ASCII.GetString(buffer));
			ChordServer chord;
			
			if(String.Compare(Encoding.ASCII.GetString(buffer), "no bootstrapnode") == 0)
			{
				Console.WriteLine("Client::receiveBootStrapNode NO bootstrapnode received");
			
				chord = (ChordServer)(TashjikServer.createNew("Chord")); //new Guid("0c400880-0722-420e-a792-0a764d6539ee")));
				Guid chordInstanceGuid = chord.getGuid();
				Console.Write("Client::receiveBootStrapNode Created Chord GUID=");
				Console.WriteLine(chordInstanceGuid.ToString());
				byte[] byteIP = {127, 0, 0, 1};
				IPAddress ipAddress = new IPAddress(byteIP);
				byte[] msg = System.Text.Encoding.ASCII.GetBytes(chordInstanceGuid.ToString());
				transportLayerCommunicator.BeginTransportLayerSend(ipAddress, msg, 0, chordInstanceGuid.ToString().Length, ClientGuid, new AsyncCallback(sendDataCallBack), ipAddress);

			}
			else
			{
				Console.WriteLine("Client::receiveBootStrapNode bootstrapnode RECEIVED");
				Console.Write("RECEIVED bootstrapnode=");
				Console.WriteLine(Encoding.ASCII.GetString(buffer));
			}
		}
			
		private void initializeChord()
		{
			
		}
		
		private void chkChord()
		{
			Console.WriteLine("Entering chkChord");
	
			//transportLayerCommunicator.register(ClientGuid, this);
		
			Console.WriteLine("Creating new Chord overlay");
			Guid chordInstanceGuid;
			//IPAddress bootStrapIP =
			requestBootStrapNode();
	/*		ChordServer chord;
			
			if(bootStrapIP == null)
			{
				chord = (ChordServer)(TashjikServer.createNew("Chord")); //new Guid("0c400880-0722-420e-a792-0a764d6539ee")));
				chordInstanceGuid = chord.getGuid();
			}
			else
				chord = (ChordServer)(TashjikServer.joinExisting(bootStrapIP, new Guid("0c400880-0722-420e-a792-0a764d6539ee"), chordInstanceGuid));
	*/		

/*			String strKey = "key";
			String strData = "data";
			Console.WriteLine(strKey);
			Console.WriteLine(strData);
			
			byte[] key = System.Text.Encoding.ASCII.GetBytes(strKey);
			byte[] data = System.Text.Encoding.ASCII.GetBytes(strData);
			Console.WriteLine(key);
			Console.WriteLine(data);
			
			Console.WriteLine(Encoding.ASCII.GetString(key));
			Console.WriteLine(Encoding.ASCII.GetString(data));
			
			Console.WriteLine(key.ToString());
			Console.WriteLine(data.ToString());
			
			Console.WriteLine("Putting data to new Chord ");
			chord.beginPutData(key, data, 0, strData.Length, new AsyncCallback(processPutDataCallBack), chord);
			Console.WriteLine("After Putting data to new Chord ");
*/		
			
			
			
			
			
			
			
			
			
			
			
			/*			//ChordServer chord = (ChordServer)(TashjikServer.createNew(String.Chord));
			ArrayList arr = TashjikServer.getList(String.Chord);
			ChordServer chord = (ChordServer)(arr[0]);
			
			String key = "key";
			Tashjik.Common.Data data = new Tashjik.Common.Data();
			AsyncCallback putDataCallBack = new AsyncCallback(processPutDataCallBack);
			//chord.beginGetData(key, data, putDataCallBack, null);
*/
//Tashjik.Server.Node node = new Tashjik.Server.Node();
			Console.WriteLine("Please enter IP address of node to send msg to");
			String IP = Console.ReadLine();
		}
		
		public void notifyMsg(IPAddress fromIP, byte[] buffer, int offset, int size)
		{
		
			Console.WriteLine("Msg received");
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();
			
			receiveBootStrapNode(fromIP, buffer, offset, size);
		}

		Guid ClientGuid = new Guid("2527df07-e8c5-4f0d-a46e-effa26cfcb0d");
		
		public void chkTransPortLayerComm()
		{
			transportLayerCommunicator.register(ClientGuid, this);
			Console.WriteLine("Please enter IP address of node to send msg to");
			String IP = Console.ReadLine();
			String[] IPsplit = IP.Split(new char[] {'.'});
			Console.WriteLine(IPsplit[0]);
			Console.WriteLine(IPsplit[1]);
			Console.WriteLine(IPsplit[2]);
			Console.WriteLine(IPsplit[3]);
			int IP0 = (int)(System.Convert.ToInt32 (IPsplit[0]));
			int IP1 = (int)(System.Convert.ToInt32 (IPsplit[1]));
			int IP2 = (int)(System.Convert.ToInt32 (IPsplit[2]));
			int IP3 = (int)(System.Convert.ToInt32 (IPsplit[3]));
			
			byte[] byteIP = {(byte)IP0, (byte)IP1, (byte)IP2, (byte)IP3};
			IPAddress ipAddress = new IPAddress(byteIP);
			
			String strMsg = "Client sending msg ";
			byte[] msg = System.Text.Encoding.ASCII.GetBytes(strMsg);

			transportLayerCommunicator.BeginTransportLayerSend(ipAddress, msg, 0, strMsg.Length, ClientGuid, new AsyncCallback(sendDataCallBack), ipAddress);
		}
		
		static void sendDataCallBack(IAsyncResult result)
		{
			IPAddress IP = (IPAddress)(result.AsyncState);
			try
			{
				transportLayerCommunicator.EndTransportLayerSend(IP);
			}
			catch(SocketException)
			{
				Console.WriteLine("Client caught SocketException");
			}
			
		}
		
		static void processPutDataCallBack(IAsyncResult result)
		{
			ChordServer chord = (ChordServer)(result.AsyncState);
			String strKey = "key";
			byte[] key = System.Text.Encoding.ASCII.GetBytes(strKey);
			chord.beginGetData(key, new AsyncCallback(processGetDataCallBack), null);
		}
		
		static void processGetDataCallBack(IAsyncResult result)
		{
			Tashjik.Common.Data_Object data_Object = (Tashjik.Common.Data_Object)(result.AsyncState);
			byte[] data = data_Object.data;
			Console.Write("DATA FOUND: IT IS ");
			Console.WriteLine(Encoding.ASCII.GetString(data));
			//#if DINKUM
		//	Console.WriteLine("tada");
		//	#endif			                  
		//	log.Info(data);
							
		}
		
		
	}
}
