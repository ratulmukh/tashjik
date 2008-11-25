﻿/************************************************************
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

namespace Tashjik
{
	internal class CTashjik : ITashjik
	{
		public IController getController(String strOverlay)
		{
			if(strOverlay=="Chord")
				return getRefChordController();
			else if(strOverlay=="BATON")
				return getRefBATONController();
			else
				throw new Exception();
		}


		private static IController chordController = null;
		private static IController BATONController = null;
		private static IController pastryController = null;
		
		private static IController getRefChordController()
		{
			if(chordController != null)
				return chordController;
			else
			{
				chordController = new Tier2.Common.Controller(new Guid("0c400880-0722-420e-a792-0a764d6539ee"));
				return chordController;
			}
		}
		
		private static IController getRefBATONController()
		{
			if(BATONController != null)
				return BATONController;
			else
			{
				BATONController = new Tier2.Common.Controller(new Guid("59a86e1b-27d1-45bb-bbfe-b9cbfbb4fdd9"));
				return BATONController;
			}
		}
		
		private static IController getRefPastrydController()
		{
			if(pastryController != null)
				return pastryController;
			else
			{
				//new guid to be added here
				pastryController = new Tier2.Common.Controller(new Guid("0c400880-0722-420e-a792-0a764d6539ee"));
				return pastryController;
			}
		}
	}
}
