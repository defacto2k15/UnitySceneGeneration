using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class HeightmapPosition
    {
        private int xPos;
        private int yPos;
        private int heightmapSizeWidth;

        public HeightmapPosition(int xPos, int yPos, int heightmapSizeWidth)
        {
            Preconditions.AssertArgumentIs(xPos >= 0 && xPos < heightmapSizeWidth, " xPos must be between 0 and heightmapSizeWidth");
            Preconditions.AssertArgumentIs(yPos >= 0 && yPos < heightmapSizeWidth, " yPos must be between 0 and heightmapSizeWidth");
            Preconditions.AssertArgumentIs(MathHelp.IsPowerOfTwo(heightmapSizeWidth), "heightmapSizeWidth must be power of two");

            this.xPos = xPos;
            this.yPos = yPos;
            this.heightmapSizeWidth = heightmapSizeWidth;
        }

        public HeightmapPosition GetPositionOnOtherHeightmap(int otherHeightmapWidth)
        {
            if (otherHeightmapWidth > heightmapSizeWidth)
            {
                Debug.Log(" Prawdopodobnie lod jest brany z bardziej skomplikowanej heightmapy do mniej skomplikowanej, to jest blad");
                return new HeightmapPosition(0,0, otherHeightmapWidth);
            }
            int divisor = heightmapSizeWidth / otherHeightmapWidth;

            return new HeightmapPosition((int)Math.Floor((decimal)(xPos / divisor)), (int)Math.Floor((decimal)(yPos / divisor)), otherHeightmapWidth);
        }

        public int X { get { return xPos; } }
        public int Y { get { return yPos; } }
    }
}
