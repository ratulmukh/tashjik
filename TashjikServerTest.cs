/************************************************************
* File Name: TashjikServerTest.cs
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
//using Tashjik;
//using Tashjik.Tier2;
using System.Collections;
using TashjikTest.Tier2Test.StreamingTest.NaradaTest;

namespace TashjikTest
{
	/// <summary>
	/// Description of TashjikServerTest_cs.
	/// </summary>
	public class TashjikServerTest
	{
		public TashjikServerTest()
		{
			
		}
		
		public void exerciseTests()
		{
			NaradaTestExerciser naradaTestExerciser = new NaradaTestExerciser();
		}
		
		public static void Main()
		{
		/*	Guid g = Guid.NewGuid();
			Console.Write(g.ToString());
			
			//ChordServer chord = (ChordServer)(TashjikServer.createNew(OverlayTypeEnum.Chord));
			ArrayList arr = TashjikServer.getList(OverlayTypeEnum.Chord);
			ChordServer chord = (ChordServer)(arr[0]);
			
			String key = "key";
			Tashjik.Common.Data data = new Tashjik.Common.Data();
			AsyncCallback putDataCallBack = new AsyncCallback(processPutDataCallBack);
			//chord.beginGetData(key, data, putDataCallBack, null);
		*/	
		TashjikServerTest tashjikServerTest = new TashjikServerTest();
		tashjikServerTest.exerciseTests();
			
		}
		
		static void processPutDataCallBack(IAsyncResult result)
		{
			
		}
		
		
	}
}
