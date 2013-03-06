using System.Collections.Generic;
using System.Drawing;

namespace DrawPad
{
    public class LineDrawerStateWaitNext : DrawerState
    {
        public LineDrawerStateWaitNext() { }

        public override DrawerState HandleRequest(DrawerContext context)
        {
            context.AddCurrentPoint();
            return new LineDrawerStateWaitLast();
        }
    }

    public class LineDrawerStateWaitLast : DrawerState
    {
        public LineDrawerStateWaitLast():base() { }

        public override DrawerState HandleRequest( DrawerContext context)
        {
            context.AddCurrentPoint();
            context.AddLine();
            return new LineDrawerStateWaitNext();
        }
    }

    public class TriangleDrawerStateWaitNext : DrawerState
    {
        public TriangleDrawerStateWaitNext():base() { }

        public override DrawerState HandleRequest(DrawerContext context)
        {
            context.AddCurrentPoint();
            if (context.Points.Count < 2)
                return new TriangleDrawerStateWaitNext();
            else
            {
                return new TriangleDrawerStateWaitLast();
            }
        }
    }

    public class TriangleDrawerStateWaitLast : DrawerState
    {
        public TriangleDrawerStateWaitLast():base() { }

        public override DrawerState HandleRequest(DrawerContext context)
        {
            context.AddCurrentPoint();
            context.AddTrigangle();
            return new TriangleDrawerStateWaitNext();
        }
    }

    public class PolygonDrawerStateWaitNext : DrawerState
    {
        public PolygonDrawerStateWaitNext( ):base() { }

        public override DrawerState HandleRequest(DrawerContext context)
        {
            context.AddCurrentPoint();
            if (context.Points.Count < 3)
            {
                return new PolygonDrawerStateWaitNext();
            }

            if (!context.IsLastTwoPointsSame())
                return new PolygonDrawerStateWaitNext();
            else
            {
                context.CurrentDrawerState = new PolygonDrawerStateEnd();
                return context.Request();
            }
        }
    }
    public class PolygonDrawerStateEnd : DrawerState
    {
        public PolygonDrawerStateEnd():base() { }

        public override DrawerState HandleRequest( DrawerContext context)
        {
            context.AddPolygon();
            return new PolygonDrawerStateWaitNext();  
        }
 }

    public class DrawerContext
    {
        private DrawerState _drawerState;
        private List<Point> _points;
        private Point _currentPoint;

        private IDrawPad _pad;
        private string _input;
        public DrawerContext(IDrawPad drawPad, DrawerState drawerState)
        {
            _pad = drawPad;
            _drawerState = drawerState;
            _points = new List<Point>();
        }
        public void AddLine()
        {
            System.Diagnostics.Trace.Assert(Points.Count==2);
            _pad.Add(new Line(_points[0],_points[1]));
            _points.Clear();
        }

        public void AddTrigangle()
        {
            System.Diagnostics.Trace.Assert(Points.Count == 3);
            _pad.Add(new Triangle(_points[0], _points[1],Points[2]));
            _points.Clear();
        }
        public void AddPolygon()
        {
            System.Diagnostics.Trace.Assert(Points.Count >=4);
            _pad.Add(new Polygon(_points));
            _points.Clear();
        }
        public DrawerState CurrentDrawerState
        {
            get { return _drawerState; }
            set { _drawerState = value; }
        }
        public bool IsLastTwoPointsSame()
        {
            return _points[_points.Count - 1].Equals(_points[_points.Count - 2]);

        }
        public Point CurrentPoint
        {
            get { return _currentPoint; }
            set { _currentPoint = value; }
        }
        public List<Point> Points
        {
            get { return _points; }
        }
        public string Input
        {
            get { return _input; }
            set { _input = value; }
        }
        public void AddCurrentPoint()
        {
            _points.Add(_currentPoint);
        }
 
        public DrawerState Request()
        {
            _drawerState = CurrentDrawerState.HandleRequest(this);
            return _drawerState;
        }
    }

    public class Mouse : IMouse
    {
        private readonly DrawerContext _drawerContext;
        private readonly IDrawPad _drawPad;
        public Mouse(IDrawPad drawPad)
        {
            _drawPad = drawPad;
            _drawerContext = new DrawerContext(drawPad, new WaitCommandDrawerState());
        }

        public IDrawPad DrawPad
        {
            get { return _drawPad; }
        }

        public DrawerState Process(string command)
        {
            _drawerContext.Input = command;
            _drawerContext.Request();
            return _drawerContext.CurrentDrawerState;
        }

        public DrawerState OnMouseClick(Point location)
        {
            _drawerContext.CurrentPoint = location;
            _drawerContext.Request();
            return _drawerContext.CurrentDrawerState;
        }
    }

    public class WaitCommandDrawerState : DrawerState
    {
        public WaitCommandDrawerState():base()
        {
        }

        public override DrawerState HandleRequest(DrawerContext context)
        {
            switch (context.Input)
            {
                case "line":
                    context.CurrentDrawerState = new LineDrawerStateWaitNext();
                    break;
                case "triangle":
                    context.CurrentDrawerState = new TriangleDrawerStateWaitNext();
                    break;
                case "polygon":
                    context.CurrentDrawerState = new PolygonDrawerStateWaitNext();
                    break;
                case "exit":
                    context.CurrentDrawerState = null;
                    break;
                default:
                    throw new InvalidCommandException(context.Input + " is not recognized.");
            }
            return context.CurrentDrawerState;
        }

    }

    public class DrawerState
    {
        public DrawerState()
        {
        }
        public virtual DrawerState HandleRequest( DrawerContext context)
        {
            return null;
        }
        
    }
}