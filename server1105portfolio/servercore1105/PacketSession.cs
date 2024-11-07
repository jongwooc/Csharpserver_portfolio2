using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace servercore1105
{
    public abstract class PacketSession : parent_Session
    {
        public static readonly int HeaderSize = 2;
        public sealed override int OnReceived(ArraySegment<byte> receivedBufferArraySegment)
        {
            int processLength = 0;

            while(true)
            {
                if (receivedBufferArraySegment.Count < 0 )
                    {
                        break;
                    }
                ushort datasize = BitConverter.ToUInt16(receivedBufferArraySegment.Array, receivedBufferArraySegment.Offset);
                if (receivedBufferArraySegment.Count < datasize )
                    {
                        break;
                    }
                OnPacketReceived(new ArraySegment<byte>(receivedBufferArraySegment.Array, receivedBufferArraySegment.Offset, datasize));

                processLength += datasize;
                receivedBufferArraySegment = new ArraySegment<byte>(receivedBufferArraySegment.Array, receivedBufferArraySegment.Offset + datasize, receivedBufferArraySegment.Count - datasize);

            }

            return processLength;
        }

        public abstract void OnPacketReceived(ArraySegment<byte> packetBufferArraySegment);



    }
}
