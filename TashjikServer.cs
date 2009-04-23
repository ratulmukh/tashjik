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
			Controller overlayController = getController(overlayGuid);
			return overlayController.getList();
			
		}
		public static ArrayList getList(String strOverlayType)
		{
			Controller overlayController = getController(strOverlayType);
			return overlayController.getList();
			
		}
		
		//get access to an overlay to which this node is already part of 
		public static IOverlay retrieve(Guid overlayGuid, Guid overlayInstanceGuid)
		{
			Controller overlayController = getController(overlayGuid);
			return overlayController.retrieve(overlayInstanceGuid);
		}
		
		//create a completely new overlay
		public static IOverlay createNew(Guid overlayGuid)
		{
			Controller overlayController = getController(overlayGuid);
			return overlayController.createNew();
			
		}
		public static IOverlay createNew(String strOverlayType)
		{
			Controller overlayController = getController(strOverlayType);
			return overlayController.createNew();
			
		}
		
		//join an existing overlay to which this node is not yet a part of 
		public static IOverlay joinExisting(IPAddress IP, Guid overlayGuid, Guid overlayInstanceGuid)
		{
			Controller overlayController = getController(overlayGuid);
			return overlayController.joinExisting(IP, overlayInstanceGuid);
		}
		public static IOverlay joinExisting(IPAddress IP, String strOverlayType, Guid overlayInstanceGuid)
		{
			Controller overlayController = getController(strOverlayType);
			return overlayController.joinExisting(IP, overlayInstanceGuid);
		}
		
		private static Controller getController(String strOverlayType)
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

		private static Controller getController(Guid overlayGuid)
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
		
		private static Controller chordController = null;
		private static Controller BATONController = null;
		private static Controller pastryController = null;
		
		private static Controller getRefChordController(String strOverlayType)
		{
			if(chordController != null)
				return chordController;
			else
			{
				chordController = new Controller(overlayServerFactory, new Guid(chordGUID), strOverlayType);
				return chordController;
			}
		}
		
		private static Controller getRefBATONController(String strOverlayType)
		{
			if(BATONController != null)
				return BATONController;
			else
			{
				BATONController = new Controller(overlayServerFactory, new Guid(BATONGUID), strOverlayType);
				return BATONController;
			}
		}
		
		private static Controller getRefPastrydController(String strOverlayType)
		{
			if(pastryController != null)
				return pastryController;
			else
			{
				//new guid to be added here
				pastryController = new Controller(overlayServerFactory, new Guid(pastryGUID), strOverlayType);
				return pastryController;
			}
		}
	}
}
