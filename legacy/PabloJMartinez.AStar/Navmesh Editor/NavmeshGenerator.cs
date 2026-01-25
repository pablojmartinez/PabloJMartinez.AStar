// Copyright (c) 2013-2017 Pablo J. Martínez. All rights reserved.
// Licensed under the MIT License.
// Part of the legacy PabloJMartinez.AStar implementation.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Known bugs:
///     · If you join only one node of the mesh to a different mesh it results in broken A* pathfinding. 
/// </summary>

namespace ComingLights
{
    [DisallowMultipleComponent]
    public class NavmeshGenerator : MonoBehaviour
    {
        public static NavmeshGenerator ThisNavmeshGenerator { get; private set; }
        [SerializeField]
        private Material meshMaterial; // https://github.com/H-man/UnityVertexColors
        private static GameObject pathfindingGameObject;
        private static Transform pathfindingTransform;
        [SerializeField]
        private GameObject nodeModel;
        [SerializeField]
        private GameObject leftModel;
        private static GameObject LeftModel;
        [SerializeField]
        private GameObject rightModel;
        private static GameObject RightModel;
        private static Ray cameraToMouseRay;
        private static RaycastHit raycastHit;
        private enum MouseMode
        {
            Select,
            Add,
            Edit
        }
        private static MouseMode mouseMode = MouseMode.Select;
        //private static int selectedNavmesh = 0; // The Navmesh which is being edited at the moment
        private static List<NavmeshNode> selectedNodes = new List<NavmeshNode>(16);
        private static int selectedNodesCount = 0;
        private static Dictionary<GameObject, NavmeshNode> gameObjectToNodePointers = new Dictionary<GameObject, NavmeshNode>();
        public static List<int> SelectedMeshes = new List<int>(16);
        private static int selectedMeshesCount = 0;
        private static Dictionary<GameObject, int> gameObjectToMeshPointers = new Dictionary<GameObject, int>();
        private static Color unselectedNodeColor = Color.cyan;
        private static Color selectedNodeColor = Color.red;
        private static int lastNavmesh = 0;

        private void Awake()
        {
            ThisNavmeshGenerator = this;
            pathfindingGameObject = ThisNavmeshGenerator.gameObject;
            pathfindingTransform = pathfindingGameObject.transform;
            LeftModel = leftModel;
            RightModel = rightModel;
            ThisNavmeshGenerator.gameObject.SetActive(false);
            Debug.Log("THIS IS NOT WORKING");
            int SelectedMeshesCount = SelectedMeshes.Count;
            for(int i = 0; i < SelectedMeshesCount; i++)
            {
                Debug.Log("Selected Meshes 1 [" + i + "] -> " + SelectedMeshes[i]);
            }
        }

        private void Update()
        {
            int SelectedMeshesCount = SelectedMeshes.Count;
            for(int i = 0; i < SelectedMeshesCount; i++)
            {
                Debug.Log("Selected Meshes 1 [" + i + "] -> " + SelectedMeshes[i]);
            }
            if(Input.GetKeyDown(KeyCode.M))
            {
                if(mouseMode == MouseMode.Select)
                {
                    mouseMode = MouseMode.Add;
                }
                else if(mouseMode == MouseMode.Add)
                {
                    mouseMode = MouseMode.Edit;
                }
                else if(mouseMode == MouseMode.Edit)
                {
                    mouseMode = MouseMode.Select;
                }

                if(mouseMode == MouseMode.Add)
                {
                    ObjectPlacement.Enable(nodeModel);
                }
                else ObjectPlacement.Disable();
                Debug.Log("Mouse Mode: " + mouseMode);
            }

            //! DISABLED TEMPORARILY
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                //!cameraToMouseRay = CLCamera.MainCamera.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(cameraToMouseRay, out raycastHit, 100);
                Vector3 nodePosition = raycastHit.point;
                //nodeModel.transform.position = new Vector3(raycastHit.point.x, raycastHit.point.y + 0.5f, raycastHit.point.z);

                if(mouseMode == MouseMode.Select)
                {
                    GameObject raycastHitGameObject = raycastHit.collider.gameObject;
                    if(!Input.GetKey(KeyCode.LeftControl))
                    {
                        UnselectAllNodes();
                        UnselectAllMeshes();
                    }

                    NavmeshNode resultingNode;
                    //int resultingMesh;

                    if(gameObjectToNodePointers.TryGetValue(raycastHitGameObject, out resultingNode) == true)
                    {
                        if(selectedNodes.Contains(resultingNode) == true)
                        {
                            UnselectNode(resultingNode, raycastHitGameObject);
                        }
                        else
                        {
                            SelectNode(resultingNode, raycastHitGameObject);
                            Debug.Log("CURRENT SELECTED NODE: " + resultingNode.Node);
                        }
                    }
                    Debug.Log("wtf?");
                    /*for(int i = 0; i < SelectedMeshes.Length; i++)
                    {
                        Debug.Log("Selected Meshes 1 [" + i + "] -> " + SelectedMeshes[i]);
                    }
                    resultingMesh = gameObjectToMeshPointers.GetValueOrDefault(raycastHitGameObject, -1);
                    if(resultingMesh != -1)
                    {
                        Debug.Log("Selecting Mesh 1 -> " + resultingMesh);
                        if(SelectedMeshes.Contains(resultingMesh) == true)
                        {
                            Debug.Log("Selecting Mesh 2 -> " + resultingMesh);
                            //UnselectMesh(resultingMesh, raycastHitGameObject);
                        }
                        else
                        {
                            Debug.Log("Selecting Mesh 3");
                            //SelectMesh(resultingMesh, raycastHitGameObject);
                            Debug.Log("CURRENT SELECTED MESH: " + Navmesh.NavmeshMeshes[Navmesh.Active][resultingMesh].Mesh);
                        }
                    }*/
                }
                else if(mouseMode == MouseMode.Add)
                {
                    AddNode(new Vector3(nodePosition.x, nodePosition.y + 0.1f, nodePosition.z));
                }
            }

            /*if(navmeshMeshes.Count > 0)
            {
                Debug.Log("Dot Product between navmeshMeshes and point, node 1 -> " + Vector3.Dot(navmeshMeshes[0][0].normalized, (raycastHit.point - navmeshMeshes[0][0]).normalized));
                Debug.Log("Dot Product between navmeshMeshes and point, node 2 -> " + Vector3.Dot(navmeshMeshes[0][1].normalized, (raycastHit.point - navmeshMeshes[0][1]).normalized));
                Debug.Log("Dot Product between navmeshMeshes and point, node 3 -> " + Vector3.Dot(navmeshMeshes[0][2].normalized, (raycastHit.point - navmeshMeshes[0][2]).normalized));
            }*/

            if(Input.GetKeyDown(KeyCode.H))
            {
                bool isPointInPoly = false;
                //!cameraToMouseRay = CLCamera.MainCamera.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(cameraToMouseRay, out raycastHit, 100);
                //isPointInPoly = (MeshUtil.FindMeshContainingPoint(navmeshMeshes[Navmesh.Active], raycastHit.point) != null) ? true : false;
                Debug.Log("Is Point in Poly: " + isPointInPoly);
            }

            // Link selected nodes
            if(Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log("Linking selected nodes...");
                Debug.Log("selectedNodesCount -> " + selectedNodesCount);
                if(selectedNodesCount > 2)
                {
                    int[] meshNodes = new int[selectedNodesCount];
                    for(int i = 0; i < selectedNodesCount; i++)
                    {
                        meshNodes[i] = selectedNodes[i].Node;
                    }
                    Vector3[] mesh = new Vector3[selectedNodesCount];
                    for(int i = 0; i < selectedNodesCount; i++)
                    {
                        mesh[i] = Navmesh.Nodes[Navmesh.Active][meshNodes[i]];
                    }
                    Debug.Log("IS THIS MESH CLOCKWISE? -> " + MeshUtil.IsMeshClockwise(mesh));
                    Debug.Log("IS THIS MESH CONVEX? -> " + MeshUtil.IsMeshConvex(mesh));
                    Debug.Log("TRI AREA!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! -> " + Vector3Util.TriArea2(Navmesh.Nodes[Navmesh.Active][meshNodes[0]],
                                                                                       Navmesh.Nodes[Navmesh.Active][meshNodes[1]],
                                                                                       Navmesh.Nodes[Navmesh.Active][meshNodes[2]]));
                    if(MeshUtil.IsMeshConvex(mesh) == true && MeshUtil.IsMeshClockwise(mesh) == true)
                    {
                        NewMesh(meshNodes);
                    }
                }
                /*if(selectedNodesCount == Navmesh.MaxNumberOfNodesPerMesh)
                {
                    int[] triangleNodes = new int[Navmesh.MaxNumberOfNodesPerMesh];
                    for(int i = 0; i < selectedNodesCount; i++)
                    {
                        triangleNodes[i] = selectedNodes[i].Node;
                    }
                    Debug.Log("TRI AREA!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! -> " + Funnel.TriArea2(Navmesh.Nodes[Navmesh.Active][triangleNodes[0]],
                                                                                       Navmesh.Nodes[Navmesh.Active][triangleNodes[1]],
                                                                                       Navmesh.Nodes[Navmesh.Active][triangleNodes[2]]));
                    NewMesh(triangleNodes);
                }*/
            }

            if(Input.GetKeyDown(KeyCode.Delete))
            {
                if(selectedNodesCount > 0)
                {
                    for(int i = 0; i < selectedNodesCount; i++)
                    {
                        RemoveNode(selectedNodes[i].Node);
                    }
                }
            }
        }

        private int AddEdge(int leftNode, int rightNode)
        {
            Vector3[] nodesToGetTheEdgePosition = new Vector3[2];
            nodesToGetTheEdgePosition[0] = Navmesh.Nodes[Navmesh.Active][leftNode];
            nodesToGetTheEdgePosition[1] = Navmesh.Nodes[Navmesh.Active][rightNode];
            Vector3 edgePosition = MeshUtil.GetCenter(nodesToGetTheEdgePosition);
            int newEdge = -1;
            int edgesLength = Navmesh.Edges[Navmesh.Active].Length;
            for(int i = 0; i < edgesLength; i++)
            {
                if(Vector3Util.IsOneVector3EqualToTheOther(Navmesh.Edges[Navmesh.Active][i], Vector3.zero))
                {
                    Navmesh.Edges[Navmesh.Active][i] = edgePosition;
                    newEdge = i;
                    if(i == edgesLength - 1)
                    {
                        System.Array.Resize<Vector3>(ref Navmesh.Edges[Navmesh.Active], edgesLength * 2);
                        System.Array.Resize<int[]>(ref Navmesh.EdgesNodes[Navmesh.Active], edgesLength * 2);
                        System.Array.Resize<int[]>(ref Navmesh.EdgesMeshes[Navmesh.Active], edgesLength * 2);
                        System.Array.Resize<NodeConnection[]>(ref Navmesh.Connections[Navmesh.Active], edgesLength * 2);
                    }
                    Navmesh.EdgesCount++;
                    break;
                }
            }
            for(int i = 0; i < Navmesh.MaxNumberOfEdgesPerNode; i++)
            {
                if(Navmesh.NodesEdges[Navmesh.Active][leftNode][i] == -1)
                {
                    Navmesh.NodesEdges[Navmesh.Active][leftNode][i] = newEdge;
                    break;
                }
            }
            for(int i = 0; i < Navmesh.MaxNumberOfEdgesPerNode; i++)
            {
                if(Navmesh.NodesEdges[Navmesh.Active][rightNode][i] == -1)
                {
                    Navmesh.NodesEdges[Navmesh.Active][rightNode][i] = newEdge;
                    break;
                }
            }
            Navmesh.EdgesNodes[Navmesh.Active][newEdge][0] = leftNode;
            Navmesh.EdgesNodes[Navmesh.Active][newEdge][1] = rightNode;
            Debug.Log("Last Edge -> " + newEdge);
            return newEdge;
        }

        private void RemoveEdge(Portal portal)
        {
            //portal = new Portal(Vector3.zero, 0, 0);
        }

        public static int AddNode(Vector3 position, int newNode = -1)
        {
            if(newNode == -1)
            {
                int nodesLength = Navmesh.Nodes[Navmesh.Active].Length;
                for(int i = 0; i < nodesLength; i++)
                {
                    if(Vector3Util.IsOneVector3EqualToTheOther(Navmesh.Nodes[Navmesh.Active][i], Vector3.zero))
                    {
                        Navmesh.Nodes[Navmesh.Active][i] = position;
                        newNode = i;
                        if(i == nodesLength - 1)
                        {
                            System.Array.Resize<Vector3>(ref Navmesh.Nodes[Navmesh.Active], nodesLength * 2);
                            System.Array.Resize<int[]>(ref Navmesh.NodesEdges[Navmesh.Active], nodesLength * 2);
                            System.Array.Resize<int[]>(ref Navmesh.NodesMeshes[Navmesh.Active], nodesLength * 2);
                        }
                        Navmesh.NodesCount++;
                        break;
                    }
                }
            }
            if(newNode != -1 && Vector3Util.IsOneVector3EqualToTheOther(position, Vector3.zero) == false)
            {
                GameObject nodeGameObject;
                if(ObjectPlacement.TheObject != null)
                {
                    nodeGameObject = ObjectPlacement.TheObject;
                }
                else
                {
                    nodeGameObject = Instantiate<GameObject>(ThisNavmeshGenerator.nodeModel);
                }
                Debug.Log("nodePointer -> " + nodeGameObject);
                nodeGameObject.GetComponent<Renderer>().material.color = unselectedNodeColor;
                nodeGameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                nodeGameObject.transform.position = position;
                nodeGameObject.name = "Node Pointer";
                nodeGameObject.transform.parent = pathfindingTransform;
                NavmeshNode navmeshNode = new NavmeshNode(Navmesh.Active, newNode, nodeGameObject);
                Navmesh.NavmeshNodes[Navmesh.Active][newNode] = navmeshNode;
                gameObjectToNodePointers.Add(nodeGameObject, navmeshNode);
                Debug.Log("Last Node -> " + newNode + " position -> " + position);
            }
            return newNode;
            //nodeModelToAdd = Instantiate<GameObject>(nodeModel);
        }

        private void RemoveNode(int node)
        {
            Navmesh.Nodes[Navmesh.Active][node] = Vector3.zero;
            Navmesh.NodesCount--;
            for(int i = 0; i < Navmesh.MaxNumberOfEdgesPerNode; i++)
            {
                int nodeEdge = Navmesh.NodesEdges[Navmesh.Active][node][i];
                if(nodeEdge == -1) continue;
                int theOtherNodeFromThisEdge = -1;
                Navmesh.NodesEdges[Navmesh.Active][node][i] = -1;
                int EdgesNodesLength = Navmesh.NodesEdges[Navmesh.Active][nodeEdge].Length;
                for(int j = 0; j < 2; j++)
                {
                    if(Navmesh.EdgesNodes[Navmesh.Active][nodeEdge][j] != node)
                    {
                        theOtherNodeFromThisEdge = Navmesh.EdgesNodes[Navmesh.Active][nodeEdge][j];
                        for(int k = 0; k < Navmesh.MaxNumberOfEdgesPerNode; k++)
                        {
                            if(Navmesh.NodesEdges[Navmesh.Active][theOtherNodeFromThisEdge][k] == nodeEdge)
                            {
                                Navmesh.NodesEdges[Navmesh.Active][theOtherNodeFromThisEdge][k] = -1;
                            }
                        }
                    }
                    Navmesh.EdgesNodes[Navmesh.Active][nodeEdge][j] = -1;
                    Navmesh.EdgesMeshes[Navmesh.Active][nodeEdge][j] = -1;
                }
                int connectionsLength = Navmesh.Connections[Navmesh.Active][nodeEdge].Length;
                for(int k = 0; k < connectionsLength; k++)
                {
                    Navmesh.Connections[Navmesh.Active][nodeEdge][k] = Navmesh.NullConnection;
                }
                Navmesh.Edges[Navmesh.Active][nodeEdge] = Vector3.zero;
                Navmesh.EdgesCount--;
            }
            GameObject resultingMesh;
            int navmeshMeshesLength = Navmesh.NavmeshMeshesNodes[Navmesh.Active].Length;
            int[] affectedMeshes = new int[navmeshMeshesLength];
            int lastAffectedMesh = 0;
            for(int i = 0; i < navmeshMeshesLength; i++)
            {
                if(Navmesh.NavmeshMeshesNodes[Navmesh.Active][i] != null)
                {
                    int meshLength = Navmesh.NavmeshMeshesNodes[Navmesh.Active][i].Length;
                    for(int j = 0; j < meshLength; j++)
                    {
                        if(Navmesh.NavmeshMeshesNodes[Navmesh.Active][i][j] == node)
                        {
                            affectedMeshes[lastAffectedMesh] = i;
                            lastAffectedMesh++;
                        }
                    }
                }
            }
            for(int i = 0; i < lastAffectedMesh; i++)
            {
                int navmeshMeshesPortalsLength = Navmesh.NavmeshMeshesPortals[Navmesh.Active][affectedMeshes[i]].Length;
                for(int j = 0; j < navmeshMeshesPortalsLength; j++)
                {
                    int edgeThatMayBeRemoved = Navmesh.NavmeshMeshesPortals[Navmesh.Active][affectedMeshes[i]][j].Edge;
                    if(edgeThatMayBeRemoved == -1) continue;
                    int canTheEdgeBeCompletelyDeleted = 0; // 2 means that yes, it can
                    for(int k = 0; k < 2; k++)
                    {
                        if(Navmesh.EdgesMeshes[Navmesh.Active][edgeThatMayBeRemoved][k] == -1)
                        {
                            canTheEdgeBeCompletelyDeleted++;
                            continue;
                        }

                        for(int kk = 0; kk < lastAffectedMesh; kk++)
                        {
                            if(Navmesh.EdgesMeshes[Navmesh.Active][edgeThatMayBeRemoved][k] == affectedMeshes[kk])
                            {
                                int connectionsLength = Navmesh.Connections[Navmesh.Active][edgeThatMayBeRemoved].Length;
                                for(int kkk = 0; kkk < connectionsLength; kkk++)
                                {
                                    if(Navmesh.Connections[Navmesh.Active][edgeThatMayBeRemoved][kkk].ToMesh == affectedMeshes[kk])
                                    {
                                        Navmesh.Connections[Navmesh.Active][edgeThatMayBeRemoved][kkk] = Navmesh.NullConnection;
                                    }
                                }
                                canTheEdgeBeCompletelyDeleted++;
                                break;
                            }
                        }
                    }
                    Debug.Log("canTheEdgeBeCompletelyDeleted -> " + canTheEdgeBeCompletelyDeleted);
                    // The edge must be deleted completely if it's not connected to any mesh other than the affected ones
                    if(canTheEdgeBeCompletelyDeleted == 2)
                    {
                        for(int k = 0; k < 2; k++)
                        {
                            int nodeFromThisEdge = Navmesh.EdgesNodes[Navmesh.Active][edgeThatMayBeRemoved][k];
                            if(nodeFromThisEdge == -1) continue;
                            for(int l = 0; l < Navmesh.MaxNumberOfEdgesPerNode; l++)
                            {
                                if(Navmesh.NodesEdges[Navmesh.Active][nodeFromThisEdge][l] == edgeThatMayBeRemoved)
                                {
                                    Navmesh.NodesEdges[Navmesh.Active][nodeFromThisEdge][l] = -1;
                                }
                            }
                            Navmesh.EdgesNodes[Navmesh.Active][edgeThatMayBeRemoved][k] = -1;
                            Navmesh.EdgesMeshes[Navmesh.Active][edgeThatMayBeRemoved][k] = -1;
                        }
                        int connectionsLength = Navmesh.Connections[Navmesh.Active][edgeThatMayBeRemoved].Length;
                        for(int k = 0; k < connectionsLength; k++)
                        {
                            Navmesh.Connections[Navmesh.Active][edgeThatMayBeRemoved][k] = Navmesh.NullConnection;
                        }
                        Navmesh.Edges[Navmesh.Active][edgeThatMayBeRemoved] = Vector3.zero;
                        Navmesh.EdgesCount--;
                    }
                }

                NavmeshMesh navmeshMesh = Navmesh.NavmeshMeshes[Navmesh.Active][affectedMeshes[i]];
                gameObjectToMeshPointers.Remove(navmeshMesh.GameObject);
                Destroy(navmeshMesh.GameObject);
                Navmesh.NavmeshMeshesNodes[Navmesh.Active][affectedMeshes[i]] = null;
                Navmesh.NavmeshMeshesFull[Navmesh.Active][affectedMeshes[i]] = null;
                Navmesh.NavmeshMeshesPortals[Navmesh.Active][affectedMeshes[i]] = null;
                Navmesh.NavmeshMeshes[Navmesh.Active][affectedMeshes[i]] = Navmesh.NullNavmeshMesh;
                SelectedMeshes.Remove(affectedMeshes[i]);
                Navmesh.MeshesCount--;
            }
            NavmeshNode navmeshNode = Navmesh.NavmeshNodes[Navmesh.Active][node];
            resultingMesh = navmeshNode.GameObject;
            gameObjectToNodePointers.Remove(resultingMesh);
            Destroy(resultingMesh);
            selectedNodes.Remove(navmeshNode);
            Navmesh.NavmeshNodes[Navmesh.Active][node] = Navmesh.NullNavmeshNode;
        }

        /*private void RemoveNode(int node)
        {
            GameObject resultingMesh;
            Navmesh.Nodes[Navmesh.Active][node] = Vector3.zero;
            Navmesh.Connections[Navmesh.Active][node].Populate<NodeConnection>(Navmesh.NullConnection);
            int connectionsLength = Navmesh.Connections.Length;
            for(int i = 0; i < connectionsLength; i++)
            {
                for(int j = 0; j < Navmesh.MaxNumberOfNodeConnections; j++)
                {
                    if(Navmesh.Connections[Navmesh.Active][i][j].ToNode == node)
                    {
                        Navmesh.Connections[Navmesh.Active][i][j] = Navmesh.NullConnection;
                    }
                }
            }
            bool meshHasBeenDestroyed = false;
            int navmeshMeshesLength = Navmesh.NavmeshMeshes[Navmesh.Active].Length;
            for(int i = 0; i < navmeshMeshesLength; i++)
            {
                if(Navmesh.NavmeshMeshes[Navmesh.Active][i] != null)
                {
                    for(int j = 0; j < Navmesh.MaxNumberOfNodesPerMesh; j++)
                    {
                        if(Navmesh.NavmeshMeshes[Navmesh.Active][i][j] == node)
                        {
                            NavmeshMesh navmeshMesh = new NavmeshMesh(Navmesh.Active, i);
                            if(navmeshMeshPointers.TryGetValue(navmeshMesh, out resultingMesh) == true)
                            {
                                DestroyObject(resultingMesh);
                            }
                            navmeshMeshPointers.Remove(navmeshMesh);
                            meshHasBeenDestroyed = true;
                        }
                    }
                    if(meshHasBeenDestroyed == true)
                    {
                        Navmesh.NavmeshMeshes[Navmesh.Active][i] = null;
                        Navmesh.NavmeshFullMeshes[Navmesh.Active][i] = null;
                        meshHasBeenDestroyed = false;
                    }
                }
            }
            NavmeshNode navmeshNode = new NavmeshNode(Navmesh.Active, node);
            if(navmeshFromNodePointers.TryGetValue(navmeshNode, out resultingMesh) == true)
            {
                selectedNodePointers.Remove(resultingMesh);
                navmeshToNodePointers.Remove(resultingMesh);
                DestroyObject(resultingMesh);
            }
            selectedNodes.Remove(navmeshNode);
            navmeshFromNodePointers.Remove(navmeshNode);
        }*/

        private void NewMesh(int[] nodes)
        {
            int nodesLength = nodes.Length;
            if(IsMeshDuplicated2(nodes) == false)
            {
                int meshId = -1;
                int navmeshMeshesLength = Navmesh.NavmeshMeshesNodes[Navmesh.Active].Length;
                for(int i = 0; i < navmeshMeshesLength; i++)
                {
                    if(Navmesh.NavmeshMeshesNodes[Navmesh.Active][i] == null)
                    {
                        meshId = i;
                        Navmesh.NavmeshMeshesNodes[Navmesh.Active][meshId] = new int[nodesLength];
                        Vector3[] meshNodes = new Vector3[nodesLength];
                        Navmesh.NavmeshMeshesFull[Navmesh.Active][meshId] = new Vector3[nodesLength];
                        for(int j = 0; j < nodesLength; j++)
                        {
                            Navmesh.NavmeshMeshesNodes[Navmesh.Active][meshId][j] = nodes[j];
                            meshNodes[j] = Navmesh.Nodes[Navmesh.Active][nodes[j]];
                            Navmesh.NavmeshMeshesFull[Navmesh.Active][meshId][j] = meshNodes[j];
                        }
                        if(MeshUtil.IsMeshClockwise(meshNodes) == false)
                        {
                            MeshUtil.ToggleMeshClockwiseOrder(nodes);
                            MeshUtil.ToggleMeshClockwiseOrder(meshNodes);
                            MeshUtil.ToggleMeshClockwiseOrder(Navmesh.NavmeshMeshesNodes[Navmesh.Active][meshId]);
                            MeshUtil.ToggleMeshClockwiseOrder(Navmesh.NavmeshMeshesFull[Navmesh.Active][meshId]);
                        }
                        GameObject mesh = CreateMesh(meshNodes, meshId, 0, 0.0f);
                        if(i == navmeshMeshesLength - 1)
                        {
                            System.Array.Resize<NavmeshMesh>(ref Navmesh.NavmeshMeshes[Navmesh.Active], navmeshMeshesLength * 2);
                            System.Array.Resize<int[]>(ref Navmesh.NavmeshMeshesNodes[Navmesh.Active], navmeshMeshesLength * 2);
                            System.Array.Resize<Portal[]>(ref Navmesh.NavmeshMeshesPortals[Navmesh.Active], navmeshMeshesLength * 2);
                            System.Array.Resize<Vector3[]>(ref Navmesh.NavmeshMeshesFull[Navmesh.Active], navmeshMeshesLength * 2);
                        }
                        Navmesh.MeshesCount++;
                        break;
                    }
                }
                if(meshId != -1)
                {
                    int[] edges = new int[nodesLength];
                    int[] left = new int[nodesLength];
                    int[] right = new int[nodesLength];
                    Navmesh.NavmeshMeshesPortals[Navmesh.Active][meshId] = new Portal[nodesLength];
                    for(int i = 0; i < nodesLength; i++)
                    {
                        left[i] = i;
                        right[i] = (i+1) % nodesLength;
                        int edgeFromNodes = GetEdgeFromNodes(nodes[left[i]], nodes[right[i]]);
                        //Instantiate<GameObject>(leftModel).transform.position = Navmesh.Nodes[Navmesh.Active][nodes[left[i]]];
                        //Instantiate<GameObject>(rightModel).transform.position = Navmesh.Nodes[Navmesh.Active][nodes[right[i]]];
                        if(edgeFromNodes == -1)
                        {
                            edges[i] = AddEdge(nodes[left[i]], nodes[right[i]]);
                        }
                        else
                        {
                            Debug.Log("It's happening baby! 2");
                            edges[i] = edgeFromNodes;
                        }
                        for(int j = 0; j < 2; j++)
                        {
                            if(Navmesh.EdgesMeshes[Navmesh.Active][edges[i]][j] == -1)
                            {
                                Navmesh.EdgesMeshes[Navmesh.Active][edges[i]][j] = meshId;
                                break;
                            }
                        }
                        Navmesh.NavmeshMeshesPortals[Navmesh.Active][meshId][i]  = new Portal(edges[i], nodes[left[i]], nodes[right[i]]);
                    }
                    for(int i = 0; i < nodesLength; i++)
                    {
                        for(int j = 0; j < nodesLength; j++)
                        {
                            if(i != j)
                            {
                                float cost = Vector3.Distance(Navmesh.Edges[Navmesh.Active][edges[i]], Navmesh.Edges[Navmesh.Active][edges[j]]);
                                NodeConnection connectionFromEdgeToPortal = new NodeConnection(cost, edges[i], new Portal(edges[j], nodes[left[j]], nodes[right[j]]), meshId);
                                // You must start at 1 when adding the connections because 0 is reserved for the from Goal to Edge connection in A Star, when initializing.
                                int connectionsLength = Navmesh.Connections[Navmesh.Active][edges[i]].Length;
                                for(int k = 1; k < connectionsLength; k++)
                                {
                                    if(Navmesh.Connections[Navmesh.Active][edges[i]][k] == Navmesh.NullConnection)
                                    {
                                        Navmesh.Connections[Navmesh.Active][edges[i]][k] = connectionFromEdgeToPortal;
                                        if(i == connectionsLength - 1)
                                        {
                                            System.Array.Resize<NodeConnection>(ref Navmesh.Connections[Navmesh.Active][edges[i]], connectionsLength * 2);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /*private void LinkNode(int fromNode, int toNode, Portal portal, bool bidirectional = false)
        {
            bool isNodeAlreadyConnected = false;
            for(int k = 0; k < Navmesh.MaxNumberOfNodeConnections; k++)
            {
                if(Navmesh.Connections[Navmesh.Active][fromNode][k].ToNode == toNode)
                {
                    isNodeAlreadyConnected = true;
                    break;
                }
            }
            if(isNodeAlreadyConnected == false)
            {
                for(int k = 0; k < Navmesh.MaxNumberOfNodeConnections; k++)
                {
                    if(Navmesh.Connections[Navmesh.Active][fromNode][k].ToNode == -1)
                    {
                        float cost = Vector3.Distance(Navmesh.Nodes[Navmesh.Active][fromNode], Navmesh.Nodes[Navmesh.Active][toNode]);
                        Navmesh.Connections[Navmesh.Active][fromNode][k] = new NodeConnection(cost, fromNode, toNode, portal);
                        Debug.Log("Node: " + fromNode + " has been connected to: " + toNode + " with cost: " + cost);
                        break;
                    }
                }
            }
            if(bidirectional == true)
            {
                LinkNode(toNode, fromNode, portal, bidirectional: false);
            }
        }

        /*private void LinkNode(int node, int nodeToLink)
        {
            bool isNodeAlreadyConnected = false;
            int connectionsLength = connections[Navmesh.Active][node].Length;
            for(int i = 0; i < connectionsLength; i++)
            {
                if(connections[Navmesh.Active][node][i].Node == nodeToLink)
                {
                    isNodeAlreadyConnected = true;
                    break;
                }
            }
            for(int i = 0; i < connectionsLength; i++)
            {
                if(isNodeAlreadyConnected == false && connections[Navmesh.Active][node][i].Node == -1)
                {
                    float cost = Vector3.Distance(nodes[Navmesh.Active][node], nodes[Navmesh.Active][nodeToLink]);
                    connections[Navmesh.Active][node][i] = new NodeConnection(nodeToLink, cost);
                    Debug.Log("Node: " + node + " has been connected to: " + nodeToLink + " with cost: " + cost);
                    break;
                }
            }
            bool isNodeInThisMesh = false;
            bool nodeHasBeenLinked = false;
            bool isMeshDuplicated = false;
            int navmeshMeshesLength = navmeshMeshes[Navmesh.Active].Length;
            for(int i = 0; i < navmeshMeshesLength; i++)
            {
                if(navmeshMeshes[Navmesh.Active][i] != null)
                {
                    isNodeInThisMesh = false;
                    nodeHasBeenLinked = false;
                    isMeshDuplicated = false;
                    for(int j = 0; j < maxNumberOfNodesPerMesh; j++)
                    {
                        if(navmeshMeshes[Navmesh.Active][i][j] == node)
                        {
                            isNodeInThisMesh = true;
                        }
                        if(isNodeInThisMesh == true)
                        {
                            if(navmeshMeshes[Navmesh.Active][i][j] == -1)
                            {
                                int[] newNavmeshMesh = new int[maxNumberOfNodesPerMesh];
                                for(int k = 0; k < maxNumberOfNodesPerMesh; k++)
                                {
                                    newNavmeshMesh[k] = navmeshMeshes[Navmesh.Active][i][k];
                                }
                                newNavmeshMesh[j] = nodeToLink;
                                isMeshDuplicated = IsMeshDuplicated(newNavmeshMesh);
                                if(isMeshDuplicated == false)
                                {
                                    AddNodeToMesh(nodeToLink, i, j);
                                    nodeHasBeenLinked = true;
                                }
                            }
                            /*else if(j == maxNumberOfNodesPerMesh-1)
                            {
                                Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAYYYYYYYYYYY: " + i);
                                int[] thisNavmeshMesh = new int[maxNumberOfNodesPerMesh];
                                for(int k = 0; k < maxNumberOfNodesPerMesh; k++)
                                {
                                    thisNavmeshMesh[k] = navmeshMeshes[Navmesh.Active][i][k];
                                }
                                isMeshDuplicated = IsMeshDuplicated(thisNavmeshMesh);
                                i = navmeshMeshesLength; // Stops the outer loop
                                break;
                            }*//*
                        }
                        if(nodeHasBeenLinked == true)
                        {
                            if(j == maxNumberOfNodesPerMesh-1 && navmeshMeshes[Navmesh.Active][i][j] != -1)
                            {
                                Vector3[] triangleNodes = new Vector3[maxNumberOfNodesPerMesh];
                                for(int k = 0; k < maxNumberOfNodesPerMesh; k++)
                                {
                                    triangleNodes[k] = nodes[Navmesh.Active][navmeshMeshes[Navmesh.Active][i][k]];
                                }
                                GameObject triangleMesh = CreateTriangleMesh(triangleNodes);
                                navmeshMeshPointers.Add(new NavmeshMesh(Navmesh.Active, i), triangleMesh);
                            }
                            i = navmeshMeshesLength; // Stops the outer loop
                            break;
                        }
                    }
                }
            }
            if(isMeshDuplicated == false && nodeHasBeenLinked == false)
            {
                for(int i = 0; i < navmeshMeshesLength; i++)
                {
                    if(navmeshMeshes[Navmesh.Active][i] == null)
                    {
                        navmeshMeshes[Navmesh.Active][i] = new int[maxNumberOfNodesPerMesh];
                        AddNodeToMesh(node, i, 0);
                        AddNodeToMesh(nodeToLink, i, 1);
                        AddNodeToMesh(-1, i, 2);
                        Debug.Log("CALLED i -> " + i);
                        break;
                    }
                }
            }
        }*/

        /*private void UnlinkNode(int node)
        {

        }*/

        /*private void RemoveEdgeIfItIsNotOnlyInTheseMeshes(int edge, int[] meshes)
        {
            bool isItInOneOfTheseMeshes = false;
            int meshesLength = meshes.Length;
            for(int i = 0; i < meshesLength; i++)
            {
                Portal portal;
                int navmeshMeshesPortalsLength = Navmesh.NavmeshMeshesPortals[Navmesh.Active][meshes[i]].Length;
                for(int j = 0; j < navmeshMeshesPortalsLength; j++)
                {
                    portal = Navmesh.NavmeshMeshesPortals[Navmesh.Active][meshes[i]][j];
                    if(portal.Edge == edge)
                    {

                    }
                }
            }
        }*/

        private int GetEdgeFromNodes(int node1, int node2)
        {
            for(int i = 0; i < Navmesh.MaxNumberOfEdgesPerNode; i++)
            {
                for(int j = 0; j < Navmesh.MaxNumberOfEdgesPerNode; j++)
                {
                    if(Navmesh.NodesEdges[Navmesh.Active][node1][i] == Navmesh.NodesEdges[Navmesh.Active][node2][j])
                    {
                        return Navmesh.NodesEdges[Navmesh.Active][node2][j];
                    }
                }
            }
            return -1;
        }


        private bool AreTheseNodesAEdge(int node1, int node2)
        {
            for(int i = 0; i < Navmesh.MaxNumberOfEdgesPerNode; i++)
            {
                for(int j = 0; j < Navmesh.MaxNumberOfEdgesPerNode; j++)
                {
                    if(Navmesh.NodesEdges[Navmesh.Active][node1][i] == Navmesh.NodesEdges[Navmesh.Active][node2][j])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool NodeHasEdges(int node)
        {
            for(int i = 0; i < Navmesh.MaxNumberOfEdgesPerNode; i++)
            {
                if(Navmesh.NodesEdges[Navmesh.Active][node][i] != -1)
                {
                    return true;
                }
            }
            return false;
        }

        /*private bool NodeHasConnections(int node)
        {
            int connectionsLength = Navmesh.Connections[Navmesh.Active][node].Length;
            for(int i = 0; i < connectionsLength; i++)
            {
                if(Navmesh.Connections[Navmesh.Active][node][i].ToNode != -1)
                {
                    return true;
                }
            }
            return false;
        }*/

        private void AddNodeToMesh(int node, int mesh, int slot)
        {
            Navmesh.NavmeshMeshesNodes[Navmesh.Active][mesh][slot] = node;
            if(node != -1 && mesh != -1)
            {
                if(IsNodeInMesh(node, mesh) == false)
                {
                    int navmeshMeshesInNodesLength = Navmesh.NodesMeshes[Navmesh.Active][node].Length;
                    for(int i = 0; i < navmeshMeshesInNodesLength; i++)
                    {
                        if(Navmesh.NodesMeshes[Navmesh.Active][node][i] == -1)
                        {
                            Navmesh.NodesMeshes[Navmesh.Active][node][i] = mesh;
                            break;
                        }
                    }
                }
            }
        }

        private bool IsNodeInMesh(int node, int mesh)
        {
            if(node != -1 && mesh != -1)
            {
                int navmeshMeshesInNodesLength = Navmesh.NodesMeshes[Navmesh.Active][node].Length;
                for(int i = 0; i < navmeshMeshesInNodesLength; i++)
                {
                    if(Navmesh.NodesMeshes[Navmesh.Active][node][i] == mesh)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsMeshDuplicated2(int[] nodes)
        {
            int nodesLength = nodes.Length;
            for(int i = 0; i < nodesLength; i++)
            {
                if(NodeHasEdges(nodes[i]) == false)
                {
                    return false;
                }
            }
            int matches = 0;
            int navmeshMeshesLength = Navmesh.NavmeshMeshesNodes[Navmesh.Active].Length;
            for(int i = 0; i < navmeshMeshesLength; i++)
            {
                if(Navmesh.NavmeshMeshesNodes[Navmesh.Active][i] != null && Navmesh.NavmeshMeshesNodes[Navmesh.Active][i].Length == nodesLength)
                {
                    for(int j = 0; j < nodesLength; j++)
                    {
                        for(int k = 0; k < nodesLength; k++)
                        {
                            if(Navmesh.NavmeshMeshesNodes[Navmesh.Active][i][j] == nodes[k])
                            {
                                matches++;
                            }
                        }
                    }
                    if(matches == nodesLength)
                    {
                        return true;
                    }
                    matches = 0;
                }
            }
            return false;
        }

        /*private bool IsMeshDuplicated(int[] nodes)
        {
            if(nodes != null)
            {
                int referenceNode = 0;
                int nodesLength = nodes.Length;
                for(int i = 0; i < nodesLength; i++)
                {
                    if(nodes[i] != -1)
                    {
                        referenceNode = nodes[i];
                        break;
                    }
                }
                int matches = 0;
                int navmeshMeshesInNodesLength = Navmesh.NavmeshMeshesInNodes[Navmesh.Active][referenceNode].Length;
                for(int i = 0; i < navmeshMeshesInNodesLength; i++)
                {
                    int currentMesh = Navmesh.NavmeshMeshesInNodes[Navmesh.Active][referenceNode][i];
                    if(currentMesh != -1 && Navmesh.NavmeshMeshes[Navmesh.Active][currentMesh] != null)
                    {
                        int navmeshMeshesLength = Navmesh.NavmeshMeshes[Navmesh.Active][currentMesh].Length;
                        for(int k = 0; k < navmeshMeshesLength; k++)
                        {
                            for(int n = 0; n < nodesLength; n++)
                            {
                                if(Navmesh.NavmeshMeshes[Navmesh.Active][currentMesh][k] == nodes[n])
                                {
                                    matches++;
                                }
                            }
                        }
                        if(matches == Navmesh.MaxNumberOfNodesPerMesh)
                        {
                            return true;
                        }
                        matches = 0;
                    }
                }
                return false;
            } 
            else return false;
        }*/

        public static GameObject CreateMesh(Vector3[] meshNodes, int meshId, int type, float weight)
        {
            if(MeshUtil.IsMeshClockwise(meshNodes) == false)
            {
                MeshUtil.ToggleMeshClockwiseOrder(meshNodes);
            }
            GameObject meshGameObject = new GameObject("Triangle Mesh");
            meshGameObject.transform.parent = pathfindingTransform;
            meshGameObject.transform.position = MeshUtil.GetCenter(meshNodes);
            int meshNodesLength = meshNodes.Length;
            Vector3[] InversedMeshNodes = new Vector3[meshNodesLength];
            for(int i = 0; i < meshNodesLength; i++)
            {
                InversedMeshNodes[i] = meshGameObject.transform.InverseTransformPoint(meshNodes[i]);
            }
            Renderer meshRenderer = meshGameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(ThisNavmeshGenerator.meshMaterial);
            meshRenderer.material.SetFloat(Shader.PropertyToID("_IntensityVC"), 1.0f);
            MeshFilter triangleMeshFilter = meshGameObject.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            triangleMeshFilter.mesh = mesh;
            mesh.vertices = InversedMeshNodes;
            mesh.triangles = MeshUtil.GetTriangles(InversedMeshNodes);
            mesh.RecalculateBounds();
            int meshVerticesLength = mesh.vertices.Length;
            Color[] meshColors = new Color[meshVerticesLength];
            for(int i = 0; i < meshVerticesLength; i++)
            {
                meshColors[i] = Navmesh.NavmeshMeshesTypesColors[Navmesh.Active][type]; // It's the default color, from the Mesh Type 0
            }
            mesh.colors = meshColors;
            meshRenderer.enabled = false;
            meshRenderer.enabled = true;
            meshGameObject.AddComponent<MeshCollider>();
            Navmesh.NavmeshMeshes[Navmesh.Active][meshId] = new NavmeshMesh(Navmesh.Active, meshId, type, weight, meshGameObject);
            gameObjectToMeshPointers.Add(meshGameObject, meshId);
            return meshGameObject;
        }

        private void AddNavmesh()
        {
            Navmesh.Active = ++lastNavmesh;
        }

        private void SelectNode(NavmeshNode node, GameObject nodeGameObject)
        {
            if(selectedNodesCount == 0 || selectedNodesCount > 0 && selectedNodes[0].Navmesh == node.Navmesh)
            {
                nodeGameObject.GetComponent<Renderer>().material.color = selectedNodeColor;
                selectedNodes.Add(node);
                selectedNodesCount++;
                Debug.Log("You just selected the Node: " + node);
            }
            else
            {
                Debug.LogError("You can't select nodes from different navmeshes.");
            }
        }

        private void UnselectNode(NavmeshNode node, GameObject nodeGameObject)
        {
            nodeGameObject.GetComponent<Renderer>().material.color = unselectedNodeColor;
            selectedNodes.Remove(node);
            selectedNodesCount--;
        }

        private void UnselectAllNodes()
        {
            for(int i = 0; i < selectedNodesCount; i++)
            {
                selectedNodes[i].GameObject.GetComponent<Renderer>().material.color = unselectedNodeColor;
            }
            selectedNodes.Clear();
            selectedNodesCount = 0;
        }

        private void SelectMesh(int mesh, GameObject meshGameObject)
        {
            NavmeshMesh navmeshMesh = Navmesh.NavmeshMeshes[Navmesh.Active][mesh];
            NavmeshMesh selectedNavmeshMesh = Navmesh.NavmeshMeshes[Navmesh.Active][SelectedMeshes[0]];
            if(selectedMeshesCount == 0 || selectedMeshesCount > 0 && selectedNavmeshMesh.Navmesh == navmeshMesh.Navmesh)
            {
                meshGameObject.GetComponent<Renderer>().material.color = selectedNodeColor;
                SelectedMeshes.Add(mesh);
                selectedMeshesCount++;
                Debug.Log("You just selected the Mesh: " + mesh);
            }
            else
            {
                Debug.LogError("You can't select nodes from differents navmeshes.");
            }
        }

        private void UnselectMesh(int mesh, GameObject meshGameObject)
        {
            NavmeshMesh navmeshMesh = Navmesh.NavmeshMeshes[Navmesh.Active][mesh];
            meshGameObject.GetComponent<Renderer>().material.color = Navmesh.NavmeshMeshesTypesColors[Navmesh.Active][navmeshMesh.Type];
            SelectedMeshes.Remove(mesh);
            selectedMeshesCount--;
        }

        private void UnselectAllMeshes()
        {
            for(int i = 0; i < selectedMeshesCount; i++)
            {
                NavmeshMesh mesh = Navmesh.NavmeshMeshes[Navmesh.Active][SelectedMeshes[i]];
                mesh.GameObject.GetComponent<Renderer>().material.color = Navmesh.NavmeshMeshesTypesColors[Navmesh.Active][mesh.Type];
            }
            SelectedMeshes.Clear();
        }

        public static void AddLeftNode(Vector3 position, string name)
        {
            GameObject gameObject = Instantiate<GameObject>(LeftModel);
            gameObject.transform.position = new Vector3(position.x, position.y, position.z + 1.0f);
            gameObject.name = name;
        }

        public static void AddRightNode(Vector3 position)
        {
            Instantiate<GameObject>(RightModel).transform.position = position;
        }
    }
}