using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using ClickWar2.Utility;

namespace ClickWar2.Game
{
    public class GameBoard
    {
        public GameBoard()
        {
            
        }

        //#####################################################################################

        protected ChunkBoard<Tile> m_board = new ChunkBoard<Tile>();
        public ChunkBoard<Tile> Board
        { get { return m_board; } }

        protected List<Point> m_factoryList = new List<Point>();
        public List<Point> Factories
        { get { return m_factoryList; } }

        protected List<Point> m_companyList = new List<Point>();
        public List<Point> Companies
        { get { return m_companyList; } }

        protected List<Point> m_chipList = new List<Point>();
        public List<Point> Chips
        { get { return m_chipList; } }

        //#####################################################################################

        protected string m_boardPath = "./";
        public string BoardPath
        {
            get { return m_boardPath; }
            set
            {
                if (value.Length > 0 && value[value.Length - 1] != '/'
                    && value[value.Length - 1] != '\\')
                {
                    throw new ArgumentException("BoardPath 마지막 문자는 \'/\'나 \'\\\'로 끝나야 합니다.");
                }

                Directory.CreateDirectory(value);

                m_boardPath = value;
            }
        }

        //#####################################################################################

        protected ChunkBoard<Tile> m_tempBoard = null;
        protected IEnumerator<ChunkInfo<Tile>> m_currentSaveChunk = null;

        protected IEnumerator<string> m_currentLoadChunk = null;
        
        //#####################################################################################

        public void SaveAll()
        {
            m_tempBoard = null;
            m_currentSaveChunk = null;

            while (this.SaveNext()) ;
        }

        /// <summary>
        /// 현재 선택된 청크를 저장하고 다음 청크를 선택한다.
        /// </summary>
        /// <returns>다음 청크의 유무</returns>
        public bool SaveNext()
        {
            // 저장할 청크가 없으면 바로 종료
            if (m_board.ChunkCount <= 0)
                return false;


            // 임시 보드에 실제 보드를 얕은 복사
            if (m_tempBoard == null)
            {
                m_tempBoard = new ChunkBoard<Tile>(m_board.ChunkSize);

                foreach (var chunkInformation in m_board.GetEnumerable())
                {
                    m_tempBoard.SetChunkAt(chunkInformation.X, chunkInformation.Y,
                        chunkInformation.Chunk);
                }
            }

            // 열거자 얻기
            if (m_currentSaveChunk == null)
            {
                m_currentSaveChunk = m_tempBoard.GetEnumerable().GetEnumerator();

                if (m_currentSaveChunk.MoveNext() == false)
                {
                    m_tempBoard.Clear();
                    m_tempBoard = null;
                    m_currentSaveChunk = null;
                    return false;
                }
            }


            // 열거자가 현재 가르키는 청크 저장
            var chunkInfo = m_currentSaveChunk.Current;
            var chunk = chunkInfo.Chunk;

            string fileName = string.Format("{0}_{1}.dat", chunkInfo.X, chunkInfo.Y);
            using (BinaryWriter bw = new BinaryWriter(new FileStream(this.BoardPath + fileName, FileMode.Create)))
            {
                // 파일 버전
                bw.Write(Application.ProductVersion.ToString());


                // 청크 위치
                bw.Write(chunkInfo.X);
                bw.Write(chunkInfo.Y);

                // 청크 크기
                bw.Write(this.Board.ChunkSize);

                // 타일 정보
                for (int x = 0; x < chunk.GetLength(0); ++x)
                {
                    for (int y = 0; y < chunk.GetLength(1); ++y)
                    {
                        var tile = chunk[x, y];

                        tile.WriteToStream(bw);
                    }
                }
            }


            // 다음 청크로 넘어가고 없으면 종료
            if (m_currentSaveChunk.MoveNext())
                return true;
            else
            {
                m_tempBoard.Clear();
                m_tempBoard = null;
                m_currentSaveChunk = null;
                return false;
            }
        }

        /// <summary>
        /// 현재 선택된 청크를 불러오고 다음 청크를 선택한다.
        /// </summary>
        /// <returns>다음 청크의 유무</returns>
        public bool LoadNext()
        {
            // 열거자 얻기
            if (m_currentLoadChunk == null)
            {
                m_board.Clear();


                m_currentLoadChunk = Directory.EnumerateFileSystemEntries(this.BoardPath).GetEnumerator();

                if (m_currentLoadChunk.MoveNext() == false)
                {
                    m_currentLoadChunk = null;
                    return false;
                }
            }


            try
            {
                using (BinaryReader br = new BinaryReader(new FileStream(m_currentLoadChunk.Current, FileMode.Open)))
                {
                    // 파일 버전
                    Utility.Version fileVersion = new Utility.Version(br.ReadString());


                    // 청크 위치
                    int chunkX = br.ReadInt32();
                    int chunkY = br.ReadInt32();

                    // 청크 생성
                    m_board.CreateChunkAt(chunkX, chunkY);
                    var chunk = m_board.GetChunkAt(chunkX, chunkY);

                    // 청크 크기
                    int chunkSize = br.ReadInt32();

                    // 타일 정보
                    for (int x = 0; x < chunkSize; ++x)
                    {
                        for (int y = 0; y < chunkSize; ++y)
                        {
                            // 타일 설정
                            var tile = chunk[x, y];

                            tile.ReadFromStream(br);


                            // 설치된 것이 있다면 목록에 위치를 추가
                            if (tile.IsFactoryTile)
                            {
                                m_factoryList.Add(new Point(
                                    chunkX * this.Board.ChunkSize + x,
                                    chunkY * this.Board.ChunkSize + y));
                            }
                            else if (tile.IsCompanyTile)
                            {
                                m_companyList.Add(new Point(
                                    chunkX * this.Board.ChunkSize + x,
                                    chunkY * this.Board.ChunkSize + y));
                            }
                            else if (tile.IsChipTile)
                            {
                                m_chipList.Add(new Point(
                                    chunkX * this.Board.ChunkSize + x,
                                    chunkY * this.Board.ChunkSize + y));
                            }
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            { }


            // 다음 파일로 넘어감
            if (m_currentLoadChunk.MoveNext())
                return true;
            else
            {
                m_currentLoadChunk = null;
                return false;
            }
        }

        public void Clear()
        {
            m_board.Clear();
        }

        //#####################################################################################

        public bool TileExistsAndVisibleAt(int x, int y)
        {
            return (this.Board.ContainsItemAt(x, y) && this.Board.GetItemAt(x, y).Visible);
        }

        public void SetOwnerAt(int x, int y, string owner)
        {
            m_board[x, y].Owner = owner;
        }

        public string GetOwnerAt(int x, int y)
        {
            return m_board[x, y].Owner;
        }

        public void SetPowerAt(int x, int y, int power)
        {
            m_board[x, y].Power = power;
        }

        public void AddPowerAt(int x, int y, int deltaPower)
        {
            m_board[x, y].Power += deltaPower;
        }

        public void SetSignAt(int x, int y, string newSign)
        {
            m_board[x, y].Sign = newSign;
        }

        public string GetSignAt(int x, int y)
        {
            return m_board[x, y].Sign;
        }

        public bool FindCountryLocation(string name, out int tileX, out int tileY)
        {
            // TODO: 나중에 유저 계정정보에서 바로 얻을 수 있도록 최적화하자.


            foreach (var chunkInfo in this.Board.GetEnumerable())
            {
                var chunk = chunkInfo.Chunk;


                for (int x = 0; x < chunk.GetLength(0); ++x)
                {
                    for (int y = 0; y < chunk.GetLength(1); ++y)
                    {
                        var tile = chunk[x, y];

                        if (tile.Owner == name)
                        {
                            tileX = chunkInfo.X * this.Board.ChunkSize + x;
                            tileY = chunkInfo.Y * this.Board.ChunkSize + y;

                            return true;
                        }
                    }
                }
            }


            tileX = 0;
            tileY = 0;
            return false;
        }

        public void CreateRandomTerritory(out int tileX, out int tileY)
        {
            int chunkSize = this.Board.ChunkSize;

            int randomX = Utility.Random.Next(0, 4) * ((Utility.Random.Next() % 2 == 0) ? 1 : -1);
            int randomY = Utility.Random.Next(0, 4) * ((Utility.Random.Next() % 2 == 0) ? 1 : -1);

            for (int i = 0; i < int.MaxValue; ++i)
            {
                if (this.Board.GetChunkAt(randomX, randomY) == null)
                {
                    this.Board.CreateChunkAt(randomX, randomY);

                    tileX = randomX * chunkSize + chunkSize / 2;
                    tileY = randomY * chunkSize + chunkSize / 2;

                    return;
                }
                else
                {
                    randomX += Utility.Random.Next(0, 4) * ((Utility.Random.Next() % 2 == 0) ? 1 : -1);
                    randomY += Utility.Random.Next(0, 4) * ((Utility.Random.Next() % 2 == 0) ? 1 : -1);
                }
            }


            this.Board.CreateChunkAt(randomX, randomY);

            tileX = randomX * chunkSize + chunkSize / 2;
            tileY = randomY * chunkSize + chunkSize / 2;

            return;
        }

        public Point[] AttackTerritory(string attackerName, int targetX, int targetY,
            out bool captureSuccess, out string oldOwner)
        {
            captureSuccess = false;
            oldOwner = "";
            List<Point> changedTileList = new List<Point>();


            var targetTile = this.Board.GetItemAt(targetX, targetY);

            if (targetTile != null)
            {
                oldOwner = targetTile.Owner;


                // 목표 타일이 이미 자신의 영토이면
                // 더이상 처리하지 않고 반환.
                if (targetTile.Owner == attackerName)
                {
                    return null;
                }


                // 인접 타일의 공격자 타일에서 공격력을 계산
                int attackPower = 0;

                int[] nearX = new int[]
                {
                    targetX, targetX + 1, targetX, targetX - 1
                };
                int[] nearY = new int[]
                {
                    targetY - 1, targetY, targetY + 1, targetY
                };

                for (int i = 0; i < 4; ++i)
                {
                    var nearTile = this.Board.GetItemAt(nearX[i], nearY[i]);

                    if (nearTile != null
                        && nearTile.Owner == attackerName
                        && nearTile.Power > 0)
                    {
                        int damage = nearTile.Power / 2;

                        if (damage > 0)
                        {
                            // 공격에 쓰이는 힘 만큼 소모
                            nearTile.Power -= damage;

                            // 공격력 합산
                            attackPower += damage;


                            changedTileList.Add(new Point(nearX[i], nearY[i]));
                        }
                    }
                }


                // 공격력이 유효하면
                if (attackPower > 0)
                {
                    // 공격
                    targetTile.Power -= attackPower;

                    // 점령 판정
                    if (targetTile.Power <= 0)
                    {
                        // 점령
                        targetTile.Owner = attackerName;
                        targetTile.Power = Math.Abs(targetTile.Power) + 1;
                        targetTile.Sign = "";

                        // 공장이 있었다면 파괴
                        this.DestroyFactory(targetX, targetY);


                        captureSuccess = true;
                    }


                    changedTileList.Add(new Point(targetX, targetY));
                }
            }


            if (changedTileList.Count > 0)
                return changedTileList.ToArray();
            return null;
        }

        public bool CheckCanSendPower(string senderName, Point fromTile, Point toTile)
        {
            // 두 타일 모두 존재하고
            if (this.Board.ContainsItemAt(fromTile.X, fromTile.Y)
                && this.Board.ContainsItemAt(toTile.X, toTile.Y))
            {
                int gapX = Math.Abs(fromTile.X - toTile.X);
                int gapY = Math.Abs(fromTile.Y - toTile.Y);

                // 두 타일이 상하좌우로 붙어있고
                if ((gapX == 1 && gapY == 0) || (gapX == 0 && gapY == 1))
                {
                    var fromTileItem = this.Board.GetItemAt(fromTile.X, fromTile.Y);
                    var toTileItem = this.Board.GetItemAt(toTile.X, toTile.Y);

                    // fromTileItem senderName의 영토이고
                    // 보낼만한 충분한 힘이 있으면
                    if (fromTileItem.Owner == senderName
                        && fromTileItem.Power >= GameValues.MinPowerToSend)
                    {
                        return true;
                    }
                }
            }


            return false;
        }

        public int SendPower(Point fromTile, Point toTile)
        {
            var fromTileItem = this.Board.GetItemAt(fromTile.X, fromTile.Y);
            var toTileItem = this.Board.GetItemAt(toTile.X, toTile.Y);

            int sendingPower = fromTileItem.Power / 2;

            // 힘 이동
            fromTileItem.Power -= sendingPower;
            toTileItem.Power += sendingPower;


            return sendingPower;
        }

        public void BuildFactory(int tileX, int tileY)
        {
            if (m_board[tileX, tileY].IsResourceTile)
            {
                m_board[tileX, tileY].Kind = TileTypes.ResourceFactory;

                // 공장 목록에 추가
                m_factoryList.Add(new Point(tileX, tileY));
            }
        }

        public void DestroyFactory(int tileX, int tileY)
        {
            if (m_board[tileX, tileY].IsFactoryTile)
            {
                m_board[tileX, tileY].Kind = TileTypes.Resource;

                // 공장 목록에서 제거
                for (int i = 0; i < m_factoryList.Count; ++i)
                {
                    if (m_factoryList[i].X == tileX
                        && m_factoryList[i].Y == tileY)
                    {
                        m_factoryList.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void BuildCompany(int tileX, int tileY, string name, string owner)
        {
            if (m_board[tileX, tileY].IsNormalTile)
            {
                m_board[tileX, tileY].Kind = TileTypes.Company;

                // 회사 목록에 추가
                m_companyList.Add(new Point(tileX, tileY));
            }
        }

        public void DestroyCompany(int tileX, int tileY)
        {
            if (m_board[tileX, tileY].IsCompanyTile)
            {
                m_board[tileX, tileY].Kind = TileTypes.Normal;

                // 회사 목록에서 제거
                for (int i = 0; i < m_companyList.Count; ++i)
                {
                    if (m_companyList[i].X == tileX
                        && m_companyList[i].Y == tileY)
                    {
                        m_companyList.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void BuildChip(int tileX, int tileY)
        {
            if (m_board[tileX, tileY].IsNormalTile)
            {
                m_board[tileX, tileY].Kind = TileTypes.Chip;

                // 칩 목록에 추가
                m_chipList.Add(new Point(tileX, tileY));
            }
        }

        public void DestroyChip(int tileX, int tileY)
        {
            if (m_board[tileX, tileY].IsChipTile)
            {
                m_board[tileX, tileY].Kind = TileTypes.Normal;

                // 칩 목록에서 제거
                for (int i = 0; i < m_chipList.Count; ++i)
                {
                    if (m_chipList[i].X == tileX
                        && m_chipList[i].Y == tileY)
                    {
                        m_chipList.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public string[] GetUsersInVision(int tileX, int tileY)
        {
            List<string> userList = new List<string>();


            int visionLength = GameValues.VisionLength;
            int beginX = tileX - visionLength;
            int beginY = tileY - visionLength;
            int endX = beginX + visionLength * 2;
            int endY = beginY + visionLength * 2;

            for (int x = beginX; x <= endX; ++x)
            {
                for (int y = beginY; y <= endY; ++y)
                {
                    // NOTE: GetItemAt을 함으로서 존재하지 않는 청크를 생성하여 맵을 확장하는 효과도 있음.
                    var tile = this.Board.GetItemAt(x, y);

                    if (tile != null
                        && tile.HaveOwner
                        && userList.Any(name => name == tile.Owner) == false)
                    {
                        userList.Add(tile.Owner);
                    }
                }
            }


            if (userList.Count > 0)
                return userList.ToArray();
            return null;
        }
    }
}
