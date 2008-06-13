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
using System.Collections;
using System.Collections.Generic;

namespace Tashjik.Tier2.BATON
{
	class Node : INode
	{
		Engine engine;
		
		class Engine
		{
	
			struct RoutingTableEntry
			{
				public INode node;
				public INode leftChild;
				public INode rightChild;
				public int lowerBound;
				public int upperBound;
			}
		
			private readonly Tashjik.Common.NodeBasic selfNodeBasic;
			
			private int level;
			private int number;
			private INode parent = null;
			private INode leftChild = null;
			private INode rightChild = null;
			private INode leftAdjacent = null;
			private INode rightAdjacent = null;
				
			private readonly Node self;
			
			private List<RoutingTableEntry> leftRoutingTable = new List<RoutingTableEntry>();
			private List<RoutingTableEntry> rightRoutingTable = new List<RoutingTableEntry>();
			private volatile bool fullLeftRoutingTable = false;
			private volatile bool fullRightRoutingTable = false;
			
			public void join(INode newNode)
			{
				if(fullLeftRoutingTable && fullRightRoutingTable && (leftChild==null || rightChild==null))
				{
					if(leftChild==null)
						leftChild = newNode;
					else if(rightChild==null)
						rightChild = newNode;
				}
				else
				{
					if((!fullLeftRoutingTable) ||(!fullRightRoutingTable))
						parent.join(newNode);
					else
					{
						foreach(RoutingTableEntry routingTableEntry in leftRoutingTable)
						{
							if(routingTableEntry.leftChild==null)
							{
								routingTableEntry.leftChild.join(newNode);
								return;
							}
							else if(routingTableEntry.rightChild==null)
							{
								routingTableEntry.rightChild.join(newNode);
								return;
							}
						}
						foreach(RoutingTableEntry routingTableEntry in rightRoutingTable)
						{
							if(routingTableEntry.leftChild==null)
							{
								routingTableEntry.leftChild.join(newNode);
								return;
							}
							else if(routingTableEntry.rightChild==null)
							{
								routingTableEntry.rightChild.join(newNode);
								return;
							}
						}
						rightAdjacent.join(newNode);
					}
				}
			}
		
			public void findReplacement(INode repNode)
			{
				if(leftChild!=null)
					leftChild.findReplacement(repNode);
				else if(rightChild!=null)
					rightChild.findReplacement(repNode);
				else
				{
					foreach(RoutingTableEntry routingTableEntry in leftRoutingTable)
						{
							if(routingTableEntry.leftChild!=null)
							{
								routingTableEntry.leftChild.findReplacement(repNode);
								return;
							}
							else if(routingTableEntry.rightChild!=null)
							{
								routingTableEntry.rightChild.findReplacement(repNode);
								return;
							}
						}
						foreach(RoutingTableEntry routingTableEntry in rightRoutingTable)
						{
							if(routingTableEntry.leftChild!=null)
							{
								routingTableEntry.leftChild.findReplacement(repNode);
								return;
							}
							else if(routingTableEntry.rightChild!=null)
							{
								routingTableEntry.rightChild.findReplacement(repNode);
								return;
							}
						}
						//REPLACE REPNODE WITH THIS NODE
						//this has to be thought through
				}
			}
				
			public Engine(Node n)
			{
				self = n;
				level = 0;
				number = 0;
				parent = null;
				leftChild = null;
				rightChild = null;
				leftAdjacent = null;
				rightAdjacent = null;
				
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
			}
		}
		
		public Node()
		{
			engine = new Engine(this);
		}
		
		public void join(INode newNode)
		{
			engine.join(newNode);
		}
		
		public void leave(INode leavingNode)
		{
			
		}
		public void findReplacement(INode repNode)
		{
			engine.findReplacement(repNode);
		}
		
		//Data searchExact(...)
			
	}
}
