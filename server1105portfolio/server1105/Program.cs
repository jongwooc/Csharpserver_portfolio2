using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Collections.Specialized.BitVector32;
using servercore1105;

namespace server1105
{
    class Program
    {

        static Listener _listener = new Listener();

        static void Main(string[] args)
        {

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddress = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 7890);


            //리스너 생성 및 설정
            _listener.Init(endPoint, () => { return new GameSessionforServer(); });
            Console.WriteLine("listening 대기 중");

            while (true)
            {
                ;
            }

        }
    }

}
