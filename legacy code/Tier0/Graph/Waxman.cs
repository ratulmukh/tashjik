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
//
//namespace Graph
//{
//    class Waxman
//    {
//        const int MAX_X = 200;
//        const int MAX_Y = 200;
//        public int GetRandomX(int seed)
//        {
//            // Use dt.Millisecond as the seed in the real system.
//            // Fr testing purpose just use a known seed.
//
//            DateTime dt = DateTime.Now;
//            //Random rand = new Random(seed + dt.Millisecond);
//            Random rand = new Random(seed);
//            int r = rand.Next(MAX_X);
//            return r;
//
//        }
//
//        public int GetRandomY(int seed)
//        {
//            // Use dt.Millisecond as the seed in the real system.
//            // Fr testing purpose just use a known seed.
//
//            DateTime dt = DateTime.Now;
//            //Random rand = new Random(seed + dt.Millisecond);
//            Random rand = new Random(seed);
//            int r = rand.Next(MAX_Y);
//
//            // this is to get a new random number as the seed will be the same when GetRandomX() and GetRandomY()
//            // are called successively
//            r = rand.Next(MAX_Y);
//            return r;
//
//        }
//
//        public double ProbFunc(Node src, Node dest)
//        {
//            double distance, L, alpha, beta;
//            alpha = 0.15;
//            beta = 0.2;
//            int x1, x2, y1, y2, dx, dy;
//            x1 = src.Xpos; x2 = dest.Xpos;
//            y1 = src.Ypos; y2 = dest.Ypos;
//
//            dx = x2 - x1;
//            dy = y2 - y1;
//
//            distance = Math.Sqrt(dx * dx + dy * dy);
//            //Console.WriteLine("Distance between " + src.node_id + " and " + dest.node_id + " = " + distance);
//            L = Math.Sqrt(2) * MAX_X;
//
//            /*Console.WriteLine("distance = " + distance);
//            Console.WriteLine("L = " + L);*/
//            //Console.WriteLine("final value = " + (alpha * Math.Exp(-1.0 * (distance / (beta * L)))));
//
//            return 10 * alpha * Math.Exp(-1.0 * (distance / (beta * L)));
//        }
//
//    }
//}
