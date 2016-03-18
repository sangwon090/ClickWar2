using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using ClickWar2.Game.Network.ServerWorker;

namespace ClickWar2.Game
{
    public class Chip
    {
        public Chip()
        {
            
        }

        //#####################################################################################

        public string Name
        { get; set; }

        //#####################################################################################

        public List<Command> Program
        { get; set; } = new List<Command>();

        protected int Pointer
        { get; set; } = 0;

        protected CommandOperator m_cmdOperator = new CommandOperator();

        //#####################################################################################

        public Chip Clone()
        {
            Chip clone = new Chip();

            clone.Name = this.Name;

            foreach (Command cmd in this.Program)
                clone.Program.Add(cmd.Clone());

            clone.Pointer = this.Pointer;


            return clone;
        }

        //#####################################################################################

        public void WriteTo(BinaryWriter bw)
        {
            bw.Write(this.Name);

            bw.Write(this.Program.Count);
            for (int i = 0; i < this.Program.Count; ++i)
            {
                this.Program[i].WriteTo(bw);
            }

            bw.Write(this.Pointer);


            m_cmdOperator.SaveStateTo(bw);
        }

        public void ReadFrom(BinaryReader br)
        {
            this.Program.Clear();


            this.Name = br.ReadString();

            int programSize = br.ReadInt32();
            for (int i = 0; i < programSize; ++i)
            {
                Command cmd = new Command();
                cmd.ReadFrom(br);

                this.Program.Add(cmd);
            }

            this.Pointer = br.ReadInt32();


            m_cmdOperator.LoadStateFrom(br);
        }

        //#####################################################################################

        public void ExcuteNext(GameBoardManager boardDirector, UserManager userDirector, Tile hereTile, Point herePos)
        {
            if (this.Pointer < 0)
                this.Pointer = 0;

            if (this.Pointer < this.Program.Count)
            {
                int deltaJump = 1;


                Command cmd = this.Program[this.Pointer];

                if (cmd != null)
                {
                    deltaJump = m_cmdOperator.Run(boardDirector, userDirector, this, cmd.Name, cmd.Parameters, hereTile, herePos);
                }


                this.Pointer += deltaJump;
            }
        }

        public void Interrupt(string varName, string data)
        {
            m_cmdOperator.Interrupt(varName, data);
        }
    }
}
