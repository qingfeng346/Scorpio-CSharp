#if SCORPIO_DEBUG
using System.Collections.Generic;
using System;
using Scorpio.Tools;
namespace Scorpio {
    public partial class Script {
        public void CollectLeak(out HashSet<(WeakReference, string)> leak, out int globalCount, out int total) {
            ScorpioProfiler.CollectLeak(this, out leak, out globalCount, out total);
        }
    }
}
#endif