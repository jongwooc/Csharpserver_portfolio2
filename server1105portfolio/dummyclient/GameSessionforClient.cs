using servercore1105;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace dummyclient
{

    internal class GameSessionforClient : GameSession
    {
        public override void OnConnected(EndPoint clientEndPoint)
        {
            Console.WriteLine($"클라이언트 GameSession OnConnected from = {clientEndPoint}");



            Packet packet = new Packet() { size = 27, packetID = 72 };

            //여러번 보낸다.
            for (int i = 0; i < 5; i++)
            {
                ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

                byte[] sendBuffer1 = BitConverter.GetBytes(packet.size);
                byte[] sendBuffer2 = BitConverter.GetBytes(packet.packetID);
                Array.Copy(sendBuffer1, 0, openSegment.Array, openSegment.Offset, sendBuffer1.Length);
                Array.Copy(sendBuffer2, 0, openSegment.Array, openSegment.Offset + sendBuffer1.Length, sendBuffer2.Length);
                ArraySegment<byte> completedBuffer = SendBufferHelper.Close(sendBuffer1.Length + sendBuffer2.Length);


                Send(completedBuffer);
            }
        }
    }
}
