using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading;

namespace ClickWar2.Database
{
    class DBHelper
    {
        public DBHelper()
        {

        }

        //#####################################################################################

        protected MongoClient m_client = null;
        protected IMongoDatabase m_db = null;

        //#####################################################################################

        public void Connect()
        {
            string key = "9FD75F3873B743E84B3ABF81EB64395DE199F9F65C33301AB573D328B109B870";

            // 암호 복호화
            string dbURI = Utility.Security.DecodeEx("H1p+c+FAcZJVBHO/IeI9L/wu1SjgGk8ogGE/5ftfgXgwv8vrYIPy71bN8Z4LxLY9X9N3wSHiy2/4XL8b07KA+gE6o7I70ECGHYnJ3k/C/L4=",
                key);
            
            m_client = new MongoClient(dbURI);
            int slashIdx = dbURI.LastIndexOf('/');
            m_db = m_client.GetDatabase(dbURI.Substring(slashIdx + 1));
        }

        //#####################################################################################

        public IMongoCollection<BsonDocument> GetCollection(string name)
        {
            return m_db.GetCollection<BsonDocument>(name);
        }

        public BsonDocument GetDocument(string collectionName, string documentName)
        {
            var col = this.GetCollection(collectionName);

            var filter = Builders<BsonDocument>.Filter.Exists(documentName);

            // 네트워크 오류가 나면 여러번 다시 시도 해본뒤 그래도 안되면 null 반환
            for (int retryCount = 0; retryCount < 3; ++retryCount)
            {
                try
                {
                    var docs = col.Find(filter).ToList();

                    if (docs.Count > 0)
                    {
                        return docs[0];
                    }
                    else
                    {
                        break;
                    }
                }
                catch (MongoConnectionException)
                {
                    Thread.Sleep(1000);
                    continue;
                }
                catch (TimeoutException)
                {
                    Thread.Sleep(1000);
                    continue;
                }
            }


            return null;
        }
    }
}
