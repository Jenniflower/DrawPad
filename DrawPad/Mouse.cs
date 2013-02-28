using System.Drawing;

namespace DrawPad
{
    public class Mouse : IMouse
    {
        private ShapeMode _shapeMode ;
        private TriangleShapeMode.TriangleDrawer _triangleDrawer = TriangleShapeMode.TriangleDrawer.None;
        private Point _begin;
        private Point _end;
        private readonly IDrawPad _drawPad;

        private class LineShapeMode : ShapeMode
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
            public LineShapeMode()
            {
                _drawer = LineDrawer.WaitBeginPoint;
            }
            public override void OnMouseClick(Point location,IDrawPad pad)
            {
                switch (_drawer)
                {
                    case LineDrawer.WaitBeginPoint:
                        _begin = location;
                        _drawer = LineShapeMode.LineDrawer.WaitEndPoint;
                        break;

                    case LineDrawer.WaitEndPoint:
                        _end = location;
                        _drawer = LineShapeMode.LineDrawer.WaitBeginPoint;
                        pad.Add(new Line(_begin, _end));
                        break;
                }

            }
        }

        private class TriangleShapeMode : ShapeMode
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
            public TriangleShapeMode()
            {
                _drawer = TriangleShapeMode.TriangleDrawer.WaitFirstPoint;
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
                    _shapeMode = new LineShapeMode();
                    return true;
                case "triangle":
                    _shapeMode = new TriangleShapeMode();
                    return true;
                case "exit":
                    return false;
                default:
                    throw new InvalidCommandException(command + " is not recognized.");
            }
        }

        public void OnMouseClick(Point location)
        {
            if (_shapeMode == null)
            {
                throw new InvalidCommandException("Draw line");
            }
            _shapeMode.OnMouseClick(location, _drawPad);
            
        }
    }

    internal class ShapeMode
    {
        public virtual void OnMouseClick(Point location,IDrawPad pad)
        {

        }
    }
}