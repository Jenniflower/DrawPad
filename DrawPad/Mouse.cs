using System.Collections.Generic;
using System.Drawing;

namespace DrawPad
{
    public class LineMouseMode : MouseMode
    {
        public LineMouseMode() { }

        public override void NextMouseState()
        {
            if (_pointList.Count < 1)
                _mouseState = MouseState.WaitNextPoint;
            else
            {
                _mouseState = MouseState.WaitLastPoint;
            }
        }
        public override void AddShape(IDrawPad pad)
        {
            pad.Add(new Line(_pointList[0], _pointList[1]));
        }
    }

    public class PolygonMouseMode : MouseMode
    {
        public PolygonMouseMode() { }

        public override void NextMouseState()
        {
            if (_pointList.Count < 3) 
            {    _mouseState = MouseState.WaitNextPoint;
                return;
            }


            if (_pointList[_pointList.Count-1].Equals(_pointList[_pointList.Count-2]))
                _mouseState = MouseState.End;
            else
            {
                _mouseState = MouseState.WaitNextPoint;
            }

        }
        public override void AddShape(IDrawPad pad)
        {
            pad.Add(new Polygon(_pointList));
        }
    }
    
    public class TriangleMouseMode : MouseMode
    {

        public TriangleMouseMode()
        {
        }
        public override void NextMouseState()
        {
            if (_pointList.Count < 2)
                _mouseState = MouseState.WaitNextPoint;
            else
            {
                _mouseState = MouseState.WaitLastPoint;
            }
        }
        public override void AddShape(IDrawPad pad)
        {
            pad.Add(new Triangle(_pointList[0], _pointList[1], _pointList[2]));
        }
    }

    public class Mouse : IMouse
    {
        private MouseMode _mouseMode ;
        private readonly IDrawPad _drawPad;

  
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
                case "polygon":
                    _mouseMode = new PolygonMouseMode();
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

    public enum MouseState
    {
        None,
        WaitNextPoint,
        WaitLastPoint,
        End
    }

    public class MouseMode
    {
        public MouseMode()
        {
            _mouseState = MouseState.WaitNextPoint;
        }
        protected List<Point> _pointList = new List<Point>();  
        protected MouseState _mouseState = MouseState.None;
        public virtual void OnMouseClick(Point location,IDrawPad pad)
        {
            switch (_mouseState)
            {
                case MouseState.WaitNextPoint:
                    _pointList.Add(location);
                    NextMouseState();
                    if (_mouseState == MouseState.End)
                    {
                        AddShape(pad);
                        ResetCache();
                        _mouseState = MouseState.WaitNextPoint;
                    }
                    break;
                case MouseState.WaitLastPoint:
                    _pointList.Add(location);
                    AddShape(pad);
                    ResetCache();
                    _mouseState = MouseState.WaitNextPoint;
                    break;
            }

        }

        private void ResetCache()
        {
            _pointList.Clear();
        }

        public virtual void NextMouseState()
        {
            throw new System.NotImplementedException();
        }
        public virtual void AddShape(IDrawPad pad)
        {
            throw new System.NotImplementedException();
        }
    }
}