using servercore1105;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace packet_generator
{
    internal class PacketFormat
    {

        //0 = 패킷 아이디 Enum리스트 *,\r\n  *,  \r\n*,  \r\n
        public static string Header = @"
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
";


        


        //0 = 클래스명              *
        //1 = 클래스의 변수       public * {{ get; set; }}\r\n
 
        public static string packetDeclare = @"
    public class {0} : Packet
    {{
";
        public static List<(string, string)> packetDeclareVariablesList = new List<(string, string)>();

        //packetDeclareVariablesList[*][0],packetDeclareVariablesList[n][1]
        public static string packetDeclareVariablesFormat = @"
        public {0} {1};";


        //0 = 전체 데이터 사이즈 합 연산.  _size += sizeof(*);\r\n
        public static string packetInit = @"
        public override void Init()
        {
            _packetID = (ushort)PacketID.PLAYERINFOREQ;
";
        public static string packetInitFormat = @"
            _size += sizeof({0});";
        public static string packetInitFormatString = @"
            _size += sizeof(ushort);
            _size += (ushort)Encoding.Unicode.GetByteCount({0});";



        //0 = 직렬화. totalPacketArrayOffset = Serialize(*, _totalPacketArrayOffset);\r\n
        //1 = 역직렬화. totalPacketArrayOffset = Deserialize(*, _totalPacketArrayOffset);\r\n
        public static string packetFunction = @"
        }}

        public override byte[] SerializeAll()
        {{
            int _finalize = 0;
            _totalPacketArrayOffset = Serialize(_packetID, _totalPacketArrayOffset);
            {0}

            if (_size == _totalPacketArrayOffset)
            {{
                Console.WriteLine(""전체 직렬화 최종 확인"");
                _finalize = Serialize(_size, 0);
            }}
            else
            {{
                Console.WriteLine(""전체 직렬화 최종 확인에 실패했습니다."");
            }}
            return this._totalPacketArray;
        }}
        public override void DeserializeAll(byte[] Packet)
        {{
            int _tempPacketArrayOffset = 0;

            _size = 0;
            _packetID = 0;


            _size = ushort_Deserialize(Packet, _tempPacketArrayOffset);
            _tempPacketArrayOffset += sizeof(ushort);
            Console.WriteLine($""전체 사이즈는 {{_size}}입니다."");

            _packetID = ushort_Deserialize(Packet, _tempPacketArrayOffset);
            _tempPacketArrayOffset += sizeof(ushort);
            Console.WriteLine($""패킷아이디는 {{_packetID}}입니다."");

            {1}
        }}

    }}
";
        public static string packetFunctionSerializeFormat = @"
            _totalPacketArrayOffset = Serialize({0}, _totalPacketArrayOffset);";
        public static string packetFunctionDeserializeFormat = @"
            {1} = {0}_Deserialize(Packet, _totalPacketArrayOffset);
            _tempPacketArrayOffset += sizeof({0});";
        public static string packetStringDeserializeFormat = @"
            int stringsize = ushort_Deserialize(Packet, _tempPacketArrayOffset);
            _tempPacketArrayOffset += sizeof(ushort);
            {0} = string_Deserialize(Packet, _tempPacketArrayOffset, stringsize);
            _tempPacketArrayOffset += stringsize;";

        public static string packetEnumList = @"    public enum PacketID
    {{
        {0}
    }}
";
        public static List<string?> packetEnumListFormatList = new List<string?>();
        public static string packetEnumListFormat;
    }
}
