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
	public enum OverlayTypeEnum
	{
		Chord,
		BATON,
		Pastry, 
		CAN,
		Narada
	}
	
	public static class TashjikServer 
	{
		
		private static TashjikFactory tashjikFactory = new TashjikFactory();
		
	/*	public TashjikServer()
		{
			init();
		}
		
		private static void init()
		{
			tashjikFactory = new TashjikFactory();
		}
	*/	
		public static ArrayList getList(Guid overlayGuid)
		{
			Controller overlayController = getController(overlayGuid);
			return overlayController.getList();
			
		}
		public static ArrayList getList(OverlayTypeEnum overlayType)
		{
			Controller overlayController = getController(overlayType);
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
		public static IOverlay createNew(OverlayTypeEnum overlayType)
		{
			Controller overlayController = getController(overlayType);
			return overlayController.createNew();
			
		}
		
		//join an existing overlay to which this node is not yet a part of 
		public static IOverlay joinExisting(IPAddress IP, Guid overlayGuid, Guid overlayInstanceGuid)
		{
			Controller overlayController = getController(overlayGuid);
			return overlayController.joinExisting(IP, overlayInstanceGuid);
		}
		public static IOverlay joinExisting(IPAddress IP, OverlayTypeEnum overlayType, Guid overlayInstanceGuid)
		{
			Controller overlayController = getController(overlayType);
			return overlayController.joinExisting(IP, overlayInstanceGuid);
		}
		
		private static Controller getController(OverlayTypeEnum overlayType)
		{
			if(overlayType==OverlayTypeEnum.Chord)
				return getRefChordController(overlayType);
			else if(overlayType==OverlayTypeEnum.BATON)
				return getRefBATONController(overlayType);
			else if(overlayType==OverlayTypeEnum.Pastry)
				return getRefBATONController(overlayType);
			else
				throw new Exception();
		}

		private static Controller getController(Guid overlayGuid)
		{
			if(overlayGuid==new Guid(chordGUID))
				return getRefChordController(OverlayTypeEnum.Chord);
			else if(overlayGuid==new Guid(BATONGUID))
				return getRefBATONController(OverlayTypeEnum.BATON);
			else if(overlayGuid==new Guid(pastryGUID))
				return getRefBATONController(OverlayTypeEnum.Pastry);
			else
				throw new Exception();
		}
		
		private const string chordGUID  = "0c400880-0722-420e-a792-0a764d6539ee";
		private const string BATONGUID  = "59a86e1b-27d1-45bb-bbfe-b9cbfbb4fdd9";
		private const string pastryGUID = "73dc00d1-40e9-4111-91a5-fa55881f0e35";
		
		private static Controller chordController = null;
		private static Controller BATONController = null;
		private static Controller pastryController = null;
		
		private static Controller getRefChordController(OverlayTypeEnum overlayType)
		{
			if(chordController != null)
				return chordController;
			else
			{
				chordController = new Controller(tashjikFactory, new Guid(chordGUID), overlayType);
				return chordController;
			}
		}
		
		private static Controller getRefBATONController(OverlayTypeEnum overlayType)
		{
			if(BATONController != null)
				return BATONController;
			else
			{
				BATONController = new Controller(tashjikFactory, new Guid(BATONGUID), overlayType);
				return BATONController;
			}
		}
		
		private static Controller getRefPastrydController(OverlayTypeEnum overlayType)
		{
			if(pastryController != null)
				return pastryController;
			else
			{
				//new guid to be added here
				pastryController = new Controller(tashjikFactory, new Guid(pastryGUID), overlayType);
				return pastryController;
			}
		}
	}
}
