// Copyright (c) 2013-2017 Pablo J. Martínez. All rights reserved.
// Licensed under the MIT License.
// Part of the legacy PabloJMartinez.AStar implementation.

using UnityEngine;
using System.Collections;

namespace ComingLights
{
    public static class Navmesh
    {
        public static int InitialNumberOfNavmeshes = 8;
        public static int InitialNumberOfMeshes = 64;
        public static int InitialNumberOfNodes = 512;
        public static int InitialNumberOfEdges = InitialNumberOfNodes;
        public static int GoalConnection = 0;
        public static int InitialNumberOfConnections = 16;
        public static int MaxNumberOfEdgesPerNode = 64;
        public static int MaxNumberOfMeshesPerNode = 64;

        public static int Active = 0; // The current active Navmesh.
        public static int NavmeshesCount = 1;
        public static NavmeshNode[][] NavmeshNodes; // Navmesh Id - Unique Node Id -> NavmeshNode
        public static Vector3[][] Nodes; // Navmesh Id - Unique Node Id -> Node Position
        public static int NodesCount = 0;
        public static int[][][] NodesEdges; // Navmesh Id - Unique Node Id -> Unique Edge Id
        public static int[][][] NodesMeshes; // Navmesh Id - Unique Node Id -> Unique Mesh Id
        public static Vector3[][] Edges; // Navmesh Id - Unique Edge Id -> Edge Position
        public static int EdgesCount = 0;
        public static int[][][] EdgesNodes; // Navmesh Id - Unique Edge Id -> Unique Node Id
        public static int[][][] EdgesMeshes; // Navmesh Id - Unique Edge Id -> Unique Mesh Id
        public static NodeConnection[][][] Connections; // Navmesh Id - Unique Edge Id -> Connection
        public static NavmeshMesh[][] NavmeshMeshes; // Navmesh Id - Mesh Id -> NavmeshMesh
        public static Color[][] NavmeshMeshesTypesColors; // Navmesh Id - Type Id -> Color
        public static int[][][] NavmeshMeshesNodes; // Navmesh Id - Mesh Id -> Unique Node Id
        public static int MeshesCount = 0;
        public static Portal[][][] NavmeshMeshesPortals; // Navmesh Id - Mesh Id -> Portal
        public static Vector3[][][] NavmeshMeshesFull; // Navmesh Id - Mesh Id -> Node Position
        public static Portal NullPortal = new Portal(-1, -1, -1);
        public static NodeConnection NullConnection = new NodeConnection(0.0f, -1, Navmesh.NullPortal, -1);
        public static NavmeshNode NullNavmeshNode = new NavmeshNode(-1, -1, null);
        public static NavmeshMesh NullNavmeshMesh = new NavmeshMesh(-1, -1, -1, 0.0f, null);

        static Navmesh()
        {
            Initialize();
        }

        public static void Initialize()
        {
            Debug.Log("Initialization...");
            NavmeshNodes = CollUtil.CreateJaggedArray<NavmeshNode[][]>(Navmesh.InitialNumberOfNavmeshes, Navmesh.InitialNumberOfNodes);
            Nodes = CollUtil.CreateJaggedArray<Vector3[][]>(Navmesh.InitialNumberOfNavmeshes, Navmesh.InitialNumberOfNodes);
            NodesEdges = CollUtil.CreateJaggedArray<int[][][]>(Navmesh.InitialNumberOfNavmeshes, Navmesh.InitialNumberOfNodes, Navmesh.MaxNumberOfEdgesPerNode);
            NodesEdges.FillJaggedArray<int>(-1);
            NodesMeshes = CollUtil.CreateJaggedArray<int[][][]>(Navmesh.InitialNumberOfNavmeshes, Navmesh.InitialNumberOfNodes, Navmesh.MaxNumberOfMeshesPerNode);
            NodesMeshes.FillJaggedArray<int>(-1);
            Edges = CollUtil.CreateJaggedArray<Vector3[][]>(Navmesh.InitialNumberOfNavmeshes, Navmesh.InitialNumberOfEdges);
            EdgesNodes = CollUtil.CreateJaggedArray<int[][][]>(Navmesh.InitialNumberOfNavmeshes, Navmesh.InitialNumberOfEdges, 2);
            EdgesNodes.FillJaggedArray<int>(-1);
            EdgesMeshes = CollUtil.CreateJaggedArray<int[][][]>(Navmesh.InitialNumberOfNavmeshes, Navmesh.InitialNumberOfEdges, 2);
            EdgesMeshes.FillJaggedArray<int>(-1);
            Connections = CollUtil.CreateJaggedArray<NodeConnection[][][]>(Navmesh.InitialNumberOfNavmeshes, Navmesh.InitialNumberOfEdges, Navmesh.InitialNumberOfConnections);
            Connections.FillJaggedArray<NodeConnection>(NullConnection);
            NavmeshMeshes = CollUtil.CreateJaggedArray<NavmeshMesh[][]>(Navmesh.InitialNumberOfNavmeshes, Navmesh.InitialNumberOfMeshes);
            NavmeshMeshes.FillJaggedArray<NavmeshMesh>(NullNavmeshMesh);
            NavmeshMeshesTypesColors = CollUtil.CreateJaggedArray<Color[][]>(Navmesh.InitialNumberOfNavmeshes, Navmesh.InitialNumberOfMeshes);
            NavmeshMeshesTypesColors[Navmesh.Active][0] = Color.magenta;
            NavmeshMeshesNodes = CollUtil.CreateJaggedArray<int[][][]>(Navmesh.InitialNumberOfNavmeshes, Navmesh.InitialNumberOfMeshes, 0);
            NavmeshMeshesPortals = CollUtil.CreateJaggedArray<Portal[][][]>(Navmesh.InitialNumberOfNavmeshes, Navmesh.InitialNumberOfMeshes, 0);
            NavmeshMeshesFull = CollUtil.CreateJaggedArray<Vector3[][][]>(Navmesh.InitialNumberOfNavmeshes, Navmesh.InitialNumberOfMeshes, 0);
        }
    }
}