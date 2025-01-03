using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace servercore1105
{
    public class PacketHandler
    {
        public static void PlayerInfoReqHandler(PacketSession session, byte[] packet)
        {
            ushort _offset = 0;
            ushort _packetSize = BitConverter.ToUInt16(packet, 0);
            ushort _packetID = BitConverter.ToUInt16(packet, 4);
            Console.WriteLine($"packet size is {_packetSize}, packet ID is {_packetID}");
            byte[] SplitedPacket = new ArraySegment<byte>(packet, 0, _packetSize).ToArray();
            _offset += _packetSize;



        }
    }
}
