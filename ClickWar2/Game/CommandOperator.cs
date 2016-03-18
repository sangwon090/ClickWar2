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
    public class CommandOperator
    {
        public CommandOperator()
        {
#if DEBUG
            SetWork("debug", 1, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                if (IsText(param[0]))
                {
                    System.Diagnostics.Debug.Write(ToText(param[0]));
                }
                else
                {
                    System.Diagnostics.Debug.Write(param[0]);
                }

                return 1;
            });
#endif


            SetWork("jump", 1, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                if (IsInt(param[0]))
                {
                    return ToInt(param[0]);
                }
                
                return 1;
            });
            SetWork("jump", "jmp");


            SetWork("jump if equals", 3, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                for (int i = 0; i < 2; ++i)
                {
                    if (IsText(param[i]))
                        param[i] = ToText(param[i]);
                }

                if (param[0] == param[1] && IsInt(param[2]))
                    return ToInt(param[2]);

                return 1;
            });
            SetWork("jump if equals", "jmp==");


            SetWork("jump if not equals", 3, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                for (int i = 0; i < 2; ++i)
                {
                    if (IsText(param[i]))
                        param[i] = ToText(param[i]);
                }

                if (param[0] != param[1] && IsInt(param[2]))
                    return ToInt(param[2]);

                return 1;
            });
            SetWork("jump if not equals", "jmp!=");


            SetWork("jump if small", 3, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                for (int i = 0; i < 2; ++i)
                {
                    if (!IsInt(param[i]))
                        return 1;
                }

                if (ToInt(param[0]) < ToInt(param[1]) && IsInt(param[2]))
                    return ToInt(param[2]);

                return 1;
            });
            SetWork("jump if small", "jmp<");


            SetWork("jump if big", 3, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                for (int i = 0; i < 2; ++i)
                {
                    if (!IsInt(param[i]))
                        return 1;
                }

                if (ToInt(param[0]) > ToInt(param[1]) && IsInt(param[2]))
                    return ToInt(param[2]);

                return 1;
            });
            SetWork("jump if big", "jmp>");


            SetWork("interrupt", 3, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                if (IsText(param[0]))
                    param[0] = ToText(param[0]);
                if (IsText(param[1]))
                    param[1] = ToText(param[1]);


                int targetX = herePos.X;
                int targetY = herePos.Y;

                switch (param[0])
                {
                    case "up":
                        --targetY;
                        break;

                    case "down":
                        ++targetY;
                        break;

                    case "left":
                        --targetX;
                        break;

                    case "right":
                        ++targetX;
                        break;

                    default:
                        return 1;
                }

                boardDirector.InterruptChip(targetX, targetY, param[1], param[2]);

                return 1;
            });
            SetWork("interrupt", "intr");


            SetWork("add", 3, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                if (IsText(param[0]))
                    param[0] = ToText(param[0]);

                string result = "";
                if (IsText(param[1]))
                {
                    result = ToText(param[1]);

                    if (IsText(param[2]))
                        result += ToText(param[2]);
                }
                else if(IsInt(param[1]) && IsInt(param[2]))
                {
                    int sum = ToInt(param[1]) + ToInt(param[2]);
                    result = sum.ToString();
                }

                this.SetVar(param[0], result);

                return 1;
            });


            SetWork("subtract", 3, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                if (IsText(param[0]))
                    param[0] = ToText(param[0]);

                string result = "";
                if (IsInt(param[1]) && IsInt(param[2]))
                {
                    int sum = ToInt(param[1]) - ToInt(param[2]);
                    result = sum.ToString();
                }

                this.SetVar(param[0], result);

                return 1;
            });
            SetWork("subtract", "sub");


            SetWork("multiply", 3, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                if (IsText(param[0]))
                    param[0] = ToText(param[0]);

                string result = "";
                if (IsInt(param[1]) && IsInt(param[2]))
                {
                    int multiple = ToInt(param[1]) * ToInt(param[2]);
                    result = multiple.ToString();
                }

                this.SetVar(param[0], result);

                return 1;
            });
            SetWork("multiply", "mul");


            SetWork("attack", 1, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                if (IsText(param[0]))
                    param[0] = ToText(param[0]);


                int targetX = herePos.X;
                int targetY = herePos.Y;

                switch (param[0])
                {
                    case "up":
                        --targetY;
                        break;

                    case "down":
                        ++targetY;
                        break;

                    case "left":
                        --targetX;
                        break;

                    case "right":
                        ++targetX;
                        break;
                }

                boardDirector.AttackTerritory(hereTile.Owner, targetX, targetY);

                return 1;
            });
            SetWork("attack", "atk");


            SetWork("send", 1, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                if (IsText(param[0]))
                    param[0] = ToText(param[0]);


                int targetX = herePos.X;
                int targetY = herePos.Y;

                switch (param[0])
                {
                    case "up":
                        --targetY;
                        break;

                    case "down":
                        ++targetY;
                        break;

                    case "left":
                        --targetX;
                        break;

                    case "right":
                        ++targetX;
                        break;

                    default:
                        return 1;
                }

                boardDirector.SendPower(hereTile.Owner, herePos.X, herePos.Y, targetX, targetY);

                return 1;
            });
            SetWork("send", "snd");


            SetWork("variable", 2, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                if (IsText(param[0]))
                {
                    param[0] = ToText(param[0]);

                    if (m_varMap.ContainsKey(param[0]))
                    {
                        m_varMap[param[0]] = param[1];
                    }
                    else if(m_varMap.Count < this.MaxVarCount)
                    {
                        m_varMap.Add(param[0], param[1]);
                    }
                }

                return 1;
            });
            SetWork("variable", "var");


            SetWork("get power", 2, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                if (IsText(param[0]))
                    param[0] = ToText(param[0]);
                if (IsText(param[1]))
                    param[1] = ToText(param[1]);


                int targetX = herePos.X;
                int targetY = herePos.Y;

                switch (param[1])
                {
                    case "up":
                        --targetY;
                        break;

                    case "down":
                        ++targetY;
                        break;

                    case "left":
                        --targetX;
                        break;

                    case "right":
                        ++targetX;
                        break;

                    case "here":
                        break;

                    default:
                        return 1;
                }

                Tile target = boardDirector.GameBoard.Board.GetItemAt(targetX, targetY);

                if (target != null)
                {
                    this.SetVar(param[0], target.Power.ToString());
                }
                
                return 1;
            });
            SetWork("get power", "pwr");


            SetWork("get owner", 2, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                if (IsText(param[0]))
                    param[0] = ToText(param[0]);
                if (IsText(param[1]))
                    param[1] = ToText(param[1]);


                int targetX = herePos.X;
                int targetY = herePos.Y;

                switch (param[1])
                {
                    case "up":
                        --targetY;
                        break;

                    case "down":
                        ++targetY;
                        break;

                    case "left":
                        --targetX;
                        break;

                    case "right":
                        ++targetX;
                        break;

                    case "here":
                        break;

                    default:
                        return 1;
                }

                Tile target = boardDirector.GameBoard.Board.GetItemAt(targetX, targetY);

                if (target != null)
                {
                    this.SetVar(param[0], "\"" + target.Owner + "\"");
                }

                return 1;
            });
            SetWork("get owner", "owr");


            SetWork("get resource", 2, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                if (IsText(param[0]))
                    param[0] = ToText(param[0]);
                if (IsText(param[1]))
                    param[1] = ToText(param[1]);

                var user = userDirector.GetAccount(param[1]);

                if (user != null)
                {
                    this.SetVar(param[0], user.Resource.ToString());
                }

                return 1;
            });
            SetWork("get resource", "src");


            SetWork("get area", 2, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                if (IsText(param[0]))
                    param[0] = ToText(param[0]);
                if (IsText(param[1]))
                    param[1] = ToText(param[1]);

                var user = userDirector.GetAccount(param[1]);

                if (user != null)
                {
                    this.SetVar(param[0], user.AreaCount.ToString());
                }

                return 1;
            });
            SetWork("get area", "ara");


            SetWork("get sign", 2, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                if (IsText(param[0]))
                    param[0] = ToText(param[0]);
                if (IsText(param[1]))
                    param[1] = ToText(param[1]);


                int targetX = herePos.X;
                int targetY = herePos.Y;

                switch (param[1])
                {
                    case "up":
                        --targetY;
                        break;

                    case "down":
                        ++targetY;
                        break;

                    case "left":
                        --targetX;
                        break;

                    case "right":
                        ++targetX;
                        break;

                    case "here":
                        break;

                    default:
                        return 1;
                }

                Tile target = boardDirector.GameBoard.Board.GetItemAt(targetX, targetY);

                if (target != null)
                {
                    this.SetVar(param[0], "\"" + target.Sign + "\"");
                }

                return 1;
            });
            SetWork("get sign", "sgn");


            /*SetWork("set sign", 2, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                if (IsText(param[0]))
                    param[0] = ToText(param[0]);
                if (IsText(param[1]))
                    param[1] = ToText(param[1]);


                int targetX = herePos.X;
                int targetY = herePos.Y;

                switch (param[0])
                {
                    case "up":
                        --targetY;
                        break;

                    case "down":
                        ++targetY;
                        break;

                    case "left":
                        --targetX;
                        break;

                    case "right":
                        ++targetX;
                        break;

                    case "here":
                        break;

                    default:
                        return 1;
                }

                boardDirector.EditSign(hereTile.Owner, targetX, targetY, param[1]);

                return 1;
            });
            SetWork("set sign", "sign");*/


            SetWork("convert", 1, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                if (IsInt(param[0]) == false)
                    return 1;

                boardDirector.ConvertResource(hereTile.Owner, herePos.X, herePos.Y, false, ToInt(param[0]));

                return 1;
            });
            SetWork("convert", "cvt");


            SetWork("to integer", 2, (boardDirector, userDirector, chip, param, hereTile, herePos) =>
            {
                if (IsText(param[0]))
                    param[0] = ToText(param[0]);

                if (IsText(param[1]))
                {
                    param[1] = ToText(param[1]);
                    
                    if (IsInt(param[1]))
                    {
                        this.SetVar(param[0], param[1]);
                    }
                }

                return 1;
            });
            SetWork("to integer", "int");
        }

        //#####################################################################################

        protected Dictionary<string, Func<GameBoardManager, UserManager, Chip, List<string>, Tile, Point, int>> m_workMap
            = new Dictionary<string, Func<GameBoardManager, UserManager, Chip, List<string>, Tile, Point, int>>();
        protected Dictionary<string, int> m_minParamCountMap = new Dictionary<string, int>();

        //#####################################################################################

        protected Dictionary<string, string> m_varMap = new Dictionary<string, string>();

        public int MaxVarCount
        { get; set; } = 1000;

        //#####################################################################################
        // 변수 참조

        protected string GetVar(string name)
        {
            if (m_varMap.ContainsKey(name))
                return m_varMap[name];
            return "";
        }

        protected void SetVar(string name, string value)
        {
            if (m_varMap.ContainsKey(name))
                m_varMap[name] = value;
        }

        //#####################################################################################
        // 문자열의 종류

        protected bool IsVar(string param)
        {
            return (!(IsText(param) || IsInt(param)));
        }

        protected bool IsText(string param)
        {
            param = param.Trim();

            if (param.Length >= 2)
            {
                return (param[0] == '\"' && param[param.Length - 1] == '\"');
            }


            return false;
        }

        protected bool IsInt(string param)
        {
            int temp;
            return int.TryParse(param, out temp);
        }

        protected string ToText(string param)
        {
            StringBuilder builder = new StringBuilder(param.Trim());

            if (builder.Length >= 2)
            {
                builder.Remove(0, 1);
                builder.Remove(builder.Length - 1, 1);

                for (int i = 0; i < builder.Length; ++i)
                {
                    if (builder[i] == '\\')
                    {
                        char specialChar = '\0';

                        if (i + 1 < builder.Length)
                        {
                            switch (builder[i + 1])
                            {
                                case '\\': specialChar = '\\'; break;
                                case '\"': specialChar = '\"'; break;
                                case '\'': specialChar = '\''; break;
                                case 't': specialChar = '\t'; break;
                                case 'n': specialChar = '\n'; break;
                            }

                            builder.Remove(i + 1, 1);
                        }

                        builder.Remove(i, 1);
                        
                        if (specialChar != '\0')
                            builder.Insert(i, specialChar);
                    }
                }
            }
            else
            {
                return "";
            }


            return builder.ToString();
        }

        protected int ToInt(string param)
        {
            int temp;
            if (int.TryParse(param, out temp))
                return temp;
            return 0;
        }

        //#####################################################################################

        protected void SetWork(string cmdName, int minParamCount,
            Func<GameBoardManager, UserManager, Chip, List<string>, Tile, Point, int> work)
        {
            m_workMap.Add(cmdName, work);
            m_minParamCountMap.Add(cmdName, minParamCount);
        }

        protected void SetWork(string originalCmdName, string cmdName)
        {
            if (m_workMap.ContainsKey(originalCmdName)
                && m_minParamCountMap.ContainsKey(originalCmdName))
            {
                m_workMap.Add(cmdName, m_workMap[originalCmdName]);
                m_minParamCountMap.Add(cmdName, m_minParamCountMap[originalCmdName]);
            }
        }

        protected void SetWork(string originalCmdName, string[] cmdNames)
        {
            if (m_workMap.ContainsKey(originalCmdName)
                && m_minParamCountMap.ContainsKey(originalCmdName))
            {
                for (int i = 0; i < cmdNames.Length; ++i)
                {
                    m_workMap.Add(cmdNames[i], m_workMap[originalCmdName]);
                    m_minParamCountMap.Add(cmdNames[i], m_minParamCountMap[originalCmdName]);
                }
            }
        }

        //#####################################################################################

        public void SaveStateTo(BinaryWriter bw)
        {
            bw.Write(m_varMap.Count);

            foreach (var name_value in m_varMap)
            {
                bw.Write(name_value.Key);
                bw.Write(name_value.Value);
            }
        }

        public void LoadStateFrom(BinaryReader br)
        {
            m_varMap.Clear();


            int varCount = br.ReadInt32();

            for (int v = 0; v < varCount; ++v)
            {
                string name = br.ReadString();
                string value = br.ReadString();

                m_varMap.Add(name, value);
            }
        }

        //#####################################################################################

        public int Run(GameBoardManager boardDirector, UserManager userDirector, Chip chip, string cmdName, List<string> parameter,
            Tile hereTile, Point herePos)
        {
            if (m_workMap.ContainsKey(cmdName)
                && m_minParamCountMap.ContainsKey(cmdName))
            {
                int minParamCount = m_minParamCountMap[cmdName];

                if (parameter.Count >= minParamCount)
                {
                    var cloneParam = new List<string>(parameter);

                    for (int i = 0; i < minParamCount; ++i)
                    {
                        if (IsVar(cloneParam[i]))
                            cloneParam[i] = GetVar(cloneParam[i]);
                    }

                    int next = m_workMap[cmdName](boardDirector, userDirector, chip, cloneParam, hereTile, herePos);


                    return next;
                }
            }


            return 1;
        }

        public void Interrupt(string varName, string data)
        {
            this.SetVar(varName, data);
        }
    }
}
