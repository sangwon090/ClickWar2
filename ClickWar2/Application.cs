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

        //#####################################################################################

        public static bool CheckUpdateAndNotice(string documentName, string appVersion,
            out string downloadLink, out bool bShutdown, out string notice)
        {
            var db = new Database.DBHelper();
            db.Connect();

            var doc = db.GetDocument("Publish", documentName);

            if (doc == null)
            {
                downloadLink = "http://blog.naver.com/tlsehdgus321";
                bShutdown = true;
                notice = "";
                return true;
            }
            else
            {
                bShutdown = doc["Shutdown"].AsBoolean;
                notice = doc["Notice"].AsString;


                var currentVersion = new Utility.Version(appVersion);
                var latestVersion = new Utility.Version(doc["LatestVersion"].AsString);

                if (currentVersion < latestVersion)
                {
                    downloadLink = doc["DownloadLink"].AsString;
                    return true;
                }
                else
                {
                    downloadLink = "";
                    return false;
                }
            }
        }

        public static List<Database.ServerData> GetOfficialServer(string documentName)
        {
            var results = new List<Database.ServerData>();


            var db = new Database.DBHelper();
            db.Connect();

            var doc = db.GetDocument("Server", documentName);

            if (doc != null)
            {
                var servers = doc["OfficialServerList"].AsBsonArray;

                foreach (var serverData in servers)
                {
                    results.Add(new Database.ServerData()
                    {
                        Name = serverData["Name"].AsString,
                        Address = serverData["Address"].AsString,
                        Port = serverData["Port"].AsString
                    });
                }
            }


            return results;
        }
    }
}
