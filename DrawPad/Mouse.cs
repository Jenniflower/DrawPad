using System;
using System.Collections.Generic;
using System.Drawing;

namespace DrawPad
{
    public class Mouse : IMouse
    {
        private readonly DrawerContext _drawerContext;
        private readonly IDrawPad _drawPad;
        public Mouse(IDrawPad drawPad)
        {
            _drawPad = drawPad;
            _drawerContext = new DrawerContext(drawPad, new CommandWaitDrawerState());
        }

        public IDrawPad DrawPad
        {
            get { return _drawPad; }
        }

        public void Process(string command)
        {
            _drawerContext.Input = command;
            _drawerContext.Request();
        }

        public DrawerContext Context
        {
            get { return _drawerContext; }
        }
        //public DrawerState OnMouseClick(Point location)
        //{
        //    _drawerContext.CurrentPoint = location;
        //    _drawerContext.Request();
        //    return _drawerContext.CurrentDrawerState;
        //}
    }

  
    
}