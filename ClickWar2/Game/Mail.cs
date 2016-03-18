using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ClickWar2.Game
{
    public class Mail
    {
        public bool Read
        { get; set; }

        public string From
        { get; set; }

        public string To
        { get; set; }

        public string SendingDate
        { get; set; }

        public string Message
        { get; set; }

        //#####################################################################################

        public void WriteTo(StreamWriter sw)
        {
            sw.WriteLine(Read ? "1" : "0");
            sw.WriteLine(this.From);
            sw.WriteLine(this.To);
            sw.WriteLine(this.SendingDate);
            sw.WriteLine(this.Message.Replace('\n', '\t'));
        }

        public void ReadFrom(StreamReader sr)
        {
            this.Read = (sr.ReadLine().Trim() != "0");
            this.From = sr.ReadLine();
            this.To = sr.ReadLine();
            this.SendingDate = sr.ReadLine();
            this.Message = sr.ReadLine().Replace('\t', '\n');
        }
    }
}
