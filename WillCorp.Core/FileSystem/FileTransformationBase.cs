using System.IO;

namespace WillCorp.Core.FileSystem
{
    /// <summary>
    /// All inheriting class will be used to perform some
    /// sort of transformation on files recived by the directory monitor class
    /// in the importer module.
    /// </summary>
    public abstract class FileTransformationBase
    {
        public virtual string OutputFolder { get; protected set; }

        public string Process(string filePath)
        {
            var loaded = Load(filePath);
            if (!loaded.IsValid) return filePath;

            var transformed = Transform(loaded);
            if (loaded == transformed) return loaded.FilePath;

            var path = Save(transformed);
            return path;
        }

        protected virtual ImportFile Load(string filePath)
        {
            if (!File.Exists(filePath)) return ImportFile.Invalid;

            var content = File.ReadAllText(filePath);

            if (string.IsNullOrWhiteSpace(content)) return ImportFile.Empty(filePath);

            return ImportFile.Create(filePath, content);
        }

        protected virtual string Save(ImportFile file)
        {
            var folder = Path.GetDirectoryName(file.FilePath);

            if (!string.IsNullOrWhiteSpace(OutputFolder)) folder = OutputFolder;

            var path = Path.Combine(folder, file.Name);
            var next = ImportFile.Create(path, file.Content);
            next.Save();

            return path;
        }


        protected abstract ImportFile Transform(ImportFile file);
    }
}
