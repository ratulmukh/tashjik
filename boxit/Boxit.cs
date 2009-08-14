/*
 * Created by SharpDevelop.
 * User: ratul
 * Date: 8/8/2009
 * Time: 6:58 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public static class Boxit
{
	/// 
	/// Example of creating an external process and killing it
	/// 
	public static void Main() 
	{
		init();
/*
    Process process;
    while(true)
    {
    	process = Process.Start("TashjikClient.exe");
       	//Thread.Sleep(15000);
    }
    	try {
    	process.BeginE
  //  		process.Kill();
    	} catch {}
*/    	
   }
	
	struct Pair
	{
		public Process process;
		public String portNo;
		public Pair(Process process, String portNo)
		{
			this.portNo = portNo;
			this.process = process;
		}
		
	}
		
	static Dictionary<String, Pair> registry = new Dictionary<String, Pair>();
	
	static void init()
	{
		Process process;
		int portNo = 2336;
		for(int i=0; i<2; i++)
		{
			Random rnd = new Random();
			
			int add1 = rnd.Next(0, 255);		
			int add2 = rnd.Next(0, 255);
			int add3 = rnd.Next(0, 255);
			int add4 = rnd.Next(0, 255);
			
			byte[] byteIP = {(byte)add1, (byte)add2, (byte)add3, (byte)add4};
			
			Console.WriteLine("Hi there");
			Console.Write((int)byteIP[0]);
			Console.Write((int)byteIP[1]);
			Console.Write((int)byteIP[2]);
			Console.WriteLine((int)byteIP[3]);
				
			
			IPAddress IP = new IPAddress(byteIP);
			String strIP = Encoding.ASCII.GetString(byteIP);
			Console.WriteLine(strIP);
			
			process = Process.Start("TashjikClient.exe", strIP + " " + Convert.ToString(portNo));
			registry.Add(strIP, new Pair(process, Convert.ToString(portNo)));

			portNo++;
		}
	}
}
