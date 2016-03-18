using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ClickWar2.Network
{
    public class Utility
    {
        //[Obsolete("성능상의 문제로 더이상 사용하지 않습니다.", true)]
        public static bool IsConnected(Socket socket)
        {
            // NOTE: 가끔씩 정상(false가 나왔지만 디버거를 통해 true로 고쳐도 잘 작동했음)인데도
            // 서버측에서 false를 반환해서 클라의 연결을 끊어버린다.

            for (int i = 0; i < 4; ++i)
            {
                try
                {
                    if (!(socket.Poll(100, SelectMode.SelectRead) && (socket.Available == 0)))
                        return true;
                    else
                        continue;
                }
                catch (SocketException) { continue; }
            }

            return false;
        }

        public static void InitializeSocketKeepAlive(Socket socket, UInt32 checkDelayMilliseconds)
        {
            int size = sizeof(UInt32);
            UInt32 on = 1;
            UInt32 keepAliveInterval = checkDelayMilliseconds;
            UInt32 retryInterval = 1000;
            byte[] inArray = new byte[size * 3];
            Array.Copy(BitConverter.GetBytes(on), 0, inArray, 0, size);
            Array.Copy(BitConverter.GetBytes(keepAliveInterval), 0, inArray, size, size);
            Array.Copy(BitConverter.GetBytes(retryInterval), 0, inArray, size * 2, size);

            socket.IOControl(IOControlCode.KeepAliveValues, inArray, null);
        }
    }
}
