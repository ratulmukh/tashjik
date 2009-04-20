/*
 * Created by SharpDevelop.
 * User: ratul
 * Date: 4/20/2009
 * Time: 7:12 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Net;
using System.Net.Sockets;

namespace Tashjik
{
	/// <summary>
	/// Description of PseudoTashjikFactory.
	/// </summary>
	internal class TashjikFactory
	{
		internal TashjikFactory()
		{
		}
		
		internal Server createServer(OverlayTypeEnum overlayType)
		{
			return null;
		}
		
		internal Server createServer(OverlayTypeEnum overlayType, IPAddress joinOtherIP, Guid joinOtherGuid, ProxyController proxyController)
		{
			return null;
		}
		
		internal NodeProxy createNodeProxy(OverlayTypeEnum overlayType, IPAddress IP, ProxyController ProxyController)
		{
			return null;
		}
	}
	
}
