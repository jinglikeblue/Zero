using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILDemo
{
    public class GlobalNotice
    {
        public static Action<string> onMsg = null;
    }
}
