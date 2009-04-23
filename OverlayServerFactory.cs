
using System;
using System.Net;
using Tashjik.Tier2;
//using Tashjik.Tier2.Streaming;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;

[assembly:InternalsVisibleTo("Pastry")]

namespace Tashjik
{
	internal class OverlayServerFactory
	{
		private Dictionary<String, Type> overlayServerTypeRegistry;
		internal OverlayServerFactory()
		{	
			init();
		}
		
		internal void init()
		{
			overlayServerTypeRegistry = new Dictionary<String, Type>();
		}
		
		internal Server createServer(String strOverlayType)
		{
			createServer(strOverlayType, null, null, null);
		
		}
		
		internal Server createServer(String strOverlayType, IPAddress joinOtherIP, Guid joinOtherGuid, ProxyController proxyController)
		{
			
			Type overlayServerType; 
			if(overlayServerTypeRegistry.TryGetValue(strOverlayType, out overlayServerType))
			{
				return (Server)(Activator.CreateInstance(overlayServerType));
			}
			else
			{
				Assembly overlayAssembly = Assembly.Load(strOverlayType);
				String strOverlayServerType = strOverlayType + "Server";
				overlayServerType = overlayAssembly.GetType(strOverlayServerType);
				overlayServerTypeRegistry.Add(strOverlayType, overlayServerType );
						
				
				//Activator.CreateInstance is very slow and should be optimized 
				if(joinOtherIP==null || joinOtherGuid==null || proxyController==null)
					return (Server)(Activator.CreateInstance(overlayServerType));
				else
				{
					Object[] constructorArgs = new Object[3]{joinOtherIP, joinOtherGuid, proxyController};
					return (Server)(Activator.CreateInstance(overlayServerType, constructorArgs));
				}

			}
		
		}
/*		
		internal Server createServer(String strOverlayType)
		{
//			if(strOverlayType==String.BATON)
//				return new BATONServer();
			if(strOverlayType==String.Pastry)
				return new PastryServer();
//			else if(overlay==String.CAN)
//				return new CANServer();
//			else if(strOverlayType==String.Chord)
//			    return new ChordServer(); 
//			else if(strOverlayType==String.Narada)
//				return new NaradaServer();
			else 
				return null;
		   
		}
		
		internal Server createServer(String strOverlayType, IPAddress joinOtherIP, Guid joinOtherGuid, ProxyController proxyController)
		{
//			if(strOverlayType==String.BATON)
//				return new Tashjik.Tier2.BATONServer(joinOtherIP, joinOtherGuid, proxyController);
			if(strOverlayType==String.Pastry)
				return new Tashjik.Tier2.PastryServer(joinOtherIP, joinOtherGuid, proxyController);
//			else if(strOverlay==String.CAN)
//				return new Tashjik.Tier2.CANServer(joinOtherIP, joinOtherGuid, proxyController);
//			else if(strOverlayType==String.Chord)
//			    return new Tashjik.Tier2.ChordServer(joinOtherIP, joinOtherGuid, proxyController);    
//			else if(strOverlayType==String.Narada)
//			    return new Tashjik.Tier2.NaradaChordServer(joinOtherIP, joinOtherGuid, proxyController);    
			else
				return null;
		}
*/
/*
		internal NodeProxy createNodeProxy(String strOverlayType, IPAddress IP, ProxyController proxyController)
		{
			Type[2] overlayTypes;
			Type overlayNodeProxyType; 
			if(overlayServerTypeRegistry.TryGetValue(strOverlayType, out overlayTypes))
			{
				overlayNodeProxyType = overlayTypes[1]
					
				Object[] constructorArgs = new Object[2]{IP, proxyController};
				return (NodeProxy)(Activator.CreateInstance(overlayServerType, constructorArgs));
			}
			else
			{
				//this place shld never be reached
			}

		}
*/

		
		internal NodeProxy createNodeProxy(String strOverlayType, IPAddress IP, ProxyController proxyController)
		{
//			if(strOverlayType==String.Chord)
//				return (NodeProxy)(new ChordNodeProxy(IP, proxyController));
			if(strOverlayType=="Pastry")
			    return (NodeProxy)(new PastryNodeProxy(IP, proxyController));    
			else
				return null; 
			
		}

	}
}
