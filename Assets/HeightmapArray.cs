using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    class HeightmapArray
    {
        private float[,] _array;
        public HeightmapArray(float[,] array)
        {
            this._array = array;
            Preconditions.AssertArgumentIs(array.GetLength(0) == array.GetLength(1), "array must be square");
            Preconditions.AssertArgumentIs(MathHelp.IsPowerOfTwo(Width-1), "array width must be power of two +1");
        }

        public int Width { get { return _array.GetLength(0); } }
        public int WorkingWidth { get { return Width - 1; } }
        public float[,] HeightmapAsArray { get { return _array; } }
        public int UnityBaseHeightmapWidth { get { return 256 / WorkingWidth; } }

        public HeightmapArray simplyfy(int lodFactor)
        {
            Preconditions.AssertArgumentIs(lodFactor > 0, "lodFactor must be bigger than 0");
            int newWorkingWidth = WorkingWidth / (int)Math.Pow(2, lodFactor - 1);
            Preconditions.AssertArgumentIs(newWorkingWidth >= 32, "Minimal heightmap dimension must be at least 32, and would be " + newWorkingWidth);
            return new HeightmapArray(simplyfy(_array, newWorkingWidth));
        }

        public void SetHeight(int x, int y, float newValue)
        {
            _array[x, y] = newValue;
        }

        public float GetHeight(int x, int y){
            return _array[x,y];
        }

        private float[,] simplyfy(float[,] heightmap, int newWorkingWidth)
        {
            if (newWorkingWidth == WorkingWidth)
            {
                return heightmap;
            }

            int newPixelWidth = WorkingWidth / newWorkingWidth;

            var newHeightmap = new float[newWorkingWidth +1, newWorkingWidth +1];

            for (int i = 0; i < newWorkingWidth; i++)
            {
                for (int j = 0; j < newWorkingWidth; j++)
                {
                    newHeightmap[i, j] = SubarraySum(i, j, newPixelWidth, heightmap);
                }
            }
            return newHeightmap;
        }

        private float SubarraySum(int i, int j, int subarrayWidth, float[,] heightmap)
        {
            float sum = 0;
            for (int k = 0; k < subarrayWidth; k++)
            {
                for (int l = 0; l < subarrayWidth; l++)
                {
                    sum += heightmap[i * subarrayWidth + k, j * subarrayWidth + l];
                }
            }
            return sum / (subarrayWidth * subarrayWidth);
        }
    }
}
