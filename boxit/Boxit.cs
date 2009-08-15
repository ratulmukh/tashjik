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
		public String portNo;
		public Socket sock;
		public Triplet(Process process, String portNo, Socket sock)
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
			
			process = Process.Start("TashjikClient.exe", strIP + " " + Convert.ToString(portNo));
			registry.Add(strIP, new Triplet(process, Convert.ToString(portNo), null));

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
        	        Console.WriteLine("Waiting for a connection at port " + UtilityMethod.GetPort());
        	        
        	        SocketState socketState = new SocketState();
					socketState.sock = listener;
					socketState.transportLayerCommunicator = TransportLayerCommunicator.getRefTransportLayerCommunicator();
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
			OVERLAYGUID_EXTRACTED,
			MESSAGE_EXTRACTED
		}
		static private void notifyUpperLayer(String content, Socket fromSock, TransportLayerCommunicator transportLayerCommunicator)
		{
			Console.WriteLine("TransportLayerCommunicator::notifyUpperLayer ENTER");
			String[] split = content.Split(new char[] {'\0'});
			
			String strFromIP = null;
			String strToIP = null;
			String strOverlayGuid;
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
					Console.WriteLine("FromIP received: ", s);
					msgExtractionStatus = MsgExtractionStatus.FROM_IP_EXTRACTED;
					
				}
				else if(msgExtractionStatus == MsgExtractionStatus.FROM_IP_EXTRACTED)
				{
					strToIP = s;	
					Console.WriteLine("ToIP received: ", s);
					msgExtractionStatus = MsgExtractionStatus.TO_IP_EXTRACTED;
						
					
				}
				else if(msgExtractionStatus == MsgExtractionStatus.TO_IP_EXTRACTED)
				{
					strOverlayGuid = s;
					Console.WriteLine("haha 1");
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
					
					byte[] byteFromIP = System.Text.Encoding.ASCII.GetBytes(strFromIP);
					IPAddress fromIP = new IPAddress(byteFromIP);
					
					Triplet triplet;
					if(registry.TryGetValue(strToIP, out triplet))
					{
						SocketFlags f = new SocketFlags();
						byte[] byteContent = System.Text.Encoding.ASCII.GetBytes(content);
						triplet.sock.BeginSend(byteContent, 0, byteContent.Length, f, null, null);
					}
					msgExtractionStatus = MsgExtractionStatus.MESSAGE_EXTRACTED;
				}
			}
		}


}
