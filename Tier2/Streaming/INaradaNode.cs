using System;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("NaradaTest")]

namespace Tashjik.Tier2.Streaming
{

	internal interface INaradaNode
	{
		void join(INaradaNode newNode);
		void leave();
		
		
		
		
		byte[] getHashedIP();
	}
}
