using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Data;

namespace Performance
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Menu();
        }

        #region View

        static void Menu()
        {
            // gets connection string
            SqlConnection s = GetConnection();
            s.Open();

            var path = @"C:\temp\Random.txt";

            Console.WriteLine("Connected. \n");

            // main menu loop
            while (true)
            {
                Console.Clear();

                Console.WriteLine("Welcome to your console database program. Your options: \n\n" +
                    "1. Run database creation script. \n" +
                    "2. Run text file creation method. \n" +
                    "3. Populate text file with a million numbers 1-10.000. \n" +
                    "4. Create Views. \n" +
                    "5. Create Instances. \n" +
                    "6. View the number that occurs the GREATEST amount of times in the random numbers. \n" +
                    "7. View the number that occurs the SMALLEST amount of times in the random numbers. \n\n" +
                    "Press Escape to exit the application. \n");

                var selection = Console.ReadKey(true).Key;

                switch (selection)
                {
                    case ConsoleKey.D1:
                        CreateDatabase(s);
                        break;

                    case ConsoleKey.D2:
                        CreateTextFile(path);
                        Task.Delay(1000).Wait();
                        break;

                    case ConsoleKey.D3:
                        PopulateTextFile(path);
                        break;

                    case ConsoleKey.D4:
                        CreateViews(s);
                        break;

                    case ConsoleKey.D5:
                        CreateInstances(s);
                        break;

                    case ConsoleKey.D6:
                        // stored procedure highest
                        SelectOccurrence(s, 0);
                        break;

                    case ConsoleKey.D7:
                        // stored procedure lowest
                        SelectOccurrence(s, 1);
                        break;

                    case ConsoleKey.Escape:
                        s.Dispose();
                        Environment.Exit(0);
                        break;
                }
            }
        }

        // progress bar so instance creation process can be estimated
        static void ProgressBar(int x, int y)
        {
            Console.SetCursorPosition(x, y);
            string topLeft = "\u2554",
                bottomLeft = "\u255A",
                topRight = "\u2557",
                bottomRight = "\u255D",
                horiz = "\u2550",
                vert = "\u2551";

            Console.Write(topLeft); Console.Write(string.Concat(Enumerable.Repeat(horiz, 20))); Console.Write(topRight + "\n");
            Console.Write(vert + string.Concat(Enumerable.Repeat(" ", 20)) + vert + "\n");
            Console.Write(bottomLeft); Console.Write(string.Concat(Enumerable.Repeat(horiz, 20))); Console.Write(bottomRight + "\n");
        }

        #endregion

        #region Controller

        // runs stored procedure for highest/lowest occurrence number
        static void SelectOccurrence(SqlConnection s, int selection)
        {
            string[] scriptArray =
            {
                "SelectHighestOccurrence",
                "SelectLowestOccurrence"
            };

            var script = "";

            if (selection == 0)
                script = scriptArray[0];
            else if (selection == 1)
                script = scriptArray[1];

            SqlDataAdapter sda = new SqlDataAdapter(script, s);
            DataTable dt = new DataTable();

            sda.Fill(dt);

            var num = dt.Rows[0].ItemArray[0].ToString();
            num = num.Substring(1, num.Length - 2);

            var oNum = dt.Rows[0].ItemArray[1].ToString();

            Console.WriteLine($"Number: {num} \n" +
                $"Number of occurrences: {oNum} \n\n" +
                "Press any key to continue. ");
            Console.ReadKey(true);
        }

        // creates views for instance table in order to track incidence
        static void CreateViews(SqlConnection s)
        {
            // index 1 because it required fewer keystrokes than index 0
            for (int i = 1; i < 10001; i++)
            {
                var script = //"USE [Performance] " +
                    $"CREATE VIEW [{i}] AS " +
                    "SELECT * FROM random " +
                    $"WHERE random_number = {i}  ";

                ExecuteScript(script, s);
            }

            Console.WriteLine("Views created. ");
            Task.Delay(1000).Wait();
        }

        // populates instance table with view info; for tracking incidence
        static void CreateInstances(SqlConnection s)
        {
            Console.WriteLine("Please wait while the program creates necessary instances. This may take a while. \n");
            ProgressBar(0, 13);

            Console.SetCursorPosition(1, 14);
            Console.CursorVisible = false;

            for (int i = 1; i < 10001; i++)
            {
                var script = "USE [Performance] " +
                    "INSERT INTO Instance " +
                    "VALUES (" +
                    $"'[{i}]', (SELECT COUNT(id) from [{i}]) " +
                    "); ";

                if (i % 500 == 0)
                {
                    Console.Write("\u2588");
                }

                ExecuteScript(script, s);
            }

            Console.SetCursorPosition(1, 16);

            Console.WriteLine("Instances created. Press any key to continue. ");
            Console.ReadKey(true);
            Console.CursorVisible = true;
        }

        // populates text file with 1M random numbers 1-10.000
        static void PopulateTextFile(string path)
        {
            Random rng = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 1000000; i++)
            {
                sb.Append($"{i + 1}, {rng.Next(0, 10001)} \r\n");
            }

            File.AppendAllText(path, sb.ToString());

            Console.WriteLine("Text file populated. Please import the .txt file. ");
            Console.ReadLine();
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
            Console.ReadLine();
        }

        #endregion
    }
}
