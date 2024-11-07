using servercore1105;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace server1105
{
    internal class GameSessionforServer : GameSession
    {
        public override void OnConnected(EndPoint clientEndPoint)
        {
            Console.WriteLine($"GameSession OnConnected from = {clientEndPoint}");

            Knight knght1 = new Knight() { name = "knight1",hp = 100, attack = 10 };

            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);

            byte[] sendBuffer1 = BitConverter.GetBytes(knght1.hp);
            byte[] sendBuffer2 = BitConverter.GetBytes(knght1.attack);
            Array.Copy(sendBuffer1, 0, openSegment.Array, openSegment.Offset, sendBuffer1.Length);
            Array.Copy(sendBuffer2, 0, openSegment.Array, openSegment.Offset + sendBuffer1.Length, sendBuffer2.Length);
            ArraySegment<byte> completedBuffer = SendBufferHelper.Close(sendBuffer1.Length + sendBuffer2.Length);


            Send(completedBuffer);
            Thread.Sleep(1000);
            Disconnect();


        }
    }
}
