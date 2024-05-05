using Microsoft.Data.Sqlite;
using PracticaParentChildStructure.Models;
using System.Data;
using System.Drawing;

namespace PracticaParentChildStructure.MyDBContext
{
    public static class DatabaseService
    {
        private const string DbFilePath = "MyTestdataBase2.db";
        private const string ConnectionString = "Data Source=" + DbFilePath;

        static DatabaseService()
        {
            if (!File.Exists(DbFilePath))
            {
                CreateTables();
            }
        }
        #region Create Table 
        public static void CreateTables()
        {
            try
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Nodes (
                        NodeId INTEGER PRIMARY KEY AUTOINCREMENT,
                        NodeName TEXT,
                        ParentNodeId TEXT,
                        IsActive INTEGER,
                        StartDate TEXT
                    )";
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Add new Node And Update Node Using Same Function
        public static bool SaveValue(Node node)
        {
            try
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    if (node.ParentNodeId == null)
                    {
                        node.ParentNodeId = "#";
                    }
                    if (node.NodeId != 0)
                    {
                        command.CommandText = @"
                                UPDATE Nodes 
                                SET NodeName = @NodeName, 
                                    ParentNodeId = @ParentNodeId, 
                                    IsActive = @IsActive, 
                                    StartDate = @StartDate 
                                WHERE NodeId = @NodeId";
                        command.Parameters.AddWithValue("@NodeId", node.NodeId);
                        command.Parameters.AddWithValue("@NodeName", node.NodeName);
                        command.Parameters.AddWithValue("@ParentNodeId", node.ParentNodeId);
                        command.Parameters.AddWithValue("@IsActive", node.IsActive);
                        command.Parameters.AddWithValue("@StartDate", node.StartDate.ToString("yyyy-MM-dd HH:mm:ss"));

                        var data = command.ExecuteNonQuery();
                        connection.Close();

                    }
                    else
                    {
                        var commands = connection.CreateCommand();
                        commands.CommandText = "SELECT * FROM Nodes ORDER BY NodeId DESC LIMIT 1";
                        string value = "0";
                        using (var reader = commands.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                value = Convert.ToString(reader["NodeId"]);
                            }
                        }
                        connection.Close();
                        connection.Open();
                        node.NodeId = Convert.ToInt32(value);
                        node.NodeId = node.NodeId + 1;
                        command.CommandText = @"
                                INSERT INTO Nodes (NodeId, NodeName, ParentNodeId, IsActive, StartDate)
                                VALUES (@NodeId, @NodeName, @ParentNodeId, @IsActive, @StartDate)";
                        command.Parameters.AddWithValue("@NodeId", node.NodeId);
                        command.Parameters.AddWithValue("@NodeName", node.NodeName);
                        command.Parameters.AddWithValue("@ParentNodeId", node.ParentNodeId);
                        command.Parameters.AddWithValue("@IsActive", node.IsActive);
                        command.Parameters.AddWithValue("@StartDate", node.StartDate.ToString("yyyy-MM-dd HH:mm:ss"));

                        var data = command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion 

        #region Get All Node 

        public static List<Node> GetValues()
        {
            List<Node> nodes = new List<Node>();

            try
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM Nodes";

                    using (var reader = command.ExecuteReader())
                    {
                        Dictionary<string, Node> nodeDict = new Dictionary<string, Node>();
                        while (reader.Read())
                        {
                            Node node = new Node
                            {
                                NodeId = Convert.ToInt32(reader["NodeId"]),
                                NodeName = Convert.ToString(reader["NodeName"]),
                                ParentNodeId = Convert.ToString(reader["ParentNodeId"]),
                                IsActive = Convert.ToInt32(reader["IsActive"]),
                                StartDate = Convert.ToDateTime(reader["StartDate"]),
                                Children = new List<Node>()
                            };

                            nodeDict[node.NodeId.ToString()] = node;
                        }
                        foreach (var kvp in nodeDict)
                        {
                            Node node = kvp.Value;
                            if (!string.IsNullOrEmpty(node.ParentNodeId) && nodeDict.ContainsKey(node.ParentNodeId))
                            {
                                Node parentNode = nodeDict[node.ParentNodeId];
                                parentNode.Children.Add(node);
                            }
                            else
                            {
                                nodes.Add(node);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               
            }

            return nodes;
        }

        #endregion

        #region Delete Node Method
        public static bool DeleteNode(int NodeId)
        {
            try
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                     command.CommandText = "DELETE FROM Nodes WHERE NodeId = @NodeId";
                     command.Parameters.AddWithValue("@NodeId", NodeId);
                   // command.CommandText = "DELETE FROM Nodes";
                    var rowsAffected = command.ExecuteNonQuery();

                    connection.Close();
                    if (rowsAffected > 0)
                    {
                        return true; 
                    }
                    else
                    {
                        return false; 
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return false;
            }

        }
        #endregion

        #region Get All Node To Bind Dropdown

        public static List<Node> GetNodeToBind()
        {
            List<string> values = new List<string>();
            List<Node> nodes = new List<Node>();

            try
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM Nodes Where IsActive = 1";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.ReadAsync().Result)
                        {
                            Node node = new Node
                            {
                                NodeId = Convert.ToInt32(reader["NodeId"]),
                                NodeName = Convert.ToString(reader["NodeName"]),
                                ParentNodeId = Convert.ToString(reader["ParentNodeId"]),
                                IsActive = Convert.ToInt32(reader["IsActive"]),
                                StartDate = Convert.ToDateTime(reader["StartDate"])
                            };


                            nodes.Add(node);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return nodes;
        }

        #endregion


        #region Get All Active Node From Database Using Stored Procedure
        public static List<Node> GetActiveNodes()
        {
            List<Node> nodes = new List<Node>();

            //CREATE PROCEDURE GetActiveNodes
            //    AS
            //    BEGIN
            //    SELECT* FROM Nodes WHERE IsActive = 1;
            //    END;


            try
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "GetActiveNodes"; 

                    command.CommandType = CommandType.StoredProcedure; 

                    using (var reader = command.ExecuteReader())
                    {
                        Dictionary<string, Node> nodeDict = new Dictionary<string, Node>();
                        while (reader.Read())
                        {
                            Node node = new Node
                            {
                                NodeId = Convert.ToInt32(reader["NodeId"]),
                                NodeName = Convert.ToString(reader["NodeName"]),
                                ParentNodeId = Convert.ToString(reader["ParentNodeId"]),
                                IsActive = Convert.ToInt32(reader["IsActive"]),
                                StartDate = Convert.ToDateTime(reader["StartDate"]),
                                Children = new List<Node>()
                            };

                            nodeDict[node.NodeId.ToString()] = node;
                        }
                        foreach (var kvp in nodeDict)
                        {
                            Node node = kvp.Value;
                            if (!string.IsNullOrEmpty(node.ParentNodeId) && nodeDict.ContainsKey(node.ParentNodeId))
                            {
                                Node parentNode = nodeDict[node.ParentNodeId];
                                parentNode.Children.Add(node);
                            }
                            else
                            {
                                nodes.Add(node); 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               
            }

            return nodes;
        }
        #endregion
    }
}
