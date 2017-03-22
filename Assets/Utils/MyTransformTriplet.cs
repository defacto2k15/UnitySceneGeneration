using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Utils
{
    class MyTransformTriplet
    {
        private Vector3 _position;
        private Vector3 _rotation;
        private Vector3 _scale;

        public MyTransformTriplet(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            _position = position;
            _rotation = rotation;
            _scale = scale;
        }

        public static MyTransformTriplet FromGlobalTransform(Transform transform)
        {
            return new MyTransformTriplet(transform.position, MyMathUtils.DegToRad(transform.eulerAngles), transform.lossyScale);
        }

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector3 Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public Vector3 Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public void SetTransformTo(Transform trans)
        {
            trans.localScale = Scale;
            trans.localEulerAngles = MyMathUtils.RadToDeg(Rotation);
            trans.localPosition = Position;
        }
    }
}
