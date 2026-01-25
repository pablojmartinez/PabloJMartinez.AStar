// Copyright (c) 2013-2017 Pablo J. Martínez. All rights reserved.
// Licensed under the MIT License.
// Part of the legacy PabloJMartinez.AStar implementation.

using UnityEngine;
using System;
using System.Collections;

namespace ComingLights
{
    public struct NodeRecord : IEquatable<NodeRecord>
    {
        public readonly Portal Portal;
        public readonly int FromNodeRecord;
        public readonly float CostSoFar;
        public readonly float EstimatedCost;

        public NodeRecord(Portal portal, int fromNodeRecord, float costSoFar, float estimatedCost)
        {
            Portal = portal;
            FromNodeRecord = fromNodeRecord;
            CostSoFar = costSoFar;
            EstimatedCost = estimatedCost;
        }

        public bool Equals(NodeRecord other)
        {
            if(other == null)
            {
                return false;
            }

            if(this.Portal == other.Portal &&
               this.FromNodeRecord == other.FromNodeRecord &&
               this.CostSoFar == other.CostSoFar &&
               this.EstimatedCost == other.EstimatedCost)
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
            NodeRecord nodeRecord = (NodeRecord)obj;
            if(nodeRecord != null)
            {
                return Equals(nodeRecord);
            }
            else return false;
        }

        public static bool operator ==(NodeRecord a, NodeRecord b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(NodeRecord a, NodeRecord b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 486187739 + Portal.GetHashCode();
                hash = hash * 486187739 + FromNodeRecord.GetHashCode();
                hash = hash * 486187739 + CostSoFar.GetHashCode();
                hash = hash * 486187739 + EstimatedCost.GetHashCode();
                return hash;
            }
        }
    }
}