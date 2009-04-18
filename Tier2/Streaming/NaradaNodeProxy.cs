using System;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("NaradaTest")]

namespace Tashjik.Tier2.Streaming
{

	internal class NaradaNodeProxy : INaradaNode
	{
		internal static NaradaNode thisNode;
	
		private Tashjik.Common.NodeBasic selfNodeBasic;
		private Base.LowLevelComm lowLevelComm;		
		
		private Tier2.Common.ProxyController proxyController;
		
		internal NaradaNodeProxy()
		{
		}
		
		void join(INaradaNode newNode)
		{
			
		}
		void leave()
		{
			
		}
		
		
		
		
		byte[] getHashedIP()
		{
			return selfNodeBasic.getHashedIP();
		}
	}
}
