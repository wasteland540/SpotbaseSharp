namespace SpotbaseSharp.Messages
{
    public class ImportConfirmMsg
    {
        public ImportConfirmMsg(string filename, bool copyLargeFiles)
        {
            Filename = filename;
            CopyLargeFiles = copyLargeFiles;
        }

        public string Filename { get; set; }
        public bool CopyLargeFiles { get; set; }
    }
}