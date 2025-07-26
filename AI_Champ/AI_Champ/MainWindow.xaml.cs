using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Printing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AI_Champ
{
    public partial class MainWindow : Window
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string OPENROUTER_API_KEY = "API_KEY";
        private const string MODEL = "tngtech/deepseek-r1t2-chimera:free";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private async void Submit_Click(object sender, RoutedEventArgs e)
        {
            string input = InputBox.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            if (input.StartsWith("@"))
            {
                var match = Regex.Match(input, @"@(?<cmd>\w+)-(?<target>[^\s]+)(\s+(?<data>.+))?");
                if (match.Success)
                {
                    string cmd = match.Groups["cmd"].Value.ToLower();
                    string target = match.Groups["target"].Value;
                    string data = match.Groups["data"].Success ? match.Groups["data"].Value : "";

                    switch (cmd)
                    {
                        case "notepad":
                            SaveNote(target, data);
                            OutputText.Text = $"📝 Saved note to '{target}'.";
                            break;

                        case "youtube":
                            OpenYouTube(target);
                            OutputText.Text = $"▶️ Opening YouTube video: {target}";
                            break;

                        case "website":
                            OpenWebsite(target);
                            OutputText.Text = $"🌐 Opening website: {target}";
                            break;

                        case "app":
                            OpenApplication(target);
                            OutputText.Text = $"🚀 Launching app: {target}";
                            break;

                        case "pdf":
                            OpenPDF(target);
                            OutputText.Text = $"📄 Opening PDF: {target}";
                            break;

                        default:
                            OutputText.Text = "❓ Unknown command.";
                            break;
                    }
                }
                else
                {
                    OutputText.Text = "❌ Invalid command format.";
                }
            }
            else
            {
                string aiResponse = await AskOpenRouterAI(input);
                OutputText.Text = aiResponse;
            }

            InputBox.Clear();
        }
        private void OpenYouTube(string videoName)
        {
            string url = $"https://www.youtube.com/results?search_query={Uri.EscapeDataString(videoName)}";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        private void OpenWebsite(string website)
        {
            string url = website.StartsWith("http") ? website : $"https://{website}";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        private void OpenApplication(string appName)
        {
            try
            {
                System.Diagnostics.Process.Start(appName);
            }
            catch (Exception ex)
            {
                OutputText.Text = $"⚠️ Failed to open app '{appName}': {ex.Message}";
            }
        }

        private void OpenPDF(string pdfName)
        {
            try
            {
                string pdfPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{pdfName}.pdf");
                if (!File.Exists(pdfPath))
                {
                    OutputText.Text = $"❌ PDF '{pdfPath}' not found.";
                    return;
                }

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = pdfPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                OutputText.Text = $"⚠️ Failed to open PDF: {ex.Message}";
            }
        }


        private void SaveNote(string title, string content)
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AI_ChampNotes");
            Directory.CreateDirectory(folder);
            string filePath = Path.Combine(folder, $"notepad-{title}.txt");
            File.AppendAllText(filePath, $"{DateTime.Now}: {content}\n");
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async Task<string> AskOpenRouterAI(string prompt)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", OPENROUTER_API_KEY);
                httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost"); // required by OpenRouter

                var payload = new
                {
                    model = MODEL,
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    }
                };

                string json = JsonConvert.SerializeObject(payload);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);

                if (!response.IsSuccessStatusCode)
                {
                    return $"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
                }

                string result = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(result);
                return jsonResponse?.choices[0]?.message?.content ?? "(No response)";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
