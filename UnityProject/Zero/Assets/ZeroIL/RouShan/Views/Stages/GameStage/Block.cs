using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zero;
using ZeroIL.Zero;

namespace ZeroIL.RouShan
{
    class Block : AView
    {
        protected override void OnInit()
        {
            Log.CI(Log.COLOR_BLUE, gameObject.transform.position.ToString());
        }
    }
}
