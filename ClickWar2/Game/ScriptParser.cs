using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickWar2.Game
{
    public class ScriptParser
    {
        public ScriptParser()
        {

        }

        //#####################################################################################

        protected string Trim(StringBuilder str)
        {
            return str.ToString().Trim();
        }

        public List<Command> Parse(string script)
        {
            List<Command> cmdList = new List<Command>();


            Command tempCmd = new Command();
            StringBuilder temp = new StringBuilder();
            Stack<char> tokenStack = new Stack<char>();

            tokenStack.Push('\0');

            foreach (char ch in script)
            {
                char currentToken = tokenStack.Peek();

                if (currentToken == '\0')
                {
                    if (ch == ':')
                    {
                        string word = this.Trim(temp);
                        temp.Clear();

                        tempCmd.Name = word;
                    }
                    else if (ch == ',')
                    {
                        string word = this.Trim(temp);
                        temp.Clear();

                        tempCmd.Parameters.Add(word);
                    }
                    else if (ch == ';')
                    {
                        string word = this.Trim(temp);
                        temp.Clear();

                        tempCmd.Parameters.Add(word);

                        cmdList.Add(tempCmd);
                        tempCmd = new ClickWar2.Game.Command();
                    }
                    else if (ch == '\"')
                    {
                        temp.Clear();
                        temp.Append(ch);

                        tokenStack.Push(ch);
                    }
                    else
                    {
                        temp.Append(ch);
                    }
                }
                else if (currentToken == '\"')
                {
                    if (ch == '\"')
                    {
                        temp.Append(ch);

                        tokenStack.Pop();
                    }
                    else
                    {
                        temp.Append(ch);
                    }
                }
            }


            return cmdList;
        }
    }
}
