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
        static Listener _listener = new Listener();

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

            //서버와 클라이언트의 연결은 언제나 위험이 있기 때문에 try-catch로 대비한다.

            //리스너 생성 및 설정
            _listener.Init(endPoint, () => { return new GameSessionforServer(); });
            Console.WriteLine("listening 대기 중");

            //서버의 무한 대기
            while (true)
            {
                ;
            }

        }
    }

}
