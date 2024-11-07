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
            Console.WriteLine($"GameSession OnConnected from = {clientEndPoint}");

            //여러번 보낸다.
            for (int i = 0; i < 5; i++)
            {
                byte[] sendBuffer = Encoding.UTF8.GetBytes("클라에서 서버로 테스트 메세지 전송");
                Send(sendBuffer);
            }
        }

        public override int OnReceived(ArraySegment<byte> receivedBufferArraySegment)
        {
            string recievedData = Encoding.UTF8.GetString(receivedBufferArraySegment.Array, receivedBufferArraySegment.Offset, receivedBufferArraySegment.Count);
            Console.WriteLine($"클라이언트가 서버로부터 {recievedData} 를 받았습니다.");
            return receivedBufferArraySegment.Count;
        }
    }
}
