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
    }
}
