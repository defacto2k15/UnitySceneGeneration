using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    class SubmapInfo
    {
        public int DownLeftX { get; set; }
        public int DownLeftY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int LodFactor { get; set; }

        public SubmapInfo(int downLeftX, int downLeftY, int width, int height, int lodFactor)
        {
            DownLeftX = downLeftX;
            DownLeftY = downLeftY;
            Width = width;
            Height = height;
            LodFactor = lodFactor;
        }

        public override string ToString()
        {
            return string.Format("DownLeftX: {0}, DownLeftY: {1}, Width: {2}, Height: {3}, LodFactor: {4}", DownLeftX, DownLeftY, Width, Height, LodFactor);
        }

        public int GetHeightPoint(int i, int currentHeight)
        {
            return DownLeftY + Height*i/currentHeight;
        }

        public int GetWidthPoint(int i, int currentWidth)
        {
            return DownLeftX + Width*i/currentWidth;
        }

        public Point2D DownLeftPoint
        {
            get
            {
                return new Point2D(DownLeftX, DownLeftY);
            }
        }

        public Point2D DownRightPoint
        {
            get
            {
                return new Point2D(DownLeftX+Width, DownLeftY);
            }
        }

        public Point2D TopLeftPoint
        {
            get
            {
                return new Point2D(DownLeftX, DownLeftY+Height);
            }
        }

        public Point2D TopRightPoint
        {
            get
            {
                return new Point2D(DownLeftX+Width, DownLeftY+Height);
            }
        }
    }
}
