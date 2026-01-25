// Copyright (c) 2013-2017 Pablo J. Martínez. All rights reserved.
// Licensed under the MIT License.
// Part of the legacy PabloJMartinez.AStar implementation.

using UnityEngine;
using System;
using System.Collections;

namespace ComingLights
{
    public struct NavmeshNode : IEquatable<NavmeshNode>
    {
        public readonly int Navmesh;
        public readonly int Node;
        public readonly GameObject GameObject;

        public NavmeshNode(int navmesh, int node, GameObject gameObject)
        {
            Navmesh = navmesh;
            Node = node;
            GameObject = gameObject;
        }

        public bool Equals(NavmeshNode other)
        {
            if(other == null)
            {
                return false;
            }

            if(this.Navmesh == other.Navmesh &&
               this.Node == other.Node &&
               this.GameObject == other.GameObject)
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
            NavmeshNode navmeshNode = (NavmeshNode)obj;
            if(navmeshNode != null)
            {
                return Equals(navmeshNode);
            }
            else return false;
        }

        public static bool operator ==(NavmeshNode a, NavmeshNode b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(NavmeshNode a, NavmeshNode b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 486187739 + Navmesh.GetHashCode();
                hash = hash * 486187739 + Node.GetHashCode();
                hash = hash * 486187739 + GameObject.GetHashCode();
                return hash;
            }
        }
    }
}