using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("NaradaTest")]

namespace Tashjik.Tier2.Streaming
{

	internal class NaradaProxyNode : ProxyNode, INaradaNode
	{
		internal static NaradaRealNode thisNode;
	
		private Tashjik.Common.NodeBasic selfNodeBasic;
		private Base.LowLevelComm lowLevelComm;	
		
		
		public NaradaProxyNode(IPAddress ip/*, ProxyController proxyController*/)
		{
			lowLevelComm = Base.LowLevelComm.getRefLowLevelComm();
			selfNodeBasic = new Tashjik.Common.NodeBasic(ip);
			//setProxyController(proxyController);
		}
		
		public override byte[] getHashedIP()
		{
			return selfNodeBasic.getHashedIP();
		}

		public override IPAddress getIP()
		{
			return selfNodeBasic.getIP();
		}

		public override void setIP(IPAddress ip)
		{
			selfNodeBasic.setIP(ip);
		}
		
		public override void beginNotifyMsgRec(IPAddress fromIP, Object data, AsyncCallback notifyMsgRecCallBack, Object appState)
		{
			
		}
		
		//need to conved join request to the real node
		public void join(INaradaNode newNode)
		{
			
		}
	}
}
