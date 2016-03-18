using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using ClickWar2.Game.View;

namespace ClickWar2.Game.Presenter
{
    public abstract class BoardPresenter
    {
        public BoardPresenter()
        {

        }

        ~BoardPresenter()
        {
            DetachEvent(this.BoardView);
        }

        //#####################################################################################

        protected Point m_oldCursor = new Point(0, 0);
        protected Point OldCursor
        {
            get { return m_oldCursor; }
            set
            {
                m_oldCursor = value;

                this.WhenCursorChanged(value, this.ViewPointToTileIndex(value));

                if (this.UI != null)
                {
                    this.UI.Cursor = value;
                    this.UI.TileCursor = this.ViewPointToTileIndex(value);
                    this.UI.FocusTile = this.GetTileContains(value);
                }
            }
        }
        protected Point m_cursorBeginMove;
        protected bool m_moveCam = false;

        //#####################################################################################

        protected IBoardView m_boardView = null;
        public IBoardView BoardView
        {
            get { return m_boardView; }
            set
            {
                DetachEvent(m_boardView);

                m_boardView = value;

                AttachEvent(value);
            }
        }

        protected IGameInterface m_ui = null;
        public IGameInterface UI
        {
            get { return m_ui; }
            set
            {
                DetachEvent(m_ui);

                m_ui = value;

                AttachEvent(value);
            }
        }

        protected IEffectView m_effectView = null;
        public IEffectView EffectDirector
        { get { return m_effectView; } set { m_effectView = value; } }

        //#####################################################################################

        protected void AttachEvent(IBoardView view)
        {
            if (view != null)
            {
                view.WhenLeftDown += this.OnLeftDown;
                view.WhenLeftUp += this.OnLeftUp;
                view.WhenRightDown += this.OnRightDown;
                view.WhenRightUp += this.OnRightUp;
                view.WhenCursorMove += this.OnCursorMove;
                view.WhenKeyDown += this.OnKeyDown;
                view.WhenMouseWheelScroll += this.OnMouseWheelScroll;
                view.WhenInputText += this.OnInputText;
                view.WhenRequestUserColor += this.OnRequestUserColor;
                view.WhenLongLeftClick += this.OnLongLeftClick;
                view.WhenInputMail += this.OnInputMail;
                view.WhenInputCopApplicationForm += this.OnInputCopApplicationForm;
                view.WhenDevelopTech += this.OnDevelopTech;
                view.WhenDiscardTech += this.OnDiscardTech;
                view.WhenProduceProduct += this.OnProduceProduct;
                view.WhenDiscardProduct += this.OnDiscardProduct;
                view.WhenRequestTechProgram += this.OnRequestTechProgram;
                view.WhenSellTech += this.OnSellTech;
                view.WhenBuyTech += this.OnBuyTech;
                view.WhenSellProduct += this.OnSellProduct;
                view.WhenBuyProduct += this.OnBuyProduct;
            }
        }

        protected void DetachEvent(IBoardView view)
        {
            if (view != null)
            {
                view.WhenLeftDown -= this.OnLeftDown;
                view.WhenLeftUp -= this.OnLeftUp;
                view.WhenRightDown -= this.OnRightDown;
                view.WhenRightUp -= this.OnRightUp;
                view.WhenCursorMove -= this.OnCursorMove;
                view.WhenKeyDown -= this.OnKeyDown;
                view.WhenMouseWheelScroll -= this.OnMouseWheelScroll;
                view.WhenInputText -= this.OnInputText;
                view.WhenRequestUserColor -= this.OnRequestUserColor;
                view.WhenLongLeftClick -= this.OnLongLeftClick;
                view.WhenInputMail -= this.OnInputMail;
                view.WhenInputCopApplicationForm -= this.OnInputCopApplicationForm;
                view.WhenDevelopTech -= this.OnDevelopTech;
                view.WhenDiscardTech -= this.OnDiscardTech;
                view.WhenProduceProduct -= this.OnProduceProduct;
                view.WhenDiscardProduct -= this.OnDiscardProduct;
                view.WhenRequestTechProgram -= this.OnRequestTechProgram;
                view.WhenSellTech -= this.OnSellTech;
                view.WhenBuyTech -= this.OnBuyTech;
                view.WhenSellProduct -= this.OnSellProduct;
                view.WhenBuyProduct -= this.OnBuyProduct;
            }
        }

        protected void AttachEvent(IGameInterface view)
        {

        }

        protected void DetachEvent(IGameInterface view)
        {

        }

        //#####################################################################################

        private void OnLeftDown(Point cursor)
        {
            this.WhenLeftDown(cursor, ViewPointToTileIndex(cursor));


            this.OldCursor = cursor;
        }

        private void OnLeftUp(Point cursor)
        {
            this.WhenLeftUp(cursor, ViewPointToTileIndex(cursor));


            this.OldCursor = cursor;
        }

        private void OnRightDown(Point cursor)
        {
            this.WhenRightDown(cursor, ViewPointToTileIndex(cursor));


            this.OldCursor = cursor;
            m_cursorBeginMove = cursor;
            m_moveCam = true;
        }

        private void OnRightUp(Point cursor)
        {
            this.WhenRightUp(cursor, ViewPointToTileIndex(cursor));


            this.OldCursor = cursor;
            m_moveCam = false;
        }

        private void OnCursorMove(Point cursor)
        {
            this.WhenCursorMove(cursor, ViewPointToTileIndex(cursor));


            if (m_moveCam)
            {
                int deltaX = cursor.X - this.OldCursor.X;
                int deltaY = cursor.Y - this.OldCursor.Y;

                // 보드 이동
                Point currentLocation = this.BoardView.BoardLocation;
                this.BoardView.BoardLocation = new Point(currentLocation.X + deltaX,
                    currentLocation.Y + deltaY);

                // 이펙트 이동
                this.EffectDirector.MoveAllEffect(deltaX, deltaY);
            }

            this.OldCursor = cursor;
        }

        private void OnKeyDown(Keys key)
        {
            this.WhenKeyDown(key);
        }

        private void OnMouseWheelScroll(int delta)
        {
            int deltaScale = 0;
            
            if (delta < 0)
            {
                deltaScale = -2;
            }
            else if (delta > 0)
            {
                deltaScale = 2;
            }


            // 보드 크기 변경하면서 카메라 위치 변화값 얻어옴.
            int deltaCamX = 0, deltaCamY = 0;

            this.BoardView.ChangeScale(deltaScale, this.OldCursor,
                out deltaCamX, out deltaCamY);
            

            // 이펙트 크기 변경
            this.EffectDirector.AddScaleAllEffect(deltaScale);
        }

        private void OnInputText(string text)
        {
            this.WhenInputText(text);
        }

        private void OnRequestUserColor(string userName)
        {
            this.WhenRequestUserColor(userName);
        }

        private void OnLongLeftClick(Point cursor)
        {
            this.WhenLongLeftClick(cursor, ViewPointToTileIndex(cursor));


            this.OldCursor = cursor;
        }

        private void OnInputMail(string targetName, string message)
        {
            this.WhenInputMail(targetName, message);
        }

        private void OnInputCopApplicationForm(string name, Action<Network.RegisterCompanyResults> callbackAsync)
        {
            this.WhenInputCopApplicationForm(name, callbackAsync);
        }

        private void OnDevelopTech(string companyName, string techName, List<Command> program,
            Action<Network.DevelopTechResults> callbackAsync)
        {
            this.WhenDevelopTech(companyName, techName, program, callbackAsync);
        }

        private void OnDiscardTech(string companyName, string techName, Action callbackAsync)
        {
            this.WhenDiscardTech(companyName, techName, callbackAsync);
        }

        private void OnProduceProduct(string companyName, string techName, Action callbackAsync)
        {
            this.WhenProduceProduct(companyName, techName, callbackAsync);
        }

        private void OnDiscardProduct(string companyName, int productIndex, Action callbackAsync)
        {
            this.WhenDiscardProduct(companyName, productIndex, callbackAsync);
        }

        private void OnRequestTechProgram(string companyName, string techName,
            Action<string, string, string> callbackAsync)
        {
            this.WhenRequestTechProgram(companyName, techName, callbackAsync);
        }

        private void OnSellTech(string companyName, string techName,
            int price, string targetUser)
        {
            this.WhenSellTech(companyName, techName, price, targetUser);
        }

        private void OnBuyTech(string sellerName, string techName,
            int price, string buyerName)
        {
            this.WhenBuyTech(sellerName, techName, price, buyerName);
        }

        private void OnSellProduct(string companyName, int productIndex,
            int price, string targetUser)
        {
            this.WhenSellProduct(companyName, productIndex, price, targetUser);
        }

        private void OnBuyProduct(string sellerName, string productName,
            int price, string buyerName)
        {
            this.WhenBuyProduct(sellerName, productName, price, buyerName);
        }

        //#####################################################################################

        public abstract void Initialize();
        public abstract void Update();

        protected abstract void WhenCursorChanged(Point cursor, Point tile);
        protected abstract void WhenLeftDown(Point cursor, Point tile);
        protected abstract void WhenLeftUp(Point cursor, Point tile);
        protected abstract void WhenRightDown(Point cursor, Point tile);
        protected abstract void WhenRightUp(Point cursor, Point tile);
        protected abstract void WhenCursorMove(Point cursor, Point tile);
        protected abstract void WhenKeyDown(Keys key);
        protected abstract void WhenInputText(string text);
        protected abstract void WhenRequestUserColor(string userName);
        protected abstract void WhenLongLeftClick(Point cursor, Point tile);
        protected abstract void WhenInputMail(string targetName, string message);
        protected abstract void WhenInputCopApplicationForm(string name, Action<Network.RegisterCompanyResults> callbackAsync);
        protected abstract void WhenDevelopTech(string companyName, string techName, List<Command> program,
            Action<Network.DevelopTechResults> callbackAsync);
        protected abstract void WhenDiscardTech(string companyName, string techName, Action callbackAsync);
        protected abstract void WhenProduceProduct(string companyName, string techName, Action callbackAsync);
        protected abstract void WhenDiscardProduct(string companyName, int productIndex, Action callbackAsync);
        protected abstract void WhenRequestTechProgram(string companyName, string techName,
            Action<string, string, string> callbackAsync);
        protected abstract void WhenSellTech(string companyName, string techName,
            int price, string targetUser);
        protected abstract void WhenBuyTech(string sellerName, string techName,
            int price, string buyerName);
        protected abstract void WhenSellProduct(string companyName, int productIndex,
            int price, string targetUser);
        protected abstract void WhenBuyProduct(string sellerName, string productName,
            int price, string buyerName);

        //#####################################################################################

        protected abstract GameBoard GetGameBoard();

        protected void ViewPointToTileIndex(Point viewPoint, out int indexX, out int indexY)
        {
            var gameBoard = this.GetGameBoard();


            var boardPos = this.BoardView.BoardLocation;
            int tileSize = this.BoardView.TileSize;


            var realX = viewPoint.X - boardPos.X;
            var realY = viewPoint.Y - boardPos.Y;

            indexX = realX / tileSize;
            indexY = realY / tileSize;

            if (realX < 0)
                --indexX;
            if (realY < 0)
                --indexY;
        }

        protected Point ViewPointToTileIndex(Point viewPoint)
        {
            int x, y;
            this.ViewPointToTileIndex(viewPoint, out x, out y);

            return new Point(x, y);
        }

        protected Tile GetTileContains(Point viewPoint)
        {
            var gameBoard = this.GetGameBoard();
            

            int x, y;
            this.ViewPointToTileIndex(viewPoint, out x, out y);


            if (gameBoard.Board.ContainsItemAt(x, y))
            {
                return gameBoard.Board.GetItemAt(x, y);
            }


            return null;
        }
    }
}
