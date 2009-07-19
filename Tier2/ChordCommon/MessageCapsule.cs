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

namespace Tashjik.Tier2.ChordCommon
{
	public class MessageCapsule
	{
		private readonly String type;
		private readonly byte[] hashedKey;
		private readonly IPAddress IP;
		private readonly Tashjik.Common.Data data;

		public bool matchedType(String t)
		{
			if(type.Equals(t))
				return true;
			else
				return false;
		}

		public byte[] getHashedKey()
		{
			byte[] copiedHashedKey = new byte[hashedKey.Length];
			Array.Copy(hashedKey, copiedHashedKey, hashedKey.Length);
			return copiedHashedKey;
	
		}

		public IPAddress getIP()
		{
			byte[] bytePossiblePredIP = System.Text.Encoding.ASCII.GetBytes(IP.ToString());
			IPAddress clonedIP = new IPAddress(bytePossiblePredIP);
			return clonedIP;

		}

		public Tashjik.Common.Data getData()
		{
			return data.getClone();
		}

		public MessageCapsule(String str, byte[] hK, IPAddress IPP)
		{
			type = str;
			hashedKey = hK;
			IP = IPP;
			data = null;
		}
	
		public MessageCapsule(String str, byte[] hK)
		{
			type = str;
			hashedKey = hK;
			IP = null;
			data = null;
		}

		public MessageCapsule(String str)
		{
			//hashedKey not assigned :O :O
			type = str;
			IP = null;
			data = null;
		}


		public MessageCapsule(String str, byte[] hK, Tashjik.Common.Data d)
		{
			type = str;
			hashedKey = hK;
			IP = null;
			data = new Tashjik.Common.Data(d);
		}
	}
}

