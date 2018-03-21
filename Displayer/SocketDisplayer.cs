using System;
using System.Net;
using System.Net.Sockets;

namespace RealTimePPDisplayer.Displayer
{
	class SocketDisplayer : DisplayerBase
	{
        private Socket sock;
		public SocketDisplayer(string ipAddr, int port)
		{
            try
            {
                IPAddress ipAddress = IPAddress.Parse(ipAddr);
                IPEndPoint destination = new IPEndPoint(ipAddress, port);
                sock = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(destination);

            } catch (Exception e)
            {
                //Console.WriteLine(e.Message);
            }
		}

		public override void Clear()
		{
		}

		public override void OnUpdateHitCount(HitCountTuple tuple)
		{
		}

		public override void OnUpdatePP(PPTuple tuple)
		{
            byte[] bytes = BitConverter.GetBytes((float)(tuple.RealTimePP));
            sock.Send(bytes);
		}
	}
}
