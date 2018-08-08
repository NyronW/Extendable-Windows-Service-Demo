using System.IO;
using WillCorp.Core.FileSystem;

namespace WillCorp.Services.FileSystem
{
    public class SetFileNameToUpperCase : FileTransformationBase, IServicePlugin
    {
        protected override ImportFile Transform(ImportFile file)
        {
            var path = Path.Combine(Path.GetDirectoryName(file.FilePath), "uppercase_name", file.Name.ToUpperInvariant());

            return ImportFile.Create(path, file.Content);
        }
    }
}
