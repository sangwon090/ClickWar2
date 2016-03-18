using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickWar2.Network.IO
{
    public enum NetProtocols
    {
        ResetKey = -42,
        CheckPacket = -77,
        Die = -4444,
    }
}
