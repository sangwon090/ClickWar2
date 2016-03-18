using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ClickWar2.Game
{
    public class Company
    {
        public Company()
        {

        }

        //#####################################################################################

        public string Name
        { get; set; }

        public string Owner
        { get; set; }

        public List<Chip> TechList
        { get; set; } = new List<Chip>();

        public List<Chip> ProductList
        { get; set; } = new List<Chip>();

        //#####################################################################################

        public void WriteTo(BinaryWriter bw)
        {
            bw.Write(this.Name);
            bw.Write(this.Owner);

            bw.Write(this.TechList.Count);
            foreach (var chip in this.TechList)
            {
                chip.WriteTo(bw);
            }

            bw.Write(this.ProductList.Count);
            foreach (var chip in this.ProductList)
            {
                chip.WriteTo(bw);
            }
        }

        public void ReadFrom(BinaryReader br)
        {
            this.TechList.Clear();
            this.ProductList.Clear();


            this.Name = br.ReadString();
            this.Owner = br.ReadString();

            int techCount = br.ReadInt32();
            for (int i = 0; i < techCount; ++i)
            {
                Chip chip = new Chip();
                chip.ReadFrom(br);

                this.TechList.Add(chip);
            }

            int productCount = br.ReadInt32();
            for (int i = 0; i < productCount; ++i)
            {
                Chip chip = new Chip();
                chip.ReadFrom(br);

                this.ProductList.Add(chip);
            }
        }

        //#####################################################################################

        public bool CheckTechOverlap(Chip chip)
        {
            return this.TechList.Any(tech => tech.Name == chip.Name);
        }

        public void AddTech(Chip chip)
        {
            this.TechList.Add(chip);
        }

        public void RemoveTech(string name)
        {
            for (int i = 0; i < this.TechList.Count; ++i)
            {
                if (this.TechList[i].Name == name)
                {
                    this.TechList.RemoveAt(i);
                    break;
                }
            }
        }

        public Chip GetTech(string name)
        {
            for (int i = 0; i < this.TechList.Count; ++i)
            {
                if (this.TechList[i].Name == name)
                {
                    return this.TechList[i];
                }
            }


            return null;
        }

        public void AddProduct(Chip chip)
        {
            this.ProductList.Add(chip);
        }

        public void RemoveProductAt(int index)
        {
            if (index >= 0 && index < this.ProductList.Count)
            {
                this.ProductList.RemoveAt(index);
            }
        }

        public Chip GetProductAt(int index)
        {
            if (index >= 0 && index < this.ProductList.Count)
            {
                return this.ProductList[index];
            }


            return null;
        }
    }
}
