using System;
using System.Collections.Generic;
using System.Drawing;

namespace DrawPad
{
    public class DrawPad : IDrawPad
    {
        private readonly List<Shape> _shapes = new List<Shape>();
        private readonly Mouse _mouse;

        public DrawPad()
        {
            _mouse = new Mouse(this);
        }

        public void OnPaint()
        {
            if (_shapes.Count == 0)
            {
                Console.WriteLine("There's nothing on canvas.");
                return;
            }

            Console.WriteLine("Canvas:");
            _shapes.ForEach(s => Console.WriteLine(s.ToString()));
        }

        public void Run()
        {
            do
            {
                OnPaint();
                if (!ProcessCommand())
                    break;
            } while (true);
        }

        public void Add(Shape shape)
        {
            _shapes.Add(shape);
            OnPaint();
        }

        private bool ProcessCommand()
        {
            try
            {
                do
                {
                    Console.WriteLine(_mouse.Context.CurrentDrawerState.StateTip);
                    string command = Console.ReadLine();
                    _mouse.Process(command);
                    if (_mouse.Context.CurrentDrawerState as CommandWaitDrawerState != null)
                        return true;
                } while (true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
            //Console.WriteLine("Please enter a command:");
            //string command = Console.ReadLine();
            //DrawerState mouseState = _mouse.Process(command);
            //if (mouseState == null)
            //    return false;
            //do
            //{
            //    Console.WriteLine("Please enter a point:");
            //    command = Console.ReadLine();

            //    Point location;
            //    if (!TryGetPoint(command, out location))
            //        break;

            //    _mouse.OnMouseClick(location);
            //} while (true);

            return true;
        }

       
    }
}