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
using System.Collections;
using System.Collections.Generic;

namespace Tashjik.Tier2.Chord
{
	class DataStore
	{
		//IOldController controller;
	
		//public DataStore(IOldController con)
		//{
		// controller = con;
		//}

		public DataStore()
		{

		}	

		private Dictionary<byte[], Tashjik.Common.Data> dataHolder =
			new Dictionary<byte[], Tashjik.Common.Data>();

		public void putData(byte[] hashedKey, Tashjik.Common.Data data)
		{
			dataHolder.Add(hashedKey, data);
		}

		public Tashjik.Common.Data getData(byte[] hashedKey)
		{
			Tashjik.Common.Data data = new Tashjik.Common.Data();
			if(dataHolder.TryGetValue(hashedKey, out data))
				return data;
			else
				throw new Exception.DataNotFoundInStoreException();
	

		}
	}
}