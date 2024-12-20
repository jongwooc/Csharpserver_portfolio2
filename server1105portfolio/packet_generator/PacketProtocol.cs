
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
        PLAYERINFOREQ,
		PLAYEROK,
		
    }

    public class PlayerInfoReq : Packet
    {

        public int playerID;
        public string PlayerName;
        public override void Init()
        {
            _packetID = (ushort)PacketID.PLAYERINFOREQ;

            _size += sizeof(int);
            _size += sizeof(ushort);
            _size += (ushort)Encoding.Unicode.GetByteCount(PlayerName);
        }

        public override byte[] SerializeAll()
        {
            int _finalize = 0;
            _totalPacketArrayOffset = Serialize(_packetID, _totalPacketArrayOffset);
            
            _totalPacketArrayOffset = Serialize(playerID, _totalPacketArrayOffset);
            _totalPacketArrayOffset = Serialize(PlayerName, _totalPacketArrayOffset);

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


            _size = ushort_Deserialize(Packet, _tempPacketArrayOffset);
            _tempPacketArrayOffset += sizeof(ushort);
            Console.WriteLine($"전체 사이즈는 {_size}입니다.");

            _packetID = ushort_Deserialize(Packet, _tempPacketArrayOffset);
            _tempPacketArrayOffset += sizeof(ushort);
            Console.WriteLine($"패킷아이디는 {_packetID}입니다.");

            
            playerID = int_Deserialize(Packet, _totalPacketArrayOffset);
            _tempPacketArrayOffset += sizeof(int);
            int stringsize = ushort_Deserialize(Packet, _tempPacketArrayOffset);
            _tempPacketArrayOffset += sizeof(ushort);
            PlayerName = string_Deserialize(Packet, _tempPacketArrayOffset, stringsize);
            _tempPacketArrayOffset += stringsize;
        }

    }

    public class PlayerOK : Packet
    {

        public string PlayerName;
        public ushort movement;
        public override void Init()
        {
            _packetID = (ushort)PacketID.PLAYERINFOREQ;

            _size += sizeof(ushort);
            _size += (ushort)Encoding.Unicode.GetByteCount(PlayerName);
            _size += sizeof(ushort);
        }

        public override byte[] SerializeAll()
        {
            int _finalize = 0;
            _totalPacketArrayOffset = Serialize(_packetID, _totalPacketArrayOffset);
            
            _totalPacketArrayOffset = Serialize(PlayerName, _totalPacketArrayOffset);
            _totalPacketArrayOffset = Serialize(movement, _totalPacketArrayOffset);

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


            _size = ushort_Deserialize(Packet, _tempPacketArrayOffset);
            _tempPacketArrayOffset += sizeof(ushort);
            Console.WriteLine($"전체 사이즈는 {_size}입니다.");

            _packetID = ushort_Deserialize(Packet, _tempPacketArrayOffset);
            _tempPacketArrayOffset += sizeof(ushort);
            Console.WriteLine($"패킷아이디는 {_packetID}입니다.");

            
            int stringsize = ushort_Deserialize(Packet, _tempPacketArrayOffset);
            _tempPacketArrayOffset += sizeof(ushort);
            PlayerName = string_Deserialize(Packet, _tempPacketArrayOffset, stringsize);
            _tempPacketArrayOffset += stringsize;
            movement = ushort_Deserialize(Packet, _totalPacketArrayOffset);
            _tempPacketArrayOffset += sizeof(ushort);
        }

    }

}