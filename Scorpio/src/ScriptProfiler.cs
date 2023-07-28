#if SCORPIO_DEBUG
using System.Collections.Generic;
using System;
using Scorpio.Tools;
namespace Scorpio {
    public partial class Script {
        public void CollectLeak(out HashSet<(WeakReference, string)> leak, out int globalCount, out int total) {
            ScorpioProfiler.CollectLeak(this, out leak, out globalCount, out total);
        }
        public void FindReference(ulong id, out HashSet<WeakReference> set, out List<string> custom) {
            ScorpioProfiler.FindReference(id, out set, out custom);
        }
    }
}
#endif