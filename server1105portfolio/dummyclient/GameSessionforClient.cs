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


            Packet test = new PlayerInfoReq
            {
                _PlayerID = 5555,
                _PlayerName = "테스트알파"
            };

            Console.WriteLine($"클라이언트 테스트 플레이어 아이디{((PlayerInfoReq)test)._PlayerID}");

            Console.WriteLine($"클라이언트 테스트 플레이어 이름{((PlayerInfoReq)test)._PlayerName}");

            test.Init();

            byte[] test222 = test.SerializeAll();

            //ArraySegment<byte> openSegment = SendBufferHelper.Open(576);
            /*
                            byte[] sendBuffer1 = BitConverter.GetBytes(packet.size);
                            byte[] sendBuffer2 = BitConverter.GetBytes(packet.packetID);
                            Array.Copy(sendBuffer1, 0, openSegment.Array, openSegment.Offset, sendBuffer1.Length);
                            Array.Copy(sendBuffer2, 0, openSegment.Array, openSegment.Offset + sendBuffer1.Length, sendBuffer2.Length);
                            ArraySegment<byte> completedBuffer = SendBufferHelper.Close(sendBuffer1.Length + sendBuffer2.Length);
            */


            //Send(completedBuffer);

            ArraySegment<byte> openSegment = test.SerializeAll();
            Send(openSegment);
        }
    }
}
