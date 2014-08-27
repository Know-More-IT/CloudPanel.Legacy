using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base
{
    public static class BaseTaskStatus
    {
        public const int Completed = 0;
        public const int NotRun = 1;
        public const int Running = 2;
        public const int Failed = 3;
    }
}
