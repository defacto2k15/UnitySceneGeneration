using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Utils
{
    class UtilsGameObject : MonoBehaviour
    {
        private static GameObject _sigletonObject;

        private void OnStart()
        {
            if (_sigletonObject == null)
            {
                _sigletonObject = new GameObject("singletonUtilsGameObject");
            }
           
        }

        public static GameObject SigletonObject
        {
            get
            {
                if (_sigletonObject == null)
                {
                    _sigletonObject = new GameObject("singletonUtilsGameObject");
                }
                return _sigletonObject;
            }
        }
    }
}
