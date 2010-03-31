/************************************************************
* File Name: OverlayServerFactory.cs
*
* Author: Ratul Mukhopadhyay
* ratuldotmukhATgmaildotcom
*
* This software is licensed under the terms and conditions of
* the MIT license, as given below.
*
* Copyright (c) <2008-2010> <Ratul Mukhopadhyay>
*
* Permission is hereby granted, free of charge, to any person
* obtaining a copy of this software and associated documentation
* files (the "Software"), to deal in the Software without
* restriction, including without limitation the rights to use,
* copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the
* Software is furnished to do so, subject to the following
* conditions:
*
* The above copyright notice and this permission notice shall be
* included in all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
* OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
* NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
* HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
* WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
* OTHER DEALINGS IN THE SOFTWARE.
*
*
* Description:
* 
* 
*
* Supporting Documentation:
*
* Portability: .NET VES
* Status: Experimental
* Reuse Reviews:
* Date Name Comment
*
* Modification History:
*
************************************************************/
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
			Console.WriteLine("OverlayServerFactory::createServer ENTER");
			Type overlayServerType = null; 
			if(overlayServerTypeRegistry.TryGetValue(strOverlayType, out overlayServerType))
			{
				Console.WriteLine("OverlayServerFactory::createServer overlayServerTypeRegistry.TryGetValue SUCCEEDED");
				Console.WriteLine("OverlayServerFactory::createServer EXIT ");
				return (OverlayServer)(Activator.CreateInstance(overlayServerType));
			}
			else
			{
				Console.WriteLine("OverlayServerFactory::createServer overlayServerTypeRegistry.TryGetValue FAILED");
				Assembly overlayAssembly = Assembly.Load("Tier2"+strOverlayType);
				if(overlayAssembly == null)
					Console.WriteLine("OverlayServerFactory::createServer assembly load FAILED");
				else
					Console.WriteLine("OverlayServerFactory::createServer assembly loaded");
				String strOverlayServerType = "Tashjik.Tier2." + strOverlayType + "Server";
				Console.WriteLine(strOverlayServerType);
				overlayServerType = overlayAssembly.GetType(strOverlayServerType);
				if(overlayServerType == null)
					Console.WriteLine("OverlayServerFactory::createServer type retrieve FAILED");
				else
					Console.WriteLine("OverlayServerFactory::createServer type retrieved");
				overlayServerTypeRegistry.Add(strOverlayType, overlayServerType );
				Console.WriteLine("OverlayServerFactory::createServer type added to registry");		
				
				//Activator.CreateInstance is very slow and should be optimized 
				if(joinOtherIP==null || joinOtherGuid==null)
				{
					Console.WriteLine("OverlayServerFactory::createServer joinOtherIP==null || joinOtherGuid==null");
					Console.WriteLine("OverlayServerFactory::createServer EXIT ");
					return (OverlayServer)(Activator.CreateInstance(overlayServerType));
				}
				else
				{   
					Console.WriteLine("OverlayServerFactory::createServer NOT [joinOtherIP==null || joinOtherGuid==null]");
					Object[] constructorArgs = new Object[2]{joinOtherIP, joinOtherGuid};
					Console.WriteLine("OverlayServerFactory::createServer EXIT ");
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
