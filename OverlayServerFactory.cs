
using System;
using System.Net;
//using Tashjik.Tier2;
//using Tashjik.Tier2.Streaming;
using System.Reflection;
//using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;

//[assembly:InternalsVisibleTo("Pastry")]

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
		
		internal OverlayServer createServer(String strOverlayType)
		{
			return createServer(strOverlayType, null, new Guid());
		}
		
		internal OverlayServer createServer(String strOverlayType, IPAddress joinOtherIP, Guid joinOtherGuid)
		{
			Console.WriteLine("OverlayServer::createServer ENTER");
			Type overlayServerType = null; 
			if(overlayServerTypeRegistry.TryGetValue(strOverlayType, out overlayServerType))
			{
				Console.WriteLine("OverlayServer::createServer overlayServerTypeRegistry.TryGetValue SUCCEEDED");
				Console.WriteLine("OverlayServer::createServer EXIT ");
				return (OverlayServer)(Activator.CreateInstance(overlayServerType));
			}
			else
			{
				Console.WriteLine("OverlayServer::createServer overlayServerTypeRegistry.TryGetValue FAILED");
				Assembly overlayAssembly = Assembly.Load(strOverlayType);
				Console.WriteLine("OverlayServer::createServer assembly loaded");
				String strOverlayServerType = strOverlayType + "Server";
				overlayServerType = overlayAssembly.GetType(strOverlayServerType);
				if(overlayServerType == null)
					Console.WriteLine("OverlayServer::createServer type retrieve FAILED");
				else
					Console.WriteLine("OverlayServer::createServer type retrieved");
				overlayServerTypeRegistry.Add(strOverlayType, overlayServerType );
				Console.WriteLine("OverlayServer::createServer type added to registry");		
				
				//Activator.CreateInstance is very slow and should be optimized 
				if(joinOtherIP==null || joinOtherGuid==null)
				{
					Console.WriteLine("OverlayServer::createServer joinOtherIP==null || joinOtherGuid==null");
					Console.WriteLine("OverlayServer::createServer EXIT ");
					return (OverlayServer)(Activator.CreateInstance((overlayServerType));
				}
				else
				{   
					Console.WriteLine("OverlayServer::createServer NOT [joinOtherIP==null || joinOtherGuid==null]");
					Object[] constructorArgs = new Object[2]{joinOtherIP, joinOtherGuid};
					Console.WriteLine("OverlayServer::createServer EXIT ");
					return (OverlayServer)(Activator.CreateInstance(overlayServerType, constructorArgs));
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
		internal ProxyNode createProxyNode(String strOverlayType, IPAddress IP, ProxyController proxyController)
		{
			Type[2] overlayTypes;
			Type overlayProxyNodeType; 
			if(overlayServerTypeRegistry.TryGetValue(strOverlayType, out overlayTypes))
			{
				overlayProxyNodeType = overlayTypes[1]
					
				Object[] constructorArgs = new Object[2]{IP, proxyController};
				return (ProxyNode)(Activator.CreateInstance(overlayServerType, constructorArgs));
			}
			else
			{
				//this place shld never be reached
			}

		}
*/

/*		
		internal ProxyNode createProxyNode(String strOverlayType, IPAddress IP, ProxyController proxyController)
		{
//			if(strOverlayType==String.Chord)
//				return (ProxyNode)(new ChordProxyNode(IP, proxyController));
			if(strOverlayType=="Pastry")
			    return (ProxyNode)(new PastryProxyNode(IP, proxyController));    
			else
				return null; 
			
		}
*/
	}
}
