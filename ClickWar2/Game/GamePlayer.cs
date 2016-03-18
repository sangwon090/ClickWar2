using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace ClickWar2.Game
{
    public class GamePlayer
    {
        public GamePlayer()
        {

        }

        //#####################################################################################

        public string Name
        { get; set; } = "";

        public string Password
        { get; set; } = "";

        public int AreaCount
        { get; set; } = 0;

        public Color UserColor
        { get; set; }

        public int Resource
        { get; set; } = 0;

        public List<Mail> Mailbox
        { get; set; } = new List<Mail>();

        public List<string> Companies
        { get; set; } = new List<string>();

        //#####################################################################################

        public void SaveTo(StreamWriter sw)
        {
            sw.WriteLine(this.Name);
            sw.WriteLine(this.Password);
            sw.WriteLine(this.AreaCount);
            sw.WriteLine(this.UserColor.ToArgb());
            sw.WriteLine(this.Resource);

            sw.WriteLine(this.Mailbox.Count);
            foreach (var mail in this.Mailbox)
            {
                mail.WriteTo(sw);
            }

            sw.WriteLine(this.Companies.Count);
            foreach (var company in this.Companies)
            {
                sw.WriteLine(company);
            }
        }

        public void LoadFrom(StreamReader sr)
        {
            this.Mailbox.Clear();
            this.Companies.Clear();


            this.Name = sr.ReadLine();
            this.Password = sr.ReadLine();
            this.AreaCount = Convert.ToInt32(sr.ReadLine());
            this.UserColor = Color.FromArgb(Convert.ToInt32(sr.ReadLine()));
            this.Resource = Convert.ToInt32(sr.ReadLine());

            int mailCount = Convert.ToInt32(sr.ReadLine());
            for (int i = 0; i < mailCount; ++i)
            {
                Mail mail = new Mail();
                mail.ReadFrom(sr);

                this.Mailbox.Add(mail);
            }

            int companyCount = Convert.ToInt32(sr.ReadLine());
            for (int i = 0; i < companyCount; ++i)
            {
                this.Companies.Add(sr.ReadLine());
            }
        }
    }
}
