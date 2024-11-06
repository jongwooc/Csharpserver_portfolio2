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
            byte[] sendBuffer = Encoding.UTF8.GetBytes("서버오픈 데이터 전송 테스트");

            Send(sendBuffer);
            Thread.Sleep(1000);
            Disconnect();
        }
    }
}
