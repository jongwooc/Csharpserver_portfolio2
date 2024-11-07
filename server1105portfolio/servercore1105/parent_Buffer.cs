using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace servercore1105
{
    public class parent_Buffer
    {
        public ArraySegment<byte> _buffer;
        public int _readoffset;
        public int _writeoffset;
        const int BASEBUFFERSIZE = 4096;

        public int DataSize { get { return _writeoffset - _readoffset; } }
        public int FreeSize { get { return _buffer.Count - _writeoffset; } }

        public ArraySegment<byte> ReadableDataSegment
        {  get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset+_readoffset, DataSize); } }

        public ArraySegment<byte> WritableDataSegment
        { get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset+_writeoffset, FreeSize); } }


        //생성자들
        public parent_Buffer() 
        { 
            _buffer = new ArraySegment<byte>(new byte[BASEBUFFERSIZE], 0, BASEBUFFERSIZE); 
        }
        public parent_Buffer(int bufferSizeInt)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSizeInt], 0, bufferSizeInt);
        }


        //기타 함수
        public void Clean()
        {
            int dataSizeInt = DataSize;

            //남은 데이터가 없으면 커서 위치만 리셋
            if (dataSizeInt == 0)
            {
                _readoffset = 0;
                _writeoffset = 0;
            }
            //남은 데이터가 있으면 리드오프셋을 0로 돌리는 위치로 전체 복사
            else
            {
                Array.Copy(_buffer.Array, _buffer.Offset+_readoffset, _buffer.Array, _buffer.Offset, DataSize);
                _readoffset = 0;
                _writeoffset = dataSizeInt;
            }
        }

        public bool OnRead(int numberofByte)
        {
            if (numberofByte > DataSize)
            {
                return false;
            }
            _readoffset += numberofByte;
            return true;
        }
        public bool OnWrite(int numberofByte)
        {
            if (numberofByte > FreeSize)
            {
                return false;
            }
            _writeoffset += numberofByte;
            return true;
        }
    }
}
