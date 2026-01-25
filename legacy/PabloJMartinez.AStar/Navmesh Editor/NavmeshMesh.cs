// Copyright (c) 2013-2017 Pablo J. Martínez. All rights reserved.
// Licensed under the MIT License.
// Part of the legacy PabloJMartinez.AStar implementation.

using UnityEngine;
using System;
using System.Collections;

namespace ComingLights
{
    public struct NavmeshMesh : IEquatable<NavmeshMesh>
    {
        public readonly int Navmesh;
        public readonly int Mesh;
        public readonly int Type;
        public readonly float Weight;
        public readonly GameObject GameObject;

        public NavmeshMesh(int navmesh, int mesh, int type, float weight, GameObject gameObject)
        {
            Navmesh = navmesh;
            Mesh = mesh;
            Type = type;
            Weight = weight;
            GameObject = gameObject;
        }

        public bool Equals(NavmeshMesh other)
        {
            if(other == null)
            {
                return false;
            }

            if(this.Navmesh == other.Navmesh &&
               this.Mesh == other.Mesh &&
               this.Type == other.Type &&
               this.Weight == other.Weight &&
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
            NavmeshMesh navmeshMesh = (NavmeshMesh)obj;
            if(navmeshMesh != null)
            {
                return Equals(navmeshMesh);
            }
            else return false;
        }

        public static bool operator ==(NavmeshMesh a, NavmeshMesh b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(NavmeshMesh a, NavmeshMesh b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 486187739 + Navmesh.GetHashCode();
                hash = hash * 486187739 + Mesh.GetHashCode();
                hash = hash * 486187739 + Type.GetHashCode();
                hash = hash * 486187739 + Weight.GetHashCode();
                hash = hash * 486187739 + GameObject.GetHashCode();
                return hash;
            }
        }
    }
}