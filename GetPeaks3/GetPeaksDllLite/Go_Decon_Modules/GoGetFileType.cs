namespace GetPeaksDllLite.Go_Decon_Modules
{
    public static class GoGetFileType
    {
        public static string GetFileExtension(string newFile)
        {
            string fileTypeFromFile = "";
            int lastLocation = newFile.LastIndexOf(".");
            for (int i = lastLocation + 1; i < newFile.Length; i++)//+1 for the next character after the .
            {
                fileTypeFromFile += newFile[i];
            }
            fileTypeFromFile = fileTypeFromFile.ToLower();
            return fileTypeFromFile;
        }
    }
}
