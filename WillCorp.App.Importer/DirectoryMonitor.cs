using System;
using System.Collections.Generic;
using System.IO;
using WillCorp.Configuration;
using WillCorp.Core.FileSystem;
using WillCorp.Logging;

namespace WillCorp.App.Importer
{
    public class DirectoryMonitor
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<FileTransformationBase> _transformations;
        private readonly string _directory;

        private FileSystemWatcher _watcher;

        public DirectoryMonitor(ILogger logger, IConfigurationRepository configuration, IEnumerable<FileTransformationBase> transformations, string directory)
        {
            _logger = logger;
            _transformations = transformations;
            _directory = directory;

            if (string.IsNullOrEmpty(_directory)) return;

            _watcher = new FileSystemWatcher(_directory, "*.txt")
            {
                IncludeSubdirectories = false
            };
            _watcher.Created += (object sender, FileSystemEventArgs e) =>
            {
                var fileName = Path.GetFileName(e.FullPath);
                _logger.Debug("Processing {fileName}", fileName);

                var path = e.FullPath;

                foreach (var transformation in _transformations)
                {
                    path = transformation.Process(path);
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
