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
        //세션을 상속받거나 외부에서 사용할 인터페이스 추가
        public abstract void OnConnected(EndPoint clientEndPoint);
        public abstract int OnReceived(ArraySegment<byte> receivedBufferArraySegment);
        public abstract void OnSending(int sendingBytesTransferredInt);
        public abstract void OnDisconnected(EndPoint clientEndPoint);

    }
}
