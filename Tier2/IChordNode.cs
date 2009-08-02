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
using System.IO;

namespace Tashjik.Tier2
{
	internal interface IChordNode
	{
		//Node findSuccessor(Node queryNode, Node queryingNode);
		//Node findSuccessor(byte[] queryHashedKey, Node queryingNode);
		void beginFindSuccessor(IChordNode queryNode, IChordNode queryingNode, AsyncCallback findSuccessorCallBack, Object appState);
		void beginFindSuccessor(byte[] queryHashedKey, IChordNode queryingNode, AsyncCallback findSuccessorCallBack, Object appState);
		//Node getPredecessor();
		void beginGetPredecessor(AsyncCallback getPredecessorCallBack, Object appState);
		//void notify(Node possiblePred);
		void beginNotify(IChordNode possiblePred, AsyncCallback notifyCallBack, Object appState);
		//bool ping();
		void beginPing(AsyncCallback pingCallBack, Object appState);
		byte[] getHashedIP();
		IPAddress getIP();
		void setIP(IPAddress ip);
		//Tashjik.Common.Data getData(byte[] byteKey);
		void beginGetData(byte[] byteKey, AsyncCallback getDataCallBack, Object appState);
		void beginPutData(byte[] key, byte[] data, int offset, int size, AsyncCallback putDataCallBack, Object appState);
		//void putData(byte[] byteKey, Tashjik.Common.Data data);

	}
}
