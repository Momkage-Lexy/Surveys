using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using System.Windows.Forms;


namespace SurveyApp{

    public partial class Form1 : Form
    {
        private string dbPath = Path.Combine(Application.StartupPath, "survey.db");

        public Form1()
        {
            InitializeComponent();
            InitializeDatabase();
            InitializeAsync();
        }

        private void InitializeDatabase()
        {
            // Connect to DB
            if (!File.Exists(dbPath))
            {
                using var connection = new SqliteConnection($"Data Source={dbPath}");
                connection.Open();
            }

            using var conn = new SqliteConnection($"Data Source={dbPath}");
            conn.Open();

            // Run up script
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS responses (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    timestamp TEXT,
                    name TEXT,
                    org TEXT,
                    color TEXT,
                    animal TEXT,
                    prize INTEGER
                );
            ";

            // Get # of entries
            cmd.ExecuteNonQuery();
        }

        private async void InitializeAsync()
        {
            // Initialize Web View 2
            await webView21.EnsureCoreWebView2Async(null);

            // Get UI 
            string indexPath = Path.Combine(Application.StartupPath, "wwwroot", "index.html");
            webView21.CoreWebView2.Navigate($"file:///{indexPath.Replace("\\", "/")}");

            // Send UI to view
            webView21.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            // Transform user input to json & save to db
            var json = e.WebMessageAsJson;
            var data = JsonSerializer.Deserialize<SurveyResult>(json);
            SaveToDatabase(data);
        }

        private void SaveToDatabase(SurveyResult r)
        {
            // Connect to DB
            using var conn = new SqliteConnection($"Data Source={dbPath}");
            conn.Open();

            // Insert user data
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO responses (timestamp, name, org, color, animal, prize)
                VALUES ($ts, $n, $o, $c, $a, $p)
            ";

            cmd.Parameters.AddWithValue("$ts", DateTime.Now.ToString("o"));
            cmd.Parameters.AddWithValue("$n", r.name);
            cmd.Parameters.AddWithValue("$o", r.org);
            cmd.Parameters.AddWithValue("$c", r.color);
            cmd.Parameters.AddWithValue("$a", r.animal);
            cmd.Parameters.AddWithValue("$p", r.prize);

            // Get number of entries
            cmd.ExecuteNonQuery();
        }

        public class SurveyResult
        {
            public string name { get; set; }
            public string org { get; set; }
            public string color { get; set; }
            public string animal { get; set; }
            public int prize { get; set; }
        }
    }
}