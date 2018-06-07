using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickWar2.Network;
using ClickWar2.Network.IO;
using ClickWar2.Game.Event;

namespace ClickWar2.Game.Network.ClientWorker
{
    public class CompanyManager
    {
        public CompanyManager()
        {

        }

        //#####################################################################################

        public NetClient Client
        { get; set; }

        public SignManager SignDirector
        { get; set; }

        public GameBoardManager BoardDirector
        { get; set; }

        public UserDataManager UserDataDirector
        { get; set; }

        //#####################################################################################
        // 상점

        public List<SellingTech> TechStore
        { get; set; } = new List<SellingTech>();

        public List<SellingTech> ProductStore
        { get; set; } = new List<SellingTech>();

        //#####################################################################################

        public Action WhenCompanyTechListChanged = null;
        public Action WhenCompanyProductListChanged = null;

        //#####################################################################################
        // 이벤트 관리자

        public GameEventList EventDirector
        { get; set; } = new GameEventList();

        //#####################################################################################
        // 메세지 수신 콜백

        protected Action<RegisterCompanyResults> m_registerCompanyCallback = null;
        protected Action<DevelopTechResults> m_developTechCallback = null;
        protected Action m_discardTechCallback = null;
        protected Action m_produceProductCallback = null;
        protected Action m_discardProductCallback = null;
        protected Action<string, string, string> m_requestTechProgramCallback = null;

        //#####################################################################################
        // 메세지 처리자 등록

        public void InitEventChain(NetClientProcedure procList)
        {
            procList.Set(this.WhenRspRegisterCompany, (int)MessageTypes.Rsp_RegisterCompany);
            procList.Set(this.WhenNtfBuildCompany, (int)MessageTypes.Ntf_BuildCompany);
            procList.Set(this.WhenNtfDestroyCompany, (int)MessageTypes.Ntf_DestroyCompany);
            procList.Set(this.WhenRspDevelopTech, (int)MessageTypes.Rsp_DevelopTech);
            procList.Set(this.WhenRspDiscardTech, (int)MessageTypes.Rsp_DiscardTech);
            procList.Set(this.WhenRspProduceProduct, (int)MessageTypes.Rsp_ProduceProduct);
            procList.Set(this.WhenRspDiscardProduct, (int)MessageTypes.Rsp_DiscardProduct);
            procList.Set(this.WhenRspTechProgram, (int)MessageTypes.Rsp_TechProgram);
            procList.Set(this.WhenNtfSellTech, (int)MessageTypes.Ntf_SellTech);
            procList.Set(this.WhenNtfBuyTech, (int)MessageTypes.Ntf_BuyTech);
            procList.Set(this.WhenNtfSellProduct, (int)MessageTypes.Ntf_SellProduct);
            procList.Set(this.WhenNtfBuyProduct, (int)MessageTypes.Ntf_BuyProduct);
        }

        //#####################################################################################
        // 수신된 메세지 처리

        private void WhenRspRegisterCompany(NetMessageStream msg)
        {
            RegisterCompanyResults result = (RegisterCompanyResults)msg.ReadInt32();
            string name = msg.ReadString();


            if (m_registerCompanyCallback != null)
            {
                m_registerCompanyCallback(result);
                m_registerCompanyCallback = null;
            }
        }

        private void WhenNtfBuildCompany(NetMessageStream msg)
        {
            string userName = msg.ReadString();
            int tileX = msg.ReadInt32();
            int tileY = msg.ReadInt32();
            string companyName = msg.ReadString();


            // 자신의 회사이면
            if (this.UserDataDirector.Me.Companies.Any((name) => name == companyName))
            {
                // 건물 개수 갱신
                this.UserDataDirector.AddMyCompanySiteCount(companyName, 1);
            }


            // 클라측 보드에 존재하는 지역이고 시야 내의 타일이면
            if (this.BoardDirector.GameBoard.TileExistsAndVisibleAt(tileX, tileY))
            {
                // 동기화
                this.BoardDirector.GameBoard.BuildCompany(tileX, tileY, companyName, userName);
            }


            // 이벤트 발생
            this.EventDirector.StartEvent((int)CompanyEvents.BuildCompany,
                tileX, tileY, userName, new object[] { companyName });
        }

        private void WhenNtfDestroyCompany(NetMessageStream msg)
        {
            string userName = msg.ReadString();
            int tileX = msg.ReadInt32();
            int tileY = msg.ReadInt32();
            string companyName = msg.ReadString();


            // 자신의 회사이면
            if (this.UserDataDirector.Me.Companies.Any((name) => name == companyName))
            {
                // 건물 개수 갱신
                this.UserDataDirector.AddMyCompanySiteCount(companyName, -1);
            }


            // 클라측 보드에 존재하는 지역이고 시야 내의 타일이면
            if (this.BoardDirector.GameBoard.TileExistsAndVisibleAt(tileX, tileY))
            {
                // 동기화
                this.BoardDirector.GameBoard.DestroyCompany(tileX, tileY);
            }


            // 이벤트 발생
            this.EventDirector.StartEvent((int)CompanyEvents.DestroyCompany,
                tileX, tileY, userName, new object[] { companyName });
        }

        public void WhenRspDevelopTech(NetMessageStream msg)
        {
            string companyName = msg.ReadString();
            string techName = msg.ReadString();
            int result = msg.ReadInt32();


            // 동기화
            this.UserDataDirector.AddTechInMyCompany(companyName, techName);


            if (m_developTechCallback != null)
            {
                m_developTechCallback((DevelopTechResults)result);
                m_developTechCallback = null;
            }

            if (WhenCompanyTechListChanged != null)
            {
                WhenCompanyTechListChanged();
            }
        }

        public void WhenRspDiscardTech(NetMessageStream msg)
        {
            string companyName = msg.ReadString();
            string techName = msg.ReadString();


            // 동기화
            this.UserDataDirector.RemoveTechInMyCompany(companyName, techName);


            if (m_discardTechCallback != null)
            {
                m_discardTechCallback();
                m_discardTechCallback = null;
            }

            if (WhenCompanyTechListChanged != null)
            {
                WhenCompanyTechListChanged();
            }
        }

        public void WhenRspProduceProduct(NetMessageStream msg)
        {
            string companyName = msg.ReadString();
            string productName = msg.ReadString();


            // 동기화
            this.UserDataDirector.AddProductInMyCompany(companyName, productName);


            if (m_produceProductCallback != null)
            {
                m_produceProductCallback();
                m_produceProductCallback = null;
            }

            if (this.WhenCompanyProductListChanged != null)
            {
                WhenCompanyProductListChanged();
            }
        }

        public void WhenRspDiscardProduct(NetMessageStream msg)
        {
            string companyName = msg.ReadString();
            int productIndex = msg.ReadInt32();


            // 동기화
            this.UserDataDirector.RemoveProductInMyCompany(companyName, productIndex);


            if (m_discardProductCallback != null)
            {
                m_discardProductCallback();
                m_discardProductCallback = null;
            }

            if (this.WhenCompanyProductListChanged != null)
            {
                WhenCompanyProductListChanged();
            }
        }

        public void WhenRspTechProgram(NetMessageStream msg)
        {
            string companyName = msg.ReadString().Trim();
            string techName = msg.ReadString().Trim();
            int programSize = msg.ReadInt32();

            StringBuilder program = new StringBuilder();

            for (int i = 0; i < programSize; ++i)
            {
                Command cmd = new Command();
                cmd.ReadFromStream(msg);

                program.AppendLine(cmd.ToString());
            }


            if (m_requestTechProgramCallback != null)
            {
                m_requestTechProgramCallback(companyName, techName, program.ToString());
                m_requestTechProgramCallback = null;
            }
        }

        public void WhenNtfSellTech(NetMessageStream msg)
        {
            string companyName = msg.ReadString().Trim();
            string techName = msg.ReadString().Trim();
            int price = msg.ReadInt32();


            // 기술 상점 목록에 추가
            SellingTech newTech = new SellingTech()
            {
                Seller = companyName,
                Name = techName,
                Price = price,
            };
            this.TechStore.Add(newTech);
        }

        public void WhenNtfBuyTech(NetMessageStream msg)
        {
            string companyName = msg.ReadString().Trim();
            string techName = msg.ReadString().Trim();
            int price = msg.ReadInt32();


            // 기술 상점 목록에서 제거
            for (int i = 0; i < this.TechStore.Count; ++i)
            {
                var tech = this.TechStore[i];

                if (tech.Seller == companyName
                    && tech.Name == techName
                    && tech.Price == tech.Price)
                {
                    this.TechStore.RemoveAt(i);
                    break;
                }
            }


            // 이벤트 발생
            this.EventDirector.StartEvent((int)CompanyEvents.BuyTech,
                0, 0, companyName, new object[] { techName, price });
        }

        public void WhenNtfSellProduct(NetMessageStream msg)
        {
            string companyName = msg.ReadString().Trim();
            string productName = msg.ReadString().Trim();
            int price = msg.ReadInt32();


            // 제품 상점 목록에 추가
            SellingTech newProduct = new SellingTech()
            {
                Seller = companyName,
                Name = productName,
                Price = price,
            };
            this.ProductStore.Add(newProduct);
        }

        public void WhenNtfBuyProduct(NetMessageStream msg)
        {
            string companyName = msg.ReadString().Trim();
            string productName = msg.ReadString().Trim();
            int price = msg.ReadInt32();


            // 제품 상점 목록에서 제거
            for (int i = 0; i < this.ProductStore.Count; ++i)
            {
                var product = this.ProductStore[i];

                if (product.Seller == companyName
                    && product.Name == productName
                    && product.Price == product.Price)
                {
                    this.ProductStore.RemoveAt(i);
                    break;
                }
            }


            // 이벤트 발생
            this.EventDirector.StartEvent((int)CompanyEvents.BuyProduct,
                0, 0, companyName, new object[] { productName, price });
        }

        //#####################################################################################
        // 사용자 입력 처리

        public void RegisterCompany(string name, Action<RegisterCompanyResults> callbackAsync)
        {
            m_registerCompanyCallback = callbackAsync;


            // 회사 등록 요청
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(this.SignDirector.LoginName);
            writer.WriteData(name.Trim());

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_RegisterCompany));
        }

        public void BuildCompany(string name, int tileX, int tileY)
        {
            if (this.BoardDirector.GameBoard.Board.ContainsItemAt(tileX, tileY))
            {
                var tile = this.BoardDirector.GameBoard.Board.GetItemAt(tileX, tileY);

                // 타일의 주인이고
                // 타일이 일반 타일이면
                if (tile.Owner == this.SignDirector.LoginName
                    && tile.IsNormalTile)
                {
                    // 회사 건설 요청
                    NetMessageStream writer = new NetMessageStream();
                    writer.WriteData(this.SignDirector.LoginName);
                    writer.WriteData(tileX);
                    writer.WriteData(tileY);
                    writer.WriteData(name.Trim());

                    this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_BuildCompany));
                }
            }
        }

        public void DestroyCompany(int tileX, int tileY)
        {
            if (this.BoardDirector.GameBoard.Board.ContainsItemAt(tileX, tileY))
            {
                var tile = this.BoardDirector.GameBoard.Board.GetItemAt(tileX, tileY);

                // 타일의 주인이고
                // 타일이 회사 타일이면
                if (tile.Owner == this.SignDirector.LoginName
                    && tile.IsCompanyTile)
                {
                    // 회사 폐쇄 요청
                    NetMessageStream writer = new NetMessageStream();
                    writer.WriteData(this.SignDirector.LoginName);
                    writer.WriteData(tileX);
                    writer.WriteData(tileY);

                    this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_DestroyCompany));
                }
            }
        }

        public void DevelopTech(string companyName, string techName, List<Command> program,
            Action<DevelopTechResults> callbackAsync)
        {
            m_developTechCallback = callbackAsync;


            companyName = companyName.Trim();
            techName = techName.Trim();

            // 자신의 회사이고
            // 기술 이름이 유효하고
            // 프로그램이 존재하면
            if (this.UserDataDirector.CheckMyCompany(companyName)
                && techName.Length > 0
                && program != null)
            {
                // 기술 등록 요청
                NetMessageStream writer = new NetMessageStream();
                writer.WriteData(this.SignDirector.LoginName);
                writer.WriteData(companyName);
                writer.WriteData(techName);
                writer.WriteData(program.Count);
                foreach (var cmd in program)
                {
                    cmd.WriteToStream(writer);
                }

                this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_DevelopTech));
            }
        }

        public void DiscardTech(string companyName, string techName, Action callbackAsync)
        {
            m_discardTechCallback = callbackAsync;


            companyName = companyName.Trim();
            techName = techName.Trim();

            // 자신의 회사이고
            // 기술 이름이 유효하면
            if (this.UserDataDirector.CheckMyCompany(companyName)
                && techName.Length > 0)
            {
                // 기술 폐기 요청
                NetMessageStream writer = new NetMessageStream();
                writer.WriteData(this.SignDirector.LoginName);
                writer.WriteData(companyName);
                writer.WriteData(techName);

                this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_DiscardTech));
            }
        }

        public void ProduceProduct(string companyName, string techName, Action callbackAsync)
        {
            m_produceProductCallback = callbackAsync;


            companyName = companyName.Trim();
            techName = techName.Trim();

            // 자신의 회사이고
            // 기술 이름이 유효하면
            if (this.UserDataDirector.CheckMyCompany(companyName)
                && techName.Length > 0)
            {
                // 제품 생산 요청
                NetMessageStream writer = new NetMessageStream();
                writer.WriteData(this.SignDirector.LoginName);
                writer.WriteData(companyName);
                writer.WriteData(techName);

                this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_ProduceProduct));
            }
        }

        public void DiscardProduct(string companyName, int productIndex, Action callbackAsync)
        {
            m_discardProductCallback = callbackAsync;


            companyName = companyName.Trim();

            // 자신의 회사이고
            // 제품 번호가 유효하면
            if (this.UserDataDirector.CheckMyCompany(companyName)
                && productIndex >= 0)
            {
                // 제품 폐기 요청
                NetMessageStream writer = new NetMessageStream();
                writer.WriteData(this.SignDirector.LoginName);
                writer.WriteData(companyName);
                writer.WriteData(productIndex);

                this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_DiscardProduct));
            }
        }

        public void RequestTechProgram(string companyName, string techName,
            Action<string, string, string> callbackAsync)
        {
            m_requestTechProgramCallback = callbackAsync;


            companyName = companyName.Trim();
            techName = techName.Trim();

            // 자신의 회사이고
            // 기술 이름이 유효하면
            if (this.UserDataDirector.CheckMyCompany(companyName)
                && techName.Length > 0)
            {
                // 기술 프로그램 요청
                NetMessageStream writer = new NetMessageStream();
                writer.WriteData(this.SignDirector.LoginName);
                writer.WriteData(companyName);
                writer.WriteData(techName);

                this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_TechProgram));
            }
        }

        public void SellTech(string companyName, string techName, int price, string targetUser)
        {
            companyName = companyName.Trim();
            techName = techName.Trim();

            // 자신의 회사이고
            // 기술 이름이 유효하면
            if (this.UserDataDirector.CheckMyCompany(companyName)
                && techName.Length > 0)
            {
                // 기술 판매 요청
                NetMessageStream writer = new NetMessageStream();
                writer.WriteData(this.SignDirector.LoginName);
                writer.WriteData(companyName);
                writer.WriteData(techName);
                writer.WriteData(price);
                writer.WriteData(targetUser);

                this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_SellTech));
            }
        }

        public void RequestAllSellingTech()
        {
            // 기술 상점 정보 요청
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(this.SignDirector.LoginName);

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_AllSellingTech));
        }

        public void BuyTech(string sellerName, string techName, int price, string buyerName)
        {
            sellerName = sellerName.Trim();
            techName = techName.Trim();
            buyerName = buyerName.Trim();

            // 자신의 회사이고
            // 기술 이름이 유효하면
            if (this.UserDataDirector.CheckMyCompany(buyerName)
                && techName.Length > 0)
            {
                // 기술 구매 요청
                NetMessageStream writer = new NetMessageStream();
                writer.WriteData(this.SignDirector.LoginName);
                writer.WriteData(sellerName);
                writer.WriteData(techName);
                writer.WriteData(price);
                writer.WriteData(buyerName);

                this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_BuyTech));
            }
        }

        public void SellProduct(string companyName, int productIndex, int price, string targetUser)
        {
            companyName = companyName.Trim();

            // 자신의 회사이고
            // 제품 번호가 유효하면
            if (this.UserDataDirector.CheckMyCompany(companyName)
                && productIndex >= 0)
            {
                // 기술 판매 요청
                NetMessageStream writer = new NetMessageStream();
                writer.WriteData(this.SignDirector.LoginName);
                writer.WriteData(companyName);
                writer.WriteData(productIndex);
                writer.WriteData(price);
                writer.WriteData(targetUser);

                this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_SellProduct));
            }
        }

        public void RequestAllSellingProduct()
        {
            // 제품 상점 정보 요청
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(this.SignDirector.LoginName);

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_AllSellingProduct));
        }

        public void BuyProduct(string sellerName, string productName, int price, string buyerName)
        {
            sellerName = sellerName.Trim();
            productName = productName.Trim();
            buyerName = buyerName.Trim();

            // 자신의 회사이고
            // 제품 이름이 유효하면
            if (this.UserDataDirector.CheckMyCompany(buyerName)
                && productName.Length > 0)
            {
                // 기술 구매 요청
                NetMessageStream writer = new NetMessageStream();
                writer.WriteData(this.SignDirector.LoginName);
                writer.WriteData(sellerName);
                writer.WriteData(productName);
                writer.WriteData(price);
                writer.WriteData(buyerName);

                this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_BuyProduct));
            }
        }
    }
}
