/************************************************************
* File Name: ChordRealNode.cs
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
		private readonly ChordDataStore dataStore = new ChordDataStore();


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

			/*public static bool operator<(IChordNode n1, ChordRealNode n2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = n2.getHashedIP().ToString();
		
				if(String.Compare(strHash1, strHash2)<0)
					return true;
				else 
					return false;
			}	*/
			
			public static bool operator>(ChordRealNode n1, IChordNode n2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = n2.getHashedIP().ToString();
	
				if(String.Compare(strHash1, strHash2)>0)
					return true;
				else
					return false;
			}

	/*		public static bool operator>(IChordNode n1, ChordRealNode n2)
			{
				String strHash1 = n1.getHashedIP().ToString();
				String strHash2 = n2.getHashedIP().ToString();
	
				if(String.Compare(strHash1, strHash2)>0)
					return true;
				else
					return false;
			}
	*/		
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



				public void beginStabilize(AsyncCallback beginStabilizeCallBack, Object appState)
				{
					Console.WriteLine("Chord::Engine::beginStabilize ENTER");

					if(successor == self)
					{
						if(beginStabilizeCallBack!=null)
						{
							IAsyncResult res = new Tashjik.Common.ObjectAsyncResult(appState, true, true);
							beginStabilizeCallBack(res);
						}
						else
							return;
					}
					
					Tashjik.Common.AsyncCallback_Object thisAppState = new Tashjik.Common.AsyncCallback_Object();
					thisAppState.callBack = beginStabilizeCallBack;
					thisAppState.obj = appState;
					
					Console.WriteLine("Chord::Engine::beginStabilize successor != self");
					AsyncCallback getPredecessorCallBack = new AsyncCallback(processGetPredecessorForStabilize);
					successor.beginGetPredecessor(getPredecessorCallBack, thisAppState);
	
					Console.WriteLine("Chord::Engine::beginStabilize EXIT");

				}
				
				void processGetPredecessorForStabilize(IAsyncResult result)
				{
					Console.WriteLine("Chord::Engine::processGetPredecessorForStabilize ENTER");

					ChordCommon.IChordNode_Object iNode_Object = (ChordCommon.IChordNode_Object)(result.AsyncState);
					Object appState = iNode_Object.obj;
	
					AsyncCallback callBack = ((Tashjik.Common.AsyncCallback_Object)appState).callBack;
					Object appState1 = ((Tashjik.Common.AsyncCallback_Object)appState).obj;

					IChordNode x = iNode_Object.node;
					if(x==null)
					{
						Console.WriteLine("Chord::Engine::processGetPredecessorForStabilize UNKNOWN_PREDECESSOR");
						if(callBack != null)
						{	iNode_Object = new ChordCommon.IChordNode_Object();
							iNode_Object.node = null;
							iNode_Object.obj = appState1;

							IAsyncResult res = new ChordCommon.IChordNode_ObjectAsyncResult(iNode_Object, true, true);
					//		callBack(res);
						}
					}
					else
					{
					
						Console.WriteLine("Chord::Engine::processGetPredecessorForStabilize before condition check");
						if((self<(ChordRealNode)x) && ((ChordRealNode)x<(successor)))
							successor = x;
						Console.WriteLine("Chord::Engine::processGetPredecessorForStabilize before calling beginNotify on successor");
						

					}		
					successor.beginPredecessorNotify(self, callBack, appState1);
				}
				
				public void beginFixFingers(AsyncCallback beginStabilizeCallBack, Object appState)
				{
					Console.WriteLine("Chord::Engine::beginFixFingers ENTER");
					//static int next = -1;
					fingerNext ++;
					if (fingerNext>=160)
						fingerNext = 0;
					//bit wise addition is required
					//will require some effort
					byte[] C = new byte[20];


					int bitPos = Int32.MaxValue;
					int bytePos = Math.DivRem(fingerNext - 1,8, out bitPos);

					C[19-bytePos] = (byte) (1 << bitPos);

					Tashjik.Common.AsyncCallback_Object thisAppState = new Tashjik.Common.AsyncCallback_Object();
					thisAppState.callBack = beginStabilizeCallBack;
					thisAppState.obj = appState;


	
					AsyncCallback findSuccessorCallBack = new AsyncCallback(processFindSuccessorForFixFingers);
					beginFindSuccessor(Tashjik.Common.UtilityMethod.moduloAdd(selfNodeBasic.getHashedIP(), C), self, findSuccessorCallBack, thisAppState, new Guid("00000000-0000-0000-0000-000000000000"));
				}


				void processFindSuccessorForFixFingers(IAsyncResult result)
				{
					Console.WriteLine("Chord::ChordRealNode::Engine processFindSuccessorForFixFingers ENTER");
					ChordCommon.IChordNode_Object iNode_Object = (ChordCommon.IChordNode_Object) (result.AsyncState);
		
					Tashjik.Common.AsyncCallback_Object fixFingersAppState = (Tashjik.Common.AsyncCallback_Object)(iNode_Object.obj);
					
					this.finger[fingerNext] = iNode_Object.node;

					AsyncCallback callBack = fixFingersAppState.callBack;
					Object origAppState = fixFingersAppState.obj;

					if(!(callBack==null))
					{
						IAsyncResult res = new Tashjik.Common.ObjectAsyncResult(origAppState, true, true);
						callBack(res);
					}
				}

			

				public void beginCheckPredecessor(AsyncCallback checkPredecesorCallBack, Object appState)
				{	
					Console.WriteLine("Chord::ChordRealNode::Engine beginCheckPredecessor ENTER");
					if(predecessor==null)
					{						
						if(checkPredecesorCallBack!=null)
						{
							IAsyncResult res = new Tashjik.Common.ObjectAsyncResult(appState, true, true);
							checkPredecesorCallBack(res);
						}
						else
							return;
					}
					
					Tashjik.Common.AsyncCallback_Object thisAppState = new Tashjik.Common.AsyncCallback_Object();
					thisAppState.callBack = checkPredecesorCallBack;
					thisAppState.obj = appState;
						
					AsyncCallback pingCallBack = new AsyncCallback(processPingForCheckPredecessor);
					predecessor.beginPing(pingCallBack, thisAppState);
					
					
				}
				
				void processPingForCheckPredecessor(IAsyncResult result)
				{
					Console.WriteLine("Chord::ChordRealNode::Engine processPingForCheckPredecessor ENTER");
					Tashjik.Common.Bool_Object thisAppState = (Tashjik.Common.Bool_Object)(result.AsyncState);
					Tashjik.Common.AsyncCallback_Object checkPredecessorAppState = (Tashjik.Common.AsyncCallback_Object)(thisAppState.obj);
					

					if(!(thisAppState.b))
						predecessor = null;
	


					//AsyncCallback callBack = ((Tashjik.Common.AsyncCallback_Object)appState).callBack;
					//Object appState1 = ((Tashjik.Common.AsyncCallback_Object)appState).obj;
	
					if(!(checkPredecessorAppState.callBack==null))
					{
						Console.WriteLine("Chord::ChordRealNode::Engine processPingForCheckPredecessor callBack!=null");
						IAsyncResult res = new Tashjik.Common.ObjectAsyncResult(checkPredecessorAppState.obj, true, true);
						checkPredecessorAppState.callBack(res);
					}
				}


	
				//this is the method that the node uses to 
				//initilize its own state based on a supplied bootstrap node
				//here, it has been suppied with a bootstrap node
				//which will be used to initilize itself
				//as well as tell other nodes in n/w of its existence
				public void beginJoin(IChordNode joinNode, AsyncCallback joinCallBack, Object appState)
				{
					Console.WriteLine("ChordRealNode::Engine::beginJoin ENTER");
					Tashjik.Common.AsyncCallback_Object thisAppState = new Tashjik.Common.AsyncCallback_Object();
					thisAppState.callBack = joinCallBack;
					thisAppState.obj = appState;
					
					predecessor = null;
					
					AsyncCallback findSuccessorCallBack = new AsyncCallback(processFindSuccessorForJoin);
					Console.WriteLine("ChordRealNode::Engine::beginJoin before calling beginFindSuccessor");
					joinNode.beginFindSuccessor(self.getHashedIP(), self, findSuccessorCallBack, thisAppState, new Guid("00000000-0000-0000-0000-000000000000"));

				}

				void processFindSuccessorForJoin(IAsyncResult result)
				{
					Console.WriteLine("ChordRealNode::Engine::processFindSuccessorForJoin ENTER");
					
					ChordCommon.IChordNode_Object iNode_Object = (ChordCommon.IChordNode_Object)(result.AsyncState);
					Tashjik.Common.AsyncCallback_Object thisAppState = (Tashjik.Common.AsyncCallback_Object)(iNode_Object.obj);
					IChordNode retrievedSuccessor = iNode_Object.node;
					Console.WriteLine("ChordRealNode::Engine::processFindSuccessorForJoin successor has been set");
					
					successor = retrievedSuccessor;
					AsyncCallback callBack = thisAppState.callBack;
					Object appState1 = thisAppState.obj;

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
					if(predecessor==null) //|| ((/*(ChordRealNode)*/possiblePred<self) && (/*(ChordRealNode)*/predecessor<possiblePred)))
						predecessor = possiblePred;
					else 
					{
						String strPossiblePredHash = possiblePred.getHashedIP().ToString();
						String strSelfHash = self.getHashedIP().ToString();
						String strPredecessorHash = predecessor.getHashedIP().ToString();
						
						if((String.Compare(strPossiblePredHash, strSelfHash)<0) && (String.Compare(strPredecessorHash, strPossiblePredHash)<0))
							predecessor = possiblePred;
					}
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
			public void beginFindSuccessor(byte[] queryHashedKey, IChordNode queryingNode, AsyncCallback findSuccessorCallBack, Object appState, Guid relayTicket)
			{
				Console.WriteLine("Chord::engine::beginFindSuccessor ENTER");
				ChordCommon.IChordNode_Object iNode_Object;
				if((ChordRealNode)self<queryHashedKey && queryHashedKey<(ChordRealNode)successor)
				{
					Console.WriteLine("Chord::engine::beginFindSuccessor query falls inbetween node and successor");
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
					Console.WriteLine("Chord::engine::beginFindSuccessor query DOES NOT fall inbetween node and successor");
					IChordNode closestPrecNode = findClosestPreceedingNode(queryHashedKey);
					if (closestPrecNode==self)
					{
						Console.WriteLine("Chord::engine::beginFindSuccessor closestPrecNode==self");
						if(!(findSuccessorCallBack==null))
						{
							Console.WriteLine("Chord::engine::beginFindSuccessor if(!(findSuccessorCallBack==null))");
							iNode_Object = new ChordCommon.IChordNode_Object();
							iNode_Object.node = successor;
							iNode_Object.obj = appState;
						
							Console.WriteLine("Chord::engine::beginFindSuccessor before new IChordNode_ObjectAsyncResult");
							IAsyncResult res = new ChordCommon.IChordNode_ObjectAsyncResult(iNode_Object, true, true);
							Console.WriteLine("Chord::engine::beginFindSuccessor before calling findSuccessorCallBack");
							findSuccessorCallBack(res);
						}
					}		
					else
					{
						Console.WriteLine("Chord::engine::beginFindSuccessor relaying request to closestPrecNode");
						closestPrecNode.beginFindSuccessor(queryHashedKey, queryingNode, findSuccessorCallBack, appState, relayTicket);
					}

				}
				Console.WriteLine("Chord::engine::beginFindSuccessor EXIT");
			}

			private IChordNode findClosestPreceedingNode(byte[] hashedKey)
			{
				Console.WriteLine("Chord::engine::findClosestPreceedingNode ENTER");
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

//				self = encapsulatingNode;
//				
//				for(int i=159; i>=0; i--)
//					finger[i] = self;
//				predecessor = self;
//				successor = finger[0];
//		
//				fingerNext = -1;
				
				self = encapsulatingNode;
                                predecessor = null;
                                successor = self;

                                for(int i=159; i>=0; i--)
                                        finger[i] = null;
                
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
			//private static EngineMgr singleton;
			private readonly Engine engine;
			private readonly int updationInterval;
			private int maintainanceCounter;

			public EngineMgr(int interval, Engine eng)
			{
				updationInterval = interval;
				engine = eng;
				maintainanceCounter = 0;
				
				ThreadStart job = new ThreadStart(Start);
				Thread thread = new Thread(job);
				thread.Start();
			}
	
			public static EngineMgr createEngineMgr(int interval, Engine eng)
			{
				//have to correct singleton call
/*				if(singleton!=null)
					return singleton;
				else
				{
					singleton = new EngineMgr(interval, eng );
					return singleton;
				}
*/
				return new EngineMgr(interval, eng );
			}
	
			public void OnClockTick() //object sender, ChordClockTimerArgs e, AsyncCallback OnClockTickCallBack, Object appState)
			{
				
				IPAddress IP = engine.getIP();
				//Console.WriteLine("Received a clock tick event. This is clock tick number {0}"); //, e.TickCount);
				//ThreadStart job = new ThreadStart(ThreadJob);
				//Thread thread = new Thread(job);
				//thread.Start();
	
				//Tashjik.Common.AsyncCallback_Object thisAppState = new Tashjik.Common.AsyncCallback_Object();
				//thisAppState.callBack = null; //OnClockTickCallBack;
				//thisAppState.obj = null;//appState;
				if(maintainanceCounter == 0)
				{
					//AsyncCallback checkPredecessorCallBack = new AsyncCallback(processCheckPredecesorForOnClockTick);
					//engine.beginCheckPredecessor(checkPredecessorCallBack, null);
					engine.beginCheckPredecessor(null, null);
					maintainanceCounter++;
				}
				else if(maintainanceCounter == 1)
				{
					engine.beginStabilize(null, null);
					maintainanceCounter++;
				}
				else if(maintainanceCounter == 2)
				{
					engine.beginFixFingers(null, null);
					//maintainanceCounter++;
					maintainanceCounter = 0;
				}
				//engine.beginStabilize(null, null);
				//engine.beginFixFingers(null, null);
	
	
				/*Tashjik.Common.AsyncCallback_Object thisAppState = new Tashjik.Common.AsyncCallback_Object();
				thisAppState.callBack = OnClockTickCallBack;
				thisAppState.obj = appState;
				*/

				//AsyncCallback stabilizeCallBack = new AsyncCallback(processStabilizeForOnClockTick);
				//engine.beginStabilize(stabilizeCallBack, appState);
				//engine.stabilize();
				//engine.fixFingers();
			}

	
			void processCheckPredecesorForOnClockTick(IAsyncResult result)
			{
				Console.WriteLine("ChordRealNode::processCheckPredecesorForOnClockTick ENTER");
				Tashjik.Common.AsyncCallback_Object thisAppState = (Tashjik.Common.AsyncCallback_Object)(result.AsyncState);
	
				//AsyncCallback callBack = ((Tashjik.Common.AsyncCallback_Object)thisAppState).callBack;
				//Object appState1 = ((Tashjik.Common.AsyncCallback_Object)thisAppState).obj;
		
				AsyncCallback stabilizeCallBack = new AsyncCallback(processStabilizeForOnClockTick);
				engine.beginStabilize(stabilizeCallBack, thisAppState);
		
			}

			void processStabilizeForOnClockTick(IAsyncResult result)
			{	
				Console.WriteLine("ChordRealNode::processStabilizeForOnClockTick ENTER");
				Tashjik.Common.AsyncCallback_Object thisAppState = (Tashjik.Common.AsyncCallback_Object)(result.AsyncState);
		
				AsyncCallback callBack = null;
				Object appState1 = null;
				
				if(thisAppState!=null)
				{
					callBack = ((Tashjik.Common.AsyncCallback_Object)thisAppState).callBack;
					appState1 = ((Tashjik.Common.AsyncCallback_Object)thisAppState).obj;
				}
				engine.beginFixFingers(callBack, appState1);
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
				for(int i=0; i<6; i++)
					Thread.Sleep(10000);
				for(; ;)
				{
					//Timer(this, new ChordClockTimerArgs(0, this.engine));
					Thread.Sleep(updationInterval);
					OnClockTick();
				}
			}
		}


		private readonly Engine engine;
		private readonly EngineMgr engineMgr;
	
		public ChordRealNode()
		{
			engine = new Engine(this);
			//engine = Engine.createEngine(this);
			engineMgr = EngineMgr.createEngineMgr(10000, engine);
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
		public void beginFindSuccessor(IChordNode queryNode, IChordNode queryingNode, AsyncCallback findSuccessorCallBack, Object appState, Guid relayTicket)
		{
			engine.beginFindSuccessor(queryNode.getHashedIP(), queryingNode, findSuccessorCallBack, appState, relayTicket);
		}

		public void beginFindSuccessor(byte[] queryHashedKey, IChordNode queryingNode, AsyncCallback findSuccessorCallBack, Object appState, Guid relayTicket)
		{
			engine.beginFindSuccessor(queryHashedKey, queryingNode, findSuccessorCallBack, appState, relayTicket);
		}



		/* public Node getPredecessor()
		{
			return engine.getPredecessor();

		}
		*/

		public void beginGetPredecessor(AsyncCallback getPredecessorCallBack, Object appState)
		{
			Console.WriteLine("ChordRealNode::beginGetPredecessor ENTER");

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

		//NOT A PART OF COMMON CHORD INTERFACE
		public IChordNode getPredecessor()
		{
			Console.WriteLine("ChordRealNode::getPredecessor ENTER");
			return engine.getPredecessor();
		}
		/* public void notify(Node possiblePred)
		{
			engine.notify(possiblePred);
		
		}
		*/
		internal void beginJoin(IChordNode joinNode, AsyncCallback joinCallBack, Object appState)
		{
			engine.beginJoin(joinNode, joinCallBack, appState);
		}
		public void beginPredecessorNotify(IChordNode possiblePred, AsyncCallback notifyCallBack, Object appState)
		{
	

			engine.notify(possiblePred);
			if(!(notifyCallBack==null))
			{
				IAsyncResult res = new Tashjik.Common.ObjectAsyncResult(appState, true, true);
				notifyCallBack(res);
			}
		}
		
		public void predecessorNotify(IChordNode possiblePred)
		{
			engine.notify(possiblePred);
		}


		/*
		public bool ping()
		{
			return true;

		}
		*/


		public void beginPing(AsyncCallback pingCallBack, Object appState)
		{
			Console.WriteLine("ChordRealNode::beginPing ENTER");
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

		public void beginPutData(byte[] key, byte[] data, int offset, int size, AsyncCallback putDataCallBack, Object appState)
		{
			Console.WriteLine("ChordRealNode::beginPutData ENTER");
			//once DataStore gets complex, this operation should not complete on the same synchronous thread
			dataStore.putData(key, data, offset, size);
			IAsyncResult putDataResult = new Tashjik.Common.ObjectAsyncResult(appState, true, true);
			putDataCallBack(putDataResult);


		}	

	}
}
