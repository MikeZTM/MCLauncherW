using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Packaging;

namespace MCLauncherW
{
    class Update
    {
        private const long BUFFER_SIZE = 4096;

        private static void ExtractPackageParts(string packagePath, string targetDirectory)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(targetDirectory);
            using (Package package = Package.Open(packagePath, FileMode.Open, FileAccess.Read))
            {
                PackagePart documentPart = null;
                PackagePart resourcePart = null;
                Uri uriResourceTarget = null;
                foreach (PackageRelationship relationship in package.GetRelationships())
                {
                    uriResourceTarget = PackUriHelper.ResolvePartUri(documentPart.Uri, relationship.TargetUri);
                    resourcePart = package.GetPart(uriResourceTarget);
                    ExtractPart(resourcePart, targetDirectory);
                }
            }
        }

        private static void ExtractPart(PackagePart packagePart, string pathToTarget)
        {
            string stringPart = packagePart.Uri.ToString().TrimStart('/');
            Uri partUri = new Uri(stringPart, UriKind.Relative);
            Uri uriFullPartPath =
            new Uri(new Uri(pathToTarget, UriKind.Absolute), partUri);
            Directory.CreateDirectory(
                Path.GetDirectoryName(uriFullPartPath.LocalPath));
            using (FileStream fileStream =
                new FileStream(uriFullPartPath.LocalPath, FileMode.Create))
            {
                CopyStream(packagePart.GetStream(), fileStream);
            }
        }

        private static void CopyStream(System.IO.Stream inputStream, System.IO.Stream outputStream)
        {
            long bufferSize = inputStream.Length < BUFFER_SIZE ? inputStream.Length : BUFFER_SIZE;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            long bytesWritten = 0;
            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
                bytesWritten += bufferSize;
            }
        }

        public static void updateFile(String file, String path)
        {
            ExtractPackageParts(file, path);
            //delete files
            System.IO.StreamReader toDeleteFile = new System.IO.StreamReader(path + "\\todelete.txt");
            String toDelete = toDeleteFile.ReadToEnd();
            toDeleteFile.Close();

            string[] fileList = toDelete.Split(';');
            foreach (string delFile in fileList)
            {
                try
                {
                    string delete = path + delFile.Replace('/', '\\');
                    File.Delete(delete);
                }
                catch (Exception)
                {
                }
            }
            try
            {
                string delete = path + "\\todelete.txt";
                File.Delete(delete);
            }
            catch (Exception)
            {
            }
        }
    }
}
