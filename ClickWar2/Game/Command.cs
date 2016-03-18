using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ClickWar2.Network.IO;

namespace ClickWar2.Game
{
    public class Command : Utility.INetMessageConvertible
    {
        public Command()
        {

        }

        //#####################################################################################

        public string Name
        { get; set; }

        public List<string> Parameters
        { get; set; } = new List<string>();

        //#####################################################################################

        public Command Clone()
        {
            Command clone = new Command();
            clone.Name = this.Name;

            for (int i = 0; i < this.Parameters.Count; ++i)
            {
                clone.Parameters.Add(this.Parameters[i]);
            }


            return clone;
        }

        //#####################################################################################

        public void WriteTo(BinaryWriter bw)
        {
            bw.Write(this.Name);

            bw.Write(this.Parameters.Count);
            for (int i = 0; i < this.Parameters.Count; ++i)
            {
                bw.Write(this.Parameters[i]);
            }
        }

        public void ReadFrom(BinaryReader br)
        {
            this.Parameters.Clear();


            this.Name = br.ReadString();

            int paramCount = br.ReadInt32();
            for (int i = 0; i < paramCount; ++i)
            {
                this.Parameters.Add(br.ReadString());
            }
        }

        public void WriteToStream(NetMessageStream stream)
        {
            stream.WriteData(this.Name);

            stream.WriteData(this.Parameters.Count);
            for (int i = 0; i < this.Parameters.Count; ++i)
            {
                stream.WriteData(this.Parameters[i]);
            }
        }

        public void ReadFromStream(NetMessageStream stream)
        {
            this.Parameters.Clear();


            this.Name = stream.ReadData<string>();

            int paramCount = stream.ReadData<int>();
            for (int i = 0; i < paramCount; ++i)
            {
                this.Parameters.Add(stream.ReadData<string>());
            }
        }

        //#####################################################################################

        public override string ToString()
        {
            StringBuilder result = new StringBuilder(this.Name);
            result.Append(':');

            foreach (string param in this.Parameters)
            {
                result.Append(param);
                result.Append(',');
            }

            if (this.Parameters.Count > 0)
            {
                result.Remove(result.Length - 1, 1);
            }

            result.Append(';');


            return result.ToString();
        }
    }
}
