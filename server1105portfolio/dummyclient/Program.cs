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

            //클라 소켓 설정
            Socket clientSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);


            Thread.Sleep(1000);


            //서버와 클라이언트의 연결은 언제나 위험이 있기 때문에 try-catch로 대비한다.
            try
            {
                clientSocket.Connect(endPoint);
                Console.WriteLine($"클라이언트가 {clientSocket.RemoteEndPoint.ToString()}서버에 엔드포인트에 연결 시도 중");

                //클라에서 서버로.
                byte[] sendBuffer = Encoding.UTF8.GetBytes("클라에서 서버로 테스트 메세지 전송");
                int sendBufferBytes = clientSocket.Send(sendBuffer);

                //서버에서 클라로.
                byte[] recievedBuffer = new byte[1024];
                int recievedBytes = clientSocket.Receive(recievedBuffer);
                string recievedData = Encoding.UTF8.GetString(recievedBuffer, 0, recievedBytes);
                Console.WriteLine($"서버에서 클라로 received {recievedData} 회신 중");
                //연결종료 직전에 리슨과 센드를 모두 종료한다 
                clientSocket.Shutdown(SocketShutdown.Both);
                //리슨과 샌드를 모두 종료후 실제 연결을 종료한다
                clientSocket.Close();
            }
            catch (Exception occuredException)
            {
                Console.WriteLine(occuredException.ToString());
            }

        }
    }
}
