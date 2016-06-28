using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestOnTheFlyService
{
    class TreeNoDBClick : TreeView
    {
        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.Msg == 8270)
                {
                    m.Result = IntPtr.Zero;
                    return;
                }
                if (this.CheckBoxes)
                {
                    // Suppress WM_LBUTTONDBLCLK
                    if (m.Msg == 0x203) { m.Result = IntPtr.Zero; }
                    else base.WndProc(ref m);
                }
                else
                    base.WndProc(ref m);
            }
            catch (Exception)
            {


            }

        }
    }
}
