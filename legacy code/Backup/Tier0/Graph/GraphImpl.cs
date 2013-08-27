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
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Collections;
//
//namespace Graph
//{
//    enum Color { WHITE, GRAY, BLACK }
//    class GraphImpl
//    {
//        private List<Node> node_list;
//        private int Num_nodes;
//        private ArrayList vertices_list;
//
//        public int GetTotalNodes()
//        {
//            return Num_nodes;
//        }
//
//        public void addNode(int id, int num_nodes)
//        {
//
//            Node n = new Node(id, num_nodes);
//
//            node_list.Add(n);
//        }
//
//
//        public GraphImpl(int num_nodes)
//        {
//            Num_nodes = num_nodes;
//            node_list = new List<Node>(num_nodes);
//            vertices_list = new ArrayList();
//        }
//
//        public Node searchNode(int id)
//        {
//            bool found = false;
//            int count = 0;
//            for (int z = 0; z < node_list.Count; z++)
//            {
//                if (node_list[z].node_id == id)
//                {
//                    found = true;
//                    break;
//                }
//                else
//                    count++;
//            }
//            if (!found)
//                return null;
//            else
//                return node_list[count];
//        }
//
//        public void CreateNodes(int num_nodes)
//        {
//            for (int z = 0; z < num_nodes; z++)
//            {
//                vertices_list.Add(z);
//                Node n = new Node(z, num_nodes);
//                node_list.Add(n);
//            }
//        }
//
//        public void CreateEdges(int num_edges)
//        {
//            Console.WriteLine("================================================================");
//           
//            Console.WriteLine("Creating " + num_edges + " edges");
//            const int RANDOM_SEED = 55555;
//            
//            DateTime dt = DateTime.Now;
//            Node from = new Node();
//            Node to = new Node();
//             for (int z = 0; z < (5 * num_edges); z++)
//            {
//                // Get two random nodes from node_list
//                Random rand = new Random(RANDOM_SEED + z);
//                int r = rand.Next(node_list.Count);
//                //Console.WriteLine("First r = " + r);
//                from = searchNode(r);
//                // get one more random number
//                r = rand.Next(node_list.Count);
//                //Console.WriteLine("second r = " + r);
//                to = searchNode(r);
//
//                // dont allow self loops as of now
//                if (from.node_id == to.node_id)
//                {
//                    r = rand.Next(node_list.Count);
//                    to = searchNode(r);
//                }
//                // avoid creating duplpicate edges between nodes
//                if(isEdge(from.node_id, to.node_id))
//                {
//                    Console.WriteLine("Avoiding duplicate edges");
//                    r = rand.Next(node_list.Count);
//                    to = searchNode(r);   
//                }
//
//                //now we have 2 random nodes. see if we can hook up these two
//                Waxman w = new Waxman();
//                double value = 0.0;
//                double compare = 0.0;
//                compare = rand.NextDouble();
//                value = w.ProbFunc(from, to);
//                //Console.WriteLine("Compare = " + compare + " value = " + value);
//
//                Random rndm = new Random();
//                if (value < compare)
//                {
//                    
//                    int rndm_num = rand.Next(1, 100);
//                
//                    Console.WriteLine("Connecting edge from " + to.node_id + " to " + from.node_id + " with cost = " + rndm_num);
//                    from.addAdjNode(to, rndm_num);
//                    to.addAdjNode(from, rndm_num);
//                }
//
//            }
//
//        }
//
//        public bool isEdge(int src_node_id, int dest_node_id)
//        {
//            Node src = searchNode(src_node_id);
//            bool is_edge = false;
//            for (int z = 0; z < src.edge_list.Count; z++)
//            {
//                if (src.edge_list[z]._dest.node_id == dest_node_id)
//                {
//                    is_edge = true;
//                    return is_edge;
//                }
//            }
//            return is_edge;
//        }
//
//        public bool isGraphConnected()
//        {
//            List<Color> color_list = new List<Color>(Num_nodes);
//            bool is_connected = true;
//            for (int z = 0; z < color_list.Capacity; z++)
//            {
//                color_list.Insert(z, Color.WHITE);
//            }
//            int[] visited_array = new int[Num_nodes];
//            //DFS(ref color_list, visited_array, 0);
//            DFS(ref color_list, 0);
//            for (int z = 0; z < color_list.Count; z++)
//            {
//                if (color_list[z] == Color.WHITE)
//                    return false;
//            }
//            return is_connected;
//        }
//
//        /*public void DFS(ref List<Color> color_list, int[] visited_array, int u)
//        {
//            int v;
//          
//            color_list[u] = Color.GRAY;
//            for (int z = 0; z < node_list.Count; z++)
//            {
//                v = node_list[z].node_id;
//                if (color_list[v] == Color.WHITE)
//                {
//
//                    visited_array[v] = u;
//                    DFS(ref color_list, visited_array, v);
//                }
//            }
//            
//            color_list[u] = Color.BLACK;
//            
//        }*/
//
//        public void DFS(ref List<Color> color_list, int u)
//        {
//            color_list[u] = Color.GRAY;
//            for (int v = 0; v < node_list.Count; v++)
//            {
//                if (isEdge(u, v) && color_list[v] == Color.WHITE)
//                {
//                    Console.WriteLine("Order = " + v);
//                    DFS(ref color_list, v);
//                }
//            }
//            color_list[u] = Color.BLACK;
//        }
//
//        public int Edge_cost(Node source, int dest_vertex)
//        {
//            Node dest_node = searchNode(dest_vertex);
//            int cost = -1;
//            if (source.node_id == dest_vertex)
//                return 0;
//            else
//            {
//                for (int z = 0; z < source.edge_list.Count; z++)
//                {
//                    if (source.edge_list[z]._dest.node_id == dest_vertex)
//                    {
//                        cost = source.edge_list[z]._cost;
//                        break;
//                    }
//                }
//            }
//            return cost;
//        }
//
//        public void display_connections()
//        {
//            Console.WriteLine("================================================================");
//            foreach (Node n in node_list)
//            {
//                for (int z = 0; z < n.edge_list.Count; z++)
//                {
//                    
//                    Console.WriteLine("Node " + n.edge_list[z]._src.node_id + " with co-ordinate "
//                        + n.edge_list[z]._src.Xpos + "," + n.edge_list[z]._src.Ypos + " is connected to Node "
//                        + n.edge_list[z]._dest.node_id + " with co-ordinate "
//                        + n.edge_list[z]._dest.Xpos + "," + n.edge_list[z]._dest.Ypos);
//                }
//            }
//        }
//    }
//
//    class Node
//    {
//        public int node_id;
//        public List<Edge> edge_list;
//        public int Xpos;
//        public int Ypos;
//
//        public Node()
//        {
//            edge_list = new List<Edge>();
//
//        }
//        public Node(int id, int num_nodes)
//        {
//            Waxman w = new Waxman();
//
//            this.Xpos = w.GetRandomX(id);
//            this.Ypos = w.GetRandomY(id);
//            
//            edge_list = new List<Edge>(num_nodes);
//            node_id = id;
//            Console.WriteLine("Creating Node " + this.node_id + " at (" + this.Xpos + "," + this.Ypos + ")");
//        }
//
//        public void addAdjNode(Node adj, int cost)
//        {
//            Edge edge = new Edge(this, adj, cost);
//
//            this.edge_list.Add(edge);
//        }
//
//        public List<Edge> GetEdgeList()
//        {
//            return edge_list;
//        }
//    }
//
//    class Edge
//    {
//        public Edge(Node src, Node dest, int cost)
//        {
//            _src = src;
//            _dest = dest;
//            _cost = cost;
//        }
//
//        public Node _src;
//        public Node _dest;
//        public int _cost;
//    }
//
//}
