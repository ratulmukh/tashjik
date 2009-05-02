using System;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("NaradaTest")]

namespace Tashjik.Tier2.Streaming
{
	internal class NaradaRealNode : Tashjik.RealNode, INaradaNode
	{
		class NaradaEngine
		{
			private readonly Tashjik.Common.NodeBasic selfNaradaNodeBasic;
			private readonly NaradaRealNode encapsulatingRealNode;
			private readonly NaradaServer naradaServer;
			
			public NaradaEngine(NaradaRealNode encapsulatingRealNode, NaradaServer naradaServer)
			{
				this.encapsulatingRealNode = encapsulatingRealNode;
				this.naradaServer = naradaServer;
				
				try
				{
					selfNaradaNodeBasic = new Tashjik.Common.NodeBasic(Tashjik.Common.UtilityMethod.GetLocalHostIP());
				}
				catch (Tashjik.Common.Exception.LocalHostIPNotFoundException e)
				{
					//local ip could not be found :O :O
					//crash the system
					//dunno how to do it though :(
				}
				
				
				//initialize();

			}
			
			public NaradaEngine(NaradaRealNode encapsulatingRealNode, NaradaServer naradaServer, INaradaNode joinOtherNaradaNode)
			{
				this.encapsulatingRealNode = encapsulatingRealNode;
				this.naradaServer = naradaServer;
				
				try
				{
					selfNaradaNodeBasic = new Tashjik.Common.NodeBasic(Tashjik.Common.UtilityMethod.GetLocalHostIP());
				}
				catch (Tashjik.Common.Exception.LocalHostIPNotFoundException e)
				{
					//local ip could not be found :O :O
					//crash the system
					//dunno how to do it though :(
				}
				
				
				initialize(joinOtherNaradaNode);
			}
			void initialize(INaradaNode joinOtherNaradaNode)
			{
				joinOtherNaradaNode.join(encapsulatingRealNode);
			}
			
			//received a join request froma remorte node
			public void join(INaradaNode newNode)
			{
				
			}
		}
		
		NaradaEngine naradaEngine;
		
		internal NaradaRealNode(NaradaServer naradaServer)
		{
			naradaEngine = new NaradaEngine(this, naradaServer);
		}
		
		public NaradaRealNode(NaradaServer naradaServer, INaradaNode joinOtherNaradaNode)
		{
			naradaEngine = new NaradaEngine(this, naradaServer, joinOtherNaradaNode);
		}
		
		//received a join request froma remorte node
		public void join(INaradaNode newNode)
		{
			naradaEngine.join(newNode);
		}
	}
}
