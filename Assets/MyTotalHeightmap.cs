using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class MyTotalHeightmap
    {
        public List<MyTerrain> Submaps; 
        // todo: choose to use submap or subterrain
        public void LoadHeightmap(HeightmapFile heightmapFile, List<SubmapInfo> submapInfos, int baseInUnityLength, int totalMapWidthInBaseUnits)
        {
            //var wholeTerrainWidth = 100000;

            var submapAndHeightmapArrayList
                = (from info in submapInfos
                    let submap = 
                        heightmapFile.getHeightSubmap(info.DownLeftX, info.DownLeftY, info.Width, info.Height)
                            .simplyfy(info.LodFactor)
                    select new SubmapInfoAndHeightmapArray(info, submap)).ToList();

            SetSubmapMargins(submapAndHeightmapArrayList);
            SetSubmapApexPoint(submapAndHeightmapArrayList);

            Submaps = new List<MyTerrain>();
            foreach (var aPair  in submapAndHeightmapArrayList)
            {
                var terrain = new MyTerrain(aPair.HeightmapArray, this, aPair.SubmapInfo);
                terrain.Position = new Vector3(aPair.SubmapInfo.DownLeftX,0,aPair.SubmapInfo.DownLeftY); // DownLeftX and DownLeftY 
                terrain.Scale = new Vector3(aPair.SubmapInfo.Width,1000, aPair.SubmapInfo.Height);  //todo what with that 1000?
                terrain.Rotation = new Vector3(0, 0, 0);
                terrain.Name = ("Terrain " + aPair.SubmapInfo.ToString());
                Submaps.Add(terrain);
            }
        }

        private void SetSubmapMargins(List<SubmapInfoAndHeightmapArray> heightmaps )
        {
            foreach (var aHeightmapPair in heightmaps)
            {
                if (aHeightmapPair.SubmapInfo.DownLeftX != 0)
                {
                    var leftNeighbours =
                        getLeftNeighbours(aHeightmapPair.SubmapInfo, heightmaps);
                    var ourDownMargin = aHeightmapPair.GetLeftMargin();
                    foreach (var neighbourPair in leftNeighbours)
                    {
                        var marginAfterLod = ourDownMargin.SetLod(neighbourPair.SubmapInfo.LodFactor);
                        neighbourPair.SetRightMargin(neighbourPair.GetRightMargin().UpdateWherePossible(marginAfterLod));
                        if (aHeightmapPair.SubmapInfo.LodFactor >= neighbourPair.SubmapInfo.LodFactor) // we have bigger lod
                        {
                            // do nthin
                        }
                        else // neighbour has bigger lod
                        {
                            var unLoddedMargin = marginAfterLod.SetLod(aHeightmapPair.SubmapInfo.LodFactor);
                            aHeightmapPair.SetLeftMargin(unLoddedMargin); // we have to make our margin with less resolution
                        }
                    }
                }

                if (aHeightmapPair.SubmapInfo.DownLeftY!= 0)
                {
                    var downNeighbours =
                        getBottomNeighbours(aHeightmapPair.SubmapInfo, heightmaps);
                    var ourDownMargin = aHeightmapPair.GetDownMargin();
                    foreach (var neighbourPair in downNeighbours)
                    {
                        var marginAfterLod = ourDownMargin.SetLod(neighbourPair.SubmapInfo.LodFactor);
                        neighbourPair.SetTopMargin(neighbourPair.GetTopMargin().UpdateWherePossible(marginAfterLod));
                        if (aHeightmapPair.SubmapInfo.LodFactor >= neighbourPair.SubmapInfo.LodFactor) // we have bigger lod
                        {
                            // do nthin
                        }
                        else // neighbour has bigger lod
                        {
                            var unLoddedMargin = marginAfterLod.SetLod(aHeightmapPair.SubmapInfo.LodFactor); //todo lodding destroys correction in touch points with arleady seamed margins
                            aHeightmapPair.SetBottomMargin(unLoddedMargin); // we have to make our margin with less resolution
                        }
                    }
                }
            }
        }

        private void SetSubmapApexPoint(List<SubmapInfoAndHeightmapArray> heightmaps )
        {
            foreach (var aHeightmapPair in heightmaps)
            {
                if (aHeightmapPair.SubmapInfo.DownLeftX != 0)
                {
                    ApexPointData downLeftSubmapApexPoint = getApexPointAt(aHeightmapPair.SubmapInfo.DownLeftPoint, heightmaps);
                    downLeftSubmapApexPoint.IntegrateApexPoint();
                }

                if (aHeightmapPair.SubmapInfo.DownLeftY != 0)
                {
                    var downLeftSubmapApexPoint = getApexPointAt(aHeightmapPair.SubmapInfo.DownRightPoint, heightmaps);
                    downLeftSubmapApexPoint.IntegrateApexPoint();
                }
            }
        }

        private IEnumerable<SubmapInfoAndHeightmapArray> getBottomNeighbours(SubmapInfo info, List<SubmapInfoAndHeightmapArray> heightmaps)
        {
            var hh = heightmaps[0];
            var mh = MathHelp.SegmentsHaveCommonElement(hh.SubmapInfo.TopLeftPoint.X, hh.SubmapInfo.TopRightPoint.X,
                info.DownLeftPoint.X, info.DownRightPoint.X);
            return (from heightmap in heightmaps 
                    where heightmap.SubmapInfo.DownLeftY + heightmap.SubmapInfo.Height == info.DownLeftY &&
                        MathHelp.SegmentsHaveCommonElement( heightmap.SubmapInfo.TopLeftPoint.X, heightmap.SubmapInfo.TopRightPoint.X, info.DownLeftPoint.X, info.DownRightPoint.X)
                    select heightmap).ToList();
        }

        private IEnumerable<SubmapInfoAndHeightmapArray> getLeftNeighbours(SubmapInfo info, List<SubmapInfoAndHeightmapArray> heightmaps)
        {
            return (from heightmap in heightmaps 
                    where heightmap.SubmapInfo.DownLeftX + heightmap.SubmapInfo.Width == info.DownLeftX &&
                            MathHelp.SegmentsHaveCommonElement(heightmap.SubmapInfo.DownRightPoint.Y, heightmap.SubmapInfo.TopRightPoint.Y, info.DownLeftPoint.Y, info.TopLeftPoint.Y)  
                    select heightmap).ToList();
        }

        private ApexPointData getApexPointAt(Point2D apexPoint, List<SubmapInfoAndHeightmapArray> heightmaps)
        {
            var downLeftSubmap = from heightmap in heightmaps
                                where heightmap.SubmapInfo.DownRightPoint.X == apexPoint.X
                                      && (heightmap.SubmapInfo.DownRightPoint.Y) < apexPoint.Y &&
                                      (heightmap.SubmapInfo.TopRightPoint.Y) >= apexPoint.Y
                                select heightmap;
            var topLeftSubmap = from heightmap in heightmaps
                                where heightmap.SubmapInfo.DownRightPoint.X == apexPoint.X
                                      && (heightmap.SubmapInfo.DownRightPoint.Y) <= apexPoint.Y &&
                                      (heightmap.SubmapInfo.TopRightPoint.Y) > apexPoint.Y
                                select heightmap;

            var downRightSubmap = from heightmap in heightmaps
                                 where heightmap.SubmapInfo.DownLeftPoint.X == apexPoint.X
                                       && (heightmap.SubmapInfo.DownLeftPoint.Y) < apexPoint.Y &&
                                       (heightmap.SubmapInfo.TopLeftPoint.Y) >= apexPoint.Y
                                 select heightmap;
            var topRightSubmap = from heightmap in heightmaps
                                 where heightmap.SubmapInfo.DownLeftPoint.X == apexPoint.X
                                      && (heightmap.SubmapInfo.DownLeftPoint.Y) <= apexPoint.Y &&
                                      (heightmap.SubmapInfo.TopLeftPoint.Y) > apexPoint.Y
                                select heightmap;
            return new ApexPointData(apexPoint, downLeftSubmap.FirstOrDefault(), downRightSubmap.FirstOrDefault(),
                topLeftSubmap.FirstOrDefault(), topRightSubmap.FirstOrDefault());
        }

    }


    internal class SubmapInfoAndHeightmapArray
    {
        public SubmapInfo SubmapInfo;
        public HeightmapArray HeightmapArray;

        public SubmapInfoAndHeightmapArray(SubmapInfo submapInfo, HeightmapArray heightmapArray)
        {
            SubmapInfo = submapInfo;
            HeightmapArray = heightmapArray;
        }

        public HeightmapMarginWithInfo GetDownMargin()
        {
            return new HeightmapMarginWithInfo(HeightmapArray.GetDownMargin(), new MarginPosition(SubmapInfo.DownLeftPoint, SubmapInfo.DownRightPoint), SubmapInfo.LodFactor );
        }

        public HeightmapMarginWithInfo GetTopMargin()
        {
            return new HeightmapMarginWithInfo(HeightmapArray.GetTopMargin(), new MarginPosition(SubmapInfo.TopLeftPoint, SubmapInfo.TopRightPoint ), SubmapInfo.LodFactor);
        }

        public HeightmapMarginWithInfo GetLeftMargin()
        {
            return new HeightmapMarginWithInfo(HeightmapArray.GetLeftMargin(), new MarginPosition(SubmapInfo.DownLeftPoint, SubmapInfo.TopLeftPoint ), SubmapInfo.LodFactor);
        }

        public HeightmapMarginWithInfo GetRightMargin()
        {
            return new HeightmapMarginWithInfo(HeightmapArray.GetRightMargin(), new MarginPosition(SubmapInfo.DownRightPoint, SubmapInfo.TopRightPoint ), SubmapInfo.LodFactor);
        }

        public void SetRightMargin(HeightmapMarginWithInfo margin)
        {
            Preconditions.Assert(margin.Position.IsVertical, string.Format("Right margin {0} cant be set as is not vertical",margin ));
            HeightmapArray.SetRightMargin(margin.HeightmapMargin);
        }

        public void SetLeftMargin(HeightmapMarginWithInfo margin)
        {
            Preconditions.Assert(margin.Position.IsVertical, string.Format("Left margin {0} cant be set as is not vertical",margin ));
            HeightmapArray.SetLeftMargin(margin.HeightmapMargin);
        }

        public void SetTopMargin(HeightmapMarginWithInfo margin)
        {
            Preconditions.Assert(margin.Position.IsHorizontal, string.Format("Top margin {0} cant be set as is not horizontal",margin ));
            HeightmapArray.SetTopMargin(margin.HeightmapMargin);
        }

        public void SetBottomMargin(HeightmapMarginWithInfo margin)
        {
            Preconditions.Assert(margin.Position.IsHorizontal, string.Format("Bottom margin {0} cant be set as is not horizontal",margin ));
            HeightmapArray.SetBottomMargin(margin.HeightmapMargin);
        }

        public float GetApexHeight(Point2D apexPoint)
        {
            if (Equals(apexPoint, SubmapInfo.DownLeftPoint))
            {
                return HeightmapArray.GetHeight(0, 0);
            } 
            else if (Equals(apexPoint, SubmapInfo.DownRightPoint))
            {
                return HeightmapArray.GetHeight( HeightmapArray.WorkingWidth,0);
            } 
            else if (Equals(apexPoint, SubmapInfo.TopLeftPoint))
            {
                return HeightmapArray.GetHeight(0, HeightmapArray.WorkingHeight);
            }
            else if (Equals(apexPoint, SubmapInfo.TopRightPoint))
            {
                return HeightmapArray.GetHeight(HeightmapArray.WorkingWidth, HeightmapArray.WorkingHeight);
            }
            else
            {
                Preconditions.Fail(string.Format("Point {0} is not apex point", apexPoint));
                return -22; //not used
            }
        }

        public void SetApexHeight(Point2D apexPoint, float value, int lod)
        {
            Preconditions.Assert(lod >= SubmapInfo.LodFactor, "Cant set apex height. Lod factor is too small");
            var pixelSize = (int)Math.Pow(2, lod - SubmapInfo.LodFactor);
            if (Equals(apexPoint, SubmapInfo.DownLeftPoint))
            {
                HeightmapArray.SetDownLeftApexMarginHeight( value, pixelSize); 
            }
            else if (Equals(apexPoint, SubmapInfo.DownRightPoint))
            {
                HeightmapArray.SetDownRightApexMarginHeight(value, pixelSize);
            }
            else if (Equals(apexPoint, SubmapInfo.TopLeftPoint))
            {
                HeightmapArray.SetTopLeftApexMarginHeight( value, pixelSize);
            }
            else if (Equals(apexPoint, SubmapInfo.TopRightPoint))
            {
                HeightmapArray.SetTopRightApexMarginHeight( value, pixelSize);
            }
            else
            {
                Preconditions.Fail(string.Format("Point {0} is not apex point", apexPoint));
            }
        }

        public float GetHeight(Point2D apexPoint)
        {
            Preconditions.Assert(SubmapInfo.IsPointPartOfSubmap(apexPoint), String.Format("Point {0} is not part of submap ", apexPoint));
            int globalOffsetX = apexPoint.X - SubmapInfo.DownLeftX;
            int globalOffsetY = apexPoint.Y - SubmapInfo.DownLeftY;
            int offsetX = (int)(HeightmapArray.WorkingWidth * Mathf.InverseLerp(0, SubmapInfo.Width, globalOffsetX));
            int offsetY = (int)(HeightmapArray.WorkingHeight * Mathf.InverseLerp(0, SubmapInfo.Height, globalOffsetY));
            return HeightmapArray.GetHeight(offsetX, offsetY);
        }
    }


    internal class ApexPointData
    {
        private readonly Point2D _apexPoint;
        private readonly SubmapInfoAndHeightmapArray _downLeft;
        private readonly SubmapInfoAndHeightmapArray _downRight;
        private readonly SubmapInfoAndHeightmapArray _topLeft;
        private readonly SubmapInfoAndHeightmapArray _topRight;

        public ApexPointData(Point2D apexPoint, SubmapInfoAndHeightmapArray downLeft, SubmapInfoAndHeightmapArray downRight, 
            SubmapInfoAndHeightmapArray topLeft, SubmapInfoAndHeightmapArray topRight)
        {
            _apexPoint = apexPoint;
            _downLeft = downLeft;
            _downRight = downRight;
            _topLeft = topLeft;
            _topRight = topRight;
        }

        public void IntegrateApexPoint()
        {
            var allSubmaps = (new List<SubmapInfoAndHeightmapArray> {_downLeft, _downRight, _topLeft, _topRight}).Where(x => x != null).ToList();
            if (allSubmaps.Count != 0)
            {
                var highestPointValue =
                    allSubmaps.Where(x => x.SubmapInfo.IsApexPoint(_apexPoint))
                        .Select(x => x.GetApexHeight(_apexPoint))
                        .Union(
                            allSubmaps.Where(x => !x.SubmapInfo.IsApexPoint(_apexPoint))
                                .Select(x => x.GetHeight(_apexPoint)))
                        .OrderByDescending(x => x).First();

                var biggestLod = allSubmaps.Select(x => x.SubmapInfo.LodFactor).OrderByDescending(x => x).First();
                foreach (var subMap in allSubmaps)
                {
                    if (subMap.SubmapInfo.IsApexPoint(_apexPoint))
                    {
                        subMap.SetApexHeight(_apexPoint, highestPointValue, biggestLod);
                    }
                }
            }
        }
    }
}
