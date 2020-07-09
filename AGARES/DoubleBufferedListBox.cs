using System.Linq;
using System.Windows.Forms;

namespace Extension
{
    public class DoubleBufferedListBox:ListBox
    {
        public DoubleBufferedListBox()
        {
            DoubleBuffered = true;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }
    }
}
