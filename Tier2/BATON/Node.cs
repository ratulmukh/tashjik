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

namespace Tashjik.Tier2.BATON
{
	public class Node : INode
	{
		//RULES OF THE THUMB
		//notify calls are made to nodes to notify them of something withot being asked
		//request calls are made to nodes to request them for info
		//send calls are made to reply back to requests
		
		Engine engine;
		
		public enum Position
		{
			LEFT,
			RIGHT,
		}
		
		public struct RoutingTableEntry
		{
			public INode node;
			public INode leftChild;
			public INode rightChild;
			public int lowerBound;
			public int upperBound;
		}
		class Engine
		{
	
			
		
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
			
			public void joinAccepted(INode acceptingNode, Position pos, INode adjacent)
			{
				parent = acceptingNode;
				if(pos==Position.LEFT)
				{
					leftAdjacent = adjacent;
					rightAdjacent = acceptingNode;
					leftAdjacent.setAdjacent(self, Position.RIGHT, rightAdjacent);
					
				}
				if(pos==Position.RIGHT)
				{
					rightAdjacent = adjacent;
					leftAdjacent = acceptingNode;
					rightAdjacent.setAdjacent(self, Position.LEFT, leftAdjacent);
				}
				//fill up its left and right routing tables by asking parent
				parent.requestRoutingTableForChild(self, Position.LEFT);
				parent.requestRoutingTableForChild(self, Position.RIGHT);
			}
			
			public void requestRoutingTableForChild( INode requestingChild, Position pos)
			{
				//assimilate rouing table
				if(pos==Position.LEFT)
				{
					List<RoutingTableEntry> leftRoutingTableOfChild = new List<RoutingTableEntry>();
					if(requestingChild == rightChild)
					{
						RoutingTableEntry firstNodeEntry = new RoutingTableEntry();
						firstNodeEntry.node = leftChild;
						leftRoutingTableOfChild.Add(firstNodeEntry);
						
						foreach(RoutingTableEntry leftRoutingTableEntry in leftRoutingTable)
						{
							RoutingTableEntry entry = new RoutingTableEntry();
							entry.node = leftRoutingTableEntry.rightChild;
							leftRoutingTableOfChild.Add(firstNodeEntry);
						}
					}
					else if(requestingChild == leftChild)
					{
						RoutingTableEntry firstNodeEntry = new RoutingTableEntry();
						firstNodeEntry.node = leftRoutingTable[0].rightChild;
						leftRoutingTableOfChild.Add(firstNodeEntry);
						
						for(int i=1;i<leftRoutingTable.Count;i++)
						{
							RoutingTableEntry leftRoutingTableEntry = leftRoutingTable[i];
							RoutingTableEntry entry = new RoutingTableEntry();
							entry.node = leftRoutingTableEntry.leftChild;
						    leftRoutingTableOfChild.Add(firstNodeEntry);
						}
					}
					requestingChild.sendNodeOnlyRoutingTableForChild(leftRoutingTableOfChild,Position.LEFT);
				}
				else if(pos==Position.RIGHT)
				{
					List<RoutingTableEntry> rightRoutingTableOfChild = new List<RoutingTableEntry>();
					if(requestingChild == leftChild)
					{
						RoutingTableEntry firstNodeEntry = new RoutingTableEntry();
						firstNodeEntry.node = rightChild;
						rightRoutingTableOfChild.Add(firstNodeEntry);
						
						foreach(RoutingTableEntry rightRoutingTableEntry in rightRoutingTable)
						{
							RoutingTableEntry entry = new RoutingTableEntry();
							entry.node = rightRoutingTableEntry.leftChild;
							rightRoutingTableOfChild.Add(firstNodeEntry);
						}
					}
					else if(requestingChild == leftChild)
					{
						RoutingTableEntry firstNodeEntry = new RoutingTableEntry();
						firstNodeEntry.node = leftRoutingTable[0].leftChild;
						rightRoutingTableOfChild.Add(firstNodeEntry);
						
						for(int i=1;i<rightRoutingTable.Count;i++)
						{
							RoutingTableEntry rightRoutingTableEntry = rightRoutingTable[i];
							RoutingTableEntry entry = new RoutingTableEntry();
							entry.node = rightRoutingTableEntry.rightChild;
							rightRoutingTableOfChild.Add(firstNodeEntry);
						}
					}
					requestingChild.sendNodeOnlyRoutingTableForChild(rightRoutingTableOfChild,Position.RIGHT);
				}
				
				
			}
			
			public void sendNodeOnlyRoutingTableForChild(List<RoutingTableEntry> routingTable, Position pos)
			{
				//for each node in routing table, query tht node for its children
				foreach(RoutingTableEntry routingTableEntry in routingTable)
					routingTableEntry.node.requestChildren(self);
			}
			
			public void requestChildren(INode requestingNode)
			{
				requestingNode.notifyChildren(self, leftChild, rightChild);
			}
			
			public void notifyChildren(INode notifyingNode, INode leftChild, INode rightChild)
			{
				for(int i=0;i<leftRoutingTable.Count;i++)
					if(notifyingNode == leftRoutingTable[i].node)
					{
						RoutingTableEntry leftRoutingTableEntry = leftRoutingTable[i];
						leftRoutingTableEntry.leftChild = leftChild;
						leftRoutingTableEntry.rightChild = rightChild;
						return;
					}
				for(int i=0;i<rightRoutingTable.Count;i++)
					if(notifyingNode == rightRoutingTable[i].node)
					{
						RoutingTableEntry rightRoutingTableEntry = rightRoutingTable[i];
						rightRoutingTableEntry.leftChild = leftChild;
						rightRoutingTableEntry.rightChild = rightChild;
						return;
					}
			}
			
			public void setAdjacent(INode newAdjacent, Position pos, INode prevNode)
			{
				if(pos==Position.LEFT)
				{
					if(leftAdjacent==prevNode)
						leftAdjacent = newAdjacent;
					else
						throw new BATON.Exception.PrevNodeNotMatching();
				}
				else if (pos==Position.RIGHT)
				{
					if(rightAdjacent==prevNode)
						rightAdjacent = newAdjacent;
					else
						throw new BATON.Exception.PrevNodeNotMatching();
				}
			}
			
			public void notifyNewChild(INode notifyingNode, Node.Position pos, INode newChild)
			{
				for(int i=0; i<leftRoutingTable.Count;i++)
				{
					RoutingTableEntry leftRoutingTableEntry = leftRoutingTable[i];
					if(leftRoutingTableEntry.node == notifyingNode)
					{
						if(pos == Position.LEFT)
							leftRoutingTableEntry.leftChild = newChild;
						else if(pos == Position.RIGHT)
							leftRoutingTableEntry.rightChild = newChild;
						leftChild.setNewPeer(i, Position.LEFT, newChild);
						rightChild.setNewPeer(i, Position.LEFT, newChild);
						return;
					}
				}
				for(int i=0; i<rightRoutingTable.Count;i++)
				{
					RoutingTableEntry rightRoutingTableEntry = rightRoutingTable[i];
					if(rightRoutingTableEntry.node == notifyingNode)
					{
						if(pos == Position.LEFT)
							rightRoutingTableEntry.leftChild = newChild;
						else if(pos == Position.RIGHT)
							rightRoutingTableEntry.rightChild = newChild;
						leftChild.setNewPeer(i, Position.RIGHT, newChild);
						rightChild.setNewPeer(i, Position.RIGHT, newChild);
						return;
					}
				}
				
			}
			
			public void setNewPeer(int routingTablepointer, Position pos, INode newChild)
			{
				//NEED TO IMPLEMENT!!!
				if(routingTablepointer != 0)
				{
					//if(pos == Position.LEFT)
						//leftRoutingTable[routingTablepointer] = 
				}
			}
			
			public void join(INode newNode)
			{
				if(fullLeftRoutingTable && fullRightRoutingTable && (leftChild==null || rightChild==null))
				{
					if(leftChild==null)
					{
						leftChild = newNode;
						newNode.joinAccepted(self, Position.LEFT, leftAdjacent);
						//split half of contents
						leftAdjacent = newNode;
						foreach(RoutingTableEntry routingTableEntry in leftRoutingTable)
							if(routingTableEntry.node != null)
								routingTableEntry.node.notifyNewChild(self, Position.LEFT, newNode);
						foreach(RoutingTableEntry routingTableEntry in rightRoutingTable)
							if(routingTableEntry.node != null)
								routingTableEntry.node.notifyNewChild(self, Position.LEFT, newNode);							
						
					}
					else if(rightChild==null)
					{
						rightChild = newNode;
						newNode.joinAccepted(self, Position.RIGHT, rightAdjacent);
						//split half of contents
						rightAdjacent = newNode;
						foreach(RoutingTableEntry routingTableEntry in leftRoutingTable)
							if(routingTableEntry.node != null)
								routingTableEntry.node.notifyNewChild(self, Position.RIGHT, newNode);
						foreach(RoutingTableEntry routingTableEntry in rightRoutingTable)
							if(routingTableEntry.node != null)
								routingTableEntry.node.notifyNewChild(self, Position.RIGHT, newNode);							
					}
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
			}
			
			public Engine(Node n, INode joinOtherNode)
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
				
				initialize();
				joinOther(joinOtherNode);
			}
			
			private void initialize()
			{
				level = 0;
				number = 0;
				parent = null;
				leftChild = null;
				rightChild = null;
				leftAdjacent = null;
				rightAdjacent = null;
				
				
			}
			
			private void joinOther(INode n)
			{
				n.join(self);
			}
		}
		
		public Node()
		{
			engine = new Engine(this);
		}
		
		public Node(INode joinOtherNode)
		{
			engine = new Engine(this, joinOtherNode);
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
		
		public void joinAccepted(INode acceptingNode, Position pos, INode adjacent)
		{
			engine.joinAccepted(acceptingNode, pos, adjacent);
		}
		public void setAdjacent(INode newAdjacent, Position pos, INode prevNode)
		{
			engine.setAdjacent(newAdjacent, pos, prevNode);
		}
		public void notifyNewChild(INode notifyingNode, Position pos, INode newChild)
		{
			engine.notifyNewChild(notifyingNode, pos, newChild);
		}

		public void requestRoutingTableForChild( INode requestingChild, Position pos)
		{
			engine.requestRoutingTableForChild(requestingChild, pos);
		}
		public void sendNodeOnlyRoutingTableForChild(List<RoutingTableEntry> routingTable, Position pos)
		{
			engine.sendNodeOnlyRoutingTableForChild(routingTable, pos);
		}
		public void requestChildren(INode requestingNode)
		{
			engine.requestChildren(requestingNode);
		}
		public void notifyChildren(INode notifyingNode, INode leftChild, INode rightChild)
		{
			engine.notifyChildren(notifyingNode, leftChild, rightChild);
		}
		
		public void setNewPeer(int routingTablepointer, Position pos, INode newChild)
		{
			engine.setNewPeer(routingTablepointer, pos, newChild);
		}
		
		//Data searchExact(...)
			
	}
}
