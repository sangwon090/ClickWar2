using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickWar2.Network;
using ClickWar2.Network.IO;
using ClickWar2.Game;
using ClickWar2.Game.Event;

namespace ClickWar2.Game.Network.ClientWorker
{
    public class GameBoardManager
    {
        public GameBoardManager()
        {

        }

        //#####################################################################################

        public NetClient Client
        { get; set; }

        public GameBoard GameBoard
        { get; set; }

        public SignManager SignDirector
        { get; set; }

        //#####################################################################################
        // 이벤트 관리자

        public GameEventList EventDirector
        { get; set; } = new GameEventList();

        //#####################################################################################
        // 메세지 수신 콜백

        protected Action<bool, int, int> m_requestCountryLocationCallback = null;
        protected Action<int, int> m_requestNewTerritoryCallback = null;
        protected Action<int, int, int> m_addTilePowerCallback = null;
        protected Action<bool, int, int> m_buildCountryCallback = null;
        protected Action<string, int, int> m_attackCallback = null;

        //#####################################################################################
        // 메세지 처리자 등록

        public void InitEventChain(NetClientProcedure procList)
        {
            procList.Set(this.WhenRspCountryLocation, (int)MessageTypes.Rsp_CountryLocation);
            procList.Set(this.WhenRspNewTerritory, (int)MessageTypes.Rsp_NewTerritory);
            procList.Set(this.WhenNtfSetTilePower, (int)MessageTypes.Ntf_SetTilePower);
            procList.Set(this.WhenRspUpdateChunk, (int)MessageTypes.Rsp_Chunk);
            procList.Set(this.WhenNtfCountryBuilt, (int)MessageTypes.Ntf_CountryBuilt);
            procList.Set(this.WhenNtfAttackTerritory, (int)MessageTypes.Ntf_AttackTerritory);
            procList.Set(this.WhenRspVision, (int)MessageTypes.Rsp_Vision);
            procList.Set(this.WhenRspAllTerritory, (int)MessageTypes.Rsp_AllTerritory);
            procList.Set(this.WhenRspAllVision, (int)MessageTypes.Rsp_AllVision);
            procList.Set(this.WhenNtfSendPower, (int)MessageTypes.Ntf_SendPower);
            procList.Set(this.WhenNtfEditTileSign, (int)MessageTypes.Ntf_EditTileSign);
            procList.Set(this.WhenNtfUserEnd, (int)MessageTypes.Ntf_UserEnd);
            procList.Set(this.WhenNtfBuildFactory, (int)MessageTypes.Ntf_BuildFactory);
            procList.Set(this.WhenNtfConvertAllResource, (int)MessageTypes.Ntf_ConvertAllResource);
            procList.Set(this.WhenNtfDestroyFactory, (int)MessageTypes.Ntf_DestroyFactory);
            procList.Set(this.WhenNtfBuildChip, (int)MessageTypes.Ntf_BuildChip);
            procList.Set(this.WhenNtfDestroyChip, (int)MessageTypes.Ntf_DestroyChip);
            procList.Set(this.WhenRspSetScreen, (int)MessageTypes.Rsp_SetScreen);
        }

        //#####################################################################################
        // 수신된 메세지 처리

        private void WhenRspCountryLocation(NetMessageStream msg)
        {
            int exist = msg.ReadInt32();
            int x = msg.ReadInt32();
            int y = msg.ReadInt32();


            // 국가 위치 알림
            if (m_requestCountryLocationCallback != null)
            {
                m_requestCountryLocationCallback((exist != 0), x, y);
                m_requestCountryLocationCallback = null;
            }
        }

        private void WhenRspNewTerritory(NetMessageStream msg)
        {
            int tileX = msg.ReadInt32();
            int tileY = msg.ReadInt32();


            // 새 영토 위치 알림
            if (m_requestNewTerritoryCallback != null)
            {
                m_requestNewTerritoryCallback(tileX, tileY);
                m_requestNewTerritoryCallback = null;
            }
        }

        private void WhenNtfSetTilePower(NetMessageStream msg)
        {
            int tileX = msg.ReadInt32();
            int tileY = msg.ReadInt32();
            int newPower = msg.ReadInt32();


            // 클라측 보드에 존재하는 지역이고 시야 내의 타일이면
            if (this.GameBoard.TileExistsAndVisibleAt(tileX, tileY))
            {
                // 힘 동기화
                this.GameBoard.SetPowerAt(tileX, tileY, newPower);
            }


            // 타일 Power 변경 알림
            if (m_addTilePowerCallback != null)
            {
                m_addTilePowerCallback(tileX, tileY, newPower);
                m_addTilePowerCallback = null;
            }


            // 이벤트 발생
            this.EventDirector.StartEvent((int)BoardEvents.PowerUpTerritory,
                tileX, tileY, "");
        }

        private void WhenRspUpdateChunk(NetMessageStream msg)
        {
            int chunkX = msg.ReadInt32();
            int chunkY = msg.ReadInt32();


            // 청크 생성
            this.GameBoard.Board.CreateChunkAt(chunkX, chunkY);


            // 청크 데이터 얻기
            int chunkSize = this.GameBoard.Board.ChunkSize;

            for (int x = 0; x < chunkSize; ++x)
            {
                int tileX = chunkX * chunkSize + x;


                for (int y = 0; y < chunkSize; ++y)
                {
                    int tileY = chunkY * chunkSize + y;


                    var tile = this.GameBoard.Board.GetItemAt(tileX, tileY);

                    tile.ReadFromStream(msg);

                    tile.Visible = true;
                }
            }
        }

        private void WhenNtfCountryBuilt(NetMessageStream msg)
        {
            bool success = (msg.ReadInt32() != 0);

            if (success)
            {
                string userName = msg.ReadString();
                int tileX = msg.ReadInt32();
                int tileY = msg.ReadInt32();
                int beginningMoney = msg.ReadInt32();


                // 클라측 보드에 존재하는 지역이고 시야 내의 타일이면
                if (this.GameBoard.TileExistsAndVisibleAt(tileX, tileY))
                {
                    // 동기화
                    this.GameBoard.SetOwnerAt(tileX, tileY, userName);
                    this.GameBoard.SetPowerAt(tileX, tileY, beginningMoney);
                }


                // 자신의 건국인데
                if (this.SignDirector.LoginName == userName)
                {
                    // 보드에 해당 지역이 존재하지 않으면
                    if (!this.GameBoard.Board.ContainsItemAt(tileX, tileY))
                    {
                        // 해당 지역의 정보를 요청
                        this.UpdateChunkContainsTileAt(tileX, tileY);
                    }


                    // 건국 알림
                    if (m_buildCountryCallback != null)
                    {
                        m_buildCountryCallback(success, tileX, tileY);
                        m_buildCountryCallback = null;
                    }
                }


                // 이벤트 발생
                this.EventDirector.StartEvent((int)BoardEvents.BuildCountry,
                    tileX, tileY, userName);
            }
            else
            {
                // 건국 실패 알림
                if (m_buildCountryCallback != null)
                {
                    m_buildCountryCallback(success, 0, 0);
                    m_buildCountryCallback = null;
                }
            }
        }

        private void WhenNtfAttackTerritory(NetMessageStream msg)
        {
            string attackerName = msg.ReadString();
            int targetX = msg.ReadInt32();
            int targetY = msg.ReadInt32();

            string victimName = msg.ReadString();

            int changedCount = msg.ReadInt32();

            for (int i = 0; i < changedCount; ++i)
            {
                int tileX = msg.ReadInt32();
                int tileY = msg.ReadInt32();


                // NOTE: 무조건 읽도록 함.
                Tile tempTile = new Tile();
                tempTile.ReadFromStream(msg);


                // 클라측 보드에 존재하는 지역이고 시야 내의 타일이면
                if (this.GameBoard.TileExistsAndVisibleAt(tileX, tileY))
                {
                    // 동기화
                    var tile = this.GameBoard.Board.GetItemAt(tileX, tileY);
                    tile.FromTile(tempTile);
                }
            }


            if (m_attackCallback != null)
            {
                m_attackCallback(attackerName, targetX, targetY);
                m_attackCallback = null;
            }


            // 이벤트 발생
            this.EventDirector.StartEvent((int)BoardEvents.AttackTerritory,
                targetX, targetY, attackerName, new object[] { victimName });
        }

        private void WhenRspVision(NetMessageStream msg)
        {
            int centerX = msg.ReadInt32();
            int centerY = msg.ReadInt32();
            int visionLength = msg.ReadInt32();

            int userTableCount = msg.ReadInt32();
            List<string> userNameTable = new List<string>();
            for (int i = 0; i < userTableCount; ++i)
            {
                userNameTable.Add(msg.ReadString());
            }


            int beginX = centerX - visionLength;
            int beginY = centerY - visionLength;
            int endX = beginX + visionLength * 2;
            int endY = beginY + visionLength * 2;


            for (int x = beginX; x <= endX; ++x)
            {
                for (int y = beginY; y <= endY; ++y)
                {
                    var tile = this.GameBoard.Board.GetItemAt(x, y);


                    // 시야에 보이는 타일로 설정
                    tile.Visible = true;

                    // 주인 동기화
                    int userNameIndex = msg.ReadInt32();
                    if (userNameIndex < userNameTable.Count)
                        tile.Owner = userNameTable[userNameIndex];

                    // 나머지 정보 동기화
                    tile.ReadFromStreamExceptOwner(msg);
                }
            }
        }

        private void WhenRspAllTerritory(NetMessageStream msg)
        {
            int chunkX = msg.ReadInt32();
            int chunkY = msg.ReadInt32();

            int chunkPosX = chunkX * this.GameBoard.Board.ChunkSize;
            int chunkPosY = chunkY * this.GameBoard.Board.ChunkSize;
            

            while(!msg.EndOfStream)
            {
                int tileX = msg.ReadInt32();
                int tileY = msg.ReadInt32();


                tileX += chunkPosX;
                tileY += chunkPosY;

                // NOTE: 없는 타일이면 자동으로 생성됨.
                var tile = this.GameBoard.Board.GetItemAt(tileX, tileY);

                // 시야에 보이는 타일로 설정
                tile.Visible = true;

                // 동기화
                tile.Owner = this.SignDirector.LoginName;
                tile.ReadFromStreamExceptOwner(msg);
            }
        }

        private void WhenRspAllVision(NetMessageStream msg)
        {
            while (!msg.EndOfStream)
            {
                int tileX = msg.ReadInt32();
                int tileY = msg.ReadInt32();


                // 시야 타일 동기화
                var tile = this.GameBoard.Board.GetItemAt(tileX, tileY);

                // 동기화
                tile.ReadFromStream(msg);

                // 시야에 보이는 타일로 설정
                tile.Visible = true;
            }
        }

        private void WhenNtfSendPower(NetMessageStream msg)
        {
            int sendingPower = msg.ReadInt32();
            int fromX = msg.ReadInt32();
            int fromY = msg.ReadInt32();
            int fromTilesPower = msg.ReadInt32();
            string fromTilesOwner = msg.ReadString();
            int toX = msg.ReadInt32();
            int toY = msg.ReadInt32();
            int toTilesPower = msg.ReadInt32();
            string toTilesOwner = msg.ReadString();


            // 변경사항 동기화

            // 클라측 보드에 존재하는 지역이고 시야 내의 타일이면
            if (this.GameBoard.TileExistsAndVisibleAt(fromX, fromY))
            {
                // 동기화
                this.GameBoard.SetPowerAt(fromX, fromY, fromTilesPower);
            }

            // 클라측 보드에 존재하는 지역이고 시야 내의 타일이면
            if (this.GameBoard.TileExistsAndVisibleAt(toX, toY))
            {
                // 동기화
                this.GameBoard.SetPowerAt(toX, toY, toTilesPower);
            }


            // 이벤트 발생
            this.EventDirector.StartEvent((int)BoardEvents.SendPower,
                fromX, fromY, fromTilesOwner, new object[] { toTilesOwner, sendingPower });
        }

        private void WhenNtfEditTileSign(NetMessageStream msg)
        {
            string userName = msg.ReadString();
            int tileX = msg.ReadInt32();
            int tileY = msg.ReadInt32();
            string sign = msg.ReadString();


            // 변경사항 동기화

            // 클라측 보드에 존재하는 지역이고 시야 내의 타일이면
            if (this.GameBoard.TileExistsAndVisibleAt(tileX, tileY))
            {
                // 동기화
                this.GameBoard.SetSignAt(tileX, tileY, sign);
            }


            // 이벤트 발생
            this.EventDirector.StartEvent((int)BoardEvents.EditTileSign,
                tileX, tileY, userName, new object[] { sign });
        }

        private void WhenNtfUserEnd(NetMessageStream msg)
        {
            string endUserName = msg.ReadString();
            int tileX = msg.ReadInt32();
            int tileY = msg.ReadInt32();


            // 이벤트 발생
            this.EventDirector.StartEvent((int)BoardEvents.EndUser,
                tileX, tileY, endUserName);
        }

        private void WhenNtfBuildFactory(NetMessageStream msg)
        {
            string userName = msg.ReadString();
            int tileX = msg.ReadInt32();
            int tileY = msg.ReadInt32();


            // 공장 동기화

            // 클라측 보드에 존재하는 지역이고 시야 내의 타일이면
            if (this.GameBoard.TileExistsAndVisibleAt(tileX, tileY))
            {
                // 동기화
                this.GameBoard.BuildFactory(tileX, tileY);
            }
        }

        private void WhenNtfConvertAllResource(NetMessageStream msg)
        {
            string userName = msg.ReadString();
            int tileX = msg.ReadInt32();
            int tileY = msg.ReadInt32();
            int powerGet = msg.ReadInt32();


            // 이벤트 발생
            this.EventDirector.StartEvent((int)BoardEvents.ConvertAllResource,
                tileX, tileY, userName, new object[] { powerGet });
        }

        private void WhenNtfDestroyFactory(NetMessageStream msg)
        {
            string userName = msg.ReadString();
            int tileX = msg.ReadInt32();
            int tileY = msg.ReadInt32();


            // 공장 동기화

            // 클라측 보드에 존재하는 지역이고 시야 내의 타일이면
            if (this.GameBoard.TileExistsAndVisibleAt(tileX, tileY))
            {
                // 동기화
                this.GameBoard.DestroyFactory(tileX, tileY);
            }
        }

        private void WhenNtfBuildChip(NetMessageStream msg)
        {
            int tileX = msg.ReadInt32();
            int tileY = msg.ReadInt32();

            
            // 클라측 보드에 존재하는 지역이고 시야 내의 타일이면
            if (this.GameBoard.TileExistsAndVisibleAt(tileX, tileY))
            {
                // 동기화
                this.GameBoard.BuildChip(tileX, tileY);
            }
        }

        private void WhenNtfDestroyChip(NetMessageStream msg)
        {
            int tileX = msg.ReadInt32();
            int tileY = msg.ReadInt32();


            // 클라측 보드에 존재하는 지역이고 시야 내의 타일이면
            if (this.GameBoard.TileExistsAndVisibleAt(tileX, tileY))
            {
                // 동기화
                this.GameBoard.DestroyChip(tileX, tileY);
            }
        }

        private void WhenRspSetScreen(NetMessageStream msg)
        {
            while (!msg.EndOfStream)
            {
                int tileX = msg.ReadInt32();
                int tileY = msg.ReadInt32();

                var serverTile = new Tile();
                serverTile.ReadFromStream(msg);

                // 클라측 보드에 존재하는 지역이고 시야 내의 타일이면
                if (this.GameBoard.TileExistsAndVisibleAt(tileX, tileY))
                {
                    // 동기화
                    var tile = this.GameBoard.Board.GetItemAt(tileX, tileY);
                    tile.FromTile(serverTile);
                }
            }
        }

        //#####################################################################################
        // 사용자 입력 처리

        public void RequestCountryLocation(Action<bool, int, int> callbackAsync)
        {
            m_requestCountryLocationCallback = callbackAsync;


            // 플레이어 국가 위치 요청
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(this.SignDirector.LoginName);

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_CountryLocation));
        }

        public void RequestNewTerritory(Action<int, int> callbackAsync)
        {
            m_requestNewTerritoryCallback = callbackAsync;


            // 건국할 땅 요청
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(this.SignDirector.LoginName);

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_NewTerritory));
        }

        public void AddTilePower(int tileX, int tileY,
            Action<int, int, int> callbackAsync)
        {
            m_addTilePowerCallback = callbackAsync;


            // Power 변경 요청
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(this.SignDirector.LoginName);
            writer.WriteData(tileX);
            writer.WriteData(tileY);

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_AddTilePower));
        }

        public void UpdateChunkContainsTileAt(int tileX, int tileY)
        {
            // 해당 위치의 타일을 포함하는 청크 요청
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(tileX);
            writer.WriteData(tileY);

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_Chunk));
        }

        public void BuildCountry(int tileX, int tileY,
            Action<bool, int, int> callbackAsync)
        {
            // 현재 플레이어에게 보이는 영토만 선택가능
            if (this.GameBoard.Board.ContainsItemAt(tileX, tileY))
            {
                m_buildCountryCallback = callbackAsync;


                // 건국 요청
                NetMessageStream writer = new NetMessageStream();
                writer.WriteData(this.SignDirector.LoginName);
                writer.WriteData(tileX);
                writer.WriteData(tileY);

                this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_BuildCountry));
            }
            else
            {
                callbackAsync(false, tileX, tileY);
            }
        }

        public void AttackTerritory(int tileX, int tileY, Action<string, int, int> callbackAsync)
        {
            m_attackCallback = callbackAsync;


            // 현재 플레이어에게 보이는 영토만 선택가능
            if (this.GameBoard.Board.ContainsItemAt(tileX, tileY))
            {
                // 공격 가능한지 확인
                bool bCanAttack = false;

                int[] nearX = new int[]
                {
                    tileX, tileX + 1, tileX, tileX - 1
                };
                int[] nearY = new int[]
                {
                    tileY - 1, tileY, tileY + 1, tileY
                };

                for (int i = 0; i < 4; ++i)
                {
                    if (this.GameBoard.Board.ContainsItemAt(nearX[i], nearY[i]))
                    {
                        var nearTile = this.GameBoard.Board.GetItemAt(nearX[i], nearY[i]);

                        if (nearTile != null
                            && nearTile.Owner == this.SignDirector.LoginName)
                        {
                            bCanAttack = true;
                            break;
                        }
                    }
                }


                if (bCanAttack)
                {
                    // 공격/점령 요청
                    NetMessageStream writer = new NetMessageStream();
                    writer.WriteData(this.SignDirector.LoginName);
                    writer.WriteData(tileX);
                    writer.WriteData(tileY);

                    this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_AttackTerritory));
                }
            }
        }

        public void RequestMyTerritory()
        {
            // 공격/점령 요청
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(this.SignDirector.LoginName);

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_AllTerritory));
        }

        public void RequestMyVision()
        {
            // 시야 요청
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(this.SignDirector.LoginName);

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_AllVision));
        }

        public void SendTerritoryPower(int fromX, int fromY, int toX, int toY)
        {
            // 힘을 보낼 수 있으면
            if (this.GameBoard.CheckCanSendPower(this.SignDirector.LoginName,
                    new System.Drawing.Point(fromX, fromY),
                    new System.Drawing.Point(toX, toY)))
            {
                // 힘 전달 요청
                NetMessageStream writer = new NetMessageStream();
                writer.WriteData(this.SignDirector.LoginName);
                writer.WriteData(fromX);
                writer.WriteData(fromY);
                writer.WriteData(toX);
                writer.WriteData(toY);

                this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_SendPower));
            }
        }

        public void EditSign(int tileX, int tileY, string newText)
        {
            // 가르키는 타일이 존재하고
            // 자신의 타일이고
            // 현재 푯말과 다르면
            if (this.GameBoard.Board.ContainsItemAt(tileX, tileY)
                && this.GameBoard.GetOwnerAt(tileX, tileY) == this.SignDirector.LoginName
                && this.GameBoard.GetSignAt(tileX, tileY) != newText)
            {
                // 푯말 수정 요청
                NetMessageStream writer = new NetMessageStream();
                writer.WriteData(this.SignDirector.LoginName);
                writer.WriteData(tileX);
                writer.WriteData(tileY);
                writer.WriteData(newText);

                this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_EditTileSign));
            }
        }

        public void BuildFactory(int tileX, int tileY)
        {
            if (this.GameBoard.Board.ContainsItemAt(tileX, tileY))
            {
                var tile = this.GameBoard.Board.GetItemAt(tileX, tileY);

                // 타일의 주인이고
                // 타일이 자원 타일이면
                if (tile.Owner == this.SignDirector.LoginName
                    && tile.IsResourceTile)
                {
                    // 공장 생성 요청
                    NetMessageStream writer = new NetMessageStream();
                    writer.WriteData(this.SignDirector.LoginName);
                    writer.WriteData(tileX);
                    writer.WriteData(tileY);

                    this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_BuildFactory));
                }
            }
        }

        public void ConvertAllResource(int tileX, int tileY)
        {
            if (this.GameBoard.Board.ContainsItemAt(tileX, tileY))
            {
                var tile = this.GameBoard.Board.GetItemAt(tileX, tileY);

                // 타일의 주인이면
                if (tile.Owner == this.SignDirector.LoginName)
                {
                    // 자원 변환 요청
                    NetMessageStream writer = new NetMessageStream();
                    writer.WriteData(this.SignDirector.LoginName);
                    writer.WriteData(tileX);
                    writer.WriteData(tileY);

                    this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_ConvertAllResource));
                }
            }
        }

        public void DestroyFactory(int tileX, int tileY)
        {
            // 해당 타일이 존재하고
            if (this.GameBoard.Board.ContainsItemAt(tileX, tileY))
            {
                var tile = this.GameBoard.Board.GetItemAt(tileX, tileY);

                // 타일의 주인이고
                // 타일이 공장 타일이면
                if (tile.Owner == this.SignDirector.LoginName
                    && tile.IsFactoryTile)
                {
                    // 공장 폐쇄 요청
                    NetMessageStream writer = new NetMessageStream();
                    writer.WriteData(this.SignDirector.LoginName);
                    writer.WriteData(tileX);
                    writer.WriteData(tileY);

                    this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_DestroyFactory));
                }
            }
        }

        public void BuildChip(int tileX, int tileY, string companyName, int productIndex)
        {
            // 해당 타일이 존재하고
            if (this.GameBoard.Board.ContainsItemAt(tileX, tileY))
            {
                var tile = this.GameBoard.Board.GetItemAt(tileX, tileY);

                // 타일의 주인이고
                // 타일이 일반 타일이면
                if (tile.Owner == this.SignDirector.LoginName
                    && tile.IsNormalTile)
                {
                    // 제품 설치 요청
                    NetMessageStream writer = new NetMessageStream();
                    writer.WriteData(this.SignDirector.LoginName);
                    writer.WriteData(tileX);
                    writer.WriteData(tileY);
                    writer.WriteData(companyName);
                    writer.WriteData(productIndex);

                    this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_BuildChip));
                }
            }
        }

        public void DestroyChip(int tileX, int tileY)
        {
            // 해당 타일이 존재하고
            if (this.GameBoard.Board.ContainsItemAt(tileX, tileY))
            {
                var tile = this.GameBoard.Board.GetItemAt(tileX, tileY);

                // 타일의 주인이고
                // 타일이 칩(제품) 타일이면
                if (tile.Owner == this.SignDirector.LoginName
                    && tile.IsChipTile)
                {
                    // 제품 설치 요청
                    NetMessageStream writer = new NetMessageStream();
                    writer.WriteData(this.SignDirector.LoginName);
                    writer.WriteData(tileX);
                    writer.WriteData(tileY);

                    this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_DestroyChip));
                }
            }
        }

        public void UpdateMyScreen(int beginX, int beginY, int width, int height)
        {
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(this.SignDirector.LoginName);
            writer.WriteData(beginX);
            writer.WriteData(beginY);
            writer.WriteData(width);
            writer.WriteData(height);

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_SetScreen));
        }
    }
}
