// Copyright (c) 2013-2017 Pablo J. Martínez. All rights reserved.
// Licensed under the MIT License.
// Part of the legacy PabloJMartinez.AStar implementation.

using UnityEngine;
using System;
using System.Collections;

namespace ComingLights
{
    public struct NodeConnection : IEquatable<NodeConnection>
    {
        public readonly float Cost;
        public readonly int FromEdge;
        public readonly Portal ToPortal;
        public readonly int ToMesh;

        public NodeConnection(float cost, int fromEdge, Portal toPortal, int toMesh)
        {
            Cost = cost;
            FromEdge = fromEdge;
            ToPortal = toPortal;
            ToMesh = toMesh;
        }

        public bool Equals(NodeConnection other)
        {
            if(other == null)
            {
                return false;
            }

            if(this.Cost == other.Cost &&
               this.FromEdge == other.FromEdge &&
               this.ToPortal == other.ToPortal &&
               this.ToMesh == other.ToMesh)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            NodeConnection nodeConnection = (NodeConnection)obj;
            if(nodeConnection != null)
            {
                return Equals(nodeConnection);
            }
            else return false;
        }

        public static bool operator ==(NodeConnection a, NodeConnection b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(NodeConnection a, NodeConnection b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 486187739 + Cost.GetHashCode();
                hash = hash * 486187739 + FromEdge.GetHashCode();
                hash = hash * 486187739 + ToPortal.GetHashCode();
                hash = hash * 486187739 + ToMesh.GetHashCode();
                return hash;
            }
        }
    }
}