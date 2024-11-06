using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace servercore1105
{
    internal class Session
    {

        #region 이니셜라이즈

        Socket _sessionSocket;
        volatile int _disconnectedCondition = 0;

        Queue<byte[]> _sendingQueue = new Queue<byte[]>();
        bool _pending = false;
        SocketAsyncEventArgs _sendingArgs = new SocketAsyncEventArgs();
        object _customlock = new object();

        public void Init(Socket incomingSocket)
        {
            _sessionSocket = incomingSocket;
            SocketAsyncEventArgs receivedArgs = new SocketAsyncEventArgs();
            receivedArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ReceiveCompleted);
            //추가로 뭔가를 하고 싶을 때
            //receivedArgs.UserToken = null;
            receivedArgs.SetBuffer(new byte[1024], 0, 1024);

            _sendingArgs.Completed += new EventHandler<SocketAsyncEventArgs>(SendCompleted);

            RegisterReceive(receivedArgs);
        }

        #endregion


        #region 리시브 네트워크 통신
        void RegisterReceive(SocketAsyncEventArgs _receivedArgs)
        {
            bool pending = _sessionSocket.ReceiveAsync(_receivedArgs);
            if (pending == false)
            {
                ReceiveCompleted(null, _receivedArgs);
            }

        }

        void ReceiveCompleted(object sender, SocketAsyncEventArgs _receivedArgs)
        {
            if (_receivedArgs.BytesTransferred > 0 && _receivedArgs.SocketError == SocketError.Success)
            {
                //추가 작업 필요
                try
                {
                    //리시브시 할일
                    string recievedData = Encoding.UTF8.GetString(_receivedArgs.Buffer, _receivedArgs.Offset, _receivedArgs.BytesTransferred);
                    Console.WriteLine($"received {recievedData}");
                    RegisterReceive(_receivedArgs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"recieve completed failed with error {ex}");
                }
            }
            else
            {
                //접속 종료. 전송된 바이트가 0이라는 것은 접속 종료의 의미
                Disconnect();
            }
        }

        #endregion




        public void Send(byte[] sendbuff)
        {
            lock (_customlock)
            {
                _sendingQueue.Enqueue(sendbuff);
                if (_pending == false)
                {
                    RegisterSend();
                }
            }
        }

        void RegisterSend()
        {
            _pending = true;
            byte[] sendbuff =_sendingQueue.Dequeue();
            _sendingArgs.SetBuffer(sendbuff,0,sendbuff.Length);

            bool pending = _sessionSocket.SendAsync(_sendingArgs);
            if (pending == false)
            {
                SendCompleted(null, _sendingArgs);
            }

        }

        void SendCompleted(object sender, SocketAsyncEventArgs _sendingArgs)
        {
            lock (_customlock)
            {
                if (_sendingArgs.BytesTransferred > 0 && _sendingArgs.SocketError == SocketError.Success)
                {
                    try
                    {
                        if (_sendingQueue.Count > 0)
                        {
                            RegisterSend();
                        }
                        _pending = false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"send completed failed with error {ex}");
                    }
                }
                else
                {
                    Disconnect();
                }


            }

        }




        public void Disconnect()
        {
            if ( Interlocked.Exchange(ref _disconnectedCondition,1) == 1 )
            {
                return;
            }
            //연결종료 직전에 리슨과 센드를 모두 종료한다 
            _sessionSocket.Shutdown(SocketShutdown.Both);
            //리슨과 샌드를 모두 종료후 실제 연결을 종료한다
            _sessionSocket.Close();
        }
    }
}
