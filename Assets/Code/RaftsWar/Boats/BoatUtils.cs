using System;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using SleepDev;
using UnityEngine;
using UnityEngine.Assertions;
using RaftsWar.Cam;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace RaftsWar.Boats
{
    public static class BoatUtils
    {
        public static readonly List<Vector3> Directions = new()
        {
            new Vector3(0,0,1),
            new Vector3(0,0,-1),
            new Vector3(1,0,0),
            new Vector3(-1,0,0),
            new Vector3(.71f,0, .71f),
            new Vector3(-.71f,0, .71f),
            new Vector3(.71f,0, -.71f),
            new Vector3(-.71f,0, -.71f),
        };

        public static List<float> EnemyMoveAroundDistances = new(){ 2f, 4f, 6f, 8f,10f };

        public static bool ConnectToBoat(Boat boat, BoatPart newPart)
        {
            // sort boat parts by distance
            var closestPair = boat.PointsKeeper.RootPair;
            var newPartPos = newPart.Point.position;
            var minD2 = DistanceSquare(boat.RootPart, newPartPos);
            var sortedParts = new List<KeyValuePair<Transform, BoatPart>>(4);
            sortedParts.Add(closestPair);
            foreach (var pair in boat.PointsKeeper.Map)
            {
                var mpart = pair.Value;
                var d2 = DistanceSquare(mpart, newPartPos);
                if (d2 <= minD2)
                {
                    minD2 = d2;
                    closestPair = pair;
                    sortedParts.Insert(0, pair);
                }
            }
            foreach (var (trackedPoint, mp) in sortedParts)
            {
                var vec = newPartPos - trackedPoint.position;
                if (Mathf.Abs(vec.z) > Mathf.Abs(vec.x)) // first check Z axis
                {
                    if(TryConnectOnZ(mp, newPart, trackedPoint, vec))
                        return true;
                    if(TryConnectOnX(mp, newPart, trackedPoint, vec))
                        return true;
                }
                else // first check  axis
                {
                    if(TryConnectOnX(mp, newPart, trackedPoint, vec))
                        return true;
                    if(TryConnectOnZ(mp, newPart, trackedPoint, vec))
                        return true;
                }
            }
            Debug.Log($"[Utils] Error, Cannot connect part after checking all sorted parts");
            return false;

#region Nested methods
            bool TryConnectOnZ(BoatPart mp, BoatPart np, 
                Transform mTrackPoint, Vector3 vec)
            {
                if (vec.z > 0) // new part above
                {
                    var newTrackPoint = mTrackPoint.localPosition + Vector3.forward * (mp.Radius * 2f);
                    return TryConnectSides(mp, 0, np, 2, newTrackPoint);
                }
                else // new part below
                {
                    var newTrackPoint = mTrackPoint.localPosition - Vector3.forward * (mp.Radius * 2f);
                    return TryConnectSides(mp, 2, np, 0, newTrackPoint);
                }
            }
            
            bool TryConnectOnX(BoatPart mp, BoatPart np, 
                Transform mTrackPoint, Vector3 vec)
            {
                if (vec.x > 0) // new part right
                {
                    var newTrackPoint = mTrackPoint.localPosition + Vector3.right * (mp.Radius * 2f);
                    return TryConnectSides(mp, 1, np, 3, newTrackPoint);
                }
                else // new part left
                {
                    var newTrackPoint = mTrackPoint.localPosition - Vector3.right * (mp.Radius * 2f);
                    return TryConnectSides(mp, 3, np, 1, newTrackPoint);
                }
            }

            bool TryConnectSides(BoatPart mp, int mPartInd, BoatPart np, int npPartInd, Vector3 newTrackPointPos)
            {
                if (mp.Sides[mPartInd].IsOccupied) // top side on me
                {
                    CLog.LogRed($"[Utils] Closest m_side ({npPartInd}) is occupied already");
                    return false;
                }
                if (np.Sides[npPartInd].IsOccupied) // bot side on another
                {
                    CLog.LogRed($"[Utils] Closest np_side ({npPartInd}) is occupied already");
                    return false;
                }
                // CLog.LogYellow($"[Utils] Connect me:{mp.gameObject.name}, " +
                               // $"m_ind_{mPartInd}, other: {np.gameObject.name} n_ind_{npPartInd}");
                var trackedPoint = boat.PointsKeeper.AddPointAt(newPart, newTrackPointPos);
                newPart.transform.parent = boat.PartsParent;
                // SetConnectingSides(boat, newPart);
                AssignPartToBoat(boat, newPart);
                SetLocalCoordToBoatPart(newPart, trackedPoint);
                newPart.MoveToConnectToBoat(trackedPoint, ()=>{});
                CheckPartsSidesWithNewPart(boat, newPart);
                return true;
            }
#endregion
        }
        
        public static void ConnectBoatPartToRoot(Boat boat, BoatPart newPart, Vector3 direction)
        {
            var trackedPointPos = boat.PointsKeeper.RootPair.Key.localPosition + direction * (newPart.Radius * 2f);
            var trackedPoint = boat.PointsKeeper.AddPointAt(newPart, trackedPointPos);
            newPart.transform.parent = boat.PartsParent;
            // SetConnectingSides(boat, newPart);
            AssignPartToBoat(boat, newPart);
            SetLocalCoordToBoatPart(newPart, trackedPoint);
            newPart.MoveToConnectToBoat(trackedPoint, ()=>{});
            CheckPartsSidesOnBoat(boat);
        }

        public static void SetLocalCoordToBoatPart(BoatPart part, Transform trackedPoint)
        {
            var localPos = trackedPoint.localPosition;
            var factor = part.Radius * 2f;
            var x = Mathf.RoundToInt(localPos.x / factor);
            var y = Mathf.RoundToInt(localPos.z / factor);
            part.X = x;
            part.Y = y;
            // CLog.LogRed($"Set {part.gameObject.name}, X {x}, Y {y}");
        }

        /// <summary>
        /// Use if newPart was not yet added to the parts list
        /// </summary>
        public static void CheckPartsSidesWithNewPart(Boat boat, BoatPart newPart)
        {
            var allParts = new List<BoatPart>(boat.Parts);
            allParts.Add(boat.RootPart);
            allParts.Add(newPart);
            CheckPartsSides(allParts);
        }

        public static void CheckPartsSidesOnBoat(Boat boat)
        {
            var allParts = new List<BoatPart>(boat.Parts);
            allParts.Add(boat.RootPart);
            CheckPartsSides(allParts);
        }

        public static void CheckPartsSides(List<BoatPart> allParts)
        {
            var set = new HashSet<Vector2Int>(allParts.Count);
            foreach (var bp in allParts)
                set.Add(new Vector2Int(bp.X, bp.Y));
            foreach (var part in allParts)
                CheckPartSides(part, set);   
        }
        
        /// <summary>
        /// 0 1 2 
        /// 3 4 5 , center => part itself
        /// 6 7 8
        /// </summary>
        public static void CheckPartSides(BoatPart part, HashSet<Vector2Int> coordinatesSet)
        {
            foreach (var side in part.Sides)
            {
                side.View.HideSideAndCorners();
                side.IsOccupied = true;
            }
            var map = BuildBoatMap(part.X, part.Y);

            #region Debug log
            // CLog.LogRed($"***********************");
            // CLog.LogRed($"Checking {part.gameObject.name} X {part.X} Y {part.Y}");
            // var msg = "";
            // for (var i = 0; i < 3; i++)
            // {
            //     for (var j = 0; j < 3; j++)
            //     {
            //         msg += $"{map[j + i * 3]},  ";
            //     }
            //     msg += "\n";
            // }
            // CLog.LogRed(msg);
            #endregion

            var shownSides = 0;
            if (map[1] == true) // top free
            {
                part.Sides[0].View.ShowSide();
                part.Sides[0].IsOccupied = false;                
                shownSides++;
            }
            if (map[5] == true) // right free
            {
                part.Sides[1].View.ShowSide();
                part.Sides[1].IsOccupied = false;
                shownSides++;
            }
            if (map[7] == true) // bottom free
            {
                part.Sides[2].View.ShowSide();
                part.Sides[2].IsOccupied = false;
                shownSides++;
            }
            if (map[3] == true) // left free
            {
                part.Sides[3].View.ShowSide();
                part.Sides[3].IsOccupied = false;
                shownSides++;
            }
            // no sides all enabled, skip corners check
            if (shownSides == 0)
                return;
            // top left corner check
            if ((map[0] && map[1] && map[3]) || (map[0] && map[3]))
            {
                part.Sides[0].View.ShowCorner1();
            }
            // top right corner check
            if ((map[1] && map[2] && map[5]) || (map[2] && map[5]))
            {
                part.Sides[0].View.ShowCorner2();
            }
            // lower right corner check
            if ((map[5] && map[7] && map[8]) || (map[5] && map[8]))
            {
                part.Sides[2].View.ShowCorner1();
            }
            // lower left corner check
            if ((map[3] && map[6] && map[7]) || (map[3] && map[6]))
            {
                part.Sides[2].View.ShowCorner2();
            }

            // True == free, false == busy
            bool[] BuildBoatMap(int x, int y)
            {
                var map = new bool[9];

                map[0] = !coordinatesSet.Contains(new Vector2Int(x-1, y + 1));
                map[1] = !coordinatesSet.Contains(new Vector2Int(x, y + 1));
                map[2] = !coordinatesSet.Contains(new Vector2Int(x+1, y + 1));

                map[3] = !coordinatesSet.Contains(new Vector2Int(x-1, y));
                map[4] = true; // the part itself
                map[5] = !coordinatesSet.Contains(new Vector2Int(x+1, y));
                
                map[6] = !coordinatesSet.Contains(new Vector2Int(x-1, y-1));
                map[7] = !coordinatesSet.Contains(new Vector2Int(x, y-1));
                map[8] = !coordinatesSet.Contains(new Vector2Int(x+1, y-1));
                return map;
            }
        }

        /// <summary>
        /// Also adds the part to the parts list on the boat
        /// </summary>
        /// <param name="boat"></param>
        /// <param name="part"></param>
        public static void AssignPartToBoat(Boat boat, BoatPart part)
        {
            // CLog.Log($"[Utils] Assigned {part.gameObject}");
            part.TriggerHandler = boat.PartTriggerHandler;
            part.HostBoat = boat;
            part.SetView(boat.ViewSettings);
            part.Damageable = boat.DamageTarget;
            part.Team = boat.Team;
            part.FloatingAnimator.enabled = false;
            part.Collider.gameObject.layer = GlobalConfig.BoatLayer;
            part.ColliderOn();
            boat.ConnectedColliders.Add(part.Collider);
            boat.ConnectedColliders.Add(part.Trigger);
            boat.Parts.Add(part);
        }

        public static void UnlinkPartFromBoat(BoatPart part, Boat boat)
        {
            // CLog.Log($"[Utils] Unlinked {part.gameObject}");
            part.transform.parent = boat.transform.parent;
            // part.ColliderOff();
            part.StopFollowing();
            boat.Parts.Remove(part);
            boat.ConnectedColliders.Remove(part.Collider);
            boat.ConnectedColliders.Remove(part.Trigger);
            boat.PointsKeeper.RemovePointFor(part);
            part.TriggerHandler = null;
            part.OtherSides.Clear();
            part.MyUsedSides.Clear();
            part.HostBoat = null;
            part.Damageable = null;
            foreach (var side in part.Sides)
            {
                side.IsOccupied = false;
                side.View.ShowSideAndCorners();
            }

            if (part.HasUnit)
            {
                boat.RemoveUnit(part.Unit);
                part.Unit.Stop();
            }
            CheckPartsSidesOnBoat(boat);
        }

        public static void SetBoatPartDefaultVisuals(BoatPart part)
        {
            part.transform.parent = RaftsDataContainer.BoatPartsPlane;
            part.SetView(RaftsDataContainer.DefaultBoatsView);
            if (part.HasUnit)
            {
                part.Unit.Unit.Team = null;
                part.Unit.Unit.SetView(RaftsDataContainer.DefaultUnitsView);
            }
        }

        public static void SetBoatPartFree(BoatPart part)
        {
            // CLog.Log($"[Utils] Refresh {part.gameObject}");
            part.HostBoat = null;
            part.TrackPoint = null;
            part.TriggerHandler = null;
            part.Damageable = null;
            part.Team = null;
            part.OtherSides.Clear();
            part.enabled = true;
            foreach (var side in part.Sides)
            {
                side.View.Refresh();
                side.IsOccupied = false;
            }
            part.Collider.gameObject.layer = GlobalConfig.DefaultLayer;
            part.TriggerOnly();
        }


        public static void PunchScaleBP(Transform scalable)
        {
            scalable.DOKill();
            scalable.localScale = Vector3.one;
            scalable.DOPunchScale(Vector3.one * .2f, .2f);
        }

        public static bool CheckBPConnectionToRoot(BoatPart otherPart, BoatPart root)
        {
            // CLog.Log("[IsConnectedToRoot]");
            foreach (var side in otherPart.OtherSides)
            {
                var p = side.BoatPart;
                if (p == root)
                    return true;
                if (CheckBPConnectionRecursive(p, otherPart, root))
                    return true;
            }
            return false;
        }

        public static bool CheckBPConnectionRecursive(BoatPart myPart, BoatPart callFrom, BoatPart root)
        {
            foreach (var side in myPart.OtherSides)
            {
                var p = side.BoatPart;
                if (p == callFrom)
                    continue;
                if (p == root)
                    return true;
                if(CheckBPConnectionRecursive(p, myPart, root))
                    return true;
            }
            return false;
        }

        public static void HideRampSidesView(BoatPart raft, ESquareSide side)
        {
            switch (side)
            {
                case ESquareSide.All:
                    foreach (var s in raft.Sides)
                        s.View.HideSideAndCorners();   
                    break;
                case ESquareSide.None:
                    break;
                case ESquareSide.TopLeft:
                    break;
                case ESquareSide.Top:
                    raft.Sides[0].View.HideSideAndCorners();
                    break;
                case ESquareSide.TopRight:
                    break;
                case ESquareSide.Right:
                    raft.Sides[1].View.HideSideAndCorners();
                    break;
                case ESquareSide.BotRight:
                    break;
                case ESquareSide.Bot:
                    raft.Sides[2].View.HideSideAndCorners();
                    break;
                case ESquareSide.BotLeft:
                    break;
                case ESquareSide.Left:
                    raft.Sides[3].View.HideSideAndCorners();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
        }
        
        public static void SetSidesView(BoatPart raft, ESquareSide side)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                foreach (var s in raft.Sides)
                    s.Awake();
            }
#endif
            foreach (var s in raft.Sides)
                s.View.HideSideAndCorners();   
            switch (side)
            {
                case ESquareSide.All:
                    foreach (var s in raft.Sides)
                        s.View.ShowSideAndCorners();   
                    break;
                case ESquareSide.None:
                    break;
                case ESquareSide.TopLeft:
                    raft.Sides[0].View.ShowCorner1();
                    raft.Sides[0].View.ShowSide();
                    raft.Sides[3].View.ShowSide();
                    break;
                case ESquareSide.Top:
                    raft.Sides[0].View.ShowSide();
                    break;
                case ESquareSide.TopRight:
                    raft.Sides[0].View.ShowCorner2();
                    raft.Sides[0].View.ShowSide();
                    raft.Sides[1].View.ShowSide();
                    break;
                case ESquareSide.Right:
                    raft.Sides[1].View.ShowSide();
                    break;
                case ESquareSide.BotRight:
                    raft.Sides[2].View.ShowCorner1();
                    raft.Sides[2].View.ShowSide();
                    raft.Sides[1].View.ShowSide();
                    break;
                case ESquareSide.Bot:
                    raft.Sides[2].View.ShowSide();
                    break;
                case ESquareSide.BotLeft:
                    raft.Sides[2].View.ShowCorner2();
                    raft.Sides[2].View.ShowSide();
                    raft.Sides[3].View.ShowSide();
                    break;
                case ESquareSide.Left:
                    raft.Sides[3].View.ShowSide();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
        }

        public static IPlayerCamera GetCamera()
        {
            var camera = Camera.main.transform.parent.GetComponent<IPlayerCamera>();
#if UNITY_EDITOR
            UnityEngine.Assertions.Assert.IsNotNull(camera);
#endif
            return camera;
        }

        public static void AnimateBoatPartPop(BoatPart part, float time)
        {
            part.gameObject.SetActive(true);
            part.StopTweens();
            part.Scalable.localScale = Vector3.one * .5f;
            part.Scalable.DOScale(Vector3.one, time);
            var pos1 = part.transform.position;
            var pos2 = pos1;
            pos2.y += 2f;
            part.transform.position = pos2;
            part.transform.DOMove(pos1, time);
        }

        public static Vector3 GetUnloadPosition(Boat boat, IUnloadPointsManager unloadPointsManager, float radius)
        {
            var boatCenter = boat.RootPart.Point.position;
            var closestUnloadPoint = unloadPointsManager.GetClosestPoint(boatCenter);
            var dirVec = closestUnloadPoint - boatCenter;
            var closestBoatPart = GetClosestBoatPart(boat, closestUnloadPoint, dirVec);
            var endPoint = closestUnloadPoint;
            if (closestBoatPart != boat.RootPart)
            {
                var localOffset = closestBoatPart.Point.position - boatCenter;
                endPoint -= localOffset;
            }
            dirVec = endPoint - closestBoatPart.Point.position;
            endPoint -= (dirVec.normalized * (radius));
            
#if UNITY_EDITOR
            Debug.DrawLine(boatCenter, endPoint, Color.yellow, 10);
            Debug.DrawLine(endPoint, endPoint + Vector3.up * 5f, Color.yellow, 10);
#endif
            return endPoint;
        }

        
                
        // Assumes clockwise sides orientation on all boat parts
        private static void SetConnectingSides(Boat boat, BoatPart targetPart)
        {
            var boatNearParts = GetNearParts(boat, targetPart);
            // CLog.LogRed($"*** Near parts: {boatNearParts.Count}");
            var myPos = targetPart.TrackPoint.position;
            var otherSides = new List<BoatSide>(2); // sides on OTHER rafts
            var mySides = new List<BoatSide>(2); // sides on MY raft
            foreach (var bp in boatNearParts)
            {
                var vec = (bp.TrackPoint.position - myPos);
                BoatSide mySide = null;
                if (Mathf.Abs(vec.z) >= Mathf.Abs(vec.x))
                {
                    if (vec.z > 0)
                    {
                        mySide = targetPart.Sides[0];
                        mySides.Add(mySide);
                        otherSides.Add(bp.Sides[2]);
                    }
                    else
                    {
                        mySide = targetPart.Sides[2];
                        mySides.Add(mySide);
                        otherSides.Add(bp.Sides[0]);
                    }
                }
                else
                {
                    if (vec.x > 0)
                    {
                        mySide = targetPart.Sides[1];
                        mySides.Add(mySide);
                        otherSides.Add(bp.Sides[3]);
                    }
                    else
                    {
                        mySide = targetPart.Sides[3];
                        mySides.Add(mySide);
                        otherSides.Add(bp.Sides[1]);
                    }
                }
#if UNITY_EDITOR
                Assert.IsNotNull(mySide);
#endif
                bp.OtherSides.Add(mySide);
            }
            foreach (var side in otherSides)
            {
                side.IsOccupied = true;
                side.View.HideSideAndCorners();
            }
            foreach (var side in mySides)
            {
                side.IsOccupied = true;
                side.View.HideSideAndCorners();
            }
            // CLog.LogRed($"*** Other sides: {otherSides.Count}, my sides count: {mySides.Count}");
            targetPart.OtherSides = otherSides;
            targetPart.MyUsedSides = mySides;
        }

        private static List<BoatPart> GetNearParts(Boat boat, BoatPart targetPart)
        {
            var closeParts = new List<BoatPart>();
            var rad2 = targetPart.Radius * 2.2f;
            rad2 *= rad2;
            var d2 = float.MaxValue;
            CheckPart(boat.RootPart);
            foreach (var part in boat.Parts)
                CheckPart(part);
            return closeParts;

            void CheckPart(BoatPart part)
            {
                if(part == targetPart)
                    return;
                d2 = DistanceSquare(targetPart.TrackPoint.localPosition, 
                    part.TrackPoint.localPosition);
                if(d2 < rad2)
                    closeParts.Add(part);
            }
        }
        
        private static BoatSide FindClosestSide(BoatPart part, Vector3 center)
        {
            var cd2 = float.MaxValue;
            BoatSide closestToPart = null;
            foreach (var side in part.Sides)
            {
                if (side.IsOccupied)
                    continue;
                var vec = center - side.Direction.position;
                var vec2 = new Vector2(vec.x, vec.z);
                var d2 = (vec2).sqrMagnitude;
                if (d2 <= cd2)
                {
                    cd2 = d2;
                    closestToPart = side;
                }
            }
            return closestToPart;
        }

        public static BoatPart GetClosestBoatPart(Boat boat, Vector3 toPoint)
        {
            var result = boat.RootPart;
            var vec = result.Point.position - toPoint;
            var d2 = vec.sqrMagnitude;
            var mindD2 = d2;
            foreach (var part in boat.Parts)
            {
                d2 = (part.Point.position - toPoint).sqrMagnitude;
                if (d2 < mindD2)
                {
                    mindD2 = d2;
                    result = part;
                }
            }
            return result;
        } 
        private static BoatPart GetClosestBoatPart(Boat boat, Vector3 endPoint, Vector3 dirVec)
        {
            var result = boat.RootPart;
            var minD2 = float.MaxValue;
            // right
            if (dirVec.x > 0)
            {
                var parts = new List<BoatPart>(4) { boat.RootPart };
                foreach (var addedParts in boat.Parts)
                {
                    if (addedParts.transform.localPosition.x >= 0)
                        parts.Add(addedParts);
                }
                // CLog.LogRed($"Parts on the right {parts.Count}");
                if (dirVec.y > 0) // up-right
                    result = GetClosestPartUp(parts, endPoint);
                else // down right
                    result = GetClosestPartDown(parts, endPoint);
            }
            else // left
            {
                var parts = new List<BoatPart>(4) { boat.RootPart };
                foreach (var addedParts in boat.Parts)
                {
                    if (addedParts.transform.localPosition.x <= 0)
                        parts.Add(addedParts);
                }
                // CLog.LogRed($"Parts on the LEFT {parts.Count}");
                if (dirVec.y > 0) // up-left
                    result = GetClosestPartUp(parts, endPoint);
                else // down left
                    result = GetClosestPartDown(parts, endPoint);
            }
            return result;
        }

        private static BoatPart GetClosestPartDown(IList<BoatPart> parts, Vector3 endPoint)
        {
            var minD2 = float.MaxValue;
            BoatPart result = null;
            foreach (var part in parts)
            {
                if (part.transform.localPosition.y > 0)
                    continue;
                var d2 = (part.transform.position - endPoint).XZDistance2();
                if (d2 <= minD2)
                {
                    minD2 = d2;
                    result = part;
                }
            }
#if UNITY_EDITOR
            Assert.IsNotNull(result);
#endif
            return result;
        }
        
        private static BoatPart GetClosestPartUp(IList<BoatPart> parts, Vector3 endPoint)
        {
            var minD2 = float.MaxValue;
            BoatPart result = null;
            foreach (var part in parts)
            {
                if (part.transform.localPosition.y < 0)
                    continue;
                var d2 = (part.transform.position - endPoint).XZDistance2();
                if (d2 <= minD2)
                {
                    minD2 = d2;
                    result = part;
                }
            }
            #if UNITY_EDITOR
            Assert.IsNotNull(result);
            #endif
            return result;
        }
      
        // up, down, right, left
        public static Vector3 GetPushOutPoint(Square square, Boat boat, Transform root)
        {
            const float distanceMult = 2f;
            var point = Vector3.zero;
            var vec = (boat.transform.position - root.position);
            var directionIndex = 0;
            if (Mathf.Abs(vec.z) > Mathf.Abs(vec.x))
            {
                if (vec.z < 0) // Try push down
                    directionIndex = 1;
                else // Try Push Up
                    directionIndex = 0;
            }
            else
            {
                if (vec.x > 0) // Try push right
                    directionIndex = 2;
                else // Try Push left
                    directionIndex = 3;
            }
            //ORDER OF DIRECTIONS: up down right left
            var boatLength = 4f;
            for (var i = 0; i < 4; i++)
            {
                var dir = Directions[directionIndex];
                switch (directionIndex)
                {
                    case 0:
                        boatLength = GetBoatLengthUp(boat);
                        break;
                    case 1:
                        boatLength = GetBoatLengthDown(boat);
                        break;
                    case 2: 
                        boatLength = GetBoatLengthRight(boat);
                        break;
                    case 3: 
                        boatLength = GetBoatLengthLeft(boat);
                        break;
                }
                point = square.Center + dir * (square.Width + boatLength * distanceMult);
                if(CheckIfWalkable(point, boat))
                    return point;
                    
                directionIndex++;
                if(directionIndex > 3)
                    directionIndex = 0;
            }
            // CLog.LogYellow($"[*******] Vec Corrected {vec}");
            return point;
        }

        public static float GetBoatLengthUp(Boat boat)
        {
            var length = boat.RootPart.Radius;
            var furthestPos = 0f;
            foreach (var part in boat.Parts)
            {
                if (part.transform.localPosition.y > furthestPos)
                {
                    furthestPos = part.transform.localPosition.y;
                }
            }
            length += furthestPos;
            return length;
        }
        
        public static float GetBoatLengthDown(Boat boat)
        {
            var length = boat.RootPart.Radius;
            var furthestPos = 0f;
            foreach (var part in boat.Parts)
            {
                if (part.transform.localPosition.y < furthestPos)
                {
                    furthestPos = part.transform.localPosition.y;
                }
            }
            length += Mathf.Abs(furthestPos);
            return length;
        }
        
        public static float GetBoatLengthRight(Boat boat)
        {
            var length = boat.RootPart.Radius;
            var furthestPos = 0f;
            foreach (var part in boat.Parts)
            {
                if (part.transform.localPosition.x > furthestPos)
                {
                    furthestPos = part.transform.localPosition.x;
                }
            }
            length += Mathf.Abs(furthestPos);
            return length;
        }
        
        public static float GetBoatLengthLeft(Boat boat)
        {
            var length = boat.RootPart.Radius;
            var furthestPos = 0f;
            foreach (var part in boat.Parts)
            {
                if (part.transform.localPosition.x < furthestPos)
                {
                    furthestPos = part.transform.localPosition.x;
                }
            }
            length += Mathf.Abs(furthestPos);
            return length;
        }

        public static bool CheckIfWalkable(Vector3 point, BoatPart part)
        {
            var size = new Vector3(1.5f,3f, 1.5f);
            var colls = Physics.OverlapBox(point, size, part.transform.rotation, GlobalConfig.BlockMask);
            if(colls.Length > 0)
                return false;
            return true;
        }
        
        public static bool CheckIfWalkable(Vector3 point, Boat boat)
        {
            var rot = boat.RootPart.transform.rotation;
            var size = boat.OverlapCheckBox;
            var colls = Physics.OverlapBox(point,
                size, rot, GlobalConfig.BlockMask);
            if(colls.Length > 0)
                return false;
            return true;
        }
        
        private static float DistanceSquare(Vector3 p1, Vector3 p2)
        {
            var vec = p1 - p2;
            return vec.x * vec.x + vec.z * vec.z;
        }
        
        private static float DistanceSquare(BoatPart part, Vector3 toPoint)
        {
            var vec = part.Point.position - toPoint;
            return vec.x * vec.x + vec.z * vec.z;
        }
     
        
        public static bool GetFirstTarget(Team team, Vector3 center, float rad, out ITarget chosenTarget, out bool isTower)
        {
            var rad2 = rad * rad;
            // CLog.LogBlue($"NON Players count {TeamsTargetsManager.Inst.NonPlayerTargets.Count}");
            isTower = false;
            foreach (var target in TeamsTargetsManager.Inst.Towers)
            {
                if (target.Team == team || target.Damageable.CanDamage == false)
                    continue;
                var vec = target.Point.position - center;
                var d2 = vec.x * vec.x + vec.z * vec.z;
                if (d2 <= rad2)
                {
                    chosenTarget = target;
                    isTower = true;
                    return true;
                }
            }
            // CLog.LogBlue($"Players count {TeamsTargetsManager.Inst.Players.Count}");
            foreach (var target in TeamsTargetsManager.Inst.Players)
            {
                if (target.Team == team || target.Target.Damageable.CanDamage == false)
                    continue;
                var vec = target.Target.Point.position - center;
                var d2 = vec.x * vec.x + vec.z * vec.z;
                // CLog.Log($"d2 {d2}, rad2 {rad2}");
                if (d2 <= rad2)
                {
                    chosenTarget = target.Target;
                    return true;
                }
            }

            foreach (var target in TeamsTargetsManager.Inst.Targets)
            {
                if (target.Team == team || target.Damageable.CanDamage == false)
                    continue;
                var vec = target.Point.position - center;
                var d2 = vec.x * vec.x + vec.z * vec.z;
                // CLog.Log($"d2 {d2}, rad2 {rad2}");
                if (d2 <= rad2)
                {
                    chosenTarget = target;
                    return true;
                }
            }
   
            chosenTarget = null;
            return false;
        }

    }
}