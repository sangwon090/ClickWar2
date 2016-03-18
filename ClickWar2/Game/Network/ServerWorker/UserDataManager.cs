using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickWar2.Network;
using ClickWar2.Network.IO;
using ClickWar2.Network.Protocol;

namespace ClickWar2.Game.Network.ServerWorker
{
    public class UserDataManager
    {
        public UserDataManager()
        {

        }

        //#####################################################################################

        public UserManager UserDirector
        { get; set; }

        public CompanyManager CompanyDirector
        { get; set; }

        //#####################################################################################
        // 메세지 처리자 등록

        public void InitEventChain(NetServerProcedure procList)
        {
            procList.Set(this.WhenReqUserColor, (int)MessageTypes.Req_UserColor);
            procList.Set(this.WhenReqAllUserInfo, (int)MessageTypes.Req_AllUserInfo);
            procList.Set(this.WhenReqMyAllCompanyName, (int)MessageTypes.Req_MyAllCompanyName);
            procList.Set(this.WhenReqMyAllCompanySiteCount, (int)MessageTypes.Req_MyAllCompanySiteCount);
            procList.Set(this.WhenReqMyAllCompanyTechList, (int)MessageTypes.Req_MyAllCompanyTechList);
            procList.Set(this.WhenReqMyAllCompanyProductList, (int)MessageTypes.Req_MyAllCompanyProductList);
        }

        //#####################################################################################
        // 수신된 메세지 처리

        private NetMessage WhenReqUserColor(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();


            var user = this.UserDirector.GetAccount(userName);
            
            if (user != null)
            {
                NetMessageStream writer = new NetMessageStream();
                writer.WriteData(userName);
                writer.WriteData(user.UserColor.ToArgb());

                return writer.CreateMessage((int)MessageTypes.Rsp_UserColor);
            }


            return null;
        }

        private NetMessage WhenReqAllUserInfo(ServerVisitor client, NetMessageStream msg)
        {
            var accounts = this.UserDirector.Accounts;

            if (accounts.Length > 0)
            {
                NetMessageStream writer = null;


                writer = new NetMessageStream();
                writer.WriteData<int>(1);

                client.Sender.SendMessage(writer.CreateMessage((int)MessageTypes.Rsp_AllUserInfo));


                foreach (var account in accounts)
                {
                    writer = new NetMessageStream();
                    writer.WriteData<int>(0);

                    writer.WriteData(account.UserColor.ToArgb());
                    writer.WriteData(account.Name);
                    writer.WriteData(account.AreaCount);
                    writer.WriteData(account.Resource);

                    writer.WriteData<int>((account == accounts.Last()) ? 1 : 0);

                    client.Sender.SendMessage(writer.CreateMessage((int)MessageTypes.Rsp_AllUserInfo));
                }
            }


            return null;
        }

        private NetMessage WhenReqMyAllCompanyName(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                user = this.UserDirector.GetAccount(user.Name);

                if (user != null)
                {
                    // 회사 이름 전송
                    NetMessageStream writer = new NetMessageStream();
                    writer.WriteData(user.Companies.Count);

                    foreach (string companyName in user.Companies)
                    {
                        writer.WriteData(companyName);
                    }


                    return writer.CreateMessage((int)MessageTypes.Rsp_MyAllCompanyName);
                }
            }


            return null;
        }

        private NetMessage WhenReqMyAllCompanySiteCount(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                user = this.UserDirector.GetAccount(user.Name);

                if (user != null)
                {
                    // 자기 회사별 건물 개수 계산
                    var wholeCompanySiteCount = this.CompanyDirector.CompanySiteCount;
                    Dictionary<string, int> myCompanySiteCount = new Dictionary<string, int>();

                    var myCompanies = user.Companies;
                    foreach (string myCompany in myCompanies)
                    {
                        if (wholeCompanySiteCount.ContainsKey(myCompany))
                        {
                            myCompanySiteCount.Add(myCompany, wholeCompanySiteCount[myCompany]);
                        }
                    }

                    
                    // 회사별 건물 개수 전송
                    NetMessageStream writer = new NetMessageStream();
                    writer.WriteData(myCompanySiteCount.Count);

                    foreach (var company_count in myCompanySiteCount)
                    {
                        writer.WriteData(company_count.Key);
                        writer.WriteData(company_count.Value);
                    }


                    return writer.CreateMessage((int)MessageTypes.Rsp_MyAllCompanySiteCount);
                }
            }


            return null;
        }

        private NetMessage WhenReqMyAllCompanyTechList(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                user = this.UserDirector.GetAccount(user.Name);

                if (user != null)
                {
                    var companies = user.Companies;


                    // 회사별 기술 정보 전송
                    NetMessageStream writer = new NetMessageStream();
                    writer.WriteData(companies.Count);

                    for (int i = 0; i < companies.Count; ++i)
                    {
                        var company = this.CompanyDirector.FindCompanyByName(companies[i]);
                        var techList = company.TechList;

                        writer.WriteData(companies[i]);
                        writer.WriteData(techList.Count);

                        foreach(var chip in techList)
                        {
                            writer.WriteData(chip.Name);
                        }
                    }


                    return writer.CreateMessage((int)MessageTypes.Rsp_MyAllCompanyTechList);
                }
            }


            return null;
        }

        private NetMessage WhenReqMyAllCompanyProductList(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                user = this.UserDirector.GetAccount(user.Name);

                if (user != null)
                {
                    var companies = user.Companies;


                    // 회사별 제품 정보 전송
                    NetMessageStream writer = new NetMessageStream();
                    writer.WriteData(companies.Count);

                    for (int i = 0; i < companies.Count; ++i)
                    {
                        var company = this.CompanyDirector.FindCompanyByName(companies[i]);
                        var productList = company.ProductList;

                        writer.WriteData(companies[i]);
                        writer.WriteData(productList.Count);

                        foreach (var chip in productList)
                        {
                            writer.WriteData(chip.Name);
                        }
                    }


                    return writer.CreateMessage((int)MessageTypes.Rsp_MyAllCompanyProductList);
                }
            }


            return null;
        }
    }
}
