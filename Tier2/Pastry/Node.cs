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
using System.Collections;
using System.Collections.Generic;

namespace Tashjik.Tier2.Pastry
{
	public class Node : Tier2.Common.Node, INode
	{
	
		
			public static bool operator<(Node n1, INode n2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = n2.getHashedIP().ToString();
		
				if(String.Compare(strHash1, strHash2)<0)
					return true;
				else 
					return false;
			}	

			public static bool operator>(Node n1, INode n2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = n2.getHashedIP().ToString();
	
				if(String.Compare(strHash1, strHash2)>0)
					return true;
				else
					return false;
			}


			public static bool operator>(Node n1, byte[] hash2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = hash2.ToString();
	
				if(String.Compare(strHash1, strHash2)>0)
				return true;
				else return false;
			}

			public static bool operator<(Node n1, byte[] hash2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = hash2.ToString();

				if(String.Compare(strHash1, strHash2)<0)
					return true;
				else 
					return false;
			}

			public static bool operator<=(Node n1, byte[] hash2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = hash2.ToString();

				if(String.Compare(strHash1, strHash2)<=0)
					return true;
				else 
					return false;
			}
			

			public static bool operator>=(Node n1, byte[] hash2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = hash2.ToString();
	
				if(String.Compare(strHash1, strHash2)>=0)
				return true;
				else return false;
			}
			
			
			public static bool operator<(byte[] hash1, Node n2)
			{
				String strHash1 = hash1.ToString();
				String strHash2 = n2.getHashedIP().ToString();
		
				if(String.Compare(strHash1, strHash2)<0)
					return true;
				else
					return false;
			}

			public static bool operator>(byte[] hash1, Node n2)
			{
				String strHash1 = hash1.ToString();
				String strHash2 = n2.getHashedIP().ToString();

				if(String.Compare(strHash1, strHash2)>0)
					return true;
				else
					return false;
			}
			
			private static int sharedPrefixLength(Node n, byte[] hash)
			{
				byte[] nodeHashedIP = n.getHashedIP();
				int maxLength = nodeHashedIP.Length;
				int prefixCount = 0;
				if(hash.Length > maxLength)
					maxLength = hash.Length;
				for(int i=0; i<maxLength; i++)
				{
					if(nodeHashedIP[i]!= hash[i])
						break;
					prefixCount++;
				}
				return prefixCount;
			}

			
			
			public static int operator-(Node n1, byte[] hash2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = hash2.ToString();
	
				return String.Compare(strHash1, strHash2);
				
			}
			
		class Engine
		{
						
					
			
			public struct RoutingTableRow
			{
				public INode[] node;
			}
			
			
			//typedef NodeEntry[3] RoutingTableRow;
			
			private const int b = 4; //special Pastry configuration parameter
			private const int L = 16; //2^b special Pastry configuration parameter; L/2 is size of each leafset
		    private const int M = 16; //2^b size of neighbourhood set
			//private readonly List<NodeEntry[5]> routingTable = new List<RoutingTableRow>();
			private readonly List<RoutingTableRow> routingTable = new List<RoutingTableRow>();
			//private readonly NodeEntry[, ]   routingTable;//     = new NodeEntry[Math.Pow(2, b)-1, ];
			private readonly List<INode> neighbourhoodSet = new List<INode>(M);
			private readonly List<INode> smallerLeafSet = new List<INode>(L/2);
			private readonly List<INode> largerLeafSet = new List<INode>(L/2); 
			
			private readonly Tashjik.Common.NodeBasic selfNodeBasic;
			private readonly Node self;
			
			
			
			public void route(Object msg, byte[] key)
			{
				if((Node)(smallerLeafSet[L/2-1])<=key)
				{	for(int i=0; i<L/2; i++)
						if((Node)smallerLeafSet[i]<=key)
						smallerLeafSet[i].route(msg, key);
				}
				else if((Node)(largerLeafSet[L/2-1])>=key)
				{
					for(int i=0; i<L/2; i++)
						if((Node)largerLeafSet[i]>=key)
							largerLeafSet[i].route(msg, key);
				}
				else
				{
					int prefixLength = sharedPrefixLength(self, key);
					int row = prefixLength;
					int col = (int)(key[prefixLength]);
					
					if((routingTable[row].node[col]) !=null)
						routingTable[row].node[col].route(msg, key);
					else
					{
						for(int j=0; j<row; j++)
							for(int k=0; k<col; k++)
							{
								if((sharedPrefixLength((Node)(routingTable[row].node[col]), key) >= prefixLength)
							 	&& ((Node)(routingTable[row].node[col]) - key) < (self - key)) //this needs to absolute
									routingTable[row].node[col].route(msg, key);
							}
						for(int i=0; i<L/2; i++)
							if((sharedPrefixLength((Node)(smallerLeafSet[i]), key) >= prefixLength)
							&& ((Node)(smallerLeafSet[i]) - key) < (self - key))   //this needs to absolute
								smallerLeafSet[i].route(msg, key);
						for(int i=0; i<L/2; i++)
							if((sharedPrefixLength((Node)(smallerLeafSet[i]), key) >= prefixLength)
							&& ((Node)(largerLeafSet[i]) - key) < (self - key))   //this needs to absolute
								largerLeafSet[i].route(msg, key);
					}
				}
				
			}
			
			public Engine(Node n)
			{
				self = n;
				try
				{
					selfNodeBasic = new Tashjik.Common.NodeBasic(Tashjik.Common.UtilityMethod.GetLocalHostIP());
				}
				catch (Tashjik.Common.Exception.LocalHostIPNotFoundException e)
				{
					//local ip could not be found :O :O
					//crash the system
					//dunno how to do it though :(
				}
				
				//routingTable     = new NodeEntry[(int) (Math.Pow(2, b)-1), 128/b];
				//smallerLeafSet   = new List<NodeEntry>(L/2);
				//largerLeafSet    = new List<NodeEntry>(L/2);	

				
				initialize();

			}
			
			public Engine(Node n, INode joinOtherNode)
			{
/*
				self = n;
				try
				{
					selfNodeBasic = new Tashjik.Common.NodeBasic(Tashjik.Common.UtilityMethod.GetLocalHostIP());
				}
				catch (Tashjik.Common.Exception.LocalHostIPNotFoundException e)
				{
					//local ip could not be found :O :O
					//crash the system
					//dunno how to do it though :(
				}
				
				initialize();
				joinOther(joinOtherNode);
*/
			}
	
			private void initialize()
			{
				
			}
			public void join(INode newNode)
			{
				
			}
		
			public void leave()
			{
				
			}
			
			public byte[] getSelfNodeBasicHashedIP()
			{
				return selfNodeBasic.getHashedIP();
			}
			
		}
		
		Engine engine;
		
		public void route(Object msg, byte[] key)
		{
			engine.route(msg, key);
		}
		
		public Node()
		{
			engine = new Engine(this);
		}
		
		public byte[] getHashedIP()
		{
			return engine.getSelfNodeBasicHashedIP();
		}
		
		public Node(INode joinOtherNode)
		{
			engine = new Engine(this, joinOtherNode);
		}
		
		public void join(INode newNode)
		{
			engine.join(newNode);
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
