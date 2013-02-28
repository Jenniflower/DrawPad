using System.Drawing;
using DrawPad;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DrawPadTest
{
    [TestClass]
    public class Mouse_OnClikc
    {
        private readonly Mock<IDrawPad> _mockPad = new Mock<IDrawPad>();
        private Mouse _sut;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new Mouse(_mockPad.Object);
        }

        [TestMethod]
        public void given_draw_line_and_2_click_should_return_a_valid_line_object()
        {
            var expect = new Line(new Point(1, 1), new Point(1, 2));

            _sut.Process("line");
            _sut.OnMouseClick(new Point(1, 1));
            _sut.OnMouseClick(new Point(1, 2));

            _mockPad.Verify(o => o.Add(expect), Times.Once());
        }

        [TestMethod]
        public void given_draw_line_and_1_click_should_wait_for_click()
        {
            _sut.Process("line");
            _sut.OnMouseClick(new Point(1, 1));

            _mockPad.Verify(o => o.Add(It.IsAny<Shape>()), Times.Never());
        }

        [TestMethod]
        public void given_draw_triangle_and_3_click_should_return_valid_triangle_object()
        {
            var expect = new Triangle(new Point(2, 2), new Point(1, 1), new Point(3, 1));
            _sut.Process("triangle");
            _sut.OnMouseClick(new Point(2,2));
            _sut.OnMouseClick(new Point(1,1));
            _sut.OnMouseClick(new Point(3,1));

            _mockPad.Verify(o => o.Add(It.IsAny<Shape>()), Times.Once());
        }
    }
}