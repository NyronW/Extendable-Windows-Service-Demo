using System;
using System.IO;
using System.Text;

namespace WillCorp.Core.FileSystem
{
    /// <summary>
    /// Represents a file that was recieved by the importer module
    /// </summary>
    public class ImportFile
    {
        private ImportFile(string filePath, string content)
        {
            if(!string.IsNullOrWhiteSpace(filePath))
            {
                FilePath = filePath;
                Content = content;
                Name = Path.GetFileName(filePath);
            }
        }

        public string FilePath { get; }
        public string Name { get; }
        public string Content { get; }

        public bool IsValid => !string.IsNullOrEmpty(FilePath);
        public bool IsEmpty => !string.IsNullOrEmpty(Content);

        public void Save()
        {
            if (!IsValid) return;
            File.WriteAllText(FilePath, Content);
        }

        public static ImportFile Create(string filePath, string content)
        {
            Contracts.Require(!string.IsNullOrWhiteSpace(filePath), "Importer file path is required");
            Contracts.Require(!string.IsNullOrWhiteSpace(content), "File content is required");

            return new ImportFile(filePath, content);
        }
        public static ImportFile Invalid => new ImportFile(string.Empty, string.Empty);
        public static ImportFile Empty(string path) => new ImportFile(path, string.Empty);

        #region Equality
        public override bool Equals(object obj)
        {
            var other = obj as ImportFile;

            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(other.Name))
                return false;

            return Name == other.Name && Content == other.Content;
        }

        public static bool operator ==(ImportFile a, ImportFile b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(ImportFile a, ImportFile b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetType().ToString() + Name + Content).GetHashCode();
        }
        #endregion

        public override string ToString()
        {
            return Name;
        }
    }
}
