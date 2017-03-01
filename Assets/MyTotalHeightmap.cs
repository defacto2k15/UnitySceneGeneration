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
            var wholeTerrainWidth = 100000;

            var submapAndHeightmapArrayList
                = (from info in submapInfos
                    let submap = 
                        heightmapFile.getHeightSubmap(info.DownLeftX, info.DownLeftY, info.Width, info.Height)
                            .simplyfy(info.LodFactor)
                    select new SubmapInfoAndHeightmapArray(info, submap)).ToList();

            SetSubmapMargins(submapAndHeightmapArrayList);

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
                        var marginAfterLod = ourDownMargin.SetLength(neighbourPair.HeightmapArray.WorkingHeight);
                        neighbourPair.SetRightMargin(neighbourPair.GetRightMargin().UpdateWherePossible(marginAfterLod));
                        if (aHeightmapPair.SubmapInfo.LodFactor >= neighbourPair.SubmapInfo.LodFactor) // we have bigger lod
                        {
                            // do nthin
                        }
                        else // neighbour has bigger lod
                        {
                            var unLoddedMargin = marginAfterLod.SetLength(aHeightmapPair.HeightmapArray.WorkingHeight);
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
                        var marginAfterLod = ourDownMargin.SetLength(neighbourPair.HeightmapArray.WorkingWidth);
                        neighbourPair.SetTopMargin(neighbourPair.GetTopMargin().UpdateWherePossible(marginAfterLod));
                        if (aHeightmapPair.SubmapInfo.LodFactor >= neighbourPair.SubmapInfo.LodFactor) // we have bigger lod
                        {
                            // do nthin
                        }
                        else // neighbour has bigger lod
                        {
                            var unLoddedMargin = marginAfterLod.SetLength(aHeightmapPair.HeightmapArray.WorkingWidth); //todo lodding destroys correction in touch points with arleady seamed margins
                            aHeightmapPair.SetBottomMargin(unLoddedMargin); // we have to make our margin with less resolution
                        }
                    }
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


        //public float GetHeight(int terrainXPos, int terrainYPos, HeightmapPosition position) //todo
        //{
        //    return heightArrays[terrainXPos, terrainYPos].GetHeight(position);
        //}
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

        public HeightmapMarginWithPosition GetDownMargin()
        {
            return new HeightmapMarginWithPosition(HeightmapArray.GetDownMargin(), new MarginPosition(SubmapInfo.DownLeftPoint, SubmapInfo.DownRightPoint) );
        }

        public HeightmapMarginWithPosition GetTopMargin()
        {
            return new HeightmapMarginWithPosition(HeightmapArray.GetTopMargin(), new MarginPosition(SubmapInfo.TopLeftPoint, SubmapInfo.TopRightPoint ));
        }

        public HeightmapMarginWithPosition GetLeftMargin()
        {
            return new HeightmapMarginWithPosition(HeightmapArray.GetLeftMargin(), new MarginPosition(SubmapInfo.DownLeftPoint, SubmapInfo.TopLeftPoint ));
        }

        public HeightmapMarginWithPosition GetRightMargin()
        {
            return new HeightmapMarginWithPosition(HeightmapArray.GetRightMargin(), new MarginPosition(SubmapInfo.DownRightPoint, SubmapInfo.TopRightPoint ));
        }

        public void SetRightMargin(HeightmapMarginWithPosition margin)
        {
            Preconditions.Assert(margin.Position.IsVertical, string.Format("Right margin {0} cant be set as is not vertical",margin ));
            HeightmapArray.SetRightMargin(margin.HeightmapMargin);
        }

        public void SetLeftMargin(HeightmapMarginWithPosition margin)
        {
            Preconditions.Assert(margin.Position.IsVertical, string.Format("Left margin {0} cant be set as is not vertical",margin ));
            HeightmapArray.SetLeftMargin(margin.HeightmapMargin);
        }

        public void SetTopMargin(HeightmapMarginWithPosition margin)
        {
            Preconditions.Assert(margin.Position.IsHorizontal, string.Format("Top margin {0} cant be set as is not horizontal",margin ));
            HeightmapArray.SetTopMargin(margin.HeightmapMargin);
        }

        public void SetBottomMargin(HeightmapMarginWithPosition margin)
        {
            Preconditions.Assert(margin.Position.IsHorizontal, string.Format("Bottom margin {0} cant be set as is not horizontal",margin ));
            HeightmapArray.SetBottomMargin(margin.HeightmapMargin);
        }
    }
}
