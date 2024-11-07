namespace servercore1105
{
    class Self_RWLOCK
    {
        const int EMPTY_FLAG = 0x00000000;
        const int WRITE_MASK = 0x7fff0000;
        const int READ_MASK = 0x0000ffff;
        const int MAX_COUNT = 1024;
        int _flag = EMPTY_FLAG;
        int _write_lock_count = 0;
        public void WriteLock()
        {
            //동일 스레드가 Writelock을 이미 가지고 있는지 확인
            int writelock_threadID = (_flag & WRITE_MASK) >> 16;
            if (Thread.CurrentThread.ManagedThreadId == writelock_threadID)
            {
                _write_lock_count++;
                return;
            }
            //Write/Read Lock의 소유주가 없을 때 경합권을 다툰다
            //비트 연산자로 Write ThreadID를 16비트를 이동시켜서 한 숫자 안에 두가지 락의 상태를 표시한다.
            //WRITE_MASK와 비트 연산을 통해서 존재하는지를 확인한다.
            //따라서 desired는 1이 아니라 쓰레드 아이디를 연산한 어떤 값이 된다.
            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;

            while (true)
            {
                for (int i = 0; i < MAX_COUNT; i++)
                { 
                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        _write_lock_count = 1;
                        return;
                    }
                }
                Thread.Yield();
            }

        }
        public void WriteUnlock()
        {
            int lockCount = --_write_lock_count;
            if (lockCount == 0)
                Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }
        public void ReadLock()
        {
            //동일 스레드가 writelock을 이미 가지고 있는지 확인
            int writelock_threadID = (_flag & WRITE_MASK) >> 16;
            if (Thread.CurrentThread.ManagedThreadId == writelock_threadID)
            {
                //플래그의 하위 비트가 리드락 상태니
                Interlocked.Increment(ref _flag);
                return;
            }
            while (true)
            {
                for (int i = 0; i < MAX_COUNT; i++)
                {
                    int expected = _flag & READ_MASK;
                    if (Interlocked.CompareExchange(ref _flag, expected +1, expected) == expected)
                    {
                        return;
                    }
                }
                Thread.Yield();
            }

        }
        public void ReadUnlock()
        {
            Interlocked.Decrement(ref _flag);
        }


    }

    class SpinLockTest
    {
        //static SpinLock S_Lock1 = new SpinLock();
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
                int lock_condition = 
                    Interlocked.CompareExchange(ref _locked,(int)lock_check.desired, (int)lock_check.expected);
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

    //커널단에 요청
    class AutoResetLockTest
    {
        // static Mutex Mu1 = New Mutex();
        AutoResetEvent _lock = new AutoResetEvent(true);
        public void Acquire()
        {
            _lock.WaitOne();
        }

        public void Release()
        {
            _lock.Set();
        }
    }


    internal class Locks
    {
        //C#의 볼리타일은 특이해서 가능하면 쓰지 않는 것이 좋다.
        //volatile static bool _thread_share = false;
        static int multiThreadingLearning = 0;
        static object monitorobj = new object();

        //RWLock
        static ReaderWriterLockSlim _RWLock = new ReaderWriterLockSlim();
        static int _ReadEvent(int id)
        {
            _RWLock.EnterReadLock();
            _RWLock.ExitReadLock();

            return 0;
        }
        static int _WriteEvent(int id)
        {
            _RWLock.EnterWriteLock();
            _RWLock.ExitWriteLock();

            return 0;
        }
        //monitor
        static void Thread_lock(object? state)
        {
            while (true)
            {
                //=lock() = try,finally, monitor를 내부적으로 구현
                lock (monitorobj)
                {
                    Console.WriteLine("Thread lock Test");
                    //multiThreadingLearning--;
                }
            }
        }

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
    }

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
}
