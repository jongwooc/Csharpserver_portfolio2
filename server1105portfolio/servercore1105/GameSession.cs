using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace servercore1105
{
    public class GameSession : parent_Session
    {
        public override void OnConnected(EndPoint connectionEndPoint)
        {
            Console.WriteLine($"GameSession OnConnected from = {connectionEndPoint}");
        }
        public override void OnDisconnected(EndPoint connectionEndPoint)
        {
            Console.WriteLine($"GameSession OnDisconnected from = {connectionEndPoint}");
        }
        public override int OnReceived(ArraySegment<byte> receivedBufferArraySegment)
        {
            string recievedData = Encoding.UTF8.GetString(receivedBufferArraySegment.Array, receivedBufferArraySegment.Offset, receivedBufferArraySegment.Count);
            Console.WriteLine($"GameSession OnReceived data string {recievedData}");
            return receivedBufferArraySegment.Count;
        }
        public override void OnSending(int sendingBytesTransferredInt)
        {
            Console.WriteLine($"GameSession OnSending Transferred bytes = {sendingBytesTransferredInt}");
        }
    }
}
