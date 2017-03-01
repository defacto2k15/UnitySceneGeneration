using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    class HeightmapMarginWithPosition
    {
        private readonly HeightmapMargin _heightmapMargin;
        private readonly MarginPosition _position;

        public HeightmapMarginWithPosition(HeightmapMargin heightmapMargin, MarginPosition position)
        {
            _heightmapMargin = heightmapMargin;
            _position = position;
        }

        public HeightmapMargin HeightmapMargin
        {
            get { return _heightmapMargin; }
        }

        public MarginPosition Position
        {
            get { return _position; }
        }

        public HeightmapMarginWithPosition SetLength(int length)
        {
            return new HeightmapMarginWithPosition(_heightmapMargin.SetLength(length), Position);
        }

        public HeightmapMarginWithPosition UpdateWherePossible(HeightmapMarginWithPosition newMargin)
        {
            Preconditions.Assert( _heightmapMargin.Length == newMargin.HeightmapLength, 
                string.Format("Current margin length is {0} != new margin length == {1} ", _heightmapMargin.Length, newMargin.HeightmapLength));
            Preconditions.Assert( (newMargin.Position.IsHorizontal && Position.IsHorizontal) || (newMargin.Position.IsVertical && Position.IsVertical), 
                string.Format("Current and new margins are one vertical one horizontal: Old {0} new {1}",_heightmapMargin, newMargin ));

            bool haveCommonElements = Position.HaveCommonElementWith(newMargin.Position);
            Preconditions.Assert(haveCommonElements, string.Format("Current {0} and new {1} margin dont have common elements", HeightmapMargin, newMargin));

            MarginPosition commonSegment = Position.GetCommonSegment(newMargin.Position);
            var startPercentage = Position.InvLerp(commonSegment.StartPoint);
            var endPercentage = Position.InvLerp(commonSegment.EndPoint);

            var ourStartOffset = (int)Math.Round((double) HeightmapLength*startPercentage);
            var ourEndOffset = (int)Math.Round((double) HeightmapLength*endPercentage);
            var theirStartOffset = (int)Math.Round((double) newMargin.HeightmapLength*startPercentage);
            var theirEndOffset = (int)Math.Round((double) newMargin.HeightmapLength*endPercentage);
            
            return new HeightmapMarginWithPosition(SetMarginSubElement(ourStartOffset, ourEndOffset, theirStartOffset, theirEndOffset, newMargin),Position);
        }

        private HeightmapMargin SetMarginSubElement(int ourStartOffset, int ourEndOffset, int theirStartOffset, int theirEndOffset, HeightmapMarginWithPosition newMargin)
        {
            Preconditions.Assert(
                (ourEndOffset - ourStartOffset) == (theirEndOffset-theirStartOffset),
                String.Format("Cant set subelement. Offset lengths are not equal. Our start {0} end {1} their start {2} end {3} ", 
                ourStartOffset, ourEndOffset, theirStartOffset, theirEndOffset));
            var length = ourEndOffset - ourStartOffset;
            var newValues = new float[_heightmapMargin.Length];
            Array.Copy(_heightmapMargin.MarginValues, newValues, newValues.Length);
            for (int i = 0; i < length; i++)
            {
                newValues[ourStartOffset + i] = newMargin.HeightmapMargin.MarginValues[theirStartOffset + i];
            }
            return new HeightmapMargin(newValues);
        }


        public int HeightmapLength
        {
            get { return _heightmapMargin.Length; }
        }
    }
}
