// Copyright (c) 2013-2017 Pablo J. Martínez. All rights reserved.
// Licensed under the MIT License.
// Part of the legacy PabloJMartinez.AStar implementation.

using UnityEngine;
using System;
using System.Collections;

namespace ComingLights
{
    public struct Portal : IEquatable<Portal>
    {
        public readonly int Edge;
        public readonly int Left;
        public readonly int Right;

        public Portal(int edge, int left, int right)
        {
            Edge = edge;
            Left = left;
            Right = right;
        }

        public bool Equals(Portal other)
        {
            if(other == null)
            {
                return false;
            }

            if(this.Edge == other.Edge &&
               this.Left == other.Left &&
               this.Right == other.Right)
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
            Portal portal = (Portal)obj;
            if(portal != null)
            {
                return Equals(portal);
            }
            else return false;
        }

        public static bool operator ==(Portal a, Portal b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Portal a, Portal b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 486187739 + Edge.GetHashCode();
                hash = hash * 486187739 + Left.GetHashCode();
                hash = hash * 486187739 + Right.GetHashCode();
                return hash;
            }
        }
    }
}