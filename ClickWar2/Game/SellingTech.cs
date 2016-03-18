using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ClickWar2.Game
{
    public class SellingTech
    {
        public SellingTech()
        {

        }

        //#####################################################################################

        public string Name
        { get; set; }

        public int Price
        { get; set; }

        public string Seller
        { get; set; }

        public string TargetUser
        { get; set; }

        public Chip Item
        { get; set; }

        //#####################################################################################

        public void WriteTo(BinaryWriter bw)
        {
            bw.Write(this.Name);
            bw.Write(this.Price);
            bw.Write(this.Seller);
            bw.Write(this.TargetUser);
            this.Item.WriteTo(bw);
        }

        public void ReadFrom(BinaryReader br)
        {
            this.Name = br.ReadString();
            this.Price = br.ReadInt32();
            this.Seller = br.ReadString();
            this.TargetUser = br.ReadString();
            this.Item = new Chip();
            this.Item.ReadFrom(br);
        }
    }
}
