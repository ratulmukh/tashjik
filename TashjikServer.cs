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
using System.Net;

namespace Tashjik
{
/*	public enum String
	{
		Chord,
		BATON,
		Pastry, 
		CAN,
		Narada
	}
*/	
	public static class TashjikServer 
	{
		
		private static OverlayServerFactory overlayServerFactory = new OverlayServerFactory();
		
	/*	public TashjikServer()
		{
			init();
		}
		
		private static void init()
		{
			overlayServerFactory = new OverlayServerFactory();
		}
	*/	
		public static ArrayList getList(Guid overlayGuid)
		{
			OverlayController overlayController = getController(overlayGuid);
			return overlayController.getList();
			
		}
		public static ArrayList getList(String strOverlayType)
		{
			OverlayController overlayController = getController(strOverlayType);
			return overlayController.getList();
			
		}
		
		//get access to an overlay to which this node is already part of 
		public static OverlayServer retrieve(Guid overlayGuid, Guid overlayInstanceGuid)
		{
			OverlayController overlayController = getController(overlayGuid);
			return overlayController.retrieve(overlayInstanceGuid);
		}
		
		//create a completely new overlay
		public static OverlayServer createNew(Guid overlayGuid)
		{
			Console.WriteLine("TashjikServer::createNew ENTER");
			OverlayController overlayController = getController(overlayGuid);
			return overlayController.createNew();
			
		}
		public static OverlayServer createNew(String strOverlayType)
		{
			Console.WriteLine("TashjikServer::createNew ENTER");
			OverlayController overlayController = getController(strOverlayType);
			Console.WriteLine("TashjikServer::createNew after getting overlayController");
			return overlayController.createNew();
			
		}
		
		//join an existing overlay to which this node is not yet a part of 
		public static OverlayServer joinExisting(IPAddress IP, Guid overlayGuid, Guid overlayInstanceGuid)
		{
			OverlayController overlayController = getController(overlayGuid);
			return overlayController.joinExisting(IP, overlayInstanceGuid);
		}
		public static OverlayServer joinExisting(IPAddress IP, String strOverlayType, Guid overlayInstanceGuid)
		{
			OverlayController overlayController = getController(strOverlayType);
			return overlayController.joinExisting(IP, overlayInstanceGuid);
		}
		
		private static OverlayController getController(String strOverlayType)
		{
			if(strOverlayType=="Chord")
				return getRefChordController(strOverlayType);
			else if(strOverlayType=="BATON")
				return getRefBATONController(strOverlayType);
			else if(strOverlayType=="Pastry")
				return getRefBATONController(strOverlayType);
			else
				throw new Exception();
		}

		private static OverlayController getController(Guid overlayGuid)
		{
			if(overlayGuid==new Guid(chordGUID))
				return getRefChordController("Chord");
			else if(overlayGuid==new Guid(BATONGUID))
				return getRefBATONController("BATON");
			else if(overlayGuid==new Guid(pastryGUID))
				return getRefBATONController("Pastry");
			else
				throw new Exception();
		}
		
		private const string chordGUID  = "0c400880-0722-420e-a792-0a764d6539ee";
		private const string BATONGUID  = "59a86e1b-27d1-45bb-bbfe-b9cbfbb4fdd9";
		private const string pastryGUID = "73dc00d1-40e9-4111-91a5-fa55881f0e35";
		
		private static OverlayController chordController = null;
		private static OverlayController BATONController = null;
		private static OverlayController pastryController = null;
		
		private static OverlayController getRefChordController(String strOverlayType)
		{
			if(chordController != null)
				return chordController;
			else
			{
				chordController = new OverlayController(overlayServerFactory, new Guid(chordGUID), strOverlayType);
				return chordController;
			}
		}
		
		private static OverlayController getRefBATONController(String strOverlayType)
		{
			if(BATONController != null)
				return BATONController;
			else
			{
				BATONController = new OverlayController(overlayServerFactory, new Guid(BATONGUID), strOverlayType);
				return BATONController;
			}
		}
		
		private static OverlayController getRefPastrydController(String strOverlayType)
		{
			if(pastryController != null)
				return pastryController;
			else
			{
				//new guid to be added here
				pastryController = new OverlayController(overlayServerFactory, new Guid(pastryGUID), strOverlayType);
				return pastryController;
			}
		}
	}
}
