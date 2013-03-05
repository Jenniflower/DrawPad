using System.Drawing;

namespace DrawPad
{
    public class Mouse : IMouse
    {
        private MouseMode _mouseMode ;
        private TriangleMouseMode.TriangleDrawer _triangleDrawer = TriangleMouseMode.TriangleDrawer.None;
        private Point _begin;
        private Point _end;
        private readonly IDrawPad _drawPad;

        private class LineMouseMode : MouseMode
        {
            private LineDrawer _drawer;
            private Point _begin;
            private Point _end;
            internal enum LineDrawer
            {
                None,
                WaitBeginPoint,
                WaitEndPoint,
            }
            public LineMouseMode()
            {
                _drawer = LineDrawer.WaitBeginPoint;
            }
            public override void OnMouseClick(Point location,IDrawPad pad)
            {
                switch (_drawer)
                {
                    case LineDrawer.WaitBeginPoint:
                        _begin = location;
                        _drawer = LineMouseMode.LineDrawer.WaitEndPoint;
                        break;

                    case LineDrawer.WaitEndPoint:
                        _end = location;
                        _drawer = LineMouseMode.LineDrawer.WaitBeginPoint;
                        pad.Add(new Line(_begin, _end));
                        break;
                }

            }
        }

        private class TriangleMouseMode : MouseMode
        {
            private Point _first;
            private Point _second;
            private Point _third;
            private TriangleDrawer _drawer = TriangleDrawer.None;
            internal enum TriangleDrawer
            {
                None,
                WaitFirstPoint,
                WaitSecondPoint,
                WaitThirdPoint
            }
            public TriangleMouseMode()
            {
                _drawer = TriangleMouseMode.TriangleDrawer.WaitFirstPoint;
            }

            public override void OnMouseClick(Point location, IDrawPad pad)
            {
                switch (_drawer)
                {
                    case TriangleDrawer.WaitFirstPoint:
                        _first = location;
                        _drawer = TriangleDrawer.WaitSecondPoint;
                        break;

                    case TriangleDrawer.WaitSecondPoint:
                        _second = location;
                        _drawer = TriangleDrawer.WaitThirdPoint;
                        break;
                    case TriangleDrawer.WaitThirdPoint:
                        _third = location;
                        _drawer = TriangleDrawer.WaitFirstPoint;
                        pad.Add(new Triangle(_first,_second,_third));
                        break;
                }

            }
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
                    _mouseMode = new LineMouseMode();
                    return true;
                case "triangle":
                    _mouseMode = new TriangleMouseMode();
                    return true;
                case "exit":
                    return false;
                default:
                    throw new InvalidCommandException(command + " is not recognized.");
            }
        }

        public void OnMouseClick(Point location)
        {
            if (_mouseMode == null)
            {
                throw new InvalidCommandException("Draw line");
            }
            _mouseMode.OnMouseClick(location, _drawPad);
            
        }
    }

    internal class MouseMode
    {
        public virtual void OnMouseClick(Point location,IDrawPad pad)
        {

        }
    }
}