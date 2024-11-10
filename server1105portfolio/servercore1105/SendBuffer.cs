using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace servercore1105
{

    public class SendBufferHelper
    {
        public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => { return null; });
        public static int sendBuffersizeInt { get; set; } = 536;
        public static ArraySegment<byte> Open(int requiredsize)
        {
            if (CurrentBuffer.Value == null)
            {
                CurrentBuffer.Value = new SendBuffer(requiredsize);

            }
            if (CurrentBuffer.Value.FreeSize < requiredsize)
            {
                CurrentBuffer.Value = new SendBuffer(requiredsize);
            }


            ArraySegment<byte> testseg = CurrentBuffer.Value.Open(requiredsize);

            return CurrentBuffer.Value.Open(requiredsize);


        }
        public static ArraySegment<byte> Close(int usedsize)
        {
            return CurrentBuffer.Value.Close(usedsize);
        }

    }


    public class SendBuffer: parent_Buffer
    {
        byte[] _textbookbuffer;
        int _usedSize = 0;
        public int sendFreeSize { get { return _textbookbuffer.Length - _usedSize; } }

        public SendBuffer() : base() { }
        public SendBuffer(int sendBuffersizeInt)

        {
            _textbookbuffer = new byte[sendBuffersizeInt];
            //_buffer = new ArraySegment<byte>(new byte[sendBuffersizeInt], 0, sendBuffersizeInt);
        }




        public ArraySegment<byte> Open (int requiredSize)
        {
            if (requiredSize > sendFreeSize)
            {

                return null;
            }
             
            return new ArraySegment<byte>(_textbookbuffer, _usedSize, requiredSize);
        }

        public ArraySegment<byte> Close(int usedSize)
        {

            ArraySegment<byte> textbooksegment = new ArraySegment<byte>(_textbookbuffer, _writeoffset, usedSize);

            _writeoffset += usedSize;
            return textbooksegment;

        }
    }
}
