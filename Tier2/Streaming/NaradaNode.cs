using System;
using System.Runtime.CompilerServices;
using Tashjik;

[assembly:InternalsVisibleTo("NaradaTest")]

namespace Tashjik.Tier2.Streaming
{
	internal class NaradaNode : INaradaNode
	{
		internal NaradaNode()
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
			return naradaEngine.getSelfPastryNodeBasicHashedIP();
			
		}
		
		private class NaradaEngine
		{
			private readonly Tashjik.Common.NodeBasic selfNaradaNodeBasic;
			private readonly NaradaNode self;
			
			public byte[] getSelfPastryNodeBasicHashedIP()
			{
				return selfNaradaNodeBasic.getHashedIP();
			}
		}
		
		NaradaEngine naradaEngine;
		
		
	}
}
