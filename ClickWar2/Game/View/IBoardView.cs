using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace ClickWar2.Game.View
{
    public interface IBoardView
    {
        event Action<Point> WhenLeftDown;
        event Action<Point> WhenLeftUp;
        event Action<Point> WhenRightDown;
        event Action<Point> WhenRightUp;
        event Action<Point> WhenCursorMove;
        event Action<Keys> WhenKeyDown;
        event Action<int> WhenMouseWheelScroll;
        event Action<string> WhenInputText;
        event Action<string> WhenRequestUserColor;
        event Action<Point> WhenLongLeftClick;
        event Action<string, string> WhenInputMail;
        event Action<string, Action<Network.RegisterCompanyResults>> WhenInputCopApplicationForm;
        event Action<string, string, List<Command>, Action<Network.DevelopTechResults>> WhenDevelopTech;
        event Action<string, string, Action> WhenDiscardTech;
        event Action<string, string, Action> WhenProduceProduct;
        event Action<string, int, Action> WhenDiscardProduct;
        event Action<string, string, Action<string, string, string>> WhenRequestTechProgram;
        event Action<string, string, int, string> WhenSellTech;
        event Action<string, string, int, string> WhenBuyTech;
        event Action<string, int, int, string> WhenSellProduct;
        event Action<string, string, int, string> WhenBuyProduct;

        //#####################################################################################

        Point BoardLocation
        { get; set; }

        int TileSize
        { get; set; }

        Point Cursor
        { get; set; }

        Point TileCursor
        { get; set; }

        Size ScreenSize
        { get; set; }

        //#####################################################################################

        void MoveScreenTo(int tileX, int tileY);
        void ChangeScale(int deltaTileSize, Point center,
            out int deltaCamX, out int deltaCamY);

        //#####################################################################################

        void ShowLeftDragHelper(Point center);
        void HideLeftDragHelper();

        //#####################################################################################

        void SetUserColor(string userName, Color color);
    }
}
