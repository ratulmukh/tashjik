/************************************************************
* File Name: PastryRealNode.cs
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
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;


namespace Tashjik.Tier2
{
	
	internal class PastryRealNode : Tashjik.RealNode, IPastryNode
	{
	
		
			public static bool operator<(PastryRealNode n1, IPastryNode n2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = n2.getHashedIP().ToString();
		
				if(String.Compare(strHash1, strHash2)<0)
					return true;
				else 
					return false;
			}	

			public static bool operator>(PastryRealNode n1, IPastryNode n2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = n2.getHashedIP().ToString();
	
				if(String.Compare(strHash1, strHash2)>0)
					return true;
				else
					return false;
			}


			public static bool operator>(PastryRealNode n1, byte[] hash2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = hash2.ToString();
	
				if(String.Compare(strHash1, strHash2)>0)
				return true;
				else return false;
			}

			public static bool operator<(PastryRealNode n1, byte[] hash2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = hash2.ToString();

				if(String.Compare(strHash1, strHash2)<0)
					return true;
				else 
					return false;
			}

			public static bool operator<=(PastryRealNode n1, byte[] hash2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = hash2.ToString();

				if(String.Compare(strHash1, strHash2)<=0)
					return true;
				else 
					return false;
			}
			

			public static bool operator>=(PastryRealNode n1, byte[] hash2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = hash2.ToString();
	
				if(String.Compare(strHash1, strHash2)>=0)
				return true;
				else return false;
			}
			
			
			public static bool operator<(byte[] hash1, PastryRealNode n2)
			{
				String strHash1 = hash1.ToString();
				String strHash2 = n2.getHashedIP().ToString();
		
				if(String.Compare(strHash1, strHash2)<0)
					return true;
				else
					return false;
			}

			public static bool operator>(byte[] hash1, PastryRealNode n2)
			{
				String strHash1 = hash1.ToString();
				String strHash2 = n2.getHashedIP().ToString();

				if(String.Compare(strHash1, strHash2)>0)
					return true;
				else
					return false;
			}
			
			private static int sharedPrefixLength(PastryRealNode n, byte[] hash)
			{
				byte[] PastryNodeHashedIP = n.getHashedIP();
				int maxLength = PastryNodeHashedIP.Length;
				int prefixCount = 0;
				if(hash.Length > maxLength)
					maxLength = hash.Length;
				for(int i=0; i<maxLength; i++)
				{
					if(PastryNodeHashedIP[i]!= hash[i])
						break;
					prefixCount++;
				}
				return prefixCount;
			}

			
			
			public static int operator-(PastryRealNode n1, byte[] hash2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = hash2.ToString();
	
				return String.Compare(strHash1, strHash2);
				
			}
			
		class Engine
		{
						
					
			
			public struct RoutingTableRow
			{
				public IPastryNode[] PastryRealNode;
			}
			
			
			//typedef PastryNodeEntry[3] RoutingTableRow;
			
			private const int b = 4; //special Pastry configuration parameter
			private const int L = 16; //2^b special Pastry configuration parameter; L/2 is size of each leafset
		    private const int M = 16; //2^b size of neighbourhood set
			//private readonly List<PastryNodeEntry[5]> routingTable = new List<RoutingTableRow>();
			private readonly List<RoutingTableRow> routingTable = new List<RoutingTableRow>();
			//private readonly PastryNodeEntry[, ]   routingTable;//     = new PastryNodeEntry[Math.Pow(2, b)-1, ];
			private readonly List<IPastryNode> neighbourhoodSet = new List<IPastryNode>(M);
			private readonly List<IPastryNode> smallerLeafSet = new List<IPastryNode>(L/2);
			private readonly List<IPastryNode> largerLeafSet = new List<IPastryNode>(L/2); 
			
			private readonly Tashjik.Common.NodeBasic selfPastryNodeBasic;
			private readonly PastryRealNode self;
			
			
			
			public void route(Object msg, byte[] key)
			{
				if((PastryRealNode)(smallerLeafSet[L/2-1])<=key)
				{	for(int i=0; i<L/2; i++)
						if((PastryRealNode)smallerLeafSet[i]<=key)
						smallerLeafSet[i].route(msg, key);
				}
				else if((PastryRealNode)(largerLeafSet[L/2-1])>=key)
				{
					for(int i=0; i<L/2; i++)
						if((PastryRealNode)largerLeafSet[i]>=key)
							largerLeafSet[i].route(msg, key);
				}
				else
				{
					int prefixLength = sharedPrefixLength(self, key);
					int row = prefixLength;
					int col = (int)(key[prefixLength]);
					
					if((routingTable[row].PastryRealNode[col]) !=null)
						routingTable[row].PastryRealNode[col].route(msg, key);
					else
					{
						for(int j=0; j<row; j++)
							for(int k=0; k<col; k++)
							{
								if((sharedPrefixLength((PastryRealNode)(routingTable[row].PastryRealNode[col]), key) >= prefixLength)
							 	&& ((PastryRealNode)(routingTable[row].PastryRealNode[col]) - key) < (self - key)) //this needs to absolute
									routingTable[row].PastryRealNode[col].route(msg, key);
							}
						for(int i=0; i<L/2; i++)
							if((sharedPrefixLength((PastryRealNode)(smallerLeafSet[i]), key) >= prefixLength)
							&& ((PastryRealNode)(smallerLeafSet[i]) - key) < (self - key))   //this needs to absolute
								smallerLeafSet[i].route(msg, key);
						for(int i=0; i<L/2; i++)
							if((sharedPrefixLength((PastryRealNode)(smallerLeafSet[i]), key) >= prefixLength)
							&& ((PastryRealNode)(largerLeafSet[i]) - key) < (self - key))   //this needs to absolute
								largerLeafSet[i].route(msg, key);
					}
				}
				
			}
			
			public Engine(PastryRealNode n)
			{
				self = n;
				try
				{
					selfPastryNodeBasic = new Tashjik.Common.NodeBasic(Tashjik.Common.UtilityMethod.GetLocalHostIP());
				}
				catch (Tashjik.Common.Exception.LocalHostIPNotFoundException e)
				{
					//local ip could not be found :O :O
					//crash the system
					//dunno how to do it though :(
				}
				
				//routingTable     = new PastryNodeEntry[(int) (Math.Pow(2, b)-1), 128/b];
				//smallerLeafSet   = new List<PastryNodeEntry>(L/2);
				//largerLeafSet    = new List<PastryNodeEntry>(L/2);	

				
				initialize();

			}
			
			public Engine(PastryRealNode n, IPastryNode joinOtherPastryNode)
			{
/*
				self = n;
				try
				{
					selfPastryNodeBasic = new Tashjik.Common.PastryNodeBasic(Tashjik.Common.UtilityMethod.GetLocalHostIP());
				}
				catch (Tashjik.Common.Exception.LocalHostIPNotFoundException e)
				{
					//local ip could not be found :O :O
					//crash the system
					//dunno how to do it though :(
				}
				
				initialize();
				joinOther(joinOtherPastryNode);
*/
			}
	
			private void initialize()
			{
				
			}
			public void join(IPastryNode newPastryNode)
			{
				
			}
		
			public void leave()
			{
				
			}
			
			public byte[] getSelfPastryNodeBasicHashedIP()
			{
				return selfPastryNodeBasic.getHashedIP();
			}
			
		}
		
		Engine engine;
		
		public void route(Object msg, byte[] key)
		{
			engine.route(msg, key);
		}
		
		public PastryRealNode()
		{
			engine = new Engine(this);
		}
		
		public byte[] getHashedIP()
		{
			return engine.getSelfPastryNodeBasicHashedIP();
		}
		
		public PastryRealNode(IPastryNode joinOtherPastryNode)
		{
			engine = new Engine(this, joinOtherPastryNode);
		}
		
		public void join(IPastryNode newPastryNode)
		{
			engine.join(newPastryNode);
		}
		
		public void leave()
		{
			engine.leave();
		}
		
		public static string test(int a)
		{
			return "1";
		}
	}

}
