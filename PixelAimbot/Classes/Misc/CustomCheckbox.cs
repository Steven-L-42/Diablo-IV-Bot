using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PixelAimbot.Classes.Misc
{
    [DefaultEvent("CheckedChanged")]
    public class CustomCheckbox : CheckBox
    {
        public CustomCheckbox()
        {
            FlatStyle = FlatStyle.Standard;
        }

    }
}
