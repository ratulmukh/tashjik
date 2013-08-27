/************************************************************
* File Name: IProxyNodeController.cs
*
* Author: Sameer Samarth
* sameersamarthpATgmaildotcom
*
* This software is licensed under the terms and conditions of
* the MIT license, as given below.
*
* Copyright (c) <2008-2010> <Sameer Samarth>
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
* Originalimplementation taken from Zhimera
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
//
//
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//
//namespace Graph
//{
//    class Program
//    {
//        public void entry()
//        {
//            int num_nodes = 500;
//            bool isGraphConnected;
//            GraphImpl gi = new GraphImpl(num_nodes);
//            gi.CreateNodes(gi.GetTotalNodes());
//            gi.CreateEdges(gi.GetTotalNodes());
//            //gi.display_connections();
//            isGraphConnected = gi.isGraphConnected();
//
//            if (!isGraphConnected)
//            {
//                Console.WriteLine("Graph not connected");
//            }
//            else
//            {
//                Console.WriteLine("Graph connected");
//
//                Dijkstra dij = new Dijkstra();
//                dij.DikkstraImpl(gi, 0, 8);
//            }
//      
//        }
//
//        // for shortest path example
//        public void entry1()
//        {
//            int num_nodes = 6;
//            bool isGraphConnected;
//            GraphImpl gi = new GraphImpl(num_nodes);
//
//            //create 8 nodes
//            
//
//            Node from = new Node();
//            Node to = new Node();
//
//            /***************************** Example 1 ***************************************************
//            //================================= Node 0 ========================================
//            for (int z = 0; z < 8; z++)
//            {
//                gi.addNode(z, num_nodes);
//            }
//            
//            from = gi.searchNode(0);
//            to = gi.searchNode(1);
//
//            from.addAdjNode(to, 5);
//            to.addAdjNode(from, 5);
//
//            to = gi.searchNode(5);
//            from.addAdjNode(to, 3);
//            to.addAdjNode(from, 3);
//
//            //================================= Node 1 ========================================
//            from = gi.searchNode(1);
//            to = gi.searchNode(2);
//
//            from.addAdjNode(to, 2);
//            to.addAdjNode(from, 2);
//
//            to = gi.searchNode(6);
//            from.addAdjNode(to, 3);
//            to.addAdjNode(from, 3);
//
//            //================================= Node 2 ========================================
//            from = gi.searchNode(2);
//            to = gi.searchNode(3);
//
//            from.addAdjNode(to, 6);
//            to.addAdjNode(from, 6);
//
//            to = gi.searchNode(7);
//            from.addAdjNode(to, 10);
//            to.addAdjNode(from, 10);
//            //================================= Node 3 ========================================
//            from = gi.searchNode(3);
//            to = gi.searchNode(4);
//
//            from.addAdjNode(to, 3);
//            to.addAdjNode(from, 3);
//            //================================= Node 4 ========================================
//            from = gi.searchNode(4);
//            to = gi.searchNode(5);
//
//            from.addAdjNode(to, 8);
//            to.addAdjNode(from, 8);
//
//            to = gi.searchNode(7);
//            from.addAdjNode(to, 5);
//            to.addAdjNode(from, 5);
//            //================================= Node 5 ========================================
//            from = gi.searchNode(5);
//            to = gi.searchNode(6);
//
//            from.addAdjNode(to, 7);
//            to.addAdjNode(from, 7);
//
//            //================================= Node 6 ========================================
//            from = gi.searchNode(6);
//            to = gi.searchNode(7);
//
//            from.addAdjNode(to, 2);
//            to.addAdjNode(from, 2);
//            //================================= Node 7 ========================================
//            //all done
//            ****************************** Example 1 End ***************************************************/
//
//            //****************************** Example 2 ***************************************************
//            for (int z = 0; z < 6; z++)
//            {
//                gi.addNode(z, num_nodes);
//            }
//
//            //================================= Node 0 ========================================
//            from = gi.searchNode(0);
//            to = gi.searchNode(1);
//
//            from.addAdjNode(to, 7);
//            to.addAdjNode(from, 7);
//
//            to = gi.searchNode(4);
//
//            from.addAdjNode(to, 14);
//            to.addAdjNode(from, 14);
//
//            to = gi.searchNode(5);
//            from.addAdjNode(to, 9);
//            to.addAdjNode(from, 9);
//
//            //================================= Node 1 ========================================
//            from = gi.searchNode(1);
//            to = gi.searchNode(2);
//
//            from.addAdjNode(to, 15);
//            to.addAdjNode(from, 15);
//            to = gi.searchNode(5);
//
//            from.addAdjNode(to, 10);
//            to.addAdjNode(from, 10);
//
//
//            //================================= Node 2 ========================================
//            from = gi.searchNode(2);
//            to = gi.searchNode(3);
//
//            from.addAdjNode(to, 6);
//            to.addAdjNode(from, 6);
//            to = gi.searchNode(5);
//
//            from.addAdjNode(to, 11);
//            to.addAdjNode(from, 11);
//
//            //================================= Node 3 ========================================
//            from = gi.searchNode(3);
//            to = gi.searchNode(4);
//
//            from.addAdjNode(to, 9);
//            to.addAdjNode(from, 9);
//            
//            //================================= Node 4 ========================================
//            from = gi.searchNode(4);
//            to = gi.searchNode(5);
//
//            from.addAdjNode(to, 2);
//            to.addAdjNode(from, 2);
//            
//            //================================= Node 5 ========================================
//            
//            
//            /****************************** Example 2 End ***************************************************/
//
//            gi.display_connections();
//            isGraphConnected = gi.isGraphConnected();
//            if (!isGraphConnected)
//            {
//                Console.WriteLine("Graph not connected");
//            }
//            else
//            {
//                Console.WriteLine("Graph connected");
//         
//                Dijkstra dij = new Dijkstra();
//                // source = 0, dest = 3
//                dij.DikkstraImpl(gi, 0, 3);
//            }
//        }
//
//        static void Main(string[] args)
//        {
//            //StreamWriter log_out; 
//
//            //log_out = new StreamWriter("F:/LiveProjects/C#Rookie/Graph/logfile.txt");
//            //Console.SetOut(log_out);
//
//            Program p = new Program();
//            //p.entry();
//            p.entry1();
//            Console.WriteLine("This is the end of the log file.");
//            //log_out.Close(); 
//            Console.Read();
//        }
//    }
//}
