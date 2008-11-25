﻿/************************************************************
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

namespace Tashjik.Tier2.Chord
{
	public interface INode : Tier2.Common.INode
	{
		//INode findSuccessor(INode queryNode, INode queryingNode);
		//INode findSuccessor(byte[] queryHashedKey, INode queryingNode);
		void beginFindSuccessor(INode queryNode, INode queryingNode, AsyncCallback findSuccessorCallBack, Object appState);
		void beginFindSuccessor(byte[] queryHashedKey, INode queryingNode, AsyncCallback findSuccessorCallBack, Object appState);
		//INode getPredecessor();
		void beginGetPredecessor(AsyncCallback getPredecessorCallBack, Object appState);
		//void notify(INode possiblePred);
		void beginNotify(INode possiblePred, AsyncCallback notifyCallBack, Object appState);
		//bool ping();
		void beginPing(AsyncCallback pingCallBack, Object appState);
		byte[] getHashedIP();
		IPAddress getIP();
		void setIP(IPAddress ip);
		//Tashjik.Common.Data getData(byte[] byteKey);
		void beginGetData(byte[] byteKey, AsyncCallback getDataCallBack, Object appState);
		void beginPutData(byte[] byteKey, Tashjik.Common.Data data, AsyncCallback putDataCallBack, Object appState);
		//void putData(byte[] byteKey, Tashjik.Common.Data data);

	}
}
