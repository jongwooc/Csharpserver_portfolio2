using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace servercore1105
{
    class Program
    {
        /*
        스레드 로컬 스토리지 테스트 주석처리 
        static ThreadLocal<string> Threadname = new ThreadLocal<string>();

        static void ThreadLocal_Learning()
        {

            Threadname.Value = $"Thread Learning test. Thread value is {Thread.CurrentThread.ManagedThreadId}";
            Thread.Sleep(1000);
            Console.WriteLine(Threadname.Value);
        }
        */

        static void Main(string[] args)
        {
            //스레드 연습 한 것은 주석처리
            /*
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(5, 5);
            static Mutex _MutexLock = new Mutex;
            _MutexLock.WaitOne();
            _MutexLock.ReleaseMutex();
            Thread test001 = new Thread(MainThread);
            test001.IsBackground = true;
            test001.Start();
            test001.Join();
            ThreadPool.QueueUserWorkItem(MainThread);
            Task testtask001 = new Task(() => { while (true); { } },TaskCreationOptions.LongRunning);
            Console.WriteLine("Thread test end");
            Parallel.Invoke(ThreadLocal_Learning, ThreadLocal_Learning, ThreadLocal_Learning, ThreadLocal_Learning);
            */
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddress = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 7890);

            // listener소켓 생성
            Socket listenerSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //서버와 클라이언트의 연결은 언제나 위험이 있기 때문에 try-catch로 대비한다.
            try
            {
                //소켓 고정
                listenerSocket.Bind(endPoint);
                //서버 대기 시작 +최대 대기수
                listenerSocket.Listen(10);
                //서버의 무한 대기
                while (true)
                {
                    Console.WriteLine("listen test");
                    //클라 소켓을 리스너랑 연결
                    Socket clientSocket = listenerSocket.Accept();
                    //연결된 소켓에서 데이터를 받는다
                    byte[] recievedBuffer = new byte[1024];
                    int recievedBytes = clientSocket.Receive(recievedBuffer);
                    string recievedData = Encoding.UTF8.GetString(recievedBuffer, 0, recievedBytes);
                    Console.WriteLine($"received {recievedData}");
                    //데이터를 보내본다
                    byte[] sendBuffer = Encoding.UTF8.GetBytes("서버오픈 데이터 전송 테스트");
                    Console.WriteLine("클라이언트에게 테스트 데이터 전송");
                    clientSocket.Send(sendBuffer);
                    //연결종료 직전에 리슨과 센드를 모두 종료한다 
                    clientSocket.Shutdown(SocketShutdown.Both);
                    //리슨과 샌드를 모두 종료후 실제 연결을 종료한다
                    clientSocket.Close();
                }
            }
            catch (Exception occuredException)
            {
                Console.WriteLine(occuredException.ToString());
            }
        }
    }
}