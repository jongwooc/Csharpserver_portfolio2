using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace servercore1105
{
    public abstract class parent_Session
    {

        #region 이니셜라이즈

        protected Socket _sessionSocket;
        protected int _disconnectedCondition = 0;
        protected bool _pending = false;

        protected Queue<byte[]> _sendingQueue = new Queue<byte[]>();
        protected List<ArraySegment<byte>> _pendingBufferList = new List<ArraySegment<byte>>();

        protected SocketAsyncEventArgs _sendingArgs = new SocketAsyncEventArgs();
        protected SocketAsyncEventArgs _receivedArgs = new SocketAsyncEventArgs();

        protected ReceiveBuffer _receiveBuffer = new ReceiveBuffer(); 


        //세션을 상속받거나 외부에서 사용할 인터페이스 추가
        public abstract void OnConnected(EndPoint clientEndPoint);
        public abstract int  OnReceived(ArraySegment<byte> receivedBufferArraySegment);
        public abstract void OnSending(int sendingBytesTransferredInt);
        public abstract void OnDisconnected(EndPoint clientEndPoint);



        public void Init(Socket incomingSocket)
        {
            _sessionSocket = incomingSocket;

            _receivedArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ReceiveCompleted);

            _sendingArgs.Completed += new EventHandler<SocketAsyncEventArgs>(SendCompleted);

            RegisterReceive();
        }

        #endregion


        #region 리시브 네트워크 통신
        void RegisterReceive()
        {
            _receiveBuffer.Clean();
            ArraySegment<byte> segmentforReceive = _receiveBuffer.WritableDataSegment;
            _receivedArgs.SetBuffer(segmentforReceive.Array,segmentforReceive.Offset,segmentforReceive.Count);

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
                try
                {
                    //writeoffset이동
                    if(_receiveBuffer.OnWrite(_receivedArgs.BytesTransferred)== false)
                    {
                        Disconnect();
                        return;
                    }



                    //컨텐츠 코드에게 데이터를 넘겨주고 얼마나 처리 했는지 확인
                    int processingLength = OnReceived(_receiveBuffer.ReadableDataSegment);
                    if (processingLength < 0 || _receiveBuffer.DataSize < processingLength)
                    {
                        Disconnect();
                        return;
                    }

                    if (_receiveBuffer.OnRead(processingLength) == false)
                    {
                        Disconnect();
                        return;
                    }


                    RegisterReceive();
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
            _sendingQueue.Enqueue(sendbuff);
            if (_pendingBufferList.Count == 0)
            {
                RegisterSend();
            }
        }



        #region 샌드 네트워크 통신

        void RegisterSend()
        {
            while (_sendingQueue.Count > 0)
            {
                byte[] sendbuff = _sendingQueue.Dequeue();
                _pendingBufferList.Add(new ArraySegment<byte>(sendbuff,0,sendbuff.Length));
            }

            _sendingArgs.BufferList = _pendingBufferList;

            bool pending = _sessionSocket.SendAsync(_sendingArgs);
            if (pending == false)
            {
                SendCompleted(null, _sendingArgs);
            }

        }

        void SendCompleted(object sender, SocketAsyncEventArgs _sendingArgs)
        {
            if (_sendingArgs.BytesTransferred > 0 && _sendingArgs.SocketError == SocketError.Success)
            {
                try
                {
                    _sendingArgs.BufferList = null;
                    _pendingBufferList.Clear();
                    OnSending(_sendingArgs.BytesTransferred);

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
#endregion



        public void Disconnect()
        {
            if ( Interlocked.Exchange(ref _disconnectedCondition,1) == 1 )
            {
                return;
            }

            OnDisconnected(_sessionSocket.RemoteEndPoint);
            //연결종료 직전에 리슨과 센드를 모두 종료한다 
            _sessionSocket.Shutdown(SocketShutdown.Both);
            //리슨과 샌드를 모두 종료후 실제 연결을 종료한다
            _sessionSocket.Close();
        }
    }
}
