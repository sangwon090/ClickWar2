using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using ClickWar2.Network;
using ClickWar2.Network.IO;
using ClickWar2.Network.Protocol;

namespace ClickWar2.Game.Network.ServerWorker
{
    public class CompanyManager
    {
        public CompanyManager()
        {

        }

        //#####################################################################################

        public Action<NetMessage> NoticeDelegate
        { get; set; } = null;

        public Action<string[], NetMessage> NoticeWhereDelegate
        { get; set; }

        protected string CompanyFileName
        { get; set; } = "Companies.dat";

        public string ServerPath
        { get; set; }

        public NetServer Server
        { get; set; }

        public UserManager UserDirector
        { get; set; }

        public GameBoard GameBoard
        { get; set; }

        //#####################################################################################
        // 회사

        protected List<Company> m_companyList = new List<Company>();
        public List<Company> Companies
        { get { return m_companyList; } }

        protected Dictionary<int, Dictionary<int, string>> m_companySiteMap = new Dictionary<int, Dictionary<int, string>>();

        protected Dictionary<string, int> m_companySiteCount = new Dictionary<string, int>();
        public Dictionary<string, int> CompanySiteCount
        { get { return m_companySiteCount; } }

        //#####################################################################################
        // 칩

        protected List<Chip> m_chipList = new List<Chip>();
        public List<Chip> Chips
        { get { return m_chipList; } }
        protected Dictionary<Chip, Point> m_chipPosMap = new Dictionary<Chip, Point>();
        protected Dictionary<int, Dictionary<int, Chip>> m_chipMap = new Dictionary<int, Dictionary<int, Chip>>();

        //#####################################################################################
        // 상점

        protected List<SellingTech> m_techStore = new List<SellingTech>();

        protected List<SellingTech> m_productStore = new List<SellingTech>();

        //#####################################################################################
        // 메세지 처리자 등록

        public void InitEventChain(NetServerProcedure procList)
        {
            procList.Set(this.WhenReqRegisterCompany, (int)MessageTypes.Req_RegisterCompany);
            procList.Set(this.WhenReqBuildCompany, (int)MessageTypes.Req_BuildCompany);
            procList.Set(this.WhenReqDestroyCompany, (int)MessageTypes.Req_DestroyCompany);
            procList.Set(this.WhenReqDevelopTech, (int)MessageTypes.Req_DevelopTech);
            procList.Set(this.WhenReqDiscardTech, (int)MessageTypes.Req_DiscardTech);
            procList.Set(this.WhenReqProduceProduct, (int)MessageTypes.Req_ProduceProduct);
            procList.Set(this.WhenReqDiscardProduct, (int)MessageTypes.Req_DiscardProduct);
            procList.Set(this.WhenReqTechProgram, (int)MessageTypes.Req_TechProgram);
            procList.Set(this.WhenReqSellTech, (int)MessageTypes.Req_SellTech);
            procList.Set(this.WhenReqAllSellingTech, (int)MessageTypes.Req_AllSellingTech);
            procList.Set(this.WhenReqBuyTech, (int)MessageTypes.Req_BuyTech);
            procList.Set(this.WhenReqSellProduct, (int)MessageTypes.Req_SellProduct);
            procList.Set(this.WhenReqAllSellingProduct, (int)MessageTypes.Req_AllSellingProduct);
            procList.Set(this.WhenReqBuyProduct, (int)MessageTypes.Req_BuyProduct);
        }

        //#####################################################################################

        public void SaveAll()
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(this.ServerPath + this.CompanyFileName, FileMode.Create)))
            {
                bw.Write(Application.ProductVersion.ToString());


                bw.Write(m_companyList.Count);

                foreach (var company in m_companyList)
                {
                    company.WriteTo(bw);
                }


                bw.Write(m_companySiteMap.Count);

                foreach (var x_y in m_companySiteMap)
                {
                    bw.Write(x_y.Key);
                    bw.Write(x_y.Value.Count);

                    foreach (var y_name in x_y.Value)
                    {
                        bw.Write(y_name.Key);

                        bw.Write(y_name.Value);
                    }
                }


                bw.Write(m_chipPosMap.Count);

                foreach (var chip_pos in m_chipPosMap)
                {
                    chip_pos.Key.WriteTo(bw);

                    bw.Write(chip_pos.Value.X);
                    bw.Write(chip_pos.Value.Y);
                }


                bw.Write(m_techStore.Count);

                foreach (var tech in m_techStore)
                {
                    tech.WriteTo(bw);
                }


                bw.Write(m_productStore.Count);

                foreach (var tech in m_productStore)
                {
                    tech.WriteTo(bw);
                }
            }
        }

        public void LoadAll()
        {
            m_companyList.Clear();
            m_companySiteMap.Clear();
            m_companySiteCount.Clear();
            m_chipList.Clear();
            m_chipPosMap.Clear();
            m_chipMap.Clear();
            m_techStore.Clear();


            try
            {
                using (BinaryReader br = new BinaryReader(new FileStream(this.ServerPath + this.CompanyFileName, FileMode.Open)))
                {
                    Version fileVersion = new Version(br.ReadString());


                    int companyCount = br.ReadInt32();

                    for (int i = 0; i < companyCount; ++i)
                    {
                        var company = new Company();
                        company.ReadFrom(br);

                        m_companyList.Add(company);
                    }


                    int countX = br.ReadInt32();

                    for (int x = 0; x < countX; ++x)
                    {
                        int tileX = br.ReadInt32();
                        int countY = br.ReadInt32();


                        m_companySiteMap.Add(tileX, new Dictionary<int, string>());
                        var mapX = m_companySiteMap[tileX];


                        for (int y = 0; y < countY; ++y)
                        {
                            int tileY = br.ReadInt32();

                            string company = br.ReadString();


                            mapX.Add(tileY, company);
                            this.AddCompanySiteCount(company, 1);
                        }
                    }


                    int chipCount = br.ReadInt32();

                    for (int i = 0; i < chipCount; ++i)
                    {
                        Chip chip = new Chip();
                        chip.ReadFrom(br);

                        int tileX = br.ReadInt32();
                        int tileY = br.ReadInt32();

                        this.AddChip(tileX, tileY, chip);
                    }


                    int techStoreSize = br.ReadInt32();

                    for (int i = 0; i < techStoreSize; ++i)
                    {
                        SellingTech tech = new SellingTech();
                        tech.ReadFrom(br);

                        m_techStore.Add(tech);
                    }


                    int productStoreSize = br.ReadInt32();

                    for (int i = 0; i < productStoreSize; ++i)
                    {
                        SellingTech tech = new SellingTech();
                        tech.ReadFrom(br);

                        m_productStore.Add(tech);
                    }
                }
            }
            catch (FileNotFoundException)
            { }
            catch (EndOfStreamException)
            { }
        }

        public bool CompanyExists(string name)
        {
            return m_companyList.Any((company) => (company.Name == name));
        }

        public Company FindCompanyByName(string name)
        {
            foreach (var company in m_companyList)
            {
                if (company.Name == name)
                {
                    return company;
                }
            }


            return null;
        }

        protected void SetCompanyNameAt(int tileX, int tileY, string name)
        {
            if (name.Length > 0)
            {
                if (!m_companySiteMap.ContainsKey(tileX))
                    m_companySiteMap.Add(tileX, new Dictionary<int, string>());

                if (m_companySiteMap[tileX].ContainsKey(tileY))
                    m_companySiteMap[tileX][tileY] = name;
                else
                    m_companySiteMap[tileX].Add(tileY, name);
            }
            else
            {
                if (m_companySiteMap.ContainsKey(tileX)
                    && m_companySiteMap[tileX].ContainsKey(tileY))
                {
                    m_companySiteMap[tileX].Remove(tileY);

                    if (m_companySiteMap[tileX].Count <= 0)
                        m_companySiteMap.Remove(tileX);
                }
            }
        }

        public string GetCompanyNameAt(int tileX, int tileY)
        {
            if (m_companySiteMap.ContainsKey(tileX)
                && m_companySiteMap[tileX].ContainsKey(tileY))
            {
                return m_companySiteMap[tileX][tileY];
            }


            return "";
        }

        protected void AddCompanySiteCount(string name, int deltaCount)
        {
            if (m_companySiteCount.ContainsKey(name))
                m_companySiteCount[name] += deltaCount;
            else
                m_companySiteCount.Add(name, deltaCount);
        }

        public int GetCompanySiteCount(string name)
        {
            if (m_companySiteCount.ContainsKey(name))
                return m_companySiteCount[name];
            else
                return 0;
        }

        public void DestroyCompany(string userName, int tileX, int tileY)
        {
            // 건물 개수 갱신
            string companyName = this.GetCompanyNameAt(tileX, tileY);

            if (companyName.Length > 0)
            {
                this.AddCompanySiteCount(companyName, -1);
            }


            // 회사 폐쇄
            this.GameBoard.DestroyCompany(tileX, tileY);
            this.SetCompanyNameAt(tileX, tileY, "");


            // 폐쇄 알림
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(userName);
            writer.WriteData(tileX);
            writer.WriteData(tileY);
            writer.WriteData(companyName);

            this.NoticeDelegate(writer.CreateMessage((int)MessageTypes.Ntf_DestroyCompany));
        }
        
        public void AddChip(int tileX, int tileY, Chip chip)
        {
            m_chipList.Add(chip);
            m_chipPosMap.Add(chip, new Point(tileX, tileY));

            if (!m_chipMap.ContainsKey(tileX))
                m_chipMap.Add(tileX, new Dictionary<int, Chip>());

            if (m_chipMap[tileX].ContainsKey(tileY))
                m_chipMap[tileX][tileY] = chip;
            else
                m_chipMap[tileX].Add(tileY, chip);
        }

        public void RemoveChip(int tileX, int tileY)
        {
            Chip chip = null;

            if (m_chipMap.ContainsKey(tileX))
            {
                if (m_chipMap[tileX].ContainsKey(tileY))
                {
                    chip = m_chipMap[tileX][tileY];
                }
            }

            if (chip != null)
            {
                m_chipList.Remove(chip);
                m_chipPosMap.Remove(chip);

                m_chipMap[tileX].Remove(tileY);
                if (m_chipMap[tileX].Count <= 0)
                {
                    m_chipMap.Remove(tileX); 
                }
            }
        }

        public NetMessage GetDiscardProductNotice(string companyName, int productIndex)
        {
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(companyName);
            writer.WriteData(productIndex);

            return writer.CreateMessage((int)MessageTypes.Rsp_DiscardProduct);
        }

        public Point GetChipLocation(Chip chip)
        {
            if (m_chipPosMap.ContainsKey(chip))
                return m_chipPosMap[chip];
            else
                throw new ArgumentException("목록에 존재하지 않는 칩 입니다.");
        }

        public Chip GetChipAt(int tileX, int tileY)
        {
            if (m_chipMap.ContainsKey(tileX)
                && m_chipMap[tileX].ContainsKey(tileY))
            {
                return m_chipMap[tileX][tileY];
            }


            return null;
        }

        public NetMessage GetNoticeDevelopTech(string buyerName, string techName, DevelopTechResults result)
        {
            NetMessageStream addTechMsg = new NetMessageStream();
            addTechMsg.WriteData(buyerName);
            addTechMsg.WriteData(techName);
            addTechMsg.WriteData((int)result);

            return addTechMsg.CreateMessage((int)MessageTypes.Rsp_DevelopTech);
        }

        //#####################################################################################
        // 수신된 메세지 처리

        private NetMessage WhenReqRegisterCompany(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            string companyName = msg.ReadData<string>().Trim();


            RegisterCompanyResults result = RegisterCompanyResults.Fail_Unauthorized;


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                user = this.UserDirector.GetAccount(user.Name);

                if (user != null)
                {
                    // 이름이 너무 길면 실패
                    if (companyName.Length > GameValues.MaxCompanyNameLength)
                    {
                        result = RegisterCompanyResults.Fail_InvalidName;
                    }
                    // 등록비가 없으면 실패
                    else if (user.Resource < GameValues.CompanyRegistrationFee)
                    {
                        result = RegisterCompanyResults.Fail_NotEnoughResource;
                    }
                    // 이름이 중복되면 실패
                    else if (this.CompanyExists(companyName))
                    {
                        result = RegisterCompanyResults.Fail_AlreadyExist;
                    }
                    else
                    {
                        // 등록비 차감
                        user.Resource -= GameValues.CompanyRegistrationFee;

                        // 목록에 등록
                        m_companyList.Add(new Company()
                        {
                            Name = companyName,
                            Owner = user.Name,
                        });

                        // 유저의 회사 목록에 등록
                        user.Companies.Add(companyName);


                        // 성공
                        result = RegisterCompanyResults.Success;
                    }
                }
            }


            // 등록 결과 알림
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData((int)result);
            writer.WriteData(companyName);
            
            return writer.CreateMessage((int)MessageTypes.Rsp_RegisterCompany);
        }

        private NetMessage WhenReqBuildCompany(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            int tileX = msg.ReadData<int>();
            int tileY = msg.ReadData<int>();
            string companyName = msg.ReadData<string>().Trim();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                user = this.UserDirector.GetAccount(user.Name);

                // 유저가 존재하고 자원이 충분하고
                // 해당 타일이 존재하고
                if (user != null && user.Resource >= GameValues.CompanyBuildFee
                    && this.GameBoard.Board.ContainsItemAt(tileX, tileY))
                {
                    var company = this.FindCompanyByName(companyName);


                    // 해당 회사가 존재하고
                    // 자신의 회사이고
                    if (company != null
                        && company.Owner == user.Name)
                    {
                        var tile = this.GameBoard.Board.GetItemAt(tileX, tileY);


                        // 일반 타일이고
                        // 주인이 맞으면
                        if (tile != null && tile.IsNormalTile
                            && tile.Owner == user.Name)
                        {
                            // 건설비 차감
                            user.Resource -= GameValues.CompanyBuildFee;


                            // 회사 건설
                            this.GameBoard.BuildCompany(tileX, tileY, companyName, userName);
                            this.SetCompanyNameAt(tileX, tileY, companyName);


                            // 건물 개수 갱신
                            this.AddCompanySiteCount(companyName, 1);


                            // 건설 알림
                            NetMessageStream writer = new NetMessageStream();
                            writer.WriteData(userName);
                            writer.WriteData(tileX);
                            writer.WriteData(tileY);
                            writer.WriteData(companyName);

                            this.NoticeDelegate(writer.CreateMessage((int)MessageTypes.Ntf_BuildCompany));
                        }
                    }
                }
            }


            return null;
        }

        private NetMessage WhenReqDestroyCompany(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            int tileX = msg.ReadData<int>();
            int tileY = msg.ReadData<int>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                user = this.UserDirector.GetAccount(user.Name);

                // 유저가 존재하고
                // 해당 타일이 존재하고
                if (user != null
                    && this.GameBoard.Board.ContainsItemAt(tileX, tileY))
                {
                    var tile = this.GameBoard.Board.GetItemAt(tileX, tileY);

                    // 회사 타일이면
                    if (tile != null && tile.IsCompanyTile)
                    {
                        // 폐쇄
                        this.DestroyCompany(userName, tileX, tileY);
                    }
                }
            }


            return null;
        }

        private NetMessage WhenReqDevelopTech(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            string companyName = msg.ReadData<string>().Trim();
            string techName = msg.ReadData<string>().Trim();
            int programSize = msg.ReadData<int>();
            List<Command> program = new List<Command>();
            for (int i = 0; i < programSize; ++i)
            {
                Command cmd = new Command();
                cmd.ReadFromStream(msg);

                program.Add(cmd);
            }


            DevelopTechResults result = DevelopTechResults.Fail;


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                user = this.UserDirector.GetAccount(user.Name);
                var company = this.FindCompanyByName(companyName);

                int devFee = program.Count * GameValues.DevFeePerProgramLine;
                int maxTechCount = this.GetCompanySiteCount(companyName) * GameValues.CompanyTechSizePerSite;

                // 유저가 존재하고
                // 존재하는 회사가 유저의 소유이고
                // 여유공간이 있고
                // 기술 이름이 유효하고
                // 개발비가 있으면

                // 해당 계정이 없으면 실패
                if (user == null)
                {
                    result = DevelopTechResults.Fail_Unauthorized;
                }
                // 해당 회사가 없거나 자신의 소유가 아니면 실패
                else if (company == null || company.Owner != user.Name)
                {
                    result = DevelopTechResults.Fail_CompanyNotExist;
                }
                // 해당 회사의 여유공간이 부족하면 실패
                else if (company.TechList.Count >= maxTechCount)
                {
                    result = DevelopTechResults.Fail_NotEnoughClearance;
                }
                // 이름이 유효하지 않으면 실패
                else if (techName.Length <= 0)
                {
                    result = DevelopTechResults.Fail_InvalidName;
                }
                // 개발비가 없으면 실패
                else if (user.Resource < devFee)
                {
                    result = DevelopTechResults.Fail_NotEnoughResource;
                }
                else
                {
                    Chip newChip = new Chip();
                    newChip.Name = techName;
                    newChip.Program = program;

                    // 기술 이름이 중복되지 않으면
                    if (company.CheckTechOverlap(newChip) == false)
                    {
                        // 개발비 차감
                        user.Resource -= devFee;


                        // 회사에 기술 추가
                        company.AddTech(newChip);


                        // 개발 성공
                        result = DevelopTechResults.Success;
                    }
                    else
                    {
                        // 이름이 중복되면 실패
                        result = DevelopTechResults.Fail_AlreadyExist;
                    }
                }
            }
            else
            {
                // 클라이언트 이름이 올바르지 않으면 실패
                result = DevelopTechResults.Fail_Unauthorized;
            }


            // 개발 성공 알림
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(companyName);
            writer.WriteData(techName);
            writer.WriteData((int)result);

            return this.GetNoticeDevelopTech(companyName, techName, result);
        }

        private NetMessage WhenReqDiscardTech(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            string companyName = msg.ReadData<string>().Trim();
            string techName = msg.ReadData<string>().Trim();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                user = this.UserDirector.GetAccount(user.Name);
                var company = this.FindCompanyByName(companyName);

                // 유저가 존재하고
                // 존재하는 회사가 유저의 소유이고
                // 기술 이름이 유효하면
                if (user != null
                    && company != null && company.Owner == user.Name
                    && techName.Length > 0)
                {
                    // 회사에서 기술 제거
                    company.RemoveTech(techName);


                    // 기술 폐기 알림
                    NetMessageStream writer = new NetMessageStream();
                    writer.WriteData(companyName);
                    writer.WriteData(techName);

                    return writer.CreateMessage((int)MessageTypes.Rsp_DiscardTech);
                }
            }


            return null;
        }

        private NetMessage WhenReqProduceProduct(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            string companyName = msg.ReadData<string>().Trim();
            string techName = msg.ReadData<string>().Trim();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                user = this.UserDirector.GetAccount(user.Name);
                var company = this.FindCompanyByName(companyName);

                // 유저가 존재하고
                // 존재하는 회사가 유저의 소유이고
                // 기술 이름이 유효하고
                if (user != null
                    && company != null && company.Owner == user.Name
                    && techName.Length > 0)
                {
                    Chip techChip = company.GetTech(techName);

                    if (techChip != null)
                    {
                        int produceFee = techChip.Program.Count * GameValues.ProduceFeePerProgramLine;
                        int maxProductCount = this.GetCompanySiteCount(companyName) * GameValues.CompanyProductSizePerSite;


                        // 생산비가 충분하고
                        // 여유공간이 있으면
                        if (user.Resource >= produceFee
                            && company.ProductList.Count < maxProductCount)
                        {
                            // 생산비 차감
                            user.Resource -= produceFee;


                            // 회사 창고에 생산품 추가
                            company.AddProduct(techChip.Clone());


                            // 생산 알림
                            NetMessageStream writer = new NetMessageStream();
                            writer.WriteData(companyName);
                            writer.WriteData(techName);

                            return writer.CreateMessage((int)MessageTypes.Rsp_ProduceProduct);
                        }
                    }
                }
            }


            return null;
        }

        private NetMessage WhenReqDiscardProduct(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            string companyName = msg.ReadData<string>().Trim();
            int productIndex = msg.ReadData<int>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                user = this.UserDirector.GetAccount(user.Name);
                var company = this.FindCompanyByName(companyName);

                // 유저가 존재하고
                // 존재하는 회사가 유저의 소유이고
                // 제품 번호가 유효하면
                if (user != null
                    && company != null && company.Owner == user.Name
                    && productIndex >= 0 && productIndex < company.ProductList.Count)
                {
                    // 회사에서 제품 폐기
                    company.RemoveProductAt(productIndex);


                    // 제품 폐기 알림
                    return this.GetDiscardProductNotice(companyName, productIndex);
                }
            }


            return null;
        }

        private NetMessage WhenReqTechProgram(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            string companyName = msg.ReadData<string>().Trim();
            string techName = msg.ReadData<string>().Trim();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                user = this.UserDirector.GetAccount(user.Name);
                var company = this.FindCompanyByName(companyName);

                // 유저가 존재하고
                // 존재하는 회사가 유저의 소유이고
                // 기술 이름이 유효하고
                if (user != null
                    && company != null && company.Owner == user.Name
                    && techName.Length > 0)
                {
                    Chip techChip = company.GetTech(techName);

                    // 해당 기술이 회사에 존재하면
                    if (techChip != null)
                    {
                        var program = techChip.Program;

                        // 기술 프로그램 전송
                        NetMessageStream writer = new NetMessageStream();
                        writer.WriteData(companyName);
                        writer.WriteData(techName);
                        writer.WriteData(program.Count);
                        foreach (var cmd in program)
                        {
                            cmd.WriteToStream(writer);
                        }

                        return writer.CreateMessage((int)MessageTypes.Rsp_TechProgram);
                    }
                }
            }


            return null;
        }

        private NetMessage WhenReqSellTech(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            string companyName = msg.ReadData<string>().Trim();
            string techName = msg.ReadData<string>().Trim();
            int price = msg.ReadData<int>();
            string targetUser = msg.ReadData<string>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                user = this.UserDirector.GetAccount(user.Name);
                var company = this.FindCompanyByName(companyName);

                // 유저가 존재하고
                // 존재하는 회사가 유저의 소유이고
                // 기술 이름이 유효하고
                if (user != null
                    && company != null && company.Owner == user.Name
                    && techName.Length > 0)
                {
                    Chip techChip = company.GetTech(techName);

                    // 해당 기술이 회사에 존재하면
                    if (techChip != null)
                    {
                        // 판매 허가 및 기술 상점에 추가
                        SellingTech sellingTech = new SellingTech()
                        {
                            Name = techChip.Name,
                            Price = price,
                            Seller = company.Name,
                            TargetUser = targetUser,
                            Item = techChip.Clone(),
                        };
                        m_techStore.Add(sellingTech);


                        // 기술 판매 알림
                        NetMessageStream writer = new NetMessageStream();
                        writer.WriteData(company.Name);
                        writer.WriteData(techChip.Name);
                        writer.WriteData(price);

                        // 따로 허가된 유저가 있으면
                        if (targetUser.Length > 0)
                        {
                            // 판매자와 그 유저에게만 알림
                            this.NoticeWhereDelegate(new string[] { targetUser, user.Name },
                               writer.CreateMessage((int)MessageTypes.Ntf_SellTech));
                        }
                        else
                        {
                            // 전체공개이면 그냥 모두에게 알림
                            this.NoticeDelegate(writer.CreateMessage((int)MessageTypes.Ntf_SellTech));
                        }
                    }
                }
            }


            return null;
        }

        private NetMessage WhenReqAllSellingTech(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                foreach (var item in m_techStore)
                {
                    // 해당 기술 구매가 허가된 유저라면
                    if (item.TargetUser.Length <= 0
                        || item.TargetUser == user.Name)
                    {
                        // 정보 전송
                        NetMessageStream writer = new NetMessageStream();
                        writer.WriteData(item.Seller);
                        writer.WriteData(item.Name);
                        writer.WriteData(item.Price);

                        client.Sender.SendMessage(writer.CreateMessage((int)MessageTypes.Ntf_SellTech));
                    }
                }
            }


            return null;
        }

        private NetMessage WhenReqBuyTech(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            string sellerName = msg.ReadData<string>().Trim();
            string techName = msg.ReadData<string>().Trim();
            int price = msg.ReadData<int>();
            string buyerName = msg.ReadData<string>().Trim();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                user = this.UserDirector.GetAccount(user.Name);
                var seller = this.FindCompanyByName(sellerName);
                var buyer = this.FindCompanyByName(buyerName);

                int maxTechCount = this.GetCompanySiteCount(buyer.Name) * GameValues.CompanyTechSizePerSite;

                // 유저가 존재하고
                // 판매자가 존재하고
                // 존재하는 구매자가 유저의 소유이고 여유공간이 있으면
                // 기술 이름이 유효하고
                if (user != null
                    && seller != null
                    && buyer != null && buyer.Owner == user.Name && buyer.TechList.Count < maxTechCount
                    && techName.Length > 0)
                {
                    SellingTech sellingTech = null;
                    foreach (var tech in m_techStore)
                    {
                        if (tech.Name == techName
                            && tech.Seller == seller.Name
                            && tech.Price == price
                            && (tech.TargetUser.Length <= 0 || tech.TargetUser == user.Name))
                        {
                            sellingTech = tech;
                            break;
                        }
                    }


                    var sellerUser = this.UserDirector.GetAccount(seller.Owner);


                    // 해당 기술이 구매자에게 판매중이며
                    // 구매자가 해당 기술과 이름이 중복되는 기술을 가지고있지 않고
                    // 판매처의 주인이 존재하면
                    if (sellingTech != null
                        && buyer.CheckTechOverlap(sellingTech.Item) == false
                        && sellerUser != null)
                    {
                        // 상점에서 제거
                        m_techStore.Remove(sellingTech);


                        // 기술 구매
                        buyer.TechList.Add(sellingTech.Item.Clone());


                        // 가격만큼 차감
                        user.Resource -= sellingTech.Price;

                        // 가격만큼 입금
                        sellerUser.Resource += sellingTech.Price;


                        // 기술이 추가되었음을 알림
                        client.Sender.SendMessage(this.GetNoticeDevelopTech(buyer.Name, sellingTech.Name,
                            (int)DevelopTechResults.Success));


                        // 기술이 구매되어 상점에서 제거됨을 알림
                        NetMessageStream writer = new NetMessageStream();
                        writer.WriteData(seller.Name);
                        writer.WriteData(sellingTech.Name);
                        writer.WriteData(sellingTech.Price);

                        // 따로 허가된 유저가 있으면
                        if (sellingTech.TargetUser.Length > 0)
                        {
                            // 판매자와 그 유저에게만 알림
                            this.NoticeWhereDelegate(new string[] { seller.Owner, sellingTech.TargetUser },
                               writer.CreateMessage((int)MessageTypes.Ntf_BuyTech));
                        }
                        else
                        {
                            // 전체공개이면 그냥 모두에게 알림
                            this.NoticeDelegate(writer.CreateMessage((int)MessageTypes.Ntf_BuyTech));
                        }
                    }
                }
            }


            return null;
        }

        private NetMessage WhenReqSellProduct(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            string companyName = msg.ReadData<string>().Trim();
            int productIndex = msg.ReadData<int>();
            int price = msg.ReadData<int>();
            string targetUser = msg.ReadData<string>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                user = this.UserDirector.GetAccount(user.Name);
                var company = this.FindCompanyByName(companyName);

                // 유저가 존재하고
                // 존재하는 회사가 유저의 소유이고
                // 제품 번호가 유효하고
                if (user != null
                    && company != null && company.Owner == user.Name
                    && productIndex >= 0)
                {
                    Chip productChip = company.GetProductAt(productIndex);

                    // 해당 제품이 회사에 존재하면
                    if (productChip != null)
                    {
                        // 판매 허가 및 제품 상점에 추가
                        SellingTech sellingProduct = new SellingTech()
                        {
                            Name = productChip.Name,
                            Price = price,
                            Seller = company.Name,
                            TargetUser = targetUser,
                            Item = productChip.Clone(),
                        };
                        m_productStore.Add(sellingProduct);


                        // 회사의 창고에서 제거
                        client.Sender.SendMessage(GetDiscardProductNotice(company.Name, productIndex));


                        // 제품 판매 알림
                        NetMessageStream writer = new NetMessageStream();
                        writer.WriteData(company.Name);
                        writer.WriteData(productChip.Name);
                        writer.WriteData(price);

                        // 따로 허가된 유저가 있으면
                        if (targetUser.Length > 0)
                        {
                            // 판매자와 그 유저에게만 알림
                            this.NoticeWhereDelegate(new string[] { targetUser, user.Name },
                               writer.CreateMessage((int)MessageTypes.Ntf_SellProduct));
                        }
                        else
                        {
                            // 전체공개이면 그냥 모두에게 알림
                            this.NoticeDelegate(writer.CreateMessage((int)MessageTypes.Ntf_SellProduct));
                        }
                    }
                }
            }


            return null;
        }

        private NetMessage WhenReqAllSellingProduct(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                foreach (var item in m_productStore)
                {
                    // 해당 제품 구매가 허가된 유저라면
                    if (item.TargetUser.Length <= 0
                        || item.TargetUser == user.Name)
                    {
                        // 정보 전송
                        NetMessageStream writer = new NetMessageStream();
                        writer.WriteData(item.Seller);
                        writer.WriteData(item.Name);
                        writer.WriteData(item.Price);

                        client.Sender.SendMessage(writer.CreateMessage((int)MessageTypes.Ntf_SellProduct));
                    }
                }
            }


            return null;
        }

        private NetMessage WhenReqBuyProduct(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            string sellerName = msg.ReadData<string>().Trim();
            string productName = msg.ReadData<string>().Trim();
            int price = msg.ReadData<int>();
            string buyerName = msg.ReadData<string>().Trim();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                user = this.UserDirector.GetAccount(user.Name);
                var seller = this.FindCompanyByName(sellerName);
                var buyer = this.FindCompanyByName(buyerName);

                int maxProductCount = this.GetCompanySiteCount(buyer.Name) * GameValues.CompanyProductSizePerSite;

                // 유저가 존재하고
                // 판매자가 존재하고
                // 존재하는 구매자가 유저의 소유이고 여유공간이 있으면
                // 제품 이름이 유효하고
                if (user != null
                    && seller != null
                    && buyer != null && buyer.Owner == user.Name && buyer.ProductList.Count < maxProductCount
                    && productName.Length > 0)
                {
                    SellingTech sellingProduct = null;
                    foreach (var product in m_productStore)
                    {
                        if (product.Name == productName
                            && product.Seller == seller.Name
                            && product.Price == price
                            && (product.TargetUser.Length <= 0 || product.TargetUser == user.Name))
                        {
                            sellingProduct = product;
                            break;
                        }
                    }


                    var sellerUser = this.UserDirector.GetAccount(seller.Owner);


                    // 해당 제품이 구매자에게 판매중이며
                    // 판매처의 주인이 존재하면
                    if (sellingProduct != null
                        && sellerUser != null)
                    {
                        // 상점에서 제거
                        m_productStore.Remove(sellingProduct);


                        // 제품 구매
                        buyer.ProductList.Add(sellingProduct.Item.Clone());


                        // 가격만큼 차감
                        user.Resource -= sellingProduct.Price;

                        // 가격만큼 입금
                        sellerUser.Resource += sellingProduct.Price;


                        // 제품이 추가되었음을 알림
                        NetMessageStream addProductMsg = new NetMessageStream();
                        addProductMsg.WriteData(buyer.Name);
                        addProductMsg.WriteData(sellingProduct.Name);

                        client.Sender.SendMessage(addProductMsg.CreateMessage((int)MessageTypes.Rsp_ProduceProduct));


                        // 제품이 구매되어 상점에서 제거됨을 알림
                        NetMessageStream writer = new NetMessageStream();
                        writer.WriteData(seller.Name);
                        writer.WriteData(sellingProduct.Name);
                        writer.WriteData(sellingProduct.Price);

                        // 따로 허가된 유저가 있으면
                        if (sellingProduct.TargetUser.Length > 0)
                        {
                            // 판매자와 그 유저에게만 알림
                            this.NoticeWhereDelegate(new string[] { seller.Owner, sellingProduct.TargetUser },
                               writer.CreateMessage((int)MessageTypes.Ntf_BuyProduct));
                        }
                        else
                        {
                            // 전체공개이면 그냥 모두에게 알림
                            this.NoticeDelegate(writer.CreateMessage((int)MessageTypes.Ntf_BuyProduct));
                        }
                    }
                }
            }


            return null;
        }
    }
}
