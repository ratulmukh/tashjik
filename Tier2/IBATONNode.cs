/************************************************************
* File Name: IBATONNode.cs
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
using System.Collections;
using System.Collections.Generic;

namespace Tashjik.Tier2
{
	internal interface IBATONNode
	{
		void join(IBATONNode newNode);
		void leave();
		
		void notifyLeave();
		void requestReplacement(IBATONNode repNode);
	    void replyReplacement(IBATONNode newNode);
	    
		void requestPersonalData(IBATONNode reqNode);
		void  sendPersonalData(IBATONNode sendingNode, int newLevel, int newNumber, IBATONNode newParent, IBATONNode newLeftChild, IBATONNode newRightChild, IBATONNode newLeftAdjacent, IBATONNode newRightAdjacent, List<BATONNode.RoutingTableEntry> newleftRoutingTable,  List<BATONNode.RoutingTableEntry> newRightRoutingTable, bool newFullLeftRoutingTable, bool newFullRightRoutingTable);
		void notifyParentAboutReplacement(IBATONNode newChild, IBATONNode oldChild);
		void notiifyLeftAdjacentAboutReplacement(IBATONNode newNode, IBATONNode oldNode);
		void notiifyRightAdjacentAboutReplacement(IBATONNode newNode, IBATONNode oldNode);
			
		//Data searchExact(...)
		void joinAccepted(IBATONNode acceptingNode, BATONNode.Position pos, IBATONNode adjacent, int newLevel, int newNumber);
		void setAdjacent(IBATONNode newAdjacent, BATONNode.Position pos, IBATONNode prevNode);
		void notifyNewChild(IBATONNode notifyingNode, BATONNode.Position pos, IBATONNode newChild);
		void requestRoutingTableForChild( IBATONNode requestingChild, BATONNode.Position pos);
		void sendBATONNodeOnlyRoutingTableToChild(List<BATONNode.RoutingTableEntry> routingTable, BATONNode.Position pos);
		void requestChildren(IBATONNode requestingNode);
		void notifyChildren(IBATONNode notifyingNode, IBATONNode leftChild, IBATONNode rightChild);
		void setNewPeer(int routingTablepointer, BATONNode.Position pos, IBATONNode newChild);		
	}
}
