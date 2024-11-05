using System;
using System.Threading;
using System.Threading.Tasks;

namespace servercore1105
{
    class SpinLockTest
    {
        private enum lock_check
        {
            expected,
            desired
        }

        volatile int _locked = 0;

        public void Acquire()
        {
            while (true)
            {
                int lock_condition = Interlocked.Exchange(ref _locked, 1);
                if (lock_condition == 0)
                {
                    break;
                }
            }
        }

        public void CompareAcquire()
        {
            while (true)
            {
                int lock_condition = Interlocked.CompareExchange(ref _locked, (int)lock_check.desired, (int)lock_check.expected);
                if (lock_condition == 0)
                {
                    break;
                }
            }
        }
        public void Release()
        {
            _locked = 0;
        }

    }

    class SwitchLockTest
    {

        volatile int _locked = 0;
        public void CompareAcquire()
        {
            while (true)
            {
                if (Interlocked.CompareExchange(ref _locked, 1, 0) == 0)
                {
                    break;
                }
                Thread.Sleep(1);    //무조건 컨텍스트 스위치
                //Thread.Sleep(0); 우선순위가 같거나 높을 때만 컨텍스트 스위치
                //Thread.Yield(); 우선순위와 무관하게 컨텍스트 스위치

            }
        }
        public void Release()
        {
            _locked = 0;
        }
    }
    class AutoResetLockTest
    {
        AutoResetEvent _lock = new AutoResetEvent(true);
        public void Acquire()
        {

        }

        public int Release()
        {
        }
    }
        class Program
    {
        //C#의 볼리타일은 특이해서 가능하면 쓰지 않는 것이 좋다.
        //volatile static bool _thread_share = false;
        static int multiThreadingLearning = 0;
        static object monitorobj = new object();

        static void MainThread(object? state)
        {
            while (true)
            {
                Console.WriteLine("Thread Test");


                //메모리 배리어
                //코드 재배치 억제 및 가시성 증가
                //------------------------
                //연습 결과 함수 내에서만 동작가능
                //Full memory barrier  = Assembly MFENCE, C# Thread.MemoryBarrier()
                //Store memory barrier = Assembly SFENCE
                //Load memory barrier  = Assembly LFENCE
                Thread.MemoryBarrier();
                //openCL의 flush()랑 유사한듯
                Thread.Sleep(1000);

                //interlocked는 원자성 보장 하지만 성능 저하
                Interlocked.Increment(ref multiThreadingLearning);
            }
        }

        static void Thread2(object? state)
        {
            while (true)
            {
                Console.WriteLine("Thread 2 Test");



            }
        }
        static void Thread_Monitor(object? state)
        {
            while (true)
            {
                //=CriticalSection, std::mutex();
                //try, (catch), finally를 쓰면 데드락 방지 가능, 
                try
                {
                    Monitor.Enter(monitorobj);
                    {
                        Console.WriteLine("Thread Monitor Test");
                        multiThreadingLearning--;
                    }
                }
                finally
                {
                    Monitor.Exit(monitorobj);
                }
            }
        }
        static void Thread_lock(object? state)
        {
            while (true)
            {
                //=lock() = try,finally, monitor를 내부적으로 구현
                lock (monitorobj)
                {
                    Console.WriteLine("Thread lock Test");
                    multiThreadingLearning--;
                }
            }
        }



        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(5, 5);

            Thread test001 = new Thread(MainThread);
            //test001.IsBackground = true;
            //test001.Start();
            //test001.Join();
            //ThreadPool.QueueUserWorkItem(MainThread);
            //Task testtask001 = new Task(() => { while (true); { } },TaskCreationOptions.LongRunning);


            Console.WriteLine("Thread test end");

        }
    }
}