using System;
using System.Net;
using System.Net.Sockets;

namespace RealTimePPDisplayer.Displayer
{
	class SocketDisplayer : DisplayerBase
	{
        private Socket sock;
        private IPEndPoint destination;
		public SocketDisplayer(string ipAddr, int port)
		{
            try
            {
                IPAddress ipAddress = IPAddress.Parse(ipAddr);
                destination = new IPEndPoint(ipAddress, port);
                sock = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(destination);

            } catch (Exception e)
            {
                //Console.WriteLine(e.Message);
            }
		}
        private void CheckSocket()
        {
            if(!sock.Connected)
            {
                sock.Connect(destination);
            }
        }

		public override void Clear()
		{
            CheckSocket();
            byte[] bytes = BitConverter.GetBytes(0f);
            sock.Send(bytes);
		}

		public override void OnUpdateHitCount(HitCountTuple tuple)
		{
		}

		public override void OnUpdatePP(PPTuple tuple)
		{
            CheckSocket();
            float[] data = { (float)tuple.MaxPP, (float)tuple.FullComboPP, (float)tuple.RealTimePP };
            byte[] bytes = new byte[data.Length * 4];
            Buffer.BlockCopy(data, 0, bytes, 0, bytes.Length);
            sock.Send(bytes);
		}
	}
}
