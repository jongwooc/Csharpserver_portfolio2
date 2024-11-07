using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace servercore1105
{
    public class SendBuffer: parent_Buffer
    {
        public SendBuffer() : base() { }
        public SendBuffer(int sendBuffersizeInt)
        {
            _buffer = new ArraySegment<byte>(new byte[sendBuffersizeInt], 0, sendBuffersizeInt);
        }
    }
}
