using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickWar2.Utility
{
    public interface INetMessageConvertible
    {
        void WriteToStream(Network.IO.NetMessageStream stream);
        void ReadFromStream(Network.IO.NetMessageStream stream);
    }
}
