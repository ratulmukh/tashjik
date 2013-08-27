/************************************************************
* File Name: BATONNode.cs
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
//using System.Net.Sockets;
//using System.Collections;
//using System.Collections.Generic;
//
//namespace Tashjik.Tier2
//{
//	internal class BATONNode : IBATONNode
//	{
//		//RULES OF THE THUMB
//		//notify calls are made to BATONNodes to notify them of something withot being asked
//		//request calls are made to BATONNodes to request them for info
//		//send/reply calls are made to reply back to requests
//		
//		Engine engine;
//		
//		public enum Position
//		{
//			LEFT,
//			RIGHT,
//		}
//		
//		public struct RoutingTableEntry
//		{
//			public IBATONNode BATONNode;
//			public IBATONNode leftChild;
//			public IBATONNode rightChild;
//			public int lowerBound;
//			public int upperBound;
//		}
//		class Engine
//		{
//	
//			
//		
//			private readonly Tashjik.Common.NodeBasic selfBATONNodeBasic;
//			
//			private int level;
//			private int number;
//			private IBATONNode parent = null;
//			private IBATONNode leftChild = null;
//			private IBATONNode rightChild = null;
//			private IBATONNode leftAdjacent = null;
//			private IBATONNode rightAdjacent = null;
//				
//			private readonly BATONNode self;
//			
//			private List<RoutingTableEntry> leftRoutingTable = new List<RoutingTableEntry>();
//			private List<RoutingTableEntry> rightRoutingTable = new List<RoutingTableEntry>();
//			private volatile bool fullLeftRoutingTable = false;
//			private volatile bool fullRightRoutingTable = false;
//			
//			public void joinAccepted(IBATONNode acceptingBATONNode, Position pos, IBATONNode adjacent, int newLevel, int newNumber)
//			{
//				parent = acceptingBATONNode;
//				level = newLevel;
//				number = newNumber;
//				
//				if(pos==Position.LEFT)
//				{
//					leftAdjacent = adjacent;
//					rightAdjacent = acceptingBATONNode;
//					leftAdjacent.setAdjacent(self, Position.RIGHT, rightAdjacent);
//					
//				}
//				if(pos==Position.RIGHT)
//				{
//					rightAdjacent = adjacent;
//					leftAdjacent = acceptingBATONNode;
//					rightAdjacent.setAdjacent(self, Position.LEFT, leftAdjacent);
//				}
//				//fill up its left and right routing tables by asking parent
//				parent.requestRoutingTableForChild(self, Position.LEFT);
//				parent.requestRoutingTableForChild(self, Position.RIGHT);
//			}
//			
//			public void requestRoutingTableForChild( IBATONNode requestingChild, Position pos)
//			{
//				//assimilate rouing table
//				if(pos==Position.LEFT)
//				{
//					List<RoutingTableEntry> leftRoutingTableOfChild = new List<RoutingTableEntry>();
//					if(requestingChild == rightChild)
//					{
//						RoutingTableEntry firstBATONNodeEntry = new RoutingTableEntry();
//						firstBATONNodeEntry.BATONNode = leftChild;
//						leftRoutingTableOfChild.Add(firstBATONNodeEntry);
//						
//						foreach(RoutingTableEntry leftRoutingTableEntry in leftRoutingTable)
//						{
//							RoutingTableEntry entry = new RoutingTableEntry();
//							entry.BATONNode = leftRoutingTableEntry.rightChild;
//							leftRoutingTableOfChild.Add(firstBATONNodeEntry);
//						}
//					}
//					else if(requestingChild == leftChild)
//					{
//						RoutingTableEntry firstBATONNodeEntry = new RoutingTableEntry();
//						firstBATONNodeEntry.BATONNode = leftRoutingTable[0].rightChild;
//						leftRoutingTableOfChild.Add(firstBATONNodeEntry);
//						
//						for(int i=1;i<leftRoutingTable.Count;i++)
//						{
//							RoutingTableEntry leftRoutingTableEntry = leftRoutingTable[i];
//							RoutingTableEntry entry = new RoutingTableEntry();
//							entry.BATONNode = leftRoutingTableEntry.leftChild;
//						    leftRoutingTableOfChild.Add(firstBATONNodeEntry);
//						}
//					}
//					requestingChild.sendBATONNodeOnlyRoutingTableToChild(leftRoutingTableOfChild,Position.LEFT);
//				}
//				else if(pos==Position.RIGHT)
//				{
//					List<RoutingTableEntry> rightRoutingTableOfChild = new List<RoutingTableEntry>();
//					if(requestingChild == leftChild)
//					{
//						RoutingTableEntry firstBATONNodeEntry = new RoutingTableEntry();
//						firstBATONNodeEntry.BATONNode = rightChild;
//						rightRoutingTableOfChild.Add(firstBATONNodeEntry);
//						
//						foreach(RoutingTableEntry rightRoutingTableEntry in rightRoutingTable)
//						{
//							RoutingTableEntry entry = new RoutingTableEntry();
//							entry.BATONNode = rightRoutingTableEntry.leftChild;
//							rightRoutingTableOfChild.Add(firstBATONNodeEntry);
//						}
//					}
//					else if(requestingChild == leftChild)
//					{
//						RoutingTableEntry firstBATONNodeEntry = new RoutingTableEntry();
//						firstBATONNodeEntry.BATONNode = leftRoutingTable[0].leftChild;
//						rightRoutingTableOfChild.Add(firstBATONNodeEntry);
//						
//						for(int i=1;i<rightRoutingTable.Count;i++)
//						{
//							RoutingTableEntry rightRoutingTableEntry = rightRoutingTable[i];
//							RoutingTableEntry entry = new RoutingTableEntry();
//							entry.BATONNode = rightRoutingTableEntry.rightChild;
//							rightRoutingTableOfChild.Add(firstBATONNodeEntry);
//						}
//					}
//					requestingChild.sendBATONNodeOnlyRoutingTableToChild(rightRoutingTableOfChild,Position.RIGHT);
//				}
//				
//				
//			}
//			
//			public void sendBATONNodeOnlyRoutingTableToChild(List<RoutingTableEntry> routingTable, Position pos)
//			{
//				//for each BATONNode in routing table, query tht BATONNode for its children
//				foreach(RoutingTableEntry routingTableEntry in routingTable)
//					routingTableEntry.BATONNode.requestChildren(self);
//			}
//			
//			public void requestChildren(IBATONNode requestingBATONNode)
//			{
//				requestingBATONNode.notifyChildren(self, leftChild, rightChild);
//			}
//			
//			public void notifyChildren(IBATONNode notifyingBATONNode, IBATONNode leftChild, IBATONNode rightChild)
//			{
//				for(int i=0;i<leftRoutingTable.Count;i++)
//					if(notifyingBATONNode == leftRoutingTable[i].BATONNode)
//					{
//						RoutingTableEntry leftRoutingTableEntry = leftRoutingTable[i];
//						leftRoutingTableEntry.leftChild = leftChild;
//						leftRoutingTableEntry.rightChild = rightChild;
//						return;
//					}
//				for(int i=0;i<rightRoutingTable.Count;i++)
//					if(notifyingBATONNode == rightRoutingTable[i].BATONNode)
//					{
//						RoutingTableEntry rightRoutingTableEntry = rightRoutingTable[i];
//						rightRoutingTableEntry.leftChild = leftChild;
//						rightRoutingTableEntry.rightChild = rightChild;
//						return;
//					}
//			}
//			
//			public void setAdjacent(IBATONNode newAdjacent, Position pos, IBATONNode prevBATONNode)
//			{
//				if(pos==Position.LEFT)
//				{
//					if(leftAdjacent==prevBATONNode)
//						leftAdjacent = newAdjacent;
//					else
//						throw new PrevBATONNodeNotMatching();
//				}
//				else if (pos==Position.RIGHT)
//				{
//					if(rightAdjacent==prevBATONNode)
//						rightAdjacent = newAdjacent;
//					else
//						throw new PrevBATONNodeNotMatching();
//				}
//			}
//			
//			public void notifyNewChild(IBATONNode notifyingBATONNode, BATONNode.Position pos, IBATONNode newChild)
//			{
//				for(int i=0; i<leftRoutingTable.Count;i++)
//				{
//					RoutingTableEntry leftRoutingTableEntry = leftRoutingTable[i];
//					if(leftRoutingTableEntry.BATONNode == notifyingBATONNode)
//					{
//						if(pos == Position.LEFT)
//							leftRoutingTableEntry.leftChild = newChild;
//						else if(pos == Position.RIGHT)
//							leftRoutingTableEntry.rightChild = newChild;
//						
//						//don't think the below 2 lines r required 
//						//since the newchild would hav got its routing table 
//						//from its parent
//						//leftChild.setNewPeer(i, Position.LEFT, newChild);
//						//rightChild.setNewPeer(i, Position.LEFT, newChild);
//						return;
//					}
//				}
//				for(int i=0; i<rightRoutingTable.Count;i++)
//				{
//					RoutingTableEntry rightRoutingTableEntry = rightRoutingTable[i];
//					if(rightRoutingTableEntry.BATONNode == notifyingBATONNode)
//					{
//						if(pos == Position.LEFT)
//							rightRoutingTableEntry.leftChild = newChild;
//						else if(pos == Position.RIGHT)
//							rightRoutingTableEntry.rightChild = newChild;
//						//don't think the below 2 lines r required 
//						//since the newchild would hav got its routing table 
//						//from its parent
//						//leftChild.setNewPeer(i, Position.RIGHT, newChild);
//						//rightChild.setNewPeer(i, Position.RIGHT, newChild);
//						return;
//					}
//				}
//				
//			}
//			
//			
//			public void setNewPeer(int routingTablepointer, Position pos, IBATONNode newChild)
//			{
//				//!!THIS FUNCTION MAY NOT BE REQUIRED
//				
//				
//				//NEED TO IMPLEMENT!!!
//				if(routingTablepointer != 0)
//				{
//					//if(pos == Position.LEFT)
//						//leftRoutingTable[routingTablepointer] = 
//				}
//			}
//			
//			public void leave()
//			{
//				bool lastLevelBATONNode = true;
//				if(leftChild==null && rightChild==null) //leaf BATONNode
//				{	
//					foreach(RoutingTableEntry routingTableEntry in leftRoutingTable)
//						if(routingTableEntry.leftChild != null || routingTableEntry.rightChild != null)
//						{
//							lastLevelBATONNode = false;
//							break;
//						}
//					if(lastLevelBATONNode==true)
//						foreach(RoutingTableEntry routingTableEntry in rightRoutingTable)
//							if(routingTableEntry.leftChild != null || routingTableEntry.rightChild != null)
//							{
//								lastLevelBATONNode = false;
//								break;
//							}
//					if(lastLevelBATONNode==true)
//					{
//						//can leave voluntarily! YIPEE!
//						//transfer content to parent
//						//index range to parent and 1 of adjacent BATONNodes
//						//PARENT ON RECEIVING CONTENT HAS TO DO FEW THINGS! DON'T FORGET THT!!
//						foreach(RoutingTableEntry routingTableEntry in leftRoutingTable)
//							routingTableEntry.BATONNode.notifyLeave();
//						foreach(RoutingTableEntry routingTableEntry in rightRoutingTable)
//							routingTableEntry.BATONNode.notifyLeave();
//					}
//					else
//					{
//						foreach(RoutingTableEntry routingTableEntry in leftRoutingTable)
//							if (routingTableEntry.leftChild != null )
//							{
//								routingTableEntry.leftChild.requestReplacement(self);
//								return;
//							}
//							else if ( routingTableEntry.rightChild != null )
//							{
//								routingTableEntry.rightChild.requestReplacement(self);
//								return;
//							}
//							foreach(RoutingTableEntry routingTableEntry in rightRoutingTable)
//							if (routingTableEntry.leftChild != null )
//							{
//								routingTableEntry.leftChild.requestReplacement(self);
//								return;
//							}
//							else if (routingTableEntry.rightChild != null )
//							{
//								routingTableEntry.rightChild.requestReplacement(self);
//								return;
//							}
//					}
//					
//				}
//				else
//					leftAdjacent.requestReplacement(self);
//					//or rightAdjacent.. need to make this random
//			}
//			
//						
//				
//			
//			
//			public void notifyLeave()
//			{
//				
//			}
//			
//			public void join(IBATONNode newBATONNode)
//			{
//				if(fullLeftRoutingTable && fullRightRoutingTable && (leftChild==null || rightChild==null))
//				{
//					if(leftChild==null)
//					{
//						leftChild = newBATONNode;
//						newBATONNode.joinAccepted(self, Position.LEFT, leftAdjacent, level+1, number*2-1);
//						//split half of contents
//						// WATS THIS FOR!? ->leftAdjacent = newBATONNode;
//						foreach(RoutingTableEntry routingTableEntry in leftRoutingTable)
//							if(routingTableEntry.BATONNode != null)
//								routingTableEntry.BATONNode.notifyNewChild(self, Position.LEFT, newBATONNode);
//						foreach(RoutingTableEntry routingTableEntry in rightRoutingTable)
//							if(routingTableEntry.BATONNode != null)
//								routingTableEntry.BATONNode.notifyNewChild(self, Position.LEFT, newBATONNode);							
//						
//					}
//					else if(rightChild==null)
//					{
//						rightChild = newBATONNode;
//						newBATONNode.joinAccepted(self, Position.RIGHT, rightAdjacent, level+1, number*2);
//						//split half of contents
//						// WATS THIS FOR!? -> rightAdjacent = newBATONNode;
//						foreach(RoutingTableEntry routingTableEntry in leftRoutingTable)
//							if(routingTableEntry.BATONNode != null)
//								routingTableEntry.BATONNode.notifyNewChild(self, Position.RIGHT, newBATONNode);
//						foreach(RoutingTableEntry routingTableEntry in rightRoutingTable)
//							if(routingTableEntry.BATONNode != null)
//								routingTableEntry.BATONNode.notifyNewChild(self, Position.RIGHT, newBATONNode);							
//					}
//				}
//				else
//				{
//					if((!fullLeftRoutingTable) ||(!fullRightRoutingTable))
//						parent.join(newBATONNode);
//					else
//					{
//						foreach(RoutingTableEntry routingTableEntry in leftRoutingTable)
//						{
//							if(routingTableEntry.leftChild==null ||routingTableEntry.rightChild==null)
//							{
//								routingTableEntry.BATONNode.join(newBATONNode);
//								return;
//							}
//						}
//						foreach(RoutingTableEntry routingTableEntry in rightRoutingTable)
//						{
//							if(routingTableEntry.leftChild==null ||routingTableEntry.rightChild==null)
//							{
//								routingTableEntry.BATONNode.join(newBATONNode);
//								return;
//							}
//						}
//						rightAdjacent.join(newBATONNode); 
//						// or to leftAdjacent BATONNode
//						//we shld select this randomly
//						//lets keep this as an enhancement for later
//					}
//				}
//			}
//		
//			public void requestReplacement(IBATONNode repBATONNode)
//			{
//				if(leftChild!=null)
//					leftChild.requestReplacement(repBATONNode);
//				else if(rightChild!=null)
//					rightChild.requestReplacement(repBATONNode);
//				else
//				{
//					foreach(RoutingTableEntry routingTableEntry in leftRoutingTable)
//					{
//						if(routingTableEntry.leftChild!=null)
//						{
//							routingTableEntry.leftChild.requestReplacement(repBATONNode);
//							return;
//						}
//						else if(routingTableEntry.rightChild!=null)
//						{
//							routingTableEntry.rightChild.requestReplacement(repBATONNode);
//							return;
//						}
//					}
//					foreach(RoutingTableEntry routingTableEntry in rightRoutingTable)
//					{
//						if(routingTableEntry.leftChild!=null)
//						{
//							routingTableEntry.leftChild.requestReplacement(repBATONNode);
//							return;
//						}
//						else if(routingTableEntry.rightChild!=null)
//						{
//							routingTableEntry.rightChild.requestReplacement(repBATONNode);
//							return;
//						}
//					}
//					//REPLACE REPBATONNode WITH THIS BATONNode
//					leave();
//					repBATONNode.requestPersonalData(self);
//					
//								
//					
//					
//						
//				}
//			}
//			
//			public void requestPersonalData(IBATONNode reqBATONNode)
//			{
//				reqBATONNode.sendPersonalData(self, level, number, parent, leftChild, rightChild, leftAdjacent, rightAdjacent, leftRoutingTable, rightRoutingTable, fullLeftRoutingTable, fullRightRoutingTable);
//			}
//			
//			public void  sendPersonalData(IBATONNode sendingBATONNode, int newLevel, int newNumber, IBATONNode newParent, IBATONNode newLeftChild, IBATONNode newRightChild, IBATONNode newLeftAdjacent, IBATONNode newRightAdjacent, List<RoutingTableEntry> newleftRoutingTable,  List<RoutingTableEntry> newRightRoutingTable, bool newFullLeftRoutingTable, bool newFullRightRoutingTable)
//			{
//				//need to send data and range too
//				level = newLevel;
//				number = newNumber;
//				parent = newParent;
//				leftChild = newLeftChild;
//				rightChild = newRightChild;
//				leftAdjacent = newLeftAdjacent;
//				rightAdjacent = newRightAdjacent;
//				leftRoutingTable = newleftRoutingTable;
//				rightRoutingTable = newRightRoutingTable;
//				fullLeftRoutingTable = newFullLeftRoutingTable;
//				fullRightRoutingTable = newFullRightRoutingTable;
//				
//				//notify all these BATONNodes abt the replacement
//				parent.notifyParentAboutReplacement(self, sendingBATONNode);
//				leftAdjacent.notiifyLeftAdjacentAboutReplacement(self, sendingBATONNode);
//				rightAdjacent.notiifyRightAdjacentAboutReplacement(self, sendingBATONNode);
//				
//				//finally respond back saying u can die
//					sendingBATONNode.replyReplacement(self);
//				
//			}
//				
//			public void notiifyLeftAdjacentAboutReplacement(IBATONNode newBATONNode, IBATONNode oldBATONNode)
//			{
//				if(rightAdjacent==oldBATONNode)
//					rightAdjacent = newBATONNode;
//				else
//					;//throw exception
//			}
//			
//			public void notiifyRightAdjacentAboutReplacement(IBATONNode newBATONNode, IBATONNode oldBATONNode)
//			{
//				if(leftAdjacent==oldBATONNode)
//					leftAdjacent = newBATONNode;
//				else
//					;//throw exception
//			}
//			
//			public void notifyParentAboutReplacement(IBATONNode newChild, IBATONNode oldChild)
//			{
//				if(leftChild==oldChild)
//				{
//					leftChild = newChild;
//					foreach(RoutingTableEntry routingTableEntry in leftRoutingTable)
//							if(routingTableEntry.BATONNode != null)
//								routingTableEntry.BATONNode.notifyNewChild(self, Position.LEFT, leftChild);
//						foreach(RoutingTableEntry routingTableEntry in rightRoutingTable)
//							if(routingTableEntry.BATONNode != null)
//								routingTableEntry.BATONNode.notifyNewChild(self, Position.LEFT, leftChild);							
//				}
//				else if(rightChild==oldChild)
//				{
//					rightChild = newChild;
//					foreach(RoutingTableEntry routingTableEntry in leftRoutingTable)
//							if(routingTableEntry.BATONNode != null)
//								routingTableEntry.BATONNode.notifyNewChild(self, Position.RIGHT, rightChild);
//						foreach(RoutingTableEntry routingTableEntry in rightRoutingTable)
//							if(routingTableEntry.BATONNode != null)
//								routingTableEntry.BATONNode.notifyNewChild(self, Position.RIGHT, rightChild);							
//				}
//			}
//			
//			
//			public void replyReplacement(IBATONNode newBATONNode)
//			{
//				//permission to die granted
//			}
//			
//						
//			public Engine(BATONNode n)
//			{
//				self = n;
//				try
//				{
//					selfBATONNodeBasic = new Tashjik.Common.NodeBasic(Tashjik.Common.UtilityMethod.GetLocalHostIP());
//				}
//				catch (Tashjik.Common.Exception.LocalHostIPNotFoundException e)
//				{
//					//local ip could not be found :O :O
//					//crash the system
//					//dunno how to do it though :(
//				}
//				initialize();
//			}
//			
//			public Engine(BATONNode n, IBATONNode joinOtherBATONNode)
//			{
//				self = n;
//				try
//				{
//					selfBATONNodeBasic = new Tashjik.Common.NodeBasic(Tashjik.Common.UtilityMethod.GetLocalHostIP());
//				}
//				catch (Tashjik.Common.Exception.LocalHostIPNotFoundException e)
//				{
//					//local ip could not be found :O :O
//					//crash the system
//					//dunno how to do it though :(
//				}
//				
//				initialize();
//				joinOther(joinOtherBATONNode);
//			}
//			
//			private void initialize()
//			{
//				level = 0;
//				number = 0;
//				parent = null;
//				leftChild = null;
//				rightChild = null;
//				leftAdjacent = null;
//				rightAdjacent = null;
//				
//				
//			}
//			
//			private void joinOther(IBATONNode n)
//			{
//				n.join(self);
//			}
//		}
//		
//		public BATONNode()
//		{
//			engine = new Engine(this);
//		}
//		
//		public BATONNode(IBATONNode joinOtherBATONNode)
//		{
//			engine = new Engine(this, joinOtherBATONNode);
//		}
//		
//		public void join(IBATONNode newBATONNode)
//		{
//			engine.join(newBATONNode);
//		}
//		
//		public void leave()
//		{
//			engine.leave();
//		}
//		
//		public void requestReplacement(IBATONNode repBATONNode)
//		{
//			engine.requestReplacement(repBATONNode);
//		}
//		
//		public void replyReplacement(IBATONNode newBATONNode)
//		{
//			engine.replyReplacement(newBATONNode);
//		}
//		
//		public void joinAccepted(IBATONNode acceptingBATONNode, Position pos, IBATONNode adjacent, int newLevel, int newNumber)
//		{
//			engine.joinAccepted(acceptingBATONNode, pos, adjacent, newLevel, newNumber);
//		}
//		public void setAdjacent(IBATONNode newAdjacent, Position pos, IBATONNode prevBATONNode)
//		{
//			engine.setAdjacent(newAdjacent, pos, prevBATONNode);
//		}
//		public void notifyNewChild(IBATONNode notifyingBATONNode, Position pos, IBATONNode newChild)
//		{
//			engine.notifyNewChild(notifyingBATONNode, pos, newChild);
//		}
//
//		public void notifyLeave()
//		{
//			engine.notifyLeave();
//		}
//			
//		public void notifyParentAboutReplacement(IBATONNode newChild, IBATONNode oldChild)
//		{
//			engine.notifyParentAboutReplacement(newChild, oldChild);
//		}
//		
//		public void notiifyLeftAdjacentAboutReplacement(IBATONNode newBATONNode, IBATONNode oldBATONNode)
//		{
//			engine.notiifyLeftAdjacentAboutReplacement(newBATONNode, oldBATONNode);
//		}
//			
//		public void notiifyRightAdjacentAboutReplacement(IBATONNode newBATONNode, IBATONNode oldBATONNode)
//		{
//			engine.notiifyRightAdjacentAboutReplacement(newBATONNode, oldBATONNode);
//		}
//		
//		public void requestRoutingTableForChild( IBATONNode requestingChild, Position pos)
//		{
//			engine.requestRoutingTableForChild(requestingChild, pos);
//		}
//		public void sendBATONNodeOnlyRoutingTableToChild(List<RoutingTableEntry> routingTable, Position pos)
//		{
//			engine.sendBATONNodeOnlyRoutingTableToChild(routingTable, pos);
//		}
//		
//		public void requestPersonalData(IBATONNode reqBATONNode)
//		{
//			engine.requestPersonalData(reqBATONNode);
//		}
//			
//		public void  sendPersonalData(IBATONNode sendingBATONNode, int newLevel, int newNumber, IBATONNode newParent, IBATONNode newLeftChild, IBATONNode newRightChild, IBATONNode newLeftAdjacent, IBATONNode newRightAdjacent, List<RoutingTableEntry> newleftRoutingTable,  List<RoutingTableEntry> newRightRoutingTable, bool newFullLeftRoutingTable, bool newFullRightRoutingTable)
//		{
//			engine.sendPersonalData(sendingBATONNode, newLevel, newNumber, newParent, newLeftChild, newRightChild, newLeftAdjacent, newRightAdjacent, newleftRoutingTable, newRightRoutingTable, newFullLeftRoutingTable, newFullRightRoutingTable);
//		}
//
//		
//		
//		public void requestChildren(IBATONNode requestingBATONNode)
//		{
//			engine.requestChildren(requestingBATONNode);
//		}
//		public void notifyChildren(IBATONNode notifyingBATONNode, IBATONNode leftChild, IBATONNode rightChild)
//		{
//			engine.notifyChildren(notifyingBATONNode, leftChild, rightChild);
//		}
//		
//		
//			
//		
//		
//		public void setNewPeer(int routingTablepointer, Position pos, IBATONNode newChild)
//		{
//			engine.setNewPeer(routingTablepointer, pos, newChild);
//		}
//		
//		//Data searchExact(...)
//			
//	}
//}
