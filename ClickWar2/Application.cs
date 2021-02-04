using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickWar2
{
    public static class Application
    {
        /// <summary>
        /// 현재 실행파일의 버전을 가져옵니다.
        /// </summary>
        public static Utility.Version ProductVersion
        { get; } = new Utility.Version(System.Windows.Forms.Application.ProductVersion);
    }
}