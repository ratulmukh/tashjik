/************************************************************
* File Name: 
*
* Author: Ratul Mukhopadhyay
* ratuldotmukhATgmaildotcom
*
* This software is licensed under the terms and conditions of
* the MIT license, as given below.
*
* Copyright (c) <2008> <Ratul Mukhopadhyay>
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
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
//using System.Runtime.CompilerServices;


//[assembly:InternalsVisibleTo("Tier2Common")]

namespace Tashjik
{
	internal class Controller : Base.LowLevelComm.ISink
	{
		internal interface ISink
		{
			void notifyMsgRec(IPAddress fromIP, Object data);
		}



		protected class OverlayInstanceMsg
		{
			public Guid guid;
			public Object data;
			public OverlayInstanceMsg(Guid g, Object o)
			{
				guid = g;
				data = o;
			}
		}

		public void notifyMsg(IPAddress fromIP, Object LowLevelData)
		{
			List<OverlayInstanceMsg> overlayInstanceMsgList = (List<OverlayInstanceMsg>)LowLevelData;
			foreach(OverlayInstanceMsg overlayInstanceMsg in overlayInstanceMsgList)
			{
				OverlayInstanceInfo overlayInstanceInfo;
				if(overlayInstanceRegistry.TryGetValue(overlayInstanceMsg.guid, out overlayInstanceInfo))
					overlayInstanceInfo.sink.notifyMsgRec(fromIP, overlayInstanceMsg.data);
				else
					throw new System.Exception();
			}
		}

		internal class OverlayInstanceInfo
		{
			public Server overlay;
			public ISink sink;
			public OverlayInstanceInfo(Server ov, ISink si)
			{
				overlay = ov;
				sink = si;
			}
		}
		
		private OverlayServerFactory overlayServerFactory;
		private readonly Guid guid;
		private readonly Dictionary<Guid, OverlayInstanceInfo> overlayInstanceRegistry =
			new Dictionary<Guid, OverlayInstanceInfo>();
		private readonly String strOverlayType;


		public  Controller(OverlayServerFactory overlayServerFactory, Guid g, String strOverlayType)
		{
			this.overlayServerFactory = overlayServerFactory ;
			guid = g;
			Base.LowLevelComm.getRefLowLevelComm().register(guid, this);
			this.strOverlayType = strOverlayType;
		}

		public  ArrayList getList()
		{
			ArrayList guids = ArrayList.Synchronized(new ArrayList());

			foreach (KeyValuePair<Guid, OverlayInstanceInfo> kvp in overlayInstanceRegistry)
			guids.Add(kvp.Key);
			
			return guids;
		}

		public  Server retrieve(Guid guid)
		{
			OverlayInstanceInfo overlayInstanceInfo;
			if(overlayInstanceRegistry.TryGetValue(guid, out overlayInstanceInfo))
				return overlayInstanceInfo.overlay;
			else
				//need to change this exception
				throw new Tashjik.Common.Exception.LocalHostIPNotFoundException();
		}		

		public  Server createNew()
		{
			Server overlay = overlayServerFactory.createServer(strOverlayType);
			ISink sink = overlay.getProxyController();
			OverlayInstanceInfo overlayInstanceInfo = new OverlayInstanceInfo(overlay, sink);
			overlayInstanceRegistry.Add(overlay.getGuid(), overlayInstanceInfo);
			return overlay;
		}

		public  Server joinExisting(IPAddress IP, Guid guid)
		{
			Server overlay = overlayServerFactory.createServer(strOverlayType, IP, guid);
			ISink sink = overlay.getProxyController();
			//Server overlay = new Server(IP, guid, (Tier2.Common.ProxyController)(sink));
			OverlayInstanceInfo overlayInstanceInfo = new OverlayInstanceInfo(overlay, sink);
			overlayInstanceRegistry.Add(overlay.getGuid(), overlayInstanceInfo);
			return overlay;
		}
/*
		
		private Tier2.Common.Server createServer(IPAddress joinOtherIP, Guid joinOtherGuid, Tier2.Common.ProxyController proxyController)
		{
			if(overlayType==OverlayTypeEnum.BATON)
				return new Tashjik.Tier2.BATONServer(joinOtherIP, joinOtherGuid, proxyController);
			else if(overlayType==OverlayTypeEnum.Pastry)
				return new Tashjik.Tier2.PastryServer(joinOtherIP, joinOtherGuid, proxyController);
//			else if(strOverlay=="CAN")
//				return new Tashjik.Tier2.CAN.Server(joinOtherIP, joinOtherGuid, proxyController);
			else if(overlayType==OverlayTypeEnum.Chord)
			    return new Tashjik.Tier2.ChordServer(joinOtherIP, joinOtherGuid, proxyController);    
			else
				return null;
		}
		
		private Tier2.Common.Server createServer()
		{
			if(overlayType==OverlayTypeEnum.BATON)
				return new Tashjik.Tier2.BATONServer();
			else if(overlayType==OverlayTypeEnum.Pastry)
				return new Tashjik.Tier2.PastryServer();
//			else if(overlay=="CAN")
	//			return new Tashjik.Tier2.CAN.Server();
			else if(overlayType==OverlayTypeEnum.Chord)
			    return new Tashjik.Tier2.ChordServer(); 
			else 
				return null;
		   
		}
*/
	}
}
