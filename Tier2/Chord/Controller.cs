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
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Tashjik.Tier2.Chord
{
	public class Controller : IController, Base.LowLevelComm.ISink
	{
		public interface ISink
		{
			void notifyMsgRec(IPAddress fromIP, Object data);
		}



		public class ChordInstanceMsg
		{
			public Guid chordInstanceGuid;
			public Object data;
			public ChordInstanceMsg(Guid g, Object o)
			{
				chordInstanceGuid = g;
				data = o;
			}
		}

		public void notifyMsg(IPAddress fromIP, Object LowLevelData)
		{
			List<ChordInstanceMsg> chordInstanceMsgList = (List<ChordInstanceMsg>)LowLevelData;
			foreach(ChordInstanceMsg chordInstanceMsg in chordInstanceMsgList)
			{
				ChordInstanceInfo chordInstanceInfo;
				if(chordInstanceRegistry.TryGetValue(chordInstanceMsg.chordInstanceGuid, out chordInstanceInfo))
					chordInstanceInfo.sink.notifyMsgRec(fromIP, chordInstanceMsg.data);
				else
					throw new System.Exception();
			}
		}

		private class ChordInstanceInfo
		{
			public IOverlay chord;
			public ISink sink;
			public ChordInstanceInfo(IOverlay ch, ISink si)
			{
				chord = ch;
				sink = si;
			}
		}

		//universal Chord GUID..to be used by all instances of Chord
		private readonly Guid guid = new Guid("0c400880-0722-420e-a792-0a764d6539ee");

		private Dictionary<Guid, ChordInstanceInfo> chordInstanceRegistry =
			new Dictionary<Guid, ChordInstanceInfo>();



		public Controller()
		{

			Base.LowLevelComm.getRefLowLevelComm().register(guid, this);

		}

		public ArrayList getList()
		{
			ArrayList guids = ArrayList.Synchronized(new ArrayList());

			foreach (KeyValuePair<Guid, ChordInstanceInfo> kvp in chordInstanceRegistry)
			guids.Add(kvp.Key);
			
			return guids;
		}

		public IOverlay retrieve(Guid guid)
		{
			ChordInstanceInfo chordInstanceInfo;
			if(chordInstanceRegistry.TryGetValue(guid, out chordInstanceInfo))
				return chordInstanceInfo.chord;
			else
				//need to change this exception
				throw new Exception.DataNotFoundInStoreException();
		}		

		public IOverlay create()
		{
			IOverlay chord = new Server();
			ISink sink = new ProxyController();
			ChordInstanceInfo chordInstanceInfo = new ChordInstanceInfo(chord, sink);
			chordInstanceRegistry.Add(chord.getGuid(), chordInstanceInfo);
			return chord;
		}


	
	}
}
