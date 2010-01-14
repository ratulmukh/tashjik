/************************************************************
* File Name:UtilityMethod.cs 
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

#define SIM  

using System;
using System.Security.Cryptography;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Tashjik.Common
{
	public static class UtilityMethod
	{
		public static SHA1 sha = new SHA1CryptoServiceProvider();

#if SIM
		private static IPAddress localIP;
		private static String port;

		public static void SetLocalHostIP(IPAddress IP)
		{
			localIP = IP;
		}
		public static void SetPort(string po)
		{
			port = po;
		}
		
		public static String GetPort()
		{
			return port;
		}
#endif	
		public static IPAddress GetLocalHostIP()
		{

#if SIM
			return localIP ;
#else
			String strHostName = Dns.GetHostName();

			// Find host by name
			IPHostEntry iphostentry = Dns.GetHostByName(strHostName);

			// Grab the first IP addresses
			foreach(IPAddress ipaddress in iphostentry.AddressList)
			{
				return ipaddress;
			}

			throw new Exception.LocalHostIPNotFoundException();
#endif
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
		
		public static IPAddress convertStrToIP(String strIP)
		{
			String[] strSplit = strIP.Split(new char[] {'.'});
				
			int IP0 = (int)(System.Convert.ToInt32 (strSplit[0]));
			int IP1 = (int)(System.Convert.ToInt32 (strSplit[1]));
			int IP2 = (int)(System.Convert.ToInt32 (strSplit[2]));
			int IP3 = (int)(System.Convert.ToInt32 (strSplit[3]));
			byte[] byteIP = {(byte)IP0, (byte)IP1, (byte)IP2, (byte)IP3};
			return new IPAddress(byteIP);
		}
		
		public static String convertToTabSeparatedStr(bool lastTab, params String[] strDataSet)
		{
			
			StringBuilder concatenatedString = new StringBuilder();
			foreach(String strData in strDataSet)
			{
				concatenatedString.Append(strData);
				if(!(strData == strDataSet[strDataSet.Length-1] && !lastTab))
					concatenatedString.Append('\r', 1);
			}
			return concatenatedString.ToString();
		}

		public static byte[] convertToTabSeparatedByteArray(bool lastTab, params String[] strDataSet)
		{
			return System.Text.Encoding.ASCII.GetBytes(convertToTabSeparatedStr(lastTab, strDataSet));
		}
	}
}
