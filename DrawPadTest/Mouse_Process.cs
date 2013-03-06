using System.Collections.Generic;
using System.Drawing;
using DrawPad;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DrawPadTest
{
    [TestClass]
    public class Mouse_Process
    {
        private readonly Mock<IDrawPad> _mockPad = new Mock<IDrawPad>();
        private Mouse _sut;


        [TestInitialize]
        public void SetUp()
        {
            _sut = new Mouse(_mockPad.Object);
        }


        [TestMethod, ExpectedException(typeof(InvalidCommandException))]
        public void given_llne_should_throw_invalid_command_exception()
        {
            _sut.Process("llne");
        }


        [TestMethod]
        public void given_draw_line_and_2_click_should_return_a_valid_line_object()
        {
            var expect = new Line(new Point(1, 1), new Point(1, 2));

            _sut.Process("line");
            _sut.Process("1,1");
            _sut.Process("1,2");
            _mockPad.Verify(o => o.Add(expect), Times.Once());
        }

        [TestMethod]
        public void given_draw_line_and_1_click_should_wait_for_click()
        {
            _sut.Process("line");
            _sut.Process("1,1");

            _mockPad.Verify(o => o.Add(It.IsAny<Shape>()), Times.Never());
        }

        [TestMethod]
        public void given_draw_triangle_and_3_click_should_return_valid_triangle_object()
        {
            var expect = new Triangle(new Point(2, 2), new Point(1, 1), new Point(3, 1));
            _sut.Process("triangle");
            _sut.Process("2,2");
            _sut.Process("1,1");
            _sut.Process("3,1");

            _mockPad.Verify(o => o.Add(It.IsAny<Shape>()), Times.Once());
        }
        [TestMethod]
        public void given_draw_polygon_and_6_clicks_when_lasttwo_diff_should__not_return_valid_polygon_object()
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(1, 1));
            points.Add(new Point(3, 0));
            points.Add(new Point(4, 1));
            points.Add(new Point(2, 3));
            points.Add(new Point(0, 5));

            var expect = new Polygon(points);
            _sut.Process("polygon");
            _sut.Process("1,1");
            _sut.Process("3,0");
            _sut.Process("4,1");
            _sut.Process("2,3");
            _sut.Process("0,5");

            _mockPad.Verify(o => o.Add(It.IsAny<Shape>()), Times.Never());
        }
        [TestMethod]
        public void given_draw_polygon_and_6_clicks_when_lasttwo_same_should_return_valid_polygon_object()
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(1, 1));
            points.Add(new Point(3, 0));
            points.Add(new Point(4, 1));
            points.Add(new Point(2, 3));
            points.Add(new Point(0, 5));
            points.Add(new Point(0, 5));

            var expect = new Polygon(points);
            _sut.Process("polygon");
            _sut.Process("1,1");
            _sut.Process("3,0");
            _sut.Process("4,1");
            _sut.Process("2,3");
            _sut.Process("0,5");
            _sut.Process("0,5");

            _mockPad.Verify(o => o.Add(It.IsAny<Shape>()), Times.Once());
        }
    }
}
