using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assets
{
    class HeightmapFile
    {

        public void loadFile(string filePath, int inMapWidth)
        {
            mapWidth = inMapWidth;
            const int bytesPerPixel = 2;

            byte[] fileByteArray = File.ReadAllBytes(filePath);
            if (fileByteArray.Length != mapWidth * mapWidth * bytesPerPixel)
            {
                throw new ArgumentException("Ilosc bajtow w pliku jest zla");
            }
            ushort[] fileHeightArray = new ushort[fileByteArray.Length / 2];

            Buffer.BlockCopy(fileByteArray, 0, fileHeightArray, 0, fileByteArray.Length);

            fileMaxValue = fileHeightArray.Max();
            fileMinValue = fileHeightArray.Min();

            heightData = new float[mapWidth, mapWidth];
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    heightData[i, j] = ((float)fileHeightArray[i * mapWidth + j] - (float)fileMinValue) / (float)delta;
                    //heightData[i, j] = (float)10*i+j / (float)mapWidth*2 * delta;
                }
            }
        }

        public HeightmapArray getHeightSubmap(int xOffset, int yOffset, int submapWidth)
        {
            if (xOffset + submapWidth > mapWidth)
            {
                throw new ArgumentException("xOffset + submapWidth > mapWidth");
            }
            if (xOffset + submapWidth > mapWidth)
            {
                throw new ArgumentException("yOffset + submapWidth > mapWidth");
            }
            int submapWorkingWidth = submapWidth +1 ;

            // last row and column will not be filled - they are used to smoothly join one heightmap with its neighbours
            float[,] heightSubmap = new float[submapWorkingWidth, submapWorkingWidth];
            for (int i = 0; i <  submapWidth; i++)
            {
                Array.Copy(heightData, (i + yOffset) * mapWidth + xOffset, heightSubmap, i * submapWorkingWidth, submapWidth);
            }
            return new HeightmapArray(heightSubmap);
        }

        public void MirrorReflectHeightDataInXAxis()
        {
            float[] buffer = new float[mapWidth];
            for( int i = 0; i < heightData.GetLength(0); i++){
                System.Buffer.BlockCopy(heightData, i * heightData.GetLength(1) * sizeof(float), buffer, 0, heightData.GetLength(0) * sizeof(float));
                Array.Reverse(buffer);
                System.Buffer.BlockCopy(buffer, 0, heightData, i * heightData.GetLength(1) * sizeof(float), heightData.GetLength(0) * sizeof(float));
            }
            
        }

        private int mapWidth;
        private float[,] heightData;

        public HeightmapArray Heightmap { get { return new HeightmapArray(heightData); } }
        

        private ushort fileMaxValue { get;  set; }
        private ushort fileMinValue { get;  set; }
        private ushort delta { get { return (ushort)(fileMaxValue - fileMinValue); } }
    }
}
