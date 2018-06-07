using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ClickWar2.Network.IO;

namespace ClickWar2.Game
{
    public enum TileTypes
    {
        Normal,
        Resource,
        ResourceFactory,
        Company,
        Chip,
    }


    public class Tile : Utility.INetMessageConvertible
    {
        public Tile()
        {
            // 확률적으로 자원타일이 됨.
            if (Utility.Random.Next(128) == 0)
            {
                m_type = TileTypes.Resource;
            }
        }

        //#####################################################################################

        protected string m_owner = "";
        public string Owner
        { get { return m_owner; } set { m_owner = value; } }

        protected int m_power = 0;
        public int Power
        { get { return m_power; } set { m_power = value; } }

        protected string m_sign = "";
        public string Sign
        { get { return m_sign; } set { m_sign = value; } }

        protected TileTypes m_type = TileTypes.Normal;
        public TileTypes Kind
        { get { return m_type; } set { m_type = value; } }

        //#####################################################################################

        public bool HaveOwner
        { get { return (m_owner.Length > 0); } }

        public bool HaveSign
        { get { return (m_sign.Length > 0); } }

        public bool Visible
        { get; set; } = false;

        public bool IsNormalTile
        { get { return (m_type == TileTypes.Normal); } }
        public bool IsResourceTile
        { get { return (m_type == TileTypes.Resource); } }
        public bool IsFactoryTile
        { get { return (m_type == TileTypes.ResourceFactory); } }
        public bool IsCompanyTile
        { get { return (m_type == TileTypes.Company); } }
        public bool IsChipTile
        { get { return (m_type == TileTypes.Chip); } }

        //#####################################################################################

        public void WriteToStream(NetMessageStream stream)
        {
            stream.WriteData(this.Owner);
            this.WriteToStreamExceptOwner(stream);
        }

        public void WriteToStreamExceptOwner(NetMessageStream stream)
        {
            stream.WriteData(this.Power);
            stream.WriteData(this.Sign);
            stream.WriteData((int)this.Kind);
        }

        public void ReadFromStream(NetMessageStream stream)
        {
            this.Owner = stream.ReadString();
            this.ReadFromStreamExceptOwner(stream);
        }

        public void ReadFromStreamExceptOwner(NetMessageStream stream)
        {
            this.Power = stream.ReadInt32();
            this.Sign = stream.ReadString();
            this.Kind = (TileTypes)stream.ReadInt32();
        }

        public void WriteToStream(BinaryWriter bw)
        {
            bw.Write(this.Owner);
            bw.Write(this.Power);
            bw.Write(this.Sign);
            bw.Write((int)this.Kind);
        }

        public void ReadFromStream(BinaryReader br)
        {
            this.Owner = br.ReadString();
            this.Power = br.ReadInt32();
            this.Sign = br.ReadString();
            this.Kind = (TileTypes)br.ReadInt32();
        }

        public void FromTile(Tile other)
        {
            this.Owner = other.Owner;
            this.Power = other.Power;
            this.Sign = other.Sign;
            this.Kind = other.Kind;
        }
    }
}
