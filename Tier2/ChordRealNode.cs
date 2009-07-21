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
using System.Threading;
using System.IO;

namespace Tashjik.Tier2
{
	/*********************************************
	* SEMANTICS: call methods on local machine
	*********************************************/
	internal class ChordRealNode : Tashjik.RealNode, IChordNode
	{

		//private readonly IController controller;
		private readonly ChordDataStore dataStore = null;


		public static bool operator==(ChordRealNode n1, IChordNode n2)
		{
			String strHash1 = n1.getHashedIP().ToString();
			String strHash2 = n2.getHashedIP().ToString();
	
			if(String.Compare(strHash1, strHash2)==0)
				return true;
			else
				return false;

		}

		public static bool operator!=(ChordRealNode n1, IChordNode n2)
		{
			String strHash1 = n1.getHashedIP().ToString();
			String strHash2 = n2.getHashedIP().ToString();
	
			if(String.Compare(strHash1, strHash2)!=0)
				return true;
			else
				return false;

		}

		public bool Equals(ChordRealNode n1, IChordNode n2)
		{
			String strHash1 = n1.getHashedIP().ToString();
			String strHash2 = n2.getHashedIP().ToString();
		
			if(String.Compare(strHash1, strHash2)==0)
				return true;
			else
				return false;

		}

		public override bool Equals(object obj)
		{
			Type thisType = this.GetType();
			String thisAssemblyQualifiedName = thisType.AssemblyQualifiedName;
			String thisFullName = thisType.FullName;
		
			Type objType = obj.GetType();
			String objAssemblyQualifiedName = objType.AssemblyQualifiedName;
			String objFullName = objType.FullName;

			if (thisAssemblyQualifiedName==objAssemblyQualifiedName
				&& thisFullName==objFullName) //the second condition may be unnecessary
			{
				// lock(nodeLock)
				// {
				if (engine.getIP().ToString() == ((ChordRealNode)obj).getIP().ToString())
					return true;
				else
					return false;
				// }
			}
			else
				return false;

			}

			public override int GetHashCode ()
			{
				//have to give a proper implementation for this
				return 1; // :O :O
			}

			public static bool operator<(ChordRealNode n1, IChordNode n2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = n2.getHashedIP().ToString();
		
				if(String.Compare(strHash1, strHash2)<0)
					return true;
				else 
					return false;
			}	

			public static bool operator>(ChordRealNode n1, IChordNode n2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = n2.getHashedIP().ToString();
	
				if(String.Compare(strHash1, strHash2)>0)
					return true;
				else
					return false;
			}


			public static bool operator>(ChordRealNode n1, byte[] hash2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = hash2.ToString();
	
				if(String.Compare(strHash1, strHash2)>0)
				return true;
				else return false;
			}

			public static bool operator<(ChordRealNode n1, byte[] hash2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = hash2.ToString();

				if(String.Compare(strHash1, strHash2)<0)
					return true;
				else 
					return false;
			}

			public static bool operator<(byte[] hash1, ChordRealNode n2)
			{
				String strHash1 = hash1.ToString();
				String strHash2 = n2.getHashedIP().ToString();
		
				if(String.Compare(strHash1, strHash2)<0)
					return true;
				else
					return false;
			}

			public static bool operator>(byte[] hash1, ChordRealNode n2)
			{
				String strHash1 = hash1.ToString();
				String strHash2 = n2.getHashedIP().ToString();

				if(String.Compare(strHash1, strHash2)>0)
					return true;
				else
					return false;
			}


			// INVARIANTS: predecessor<self<successor
			// finger[i+1]>finger [i]
			class Engine
			{
	
				private readonly Tashjik.Common.NodeBasic selfNodeBasic;
		
				//private readonly static Engine singleton = null;
				private readonly ChordRealNode self;
				private IChordNode predecessor;
				private IChordNode successor;

				//the 1st node will be an Node
				//need to take care of that
				private readonly IChordNode[] finger = new IChordNode[160];
				private int fingerNext = -1;
	
				/*public void stabilize()
				{
					AsyncCallback getPredecessorCallBack = new AsyncCallback(processGetPredecessorForStabilize);
					successor.beginGetPredecessor(getPredecessorCallBack);
					//if(((Node)self<x) && ((Node)x<successor))
					// successor = x;
					//successor.notify(self);
				}*/

				//NOT THREAD SAFE
				//to be used only to pass application state while making async calls
				class StabilizeAppState
				{
					public ChordRealNode self;
					public IChordNode successor;
					public AsyncCallback callBack;
					public Object appState;
				}

				public void beginStabilize(AsyncCallback beginStabilizeCallBack, Object appState)
				{
					/*Tashjik.Common.AsyncCallback_Object thisAppState = new Tashjik.Common.AsyncCallback_Object();
					thisAppState.callBack = beginStabilizeCallBack;
					thisAppState.obj = appState;
					*/
					StabilizeAppState stabilizeAppState = new StabilizeAppState();
					stabilizeAppState.appState = appState;
					stabilizeAppState.callBack = beginStabilizeCallBack;
					stabilizeAppState.self = self;
					stabilizeAppState.successor = successor;
	

					AsyncCallback getPredecessorCallBack = new AsyncCallback(processGetPredecessorForStabilize);
					successor.beginGetPredecessor(getPredecessorCallBack, stabilizeAppState);
				}

				static void processGetPredecessorForStabilize(IAsyncResult result)
				{
					ChordCommon.IChordNode_Object iNode_Object = (ChordCommon.IChordNode_Object)(result.AsyncState);
					Object appState = iNode_Object.obj;
	
					AsyncCallback callBack = ((StabilizeAppState)appState).callBack;
					Object appState1 = ((StabilizeAppState)appState).appState;



					IChordNode x = iNode_Object.node;
					ChordRealNode self = ((StabilizeAppState)appState).self;
					IChordNode successor = ((StabilizeAppState)appState).successor;

					if((self<(ChordRealNode)x) && ((ChordRealNode)x<(successor)))
						successor = x;
					successor.beginNotify(self, callBack, appState1);

				}		

				//NOT THREAD SAFE
				//to be used only to pass application state while making async calls
				class FixFingersAppState
				{
					public int fingerNext;
					public IChordNode[] finger;
					public AsyncCallback callback;
					public Object appState;
				}
	
				public void beginFixFingers(AsyncCallback beginStabilizeCallBack, Object appState)
				{
					//static int next = -1;
					fingerNext ++;
					if (fingerNext>160)
						fingerNext = 1;
					//bit wise addition is required
					//will require some effort
					byte[] C = new byte[20];


					int bitPos = Int32.MaxValue;
					int bytePos = Math.DivRem(fingerNext - 1,8, out bitPos);

					C[19-bytePos] = (byte) (1 << bitPos);


					FixFingersAppState fixFingersAppState = new FixFingersAppState();
					fixFingersAppState.callback = beginStabilizeCallBack;
					fixFingersAppState.appState = appState;
					fixFingersAppState.fingerNext = fingerNext;
					fixFingersAppState.finger = finger;
	
					AsyncCallback findSuccessorCallBack = new AsyncCallback(processFindSuccessorForFixFingers);
					beginFindSuccessor(Tashjik.Common.UtilityMethod.moduloAdd(selfNodeBasic.getHashedIP(), C), self, findSuccessorCallBack, fixFingersAppState);
				}


				static void processFindSuccessorForFixFingers(IAsyncResult result)
				{
					ChordCommon.IChordNode_Object iNode_Object = (ChordCommon.IChordNode_Object) (result.AsyncState);
		
					FixFingersAppState fixFingersAppState = (FixFingersAppState)(iNode_Object.obj);
					int i = fixFingersAppState.fingerNext;
					fixFingersAppState.finger[i] = iNode_Object.node;
					//Node finger = fixFingersAppState.finger[i];
					//finger = iNode_Object.node;


					//Object appState = iNode_Object.obj;

					AsyncCallback callBack = fixFingersAppState.callback;
					Object origAppState = fixFingersAppState.appState;

					if(!(callBack==null))
					{
						IAsyncResult res = new Tashjik.Common.ObjectAsyncResult(origAppState, true, true);
						callBack(res);
					}
				}

				//NOT THREAD SAFE
				//to be used only to pass application state while making async calls
				class CheckPredecessorAppState
				{
					public IChordNode predecessor;
					public AsyncCallback callback ;
					public Object appState;
				}

				public void beginCheckPredecessor(AsyncCallback checkPredecesorCallBack, Object appState)
				{	
					/*
					Tashjik.Common.AsyncCallback_Object thisAppState = new Tashjik.Common.AsyncCallback_Object();
					thisAppState.callBack = checkPredecesorCallBack;
					thisAppState.obj = appState;
					*/
					CheckPredecessorAppState checkPredecessorAppState = new CheckPredecessorAppState();
					checkPredecessorAppState.predecessor = predecessor;
					checkPredecessorAppState.callback = checkPredecesorCallBack;
					checkPredecessorAppState.appState = appState;
	
					AsyncCallback pingCallBack = new AsyncCallback(processPingForCheckPredecessor);
					predecessor.beginPing(pingCallBack, checkPredecessorAppState);
				}

				static void processPingForCheckPredecessor(IAsyncResult result)
				{
					Tashjik.Common.Bool_Object thisAppState = (Tashjik.Common.Bool_Object)(result.AsyncState);
					CheckPredecessorAppState checkPredecessorAppState = (CheckPredecessorAppState)(thisAppState.obj);
					IChordNode predecessor = checkPredecessorAppState.predecessor;

					if(!(thisAppState.b))
						predecessor = null;
	


					//AsyncCallback callBack = ((Tashjik.Common.AsyncCallback_Object)appState).callBack;
					//Object appState1 = ((Tashjik.Common.AsyncCallback_Object)appState).obj;
	
					if(!(checkPredecessorAppState.callback==null))
					{
						IAsyncResult res = new Tashjik.Common.ObjectAsyncResult(checkPredecessorAppState.appState, true, true);
					}
				}


				//NOT THREAD SAFE
				//to be used only to pass application state while making async calls
				class JoinAppState
				{
					public IChordNode successor;
					public AsyncCallback callback;
					public Object appState;
				}
	
				private void beginJoin(IChordNode joinNode, AsyncCallback joinCallBack, Object appState)
				{
					/*Tashjik.Common.AsyncCallback_Object thisAppState = new Tashjik.Common.AsyncCallback_Object();
					thisAppState.callBack = joinCallBack;
					thisAppState.obj = appState;
					*/
		
					JoinAppState joinAppState = new JoinAppState();
					joinAppState.appState = appState;
					joinAppState.successor = successor;
					joinAppState.callback = joinCallBack;
	
					predecessor = null;
					AsyncCallback findSuccessorCallBack = new AsyncCallback(processFindSuccessorForJoin);
					joinNode.beginFindSuccessor(self, self, findSuccessorCallBack, joinAppState);

				}

				static void processFindSuccessorForJoin(IAsyncResult result)
				{
					ChordCommon.IChordNode_Object iNode_Object = (ChordCommon.IChordNode_Object)(result.AsyncState);
					JoinAppState joinAppState = (JoinAppState)(iNode_Object.obj);
					joinAppState.successor = iNode_Object.node;
	
					//Object thisAppState = Node_Object.obj;

					AsyncCallback callBack = joinAppState.callback;
					Object appState1 = joinAppState.appState;

					if(!(callBack==null))
					{
						IAsyncResult res = new Tashjik.Common.ObjectAsyncResult(appState1, true, true);
						callBack(res);	
					}
				}
				private int leave()
				{
					return 1;
				}

				//DONE
				private void create()
				{
					predecessor.setIP(null);
					successor.setIP(selfNodeBasic.getIP());
				}

				public IPAddress getIP()
				{
					return selfNodeBasic.getIP();
				}

				public void setIP(IPAddress ip)
				{
					selfNodeBasic.setIP(ip);
				}

				public void notify(IChordNode possiblePred) //IPAddress possiblePredIP, byte[] possiblePredHashedIP)
				{
					if((predecessor==null) || (((ChordRealNode)possiblePred<self) && ((ChordRealNode)predecessor<possiblePred)))
					predecessor = possiblePred;
				}

				/*
				public Node findSuccessor(byte[] queryHashedKey, Node queryingNode)
				{
					if((Node)self<queryHashedKey && queryHashedKey<(Node)successor)
						return successor;
					else
					{
						Node closestPrecNode = findClosestPreceedingNode(queryHashedKey);
						if (closestPrecNode==self)
							return successor;
						else
						{
							return closestPrecNode.findSuccessor(queryHashedKey, queryingNode);
					}
				}
			}
			*/
			public void beginFindSuccessor(byte[] queryHashedKey, IChordNode queryingNode, AsyncCallback findSuccessorCallBack, Object appState)
			{
				ChordCommon.IChordNode_Object iNode_Object;
				if((ChordRealNode)self<queryHashedKey && queryHashedKey<(ChordRealNode)successor)
				{
					if(!(findSuccessorCallBack==null))
					{
						iNode_Object = new ChordCommon.IChordNode_Object();
						iNode_Object.node = successor;
						iNode_Object.obj = appState;

						IAsyncResult res = new ChordCommon.IChordNode_ObjectAsyncResult(iNode_Object, true, true);
						findSuccessorCallBack(res);
					}
				}
				else
				{
					IChordNode closestPrecNode = findClosestPreceedingNode(queryHashedKey);
					if (closestPrecNode==self)
					{
						if(!(findSuccessorCallBack==null))
						{
							iNode_Object = new ChordCommon.IChordNode_Object();
							iNode_Object.node = successor;
							iNode_Object.obj = appState;

							IAsyncResult res = new ChordCommon.IChordNode_ObjectAsyncResult(iNode_Object, true, true);
							findSuccessorCallBack(res);
						}
					}		
					else
						closestPrecNode.beginFindSuccessor(queryHashedKey, queryingNode, findSuccessorCallBack, appState);

				}
			}

			private IChordNode findClosestPreceedingNode(byte[] hashedKey)
			{
				for(int i=159; i>=0 && finger[i]!=null; i--)
					if((ChordRealNode)self<finger[i] && (ChordRealNode)(finger[i])<hashedKey)
						return finger[i];
				return self;
			}
		
			public byte[] getSelfNodeBasicHashedIP()
			{
				return selfNodeBasic.getHashedIP();
			}

			public IChordNode getPredecessor()
			{
				return predecessor;
			}

			public Engine(ChordRealNode encapsulatingNode)
			{
				try
				{
					selfNodeBasic = new Tashjik.Common.NodeBasic(Tashjik.Common.UtilityMethod.GetLocalHostIP());
				}
				catch (Tashjik.Common.Exception.LocalHostIPNotFoundException)
				{
					//local ip could not be found :O :O
					//crash the system
					//dunno how to do it though :(
				}

				self = encapsulatingNode;
				predecessor = null;
				successor = null;

				for(int i=159; i>=0; i--)
					finger[0] = null;
		
				fingerNext = -1;
			}

			//should never be called
			//reference to self is required
			//so call the constructor that requires it
			private Engine()
			{
			}
	
			//singleton creator
			/* public static Engine createEngine(Node encapsulatingNode)
			{
				//have to correct singleton call
				if(singleton==null)
					singleton = new Engine(encapsulatingNode);
				return singleton;
			}
			*/
		}	
		
		private class EngineMgr
		{
			private static EngineMgr singleton;
			private readonly Engine engine;
			private readonly int updationInterval;


			public EngineMgr(int interval, Engine eng)
			{
				updationInterval = interval;
				engine = eng;
			}
	
			public static EngineMgr createEngineMgr(int interval, Engine eng)
			{
				//have to correct singleton call
				if(singleton!=null)
					return singleton;
				else
				{
					singleton = new EngineMgr(interval, eng );
					return singleton;
				}
			}
	
			public void OnClockTick(object sender, ChordClockTimerArgs e, AsyncCallback OnClockTickCallBack, Object appState)
			{
				IPAddress IP = e.Engine.getIP();
				Console.WriteLine("Received a clock tick event. This is clock tick number {0}", e.TickCount);
				//ThreadStart job = new ThreadStart(ThreadJob);
				//Thread thread = new Thread(job);
				//thread.Start();
	
				Tashjik.Common.AsyncCallback_Object thisAppState = new Tashjik.Common.AsyncCallback_Object();
				thisAppState.callBack = OnClockTickCallBack;
				thisAppState.obj = appState;

				AsyncCallback checkPredecessorCallBack = new AsyncCallback(processCheckPredecesorForOnClockTick);
				engine.beginCheckPredecessor(checkPredecessorCallBack, thisAppState);
		
				/*Tashjik.Common.AsyncCallback_Object thisAppState = new Tashjik.Common.AsyncCallback_Object();
				thisAppState.callBack = OnClockTickCallBack;
				thisAppState.obj = appState;
				*/

				//AsyncCallback stabilizeCallBack = new AsyncCallback(processStabilizeForOnClockTick);
				//engine.beginStabilize(stabilizeCallBack, appState);
				//engine.stabilize();
				//engine.fixFingers();
			}

	
			static void processCheckPredecesorForOnClockTick(IAsyncResult result)
			{
				Tashjik.Common.AsyncCallback_Object thisAppState = (Tashjik.Common.AsyncCallback_Object)(result.AsyncState);
	
				//AsyncCallback callBack = ((Tashjik.Common.AsyncCallback_Object)thisAppState).callBack;
				//Object appState1 = ((Tashjik.Common.AsyncCallback_Object)thisAppState).obj;
		
				AsyncCallback stabilizeCallBack = new AsyncCallback(processStabilizeForOnClockTick);
				singleton.engine.beginStabilize(stabilizeCallBack, thisAppState);
		
			}

			static void processStabilizeForOnClockTick(IAsyncResult result)
			{	
				Tashjik.Common.AsyncCallback_Object thisAppState = (Tashjik.Common.AsyncCallback_Object)(result.AsyncState);
		
				AsyncCallback callBack = ((Tashjik.Common.AsyncCallback_Object)thisAppState).callBack;
				Object appState1 = ((Tashjik.Common.AsyncCallback_Object)thisAppState).obj;
	
				singleton.engine.beginFixFingers(callBack, appState1);
			}

			public class ChordClockTimerArgs : EventArgs
			{
				public int tickCount;
				private Engine engine;

				public ChordClockTimerArgs(int tickCount, Engine eng)
				{
					this.tickCount = tickCount;
					engine = eng;
				}

				public int TickCount
				{
					get { return tickCount; }
				}
	
				public Engine Engine
				{
					get { return engine; }
				}

			}


			public delegate void TimerEvent (object sender, ChordClockTimerArgs e);
	
			private event TimerEvent Timer;

			private void Start()
			{
				for(; ;)
				{
					Timer(this, new ChordClockTimerArgs(0, this.engine));
					Thread.Sleep(1000);
				}
			}
		}


		private readonly Engine engine;
		private readonly EngineMgr engineMgr;
	
		public ChordRealNode()
		{
			engine = new Engine(this);
			//engine = Engine.createEngine(this);
			engineMgr = EngineMgr.createEngineMgr(1000, engine);
			ChordDataStore dataStore = new ChordDataStore();

			/*
			NOTE: THIS FUNCTIONALITY GOES INTO ENGINE
			finger[0] = this;
			need to fill up the rest
			am avoiding filling them up with empty ProxyNodes
			because I want to keepProxyNodes immutable
			no need to create now
			for(int i=1;i<160;i++)
			finger[i] = blah blah blah
			*/
	

		}

		public IPAddress getIP()
		{
			return engine.getIP();
		}

		public void setIP(IPAddress ip)
		{
			engine.setIP(ip);
		}
		public byte[] getHashedIP()
		{
			return engine.getSelfNodeBasicHashedIP();
		}

		/*
		public Node findSuccessor(Node queryNode, Node queryingNode)
		{
			return findSuccessor(queryNode.getHashedIP(), queryingNode);
		}

		public Node findSuccessor(byte[] queryHashedKey, Node queryingNode)
		{
			return engine.findSuccessor(queryHashedKey, queryingNode);
		}
		*/

		public void beginFindSuccessor(IChordNode queryNode, IChordNode queryingNode, AsyncCallback findSuccessorCallBack, Object appState)
		{
			engine.beginFindSuccessor(queryNode.getHashedIP(), queryingNode, findSuccessorCallBack, appState);
		}

		public void beginFindSuccessor(byte[] queryHashedKey, IChordNode queryingNode, AsyncCallback findSuccessorCallBack, Object appState)
		{
			engine.beginFindSuccessor(queryHashedKey, queryingNode, findSuccessorCallBack, appState);
		}



		/* public Node getPredecessor()
		{
			return engine.getPredecessor();

		}
		*/

		public void beginGetPredecessor(AsyncCallback getPredecessorCallBack, Object appState)
		{
			IChordNode predecessor = engine.getPredecessor();
		
			ChordCommon.IChordNode_Object iNode_Object = new ChordCommon.IChordNode_Object();
			iNode_Object.node = predecessor;
			iNode_Object.obj = appState;

			if(!(getPredecessorCallBack==null))
			{
				IAsyncResult res = new ChordCommon.IChordNode_ObjectAsyncResult(iNode_Object, false,true);
				getPredecessorCallBack(res);
			}
		}

		/* public void notify(Node possiblePred)
		{
			engine.notify(possiblePred);
		
		}
		*/

		public void beginNotify(IChordNode possiblePred, AsyncCallback notifyCallBack, Object appState)
		{
	

			engine.notify(possiblePred);
			if(!(notifyCallBack==null))
			{
				IAsyncResult res = new Tashjik.Common.ObjectAsyncResult(appState, true, true);
				notifyCallBack(res);
			}
		}


		/*
		public bool ping()
		{
			return true;

		}
		*/


		public void beginPing(AsyncCallback pingCallBack, Object appState)
		{
			Tashjik.Common.Bool_Object bool_Object = new Tashjik.Common.Bool_Object();
			bool_Object.b = true;
			bool_Object.obj = appState;
			if(!(pingCallBack==null))
			{
				IAsyncResult res = new Tashjik.Common.Bool_ObjectAsyncResult(bool_Object, true, true);
				pingCallBack(res);
			}
		}

		/* public Tashjik.Common.Data getData(byte[] byteKey)
		{
			return dataStore.getData(byteKey);
		}
		*/

		public void beginGetData(byte[] byteKey, AsyncCallback getDataCallBack, Object appState)
		{
			//once DataStore gets complex, this operation should not complete on the same synchronous thread
			Tashjik.Common.Data_Object data_Object = new Tashjik.Common.Data_Object();
			data_Object.data = dataStore.getData(byteKey);
			data_Object.obj = appState;

			IAsyncResult getDataResult = new Tashjik.Common.Data_ObjectAsyncResult(data_Object, true, true);
			getDataCallBack(getDataResult);
		}

		public void beginPutData(byte[] byteKey, Stream data, UInt64 dataLength, AsyncCallback putDataCallBack, Object appState)
		{
			//once DataStore gets complex, this operation should not complete on the same synchronous thread
			dataStore.putData(byteKey, data, dataLength);
			IAsyncResult putDataResult = new Tashjik.Common.ObjectAsyncResult(appState, true, true);
			putDataCallBack(putDataResult);


		}	

	}
}
