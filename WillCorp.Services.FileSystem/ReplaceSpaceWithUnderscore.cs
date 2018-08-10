using System.IO;
using WillCorp.Core.FileSystem;

namespace WillCorp.Services.FileSystem
{
    public class ReplaceSpaceWithUnderscore : FileTransformationBase, IServicePlugin
    {
        protected override ImportFile Transform(ImportFile file)
        {
            var path = Path.Combine(Path.GetDirectoryName(file.FilePath), "underscore", file.Name);

            return ImportFile.Create(path, file.Content.Replace(" ","_"));
        }
    }
}
