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
using System.Security.Cryptography;
using System.Net;
using System.Net.Sockets;

namespace Tashjik.Tier2.Chord.Common
{
	public static class UtilityMethod
	{
		public static SHA1 sha = new SHA1CryptoServiceProvider();
	
		public static IPAddress GetLocalHostIP()
		{
			String strHostName = Dns.GetHostName();

			// Find host by name
			IPHostEntry iphostentry = Dns.GetHostByName(strHostName);

			// Grab the first IP addresses
			foreach(IPAddress ipaddress in iphostentry.AddressList)
			{
				return ipaddress;
			}

			throw new Exception.LocalHostIPNotFoundException();
		}

		public static byte[] moduloAdd(byte[] A, byte[] B)
		{
			if(A.Length != B.Length)
				throw new Exception.BytesLengthsNotMatchingException();
			//int length = A.Length;
			byte[] C = new byte[A.Length];
			UInt16 tempA = 0;
			UInt16 tempB = 0;
			UInt16 tempC = 0;
			bool carry = false;


			for (int i=A.Length-1; i>=0; i--)
			{
		
				C[i] = 0;

				for(int j=15; j>=8; j--)
				{

					tempC = 0;

					tempA = (UInt16)(((UInt16) A[i]) << j);
					tempB = (UInt16)(((UInt16) B[i]) << j);

					tempA = (UInt16)(((UInt16) A[i]) >> 15);
					tempB = (UInt16)(((UInt16) B[i]) >> 15);

					if(carry==false)
					{
						if(tempA==0 && tempB==0)
						{
							tempC = 0;
							carry = false;
						}
						else if((tempA==0 && tempB==1) || (tempA==1 && tempB==0))
						{
							tempC = 1;
							carry = false;
						}
	
						else if(tempA==1 && tempB==1)
						{
							tempC = 0;
							carry = true;
						}
					}
					else if(carry==true)
					{
						if(tempA==0 && tempB==0)
						{
							tempC = 1;
							carry = false;
						}
						else if((tempA==0 && tempB==1) || (tempA==1 && tempB==0))
						{
							tempC = 0;
							carry = true;
						}
						else if(tempA==1 && tempB==1)
						{
							tempC = 1;
							carry = true;
						}
					}
					tempC = (UInt16)(((UInt16) tempC) >> (15-j));
	

					C[i] = (byte)(C[i] | (byte) tempC);
				}



			}

			if(carry==true)
			{
				byte[] D = new byte[C.Length];
				D[C.Length - 1] = 1;
				return moduloAdd(C, D);
			}
			else return C;

		}

		public static Socket CreateSocketConnection(IPAddress IP)
		{
			try
			{
				// Create the socket instance
				Socket newSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
		
				int iPortNo = System.Convert.ToInt16 ("2334");
				// Create the end point
				IPEndPoint ipEnd = new IPEndPoint (IP,iPortNo);
				// Connect to the remote host
				newSocket.Connect ( ipEnd );
				return newSocket;
			}
			catch(SocketException se)
			{
				Console.WriteLine(se.Message);
				throw se;
			}
		}	
	}
}
