using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace SkyrimMasterServer
{
    public class IDatabase
    {
        private MySqlConnectionStringBuilder _builder;

        public IDatabase(string IpAddress, string Port, string Username, string Password, string DbName)
        {
            _builder = new MySqlConnectionStringBuilder
            {
                ConnectionTimeout = 10,
                Database = DbName,
                DefaultCommandTimeout = 30,
                Logging = false,
                MaximumPoolSize = 5,
                MinimumPoolSize = 1,
                Password = Password,
                Pooling = true,
                Port = uint.Parse(Port),
                Server = IpAddress,
                UserID = Username,
                AllowZeroDateTime = true,
                ConvertZeroDateTime = true,
            };
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_builder.ToString());
        }

        public bool RunPreparedStatement(string Query, Dictionary<string, object> Parameters)
        {
            using (MySqlConnection Connection = GetConnection())
            {
                try
                {
                    Connection.Open();
                    MySqlCommand Command = Connection.CreateCommand();
                    Command.CommandText = Query;

                    foreach (KeyValuePair<string, object> Params in Parameters)
                    {
                        Command.Parameters.AddWithValue("@" + Params.Key, Params.Value);
                    }

                    Command.ExecuteNonQuery();
                    Command.Dispose();
                    Connection.Close();
                    Connection.Dispose();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("[IDatabase] Error running query: " + Query);
                    Console.WriteLine(e.ToString());
                    return false;
                }
            }
        }

        public Dictionary<int, Dictionary<string, object>> RunPreparedStatementWithResult(string Query, Dictionary<string, object> Parameters)
        {
            Dictionary<int, Dictionary<string, object>> Results = new Dictionary<int, Dictionary<string, object>>();

            using (MySqlConnection Connection = GetConnection())
            {
                try
                {
                    Connection.Open();
                    MySqlCommand Command = Connection.CreateCommand();
                    Command.CommandText = Query;

                    foreach (KeyValuePair<string, object> Params in Parameters)
                    {
                        Command.Parameters.AddWithValue("@" + Params.Key, Params.Value);
                    }

                    MySqlDataReader Reader = Command.ExecuteReader();

                    int Row = 0;
                    while (Reader.Read())
                    {
                        Dictionary<string, object> Data = new Dictionary<string, object>();

                        for (int i = 0; i < Reader.FieldCount; i++)
                        {
                            Data.Add(Reader.GetName(i), Reader.GetValue(i));
                        }

                        Results.Add(Row, Data);
                        Row++;
                    }

                    Reader.Dispose();
                    Command.Dispose();
                    Connection.Close();
                    Connection.Dispose();
                    return Results;
                }
                catch (Exception e)
                {
                    Console.WriteLine("[IDatabase] Error running query: " + Query);
                    Console.WriteLine(e.ToString());
                    return Results;
                }
            }
        }

        public Dictionary<int, Dictionary<string, object>> RunQuickWithResult(string Query)
        {
            Dictionary<int, Dictionary<string, object>> Results = new Dictionary<int, Dictionary<string, object>>();

            using (MySqlConnection Connection = GetConnection())
            {
                try
                {
                    Connection.Open();
                    MySqlCommand Command = Connection.CreateCommand();
                    Command.CommandText = Query;
                    MySqlDataReader Reader = Command.ExecuteReader();

                    int Row = 0;
                    while (Reader.Read())
                    {
                        Dictionary<string, object> Data = new Dictionary<string, object>();

                        for (int i = 0; i < Reader.FieldCount; i++)
                        {
                            Data.Add(Reader.GetName(i), Reader.GetValue(i));
                        }

                        Results.Add(Row, Data);
                        Row++;
                    }

                    Reader.Dispose();
                    Command.Dispose();
                    Connection.Close();
                    Connection.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine("[IDatabase] Error running query: " + Query);
                    Console.WriteLine(e.ToString());
                }
            }

            return Results;
        }

        public bool RunQuickNoResult(string Query)
        {
            using (MySqlConnection Connection = GetConnection())
            {
                try
                {
                    Connection.Open();
                    MySqlCommand Command = Connection.CreateCommand();
                    Command.CommandText = Query;
                    Command.ExecuteNonQuery();
                    Command.Dispose();
                    Connection.Close();
                    Connection.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine("[IDatabase] Error running query: " + Query);
                    Console.WriteLine(e.ToString());
                    return false;
                }

                return true;
            }
        }
    }
}