using System.Drawing;

namespace DrawPad
{
    public class Mouse : IMouse
    {
        private LineDrawer _drawer = LineDrawer.None;
        private Point _begin;
        private Point _end;
        private readonly IDrawPad _drawPad;

        enum LineDrawer
        {
            None,
            WaitBeginPoint,
            WaitEndPoint,
        }

        public Mouse(IDrawPad drawPad)
        {
            _drawPad = drawPad;
        }

        public IDrawPad DrawPad
        {
            get { return _drawPad; }
        }

        public bool Process(string command)
        {
            switch (command)
            {
                case "line":
                    _drawer = LineDrawer.WaitBeginPoint;
                    return true;
                case "exit":
                    return false;
                default:
                    throw new InvalidCommandException(command + " is not recognized.");
            }
        }

        public void OnMouseClick(Point location)
        {
            switch (_drawer)
            {
                case LineDrawer.WaitBeginPoint:
                    _begin = location;
                    _drawer = LineDrawer.WaitEndPoint;
                    break;

                case LineDrawer.WaitEndPoint:
                    _end = location;
                    _drawer = LineDrawer.WaitBeginPoint;
                    DrawPad.Add(new Line(_begin, _end));
                    break;

                default:
                    throw new InvalidCommandException("Draw line");
            }
        }
    }
}