using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Utils
{
    class MyMathUtils
    {
        static public Vector3 RadToDeg(Vector3 input)
        {
            return new Vector3(Mathf.Rad2Deg * input.x, Mathf.Rad2Deg * input.y, Mathf.Rad2Deg * input.z);
        }

        public static Vector3 DegToRad(Vector3 input)
        {
            return new Vector3(Mathf.Deg2Rad * input.x, Mathf.Deg2Rad * input.y, Mathf.Deg2Rad * input.z);
        }

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 eulerAngles) {
            return Quaternion.Euler(eulerAngles) * (point - pivot) + pivot;
        }

        // returns between 0 and 1
        public static float InvLerp(float min, float max, float value)
        {
            Preconditions.Assert(min >= value, string.Format("Min {0} max {1} value {2} E1",min,max,value));
            Preconditions.Assert(max <= value, string.Format("Min {0} max {1} value {2} E2", min, max, value));
            return (value - min)/(max - min);
        }

        public static Vector3 MultiplyMembers(Vector3 inValue, Vector3 multiplier)
        {
            return new Vector3( inValue.x * multiplier.x, inValue.y * multiplier.y, inValue.z * multiplier.z);
        }
    }
}
