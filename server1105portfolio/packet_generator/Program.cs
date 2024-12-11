using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Threading.Channels;
using System.Xml;

namespace packet_generator
{
    internal class Program
    {
        static string generatedPackets = System.String.Empty;
        static string finalPackets = System.String.Empty;
        static string EnumList = System.String.Empty;
        static void Main(string[] args)
        {
            XmlReaderSettings settings = new XmlReaderSettings() { IgnoreComments = true, IgnoreWhitespace = true };

            XmlReader xmlReader = XmlReader.Create("Packet_Def.xml", settings);
            xmlReader.MoveToContent();


            while (xmlReader.Read())
            {
                if (xmlReader.Depth == 1 && xmlReader.NodeType == XmlNodeType.Element)
                    ParsePacket(xmlReader);
            }

            finalPackets += PacketFormat.Header;
            finalPackets += String.Format(PacketFormat.packetEnumList, EnumList);
            finalPackets += generatedPackets;

            finalPackets += "\r\n}";
            File.WriteAllText("PacketProtocol.cs", finalPackets);

            xmlReader.Close();


        }

        private static void ParsePacket(XmlReader xmlReader)
        {
            string DeclareVariables = System.String.Empty;
            string InitVariables = System.String.Empty;
            string FunctionSerialize = System.String.Empty;
            string FunctionDeserialize = System.String.Empty;
            PacketFormat.packetDeclareVariablesList.Clear();

            if (xmlReader.NodeType == XmlNodeType.EndElement)
            {
                return;
            }
            if (xmlReader.Name.ToLower() != "packet")
            {
                Console.WriteLine("Not packet");
                return;
            }
            string packetname = xmlReader["name"];
            PacketFormat.packetEnumListFormatList.Add(packetname);


            generatedPackets += String.Format(PacketFormat.packetDeclare, packetname);
            EnumList += packetname.ToUpper();
            EnumList += ",\r\n\t\t";

            if (string.IsNullOrEmpty(packetname))
            {
                Console.WriteLine("No named packet");
                return;
            }

            ParseMembers(xmlReader);

            foreach ((string, string) _tuple in PacketFormat.packetDeclareVariablesList)
            {
                DeclareVariables += String.Format(PacketFormat.packetDeclareVariablesFormat, _tuple.Item1,_tuple.Item2);

                if (_tuple.Item1 == "string")
                {
                    InitVariables += String.Format(PacketFormat.packetInitFormatString, _tuple.Item2);
                }
                else
                {
                    InitVariables += String.Format(PacketFormat.packetInitFormat, _tuple.Item1);
                }
                
                FunctionSerialize += String.Format(PacketFormat.packetFunctionSerializeFormat, _tuple.Item2);

                if (_tuple.Item1 == "string")
                {
                    FunctionDeserialize += String.Format(PacketFormat.packetStringDeserializeFormat, _tuple.Item2);
                }
                else
                {
                    FunctionDeserialize += String.Format(PacketFormat.packetFunctionDeserializeFormat, _tuple.Item1, _tuple.Item2);
                }
            }
            generatedPackets += DeclareVariables;
            generatedPackets += PacketFormat.packetInit;
            generatedPackets += InitVariables;
            generatedPackets += String.Format(PacketFormat.packetFunction, FunctionSerialize, FunctionDeserialize);
        }

        private static void ParseMembers(XmlReader xmlReader)
        {
            string packetname = xmlReader["name"];
            int depth = xmlReader.Depth + 1;

            while (xmlReader.Read())
            {
                if (xmlReader.Depth != depth)
                {
                    break;
                }

                string membertname = xmlReader["name"];

                if (string.IsNullOrEmpty(packetname))
                {
                    Console.WriteLine("there is no member with name");
                    return;
                }
                string memberType = xmlReader.Name.ToLower();
                PacketFormat.packetDeclareVariablesList.Add((memberType, membertname));
            }
        }
    }
}
