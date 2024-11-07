using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace servercore1105
{
    public class ReceiveBuffer: parent_Buffer
    {
        public ReceiveBuffer():base(){}
        public ReceiveBuffer(int bufferSizeInt)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSizeInt], 0, bufferSizeInt);
        }
    }
}
