using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace ClickWar2.Game.View
{
    public class BoardView : IBoardView
    {
        public BoardView()
        {
            m_playerColorMap.Add("", SystemColors.Control);
            m_playerBrushMap.Add("", new KeyValuePair<Brush, bool>(SystemBrushes.Control, false));
        }

        ~BoardView()
        {
            foreach (var brh in m_playerBrushMap)
            {
                if (brh.Value.Value)
                {
                    brh.Value.Key.Dispose();
                }
            }
        }

        //#####################################################################################
        // 사용자 입력 이벤트 구독

        public event Action<Point> WhenLeftDown;
        public event Action<Point> WhenLeftUp;
        public event Action<Point> WhenRightDown;
        public event Action<Point> WhenRightUp;
        public event Action<Point> WhenCursorMove;
        public event Action<Keys> WhenKeyDown;
        public event Action<int> WhenMouseWheelScroll;
        public event Action<string> WhenInputText;
        public event Action<string> WhenRequestUserColor;
        public event Action<Point> WhenLongLeftClick;
        public event Action<string, string> WhenInputMail;
        public event Action<string, Action<Network.RegisterCompanyResults>> WhenInputCopApplicationForm;
        public event Action<string ,string, List<Command>, Action<Network.DevelopTechResults>> WhenDevelopTech;
        public event Action<string, string, Action> WhenDiscardTech;
        public event Action<string, string, Action> WhenProduceProduct;
        public event Action<string, int, Action> WhenDiscardProduct;
        public event Action<string, string, Action<string, string, string>> WhenRequestTechProgram;
        public event Action<string, string, int, string> WhenSellTech;
        public event Action<string, string, int, string> WhenBuyTech;
        public event Action<string, int, int, string> WhenSellProduct;
        public event Action<string, string, int, string> WhenBuyProduct;

        //#####################################################################################
        // 그리기 정보

        public Point BoardLocation
        { get; set; } = new Point(0, 0);

        public int TileSize
        { get; set; } = 32;

        public Point Cursor
        { get; set; }

        public Point TileCursor
        { get; set; }

        public Size ScreenSize
        { get; set; }

        protected Dictionary<string, Color> m_playerColorMap = new Dictionary<string, Color>();
        protected Dictionary<string, KeyValuePair<Brush, bool>> m_playerBrushMap = new Dictionary<string, KeyValuePair<Brush, bool>>();
        protected List<string> m_requestColorList = new List<string>();

        protected const int MinTileSizeToShowDetail = 32;

        //#####################################################################################

        protected bool m_bShowDragHelper = false;
        protected Point m_dragHelperCenter;

        //#####################################################################################
        // 사용자 입력 이벤트

        public void OnMouseLeftDown(Point cursor)
        {
            this.WhenLeftDown(cursor);
        }

        public void OnMouseLeftUp(Point cursor)
        {
            this.WhenLeftUp(cursor);
        }
        public void OnMouseLeftUp()
        {
            this.WhenLeftUp(this.Cursor);
        }

        public void OnMouseRightDown(Point cursor)
        {
            this.WhenRightDown(cursor);
        }

        public void OnMouseRightUp(Point cursor)
        {
            this.WhenRightUp(cursor);
        }
        public void OnMouseRightUp()
        {
            this.WhenRightUp(this.Cursor);
        }

        public void OnMouseMove(Point cursor)
        {
            this.WhenCursorMove(cursor);
        }

        public void OnKeyDown(Keys key)
        {
            this.WhenKeyDown(key);
        }

        public void OnMouseWheelScroll(int delta)
        {
            this.WhenMouseWheelScroll(delta);
        }

        public void OnInputText(string text)
        {
            this.WhenInputText(text);
        }

        public void OnLongLeftClick(Point cursor)
        {
            this.WhenLongLeftClick(cursor);
        }

        public void OnInputMail(string targetName, string message)
        {
            this.WhenInputMail(targetName, message);
        }

        public void OnInputCopApplicationForm(string name, Action<Network.RegisterCompanyResults> callbackAsync)
        {
            this.WhenInputCopApplicationForm(name, callbackAsync);
        }

        public void OnDevelopTech(string companyName, string techName, List<Command> program,
            Action<Network.DevelopTechResults> callbackAsync)
        {
            this.WhenDevelopTech(companyName, techName, program, callbackAsync);
        }

        public void OnDiscardTech(string companyName, string techName, Action callbackAsync)
        {
            this.WhenDiscardTech(companyName, techName, callbackAsync);
        }

        public void OnProduceProduct(string companyName, string techName, Action callbackAsync)
        {
            this.WhenProduceProduct(companyName, techName, callbackAsync);
        }

        public void OnDiscardProduct(string companyName, int productIndex, Action callbackAsync)
        {
            this.WhenDiscardProduct(companyName, productIndex, callbackAsync);
        }

        public void OnRequestTechProgram(string companyName, string techName,
            Action<string, string, string> callbackAsync)
        {
            this.WhenRequestTechProgram(companyName, techName, callbackAsync);
        }

        public void OnSellTech(string companyName, string techName,
            int price, string targetUser)
        {
            this.WhenSellTech(companyName, techName, price, targetUser);
        }

        public void OnBuyTech(string sellerName, string techName,
            int price, string buyerName)
        {
            this.WhenBuyTech(sellerName, techName, price, buyerName);
        }

        public void OnSellProduct(string companyName, int productIndex,
            int price, string targetUser)
        {
            this.WhenSellProduct(companyName, productIndex, price, targetUser);
        }

        public void OnBuyProduct(string sellerName, string productName,
            int price, string buyerName)
        {
            this.WhenBuyProduct(sellerName, productName, price, buyerName);
        }

        //#####################################################################################
        // 그리기

        public void DrawBoard(GameBoard board, Rectangle screenRect, bool showInvisibleTile, Graphics g)
        {
            int chunkSize = board.Board.ChunkSize;

            int chunkXBegin = -this.BoardLocation.X / (chunkSize * this.TileSize) - 1;
            int chunkYBegin = -this.BoardLocation.Y / (chunkSize * this.TileSize) - 1;

            int chunkXEndInScreen = screenRect.Width / (chunkSize * this.TileSize) + 2
                + chunkXBegin;
            int chunkYEndInScreen = screenRect.Height / (chunkSize * this.TileSize) + 2
                + chunkYBegin;


            for (int chunkX = chunkXBegin; chunkX <= chunkXEndInScreen; ++chunkX)
            {
                int x = chunkX * (chunkSize * this.TileSize) + this.BoardLocation.X;


                for (int chunkY = chunkYBegin; chunkY <= chunkYEndInScreen; ++chunkY)
                {
                    var chunk = board.Board.GetChunkAt(chunkX, chunkY);

                    if (chunk != null)
                    {
                        this.DrawChunkBack(chunk,
                            x,
                            chunkY * (chunkSize * this.TileSize) + this.BoardLocation.Y,
                            showInvisibleTile, g);
                    }
                }
            }


            // 타일 크기가 충분히 크면
            if (this.TileSize >= MinTileSizeToShowDetail)
            {
                const int minLength = 8;

                Point[] rectVtx = new Point[]
                {
                    new Point(this.TileCursor.X, this.TileCursor.Y),
                    new Point(this.TileCursor.X - minLength, this.TileCursor.Y),
                    new Point(this.TileCursor.X + minLength, this.TileCursor.Y),
                    new Point(this.TileCursor.X, this.TileCursor.Y - minLength),
                    new Point(this.TileCursor.X, this.TileCursor.Y + minLength),
                    new Point(this.TileCursor.X - minLength, this.TileCursor.Y - minLength),
                    new Point(this.TileCursor.X + minLength, this.TileCursor.Y - minLength),
                    new Point(this.TileCursor.X + minLength, this.TileCursor.Y + minLength),
                    new Point(this.TileCursor.X - minLength, this.TileCursor.Y + minLength),
                };

                foreach (Point vtx in rectVtx)
                {
                    int chunkX, chunkY;

                    board.Board.GetChunkPosContainsItemAt(vtx.X, vtx.Y,
                        out chunkX, out chunkY);

                    var focusChunk = board.Board.GetChunkAt(chunkX, chunkY);

                    if (focusChunk != null)
                    {
                        this.DrawChunkTileDetail(focusChunk,
                            chunkX * (chunkSize * this.TileSize) + this.BoardLocation.X,
                            chunkY * (chunkSize * this.TileSize) + this.BoardLocation.Y,
                            showInvisibleTile, g);
                    }
                }
            }


            for (int chunkX = chunkXBegin; chunkX <= chunkXEndInScreen; ++chunkX)
            {
                int x = chunkX * (chunkSize * this.TileSize) + this.BoardLocation.X;


                for (int chunkY = chunkYBegin; chunkY <= chunkYEndInScreen; ++chunkY)
                {
                    var chunk = board.Board.GetChunkAt(chunkX, chunkY);

                    if (chunk != null)
                    {
                        this.DrawChunkBorder(board, chunk,
                            chunkX, chunkY,
                            x,
                            chunkY * (chunkSize * this.TileSize) + this.BoardLocation.Y,
                            g);
                    }
                }
            }


            for (int chunkX = chunkXBegin; chunkX <= chunkXEndInScreen; ++chunkX)
            {
                int x = chunkX * (chunkSize * this.TileSize) + this.BoardLocation.X;


                for (int chunkY = chunkYBegin; chunkY <= chunkYEndInScreen; ++chunkY)
                {
                    var chunk = board.Board.GetChunkAt(chunkX, chunkY);

                    if (chunk != null)
                    {
                        this.DrawChunkSign(chunk,
                            x,
                            chunkY * (chunkSize * this.TileSize) + this.BoardLocation.Y,
                            showInvisibleTile, g);
                    }
                }
            }


            Tile focusTile = null;

            if (board.Board.ContainsItemAt(this.TileCursor.X, this.TileCursor.Y))
                focusTile = board.Board.GetItemAt(this.TileCursor.X, this.TileCursor.Y);

            DrawFocusTile(g, focusTile);
        }

        protected void DrawChunkBack(Tile[,] chunk, int beginX, int beginY, bool showInvisibleTile, Graphics g)
        {
            int width = chunk.GetLength(0);
            int height = chunk.GetLength(1);

            int tileSize = this.TileSize;
            int halfTileSize = tileSize / 2;

            
            for (int x = 0; x < width; ++x)
            {
                int tileX = x * tileSize + beginX + halfTileSize;


                for (int y = 0; y < height; ++y)
                {
                    var tile = chunk[x, y];

                    // 영토(타일)가 존재하고
                    // 보이는 상태이거나 무조건 보이는 모드이면
                    if (tile != null
                        && (tile.Visible || showInvisibleTile))
                    {
                        int tileY = y * tileSize + beginY + halfTileSize;


                        Brush tileColor = null;

                        // 주인에 해당하는 타일 색상 가져오기
                        if (m_playerBrushMap.ContainsKey(tile.Owner))
                            tileColor = m_playerBrushMap[tile.Owner].Key;
                        else
                        {
                            // 없으면 기본 색으로 설정
                            tileColor = Brushes.White;

                            // 이전에 요청하지 않았으면
                            if (m_requestColorList.Any(name => name == tile.Owner) == false)
                            {
                                // 플레이어 색 요청
                                this.WhenRequestUserColor(tile.Owner);

                                // 요청 목록에 추가
                                m_requestColorList.Add(tile.Owner);
                            }
                        }

                        // 색 칠하기
                        if (tileColor != null)
                        {
                            int drawX = beginX + x * this.TileSize;
                            int drawY = beginY + y * this.TileSize;
                            int drawSize = this.TileSize;

                            g.FillRectangle(tileColor,
                                drawX, drawY,
                                drawSize, drawSize);

                            // 타일 종류에 따른 표식
                            switch (tile.Kind)
                            {
                                case TileTypes.Resource:
                                    g.DrawEllipse(Pens.Black,
                                        drawX + 1, drawY + 1,
                                        drawSize - 2, drawSize - 2);
                                    break;

                                case TileTypes.ResourceFactory:
                                    g.FillEllipse(Brushes.MediumAquamarine,
                                        drawX + 1, drawY + 1,
                                        drawSize - 2, drawSize - 2);
                                    g.DrawEllipse(Pens.Black,
                                        drawX + 1, drawY + 1,
                                        drawSize - 2, drawSize - 2);
                                    g.FillEllipse(tileColor,
                                        drawX + 3, drawY + 3,
                                        drawSize - 6, drawSize - 6);

                                    if (tile.Power > GameValues.FactoryConversionAmount)
                                    {
                                        g.DrawEllipse(Pens.Black,
                                            drawX + 3, drawY + 3,
                                            drawSize - 6, drawSize - 6);
                                    }
                                    else
                                    {
                                        g.FillEllipse(Brushes.DarkGray,
                                            drawX + 3, drawY + 3,
                                            drawSize - 6, drawSize - 6);
                                    }
                                    break;

                                case TileTypes.Company:
                                    if (this.TileSize >= 6)
                                    {
                                        g.DrawRectangle(Pens.Black,
                                            drawX + 2, drawY + 2,
                                            drawSize - 4, drawSize - 4);
                                    }
                                    break;

                                case TileTypes.Chip:
                                    if (this.TileSize >= 10)
                                    {
                                        g.DrawRectangle(Pens.Black,
                                            drawX + 4, drawY + 4,
                                            drawSize - 8, drawSize - 8);
                                        g.FillRectangle(Brushes.DarkGray,
                                            drawX + 4, drawY + 4,
                                            drawSize - 8, drawSize - 8);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        protected void DrawChunkTileDetail(Tile[,] chunk, int beginX, int beginY, bool showInvisibleTile, Graphics g)
        {
            var centerStringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };


            int width = chunk.GetLength(0);
            int height = chunk.GetLength(1);

            int tileSize = this.TileSize;
            int halfTileSize = tileSize / 2;


            for (int x = 0; x < width; ++x)
            {
                int tileX = x * tileSize + beginX + halfTileSize;


                for (int y = 0; y < height; ++y)
                {
                    var tile = chunk[x, y];

                    // 영토(타일)가 존재하고
                    // 보이는 상태이거나 무조건 보이는 모드이면
                    if (tile != null
                        && (tile.Visible || showInvisibleTile))
                    {
                        int tileY = y * tileSize + beginY + halfTileSize;

                        
                        // 타일의 힘이 0이 아니면
                        if (tile.Power != 0)
                        {
                            // 타일 힘 표시
                            g.DrawString(tile.Power.ToString(), SystemFonts.DefaultFont, Brushes.Black,
                                tileX,
                                tileY + 1,
                                centerStringFormat);
                        }
                    }
                }
            }
        }

        private void DrawChunkSign(Tile[,] chunk, int beginX, int beginY, bool showInvisibleTile, Graphics g)
        {
            int width = chunk.GetLength(0);
            int height = chunk.GetLength(1);

            int tileSize = this.TileSize;
            int halfTileSize = tileSize / 2;


            // 타일 크기가 충분히 크면
            if (this.TileSize >= MinTileSizeToShowDetail)
            {
                for (int x = 0; x < width; ++x)
                {
                    int tileX = x * tileSize + beginX + halfTileSize;


                    for (int y = 0; y < height; ++y)
                    {
                        var tile = chunk[x, y];

                        // 영토(타일)가 존재하고
                        // 보이는 상태이거나 무조건 보이는 모드이면
                        if (tile != null
                            && (tile.Visible || showInvisibleTile))
                        {
                            int tileY = y * tileSize + beginY + halfTileSize;


                            // 타일의 푯말이 있으면
                            if (tile.HaveSign)
                            {
                                this.DrawSign(g, tileX, tileY, tile);
                            }
                        }
                    }
                }
            }
        }

        private void DrawChunkBorder(GameBoard board, Tile[,] chunk, int chunkX, int chunkY, int beginX, int beginY, Graphics g)
        {
            int width = chunk.GetLength(0);
            int height = chunk.GetLength(1);

            int tileSize = this.TileSize;
            int halfTileSize = tileSize / 2;


            for (int x = 0; x < width; ++x)
            {
                int tileX = x * tileSize + beginX + halfTileSize;


                for (int y = 0; y < height; ++y)
                {
                    var tile = chunk[x, y];

                    // 영토(타일)가 존재하면
                    if (tile != null)
                    {
                        int tileY = y * tileSize + beginY + halfTileSize;


                        int[] nearX = new int[]
                        {
                            x, x + 1
                        };
                        int[] nearY = new int[]
                        {
                            y - 1, y
                        };

                        for (int i = 0; i < 2; ++i)
                        {
                            Tile nearTile = null;

                            if (nearX[i] < 0 || nearX[i] >= width
                                ||
                                nearY[i] < 0 || nearY[i] >= height)
                            {
                                int globalX = chunkX * board.Board.ChunkSize + nearX[i];
                                int globalY = chunkY * board.Board.ChunkSize + nearY[i];

                                if (board.Board.ContainsItemAt(globalX, globalY))
                                {
                                    nearTile = board.Board.GetItemAt(globalX, globalY);
                                }
                            }
                            else
                            {
                                nearTile = chunk[nearX[i], nearY[i]];
                            }

                            if (nearTile != null && nearTile.Owner != tile.Owner)
                            {
                                switch (i)
                                {
                                    case 0:
                                        g.DrawLine(Pens.Black,
                                            beginX + x * TileSize, beginY + y * TileSize,
                                            beginX + x * TileSize + TileSize, beginY + y * TileSize);
                                        break;

                                    case 1:
                                        g.DrawLine(Pens.Black,
                                            beginX + x * TileSize + TileSize, beginY + y * TileSize,
                                            beginX + x * TileSize + TileSize, beginY + y * TileSize + TileSize);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DrawSign(Graphics g, int x, int y, Tile tile)
        {
            var boxSize = TextRenderer.MeasureText(tile.Sign, SystemFonts.DefaultFont);
            int boxWidth = boxSize.Width + 4;
            int boxHeight = boxSize.Height + 8;

            g.FillPie(Brushes.SandyBrown,
                x - 16,
                y - 14,
                32,
                28,
                270 - 20,
                40);

            g.FillRectangle(Brushes.SandyBrown,
                x - (boxWidth >> 1),
                y - boxHeight - 8,
                boxWidth,
                boxHeight);

            g.DrawString(tile.Sign, SystemFonts.DefaultFont, Brushes.Black,
                x,
                y - boxHeight - 4,
                new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                });
        }

        private void DrawFocusTile(Graphics g, Tile tile)
        {
            if (tile != null && tile.Visible && tile.HaveSign)
            {
                int halfTileSize = this.TileSize / 2;

                this.DrawSign(g,
                    this.BoardLocation.X + this.TileCursor.X * this.TileSize + halfTileSize,
                    this.BoardLocation.Y + this.TileCursor.Y * this.TileSize + halfTileSize,
                    tile);
            }

            this.DrawTileOverlay(g, this.TileCursor.X, this.TileCursor.Y, Color.Aqua);
        }

        private void DrawTileOverlay(Graphics g, int x, int y, Color color, int opacity = 128)
        {
            using (Brush brh = new SolidBrush(Color.FromArgb(opacity, color)))
            {
                g.FillRectangle(brh,
                    this.BoardLocation.X + x * this.TileSize,
                    this.BoardLocation.Y + y * this.TileSize,
                    this.TileSize, this.TileSize);
            }
        }

        public void DrawDragHelper(Graphics g)
        {
            if (m_bShowDragHelper)
            {
                int[] nearX = new int[]
                {
                    m_dragHelperCenter.X, m_dragHelperCenter.X + 1,
                    m_dragHelperCenter.X, m_dragHelperCenter.X - 1
                };
                int[] nearY = new int[]
                {
                    m_dragHelperCenter.Y - 1, m_dragHelperCenter.Y,
                    m_dragHelperCenter.Y + 1, m_dragHelperCenter.Y
                };

                for (int i = 0; i < 4; ++i)
                {
                    this.DrawTileOverlay(g, nearX[i], nearY[i], Color.Aquamarine);
                }
            }
        }

        //#####################################################################################

        public void MoveScreenTo(int tileX, int tileY)
        {
            this.BoardLocation = new Point(-tileX * this.TileSize + ScreenSize.Width / 2,
                    -tileY * this.TileSize + ScreenSize.Height / 2);
        }

        public void ChangeScale(int deltaTileSize, Point center,
            out int deltaCamX, out int deltaCamY)
        {
            // 확대/축소에 따라 화면도 보던 곳을 추적.
            deltaCamX = deltaTileSize * (this.BoardLocation.X - center.X) / this.TileSize;
            deltaCamY = deltaTileSize * (this.BoardLocation.Y - center.Y) / this.TileSize;

            // 추적값을 계산한 뒤에 타일크기를 바꿔야 제대로 된다.

            if (deltaTileSize < 0)
            {
                if (this.TileSize > -deltaTileSize + 1)
                {
                    this.TileSize += deltaTileSize;
                }
            }
            else
            {
                if (this.TileSize < 256)
                {
                    this.TileSize += deltaTileSize;
                }
            }

            this.BoardLocation = new Point(this.BoardLocation.X + deltaCamX, this.BoardLocation.Y + deltaCamY);
        }

        //#####################################################################################

        public void ShowLeftDragHelper(Point center)
        {
            m_bShowDragHelper = true;

            m_dragHelperCenter = center;
        }

        public void HideLeftDragHelper()
        {
            m_bShowDragHelper = false;
        }

        //#####################################################################################

        public void SetUserColor(string userName, Color color)
        {
            if (m_playerColorMap.ContainsKey(userName))
                m_playerColorMap[userName] = color;
            else
                m_playerColorMap.Add(userName, color);

            if (m_playerBrushMap.ContainsKey(userName))
            {
                if (m_playerBrushMap[userName].Value)
                    m_playerBrushMap[userName].Key.Dispose();

                m_playerBrushMap[userName] = new KeyValuePair<Brush, bool>(new SolidBrush(color), true);
            }
            else
            {
                m_playerBrushMap.Add(userName, new KeyValuePair<Brush, bool>(new SolidBrush(color), true));
            }


            // 요청에 대한 응답을 받았으므로 요청 목록에서 제거
            m_requestColorList.Remove(userName);
        }
    }
}
