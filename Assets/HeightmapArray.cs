using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class HeightmapArray
    {
        private float[,] _array;
        public HeightmapArray(float[,] array)
        {
            this._array = array;
        }

        public int Width { get { return _array.GetLength(0); } }
        public int WorkingWidth { get { return Width - 1; } }
        public int Height { get { return _array.GetLength(1); }}
        public int WorkingHeight { get { return Height - 1; }}

        public float[,] HeightmapAsArray { get { return _array; } }

        public HeightmapArray simplyfy(int lodFactor)
        {
            Preconditions.Assert(lodFactor > 0, "lodFactor must be bigger than 0");
            int newWorkingWidth = WorkingWidth / (int)Math.Pow(2, lodFactor - 1);
            int newWorkingHeight = WorkingHeight /(int)Math.Pow(2, lodFactor - 1); 

            Preconditions.Assert(newWorkingWidth >= 4, "Minimal heightmap  width dimension must be at least 4, and would be " + newWorkingWidth);
            Preconditions.Assert(newWorkingHeight >= 4, "Minimal heightmap  height dimension must be at least 4, and would be " + newWorkingHeight);
            if (lodFactor != 1)
            {
                return new HeightmapArray(simplyfy(newWorkingWidth, newWorkingHeight));
            }
            else
            {
                return this;
            }
        }

        public void SetHeight(int x, int y, float newValue)
        {
            _array[x, y] = newValue;
        }

        public float GetHeight(int x, int y){
            return _array[x,y];
        }

        private float[,] simplyfy( int newWorkingWidth, int newWorkingHeight)
        {
            if (newWorkingWidth == WorkingWidth && newWorkingHeight == WorkingHeight)
            {
                return _array;
            }

            int newPixelWidth = WorkingWidth / newWorkingWidth;
            int newPixelHeight = WorkingHeight/newWorkingHeight;

            var newHeightmap = new float[newWorkingWidth +1, newWorkingHeight +1];

            for (int i = 0; i < newWorkingWidth; i++)
            {
                for (int j = 0; j < newWorkingHeight; j++)
                {
                    newHeightmap[i, j] = SubarraySum(i, j, newPixelWidth, newPixelHeight);
                }
            }
            return newHeightmap;
        }

        private float SubarraySum(int i, int j, int subarrayWidth,  int subarrayHeight)
        {
            float sum = 0;
            for (int k = 0; k < subarrayWidth; k++)
            {
                for (int l = 0; l < subarrayHeight; l++)
                {
                    sum += _array[i * subarrayWidth + k, j * subarrayHeight + l];
                }
            }
            return sum / (subarrayWidth * subarrayHeight);
        }

        public HeightmapMargin GetLeftMargin()
        {
            return GetMargin((i)=> _array[0,i], Height);
        }

        public HeightmapMargin GetRightMargin()
        {
            return GetMargin((i)=> _array[WorkingWidth ,i], Height);
        }

        public HeightmapMargin GetDownMargin()
        {
            return GetMargin((i)=> _array[i,0], Width);
        }

        public HeightmapMargin GetTopMargin()
        {
            return GetMargin((i)=> _array[i,WorkingHeight ], Width);
        }

        private HeightmapMargin GetMargin(Func<int, float> elementGetter, int marginLength)
        {
            float[] outArray = new float[marginLength];
            for (int i = 0; i < marginLength; i++)
            {
                outArray[i] = elementGetter.Invoke(i);
            }
            return new HeightmapMargin(outArray);
        }

        public void SetRightMargin(HeightmapMargin margin)
        {
            AssertMarginHasProperLength(margin, WorkingHeight);
            for (int i = 0; i < Height; i++)
            {
                _array[WorkingWidth, i] = margin.MarginValues[i];
            }
        }

        public void SetLeftMargin(HeightmapMargin margin)
        {
            AssertMarginHasProperLength(margin, WorkingHeight);
            for (int i = 0; i < Height; i++)
            {
                _array[0, i] = margin.MarginValues[i];
            }
        }

        public void SetTopMargin(HeightmapMargin margin)
        {
            AssertMarginHasProperLength(margin, WorkingWidth);
            for (int i = 0; i < Width; i++)
            {
                _array[ i, WorkingHeight] = margin.MarginValues[i];
            }
        }

        public void SetBottomMargin(HeightmapMargin margin)
        {
            AssertMarginHasProperLength(margin, WorkingWidth);
            for (int i = 0; i < Width; i++)
            {
                _array[ i, 0] = margin.MarginValues[i];
            }
        }

        private void AssertMarginHasProperLength(HeightmapMargin margin, int workingLength)
        {
            Preconditions.Assert(margin.WorkingLength==workingLength,
                "Cant set margin. It has wrong length. Old working length "+workingLength+" new working length "+margin.WorkingLength);
        }

        public void SetDownLeftApexMarginHeight(float apexValue, int pixelSize)
        {
            for (int i = 0; i < pixelSize; i++)
            {
                var value = apexValue;
                if (pixelSize != 0)
                {
                    value = Mathf.Lerp(apexValue, GetHeight(pixelSize, 0), (float) i/pixelSize);
                }
                SetHeight(i,0,value);
                SetHeight(0,i,value);
            }
            for (int i = 0; i < pixelSize; i++)
            {
                var value = apexValue;
                if (pixelSize != 0)
                {
                    value = Mathf.Lerp(apexValue, GetHeight(0, pixelSize), (float)i / pixelSize);
                }
                SetHeight(0, i, value);
            }              
        }

        public void SetDownRightApexMarginHeight(float apexValue, int pixelSize)
        {
            for (int i = 0; i < pixelSize; i++)
            {
                var value = apexValue;
                if (pixelSize != 0)
                {
                    value = Mathf.Lerp(apexValue, GetHeight(WorkingWidth-pixelSize, 0), (float)i / pixelSize);
                }
                SetHeight(WorkingWidth-i, 0, value);
            }
            for (int i = 0; i < pixelSize; i++)
            {
                var value = apexValue;
                if (pixelSize != 0)
                {
                    value = Mathf.Lerp(apexValue, GetHeight(WorkingWidth, pixelSize), (float)i / pixelSize);
                }
                SetHeight(WorkingWidth, i, value);
            }
        }

        public void SetTopLeftApexMarginHeight(float apexValue, int pixelSize)
        {
            for (int i = 0; i < pixelSize; i++)
            {
                var value = apexValue;
                if (pixelSize != 0)
                {
                    value = Mathf.Lerp(apexValue, GetHeight(0,WorkingHeight - pixelSize), (float)i / pixelSize);
                }
                SetHeight(0, WorkingHeight-i, value);
            }
            for (int i = 0; i < pixelSize; i++)
            {
                var value = apexValue;
                if (pixelSize != 0)
                {
                    value = Mathf.Lerp(apexValue, GetHeight(pixelSize, WorkingHeight ), (float)i / pixelSize);
                }
                SetHeight(i, WorkingHeight, value);
            }
        }

        public void SetTopRightApexMarginHeight(float apexValue, int pixelSize)
        {
            for (int i = 0; i < pixelSize; i++)
            {
                var value = apexValue;
                if (pixelSize != 0)
                {
                    value = Mathf.Lerp(apexValue, GetHeight(WorkingWidth-pixelSize, WorkingHeight ), (float)i / pixelSize);
                }
                SetHeight(WorkingWidth-i, WorkingHeight, value);
            }
            for (int i = 0; i < pixelSize; i++)
            {
                var value = apexValue;
                if (pixelSize != 0)
                {
                    value = Mathf.Lerp(apexValue, GetHeight(WorkingWidth, WorkingHeight - pixelSize), (float)i / pixelSize);
                }
                SetHeight(WorkingWidth, WorkingHeight - i, value);
            }
        }
    }
}
