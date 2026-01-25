// Copyright (c) 2013-2017 Pablo J. Martínez. All rights reserved.
// Licensed under the MIT License.
// Part of the legacy PabloJMartinez.AStar implementation.

using UnityEngine;
using UnityEngine.Profiling;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ComingLights
{
    public static class AStar
    {
        private const int initialPathLength = 32;
        private static BinaryHeap open = new BinaryHeap(Navmesh.InitialNumberOfNodes);
        private static NodeRecord[] closed = new NodeRecord[Navmesh.InitialNumberOfNodes];
        private static int lastClosedSlot = 0;
        public static NodeRecord NullNodeRecord = new NodeRecord(Navmesh.NullPortal, -1, 0.0f, 0.0f);
        public static NodeRecord InfiniteNodeRecord = new NodeRecord(Navmesh.NullPortal, -1, Single.MaxValue, Single.MaxValue);
        private static NodeRecord[] nodeRecords = new NodeRecord[Navmesh.InitialNumberOfNodes]; // Store where the node comes from
        private static int lastNodeRecordSlot = 0;

        static AStar()
        {
            closed.Fill<NodeRecord>(NullNodeRecord);
        }

        public static Vector3[] Path(Vector3 start, Vector3 goal, ref int pathLength)
        {
            // Initialization BEGIN
            open.Clear();
            lastClosedSlot = 0;
            lastNodeRecordSlot = 0;
            // Initialization END

            //float estimatedTotalCost = Vector3.Distance(start, goal);
            int startNode = -1;
            Portal startPortal = new Portal(startNode, -1, -1);
            int startMesh = -1;
            NodeRecord startNodeRecord = new NodeRecord(Navmesh.NullPortal, -1, 0.0f, 0.0f);
            int goalNode = -2; // -1 is null, -2 is the goal.
            Portal goalPortal = new Portal(goalNode, -1, -1);
            int goalMesh = -1;
            int goalMeshEdge = -1;
            int meshesLength = Navmesh.NavmeshMeshesFull[Navmesh.Active].Length;
            Profiler.BeginSample("Starting A Star");
            for(int i = 0; i < meshesLength; i++)
            {
                if(Navmesh.NavmeshMeshesFull[Navmesh.Active][i] == null) continue;
                if(MeshUtil.IsPointInPoly(Navmesh.NavmeshMeshesFull[Navmesh.Active][i], start) == true)
                {
                    startMesh = i;
                }
                if(MeshUtil.IsPointInPoly(Navmesh.NavmeshMeshesFull[Navmesh.Active][i], goal) == true)
                {
                    goalMesh = i;
                }
                if(startMesh != -1 && goalMesh != -1 && startMesh != goalMesh)
                {
                    float costSoFar;
                    float estimatedCost;
                    int meshLength = Navmesh.NavmeshMeshesPortals[Navmesh.Active][startMesh].Length;
                    for(int j = 0; j < meshLength; j++)
                    {
                        Portal currentMeshPortal = Navmesh.NavmeshMeshesPortals[Navmesh.Active][startMesh][j];
                        costSoFar = Vector3.Distance(start, Navmesh.Edges[Navmesh.Active][currentMeshPortal.Edge]);
                        estimatedCost = costSoFar + Vector3.Distance(Navmesh.Edges[Navmesh.Active][currentMeshPortal.Edge], goal);
                        open.Insert(new NodeRecord(currentMeshPortal, lastNodeRecordSlot, costSoFar, estimatedCost));
                        if(lastNodeRecordSlot == nodeRecords.Length)
                        {
                            Array.Resize<NodeRecord>(ref nodeRecords, nodeRecords.Length*2);
                        }
                        nodeRecords[lastNodeRecordSlot] = startNodeRecord;
                        lastNodeRecordSlot++;
                    }

                    meshLength = Navmesh.NavmeshMeshesPortals[Navmesh.Active][goalMesh].Length;
                    for(int j = 0; j < meshLength; j++)
                    {
                        goalMeshEdge = Navmesh.NavmeshMeshesPortals[Navmesh.Active][goalMesh][j].Edge;
                        Navmesh.Connections[Navmesh.Active][goalMeshEdge][Navmesh.GoalConnection] = new NodeConnection(Vector3.Distance(Navmesh.Edges[Navmesh.Active][goalMeshEdge], goal),
                                                                                                                        goalMeshEdge,
                                                                                                                        goalPortal, 
                                                                                                                        goalMesh);
                    }
                    break;
                }
            }
            Profiler.EndSample();
            if(startMesh != -1 && goalMesh != -1)
            {
                if(startMesh == goalMesh)
                {
                    Vector3[] path = new Vector3[2];
                    path[0] = start;
                    path[1] = goal;
                    pathLength = 2;
                    return path;
                }
                else
                {
                    NodeRecord smallestNode = NullNodeRecord;
                    NodeConnection connectedEdge;
                    NodeRecord connectedNodeRecord;
                    float connectedNodeHeuristic = 0.0f;
                    float connectedNodeCostSoFar = 0.0f;
                    //float connectedNodeEstimatedCost = 0.0f;
                    bool goalHasBeenFound = false;
                    bool isNodeClosed = false;
                    bool isNodeOpen = false;
                    bool hasFoundAShorterRoute = false;
                    int elementToUpdate = 0;
                    int nodesNotOpen = 0;
                    int nodesOpened = 0;
                    while(open.NextFreeSlot > 1) // Heap isn't empty
                    {
                        smallestNode = open.Extract();
                        if(smallestNode.Portal.Edge == goalPortal.Edge)
                        {
                            goalHasBeenFound = true;
                            break;
                        }
                        int edgeConnectionsLength = Navmesh.Connections[Navmesh.Active][smallestNode.Portal.Edge].Length;
                        Profiler.BeginSample("Accessing node connections");
                        for(int i = 0; i < edgeConnectionsLength; i++)
                        {
                            connectedEdge = Navmesh.Connections[Navmesh.Active][smallestNode.Portal.Edge][i];
                            if(connectedEdge.FromEdge == -1) continue;
                            connectedNodeCostSoFar = smallestNode.CostSoFar + connectedEdge.Cost;
                            isNodeClosed = false;
                            isNodeOpen = false;
                            hasFoundAShorterRoute = false;
                            connectedNodeHeuristic = 0.0f;
                            for(int j = 0; j < lastClosedSlot; j++)
                            {
                                if(closed[j].Portal.Edge == connectedEdge.ToPortal.Edge)
                                {
                                    connectedNodeRecord = closed[j];
                                    if(connectedNodeCostSoFar < connectedNodeRecord.CostSoFar)
                                    {
                                        Profiler.BeginSample("Revisiting Closed Node");
                                        connectedNodeHeuristic = connectedNodeRecord.EstimatedCost - connectedNodeRecord.CostSoFar;
                                        closed[j] = NullNodeRecord;
                                        hasFoundAShorterRoute = true;
                                        Profiler.EndSample();
                                    }
                                    isNodeClosed = true;
                                    break;
                                }
                            }
                            if(isNodeClosed == true && hasFoundAShorterRoute == false) continue;
                            for(int j = 1; j < open.NextFreeSlot; j++)
                            {
                                if(open.Array[j].Portal.Edge == connectedEdge.ToPortal.Edge)
                                {
                                    connectedNodeRecord = open.Array[j];
                                    if(connectedNodeCostSoFar < connectedNodeRecord.CostSoFar)
                                    {
                                        Profiler.BeginSample("Revisiting Open Node");
                                        connectedNodeHeuristic = connectedNodeRecord.EstimatedCost - connectedNodeRecord.CostSoFar;
                                        hasFoundAShorterRoute = true;
                                        elementToUpdate = j;
                                        Profiler.EndSample();
                                    }
                                    isNodeOpen = true;
                                    break;
                                }
                            }
                            if(isNodeOpen == true && hasFoundAShorterRoute == false) continue;
                            if(isNodeClosed == false && isNodeOpen == false && connectedEdge.ToPortal.Edge != goalPortal.Edge)
                            {
                                connectedNodeHeuristic = Vector3.Distance(Navmesh.Edges[Navmesh.Active][connectedEdge.ToPortal.Edge], goal);
                            }
                            connectedNodeRecord = new NodeRecord(connectedEdge.ToPortal, lastNodeRecordSlot, connectedNodeCostSoFar, connectedNodeCostSoFar + connectedNodeHeuristic);
                            if(lastNodeRecordSlot == nodeRecords.Length)
                            {
                                Array.Resize<NodeRecord>(ref nodeRecords, nodeRecords.Length*2);
                            }
                            nodeRecords[lastNodeRecordSlot] = smallestNode;
                            lastNodeRecordSlot++;
                            if(isNodeOpen == false)
                            {
                                open.Insert(connectedNodeRecord);
                                nodesNotOpen++;
                            }
                            else
                            {
                                open.Update(elementToUpdate, connectedNodeRecord);
                                nodesOpened++;
                            }
                        }
                        if(lastClosedSlot == closed.Length)
                        {
                            Array.Resize<NodeRecord>(ref closed, closed.Length*2);
                        }
                        closed[lastClosedSlot] = smallestNode;
                        lastClosedSlot++;
                        Profiler.EndSample();
                    }

                    int meshLength = Navmesh.NavmeshMeshesPortals[Navmesh.Active][goalMesh].Length;
                    for(int i = 0; i < meshLength; i++)
                    {
                        goalMeshEdge = Navmesh.NavmeshMeshesPortals[Navmesh.Active][goalMesh][i].Edge;
                        Navmesh.Connections[Navmesh.Active][goalMeshEdge][Navmesh.GoalConnection] = Navmesh.NullConnection;
                    }

                    if(goalHasBeenFound == true)
                    {
                        //Portal[] portals = new Portal[initialPathLength];
                        //portals[0] = goalPortal;
                        //path[1] = Navmesh.Nodes[Navmesh.Active][smallestNode.Node];
                        NodeRecord currentNode = nodeRecords[smallestNode.FromNodeRecord];
                        int length = 1;
                        for(int i = 0; currentNode.FromNodeRecord != -1; i++)
                        {
                            if(currentNode.FromNodeRecord != -1)
                            {
                                currentNode = nodeRecords[currentNode.FromNodeRecord];
                            }
                            length++;
                        }

                        Portal[] portals = new Portal[length - 1];
                        currentNode = nodeRecords[smallestNode.FromNodeRecord];
                        for(int i = 0; currentNode.FromNodeRecord != -1; i++)
                        {
                            if(currentNode.FromNodeRecord != -1)
                            {
                                portals[i] = currentNode.Portal;
                                currentNode = nodeRecords[currentNode.FromNodeRecord];
                            }
                        }
                        //exactLength++;
                        //portals[exactLength] = startPortal;
                        //Array.Resize<Portal>(ref portals, exactLength+1);
                        //Array.Reverse(portals);
                        Vector3[] path = new Vector3[portals.Length+2];
                        int currentPath = 0;
                        path[currentPath] = start;
                        currentPath++;
                        for(int i = portals.Length - 1; i >= 0; i--)
                        {
                            NavmeshGenerator.AddLeftNode(Navmesh.Nodes[Navmesh.Active][portals[i].Left], "Left["+i+"] " + portals[i].Left);
                            NavmeshGenerator.AddLeftNode(Navmesh.Edges[Navmesh.Active][portals[i].Edge], "Edge["+i+"] " + portals[i].Edge);
                            NavmeshGenerator.AddLeftNode(Navmesh.Nodes[Navmesh.Active][portals[i].Right], "Right["+i+"] " + portals[i].Right);
                            path[currentPath] = Navmesh.Edges[Navmesh.Active][portals[i].Edge];
                            currentPath++;
                        }
                        path[currentPath] = goal;
                        currentPath++;
                        pathLength = currentPath;
                        return path; //Funnel.StringPull(portals, start, goal, ref pathLength);
                        //return path;
                    }
                    else
                    {
                        pathLength = -1;
                        return null;
                    }
                }
            }
            else
            {
                pathLength = -1;
                return null;
            }
        }

        public static void MoveTowardsPath(Transform transform, Vector3[] path)
        {
            int pathLength = path.Length;
            for(int i = 0; i < pathLength; i++)
            {
                if(path[i] != Vector3.zero)
                {
                    transform.position = Vector3.MoveTowards(transform.position, path[0], 0.0f);
                }
                else break;
            }
        }

        /*private static float HeuristicEstimation(int start, int goal)
        {
            return Vector3.Distance(Navmesh.Nodes[Navmesh.Active][start], Navmesh.Nodes[Navmesh.Active][goal]);
        }*/
    }
}