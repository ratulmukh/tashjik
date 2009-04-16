/*
 * Created by SharpDevelop.
 * User: ratul
 * Date: 4/12/2009
 * Time: 6:41 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using Tashjik.Tier2.Streaming;
//using MbUnit.Framework;

namespace TashjikTest.Tier2Test.StreamingTest.NaradaTest
{

	public class NaradaBootStrapperTest
	{
		public NaradaBootStrapperTest()
		{
		}
		
		public void testInit()
		{
			NaradaBootstrapper naradaBootstrapper = new NaradaBootstrapper();
			naradaBootstrapper.init();
			
		}
	}
}
