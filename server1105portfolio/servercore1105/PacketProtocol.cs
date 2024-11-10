using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
        public int _PlayerID;
        public string _PlayerName;

        public void Init()
        {
            _size += sizeof(int);
            _packetID = (ushort)PacketID.PLAYERINFOREQ;
        }
        public void Init(int PlayerID, string Playername) 
        {
            _size += sizeof(int);

            _size += sizeof(ushort);
            _size += (ushort)Encoding.Unicode.GetByteCount(Playername);

            _packetID = (ushort)PacketID.PLAYERINFOREQ;

            _PlayerID = PlayerID;

            _PlayerName = Playername;
        }


        public override void SerializeAll()
        {
            int _finalize = 0;
            _totalPacketArrayOffset = Serialize(_packetID, _totalPacketArrayOffset);
            _totalPacketArrayOffset = Serialize(_PlayerID, _totalPacketArrayOffset);
            _totalPacketArrayOffset = Serialize((ushort)Encoding.Unicode.GetByteCount(_PlayerName), _totalPacketArrayOffset);

            if (_size == _totalPacketArrayOffset)
            {

                Console.WriteLine("전체 직렬화 최종 확인 중");
                _finalize = Serialize(_size, 0);
            }
            Console.WriteLine("전체 직렬화 최종 확인에 실패했습니다.");
        }
        public override void DeserializeAll(byte[] Packet)
        {
            int _tempPacketArrayOffset = 0;

            _size = 0;
            _packetID = 0;
            _PlayerID = 0;
            _PlayerName = "직렬화작업 실패";


            _size = Ushort_Deserialize(Packet, _tempPacketArrayOffset);
            _tempPacketArrayOffset += sizeof(ushort);
            Console.WriteLine($"전체 사이즈는 {_size}입니다.");

            _packetID = Ushort_Deserialize(Packet, _tempPacketArrayOffset);
            _tempPacketArrayOffset += sizeof(ushort);
            Console.WriteLine($"패킷아이디는 {_packetID}입니다.");

            _PlayerID = Int_Deserialize(Packet, _tempPacketArrayOffset);
            _tempPacketArrayOffset += sizeof(int);
            Console.WriteLine($"플레이어 아이디는 {_PlayerID}입니다.");

            int stringsize = Ushort_Deserialize(Packet, _tempPacketArrayOffset);
            _tempPacketArrayOffset += sizeof(ushort);
            _PlayerName = String_Deserialize(Packet, _tempPacketArrayOffset, stringsize);
            Console.WriteLine($"플레이어 이름은 {_PlayerName}입니다.");
        }

    }
}
