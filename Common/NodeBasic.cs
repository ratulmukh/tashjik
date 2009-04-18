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

namespace Tashjik.Common
{
	public class NodeBasic
	{
		private readonly Object nodeBasicLock;
		private IPAddress IP;
		private byte[] hashedIP;

		//WARNING: there is a return inside locks
		public byte[] getHashedIP()
		{
			lock(nodeBasicLock)
			{
				byte[] copiedHashedIP = new byte[hashedIP.Length];
				Array.Copy(hashedIP, copiedHashedIP, hashedIP.Length);
				return copiedHashedIP;
			}
		}

		public IPAddress getIP()
		{
			lock(nodeBasicLock)
			{
				byte[] byteIP = System.Text.Encoding.ASCII.GetBytes(IP.ToString());
				IPAddress copiedIP = new IPAddress(byteIP);
				return copiedIP;
			}
	
		}

		public void setIP(IPAddress i)
		{	
			lock(nodeBasicLock)
			{

				IP = i;
				if(IP==null)
					hashedIP = null;
				else
				{
					//IP and hashedIP are invariants
					byte[] byteIP = System.Text.Encoding.ASCII.GetBytes(IP.ToString());
					hashedIP = Common.UtilityMethod.sha.ComputeHash(byteIP);
				}
			}

		}


		public NodeBasic()
		{
			nodeBasicLock = new Object();
			IP = null;
			hashedIP = null;
		}

		public NodeBasic(IPAddress IP1)
		{
			nodeBasicLock = new Object();
			setIP(IP1);
		}


		public static bool operator==(NodeBasic n1, NodeBasic n2)
		{
			if ((n1.getIP().ToString() == n2.getIP().ToString())) //hashedIPs should be equal if IPs are equal; so need to check
				return true;
			else
				return false;
		}

		public static bool operator!=(NodeBasic n1, NodeBasic n2)
		{
			if ((n1.getIP().ToString() != n2.getIP().ToString())) //hashedIPs should be equal if IPs are equal; so need to check
				return true;
			else
				return false;
		}


		public override bool Equals(object obj)
		{
			Type thisType = this.GetType();
			String thisAssemblyQualifiedName = thisType.AssemblyQualifiedName;
			String thisFullName = thisType.FullName;

			Type objType = obj.GetType();
			String objAssemblyQualifiedName = objType.AssemblyQualifiedName;
			String objFullName = objType.FullName;

			if (thisAssemblyQualifiedName==objAssemblyQualifiedName
				&& thisFullName==objFullName) //the second condition may be unnecessary
			{
				lock(nodeBasicLock)
				{
					if (IP.ToString() == ((NodeBasic)obj).getIP().ToString())
						return true;
					else
						return false;
				}
			}
			else
				return false;

		}		

		public static bool Equals(NodeBasic n1, NodeBasic n2)
		{
			if ((n1.getIP().ToString() == n2.getIP().ToString())) //hashedIPs should be equal if IPs are equal; so need to check
				return true;
			else
				return false;
		}

		public override int GetHashCode ()
		{
			//have to give a proper implementation for this
			return 1; // :O :O
		}
	}
}


