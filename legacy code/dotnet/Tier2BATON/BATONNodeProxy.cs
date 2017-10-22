/************************************************************
* File Name: BATONNodeProxy.cs
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
//	internal class BATONProxyNode : Tier2.Common.ProxyNode, IBATONNode, Tier2.Common.ProxyController.IProxy
//	{
//		internal static BATONNode thisBATONNode;
//	
//		private Tashjik.Common.NodeBasic selfNodeBasic;
//		private Base.LowLevelComm lowLevelComm;
//
//		private readonly int iPortNo = System.Convert.ToInt16 ("2334");
//		private readonly Socket sock = null;
//
//		//not necessary; BATONProxyNode will be added to the registry in ProxyController by ProxyController itself
//		//BUT TO SEND MSGS, U NEED THE INTERFACE
//		private Tier2.Common.ProxyController proxyController;
//
//		
//		public BATONProxyNode(IPAddress ip, Tier2.Common.ProxyController proxyController)
//		{
//			lowLevelComm = Base.LowLevelComm.getRefLowLevelComm();
//			selfNodeBasic = new Tashjik.Common.NodeBasic(ip);
//			setProxyController(proxyController);
//		}
//		
//		public override void setProxyController(Tier2.Common.ProxyController c)
//		{
//			//need to handle synchronised calls here
//			if(proxyController!=null)
//			proxyController = c;
//		}	
//		
//		public override byte[] getHashedIP()
//		{
//			return selfNodeBasic.getHashedIP();
//		}
//
//		public override IPAddress getIP()
//		{
//			return selfNodeBasic.getIP();
//		}
//
//		public override void setIP(IPAddress ip)
//		{
//			selfNodeBasic.setIP(ip);
//		}
//		
//		public override void beginNotifyMsgRec(IPAddress fromIP, Object data, AsyncCallback notifyMsgRecCallBack, Object appState)
//		{
//			
//		}
//		
//		public void join(IBATONNode newBATONNode)
//		{
//			
//		}
//		
//		public void join(IBATONNode newBATONNode, Guid overlayInstanceGuid)
//		{
//			
//		}
//		
//		public void requestReplacement(IBATONNode repBATONNode)
//		{
//			
//		}
//		
//		public void replyReplacement(IBATONNode newBATONNode)
//		{
//			
//		}
//		
//			
//		public void requestPersonalData(IBATONNode reqBATONNode)
//		{
//			
//		}
//			
//		public void  sendPersonalData(IBATONNode sendingBATONNode, int newLevel, int newNumber, IBATONNode newParent, IBATONNode newLeftChild, IBATONNode newRightChild, IBATONNode newLeftAdjacent, IBATONNode newRightAdjacent, List<BATONNode.RoutingTableEntry> newleftRoutingTable,  List<BATONNode.RoutingTableEntry> newRightRoutingTable, bool newFullLeftRoutingTable, bool newFullRightRoutingTable)
//		{
//				
//		}
//
//		
//		public void leave()
//		{
//			
//		}
//		
//		public void notifyLeave()
//		{
//			
//		}
//		
//		public void notiifyLeftAdjacentAboutReplacement(IBATONNode newBATONNode, IBATONNode oldBATONNode)
//		{
//			
//		}
//			
//		public void notiifyRightAdjacentAboutReplacement(IBATONNode newBATONNode, IBATONNode oldBATONNode)
//		{
//			
//		}
//			
//		//Data searchExact(...)
//		
//		
//		
//		
//		
//		public void joinAccepted(IBATONNode acceptingBATONNode, BATONNode.Position pos, IBATONNode adjacent, int newLevel, int newNumber)
//		{
//		
//		}
//		public void setAdjacent(IBATONNode newAdjacent, BATONNode.Position pos, IBATONNode prevBATONNode)
//		{
//			
//		}
//		public void notifyNewChild(IBATONNode notifyingBATONNode, BATONNode.Position pos, IBATONNode newChild)
//		{
//			
//		}
//		
//		public void requestRoutingTableForChild( IBATONNode requestingChild, BATONNode.Position pos)
//		{
//		
//		}
//		public void sendBATONNodeOnlyRoutingTableToChild(List<BATONNode.RoutingTableEntry> routingTable, BATONNode.Position pos)
//		{
//			
//		}
//		public void requestChildren(IBATONNode requestingBATONNode)
//		{
//			
//		}
//		public void notifyChildren(IBATONNode notifyingBATONNode, IBATONNode leftChild, IBATONNode rightChild)
//		{
//			
//		}
//		
//		public void setNewPeer(int routingTablepointer, BATONNode.Position pos, IBATONNode newChild)
//		{
//			
//		}
//		
//		public void notifyParentAboutReplacement(IBATONNode newChild, IBATONNode oldChild)
//		{
//			
//		}
//	}
//}
