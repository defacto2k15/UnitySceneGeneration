using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Utils
{
    class TransformUtils
    {
        public static Matrix4x4 GetLocalToWorldMatrix(Vector3 position)
        {
            UtilsGameObject.SigletonObject.transform.position = position;
            UtilsGameObject.SigletonObject.transform.eulerAngles = new Vector3(0,0,0);
            UtilsGameObject.SigletonObject.transform.localScale = new Vector3(1,1,1);
            return UtilsGameObject.SigletonObject.transform.localToWorldMatrix;
        }
    }
}
