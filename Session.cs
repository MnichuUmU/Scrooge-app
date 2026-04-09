using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrooge_app
{
    public static class Session
    {
        public static event EventHandler OnNotifyRefresh;

        public static void NotifyRefresh()
        {
            if (OnNotifyRefresh != null)
            {
                OnNotifyRefresh(null, EventArgs.Empty);
            }
        }
    }
}
