/************************************************************
* File Name: ChordDataStore.cs 
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
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Tashjik.Tier2
{
	internal class ChordDataStore
	{
		//IOldController controller;
	
		//public DataStore(IOldController con)
		//{
		// controller = con;
		//}

		public ChordDataStore()
		{

		}	

		private Dictionary<String, byte[]> dataHolder =
			new Dictionary<String, byte[]>();

		public void putData(byte[] key, byte[] data, int offset, int size)
		{
			Console.WriteLine("ChordDataStore::putData ENTER");
			String strData = Encoding.ASCII.GetString(data);
			String strExtractedData = strData.Substring(offset, size);
			byte[] extractedData = System.Text.Encoding.ASCII.GetBytes(strExtractedData);

			Console.Write("ChordDataStore::putData key = ");
			Console.WriteLine(Encoding.ASCII.GetString(key));
			Console.Write("ChordDataStore::putData extractedData = ");
			Console.WriteLine(Encoding.ASCII.GetString(data));
			dataHolder.Add(Encoding.ASCII.GetString(key), extractedData);
					
			Console.WriteLine("ChordDataStore::putData EXIT");
		}

		public byte[] getData(byte[] key)
		{
			Console.WriteLine("ChordDataStore::getData ENTER");
			Console.Write("ChordDataStore::getData hashedKey = ");			                 
			Console.WriteLine(Encoding.ASCII.GetString(key));
			
			Dictionary<String, byte[]>.Enumerator enumerator = dataHolder.GetEnumerator();
			for(int i=0; i<dataHolder.Count;i++)
			{
				enumerator.MoveNext();
				Console.Write(enumerator.Current.Key);
				Console.WriteLine(Encoding.ASCII.GetString(enumerator.Current.Value));
				if(enumerator.Current.Key.Equals(Encoding.ASCII.GetString(key)))
	            	Console.WriteLine("both strings match");
				else
					Console.WriteLine("both strings DON'T match");
			}
			
						
			byte[] data;
			if(dataHolder.ContainsKey(Encoding.ASCII.GetString(key)))
				Console.WriteLine("ChordDataStore::getData key found");
			else
				Console.WriteLine("ChordDataStore::getData key NOT found");
			if(dataHolder.TryGetValue(Encoding.ASCII.GetString(key), out data))
				return data;
			else
				throw new Chord.Exception.DataNotFoundInStoreException();
	

		}
	}
}
