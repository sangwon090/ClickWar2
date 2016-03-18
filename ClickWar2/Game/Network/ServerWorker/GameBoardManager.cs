using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ClickWar2.Network;
using ClickWar2.Network.IO;
using ClickWar2.Network.Protocol;

namespace ClickWar2.Game.Network.ServerWorker
{
    public class GameBoardManager
    {
        public GameBoardManager()
        {

        }

        //#####################################################################################

        public Action<NetMessage> NoticeDelegate
        { get; set; } = null;

        public Action<string[], NetMessage> NoticeWhereDelegate
        { get; set; } = null;

        public Func<int, ServerVisitor> FindClientByIDDelegate
        { get; set; }

        public GameBoard GameBoard
        { get; set; }

        public UserManager UserDirector
        { get; set; }

        public CompanyManager CompanyDirector
        { get; set; }

        //#####################################################################################

        protected Dictionary<string, Rectangle> m_userScreen = new Dictionary<string, Rectangle>();

        protected Dictionary<string, List<Point>> m_changedTileList = new Dictionary<string, List<Point>>();

        //#####################################################################################
        // 시간 제한자

        protected Utility.TimeLagManager m_updateLimiter = new Utility.TimeLagManager();

        protected Utility.TimeLagManager m_addPowerLimiter = new Utility.TimeLagManager();
        protected Utility.TimeLagManager m_attackLimiter = new Utility.TimeLagManager();

        //#####################################################################################

        protected int m_beginUpdateFactoryIndex = 0;
        protected int m_beginUpdateChipIndex = 0;

        //#####################################################################################
        // 실시간 갱신

        public void Update()
        {
            m_updateLimiter.Set(0, GameValues.MinRunFactoryDelay);
            if (m_updateLimiter.Tick(0))
            {
                var factories = this.GameBoard.Factories;

                int endIndex = m_beginUpdateFactoryIndex + 2;
                if (endIndex > factories.Count)
                    endIndex = factories.Count;

                for (int i = m_beginUpdateFactoryIndex; i < endIndex; ++i)
                {
                    ++m_beginUpdateFactoryIndex;


                    Point factoryPos = factories[i];
                    Tile factoryTile = this.GameBoard.Board.GetItemAt(factoryPos.X, factoryPos.Y);

                    // 타일이 존재하고
                    // 공장 타일이며
                    // 주인이 있고
                    // 충분한 힘이 있으면
                    if (factoryTile != null
                        && factoryTile.IsFactoryTile
                        && factoryTile.HaveOwner
                        && factoryTile.Power > GameValues.FactoryConversionAmount)
                    {
                        // 생산 활동을 통해서 땅의 힘을 소모하고 주인의 자원 증가

                        var user = this.UserDirector.GetAccount(factoryTile.Owner);

                        factoryTile.Power -= GameValues.FactoryConversionAmount;
                        user.Resource += GameValues.FactoryConversionAmount;


                        // 변경사항 알림
                        this.NtfSetPower(factoryPos.X, factoryPos.Y, factoryTile.Power);
                    }
                }


                if (m_beginUpdateFactoryIndex >= factories.Count)
                {
                    m_beginUpdateFactoryIndex = 0;


                    m_updateLimiter.Update(0);
                }
            }


            m_updateLimiter.Set(1, GameValues.MinRunChipDelay);
            if (m_updateLimiter.Tick(1))
            {
                var chips = this.CompanyDirector.Chips;

                int endIndex = m_beginUpdateChipIndex + 8;
                if (endIndex > chips.Count)
                    endIndex = chips.Count;

                for (int i = m_beginUpdateChipIndex; i < endIndex; ++i)
                {
                    ++m_beginUpdateChipIndex;


                    Chip chip = chips[i];
                    Point chipPos = this.CompanyDirector.GetChipLocation(chip);
                    Tile chipTile = this.GameBoard.Board.GetItemAt(chipPos.X, chipPos.Y);

                    // 타일이 존재하고
                    // 칩 타일이면
                    if (chipTile != null
                        && chipTile.IsChipTile)
                    {
                        // 칩 작동
                        chip.ExcuteNext(this, this.UserDirector, chipTile, chipPos);
                    }
                }


                if (m_beginUpdateChipIndex >= chips.Count)
                {
                    m_beginUpdateChipIndex = 0;


                    m_updateLimiter.Update(1);
                }
            }
        }

        //#####################################################################################
        // 메세지 처리자 등록

        public void InitEventChain(NetServerProcedure procList)
        {
            procList.Set(this.WhenReqCountryLocation, (int)MessageTypes.Req_CountryLocation);
            procList.Set(this.WhenReqNewTerritory, (int)MessageTypes.Req_NewTerritory);
            procList.Set(this.WhenReqAddTilePower, (int)MessageTypes.Req_AddTilePower);
            procList.Set(this.WhenReqChunk, (int)MessageTypes.Req_Chunk);
            procList.Set(this.WhenReqBuildCountry, (int)MessageTypes.Req_BuildCountry);
            procList.Set(this.WhenReqAttackTerritory, (int)MessageTypes.Req_AttackTerritory);
            procList.Set(this.WhenReqAllTerritory, (int)MessageTypes.Req_AllTerritory);
            procList.Set(this.WhenReqAllVision, (int)MessageTypes.Req_AllVision);
            procList.Set(this.WhenReqSendPower, (int)MessageTypes.Req_SendPower);
            procList.Set(this.WhenReqEditTileSign, (int)MessageTypes.Req_EditTileSign);
            procList.Set(this.WhenReqBuildFactory, (int)MessageTypes.Req_BuildFactory);
            procList.Set(this.WhenReqConvertAllResource, (int)MessageTypes.Req_ConvertAllResource);
            procList.Set(this.WhenReqDestroyFactory, (int)MessageTypes.Req_DestroyFactory);
            procList.Set(this.WhenReqBuildChip, (int)MessageTypes.Req_BuildChip);
            procList.Set(this.WhenReqDestroyChip, (int)MessageTypes.Req_DestroyChip);
            procList.Set(this.WhenReqSetScreen, (int)MessageTypes.Req_SetScreen);
        }

        //#####################################################################################
        // 메세지 처리에 사용되는 공통 작업

        protected bool CheckTileInScreen(int tileX, int tileY, string targetUserName)
        {
            Rectangle screen = this.GetUserScreen(targetUserName);
            
            return (screen != Rectangle.Empty
                && tileX >= screen.Left && tileX <= screen.Right
                && tileY >= screen.Top && tileY <= screen.Bottom);
        }

        protected void AddIndexForSync(int tileX, int tileY, string targetUserName)
        {
            // 수신할 유저가 로그인 상태이면
            if (this.UserDirector.GetLoginUser(targetUserName) != null)
            {
                // 변경점 목록에 추가

                List<Point> changedList = null;

                if (m_changedTileList.ContainsKey(targetUserName))
                {
                    changedList = m_changedTileList[targetUserName];
                }
                else
                {
                    changedList = new List<Point>();
                    m_changedTileList.Add(targetUserName, changedList);
                }

                // 목록에 이미 있지 않으면
                if (changedList.Any(point => point.X == tileX && point.Y == tileY) == false)
                {
                    // 추가
                    m_changedTileList[targetUserName].Add(new Point(tileX, tileY));
                }
            }
        }

        protected void SyncChangedTiles(ServerVisitor client, string userName, Rectangle screen)
        {
            if (m_changedTileList.ContainsKey(userName))
            {
                List<Point> changedList = m_changedTileList[userName];

                List<Point> notSyncList = new List<Point>();


                NetMessageStream writer = null;
                int writeCount = 0;

                foreach (Point index in changedList)
                {
                    // 화면 영역 밖이면 동기화 안함.
                    if (index.X < screen.Left || index.X > screen.Right
                        || index.Y < screen.Top || index.Y > screen.Bottom)
                    {
                        notSyncList.Add(index);

                        continue;
                    }


                    var tile = this.GameBoard.Board.GetItemAt(index.X, index.Y);

                    if (tile != null)
                    {
                        if (writer == null)
                            writer = new NetMessageStream();


                        writer.WriteData(index.X);
                        writer.WriteData(index.Y);

                        tile.WriteToStream(writer);


                        ++writeCount;

                        if (writeCount >= 32)
                        {
                            writeCount = 0;

                            client.Sender.SendMessage(writer.CreateMessage((int)MessageTypes.Rsp_SetScreen));

                            writer = null;
                        }
                    }
                }

                if (writer != null)
                {
                    client.Sender.SendMessage(writer.CreateMessage((int)MessageTypes.Rsp_SetScreen));
                }


                changedList.Clear();
                changedList.AddRange(notSyncList);
            }
        }

        protected void NoticeToUsersInVision(NetMessage msg, int tileX, int tileY)
        {
            var usersInVision = this.GameBoard.GetUsersInVision(tileX, tileY);

            if (usersInVision != null)
            {
                List<string> userContainInScreen = new List<string>();

                for (int i = 0; i < usersInVision.Length; ++i)
                {
                    if (this.CheckTileInScreen(tileX, tileY, usersInVision[i]))
                    {
                        userContainInScreen.Add(usersInVision[i]);
                    }
                    else
                    {
                        this.AddIndexForSync(tileX, tileY, usersInVision[i]);
                    }
                }

                this.NoticeWhereDelegate(userContainInScreen.ToArray(), msg);
            }
        }

        protected void NtfSetPower(int tileX, int tileY, int newPower)
        {
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(tileX);
            writer.WriteData(tileY);
            writer.WriteData(newPower);

            this.NoticeToUsersInVision(writer.CreateMessage((int)MessageTypes.Ntf_SetTilePower),
                tileX, tileY);
        }

        //#####################################################################################
        // 수신된 메세지 처리

        private NetMessage WhenReqCountryLocation(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();


            // 국가 위치 찾기
            int x, y;
            bool exist = this.GameBoard.FindCountryLocation(userName, out x, out y);


            // 국가 위치 전송
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData<int>(exist ? 1 : 0);
            writer.WriteData(x);
            writer.WriteData(y);

            return writer.CreateMessage((int)MessageTypes.Rsp_CountryLocation);
        }

        private NetMessage WhenReqNewTerritory(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();


            // 적당한 영토 고름
            int tileX, tileY;
            this.GameBoard.CreateRandomTerritory(out tileX, out tileY);


            // 영토 위치 전송
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(tileX);
            writer.WriteData(tileY);

            return writer.CreateMessage((int)MessageTypes.Rsp_NewTerritory);
        }

        private NetMessage WhenReqAddTilePower(ServerVisitor client, NetMessageStream msg)
        {
            // 클릭속도 제한
            m_addPowerLimiter.Set(client.ID, GameValues.MinAddPowerDelay);
            if (m_addPowerLimiter.Tick(client.ID))
            {
                m_addPowerLimiter.Update(client.ID);


                string userName = msg.ReadData<string>();
                int tileX = msg.ReadData<int>();
                int tileY = msg.ReadData<int>();
                const int deltaPower = 1;


                // 클라이언트가 userName을 변조해서 보낸것인지 확인한다음 문제없고
                // 요청받은 타일이 서버에 존재하면
                var user = this.UserDirector.GetLoginUser(client.ID);
                if (user != null && user.Name == userName
                    && this.GameBoard.Board.ContainsItemAt(tileX, tileY))
                {
                    var tile = this.GameBoard.Board.GetItemAt(tileX, tileY);

                    // 유저의 소유 타일이 맞으면
                    if (tile.Owner == userName)
                    {
                        // GameBoard 갱신
                        this.GameBoard.AddPowerAt(tileX, tileY, deltaPower);


                        // 타일 Power 변경 통보
                        this.NtfSetPower(tileX, tileY, tile.Power);
                    }
                }
            }


            return null;
        }

        private NetMessage WhenReqChunk(ServerVisitor client, NetMessageStream msg)
        {
            int tileX = msg.ReadData<int>();
            int tileY = msg.ReadData<int>();


            int chunkX, chunkY;
            this.GameBoard.Board.GetChunkPosContainsItemAt(tileX, tileY, out chunkX, out chunkY);


            var chunk = this.GameBoard.Board.GetChunkContainsItemAt(tileX, tileY);

            if (chunk != null)
            {
                // 청크 데이터 얻기
                NetMessageStream writer = new NetMessageStream();

                writer.WriteData(chunkX);
                writer.WriteData(chunkY);

                for (int x = 0; x < chunk.GetLength(0); ++x)
                {
                    for (int y = 0; y < chunk.GetLength(1); ++y)
                    {
                        var tile = chunk[x, y];

                        tile.WriteToStream(writer);
                    }
                }


                // 청크 데이터 전송
                return writer.CreateMessage((int)MessageTypes.Rsp_Chunk);
            }


            return null;
        }

        private NetMessage WhenReqBuildCountry(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            int tileX = msg.ReadData<int>();
            int tileY = msg.ReadData<int>();

            const int beginningMoney = 100;
            

            // 로그
            Utility.Logger.GetInstance().Log(string.Format("\"{0}\"님이 건국을 요청했습니다.",
                userName));


            // 유저 인증하고
            // 해당 지역이 있고
            // 유저가 아무런 땅도 가지지 않은게 맞으면
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName
                && this.GameBoard.Board.ContainsItemAt(tileX, tileY)
                && this.UserDirector.GetAccount(userName).AreaCount <= 0)
            {
                var tile = this.GameBoard.Board.GetItemAt(tileX, tileY);

                // 주인없는 땅이면
                if (tile.HaveOwner == false)
                {
                    // 건국
                    this.GameBoard.SetOwnerAt(tileX, tileY, userName);
                    this.GameBoard.SetPowerAt(tileX, tileY, beginningMoney);


                    // 계정의 영토크기 정보 갱신
                    this.UserDirector.GetAccount(userName).AreaCount = 1;


                    // 건국 알림
                    NetMessageStream writer = new NetMessageStream();
                    writer.WriteData<int>(1); // 성공여부
                    writer.WriteData(userName);
                    writer.WriteData(tileX);
                    writer.WriteData(tileY);
                    writer.WriteData(beginningMoney);

                    this.NoticeDelegate(writer.CreateMessage((int)MessageTypes.Ntf_CountryBuilt));


                    return null;
                }
            }


            // 건국 실패 알림
            NetMessageStream failWriter = new NetMessageStream();
            failWriter.WriteData<int>(0);

            return failWriter.CreateMessage((int)MessageTypes.Ntf_CountryBuilt);
        }

        private NetMessage WhenReqAttackTerritory(ServerVisitor client, NetMessageStream msg)
        {
            // 클릭속도 제한
            m_attackLimiter.Set(client.ID, GameValues.MinAttackDelay);
            if (m_attackLimiter.Tick(client.ID))
            {
                m_attackLimiter.Update(client.ID);


                string attackerName = msg.ReadData<string>();
                int tileX = msg.ReadData<int>();
                int tileY = msg.ReadData<int>();


                // 유저 인증하고
                // 해당 지역이 있으면
                var user = this.UserDirector.GetLoginUser(client.ID);
                if (user != null && user.Name == attackerName
                    && this.GameBoard.Board.ContainsItemAt(tileX, tileY))
                {
                    // 공격/점령
                    this.AttackTerritory(attackerName, tileX, tileY, client);
                }
            }


            return null;
        }

        private NetMessage WhenReqAllTerritory(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();


            // 로그
            Utility.Logger.GetInstance().Log(string.Format("\"{0}\"님이 자신의 전체 영토정보를 요청했습니다.",
                userName));


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                foreach (var chunkInfo in this.GameBoard.Board.GetEnumerable())
                {
                    var chunk = chunkInfo.Chunk;


                    if (chunk != null)
                    {
                        NetMessageStream writer = null;
                        int writeCount = 0;


                        // 유저 타일의 정보
                        for (int x = 0; x < chunk.GetLength(0); ++x)
                        {
                            for (int y = 0; y < chunk.GetLength(1); ++y)
                            {
                                var tile = chunk[x, y];

                                if (tile.Owner == userName)
                                {
                                    if (writer == null)
                                    {
                                        writer = new NetMessageStream();

                                        // 청크 위치
                                        writer.WriteData(chunkInfo.X);
                                        writer.WriteData(chunkInfo.Y);
                                    }


                                    writer.WriteData(x);
                                    writer.WriteData(y);

                                    // NOTE: 주인 이름은 필요없다. 왜냐하면 이미 '자신'의 영토 정보라는걸 아니까.
                                    tile.WriteToStreamExceptOwner(writer);

                                    ++writeCount;
                                    if (writeCount >= 16)
                                    {
                                        writeCount = 0;

                                        // 전송
                                        client.Sender.SendMessage(writer.CreateMessage((int)MessageTypes.Rsp_AllTerritory));

                                        writer = null;
                                    }
                                }
                            }
                        }


                        // 남은 데이터 전송
                        if (writer != null)
                        {
                            client.Sender.SendMessage(writer.CreateMessage((int)MessageTypes.Rsp_AllTerritory));
                        }
                    }
                }
            }


            return null;
        }

        private NetMessage WhenReqAllVision(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();


            int chunkSize = this.GameBoard.Board.ChunkSize;


            // 로그
            Utility.Logger.GetInstance().Log(string.Format("\"{0}\"님이 자신의 전체 시야정보를 요청했습니다.",
                userName));


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                // 유저 영토 얻기
                List<Point> userTileList = new List<Point>();

                foreach (var chunkInfo in this.GameBoard.Board.GetEnumerable())
                {
                    var chunk = chunkInfo.Chunk;
                    int chunkPosX = chunkInfo.X * chunkSize;
                    int chunkPosY = chunkInfo.Y * chunkSize;


                    for (int x = 0; x < chunk.GetLength(0); ++x)
                    {
                        for (int y = 0; y < chunk.GetLength(1); ++y)
                        {
                            var tile = chunk[x, y];

                            if (tile.Owner == userName)
                            {
                                userTileList.Add(new Point(x + chunkPosX, y + chunkPosY));
                            }
                        }
                    }
                }


                // 시야 얻기
                Dictionary<int, Dictionary<int, Tile>> visionTileMap = new Dictionary<int, Dictionary<int, Tile>>();

                foreach (var pos in userTileList)
                {
                    int visionLength = GameValues.VisionLength;
                    int beginX = pos.X - visionLength;
                    int beginY = pos.Y - visionLength;
                    int endX = beginX + visionLength * 2;
                    int endY = beginY + visionLength * 2;


                    for (int x = beginX; x <= endX; ++x)
                    {
                        for (int y = beginY; y <= endY; ++y)
                        {
                            var tile = this.GameBoard.Board.GetItemAt(x, y);

                            if (tile.Owner != userName)
                            {
                                if (visionTileMap.ContainsKey(x) == false)
                                    visionTileMap.Add(x, new Dictionary<int, Tile>());

                                var vertical = visionTileMap[x];
                                if (vertical.ContainsKey(y) == false)
                                    vertical.Add(y, tile);
                            }
                        }
                    }
                }


                // 시야 전송
                NetMessageStream writer = null;
                int writeCount = 0;

                foreach (var vertical in visionTileMap)
                {
                    int x = vertical.Key;

                    foreach (var y_tile in vertical.Value)
                    {
                        int y = y_tile.Key;
                        var tile = y_tile.Value;


                        if (writer == null)
                            writer = new NetMessageStream();


                        writer.WriteData(x);
                        writer.WriteData(y);

                        tile.WriteToStream(writer);


                        // 한 세트 다 채웠으면 전송
                        ++writeCount;

                        if (writeCount >= 16)
                        {
                            writeCount = 0;

                            client.Sender.SendMessage(writer.CreateMessage((int)MessageTypes.Rsp_AllVision));

                            writer = null;
                        }
                    }
                }

                // 남은 시야 데이터 전송
                if (writer != null)
                {
                    return writer.CreateMessage((int)MessageTypes.Rsp_AllVision);
                }
            }


            return null;
        }

        private NetMessage WhenReqSendPower(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            int fromX = msg.ReadData<int>();
            int fromY = msg.ReadData<int>();
            int toX = msg.ReadData<int>();
            int toY = msg.ReadData<int>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                // 힘 보내기
                this.SendPower(user.Name, fromX, fromY, toX, toY);
            }


            return null;
        }

        private NetMessage WhenReqEditTileSign(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            int tileX = msg.ReadData<int>();
            int tileY = msg.ReadData<int>();
            string sign = msg.ReadData<string>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                this.EditSign(userName, tileX, tileY, sign);
            }


            return null;
        }

        private NetMessage WhenReqBuildFactory(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            int tileX = msg.ReadData<int>();
            int tileY = msg.ReadData<int>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                // 해당 타일이 존재하고
                if (this.GameBoard.Board.ContainsItemAt(tileX, tileY))
                {
                    var tile = this.GameBoard.Board.GetItemAt(tileX, tileY);

                    // 타일의 주인이 맞고
                    // 자원타일이면
                    if (tile.Owner == userName
                        && tile.IsResourceTile)
                    {
                        // 공장 세우기
                        this.GameBoard.BuildFactory(tileX, tileY);


                        // 공장 생성 알림
                        NetMessageStream writer = new NetMessageStream();
                        writer.WriteData(userName);
                        writer.WriteData(tileX);
                        writer.WriteData(tileY);

                        this.NoticeToUsersInVision(writer.CreateMessage((int)MessageTypes.Ntf_BuildFactory),
                            tileX, tileY);
                    }
                }
            }


            return null;
        }

        private NetMessage WhenReqConvertAllResource(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            int tileX = msg.ReadData<int>();
            int tileY = msg.ReadData<int>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                this.ConvertResource(user.Name, tileX, tileY, true);
            }


            return null;
        }

        private NetMessage WhenReqDestroyFactory(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            int tileX = msg.ReadData<int>();
            int tileY = msg.ReadData<int>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                // 해당 타일이 존재하고
                if (this.GameBoard.Board.ContainsItemAt(tileX, tileY))
                {
                    var tile = this.GameBoard.Board.GetItemAt(tileX, tileY);

                    // 타일의 주인이 맞고
                    // 공장타일이면
                    if (tile.Owner == userName
                        && tile.IsFactoryTile)
                    {
                        // 공장 폐쇄
                        this.GameBoard.DestroyFactory(tileX, tileY);


                        // 공장 폐쇄 알림
                        NetMessageStream writer = new NetMessageStream();
                        writer.WriteData(userName);
                        writer.WriteData(tileX);
                        writer.WriteData(tileY);

                        this.NoticeToUsersInVision(writer.CreateMessage((int)MessageTypes.Ntf_DestroyFactory),
                            tileX, tileY);
                    }
                }
            }


            return null;
        }

        private NetMessage WhenReqBuildChip(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            int tileX = msg.ReadData<int>();
            int tileY = msg.ReadData<int>();
            string companyName = msg.ReadData<string>();
            int productIndex = msg.ReadData<int>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                user = this.UserDirector.GetAccount(user.Name);
                var company = this.CompanyDirector.FindCompanyByName(companyName);


                // 유저가 존재하고
                // 존재하는 회사가 유저의 소유이고
                // 제품번호가 유효하고
                // 해당 타일이 존재하고
                if (user != null
                    && company != null && company.Owner == user.Name
                    && productIndex >= 0 && productIndex < company.ProductList.Count
                    && this.GameBoard.Board.ContainsItemAt(tileX, tileY))
                {
                    var tile = this.GameBoard.Board.GetItemAt(tileX, tileY);
                    var chip = company.ProductList[productIndex];

                    // 타일의 주인이 맞고
                    // 일반 타일이고
                    // 설치할 칩이 존재하면
                    if (tile.Owner == userName
                        && tile.IsNormalTile
                        && chip != null)
                    {
                        // 칩 설치
                        this.CompanyDirector.AddChip(tileX, tileY, chip.Clone());
                        this.GameBoard.BuildChip(tileX, tileY);


                        // 제품 목록에서 제거
                        company.RemoveProductAt(productIndex);

                        // 제거 알림
                        client.Sender.SendMessage(this.CompanyDirector.GetDiscardProductNotice(companyName, productIndex));


                        // 칩 설치 알림
                        NetMessageStream writer = new NetMessageStream();
                        writer.WriteData(tileX);
                        writer.WriteData(tileY);

                        this.NoticeToUsersInVision(writer.CreateMessage((int)MessageTypes.Ntf_BuildChip),
                            tileX, tileY);
                    }
                }
            }


            return null;
        }

        private NetMessage WhenReqDestroyChip(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            int tileX = msg.ReadData<int>();
            int tileY = msg.ReadData<int>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                // 해당 타일이 존재하고
                if (this.GameBoard.Board.ContainsItemAt(tileX, tileY))
                {
                    var tile = this.GameBoard.Board.GetItemAt(tileX, tileY);

                    // 타일의 주인이 맞고
                    // 칩 타일이면
                    if (tile.Owner == userName
                        && tile.IsChipTile)
                    {
                        // 칩 제거
                        this.CompanyDirector.RemoveChip(tileX, tileY);
                        this.GameBoard.DestroyChip(tileX, tileY);


                        // 칩 제거 알림
                        NetMessageStream writer = new NetMessageStream();
                        writer.WriteData(tileX);
                        writer.WriteData(tileY);

                        this.NoticeToUsersInVision(writer.CreateMessage((int)MessageTypes.Ntf_DestroyChip),
                            tileX, tileY);
                    }
                }
            }


            return null;
        }

        private NetMessage WhenReqSetScreen(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            int left = msg.ReadData<int>();
            int top = msg.ReadData<int>();
            int width = msg.ReadData<int>();
            int height = msg.ReadData<int>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                Rectangle rect = new Rectangle(left, top, width, height);

                if (m_userScreen.ContainsKey(user.Name) == false)
                    m_userScreen.Add(user.Name, new Rectangle(0, 0, 0, 0));

                // 이전 화면영역 정보와 다르면
                if (m_userScreen[user.Name] != rect)
                {
                    // 새로 보이는 곳의 정보 전달
                    this.SyncChangedTiles(client, user.Name, rect);


                    // 갱신
                    m_userScreen[user.Name] = rect;
                }
            }


            return null;
        }

        //#####################################################################################
        // 게임판 작업

        protected ServerVisitor GetClientByName(string name)
        {
            int clientID = this.UserDirector.GetLoginClientID(name);
            return this.FindClientByIDDelegate(clientID);
        }

        public void AttackTerritory(string attackerName, int tileX, int tileY, ServerVisitor client = null)
        {
            if (client == null)
            {
                client = this.GetClientByName(attackerName);
            }


            var targetTile = this.GameBoard.Board.GetItemAt(tileX, tileY);
            string currentOwner = targetTile.Owner;

            // 공격
            bool captureSuccess;
            string oldOwner;
            var changedTiles = this.GameBoard.AttackTerritory(attackerName, tileX, tileY,
                out captureSuccess, out oldOwner);

            // 공격으로 변경된 사항이 있으면 모두에게 알림.
            if (changedTiles != null)
            {
                NetMessageStream writer = new NetMessageStream();
                writer.WriteData(attackerName); // 공격자
                writer.WriteData(tileX);
                writer.WriteData(tileY);

                writer.WriteData(currentOwner); // 피해자

                writer.WriteData(changedTiles.Length); // 변경된 타일 개수

                foreach (var tilePos in changedTiles)
                {
                    var tile = this.GameBoard.Board.GetItemAt(tilePos.X, tilePos.Y);

                    writer.WriteData(tilePos.X);
                    writer.WriteData(tilePos.Y);

                    tile.WriteToStream(writer);
                }

                this.NoticeDelegate(writer.CreateMessage((int)MessageTypes.Ntf_AttackTerritory));
            }


            if (captureSuccess)
            {
                // 점령된 타일에 회사가 존재하면
                if (targetTile != null && targetTile.IsCompanyTile)
                {
                    // 회사 파괴
                    this.CompanyDirector.DestroyCompany(attackerName, tileX, tileY);
                }


                // 피의자와 피해자의 영토 개수에 변화가 생겼으므로 갱신
                this.UserDirector.GetAccount(attackerName).AreaCount += 1;

                if (oldOwner.Length > 0)
                {
                    var victimUser = this.UserDirector.GetAccount(oldOwner);

                    victimUser.AreaCount -= 1;

                    // 피해자가 멸망했으면
                    if (victimUser.AreaCount <= 0)
                    {
                        // 멸망한 사실을 모두에게 알림
                        NetMessageStream userEndMsg = new NetMessageStream();
                        userEndMsg.WriteData(victimUser.Name);
                        userEndMsg.WriteData(tileX);
                        userEndMsg.WriteData(tileY);

                        this.NoticeDelegate(userEndMsg.CreateMessage((int)MessageTypes.Ntf_UserEnd));
                    }
                }


                if (client != null)
                {
                    // 점령에 성공했다는건 땅이 확장되었다는 뜻이므로
                    // 시야에 해당하는 영토의 정보 전송.
                    int visionLength = GameValues.VisionLength;
                    int beginX = tileX - visionLength;
                    int beginY = tileY - visionLength;
                    int endX = beginX + visionLength * 2;
                    int endY = beginY + visionLength * 2;


                    NetMessageStream writer = new NetMessageStream();

                    writer.WriteData(tileX);
                    writer.WriteData(tileY);
                    writer.WriteData(visionLength);

                    Dictionary<string, int> userNameTable = new Dictionary<string, int>();
                    List<string> userIndexTable = new List<string>();

                    // 시야내의 유저 이름을 테이블화
                    for (int x = beginX; x <= endX; ++x)
                    {
                        for (int y = beginY; y <= endY; ++y)
                        {
                            // NOTE: GetItemAt을 함으로서 존재하지 않는 청크를 생성하여 맵을 확장하는 효과도 있음.
                            var tile = this.GameBoard.Board.GetItemAt(x, y);

                            if (userNameTable.ContainsKey(tile.Owner) == false)
                            {
                                int index = userIndexTable.Count;

                                userNameTable.Add(tile.Owner, index);
                                userIndexTable.Add(tile.Owner);
                            }
                        }
                    }

                    // 유저 테이블을 메세지에 기록
                    writer.WriteData(userIndexTable.Count);

                    for (int i = 0; i < userIndexTable.Count; ++i)
                    {
                        writer.WriteData(userIndexTable[i]);
                    }

                    // 유저 이름을 제외한 타일 정보를 메세지에 기록
                    for (int x = beginX; x <= endX; ++x)
                    {
                        for (int y = beginY; y <= endY; ++y)
                        {
                            var tile = this.GameBoard.Board.GetItemAt(x, y);

                            writer.WriteData(userNameTable[tile.Owner]);

                            tile.WriteToStreamExceptOwner(writer);
                        }
                    }

                    client.Sender.SendMessage(writer.CreateMessage((int)MessageTypes.Rsp_Vision));
                }
            }
        }

        public void SendPower(string userName, int fromX, int fromY, int toX, int toY)
        {
            Point fromTilePos = new Point(fromX, fromY);
            Point toTilePos = new Point(toX, toY);

            // 유효한 힘 보내기라면
            if (this.GameBoard.CheckCanSendPower(userName,
                fromTilePos, toTilePos))
            {
                // 힘 전달
                int sendingPower = this.GameBoard.SendPower(fromTilePos, toTilePos);


                var fromTile = this.GameBoard.Board.GetItemAt(fromX, fromY);
                var toTile = this.GameBoard.Board.GetItemAt(toX, toY);


                // 변경사항 알림
                NetMessageStream writer = new NetMessageStream();
                writer.WriteData(sendingPower);
                writer.WriteData(fromX);
                writer.WriteData(fromY);
                writer.WriteData(fromTile.Power);
                writer.WriteData(fromTile.Owner);
                writer.WriteData(toX);
                writer.WriteData(toY);
                writer.WriteData(toTile.Power);
                writer.WriteData(toTile.Owner);
                
                this.NoticeDelegate(writer.CreateMessage((int)MessageTypes.Ntf_SendPower));
            }
        }

        public void InterruptChip(int targetX, int targetY, string varName, string data)
        {
            Chip chip = this.CompanyDirector.GetChipAt(targetX, targetY);

            if (chip != null)
            {
                chip.Interrupt(varName, data);
            }
        }

        public void EditSign(string userName, int tileX, int tileY, string sign)
        {
            // 금지문자 대체
            sign = sign.Replace('\\', '/');


            // 해당 타일이 존재하고
            // 요청자의 타일이고
            // 설정할 푯말이 제한 길이를 넘지 않으면
            if (this.GameBoard.Board.ContainsItemAt(tileX, tileY)
                && this.GameBoard.GetOwnerAt(tileX, tileY) == userName
                && sign.Length <= GameValues.MaxSignLength)
            {
                // 푯말을 설정하고
                this.GameBoard.SetSignAt(tileX, tileY, sign);


                // 변경사항을 모두에게 알림
                NetMessageStream writer = new NetMessageStream();
                writer.WriteData(userName);
                writer.WriteData(tileX);
                writer.WriteData(tileY);
                writer.WriteData(sign);

                this.NoticeDelegate(writer.CreateMessage((int)MessageTypes.Ntf_EditTileSign));
            }
        }

        public void ConvertResource(string userName, int tileX, int tileY, bool bAll, int useSrc = 0)
        {
            var user = this.UserDirector.GetAccount(userName);


            // 변환하려는 수치가 유효하고
            // 플레이어가 충분한 자원을 가지고 있고
            // 해당 타일이 존재하고
            if (useSrc >= 0
                && user.Resource >= useSrc
                && this.GameBoard.Board.ContainsItemAt(tileX, tileY))
            {
                var tile = this.GameBoard.Board.GetItemAt(tileX, tileY);

                // 타일의 주인이 맞으면
                if (tile.Owner == userName)
                {
                    if (bAll)
                        useSrc = user.Resource;

                    // 자원 변환
                    int powerGet = useSrc * GameValues.ResourcePowerRate;
                    this.GameBoard.AddPowerAt(tileX, tileY, powerGet);
                    user.Resource -= useSrc;


                    // 자원 변환 알림
                    this.NtfSetPower(tileX, tileY, tile.Power);

                    NetMessageStream writer = new NetMessageStream();
                    writer.WriteData(userName);
                    writer.WriteData(tileX);
                    writer.WriteData(tileY);
                    writer.WriteData(powerGet);

                    this.NoticeDelegate(writer.CreateMessage((int)MessageTypes.Ntf_ConvertAllResource));
                }
            }
        }

        //#####################################################################################

        public Rectangle GetUserScreen(string userName)
        {
            if (m_userScreen.ContainsKey(userName))
                return m_userScreen[userName];


            return Rectangle.Empty;
        }
    }
}
