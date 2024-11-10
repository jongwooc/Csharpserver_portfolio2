using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace servercore1105
{
    public abstract class Packet
    {
        //미리 세팅된 값들을 적어놓는다.
        internal const int THEOROGICALOPTPACKETSIZE = 536;
        internal const int USHORTSIZE = 2;
        internal const int SHORTSIZE = 2;
        internal const int INTSIZE = 4;
        internal const int SINGLESIZE = 4;
        internal const int DOUBLESIZE = 8;
        internal const int ONEUTF16SIZE = 16;

        //전체 패킷의 0번자리는 전체 패킷의 사이즈이다. 따라서 사이즈와 패킷 아이디는 첫 4바이트에 항상 예약되어 있다.
        internal ushort _size = 4;
        internal ushort _packetID;

        //임시 패킷을 만들고 전체 사이즈를 기록할 첫자리를 비워둔다
        internal byte[] _totalPacketArray = new byte[THEOROGICALOPTPACKETSIZE];
        internal int _totalPacketArrayOffset = 2;



        public abstract void SerializeAll();

        public abstract void DeserializeAll(byte[] Packet);

        #region serialize
        public int Serialize(ushort packetdata,int Offset)
        {
            Span<byte> _Span = new Span<byte>(this._totalPacketArray,Offset,sizeof(ushort));
            bool success = BitConverter.TryWriteBytes(_Span, packetdata);
            if (success)
            {
                Offset += sizeof(ushort);
                return Offset;
            }
            return 0;
        }
        public int Serialize(short packetdata, int Offset)
        {
            Span<byte> _Span = new Span<byte>(this._totalPacketArray, Offset, sizeof(short));
            bool success = BitConverter.TryWriteBytes(_Span, packetdata);
            if (success)
            {
                Offset += sizeof(ushort);
                return Offset;
            }
            return 0;
        }
        public int Serialize(int packetdata, int Offset)
        {
            Span<byte> _Span = new Span<byte>(this._totalPacketArray, Offset, sizeof(int));
            bool success = BitConverter.TryWriteBytes(_Span, packetdata);
            if (success)
            {
                Offset += sizeof(ushort);
                return Offset;
            }
            return 0;
        }
        public int Serialize(float packetdata, int Offset)
        {
            Span<byte> _Span = new Span<byte>(this._totalPacketArray, Offset, sizeof(float));
            bool success = BitConverter.TryWriteBytes(_Span, packetdata);
            if (success)
            {
                Offset += sizeof(ushort);
                return Offset;
            }
            return 0;
        }
        public int Serialize(double packetdata, int Offset)
        {
            Span<byte> _Span = new Span<byte>(this._totalPacketArray, Offset, sizeof(double));
            bool success = BitConverter.TryWriteBytes(_Span, packetdata);
            if (success)
            {
                Offset += sizeof(ushort);
                return Offset;
            }
            return 0;
        }
        public int Serialize(string packetdata, int Offset)
        {
            //먼저 들어갈 스트링의 길이를 적는다. 나중에 파싱할 때 사이즈를 알아야하니까..

            Span<byte> size_Span = new Span<byte>(this._totalPacketArray, Offset, sizeof(ushort));

            int _StringSizeInt = Encoding.Unicode.GetByteCount(packetdata);
            bool success = BitConverter.TryWriteBytes(size_Span, (ushort)_StringSizeInt);
            Offset += sizeof(ushort);

            Span<byte> _Span = new Span<byte>(this._totalPacketArray, Offset, _StringSizeInt);
            byte[] tmpStringBytes = Encoding.Unicode.GetBytes(packetdata);


            Span<byte> stringSpan = new Span<byte>(tmpStringBytes);
            stringSpan.TryCopyTo(_Span);

            if (_StringSizeInt > 0)
            {
                Offset += _StringSizeInt;
                return Offset;
            }
            return 0;
        }
        #endregion



        #region deserialize
        internal ushort Ushort_Deserialize(byte[] Packet, int Offset)
        {
            ushort data = BitConverter.ToUInt16(Packet, Offset);
            return data;
        }
        internal short Short_Deserialize(byte[] Packet, int Offset)
        {
            short data = BitConverter.ToInt16(Packet, Offset);
            return data;
        }
        internal int Int_Deserialize(byte[] Packet, int Offset)
        {
            int data = BitConverter.ToInt32(Packet, Offset);
            return data;
        }
        internal float Float_Deserialize(byte[] Packet, int Offset)
        {
            float data = BitConverter.ToSingle(Packet, Offset);
            return data;
        }
        internal double Double_Deserialize(byte[] Packet, int Offset)
        {
            double data = BitConverter.ToDouble(Packet, Offset);
            return data;
        }
        internal string String_Deserialize(byte[] Packet, int Offset, int size)
        {
            string data = Encoding.Default.GetString(Packet,Offset, size);
            return data;
        }
        #endregion
    }

}
