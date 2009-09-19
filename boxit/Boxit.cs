/*
 * Created by SharpDevelop.
 * User: ratul
 * Date: 8/8/2009
 * Time: 6:58 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Tashjik.Tier0;
using Tashjik.Common;

public static class Boxit
{
	/// 
	/// Example of creating an external process and killing it
	/// 
	public static void Main() 
	{
		init();
/*
    Process process;
    while(true)
    {
    	process = Process.Start("TashjikClient.exe");
       	//Thread.Sleep(15000);
    }
    	try {
    	process.BeginE
  //  		process.Kill();
    	} catch {}
*/    	
   }
	
	struct Triplet
	{
		public Process process;
		public int portNo;
		public Socket sock;
		public Triplet(Process process, int portNo, Socket sock)
		{
			this.portNo = portNo;
			this.process = process;
			this.sock = sock;
		}
		
	}
		
	static Dictionary<String, Triplet> registry = new Dictionary<String, Triplet>();
	
	static void init()
	{
		ThreadStart listenJob = new ThreadStart(StartListening);
		Thread listener = new Thread(listenJob);
		listener.Start();
		
		Process process;
		int portNo = 2336;
		for(int i=0; i<2; i++)
		{
			Random rnd = new Random();
			
			int add1 = rnd.Next(0, 255);		
			int add2 = rnd.Next(0, 255);
			int add3 = rnd.Next(0, 255);
			int add4 = rnd.Next(0, 255);
			
			byte[] byteIP = {(byte)add1, (byte)add2, (byte)add3, (byte)add4};
			
			Console.WriteLine("Hi there");
			Console.Write((int)byteIP[0]);
			Console.Write((int)byteIP[1]);
			Console.Write((int)byteIP[2]);
			Console.WriteLine((int)byteIP[3]);
				
			
			IPAddress IP = new IPAddress(byteIP);
			String strIP = Encoding.ASCII.GetString(byteIP);
			Console.WriteLine(strIP);
			Console.WriteLine(IP.ToString());
			process = Process.Start("TashjikClient.exe", IP.ToString() + " " + Convert.ToString(portNo));
			registry.Add(IP.ToString(), new Triplet(process, Convert.ToInt16(Convert.ToString(portNo)), null));
			//registry.Add(IP.ToString(), new Triplet(process, portNo, null));
			portNo++;
		}
	}
	
			static ManualResetEvent allDone = new ManualResetEvent(false);
		
			internal class SocketState
			{
				public Socket sock;
				public byte[] buffer = new byte[1024];
				public StringBuilder concatenatedString = new StringBuilder();
				public TransportLayerCommunicator transportLayerCommunicator;
			}	
			
		private static void StartListening()
		{
			//IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
        	//	IPAddress ipAddress = ipHostInfo.AddressList[0];
        	int iPortNo = System.Convert.ToInt16 ("2335");

        	byte[] byteIP = {127, 0, 0, 1};
			IPAddress ipAddress = new IPAddress(byteIP);

        	IPEndPoint localEndPoint = new IPEndPoint(ipAddress, iPortNo);
        	
        	Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
        	try 
        	{
            	listener.Bind(localEndPoint);
            	listener.Listen(100);
            	//listener.Listen(SocketOptionName.MaxConnections );

	            while (true) 
	            {
    	            // Set the event to nonsignaled state.
        	        allDone.Reset();
	
    	            // Start an asynchronous socket to listen for connections.
        	        Console.Write("Boxit::Waiting for a connection at port ");
    	            Console.WriteLine(iPortNo);
    	            
        	        SocketState socketState = new SocketState();
					socketState.sock = listener;
					socketState.transportLayerCommunicator = null; //TransportLayerCommunicator.getRefTransportLayerCommunicator();
            	    listener.BeginAccept( 
                	    new AsyncCallback(beginAcceptCallback_forStartListening),
                    	socketState );

					
	                // Wait until a connection is made before continuing.
    	            allDone.WaitOne();
        	    }

	        } 
        	catch (Exception e) 
        	{
    	        Console.WriteLine(e.ToString());
        	}	

		}
		
		static private void beginAcceptCallback_forStartListening(IAsyncResult result)
		{
			Console.WriteLine("TransportLayerCommunicator::beginAcceptCallback_forStartListening ENTER");
			// Signal the main thread to continue.
        	allDone.Set();

	        // Get the socket that handles the client request.
    	    SocketState socketState = (SocketState) result.AsyncState;
    	    Socket listener = socketState.sock;
        	Socket handler = listener.EndAccept(result);
			TransportLayerCommunicator transportLayerCommunicator = socketState.transportLayerCommunicator;
			
        	//SockMsgQueue sockMsgQueue = new SockMsgQueue(handler);
        	IPAddress IP = ((IPEndPoint)(handler.RemoteEndPoint)).Address;
        	
        	Triplet triplet;
        	if(registry.TryGetValue(IP.ToString(), out triplet))
        		triplet.sock = handler;
        		
        	try
			{
				//transportLayerCommunicator.commRegistry.Add(IP, sockMsgQueue);
				socketState.sock = handler;
		        handler.BeginReceive( socketState.buffer, 0, socketState.buffer.Length, new SocketFlags(), new AsyncCallback(beginReceiveCallBack), socketState);	
				Console.WriteLine("TransportLayerCommunicator::beginAcceptCallback_forStartListening ENTER");
        	}
			catch(System.ArgumentException)
			{
				socketState.sock = handler;
		        handler.BeginReceive( socketState.buffer, 0, socketState.buffer.Length, new SocketFlags(), new AsyncCallback(beginReceiveCallBack), socketState);	
		        Console.WriteLine("TransportLayerCommunicator::beginAcceptCallback_forStartListening ENTER");
			}
			Console.WriteLine("TransportLayerCommunicator::beginAcceptCallback_forStartListening ENTER");
	
		}
		
		static private void beginReceiveCallBack(IAsyncResult result)
		{
			Console.WriteLine("TransportLayerCommunicator::beginReceiveCallBack ENTER");
			String content = String.Empty;
			SocketState socketState = ((SocketState)(result.AsyncState));
			Socket sock = socketState.sock;
			
			int bytesRead = sock.EndReceive(result);
			if(bytesRead > 0)
			{
				socketState.concatenatedString.Remove(0, socketState.concatenatedString.Length);
				socketState.concatenatedString.Append(Encoding.ASCII.GetString(socketState.buffer, 0, bytesRead));
				
				if(content.IndexOf("\n") > -1)
				{
					content = socketState.concatenatedString.ToString();
					notifyUpperLayer(content, sock, socketState.transportLayerCommunicator );
				}
				else 
					sock.BeginReceive(socketState.buffer, 0, socketState.buffer.Length, new SocketFlags(), new AsyncCallback(beginReceiveCallBack), socketState);
			}
			content = socketState.concatenatedString.ToString();
			Console.WriteLine(content);
			notifyUpperLayer(content, sock, socketState.transportLayerCommunicator );
			Console.WriteLine("TransportLayerCommunicator::beginReceiveCallBack EXIT");
		}
		
	
		enum MsgExtractionStatus
		{
			NOTHING_EXTRACTED,
			FROM_IP_EXTRACTED,
			TO_IP_EXTRACTED,
			CALLTYPE_EXTRACTED,
			OVERLAYGUID_EXTRACTED,
			MESSAGE_EXTRACTED
		}
		static private void notifyUpperLayer(String content, Socket fromSock, TransportLayerCommunicator transportLayerCommunicator)
		{
			Console.WriteLine("TransportLayerCommunicator::notifyUpperLayer ENTER");
			Console.WriteLine("TransportLayerCommunicator::notifyUpperLayer content=");
			Console.WriteLine(content);
			Console.WriteLine("TransportLayerCommunicator::notifyUpperLayer split contents=");
			String[] split = content.Split(new char[] {'\0'});
			foreach (String s in split)
				Console.WriteLine(s);
			byte[] byteContent = System.Text.Encoding.ASCII.GetBytes(content);
			
			String strFromIP = null;
			String strToIP = null;
			String strOverlayGuid = null;
			String strCallType = null;
			byte[] byteOverlayGuid; 
			Guid overlayGuid = new Guid();;
			String strBuffer;
			byte[] byteBuffer;
			MsgExtractionStatus msgExtractionStatus = MsgExtractionStatus.NOTHING_EXTRACTED;  
			foreach (String s in split)
			{
				if(msgExtractionStatus == MsgExtractionStatus.NOTHING_EXTRACTED || msgExtractionStatus == MsgExtractionStatus.MESSAGE_EXTRACTED)
				{
					if(s.Length == 0)
						break;
					strFromIP = s;  	
					Console.Write("FromIP received: ");
					Console.WriteLine(s);
					msgExtractionStatus = MsgExtractionStatus.FROM_IP_EXTRACTED;
					
				}
				else if(msgExtractionStatus == MsgExtractionStatus.FROM_IP_EXTRACTED)
				{
					strToIP = s;	
					Console.Write("ToIP received: ");
					Console.WriteLine(s);
					msgExtractionStatus = MsgExtractionStatus.TO_IP_EXTRACTED;
						
					
				}
				else if(msgExtractionStatus == MsgExtractionStatus.TO_IP_EXTRACTED)
				{
					strCallType = s;
					Console.WriteLine("haha 1");
					Console.WriteLine(s);
					Console.WriteLine(s.Length);
					//byteOverlayGuid = System.Text.Encoding.ASCII.GetBytes(strOverlayGuid);
					//overlayGuid = new Guid(s);
					msgExtractionStatus = MsgExtractionStatus.CALLTYPE_EXTRACTED;
				}
				else if(msgExtractionStatus == MsgExtractionStatus.CALLTYPE_EXTRACTED)
				{
					strOverlayGuid = s;
					Console.WriteLine("haha 1.5");
					Console.WriteLine(s);
					Console.WriteLine(s.Length);
					byteOverlayGuid = System.Text.Encoding.ASCII.GetBytes(strOverlayGuid);
					overlayGuid = new Guid(s);
					msgExtractionStatus = MsgExtractionStatus.OVERLAYGUID_EXTRACTED;
				}
				else if(msgExtractionStatus == MsgExtractionStatus.OVERLAYGUID_EXTRACTED)
				{
					strBuffer = s;
					Console.WriteLine("haha 2");
					Console.WriteLine(s);
					Console.WriteLine(s.Length);
					byteBuffer = System.Text.Encoding.ASCII.GetBytes(strBuffer);
					
					//byte[] byteFromIP = System.Text.Encoding.ASCII.GetBytes(strFromIP);
					//IPAddress fromIP = new IPAddress(byteFromIP);
					
					IncomingMsg incomingMsg = new IncomingMsg();
					incomingMsg.strFromIP = strFromIP;
					incomingMsg.strToIP = strToIP;
					incomingMsg.strCallType = strCallType;
					incomingMsg.strOverlayGuid = strOverlayGuid;
					incomingMsg.extractedMsg = strBuffer;
					incomingMsg.completeMsg = byteContent;
					incomingMsg.offset = 0;
					incomingMsg.size = byteContent.Length;
					ParameterizedThreadStart processMsgJob = new ParameterizedThreadStart(processMsg);
					Thread processMsgThread = new Thread(processMsgJob);
					processMsgThread.Start(incomingMsg);
					//processMsg(strFromIP, strToIP, strOverlayGuid, strBuffer, byteContent, 0, byteContent.Length);
					msgExtractionStatus = MsgExtractionStatus.MESSAGE_EXTRACTED;
				}
			}
		}
		
		struct IncomingMsg
		{
			public String strFromIP;
			public String strToIP;
			public String strCallType;
			public String strOverlayGuid; 
			public string extractedMsg;
			public byte[] completeMsg; 
			public int offset; 
			public int size;

		}
		
		enum BootstrapState
		{
			NO_BOOTSTRAP,
			REQUEST_RECEIVED,
			NULL_RETURNED,
			OVERLAY_INSTANCE_GUID_RETURNED,
			OVERLAY_INSTANCE_GUID_RECEIVED
		}
		static Guid overlayInstanceGuid;
		static List<String> strBootstrapNodes = new List<String>();
		static BootstrapState bootstrapState = BootstrapState.NO_BOOTSTRAP;
		static Object bootStrapLock = new Object();
		static ManualResetEvent bootStrapAllDone = new ManualResetEvent(false);
		
		//static void processMsg(String strFromIP, String strToIP, String strOverlayGuid, string extractedMsg, byte[] completeMsg, int offset, int size)
		static void processMsg(Object incomingMsg)
		{
			String strFromIP = ((IncomingMsg)incomingMsg).strFromIP;
			String strToIP = ((IncomingMsg)incomingMsg).strToIP;
			String strCallType = ((IncomingMsg)incomingMsg).strCallType;
			String strOverlayGuid = ((IncomingMsg)incomingMsg).strOverlayGuid;
			string extractedMsg = ((IncomingMsg)incomingMsg).extractedMsg;
			byte[] completeMsg = ((IncomingMsg)incomingMsg).completeMsg;
			int offset = ((IncomingMsg)incomingMsg).offset;
			int size = ((IncomingMsg)incomingMsg).size;
				
			if(String.Compare(strToIP, "127.0.0.1") == 0)
			{
				Console.WriteLine("Boxit::processMsg Msg for server!");
				if(String.Compare(extractedMsg, "bootStrap request") == 0)
				{
					Console.WriteLine("Boxit::processMsg Msg to server: bootStrap request");

					lock(bootStrapLock)
					{
						//bootStrapAllDone.WaitOne();
						Console.WriteLine("Boxit::processMsg bootStrap request : inside lock");
						//bootstrapState = BootstrapState.REQUEST_RECEIVED;
						if(strBootstrapNodes.Count == 0)
						{
							Console.WriteLine("Boxit::processMsg Empty BootstrapNodes");
							//if(bootstrapState == BootstrapState.NULL_RETURNED)
							//	bootStrapAllDone.WaitOne();
							//else
							bootStrapAllDone.Reset();
							StringBuilder concatenatedMsg = new StringBuilder();
							concatenatedMsg.Append(strToIP);
							concatenatedMsg.Append('\0', 1);
							concatenatedMsg.Append(strFromIP);
							concatenatedMsg.Append('\0', 1);
							concatenatedMsg.Append(strCallType);
             		        concatenatedMsg.Append('\0', 1);
							concatenatedMsg.Append(strOverlayGuid);
							concatenatedMsg.Append('\0', 1);
							byte[] msg = {(byte)'n', (byte)'o', (byte)' ', (byte)'b', (byte)'o', (byte)'o', (byte)'t', (byte)'s', (byte)'t', (byte)'r', (byte)'a', (byte)'p', (byte)'n', (byte)'o', (byte)'d', (byte)'e'};
							concatenatedMsg.Append(Encoding.ASCII.GetString(msg));
							concatenatedMsg.Append('\0', 1);
							concatenatedMsg.Append('\n', 0);
						
							String strCompositeMsg = concatenatedMsg.ToString();
							int compositeMsgLen    = strCompositeMsg.Length;
							byte[] compositeMsg    = System.Text.Encoding.ASCII.GetBytes(strCompositeMsg);
				
							Console.WriteLine("Boxit::processMsg sending NULL back to client");
							sendMsg(strFromIP, compositeMsg, 0, compositeMsgLen);
							bootstrapState = BootstrapState.NULL_RETURNED;
							bootStrapAllDone.WaitOne();
						}
						else
						{
							Console.WriteLine("Boxit::processMsg NON Empty BootstrapNodes");
							
							StringBuilder concatenatedMsg = new StringBuilder();
							concatenatedMsg.Append(strToIP);
							concatenatedMsg.Append('\0', 1);
							concatenatedMsg.Append(strFromIP);
							concatenatedMsg.Append('\0', 1);
							concatenatedMsg.Append(strCallType);
             		        concatenatedMsg.Append('\0', 1);
							concatenatedMsg.Append(strOverlayGuid);
							concatenatedMsg.Append('\0', 1);
							concatenatedMsg.Append(strBootstrapNodes[0]);
							concatenatedMsg.Append('\t', 1);                  
							concatenatedMsg.Append(overlayInstanceGuid);
							concatenatedMsg.Append('\0', 1);
							concatenatedMsg.Append('\n', 0);
						
							String strCompositeMsg = concatenatedMsg.ToString();
							int compositeMsgLen    = strCompositeMsg.Length;
							byte[] compositeMsg    = System.Text.Encoding.ASCII.GetBytes(strCompositeMsg);
				
							Console.WriteLine("Boxit::processMsg sending overlay back to client");
							sendMsg(strFromIP, compositeMsg, 0, compositeMsgLen);
							bootstrapState = BootstrapState.OVERLAY_INSTANCE_GUID_RETURNED;
						}
					}
						
				}
				else if(bootstrapState == BootstrapState.NULL_RETURNED)
				{
					Console.WriteLine("Boxit::processMsg overlayInstanceGuid received from client");
					overlayInstanceGuid = new Guid(extractedMsg);
					//lock(bootStrapLock)
					{
						strBootstrapNodes.Add(strFromIP);
					}
					bootstrapState = BootstrapState.OVERLAY_INSTANCE_GUID_RECEIVED;
					bootStrapAllDone.Set();
				}
				else
				{
					Console.WriteLine("Boxit::processMsg ?????");
					
				}
			}
			else
			{
				Console.WriteLine("Boxit::processMsg Msg for some other client");
				sendMsg(strToIP, completeMsg, offset, size);
			}
				
				
				
		}
		static void sendMsg(String strToIP, byte[] msg, int offset, int size)
		{
			Triplet triplet;
			if(registry.TryGetValue(strToIP, out triplet))
			{
				Console.WriteLine("strToIP found in registry");
				SocketFlags f = new SocketFlags();
				
				if(triplet.sock != null)
					triplet.sock.BeginSend(msg, 0, msg.Length, f, null, null);
				else 
					establishRemoteConnection(strToIP, triplet, msg, 0, msg.Length);
					
			}
				
		}
		
			private static void establishRemoteConnection(String strToIP, Triplet triplet, byte[] msg, int offset, int size)
			{
				
				Console.WriteLine("Boxit::establishRemoteConnection ENTER");
				
				byte[] byteIP = {127, 0, 0, 1};
				IPAddress ipAddress = new IPAddress(byteIP);
				IPEndPoint ipEnd = new IPEndPoint (ipAddress, triplet.portNo);
				
				Console.WriteLine("Boxit::establishRemoteConnection endPoint created");
				//AsyncCallback beginConnectCallBack = new AsyncCallback(beginConnectCallBackFor_establishRemoteConnection);
				
				Console.WriteLine("Boxit::establishRemoteConnection before calling Connect");					
				triplet.sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				triplet.sock.Connect(ipEnd);
				
				Console.WriteLine("Boxit::establishRemoteConnection connect DONE");
				
				SocketFlags f = new SocketFlags();
				triplet.sock.BeginSend(msg, 0, msg.Length, f, null, null);
			}

				
}
