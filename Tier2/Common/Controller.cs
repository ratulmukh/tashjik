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

namespace Tashjik.Tier2.Common
{
	public abstract class Controller : IController, Base.LowLevelComm.ISink
	{
		public interface ISink
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

		protected class OverlayInstanceInfo
		{
			public IOverlay overlay;
			public ISink sink;
			public OverlayInstanceInfo(IOverlay ov, ISink si)
			{
				overlay = ov;
				sink = si;
			}
		}

		//universal Chord GUID..to be used by all instances of Chord
		//private readonly Guid guid = new Guid("0c400880-0722-420e-a792-0a764d6539ee");

		protected readonly Guid guid;
		protected Dictionary<Guid, OverlayInstanceInfo> overlayInstanceRegistry =
			new Dictionary<Guid, OverlayInstanceInfo>();



		public Controller(Guid g)
		{
			guid = g;
			Base.LowLevelComm.getRefLowLevelComm().register(guid, this);

		}

		public ArrayList getList()
		{
			ArrayList guids = ArrayList.Synchronized(new ArrayList());

			foreach (KeyValuePair<Guid, OverlayInstanceInfo> kvp in overlayInstanceRegistry)
			guids.Add(kvp.Key);
			
			return guids;
		}

		public IOverlay retrieve(Guid guid)
		{
			OverlayInstanceInfo overlayInstanceInfo;
			if(overlayInstanceRegistry.TryGetValue(guid, out overlayInstanceInfo))
				return overlayInstanceInfo.overlay;
			else
				//need to change this exception
				throw new Tashjik.Common.Exception.LocalHostIPNotFoundException();
		}		

		/*
		public IOverlay create()
		{
			IOverlay chord = new Server();
			ISink sink = new ProxyController();
			OverlayInstanceInfo overlayInstanceInfo = new OverlayInstanceInfo(chord, sink);
			overlayInstanceRegistry.Add(chord.getGuid(), overlayInstanceInfo);
			return chord;
		}
		*/
		
		public abstract IOverlay create();		
	}
}
