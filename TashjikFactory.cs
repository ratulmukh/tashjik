
using System;
using System.Net;
using Tashjik.Tier2;
//using Tashjik.Tier2.Streaming;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Pastry")]

namespace Tashjik
{
	internal class TashjikFactory
	{
		internal TashjikFactory()
		{
		}
		
		internal Server createServer(OverlayTypeEnum overlayType)
		{
//			if(overlayType==OverlayTypeEnum.BATON)
//				return new BATONServer();
			if(overlayType==OverlayTypeEnum.Pastry)
				return new PastryServer();
//			else if(overlay==OverlayTypeEnum.CAN)
//				return new CANServer();
//			else if(overlayType==OverlayTypeEnum.Chord)
//			    return new ChordServer(); 
//			else if(overlayType==OverlayTypeEnum.Narada)
//				return new NaradaServer();
			else 
				return null;
		   
		}
		
		internal Server createServer(OverlayTypeEnum overlayType, IPAddress joinOtherIP, Guid joinOtherGuid, ProxyController proxyController)
		{
//			if(overlayType==OverlayTypeEnum.BATON)
//				return new Tashjik.Tier2.BATONServer(joinOtherIP, joinOtherGuid, proxyController);
			if(overlayType==OverlayTypeEnum.Pastry)
				return new Tashjik.Tier2.PastryServer(joinOtherIP, joinOtherGuid, proxyController);
//			else if(strOverlay==OverlayTypeEnum.CAN)
//				return new Tashjik.Tier2.CANServer(joinOtherIP, joinOtherGuid, proxyController);
//			else if(overlayType==OverlayTypeEnum.Chord)
//			    return new Tashjik.Tier2.ChordServer(joinOtherIP, joinOtherGuid, proxyController);    
//			else if(overlayType==OverlayTypeEnum.Narada)
//			    return new Tashjik.Tier2.NaradaChordServer(joinOtherIP, joinOtherGuid, proxyController);    
			else
				return null;
		}
		
		internal NodeProxy createNodeProxy(OverlayTypeEnum overlayType, IPAddress IP, ProxyController proxyController)
		{
//			if(overlayType==OverlayTypeEnum.Chord)
//				return (NodeProxy)(new ChordNodeProxy(IP, proxyController));
			if(overlayType==OverlayTypeEnum.Pastry)
			    return (NodeProxy)(new PastryNodeProxy(IP, proxyController));    
			else
				return null; 
			
		}
	}
}
