﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

//33cfe8.dvrinside.com/CMSPluginInstaller64.msi
namespace servercore1105
{
    internal class Listener
    {
        Socket _listenerSocket;
        Action<Socket> _actionAtAccpet;

        public void Init(IPEndPoint _endpoint,Action<Socket> actionAtAccept)
        {
            // listener소켓 생성
            _listenerSocket = new Socket(_endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //내부 변수 액션에 원하는 작업을 받아 넣는다.
            _actionAtAccpet += actionAtAccept;
            //소켓 고정
            _listenerSocket.Bind(_endpoint);
            //서버 대기 시작 +최대 대기수
            _listenerSocket.Listen(10);
            Console.WriteLine("listener ready");


            //위에 설정한 최대 대기수 만큼 비동기적 함수를 등록하도록 하자.

            //비동기적 이벤트 콜백함수로 등록
            SocketAsyncEventArgs _Args1 = new SocketAsyncEventArgs();
            _Args1.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptCompleted);
            RegisterAccept(_Args1);

        }


        //클라 소켓을 비동기적으로 처리하기 위해서 아규먼트를 받는다.
        //이때 받아야하는 아규먼트는 액셉트 요청 아규먼트.
        void RegisterAccept(SocketAsyncEventArgs _AsyncArgus)
        {
            //기존 자료는 삭제한다.
            _AsyncArgus.AcceptSocket = null;

            bool pending = _listenerSocket.AcceptAsync(_AsyncArgus);
            if (pending == false)
            {
                AcceptCompleted(null,_AsyncArgus);
            }
        }

        void AcceptCompleted(object sender, SocketAsyncEventArgs _Args1)
        {
            if (_Args1.SocketError == SocketError.Success)
            {
                //TODO: 엑셉트가 성공되면 뭔가 한다
                _actionAtAccpet.Invoke(_Args1.AcceptSocket);
            }   
            else
            {
                Console.WriteLine(_Args1.SocketError.ToString());
            }

            //작업이 완료되고 새롭게 등록을 해줌
            RegisterAccept(_Args1);
        }
        public Socket Accept()
        {
            return _listenerSocket.Accept();
        }
    }
}