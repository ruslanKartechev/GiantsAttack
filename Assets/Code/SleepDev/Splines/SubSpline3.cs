using UnityEngine;

namespace SleepDev.Splines
{
    [System.Serializable]
    public class SubSpline3
    {
        public const float DefaultTangentAsLengthFraction = .25f;
        
        public SplineNode p0;
        public SplineNode p1;
        public SplineNode p2;
        public SplineNode p3;
        [Space(10)]
        public SplineNode controlNode;
        public float tangentLength = 1;
     
        public SubSpline3(SplineNode p0, SplineNode p1, SplineNode p2, SplineNode p3)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }

        /// <summary>
        /// Initialize using Start and END points
        /// </summary>
        /// <param name="p0">start point</param>
        /// <param name="p3">end point</param>
        public SubSpline3(SplineNode p0, SplineNode p3)
        {
            this.p0 = p0;
            this.p3 = p3;
            var midPoint = Vector3.Lerp(p0.worldPosition, p3.worldPosition, .5f);
            
            var midNode = new SplineNode(midPoint, Quaternion.Lerp(p0.worldRotation, p3.worldRotation, .5f));
            SetupMidPoints(midNode);
        }

        public void SetupMidPoints(SplineNode controlNode)
        {
            var distance = (p3.worldPosition - p0.worldPosition).magnitude;
            var tangentLength = DefaultTangentAsLengthFraction * distance;
            SetupMidPoints(controlNode, tangentLength);
        }
        
        public void SetupMidPoints(SplineNode controlNode, float tangentLength)
        {
            this.controlNode = controlNode;
            this.tangentLength = tangentLength;
            var midPoint = controlNode.worldPosition;
            var leftDir = (p0.worldPosition - midPoint).normalized;
            var leftPos = midPoint + leftDir * tangentLength;

            var rightDir = (p3.worldPosition - midPoint).normalized;
            var rightPos = midPoint + rightDir * tangentLength;

            this.p1 = new SplineNode(leftPos, p0.worldRotation);
            this.p2 = new SplineNode(rightPos, p0.worldRotation);
        }
    }
}