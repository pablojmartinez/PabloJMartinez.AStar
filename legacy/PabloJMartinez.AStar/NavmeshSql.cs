// Copyright (c) 2013-2017 Pablo J. Martínez. All rights reserved.
// Licensed under the MIT License.
// Part of the legacy PabloJMartinez.AStar implementation.

/*
using UnityEngine;
using System.Data;
using System.Text;
using SQLite;
namespace ComingLights
{
    public static class NavmeshSql
    {
        private static string createNavmeshMeshesFullTableQuery = @"CREATE TABLE IF NOT EXISTS NavmeshMeshesFull (Id int PRIMARY KEY,
                                                                                                                  MeshId int,
                                                                                                                  ElementId int,
                                                                                                                  ElementsLength int,
                                                                                                                  NodePositionX float,
                                                                                                                  NodePositionY float,
                                                                                                                  NodePositionZ float,
                                                                                                                  Type int,
                                                                                                                  Weight float)";
        public static void Create()
        {
            using(SqliteConnection connection = Sqlite.Connect(AlternateRealities.CurrentAlternateReality.DatabaseConnectionData))
            {
                connection.BeginTransaction();
                Create(connection);
                connection.Commit();
            }
        }

        public static void Create(SqliteConnection databaseConnection)
        {
            Sqlite.Query(databaseConnection, @"CREATE TABLE IF NOT EXISTS NavmeshData (Id int PRIMARY KEY,
                                                                               NavmeshId int,
                                                                               NodesLength int,
                                                                               NodesCount int,
                                                                               EdgesLength int,
                                                                               EdgesCount int,
                                                                               MeshesLength int,
                                                                               MeshesCount int)");

            Sqlite.Query(databaseConnection, @"CREATE TABLE IF NOT EXISTS Nodes (Id int PRIMARY KEY,
                                                                         NodeId int,
                                                                         NodePositionX float,
                                                                         NodePositionY float,
                                                                         NodePositionZ float)");

            Sqlite.Query(databaseConnection, @"CREATE TABLE IF NOT EXISTS NodesEdges (Id int PRIMARY KEY,
                                                                              NodeId int,
                                                                              ElementId int,
                                                                              EdgeId int)");

            Sqlite.Query(databaseConnection, @"CREATE TABLE IF NOT EXISTS NodesMeshes (Id int PRIMARY KEY,
                                                                               NodeId int,
                                                                               ElementId int,
                                                                               ElementsLength int,
                                                                               MeshId int)");

            Sqlite.Query(databaseConnection, @"CREATE TABLE IF NOT EXISTS Edges (Id int PRIMARY KEY,
                                                                         EdgeId int,
                                                                         EdgePositionX float,
                                                                         EdgePositionY float,
                                                                         EdgePositionZ float)");

            Sqlite.Query(databaseConnection, @"CREATE TABLE IF NOT EXISTS EdgesNodes (Id int PRIMARY KEY,
                                                                              EdgeId int,
                                                                              ElementId int,
                                                                              NodeId int)");

            Sqlite.Query(databaseConnection, @"CREATE TABLE IF NOT EXISTS EdgesMeshes (Id int PRIMARY KEY,
                                                                               EdgeId int,
                                                                               ElementId int,
                                                                               MeshId int)");

            Sqlite.Query(databaseConnection, @"CREATE TABLE IF NOT EXISTS Connections (Id int PRIMARY KEY,
                                                                               EdgeId int,
                                                                               ElementId int,
                                                                               ElementsLength int,
                                                                               Cost float,
                                                                               FromEdge int,
                                                                               ToPortalEdge int,
                                                                               ToPortalLeft int,
                                                                               ToPortalRight int,
                                                                               ToMesh int)");

            Sqlite.Query(databaseConnection, @"CREATE TABLE IF NOT EXISTS NavmeshMeshesTypesColors (Id int PRIMARY KEY,
                                                                                            TypeId int,
                                                                                            ColorR float,
                                                                                            ColorG float,
                                                                                            ColorB float)", null);

            Sqlite.Query(databaseConnection, @"CREATE TABLE IF NOT EXISTS NavmeshMeshesNodes (Id int PRIMARY KEY,
                                                                                      MeshId int,
                                                                                      ElementId int,
                                                                                      ElementsLength int,
                                                                                      NodeId int)", null);

            Sqlite.Query(databaseConnection, @"CREATE TABLE IF NOT EXISTS NavmeshMeshesPortals (Id int PRIMARY KEY,
                                                                                        MeshId int,
                                                                                        ElementId int,
                                                                                        ElementsLength int,
                                                                                        PortalEdge int,
                                                                                        PortalLeft int,
                                                                                        PortalRight int)", null);

            // This table includes variables from the NavmeshMeshes array too
            Sqlite.Query(databaseConnection, createNavmeshMeshesFullTableQuery, null);
        }

        public static void Drop()
        {
            using(SqliteConnection databaseConnection = Sqlite.Connect(AlternateRealities.CurrentAlternateReality.DatabaseConnectionData))
            {
                databaseConnection.BeginTransaction();
                Drop(databaseConnection);
                databaseConnection.Commit();
            }
        }

        public static void Drop(SqliteConnection databaseConnection)
        {
            Sqlite.Query(databaseConnection, "DROP TABLE IF EXISTS NavmeshData", null);
            Sqlite.Query(databaseConnection, "DROP TABLE IF EXISTS Nodes", null);
            Sqlite.Query(databaseConnection, "DROP TABLE IF EXISTS NodesEdges", null);
            Sqlite.Query(databaseConnection, "DROP TABLE IF EXISTS NodesMeshes", null);
            Sqlite.Query(databaseConnection, "DROP TABLE IF EXISTS Edges", null);
            Sqlite.Query(databaseConnection, "DROP TABLE IF EXISTS EdgesNodes", null);
            Sqlite.Query(databaseConnection, "DROP TABLE IF EXISTS EdgesMeshes", null);
            Sqlite.Query(databaseConnection, "DROP TABLE IF EXISTS Connections", null);
            Sqlite.Query(databaseConnection, "DROP TABLE IF EXISTS NavmeshMeshesTypesColors", null);
            Sqlite.Query(databaseConnection, "DROP TABLE IF EXISTS NavmeshMeshesNodes", null);
            Sqlite.Query(databaseConnection, "DROP TABLE IF EXISTS NavmeshMeshesPortals", null);
            Sqlite.Query(databaseConnection, "DROP TABLE IF EXISTS NavmeshMeshesFull", null);
        }

        public static void Update()
        {
            using(IDbConnection connection = Sqlite.Connect(AlternateRealities.CurrentAlternateReality.DatabaseConnectionData))
            using(SqliteTransaction sqliteTransaction = sqliteConnection.BeginTransaction())
            using(SqliteCommand sqliteCommand = sqliteConnection.CreateCommand())
            {
                Update(connection, command);
                transaction.Commit();
            }
        }

        public static void Update(SqliteConnection connection)
        {
            Drop(connection);
            Create(connection);

            for(int i = 0; i < Navmesh.NavmeshesCount; i++)
            {
                string insertQuery = null;

                // NAVMESH COUNTS
                insertQuery = @"INSERT INTO NavmeshData (Id,
                                                         NavmeshId,
                                                         NodesLength,
                                                         NodesCount,
                                                         EdgesLength,
                                                         EdgesCount,
                                                         MeshesLength,
                                                         MeshesCount)
                                                 VALUES (?,
                                                         ?,
                                                         ?,
                                                         ?,
                                                         ?,
                                                         ?,
                                                         ?,
                                                         ?)";
                command.AddWithValue<int>("Id", 0);
                command.AddWithValue<int>("NavmeshId", i);
                command.AddWithValue<int>("NodesLength", Navmesh.Nodes[i].Length);
                command.AddWithValue<int>("NodesCount", Navmesh.NodesCount);
                command.AddWithValue<int>("EdgesLength", Navmesh.Edges[i].Length);
                command.AddWithValue<int>("EdgesCount", Navmesh.EdgesCount);
                command.AddWithValue<int>("MeshesLength", Navmesh.NavmeshMeshesNodes[i].Length);
                command.AddWithValue<int>("MeshesCount", Navmesh.MeshesCount);
                Sqlite.Query(connection, insertQuery, command);
                command.Parameters.Clear();

                // NODES
                int nodesLength = Navmesh.Nodes[i].Length;
                for(int j = 0; j < nodesLength; j++)
                {
                    insertQuery = @"INSERT INTO Nodes (NodeId,
                                                       NodePositionX,
                                                       NodePositionY,
                                                       NodePositionZ)
                                               VALUES (?,
                                                       ?,
                                                       ?,
                                                       ?)";
                    command.AddWithValue<int>("NodeId", j);
                    command.AddWithValue<float>("NodePositionX", Navmesh.Nodes[i][j].x);
                    command.AddWithValue<float>("NodePositionY", Navmesh.Nodes[i][j].y);
                    command.AddWithValue<float>("NodePositionZ", Navmesh.Nodes[i][j].z);
                    Sqlite.Query(connection, insertQuery, command);
                    command.Parameters.Clear();

                    // NODES EDGES
                    int nodesEdgesLength = Navmesh.NodesEdges[i][j].Length;
                    for(int k = 0; k < nodesEdgesLength; k++)
                    {
                        insertQuery = @"INSERT INTO NodesEdges (NodeId,
                                                                ElementId,
                                                                EdgeId)
                                                        VALUES (?,
                                                                ?,
                                                                ?)";
                        command.AddWithValue<int>("NodeId", j);
                        command.AddWithValue<int>("ElementId", k);
                        command.AddWithValue<int>("EdgeId", Navmesh.NodesEdges[i][j][k]);
                        Sqlite.Query(connection, insertQuery, command);
                        command.Parameters.Clear();
                    }

                    // NODES MESHES
                    int nodesMeshesLength = Navmesh.NodesMeshes[i][j].Length;
                    for(int k = 0; k < nodesMeshesLength; k++)
                    {
                        insertQuery = @"INSERT INTO NodesMeshes (NodeId,
                                                                 ElementId,
                                                                 MeshId)
                                                         VALUES (?,
                                                                 ?,
                                                                 ?)";
                        command.AddWithValue<int>("NodeId", j);
                        command.AddWithValue<int>("ElementId", k);
                        command.AddWithValue<int>("MeshId", Navmesh.NodesMeshes[i][j][k]);
                        Sqlite.Query(connection, insertQuery, command);
                        command.Parameters.Clear();
                    }
                }

                // EDGES
                int edgesLength = Navmesh.Edges[i].Length;
                for(int j = 0; j < edgesLength; j++)
                {
                    insertQuery = @"INSERT INTO Edges (EdgeId,
                                                       EdgePositionX,
                                                       EdgePositionY,
                                                       EdgePositionZ)
                                               VALUES (?,
                                                       ?,
                                                       ?,
                                                       ?)";
                    command.AddWithValue<int>("EdgeId", j);
                    command.AddWithValue<float>("EdgePositionX", Navmesh.Edges[i][j].x);
                    command.AddWithValue<float>("EdgePositionY", Navmesh.Edges[i][j].y);
                    command.AddWithValue<float>("EdgePositionZ", Navmesh.Edges[i][j].z);
                    Sqlite.Query(connection, insertQuery, command);
                    command.Parameters.Clear();

                    // EDGES NODES
                    int edgesNodesLength = Navmesh.EdgesNodes[i][j].Length;
                    for(int k = 0; k < edgesNodesLength; k++)
                    {
                        insertQuery = @"INSERT INTO EdgesNodes (EdgeId,
                                                                ElementId,
                                                                NodeId)
                                                        VALUES (?,
                                                                ?,
                                                                ?)";
                        command.AddWithValue<int>("EdgeId", j);
                        command.AddWithValue<int>("ElementId", k);
                        command.AddWithValue<int>("NodeId", Navmesh.EdgesNodes[i][j][k]);
                        Sqlite.Query(connection, insertQuery, command);
                        command.Parameters.Clear();
                    }

                    // EDGES MESHES
                    int edgesMeshesLength = Navmesh.EdgesMeshes[i][j].Length;
                    for(int k = 0; k < edgesMeshesLength; k++)
                    {
                        insertQuery = @"INSERT INTO EdgesMeshes (EdgeId,
                                                                 ElementId,
                                                                 MeshId)
                                                         VALUES (?,
                                                                 ?,
                                                                 ?)";
                        command.AddWithValue<int>("EdgeId", j);
                        command.AddWithValue<int>("ElementId", k);
                        command.AddWithValue<int>("MeshId", Navmesh.EdgesMeshes[i][j][k]);
                        Sqlite.Query(connection, insertQuery, command);
                        command.Parameters.Clear();
                    }

                    // CONNECTIONS
                    int connectionsLength = Navmesh.Connections[i][j].Length;
                    for(int k = 0; k < connectionsLength; k++)
                    {
                        insertQuery = @"INSERT INTO Connections (EdgeId,
                                                                 ElementId,
                                                                 ElementsLength,
                                                                 Cost,
                                                                 FromEdge,
                                                                 ToPortalEdge,
                                                                 ToPortalLeft,
                                                                 ToPortalRight,
                                                                 ToMesh)
                                                         VALUES (?,
                                                                 ?,
                                                                 ?,
                                                                 ?,
                                                                 ?,
                                                                 ?,
                                                                 ?,
                                                                 ?,
                                                                 ?)";
                        command.AddWithValue<int>("EdgeId", j);
                        command.AddWithValue<int>("ElementId", k);
                        command.AddWithValue<int>("ElementsLength", connectionsLength);
                        command.AddWithValue<float>("Cost", Navmesh.Connections[i][j][k].Cost);
                        command.AddWithValue<int>("FromEdge", Navmesh.Connections[i][j][k].FromEdge);
                        command.AddWithValue<int>("ToPortalEdge", Navmesh.Connections[i][j][k].ToPortal.Edge);
                        command.AddWithValue<int>("ToPortalLeft", Navmesh.Connections[i][j][k].ToPortal.Left);
                        command.AddWithValue<int>("ToPortalRight", Navmesh.Connections[i][j][k].ToPortal.Right);
                        command.AddWithValue<int>("ToMesh", Navmesh.Connections[i][j][k].ToMesh);
                        Sqlite.Query(connection, insertQuery, command);
                        command.Parameters.Clear();
                    }
                }

                // NAVMESH MESHES TYPES COLORS
                int navmeshMeshesTypesColorsLength = Navmesh.NavmeshMeshesTypesColors[i].Length;
                for(int j = 0; j < navmeshMeshesTypesColorsLength; j++)
                {
                    if(Navmesh.NavmeshMeshesTypesColors[i][j] == null) continue;
                    insertQuery = @"INSERT INTO NavmeshMeshesTypesColors (TypeId,
                                                                          ColorR,
                                                                          ColorG,
                                                                          ColorB)
                                                                  VALUES (?,
                                                                          ?,
                                                                          ?,
                                                                          ?)";
                    command.AddWithValue<int>("TypeId", j);
                    command.AddWithValue<float>("ColorR", Navmesh.NavmeshMeshesTypesColors[i][j].r);
                    command.AddWithValue<float>("ColorG", Navmesh.NavmeshMeshesTypesColors[i][j].g);
                    command.AddWithValue<float>("ColorB", Navmesh.NavmeshMeshesTypesColors[i][j].b);
                    Sqlite.Query(connection, insertQuery, command);
                    command.Parameters.Clear();
                }





                // NAVMESH MESHES
                int meshesLength = Navmesh.NavmeshMeshesNodes[i].Length;
                for(int j = 0; j < meshesLength; j++)
                {
                    if(Navmesh.NavmeshMeshesNodes[i][j] == null) continue;
                    int meshLength = Navmesh.NavmeshMeshesNodes[i][j].Length;
                    for(int k = 0; k < meshLength; k++)
                    {
                        insertQuery = @"INSERT INTO NavmeshMeshesNodes (MeshId,
                                                                        ElementId,
                                                                        ElementsLength,
                                                                        NodeId)
                                                                VALUES (?,
                                                                        ?,
                                                                        ?,
                                                                        ?)";
                        command.AddWithValue<int>("MeshId", j);
                        command.AddWithValue<int>("ElementId", k);
                        command.AddWithValue<int>("ElementsLength", meshLength);
                        command.AddWithValue<int>("NodeId", Navmesh.NavmeshMeshesNodes[i][j][k]);
                        Sqlite.Query(connection, insertQuery, command);
                        command.Parameters.Clear();

                        // NAVMESH MESHES PORTALS
                        insertQuery = @"INSERT INTO NavmeshMeshesPortals (MeshId,
                                                                          ElementId,
                                                                          ElementsLength,
                                                                          PortalEdge,
                                                                          PortalLeft,
                                                                          PortalRight)
                                                                  VALUES (?,
                                                                          ?,
                                                                          ?,
                                                                          ?,
                                                                          ?,
                                                                          ?)";
                        command.AddWithValue<int>("MeshId", j);
                        command.AddWithValue<int>("ElementId", k);
                        command.AddWithValue<int>("ElementsLength", meshLength);
                        command.AddWithValue<int>("PortalEdge", Navmesh.NavmeshMeshesPortals[i][j][k].Edge);
                        command.AddWithValue<int>("PortalLeft", Navmesh.NavmeshMeshesPortals[i][j][k].Left);
                        command.AddWithValue<int>("PortalRight", Navmesh.NavmeshMeshesPortals[i][j][k].Right);
                        Sqlite.Query(connection, insertQuery, command);
                        command.Parameters.Clear();

                        // NAVMESH MESHES FULL
                        insertQuery = @"INSERT INTO NavmeshMeshesFull (MeshId,
                                                                       ElementId,
                                                                       ElementsLength,
                                                                       NodePositionX,
                                                                       NodePositionY,
                                                                       NodePositionZ,
                                                                       Type,
                                                                       Weight)
                                                               VALUES (?,
                                                                       ?,
                                                                       ?,
                                                                       ?,
                                                                       ?,
                                                                       ?,
                                                                       ?,
                                                                       ?)";
                        command.AddWithValue<int>("MeshId", j);
                        command.AddWithValue<int>("ElementId", k);
                        command.AddWithValue<int>("ElementsLength", meshLength);
                        command.AddWithValue<float>("NodePositionX", Navmesh.NavmeshMeshesFull[i][j][k].x);
                        command.AddWithValue<float>("NodePositionY", Navmesh.NavmeshMeshesFull[i][j][k].y);
                        command.AddWithValue<float>("NodePositionZ", Navmesh.NavmeshMeshesFull[i][j][k].z);
                        command.AddWithValue<int>("Type", Navmesh.NavmeshMeshes[i][j].Type);
                        command.AddWithValue<float>("Weight", Navmesh.NavmeshMeshes[i][j].Weight);
                        Sqlite.Query(connection, insertQuery, command);
                        command.Parameters.Clear();
                    }
                }
            }

            /*for(int i = 0; i < Navmesh.NavmeshesCount; i++)
            {
                string insertQuery = null;
                int numberOfRowsAffected = 0;

                // NAVMESH COUNTS
                insertQuery = @"UPDATE NavmeshCounts
                                           SET NodesCount = ?,
                                               EdgesCount = ?,
                                               MeshesCount = ?
                                           WHERE Id = ?";
                command.AddWithValue<int>("NodesCount", Navmesh.NodesCount);
                command.AddWithValue<int>("EdgesCount", Navmesh.EdgesCount);
                command.AddWithValue<int>("MeshesCount", Navmesh.MeshesCount);
                command.AddWithValue<int>("Id", 0);
                numberOfRowsAffected = Sqlite.Query(connection, insertQuery, command);
                command.Parameters.Clear();
                if(numberOfRowsAffected <= 0)
                {
                    insertQuery = @"INSERT INTO NavmeshCounts (Id,
                                                               NodesCount,
                                                               EdgesCount,
                                                               MeshesCount)
                                                       VALUES (?,
                                                               ?,
                                                               ?,
                                                               ?)";
                    command.AddWithValue<int>("Id", 0);
                    command.AddWithValue<int>("NodesCount", Navmesh.NodesCount);
                    command.AddWithValue<int>("EdgesCount", Navmesh.EdgesCount);
                    command.AddWithValue<int>("MeshesCount", Navmesh.MeshesCount);
                    Sqlite.Query(connection, insertQuery, command);
                    command.Parameters.Clear();
                }

                // NODES
                for(int j = 0; j < Navmesh.NodesCount; j++)
                {
                    insertQuery = @"UPDATE Nodes
                                           SET NodePositionX = ?,
                                               NodePositionY = ?,
                                               NodePositionZ = ?
                                           WHERE NodeId = ?";
                    command.AddWithValue<float>("NodePositionX", Navmesh.Nodes[i][j].x);
                    command.AddWithValue<float>("NodePositionY", Navmesh.Nodes[i][j].y);
                    command.AddWithValue<float>("NodePositionZ", Navmesh.Nodes[i][j].z);
                    command.AddWithValue<int>("NodeId", j);
                    numberOfRowsAffected = Sqlite.Query(connection, insertQuery, command);
                    command.Parameters.Clear();
                    if(numberOfRowsAffected <= 0)
                    {
                        insertQuery = @"INSERT INTO Nodes (NodeId,
                                                           NavmeshId,
                                                           NodePositionX,
                                                           NodePositionY,
                                                           NodePositionZ)
                                                   VALUES (?,
                                                           ?,
                                                           ?,
                                                           ?,
                                                           ?)";
                        command.AddWithValue<int>("NodeId", j);
                        command.AddWithValue<int>("NavmeshId", i);
                        command.AddWithValue<float>("NodePositionX", Navmesh.Nodes[i][j].x);
                        command.AddWithValue<float>("NodePositionY", Navmesh.Nodes[i][j].y);
                        command.AddWithValue<float>("NodePositionZ", Navmesh.Nodes[i][j].z);
                        Sqlite.Query(connection, insertQuery, command);
                        command.Parameters.Clear();
                    }

                    // NODES EDGES
                    int nodesEdgesLength = Navmesh.NodesEdges[i][j].Length;
                    for(int k = 0; k < nodesEdgesLength; k++)
                    {
                        insertQuery = @"UPDATE NodesEdges
                                           SET EdgeId = ?
                                           WHERE NodeId = ?";
                        command.AddWithValue<int>("EdgeId", Navmesh.NodesEdges[i][j][k]);
                        command.AddWithValue<int>("NodeId", j);
                        numberOfRowsAffected = Sqlite.Query(connection, insertQuery, command);
                        command.Parameters.Clear();
                        if(numberOfRowsAffected <= 0)
                        {
                            insertQuery = @"INSERT INTO NodesEdges (NavmeshId,
                                                                    NodeId,
                                                                    EdgeId)
                                                            VALUES (?,
                                                                    ?,
                                                                    ?)";
                            command.AddWithValue<int>("NavmeshId", i);
                            command.AddWithValue<int>("NodeId", j);
                            command.AddWithValue<int>("EdgeId", Navmesh.NodesEdges[i][j][k]);
                            Sqlite.Query(connection, insertQuery, command);
                            command.Parameters.Clear();
                        }
                    }

                    // NODES MESHES
                    int nodesMeshesLength = Navmesh.NodesMeshes[i][j].Length;
                    for(int k = 0; k < nodesMeshesLength; k++)
                    {
                        insertQuery = @"UPDATE NodesMeshes
                                           SET MeshId = ?
                                           WHERE NodeId = ?";
                        command.AddWithValue<int>("MeshId", Navmesh.NodesMeshes[i][j][k]);
                        command.AddWithValue<int>("NodeId", j);
                        numberOfRowsAffected = Sqlite.Query(connection, insertQuery, command);
                        command.Parameters.Clear();
                        if(numberOfRowsAffected <= 0)
                        {
                            insertQuery = @"INSERT INTO NodesMeshes (NavmeshId,
                                                                     NodeId,
                                                                     MeshId)
                                                             VALUES (?,
                                                                     ?,
                                                                     ?)";
                            command.AddWithValue<int>("NavmeshId", i);
                            command.AddWithValue<int>("NodeId", j);
                            command.AddWithValue<int>("MeshId", Navmesh.NodesMeshes[i][j][k]);
                            Sqlite.Query(connection, insertQuery, command);
                            command.Parameters.Clear();
                        }
                    }
                }

                // EDGES
                for(int j = 0; j < Navmesh.EdgesCount; j++)
                {
                    insertQuery = @"UPDATE Edges
                                           SET EdgePositionX = ?,
                                               EdgePositionY = ?,
                                               EdgePositionZ = ?
                                           WHERE EdgeId = ?";
                    command.AddWithValue<float>("EdgePositionX", Navmesh.Edges[i][j].x);
                    command.AddWithValue<float>("EdgePositionY", Navmesh.Edges[i][j].y);
                    command.AddWithValue<float>("EdgePositionZ", Navmesh.Edges[i][j].z);
                    command.AddWithValue<int>("EdgeId", j);
                    numberOfRowsAffected = Sqlite.Query(connection, insertQuery, command);
                    command.Parameters.Clear();
                    if(numberOfRowsAffected <= 0)
                    {
                        insertQuery = @"INSERT INTO Edges (EdgeId,
                                                           NavmeshId,
                                                           EdgePositionX,
                                                           EdgePositionY,
                                                           EdgePositionZ)
                                                   VALUES (?,
                                                           ?,
                                                           ?,
                                                           ?,
                                                           ?)";
                        command.AddWithValue<int>("EdgeId", j);
                        command.AddWithValue<int>("NavmeshId", i);
                        command.AddWithValue<float>("EdgePositionX", Navmesh.Edges[i][j].x);
                        command.AddWithValue<float>("EdgePositionY", Navmesh.Edges[i][j].y);
                        command.AddWithValue<float>("EdgePositionZ", Navmesh.Edges[i][j].z);
                        Sqlite.Query(connection, insertQuery, command);
                        command.Parameters.Clear();
                    }

                    // EDGES NODES
                    int nodesEdgesLength = Navmesh.EdgesNodes[i][j].Length;
                    for(int k = 0; k < nodesEdgesLength; k++)
                    {
                        insertQuery = @"UPDATE EdgesNodes
                                           SET NodeId = ?
                                           WHERE EdgeId = ?";
                        command.AddWithValue<int>("NodeId", Navmesh.EdgesNodes[i][j][k]);
                        command.AddWithValue<int>("EdgeId", j);
                        numberOfRowsAffected = Sqlite.Query(connection, insertQuery, command);
                        command.Parameters.Clear();
                        if(numberOfRowsAffected <= 0)
                        {
                            insertQuery = @"INSERT INTO EdgesNodes (NavmeshId,
                                                                    EdgeId,
                                                                    NodeId)
                                                            VALUES (?,
                                                                    ?,
                                                                    ?)";
                            command.AddWithValue<int>("NavmeshId", i);
                            command.AddWithValue<int>("EdgeId", j);
                            command.AddWithValue<int>("NodeId", Navmesh.EdgesNodes[i][j][k]);
                            Sqlite.Query(connection, insertQuery, command);
                            command.Parameters.Clear();
                        }
                    }

                    // EDGES MESHES
                    int nodesMeshesLength = Navmesh.EdgesMeshes[i][j].Length;
                    for(int k = 0; k < nodesMeshesLength; k++)
                    {
                        insertQuery = @"UPDATE EdgesMeshes
                                           SET MeshId = ?
                                           WHERE EdgeId = ?";
                        command.AddWithValue<int>("MeshId", Navmesh.EdgesMeshes[i][j][k]);
                        command.AddWithValue<int>("EdgeId", j);
                        numberOfRowsAffected = Sqlite.Query(connection, insertQuery, command);
                        command.Parameters.Clear();
                        if(numberOfRowsAffected <= 0)
                        {
                            insertQuery = @"INSERT INTO EdgesMeshes (NavmeshId,
                                                                     EdgeId,
                                                                     MeshId)
                                                             VALUES (?,
                                                                     ?,
                                                                     ?)";
                            command.AddWithValue<int>("NavmeshId", i);
                            command.AddWithValue<int>("EdgeId", j);
                            command.AddWithValue<int>("MeshId", Navmesh.EdgesMeshes[i][j][k]);
                            Sqlite.Query(connection, insertQuery, command);
                            command.Parameters.Clear();
                        }
                    }

                    // CONNECTIONS
                    int edgeConnectionsLength = Navmesh.Connections[i][j].Length;
                    for(int k = 0; k < edgeConnectionsLength; k++)
                    {
                        insertQuery = @"UPDATE Connections
                                           SET Cost = ?,
                                               FromEdge = ?,
                                               ToPortalEdge = ?,
                                               ToPortalLeft = ?,
                                               ToPortalRight = ?,
                                               ToMesh = ?
                                           WHERE EdgeId = ?";
                        command.AddWithValue<float>("Cost", Navmesh.Connections[i][j][k].Cost);
                        command.AddWithValue<int>("FromEdge", Navmesh.Connections[i][j][k].FromEdge);
                        command.AddWithValue<int>("ToPortalEdge", Navmesh.Connections[i][j][k].ToPortal.Edge);
                        command.AddWithValue<int>("ToPortalLeft", Navmesh.Connections[i][j][k].ToPortal.Left);
                        command.AddWithValue<int>("ToPortalRight", Navmesh.Connections[i][j][k].ToPortal.Right);
                        command.AddWithValue<int>("ToMesh", Navmesh.Connections[i][j][k].ToMesh);
                        command.AddWithValue<int>("EdgeId", j);
                        numberOfRowsAffected = Sqlite.Query(connection, insertQuery, command);
                        command.Parameters.Clear();
                        if(numberOfRowsAffected <= 0)
                        {
                            insertQuery = @"INSERT INTO Connections (NavmeshId,
                                                                     EdgeId,
                                                                     Cost,
                                                                     FromEdge,
                                                                     ToPortalEdge,
                                                                     ToPortalLeft,
                                                                     ToPortalRight,
                                                                     ToMesh)
                                                             VALUES (?,
                                                                     ?,
                                                                     ?,
                                                                     ?,
                                                                     ?,
                                                                     ?,
                                                                     ?,
                                                                     ?)";
                            command.AddWithValue<int>("NavmeshId", i);
                            command.AddWithValue<int>("EdgeId", j);
                            command.AddWithValue<float>("Cost", Navmesh.Connections[i][j][k].Cost);
                            command.AddWithValue<int>("FromEdge", Navmesh.Connections[i][j][k].FromEdge);
                            command.AddWithValue<int>("ToPortalEdge", Navmesh.Connections[i][j][k].ToPortal.Edge);
                            command.AddWithValue<int>("ToPortalLeft", Navmesh.Connections[i][j][k].ToPortal.Left);
                            command.AddWithValue<int>("ToPortalRight", Navmesh.Connections[i][j][k].ToPortal.Right);
                            command.AddWithValue<int>("ToMesh", Navmesh.Connections[i][j][k].ToMesh);
                            Sqlite.Query(connection, insertQuery, command);
                            command.Parameters.Clear();
                        }
                    }
                }

                // NAVMESH MESHES
                for(int j = 0; j < Navmesh.MeshesCount; j++)
                {

                    int navmeshMeshesLength = Navmesh.NavmeshMeshes[i][j].Length;
                    for(int k = 0; k < navmeshMeshesLength; k++)
                    {
                        Sqlite.Query(connection, @"CREATE TABLE IF NOT EXISTS NavmeshMeshes (Id int PRIMARY KEY,
                                                                                 NavmeshId int,
                                                                                 MeshId int
                                                                                 NodeId int)", command);

                        Sqlite.Query(connection, @"CREATE TABLE IF NOT EXISTS NavmeshMeshesPortals (Id int PRIMARY KEY,
                                                                                        NavmeshId int,
                                                                                        MeshId int,
                                                                                        PortalEdge int,
                                                                                        PortalLeft int,
                                                                                        PortalRight int)", command);

                        Sqlite.Query(connection, @"CREATE TABLE IF NOT EXISTS NavmeshMeshesFull (Id int PRIMARY KEY,
                                                                                     NavmeshId int,
                                                                                     MeshId int,
                                                                                     NodePositionX float,
                                                                                     NodePositionY float,
                                                                                     NodePositionZ float)", command);

                        insertQuery = @"UPDATE NavmeshMeshes
                                           SET NodeId = ?
                                           WHERE NavmeshId = ?
                                             AND MeshId = ?
                                             AND ElementId = ?";
                        command.AddWithValue<int>("NodeId", Navmesh.NavmeshMeshes[i][j][k]);
                        command.AddWithValue<int>("MeshId", j);
                        numberOfRowsAffected = Sqlite.Query(connection, insertQuery, command);
                        command.Parameters.Clear();
                        if(numberOfRowsAffected <= 0)
                        {
                            insertQuery = @"INSERT INTO Edges (EdgeId,
                                                           NavmeshId,
                                                           EdgePositionX,
                                                           EdgePositionY,
                                                           EdgePositionZ)
                                                   VALUES (?,
                                                           ?,
                                                           ?,
                                                           ?,
                                                           ?)";
                            command.AddWithValue<int>("EdgeId", j);
                            command.AddWithValue<int>("NavmeshId", i);
                            command.AddWithValue<float>("EdgePositionX", Navmesh.Edges[i][j].x);
                            command.AddWithValue<float>("EdgePositionY", Navmesh.Edges[i][j].y);
                            command.AddWithValue<float>("EdgePositionZ", Navmesh.Edges[i][j].z);
                            Sqlite.Query(connection, insertQuery, command);
                            command.Parameters.Clear();
                        }
                    }
                }
            }*//*
        }

        public static void Load()
        {
            using(IDbConnection connection = Sqlite.Connect(AlternateRealities.CurrentAlternateReality.DatabaseConnectionData))
            using(SqliteTransaction sqliteTransaction = sqliteConnection.BeginTransaction())
            using(SqliteCommand sqliteCommand = sqliteConnection.CreateCommand())
            {
                Load(connection, command);
                transaction.Commit();
            }
        }

        public static void Load(IDbConnection connection, IDbCommand command)
        {
            //Drop(connection, command);
            Create(connection, command);
            int navmeshId = -1;
            int nodesLength = 0;
            int edgesLength = 0;
            int meshesLength = 0;
            Navmesh.NodesCount = 0;
            Navmesh.EdgesCount = 0;
            Navmesh.MeshesCount = 0;

            using(IDataReader dataReader = Sqlite.ReadQuery(connection, @"SELECT NavmeshId,
                                                                                 NodesLength,
                                                                                 NodesCount,
                                                                                 EdgesLength,
                                                                                 EdgesCount,
                                                                                 MeshesLength,
                                                                                 MeshesCount
                                                                            FROM NavmeshData
                                                                            WHERE Id = 0", command))
            {
                /*if(dataReader == null)
                {
                    Create(connection, command);
                    Load(connection, command);
                }
                else
                {*//*
                while(dataReader.Read())
                {
                    int column = 0;
                    navmeshId = dataReader.GetInt32(column++);
                    nodesLength = dataReader.GetInt32(column++);
                    Navmesh.NodesCount = dataReader.GetInt32(column++);
                    edgesLength = dataReader.GetInt32(column++);
                    Navmesh.EdgesCount = dataReader.GetInt32(column++);
                    meshesLength = dataReader.GetInt32(column++);
                    Navmesh.MeshesCount = dataReader.GetInt32(column++);
                }
            }

            if(navmeshId != -1)
            {
                if(nodesLength > 0)
                {
                    Navmesh.InitialNumberOfNodes = nodesLength;
                }
                if(edgesLength > 0)
                {
                    Navmesh.InitialNumberOfEdges = edgesLength;
                }
                if(meshesLength > 0)
                {
                    Navmesh.InitialNumberOfMeshes = meshesLength;
                }
                Navmesh.Initialize();

                // NODES
                using(IDataReader dataReader = Sqlite.ReadQuery(connection, @"SELECT NodeId,
                                                                                     NodePositionX,
                                                                                     NodePositionY,
                                                                                     NodePositionZ
                                                                                FROM Nodes", command))
                {
                    while(dataReader.Read())
                    {
                        int column = 0;
                        int nodeId = dataReader.GetInt32(column++);
                        float nodePositionX = dataReader.GetFloat(column++);
                        float nodePositionY = dataReader.GetFloat(column++);
                        float nodePositionZ = dataReader.GetFloat(column++);
                        Navmesh.Nodes[navmeshId][nodeId] = new Vector3(nodePositionX, nodePositionY, nodePositionZ);
                        NavmeshGenerator.AddNode(Navmesh.Nodes[navmeshId][nodeId], nodeId);
                    }
                }

                using(IDataReader dataReader = Sqlite.ReadQuery(connection, @"SELECT NodeId,
                                                                                     ElementId,
                                                                                     EdgeId
                                                                                FROM NodesEdges", command))
                {
                    while(dataReader.Read())
                    {
                        int column = 0;
                        int nodeId = dataReader.GetInt32(column++);
                        int elementId = dataReader.GetInt32(column++);
                        int edgeId = dataReader.GetInt32(column++);
                        Navmesh.NodesEdges[navmeshId][nodeId][elementId] = edgeId;
                    }
                }

                using(IDataReader dataReader = Sqlite.ReadQuery(connection, @"SELECT NodeId,
                                                                                     ElementId,
                                                                                     MeshId
                                                                                FROM NodesMeshes", command))
                {
                    while(dataReader.Read())
                    {
                        int column = 0;
                        int nodeId = dataReader.GetInt32(column++);
                        int elementId = dataReader.GetInt32(column++);
                        int meshId = dataReader.GetInt32(column++);
                        Navmesh.NodesMeshes[navmeshId][nodeId][elementId] = meshId;
                    }
                }

                // EDGES
                using(IDataReader dataReader = Sqlite.ReadQuery(connection, @"SELECT EdgeId,
                                                                                     EdgePositionX,
                                                                                     EdgePositionY,
                                                                                     EdgePositionZ
                                                                                FROM Edges", command))
                {
                    while(dataReader.Read())
                    {
                        int column = 0;
                        int edgeId = dataReader.GetInt32(column++);
                        float edgePositionX = dataReader.GetFloat(column++);
                        float edgePositionY = dataReader.GetFloat(column++);
                        float edgePositionZ = dataReader.GetFloat(column++);
                        Navmesh.Edges[navmeshId][edgeId] = new Vector3(edgePositionX, edgePositionY, edgePositionZ);
                    }
                }

                using(IDataReader dataReader = Sqlite.ReadQuery(connection, @"SELECT EdgeId,
                                                                                     ElementId,
                                                                                     NodeId
                                                                                FROM EdgesNodes", command))
                {
                    while(dataReader.Read())
                    {
                        int column = 0;
                        int edgeId = dataReader.GetInt32(column++);
                        int elementId = dataReader.GetInt32(column++);
                        int nodeId = dataReader.GetInt32(column++);
                        Navmesh.EdgesNodes[navmeshId][edgeId][elementId] = nodeId;
                    }
                }

                using(IDataReader dataReader = Sqlite.ReadQuery(connection, @"SELECT EdgeId,
                                                                                     ElementId,
                                                                                     MeshId
                                                                                FROM EdgesMeshes", command))
                {
                    while(dataReader.Read())
                    {
                        int column = 0;
                        int edgeId = dataReader.GetInt32(column++);
                        int elementId = dataReader.GetInt32(column++);
                        int meshId = dataReader.GetInt32(column++);
                        Navmesh.EdgesMeshes[navmeshId][edgeId][elementId] = meshId;
                    }
                }

                using(IDataReader dataReader = Sqlite.ReadQuery(connection, @"SELECT EdgeId,
                                                                                     ElementId,
                                                                                     ElementsLength,
                                                                                     Cost,
                                                                                     FromEdge,
                                                                                     ToPortalEdge,
                                                                                     ToPortalLeft,
                                                                                     ToPortalRight,
                                                                                     ToMesh
                                                                                FROM Connections", command))
                {
                    while(dataReader.Read())
                    {
                        int column = 0;
                        int edgeId = dataReader.GetInt32(column++);
                        int elementId = dataReader.GetInt32(column++);
                        int elementsLength = dataReader.GetInt32(column++);
                        float cost = dataReader.GetFloat(column++);
                        int fromEdge = dataReader.GetInt32(column++);
                        int toPortalEdge = dataReader.GetInt32(column++);
                        int toPortalLeft = dataReader.GetInt32(column++);
                        int toPortalRight = dataReader.GetInt32(column++);
                        int toMesh = dataReader.GetInt32(column++);
                        if(elementId == 0)
                        {
                            Navmesh.Connections[navmeshId][edgeId] = new NodeConnection[elementsLength];
                        }
                        Navmesh.Connections[navmeshId][edgeId][elementId] = new NodeConnection(cost, fromEdge, new Portal(toPortalEdge, toPortalLeft, toPortalRight), toMesh);
                    }
                }

                using(IDataReader dataReader = Sqlite.ReadQuery(connection, @"SELECT TypeId,
                                                                                     ColorR,
                                                                                     ColorG,
                                                                                     ColorB
                                                                                FROM NavmeshMeshesTypesColors", command))
                {
                    while(dataReader.Read())
                    {
                        int column = 0;
                        int typeId = dataReader.GetInt32(column++);
                        float colorR = dataReader.GetFloat(column++);
                        float colorG = dataReader.GetFloat(column++);
                        float colorB = dataReader.GetFloat(column++);
                        Navmesh.NavmeshMeshesTypesColors[navmeshId][typeId] = new Color(colorR, colorG, colorB);
                    }
                }

                /*using(SqliteTransaction sqliteTransaction = sqliteConnection.BeginTransaction())
                {
                    //Sqlite.Query(connection, "ALTER TABLE NavmeshMeshes RENAME TO NavmeshMeshesNodes", command);
                    transaction.Commit();
                }*//*
                //Sqlite.RecreateTableWithNewColumns(connection, command, "NavmeshMeshesFull", createNavmeshMeshesFullTableQuery);

                using(IDataReader dataReader = Sqlite.ReadQuery(connection, @"SELECT MeshId,
                                                                                     ElementId,
                                                                                     ElementsLength,
                                                                                     NodeId
                                                                                FROM NavmeshMeshesNodes", command))
                {
                    while(dataReader.Read())
                    {
                        int column = 0;
                        int meshId = dataReader.GetInt32(column++);
                        int elementId = dataReader.GetInt32(column++);
                        int elementsLength = dataReader.GetInt32(column++);
                        int nodeId = dataReader.GetInt32(column++);
                        if(elementId == 0)
                        {
                            Navmesh.NavmeshMeshesNodes[navmeshId][meshId] = new int[elementsLength];
                        }
                        Navmesh.NavmeshMeshesNodes[navmeshId][meshId][elementId] = nodeId;
                    }
                }

                using(IDataReader dataReader = Sqlite.ReadQuery(connection, @"SELECT MeshId,
                                                                                     ElementId,
                                                                                     ElementsLength,
                                                                                     PortalEdge,
                                                                                     PortalLeft,
                                                                                     PortalRight
                                                                                FROM NavmeshMeshesPortals", command))
                {
                    while(dataReader.Read())
                    {
                        int column = 0;
                        int meshId = dataReader.GetInt32(column++);
                        int elementId = dataReader.GetInt32(column++);
                        int elementsLength = dataReader.GetInt32(column++);
                        int portalEdge = dataReader.GetInt32(column++);
                        int portalLeft = dataReader.GetInt32(column++);
                        int portalRight = dataReader.GetInt32(column++);
                        if(elementId == 0)
                        {
                            Navmesh.NavmeshMeshesPortals[navmeshId][meshId] = new Portal[elementsLength];
                        }
                        Navmesh.NavmeshMeshesPortals[navmeshId][meshId][elementId] = new Portal(portalEdge, portalLeft, portalRight);
                    }
                }

                using(IDataReader dataReader = Sqlite.ReadQuery(connection, @"SELECT MeshId,
                                                                                     ElementId,
                                                                                     ElementsLength,
                                                                                     NodePositionX,
                                                                                     NodePositionY,
                                                                                     NodePositionZ,
                                                                                     Type,
                                                                                     Weight
                                                                                FROM NavmeshMeshesFull", command))
                {
                    while(dataReader.Read())
                    {
                        int column = 0;
                        int meshId = dataReader.GetInt32(column++);
                        int elementId = dataReader.GetInt32(column++);
                        int elementsLength = dataReader.GetInt32(column++);
                        float nodePositionX = dataReader.GetFloat(column++);
                        float nodePositionY = dataReader.GetFloat(column++);
                        float nodePositionZ = dataReader.GetFloat(column++);
                        int type = dataReader.GetInt32(column++);
                        float weight = dataReader.GetFloat(column++);
                        if(elementId == 0)
                        {
                            Navmesh.NavmeshMeshesFull[navmeshId][meshId] = new Vector3[elementsLength];
                        }
                        Navmesh.NavmeshMeshesFull[navmeshId][meshId][elementId] = new Vector3(nodePositionX, nodePositionY, nodePositionZ);
                        if(elementId == elementsLength - 1)
                        {
                            NavmeshGenerator.CreateMesh(Navmesh.NavmeshMeshesFull[navmeshId][meshId], meshId, type, weight);
                        }
                    }
                }
            }
        }
    }
}*/