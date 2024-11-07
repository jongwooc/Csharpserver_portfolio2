using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace servercore1105
{
    public class Connector
    {
        Func<parent_Session> _sessionFactory;

        public void Init (IPEndPoint endPoint, Func<parent_Session> sessionFactory)
        {
            Socket initSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory = sessionFactory;


            SocketAsyncEventArgs connectorArgs = new SocketAsyncEventArgs();
            connectorArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnectCompleted);

            connectorArgs.RemoteEndPoint = endPoint;
            connectorArgs.UserToken = initSocket;

            RegisterConnect(connectorArgs);
        }

        void RegisterConnect(SocketAsyncEventArgs registerConnectArgs)
        {
            Socket registerConnectSocket = registerConnectArgs.UserToken as Socket;


            if (registerConnectSocket == null)
            {
                return;
            }

            bool pending = false;
            pending = registerConnectSocket.ConnectAsync(registerConnectArgs);


            if (pending == false)
            {
                OnConnectCompleted(null, registerConnectArgs);
            }

        }
        void OnConnectCompleted(object sender, SocketAsyncEventArgs onConnectArgs)
        {
            if (onConnectArgs.SocketError == SocketError.Success)
            {
                parent_Session session = _sessionFactory.Invoke();
                session.Init(onConnectArgs.ConnectSocket);
                session.OnDisconnected(onConnectArgs.RemoteEndPoint);

            }
            else
            {
                Console.WriteLine($"onConnectCompleted Failed : {onConnectArgs.SocketError}");

            }
        }
    }
}
