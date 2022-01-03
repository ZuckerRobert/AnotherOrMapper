using NewOrMapper_if19b098;
using Npgsql;
using SampleApp.Show;
using System;
using System.Data;



namespace SampleApp
{
    /// <summary>This is the main program class for this sample application.</summary>
    public class Program
    {

        static string connectionString2 = "Host=localhost;Username=postgres;Password=admin;";
        static string connectionString3 = $"Host=localhost;Username=postgres;Password=admin;Database=newORTest;Pooling=true";

        static void Main(string[] args)
        {
            string databaseName = "newORTest";
            string userName = "postgres";
            string password = "admin";
            string connectionString1 = $"Host=localhost;Username={userName};Password={password};Database={databaseName};Pooling=true";


            IDbCommand cmd;

            //datenbank öffnen, sonst erstellen
            try
            {
                Orm.Connection = new NpgsqlConnection(connectionString1);
                Orm.Connection.Open();
                Console.WriteLine("Establish connection");

            }
            catch
            {
                Console.WriteLine("Create Database " + databaseName);
                try
                {
                    Orm.Connection = new NpgsqlConnection(connectionString2);
                    cmd = Orm.Connection.CreateCommand();
                    cmd.CommandText = $"CREATE DATABASE \"{databaseName}\" WITH OWNER = \"{userName}\" ENCODING = 'UTF8' ";
                    Orm.Connection.Open();
                    Console.WriteLine("Establish connection");
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine("error occured, deleting DB");
                    Orm.Connection = new NpgsqlConnection(connectionString2);
                    cmd = Orm.Connection.CreateCommand();
                    cmd.CommandText = $"DROP DATABASE [IF EXISTS] {databaseName}";
                    Orm.Connection.Close();
                    Console.WriteLine("Droped Database");
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }

            //actions ausführen
            InsertObject.Show();

            //ORMapping.Connection.Close();
            //ORMapping.Connection.Open();

            RefreshCon();

            ModifyObject.Show();
            WithFK.Show();
            WithFKList.Show();

            Orm.Connection.Close();
        }

        public static void RefreshCon()
        {
            Orm.Connection.Close();
            Orm.Connection = new NpgsqlConnection(connectionString3);
            Orm.Connection.Open();
        }
    }
}
