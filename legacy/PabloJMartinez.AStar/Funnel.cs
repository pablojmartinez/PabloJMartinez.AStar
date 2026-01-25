// Copyright (c) 2013-2017 Pablo J. Martínez. All rights reserved.
// Licensed under the MIT License.
// Part of the legacy PabloJMartinez.AStar implementation.

using UnityEngine;
using System;
using System.Collections;

namespace ComingLights
{
    public class Funnel
    {
        private static Vector3[] path = new Vector3[256];

        public static Vector3[] StringPull(Portal[] portals, Vector3 start, Vector3 goal, ref int pathLength)
        {
            // Find straight path.
            int npts = 0;
            int portalsLength = portals.Length;
            int portalsLengthMinusOne = portalsLength - 1;

            if(portalsLength * 2 > path.Length)
            {
                System.Array.Resize<Vector3>(ref path, portalsLength * 2);
            }
            //Vector3[] path = new Vector3[portalsLength * 2];

            // Init scan state
            Vector3 portalApex;
            Vector3 portalLeft;
            Vector3 portalRight;
            int apexIndex = portalsLengthMinusOne;
            int leftIndex = portalsLengthMinusOne;
            int rightIndex = portalsLengthMinusOne;
            portalApex = start;
            portalLeft = start;
            portalRight = start;

            // Add start point.
            path[npts] = portalApex;
            npts++;

            for(int i = portalsLengthMinusOne; i >= -1; i--)
            {
                Vector3 left;
                Vector3 right;
                if(i > -1)
                {
                    left = Navmesh.Nodes[Navmesh.Active][portals[i].Left];
                    right = Navmesh.Nodes[Navmesh.Active][portals[i].Right];
                }
                else
                {
                    left = goal;
                    right = goal;
                }

                // Update right vertex.
                if(Vector3Util.TriArea2(portalApex, portalRight, right) <= 0.0f) // <= 0.0f // Counterclockwise -> portalRight is on the right of portalApex and right is on the left of portalApex.
                {
                    if(Vector3Util.IsOneVector3EqualToTheOther(portalApex, portalRight) || Vector3Util.TriArea2(portalApex, portalLeft, right) > 0.0f) // > 0.0f // Clockwise -> portalLeft is on the left of portalApex and right is on the right of portalApex
                    {
                        // Tighten the funnel.
                        portalRight = right;
                        rightIndex = i;
                    }
                    else
                    {
                        // Right over left, insert left to path and restart scan from portal left point.
                        path[npts] = portalLeft;
                        npts++;
                        // Make current left the new apex.
                        portalApex = portalLeft;
                        apexIndex = leftIndex;
                        // Reset portal
                        portalLeft = portalApex;
                        portalRight = portalApex;
                        leftIndex = apexIndex;
                        rightIndex = apexIndex;
                        // Restart scan
                        i = apexIndex;
                        continue;
                    }
                }

                // Update left vertex.
                if(Vector3Util.TriArea2(portalApex, portalLeft, left) >= 0.0f) // >= 0.0f
                {
                        if(Vector3Util.IsOneVector3EqualToTheOther(portalApex, portalLeft) || Vector3Util.TriArea2(portalApex, portalRight, left) < 0.0f) // < 0.00f
                        {
                            // Tighten the funnel.
                            portalLeft = left;
                            leftIndex = i;
                        }
                        else
                        {
                            // Left over right, insert right to path and restart scan from portal right point.
                            path[npts] = portalRight;
                            npts++;
                            // Make current right the new apex.
                            portalApex = portalRight;
                            apexIndex = rightIndex;
                            // Reset portal
                            portalLeft = portalApex;
                            portalRight = portalApex;
                            leftIndex = apexIndex;
                            rightIndex = apexIndex;
                            // Restart scan
                            i = apexIndex;
                            continue;
                        }
                }
            }
            path[npts] = goal; // Save the longitude of the path in another variable
            npts++;
            //Array.Resize<Vector3>(ref path, npts+1);
            pathLength = npts;
            return path;
        }
    }
}