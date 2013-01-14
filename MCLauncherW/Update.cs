using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using Ionic.Zip;
using System.Windows.Forms;

namespace MCLauncherW
{
    class Update
    {

        private static void unzip(String file, String path)
        {
            using (ZipFile zip1 = ZipFile.Read(file))
            {
                // here, we extract every entry, but we could extract conditionally
                // based on entry name, size, date, checkbox status, etc.  
                foreach (ZipEntry e in zip1)
                {
                    e.Extract(path, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        public static void updateFile(String file, String path)
        {
            unzip(file, path);
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
            MessageBox.Show("Done!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
