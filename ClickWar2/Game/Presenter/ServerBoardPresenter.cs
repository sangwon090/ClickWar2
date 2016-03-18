using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using ClickWar2.Game.Network;

namespace ClickWar2.Game.Presenter
{
    public class ServerBoardPresenter : BoardPresenter
    {
        public ServerBoardPresenter()
        {

        }

        //#####################################################################################

        public GameServer Server
        { get; set; } = null;

        //#####################################################################################

        public override void Initialize()
        {

        }

        public override void Update()
        {

        }

        protected override void WhenCursorChanged(Point cursor, Point tile)
        {
            this.BoardView.Cursor = cursor;
            this.BoardView.TileCursor = tile;
        }

        protected override void WhenLeftDown(Point cursor, Point tile)
        {

        }

        protected override void WhenLeftUp(Point cursor, Point tile)
        {

        }

        protected override void WhenRightDown(Point cursor, Point tile)
        {
            
        }

        protected override void WhenRightUp(Point cursor, Point tile)
        {
            
        }

        protected override void WhenCursorMove(Point cursor, Point tile)
        {
            
        }

        protected override void WhenKeyDown(Keys key)
        {

        }

        protected override void WhenInputText(string text)
        {

        }

        protected override void WhenRequestUserColor(string userName)
        {
            var user = this.Server.UserDirector.GetAccount(userName);

            if (user != null)
            {
                this.BoardView.SetUserColor(userName, user.UserColor);
            }
        }

        protected override void WhenLongLeftClick(Point cursor, Point tile)
        {

        }

        protected override void WhenInputMail(string targetName, string message)
        {

        }

        protected override void WhenInputCopApplicationForm(string name, Action<RegisterCompanyResults> callbackAsync)
        {

        }

        protected override void WhenDevelopTech(string companyName, string techName, List<Command> program,
            Action<DevelopTechResults> callbackAsync)
        {

        }

        protected override void WhenDiscardTech(string companyName, string techName, Action callbackAsync)
        {

        }

        protected override void WhenProduceProduct(string companyName, string techName, Action callbackAsync)
        {

        }

        protected override void WhenDiscardProduct(string companyName, int productIndex, Action callbackAsync)
        {

        }

        protected override void WhenRequestTechProgram(string companyName, string techName,
            Action<string, string, string> callbackAsync)
        {

        }

        protected override void WhenSellTech(string companyName, string techName, int price, string targetUser)
        {

        }

        protected override void WhenBuyTech(string sellerName, string techName, int price, string buyerName)
        {

        }

        protected override void WhenSellProduct(string companyName, int productIndex, int price, string targetUser)
        {
            
        }

        protected override void WhenBuyProduct(string sellerName, string productName, int price, string buyerName)
        {
            
        }

        //#####################################################################################

        protected override GameBoard GetGameBoard()
        {
            return this.Server.GameBoard;
        }
    }
}
