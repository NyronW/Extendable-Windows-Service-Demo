using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WillCorp.Configuration;
using WillCorp.Logging;

namespace WillCorp.App.Importer
{
    public class DirectoryMonitor
    {
        private readonly ILogger _logger;
        private readonly string _directory;
        private readonly HttpClient _client;

        private FileSystemWatcher _watcher;

        public DirectoryMonitor(ILogger logger, string endpoint, string directory)
        {
            if (string.IsNullOrEmpty(directory) || !Directory.Exists(Path.GetFullPath(directory)))
            {
                logger.Warn("Invalid import directory");
                return;
            }

            if (string.IsNullOrEmpty(endpoint))
            {
                logger.Warn("Invalid web api endpoint");
                return;
            }

            _logger = logger;
            _directory = Path.GetFullPath(directory);

            _client = new HttpClient
            {
                BaseAddress = new Uri(endpoint)
            };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            _watcher = new FileSystemWatcher(_directory, "*.txt")
            {
                IncludeSubdirectories = false
            };
            _watcher.Created += (object sender, FileSystemEventArgs e) =>
            {
                var fileName = Path.GetFileName(e.FullPath);
                _logger.Debug("Processing {fileName}", fileName);

                using (_logger.AddContext("ImportedFileName", fileName))
                {
                    try
                    {
                        var todos = File.ReadAllLines(e.FullPath);
                        if (!todos.Any()) return;

                        foreach (var todo in todos)
                        {
                            try
                            {
                                AddTodo(todo).Wait();
                            }
                            catch
                            {
                                throw;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }
            };
            _watcher.Error += (object sender, ErrorEventArgs e) =>
            {
                Exception ex = e.GetException();
                _logger.Error(ex.Message);

                if (ex.InnerException != null)
                {
                    _logger.Error(ex.InnerException);
                }
            };
        }

        private async Task AddTodo(string todo)
        {
            var content = new FormUrlEncodedContent(new []
            {
                new KeyValuePair<string, string>("", todo)
            });

            var response = await _client.PostAsync(
                    "api/todos", content);

            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            var url = response.Headers.Location;

            _logger.Information("Todo created at {url}", url);
        }

        public void Start()
        {
            if (string.IsNullOrEmpty(_directory)) return;

            _watcher.EnableRaisingEvents = true;
            _logger.Debug($"File watcher monitoring {_directory} directory for files");
        }

        public void Stop()
        {
            _watcher?.Dispose();
            _logger.Information("Stopped file watcher");
        }
    }
}
