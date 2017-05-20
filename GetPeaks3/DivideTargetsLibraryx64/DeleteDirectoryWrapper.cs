using System.IO;
using System.Threading;

namespace DivideTargetsLibraryX64
{
    public static class DeleteDirectoryWrapper
    {
        public static void TryDelete(string destinationDirectory)
        {
            try
            {
                DeleteDirectory(destinationDirectory);
            }
            catch (IOException)
            {
                Thread.Sleep(0);
                DeleteDirectory(destinationDirectory);
            }
        }

        private static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
    }
}
