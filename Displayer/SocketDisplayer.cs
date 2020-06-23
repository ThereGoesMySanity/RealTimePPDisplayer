using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace RealTimePPDisplayer.Displayer
{
	class SocketDisplayer : DisplayerBase
	{
        private Socket sock;
        private IPEndPoint destination;
		public SocketDisplayer(int? id, string ipAddr, int port) : base(id)
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

        private struct DataPacket
        {
            public float maxPP, fcPP, rtPP;
            public int hit300, hit100, hit50, miss;
            public int combo;
        }
        public override void Display()
        {
            base.Display();
            CheckSocket();
            DataPacket packet = new DataPacket()
            {
                maxPP = (float)Pp.MaxPP,
                fcPP = (float)Pp.FullComboPP,
                rtPP = (float)Pp.RealTimePP,
                hit300 = HitCount.Count300,
                hit100 = HitCount.Count100,
                hit50 = HitCount.Count50,
                miss = HitCount.CountMiss,
                combo = HitCount.Combo,
            };
            int size = Marshal.SizeOf(packet);
            byte[] bytes = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(packet, ptr, true);
            Marshal.Copy(ptr, bytes, 0, size);
            Marshal.FreeHGlobal(ptr);
            sock.Send(bytes);
		}
	}
}
