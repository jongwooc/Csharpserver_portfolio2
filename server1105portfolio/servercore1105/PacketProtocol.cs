using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace servercore1105
{
    public enum PacketID
    {
        PLAYERINFOREQ = 1,
        PLAYERINFOOK = 2,
    }
    public class PlayerInfoReq : Packet
    {
        public int _PlayerID { get; set; }
        public string _PlayerName { get; set; }

        public override void Init()
        {
            _packetID = (ushort)PacketID.PLAYERINFOREQ;

            _size += sizeof(int);

            _size += sizeof(ushort);
            _size += (ushort)Encoding.Unicode.GetByteCount(_PlayerName);
        }




        public override byte[] SerializeAll()
        {
            int _finalize = 0;
            _totalPacketArrayOffset = Serialize(_packetID, _totalPacketArrayOffset);
            _totalPacketArrayOffset = Serialize(_PlayerID, _totalPacketArrayOffset);
            _totalPacketArrayOffset = Serialize(_PlayerName, _totalPacketArrayOffset);

            if (_size == _totalPacketArrayOffset)
            {
                Console.WriteLine("전체 직렬화 최종 확인");
                _finalize = Serialize(_size, 0);
            }
            else
            {
                Console.WriteLine("전체 직렬화 최종 확인에 실패했습니다.");
            }
            return this._totalPacketArray;
        }
        public override void DeserializeAll(byte[] Packet)
        {
            int _tempPacketArrayOffset = 0;

            _size = 0;
            _packetID = 0;
            _PlayerID = 0;
            _PlayerName = "직렬화작업 실패";


            _size = ushort_Deserialize(Packet, _tempPacketArrayOffset);
            _tempPacketArrayOffset += sizeof(ushort);
            Console.WriteLine($"전체 사이즈는 {_size}입니다.");

            _packetID = ushort_Deserialize(Packet, _tempPacketArrayOffset);
            _tempPacketArrayOffset += sizeof(ushort);
            Console.WriteLine($"패킷아이디는 {_packetID}입니다.");

            _PlayerID = int_Deserialize(Packet, _tempPacketArrayOffset);
            _tempPacketArrayOffset += sizeof(int);
            Console.WriteLine($"플레이어 아이디는 {_PlayerID}입니다.");

            //스트링은 먼저 사이즈 정보의 오프셋을 수정해야한다.
            int stringsize = ushort_Deserialize(Packet, _tempPacketArrayOffset);
            _tempPacketArrayOffset += sizeof(ushort);
            _PlayerName = string_Deserialize(Packet, _tempPacketArrayOffset, stringsize);
            _tempPacketArrayOffset += stringsize;
            Console.WriteLine($"플레이어 이름은 {_PlayerName}입니다.");
        }

    }
}
