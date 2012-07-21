/************************************************************
* File Name: Node.cs
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


//using System;
//using System.Net;
//
//namespace Tashjik.Tier2.ChordCommon
//{
//	/*************************************************
//	* THREAD SAFE
//	*
//	* SYNCHRONIZATION POLICY
//	* Private lock used to access state
//	*
//* INVARIANTS
//	* hashedIP should always be the hash of IP,
//	* computed by calling
//	* Common.UtilityMethod.sha.ComputeHash(IP)
//	*
//	*************************************************/
//	public class Node
//	{
//		private readonly Object nodeLock;
//		private IPAddress IP;
//		private byte[] hashedIP;
//
//		public byte[] getHashedIP()
//		{
//			lock(nodeLock)
//			{
//				byte[] copiedHashedIP = new byte[hashedIP.Length];
//				Array.Copy(hashedIP, copiedHashedIP, hashedIP.Length);
//				return copiedHashedIP;
//			}
//		}
//
//		public IPAddress getIP()
//		{
//			lock(nodeLock)
//			{
//				byte[] byteIP = System.Text.Encoding.ASCII.GetBytes(IP.ToString());
//				IPAddress copiedIP = new IPAddress(byteIP);
//				return copiedIP;
//			}
//
//		}
//
//		public void setIP(IPAddress i)
//		{
//			lock(nodeLock)
//			{
//	
//				IP = i;
//				if(IP==null)
//					hashedIP = null;
//				else
//				{
//					//IP and hashedIP are invariants
//					byte[] byteIP = System.Text.Encoding.ASCII.GetBytes(IP.ToString());
//					hashedIP = Tashjik.Common.UtilityMethod.sha.ComputeHash(byteIP);
//				}
//			}
//	
//		}
//
//
//		public Node()
//		{
//			nodeLock = new Object();
//			IP = null;
//			hashedIP = null;
//		}
//
//		public Node(IPAddress IP1)
//		{
//			nodeLock = new Object();
//			setIP(IP1);
//		}
//
//
//		public static bool operator==(Node n1, Node n2)
//		{
//			if ((n1.getIP().ToString() == n2.getIP().ToString())) //hashedIPs should be equal if IPs are equal; so need to check
//				return true;
//			else
//				return false;
//		}
//
//
//		public static bool operator!=(Node n1, Node n2)
//		{
//			if ((n1.getIP().ToString() != n2.getIP().ToString())) //hashedIPs should be equal if IPs are equal; so need to check
//				return true;
//			else
//				return false;
//		}
//
//
//		public override bool Equals(object obj)
//		{
//			Type thisType = this.GetType();
//			String thisAssemblyQualifiedName = thisType.AssemblyQualifiedName;
//			String thisFullName = thisType.FullName;
//
//			Type objType = obj.GetType();
//			String objAssemblyQualifiedName = objType.AssemblyQualifiedName;
//			String objFullName = objType.FullName;
//
//			if (thisAssemblyQualifiedName==objAssemblyQualifiedName
//				&& thisFullName==objFullName) //the second condition may be unnecessary
//			{
//				lock(nodeLock)
//				{
//					if (IP.ToString() == ((Node)obj).getIP().ToString())
//						return true;
//					else
//						return false;
//				}
//			}
//			else
//				return false;
//
//		}
//
//		public static bool Equals(Node n1, Node n2)
//		{
//			if ((n1.getIP().ToString() == n2.getIP().ToString())) //hashedIPs should be equal if IPs are equal; so need to check
//				return true;
//			else
//				return false;
//		}
//	
//		public override int GetHashCode ()
//		{
//			//have to give a proper implementation for this
//			return 1; // :O :O
//		}
//	}
//}
