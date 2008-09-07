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
	public class NodeProxy :INode
	{
		
		public NodeProxy(IPAddress IP)
		{
			//initialize new NodeProxy
			//add itself to ProxyController registry
		}
		
		public void join(INode newNode)
		{
			
		}
		
		public void join(INode newNode, Guid overlayInstanceGuid)
		{
			
		}
		
		public void requestReplacement(INode repNode)
		{
			
		}
		
		public void replyReplacement(INode newNode)
		{
			
		}
		
			
		public void requestPersonalData(INode reqNode)
		{
			
		}
			
		public void  sendPersonalData(INode sendingNode, int newLevel, int newNumber, INode newParent, INode newLeftChild, INode newRightChild, INode newLeftAdjacent, INode newRightAdjacent, List<Node.RoutingTableEntry> newleftRoutingTable,  List<Node.RoutingTableEntry> newRightRoutingTable, bool newFullLeftRoutingTable, bool newFullRightRoutingTable)
		{
				
		}

		
		public void leave()
		{
			
		}
		
		public void notifyLeave()
		{
			
		}
		
		public void notiifyLeftAdjacentAboutReplacement(INode newNode, INode oldNode)
		{
			
		}
			
		public void notiifyRightAdjacentAboutReplacement(INode newNode, INode oldNode)
		{
			
		}
			
		//Data searchExact(...)
		
		
		
		
		
		public void joinAccepted(INode acceptingNode, Node.Position pos, INode adjacent, int newLevel, int newNumber)
		{
		
		}
		public void setAdjacent(INode newAdjacent, Node.Position pos, INode prevNode)
		{
			
		}
		public void notifyNewChild(INode notifyingNode, Node.Position pos, INode newChild)
		{
			
		}
		
		public void requestRoutingTableForChild( INode requestingChild, Node.Position pos)
		{
		
		}
		public void sendNodeOnlyRoutingTableToChild(List<Node.RoutingTableEntry> routingTable, Node.Position pos)
		{
			
		}
		public void requestChildren(INode requestingNode)
		{
			
		}
		public void notifyChildren(INode notifyingNode, INode leftChild, INode rightChild)
		{
			
		}
		
		public void setNewPeer(int routingTablepointer, Node.Position pos, INode newChild)
		{
			
		}
		
		public void notifyParentAboutReplacement(INode newChild, INode oldChild)
		{
			
		}
	}
}
