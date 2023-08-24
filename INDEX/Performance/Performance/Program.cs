using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Performance
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Menu();

            Console.ReadLine();
        }

        #region View
        static void Menu()
        {
            SqlConnection s = GetConnection();
            s.Open();

            var path = @"C:\temp\random_numbers.txt";

            Console.WriteLine("Connected. \n");

            while (true)
            {
                Console.WriteLine("Welcome to your console database program. Your options: \n\n'" +
                    "1. Run database creation script. \n" +
                    "2. Run text file creation method. \n" +
                    "3. Populate text file with a million numbers 1-10.000. \n" +
                    "4. View the number that occurs the GREATEST amount of times in the random numbers. \n" +
                    "5. View the number that occurs the SMALLEST amount of times in the random numbers.");

                var selection = Convert.ToInt32(Console.ReadKey(true).Key);

                switch (selection)
                {
                    case 1:
                        CreateDatabase(s);
                        break;

                    case 2:
                        CreateTextFile(path);
                        break;

                    case 3:
                        PopulateTextFile(path);
                        break;

                    case 4:
                        // stored procedure highest
                        break;

                    case 5:
                        // stored procedure lowest
                        break;

                }
            }
        }

        #endregion

        #region Controller

        // creates views for instance table in order to track incidence  // TODO: CREATE INSTANCE TABLE
        static void CreateViews(SqlConnection s)
        {
            for (int i = 1; i < 1000001; i++)
            {
                var script = //"USE [Performance] " +
                    $"CREATE VIEW [{i + 1}] AS " +
                    "SELECT * FROM random " +
                    $"WHERE random_number = {i + 1}  ";

                ExecuteScript(script, s);
            }

            Console.WriteLine("Views created. ");
        }

        static void CreateInstances(SqlConnection s)
        {
            for (int i = 1; i < 1000001; i++)
            {
                var script = "USE [Performance] " +
                    "INSERT INTO Instance " +
                    "VALUES (" +
                    $"'[{i}]', (SELECT COUNT(id) from [{i}]) " +
                    "); ";

                ExecuteScript(script, s);
            }

            Console.WriteLine("Instances created. ");
        }

        // populates text file with 1M random numbers 1-10.000
        static void PopulateTextFile(string path)
        {
            Random rng = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 1000000; i++)
            {
                sb.Append($"{i + 1}, {rng.Next(0, 10000)} \r\n");
            }

            File.AppendAllText(path, sb.ToString());

            Console.WriteLine("Text file populated. Please import the .txt file. ");
        }

        // creates a text file in temp if missing
        static void CreateTextFile(string path)
        {
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.Write("");
                }

                Console.WriteLine("Text file created or extant. ");
            }
        }

        // method for freestyle script execution, accepts a script as parameter
        static void ExecuteScript(string script, SqlConnection s)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(script);

            string sqlQuery = sb.ToString();
            using (SqlCommand cmd = new SqlCommand(sqlQuery, s))
            {
                cmd.ExecuteNonQuery();
            }
        }

        // establishes connection - contains connection string for relevant database
        static SqlConnection GetConnection()
        {
            return new SqlConnection(@"Data Source = localhost; Initial Catalog = Performance; Trusted_Connection=true; TrustServerCertificate=true;");
        }

        // runs database creation script
        static void CreateDatabase(SqlConnection s)
        {
            var import = File.ReadAllLines(@"C:\temp\create_database.sql");
            var script = "";

            foreach (string line in import)
                script += $"{line} ";

            ExecuteScript(script, s);

            Console.WriteLine("Database created or extant. ");
        }

        #endregion
    }
}
