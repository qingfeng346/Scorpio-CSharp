#if SCORPIO_DEBUG
using System.Collections.Generic;
using System;
using Scorpio.Tools;
namespace Scorpio {
    public partial class Script {
        public void CollectLeak(out HashSet<(WeakReference, string)> set, out int count) {
            ScorpioProfiler.CollectLeak(this, out set, out count);
        }
    }
}
#endif