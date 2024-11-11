using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Threading.Channels;
using System.Xml;

namespace packet_generator
{
    internal class Program
    {


        static string generatedPackets;
        static void Main(string[] args)
        {
            XmlReaderSettings settings = new XmlReaderSettings() { IgnoreComments = true, IgnoreWhitespace = true };

            XmlReader xmlReader = XmlReader.Create("Packet_Def.xml", settings);
            xmlReader.MoveToContent();

            generatedPackets += PacketFormat.Header;

            while (xmlReader.Read())
            {
                if (xmlReader.Depth == 1 && xmlReader.NodeType == XmlNodeType.Element)
                    ParsePacket(xmlReader);


            }

            File.WriteAllText("PacketProtocl.cs", generatedPackets);

            xmlReader.Close();


        }

        private static void ParsePacket(XmlReader xmlReader)
        {
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


            generatedPackets += String.Format(PacketFormat.packetDeclare, PacketFormat.packetEnumListFormatList[0]);


            if (string.IsNullOrEmpty(packetname))
            {
                Console.WriteLine("No named packet");
                return;
            }

            ParseMembers(xmlReader);
            string DeclareVariables = System.String.Empty;
            string InitVariables = System.String.Empty;
            string FunctionSerialize = System.String.Empty;
            string FunctionDeserialize = System.String.Empty;
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
                FunctionDeserialize += String.Format(PacketFormat.packetFunctionDeserializeFormat, _tuple.Item1,_tuple.Item2);

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
