namespace SpotbaseSharp.Messages
{
    public class ChooseFilesConfirmMsg
    {
        public ChooseFilesConfirmMsg(string[] filenames, bool saveLargeFile)
        {
            Filenames = filenames;
            SaveLargeFile = saveLargeFile;
        }

        public string[] Filenames { get; private set; }
        public bool SaveLargeFile { get; private set; }
    }
}