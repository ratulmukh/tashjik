/************************************************************
* File Name: Bool_ObjectAsyncResult.cs
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
using System.Threading;

namespace Tashjik.Common
{
	public class Bool_ObjectAsyncResult : IAsyncResult
	{
		public Bool_ObjectAsyncResult(Bool_Object b, bool compSync, bool isComp)
		{
			AsyncState = (Object)b;
			AsyncWaitHandle = null;
			CompletedSynchronously = compSync;
			IsCompleted = isComp;
		}

		/* public AsynResultGetTashjik.Common.Data()
		{
			AsyncState = null;
			AsyncWaitHandle = null;
			CompletedSynchronously = false;
			IsCompleted = false;
		}
		*/
		
		private Bool_Object res;
		public Object AsyncState
		{
			get
			{
				//copy stuff from AsyncState to state
				//if casting dosen't work, then separate
				//private variable maintaining Tashjik.Common.Data may be
				//required
				return res;
			}
			set
			{
				//copy stuff from value to AsyncState
				//AyncState = (Common.Tashjik.Common.Data)value;
				res = (Bool_Object)value;

			}	
		}

		public WaitHandle AsyncWaitHandle
		{

			get
			{
				WaitHandle handle = new ManualResetEvent(false);
				//copy stuff from AsyncWaitHandle to handle
				return handle;
			}
			set
			{
				//copy stuff from value to AsyncState
				//AyncWaitHandle = value;
			}
		}
		
		public bool CompletedSynchronously
		{
			get
			{
				return CompletedSynchronously;
			}
			set
			{
				CompletedSynchronously = value;
			}
		}
		
		public bool IsCompleted
		{
			get
			{
				return IsCompleted;
			}
			set	
			{
				IsCompleted = value;
			}
		}

	}
}
