using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.Utilities
{
    public class HandleUtility
    {

        public HashSet<int> ClaimedHandles = new HashSet<int>();
        public static HashSet<int> CachedHandles = new HashSet<int>();

        public int GenerateHandle()
        {
            lock (ClaimedHandles)
            {
                if (ClaimedHandles.Count == 0)
                {
                    ClaimedHandles.Add(0);

                    return 0;
                }

                int maxId = ClaimedHandles.Max() + 1;

                ClaimedHandles.Add(maxId);

                return maxId;
            }
        }

        public void ReleaseHandle(int handle)
        {
            lock (ClaimedHandles) 
            {
                ClaimedHandles.Remove(handle);
            }

            lock (CachedHandles)
            {
                CachedHandles.Add(handle);
            }
        }

    }
}
