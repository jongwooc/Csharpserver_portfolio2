using System;
using System.Collections.Generic;
using System.Drawing;
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
        public const int THEOROGICALOPTPACKETSIZE = 536;
        public const int USHORTSIZE = 2;
        public const int SHORTSIZE = 2;
        public const int INTSIZE = 4;
        public const int SINGLESIZE = 4;
        public const int DOUBLESIZE = 8;
        public const int ONEUTF16SIZE = 16;

        //전체 패킷의 0번자리는 전체 패킷의 사이즈이다. 따라서 사이즈와 패킷 아이디는 첫 4바이트에 항상 예약되어 있다.
        public ushort _size = 4;
        public ushort _packetID;

        //임시 패킷을 만들고 전체 사이즈를 기록할 첫자리를 비워둔다
        public byte[] _totalPacketArray = new byte[THEOROGICALOPTPACKETSIZE];
        public int _totalPacketArrayOffset = 2;

        public ushort Protocol { get { return this._packetID; } }



        public abstract void Init();

        public abstract byte[] SerializeAll();

        public abstract void DeserializeAll(byte[] Packet);

        #region serialize
        public int Serialize(ushort packetdata, int Offset)
        {
            Span<byte> _Span = new Span<byte>(this._totalPacketArray, Offset, sizeof(ushort));
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
                Offset += sizeof(short);
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
                Offset += sizeof(int);
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
                Offset += sizeof(float);
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
                Offset += sizeof(double);
                return Offset;
            }
            return 0;
        }
        public int Serialize(string packetdata, int Offset)
        {
            //먼저 들어갈 스트링의 길이를 적는다. 나중에 파싱할 때 사이즈를 알아야하니까..

            Span<byte> size_Span = new Span<byte>(this._totalPacketArray, Offset, sizeof(ushort));

            ushort _StringSizeInt = (ushort)Encoding.Unicode.GetByteCount(packetdata);
            bool success = BitConverter.TryWriteBytes(size_Span, _StringSizeInt);
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
        public int Serialize(List<ushort> packetlist, int Offset)
        {
            int _oldOffset = Offset;
            Span<byte> size_Span = new Span<byte>(this._totalPacketArray, Offset, sizeof(ushort));

            ushort _listcount = (ushort)packetlist.Count;
            bool success = BitConverter.TryWriteBytes(size_Span, _listcount);
            Offset += sizeof(ushort);

            foreach (ushort item in packetlist)
            {
                Offset = Serialize(item, Offset);
            }

            if ((Offset - _oldOffset) == ((_listcount + 1) * sizeof(ushort)))
            {
                return Offset;
            }
            return 0;
        }
        public int Serialize(List<short> packetlist, int Offset)
        {
            int _oldOffset = Offset;
            Span<byte> size_Span = new Span<byte>(this._totalPacketArray, Offset, sizeof(ushort));

            ushort _listcount = (ushort)packetlist.Count;
            bool success = BitConverter.TryWriteBytes(size_Span, _listcount);
            Offset += sizeof(ushort);

            foreach (short item in packetlist)
            {
                Offset = Serialize(item, Offset);
            }

            if ((Offset - _oldOffset) == ((_listcount + 1) * sizeof(short)))
            {
                return Offset;
            }
            return 0;
        }
        public int Serialize(List<int> packetlist, int Offset)
        {
            int _oldOffset = Offset;
            Span<byte> size_Span = new Span<byte>(this._totalPacketArray, Offset, sizeof(ushort));

            ushort _listcount = (ushort)packetlist.Count;
            bool success = BitConverter.TryWriteBytes(size_Span, _listcount);
            Offset += sizeof(ushort);

            foreach (int item in packetlist)
            {
                Offset = Serialize(item, Offset);
            }

            if ((Offset - _oldOffset) == ((_listcount + 1) * sizeof(int)))
            {
                return Offset;
            }
            return 0;
        }
        public int Serialize(List<float> packetlist, int Offset)
        {
            int _oldOffset = Offset;
            Span<byte> size_Span = new Span<byte>(this._totalPacketArray, Offset, sizeof(ushort));

            ushort _listcount = (ushort)packetlist.Count;
            bool success = BitConverter.TryWriteBytes(size_Span, _listcount);
            Offset += sizeof(ushort);

            foreach (float item in packetlist)
            {
                Offset = Serialize(item, Offset);
            }

            if ((Offset - _oldOffset) == ((_listcount + 1) * sizeof(float)))
            {
                return Offset;
            }
            return 0;
        }
        public int Serialize(List<double> packetlist, int Offset)
        {
            int _oldOffset = Offset;
            Span<byte> size_Span = new Span<byte>(this._totalPacketArray, Offset, sizeof(ushort));

            ushort _listcount = (ushort)packetlist.Count;
            bool success = BitConverter.TryWriteBytes(size_Span, _listcount);
            Offset += sizeof(ushort);

            foreach (double item in packetlist)
            {
                Offset = Serialize(item, Offset);
            }

            if ((Offset - _oldOffset) == ((_listcount + 1) * sizeof(double)))
            {
                return Offset;
            }
            return 0;
        }
        public int Serialize(List<string> packetlist, int Offset)
        {
            int _oldOffset = Offset;
            int list_Size = 0;
            Span<byte> listcount_Span = new Span<byte>(this._totalPacketArray, Offset, sizeof(ushort));

            ushort _listcount = (ushort)packetlist.Count;
            bool success = BitConverter.TryWriteBytes(listcount_Span, _listcount);
            Offset += sizeof(ushort);
            list_Size += sizeof(ushort);

            //스트링 전체 크기의 합을 기록할 자리를 임시로 비우고 오프셋을 밀어둔다.
            int tmp_list_string_size_offset = Offset;
            Offset += sizeof(ushort);
            list_Size += sizeof(ushort);

            foreach (string item in packetlist)
            {
                list_Size += Encoding.Unicode.GetByteCount(item);
                Offset = Serialize(item, Offset);
            }

            //list size에서 ushort를 두번 뺀 것을 리스트 카운트 다음에 비워둔 tmp_list_string_size_offset 에 넣는다.
            Span<byte> size_Span = new Span<byte>(this._totalPacketArray, tmp_list_string_size_offset, sizeof(ushort));
            success = BitConverter.TryWriteBytes(size_Span, list_Size - (sizeof(ushort) * 2));


            if ((Offset - _oldOffset) == list_Size)
            {
                return Offset;
            }
            return 0;
        }
        #endregion



        #region deserialize
        public ushort ushort_Deserialize(byte[] Packet, int Offset)
        {
            ushort data = BitConverter.ToUInt16(Packet, Offset);
            return data;
        }
        public short short_Deserialize(byte[] Packet, int Offset)
        {
            short data = BitConverter.ToInt16(Packet, Offset);
            return data;
        }
        public int int_Deserialize(byte[] Packet, int Offset)
        {
            int data = BitConverter.ToInt32(Packet, Offset);
            return data;
        }
        public float float_Deserialize(byte[] Packet, int Offset)
        {
            float data = BitConverter.ToSingle(Packet, Offset);
            return data;
        }
        public double double_Deserialize(byte[] Packet, int Offset)
        {
            double data = BitConverter.ToDouble(Packet, Offset);
            return data;
        }
        public string string_Deserialize(byte[] Packet, int Offset, int size)
        {
            string data = Encoding.Unicode.GetString(Packet, Offset, size);
            return data;
        }
        //리스트는 호출하는 함수에서 항목의 갯수를 ushort으로 항목의 숫자를 헤아린 뒤 항목의 숫자만큼 offset을 고쳐줘야한다.이후 내부에서는 임시 리스트를 만들어서 반환한다.
        public List<ushort> ushort_list_Deserialize(byte[] Packet, int Offset, int list_Size)
        {
            List<ushort> _temp_List_Ushort = new List<ushort>();
            for (int i = 0; i < list_Size; i++)
            {
                ushort data = BitConverter.ToUInt16(Packet, Offset);
                _temp_List_Ushort.Add(data);
            }
            return _temp_List_Ushort;
        }
        public List<short> short_list_Deserialize(byte[] Packet, int Offset, int list_Size)
        {
            List<short> _temp_List_Short = new List<short>();
            for (int i = 0; i < list_Size; i++)
            {
                short data = BitConverter.ToInt16(Packet, Offset);
                _temp_List_Short.Add(data);
            }
            return _temp_List_Short;
        }
        public List<int> int_list_Deserialize(byte[] Packet, int Offset, int list_Size)
        {
            List<int> _temp_List_Int = new List<int>();
            for (int i = 0; i < list_Size; i++)
            {
                int data = BitConverter.ToInt32(Packet, Offset);
                _temp_List_Int.Add(data);
            }
            return _temp_List_Int;
        }
        public List<float> float_list_Deserialize(byte[] Packet, int Offset, int list_Size)
        {
            List<float> _temp_List_Float = new List<float>();
            for (int i = 0; i < list_Size; i++)
            {
                float data = BitConverter.ToSingle(Packet, Offset);
                _temp_List_Float.Add(data);
            }
            return _temp_List_Float;
        }
        public List<double> list_Deserialize(byte[] Packet, int Offset, int list_Size)
        {
            List<double> _temp_List_Double = new List<double>();
            for (int i = 0; i < list_Size; i++)
            {
                double data = BitConverter.ToDouble(Packet, Offset);
                _temp_List_Double.Add(data);
            }
            return _temp_List_Double;
        }

        //리스트 스트링은 항목 갯수와 다음에 있는 전체 사이즈를 호출하는 함수측에서 처리해줘야 한다.
        public List<string> string_list_Deserialize(byte[] Packet, int Offset, int list_Size)
        {
            List<string> _temp_List_String = new List<string>();
            for (int i = 0; i < list_Size; i++)
            {
                ushort string_size = BitConverter.ToUInt16(Packet, Offset);
                Offset += sizeof(ushort);

                string data = Encoding.Unicode.GetString(Packet, Offset, string_size);
                _temp_List_String.Add(data);
            }
            return _temp_List_String;
        }
        #endregion

    }
}
