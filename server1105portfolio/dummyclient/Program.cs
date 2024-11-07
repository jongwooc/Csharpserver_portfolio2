using servercore1105;
using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;

namespace dummyclient
{


    internal class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            //IPAddress ipAddress = ipHost.AddressList[0];
            //도커라 바로 옆에 있는 자기 자신의 아이피 대신 서버의 아이피를 하드코딩했다
            IPAddress ipAddress = IPAddress.Parse("172.17.0.3");
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 7890);


            Console.WriteLine($"클라이언트 시작.{endPoint} 로 접속 생성 중");

            Connector clientconnector = new Connector();
            clientconnector.Init(endPoint, () => { return new GameSessionforClient(); });



            while (true)
            {


                //서버와 클라이언트의 연결은 언제나 위험이 있기 때문에 try-catch로 대비한다.
                try
                {
                }
                catch (Exception occuredException)
                {
                    Console.WriteLine(occuredException.ToString());
                }
            }



        }
    }
}
