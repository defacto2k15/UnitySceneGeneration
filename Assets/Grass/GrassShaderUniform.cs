using UnityEngine;

namespace Assets.Grass
{
    class GrassShaderUniform<T> 
    {
        private readonly GrassShaderUniformName _name;
        private readonly T _value;

        public GrassShaderUniform(GrassShaderUniformName name, T value)
        {
            _name = name;
            _value = value;
        }

        public string Name
        {
            get { return _name.ToString(); }
        }

        public T Get()
        {
            return _value;
        }
    }
}