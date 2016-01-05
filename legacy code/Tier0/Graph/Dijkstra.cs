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

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Collections;
//
//namespace Graph
//{
//    class Dijkstra
//    {
//        public void DikkstraImpl(GraphImpl g, int start_vertex)
//        {
//            /***********************************************************************************************
//             * [1] Assign to every node a distance value. Set it to zero for our initial node and to infinity 
//             * for all other nodes.
//             * [2] Mark all nodes as unvisited. Set initial node as current.
//             * [3] For current node, consider all its unvisited neighbours and calculate their distance 
//             * (from the initial node). If this distance is less than the previously recorded distance 
//             * (infinity in the beginning, zero for the initial node), overwrite the distance.
//             * [4] When we are done considering all neighbours of the current node, mark it as visited. 
//             * A visited node will not be checked ever again; its distance recorded now is final and minimal.
//             * [5] Set the unvisited node with the smallest distance (from the initial node) as the next 
//             * "current node" and continue from step 3.
//             * ~ Wiki
//             * ********************************************************************************************/
//
//            Queue<int> visited_vertices = new Queue<int>(g.GetTotalNodes());
//            int[] distance_list = new Int32[g.GetTotalNodes()];
//            List<int> current_list = new List<int>(g.GetTotalNodes());
//
//            for (int z = 0; z < g.GetTotalNodes(); z++)
//            {
//                distance_list[z] = Int32.MaxValue;
//            }
//            distance_list[start_vertex] = 0;
//            current_list.Add(start_vertex);
//
//
//            Node start_node = g.searchNode(start_vertex);
//            while (current_list.Count != 0)
//            {
//                List<int> pseudo_priority_list = new List<int>(current_list.Count);
//                
//                foreach(int value in current_list)
//                {
//                    int xxx = distance_list[value];
//                    pseudo_priority_list.Add(xxx);
//                }
//                
//                int small = pseudo_priority_list.Min();
//                int index = pseudo_priority_list.IndexOf(small);
//                int current_vertex = current_list[index];
//
//                Node current_node = g.searchNode(current_vertex);
//                List<Edge> edge_list = new List<Edge>();
//                edge_list = current_node.GetEdgeList();
//
//                for (int z = 0; z < edge_list.Count; z++)
//                {
//                    int r = edge_list[z]._dest.node_id;
//
//                    if (!visited_vertices.Contains(r) && !current_list.Contains(r))
//                    {
//                        current_list.Add(r);
//                        
//                    }
//                    int edge1 = g.Edge_cost(start_node, current_vertex);
//                    if (edge1 == -1)
//                    {
//                        edge1 = distance_list[current_vertex];
//                    }
//                    if (edge1 > distance_list[current_vertex])
//                    {
//                        Console.WriteLine("For Edge1, Current vertex = " + current_vertex + " , R = " + r);
//                        
//                        edge1 = distance_list[current_vertex];
//                    }
//                    int edge2 = g.Edge_cost(current_node, r);
//                    if (distance_list[r] > (edge1 + edge2))
//                    {
//                        Console.WriteLine("For Edge2, Current vertex = " + current_vertex + " , R = " + r);
//                        
//                        distance_list[r] = edge1 + edge2;
//                    }
//                }
//                visited_vertices.Enqueue(current_vertex);
//                
//                current_list.Remove(current_vertex);
//                
//            }
//            for (int z = 0; z < g.GetTotalNodes(); z++)
//            {
//                Console.WriteLine("shortest cost to reach " + z + " from " + start_vertex + " -> " + distance_list[z]);
//            }
//           
//        }
//
//        public void DikkstraImpl(GraphImpl g, int start_vertex, int dest_vertex)
//        {  
//            Queue<int> visited_vertices = new Queue<int>(g.GetTotalNodes());
//            int[] distance_list = new Int32[g.GetTotalNodes()];
//            List<int> current_list = new List<int>(g.GetTotalNodes());
//
//            int[] shortest_vertices = new Int32[g.GetTotalNodes()];
//
//            for (int z = 0; z < g.GetTotalNodes(); z++)
//            {
//                distance_list[z] = Int32.MaxValue;
//            }
//            distance_list[start_vertex] = 0;
//            current_list.Add(start_vertex);
//
//
//            Node start_node = g.searchNode(start_vertex);
//            while (current_list.Count != 0)
//            {
//                List<int> pseudo_priority_list = new List<int>(current_list.Count);
//
//                foreach (int value in current_list)
//                {
//                    int xxx = distance_list[value];
//                    pseudo_priority_list.Add(xxx);
//                }
//
//                int small = pseudo_priority_list.Min();
//                int index = pseudo_priority_list.IndexOf(small);
//                int current_vertex = current_list[index];
//
//                Node current_node = g.searchNode(current_vertex);
//                List<Edge> edge_list = new List<Edge>();
//                edge_list = current_node.GetEdgeList();
//
//                for (int z = 0; z < edge_list.Count; z++)
//                {
//                    int r = edge_list[z]._dest.node_id;
//
//                    if (!visited_vertices.Contains(r) && !current_list.Contains(r))
//                    {
//                        current_list.Add(r);
//
//                    }
//                    int edge1 = g.Edge_cost(start_node, current_vertex);
//                    if (edge1 == -1)
//                    {
//                        edge1 = distance_list[current_vertex];
//                    }
//                    if (edge1 > distance_list[current_vertex])
//                    {
//                        edge1 = distance_list[current_vertex];
//                    }
//                    int edge2 = g.Edge_cost(current_node, r);
//                    if (distance_list[r] > (edge1 + edge2))
//                    {
//                        shortest_vertices[r] = r;
//                        
//                        shortest_vertices[r] = current_vertex;
//                        distance_list[r] = edge1 + edge2;
//                    }
//                }
//                visited_vertices.Enqueue(current_vertex);
//
//                current_list.Remove(current_vertex);
//
//            }
//            for (int z = 0; z < g.GetTotalNodes(); z++)
//            {
//                Console.WriteLine("shortest cost to reach " + z + " from " + start_vertex + " -> " + distance_list[z]);
//            }
//            ReturnCostForSpecificPath(g, start_vertex, dest_vertex, distance_list, shortest_vertices);
//        }
//
//        public void ReturnAllPath(GraphImpl g, int start_vertex, int[] distance_list, int[] short_vertices)
//        {
//            int first_edge, second_edge;
//            for (int z = 0; z < g.GetTotalNodes(); z++)
//            {
//                if (short_vertices[z] == start_vertex)
//                {
//                    Console.WriteLine("Path for " + z + " = (" + start_vertex + "," + z + ")");
//                }
//                else
//                {
//                    first_edge = z;
//                    do
//                    {
//                        second_edge = short_vertices[first_edge];
//                        Console.WriteLine("Path for " + z + " = (" + first_edge + "," + second_edge + ")");
//
//                        first_edge = second_edge;
//                    } while (second_edge != start_vertex);
//
//                }
//            }
//
//        }
//
//        public void ReturnCostForSpecificPath(GraphImpl g, int start_vertex, int dest_vertex, int[] distance_list, int[] short_vertices)
//        {
//            int first_edge, second_edge;
//
//            Node source_node = g.searchNode(start_vertex);
//            for (int z = 0; z < g.GetTotalNodes(); z++)
//            {
//                if (z == dest_vertex)
//                {
//                    double total_cost = 0;
//                    double total_cost1 = 0;
//                    if (short_vertices[z] == start_vertex)
//                    {
//                        total_cost = g.Edge_cost(source_node, z);
//                        Console.WriteLine("Path for " + z + " = (" + start_vertex + "," + z + ")");
//                        Console.WriteLine("Total latency = " + total_cost);
//                    }
//                    else
//                    {
//
//                        first_edge = z;
//                        do
//                        {
//                            second_edge = short_vertices[first_edge];
//                            // The last param should be in KiloBytes
//                            total_cost1 += ReturnLatencyForGivenEdge(g, first_edge, second_edge, /* File size*/10);
//                            Console.WriteLine("Path for " + z + " = (" + first_edge + "," + second_edge + ")");
//
//                            first_edge = second_edge;
//                        } while (second_edge != start_vertex);
//                        Console.WriteLine("Total latency = " + total_cost1);
//                    }
//                }
//            }
//
//        }
//
//        public double ReturnLatencyForGivenEdge(GraphImpl gi, int first_edge, int second_edge, long file_size)
//        {
//            Node source_node = gi.searchNode(first_edge);
//            int cost = 0;
//            double latency = 0.0;
//
//            // FileSize - KB
//            // Bandwidth - KBps
//            // Cost - milliseconds
//            cost = gi.Edge_cost(source_node, second_edge);
//            latency = (file_size / cost) * Math.Pow(10, -3);
//            return latency;
//        }
//    }
//}
