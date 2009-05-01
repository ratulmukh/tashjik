using System;
using System.Net;
using System.Net.Sockets;
//using System.Runtime.CompilerServices;

//[assembly:InternalsVisibleTo("NaradaTest")]

namespace Tashjik.Tier2.Streaming
{

	public class NaradaServer : StreamingOverlayServer
	{
		
		private  readonly Guid guid;
		private readonly NaradaRealNode thisNode;
		
		internal NaradaServer()
		{
			guid = System.Guid.NewGuid();
			thisNode = new NaradaRealNode();
			OverlayServer.CreateProxyNodeDelegate createProxyNodeDelegate = new  OverlayServer.CreateProxyNodeDelegate(createNaradaProxyNode);
			base.setCreateProxyNodeDelegate(createProxyNodeDelegate);
		}
		
		private ProxyNode createNaradaProxyNode(IPAddress IP)
		{
			return new NaradaProxyNode(IP);
		}
		
		internal INaradaNode getNaradaProxyNode(IPAddress IP)
		{      
			return (INaradaNode)(base.getProxyNode(IP));
		}
		
		public override Guid getGuid()
		{
			return guid;
		}
		
		public override void shutdown()
		{
			
		}
		
		
		
		
		
	}
}
