using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipData
{
    namespace Utilities
    {
        public class intPos2EqualityComparer : IEqualityComparer<intPos2>
        {
            public bool Equals(intPos2 x, intPos2 y)
            {
                if (x == null && y == null)
                    return false;
                else if (x == null || y == null)
                    return false;
                else if (x.Column == y.Column && x.Row == y.Row)
                    return true;
                else
                    return false;
            }

            public int GetHashCode(intPos2 obj)
            {
                int hashCode = obj.Column + obj.Row;
                return hashCode.GetHashCode();
            }
        }

        
    }
}
