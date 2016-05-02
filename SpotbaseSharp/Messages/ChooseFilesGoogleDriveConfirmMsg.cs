namespace SpotbaseSharp.Messages
{
    public class ChooseFilesGoogleDriveConfirmMsg
    {
        public ChooseFilesGoogleDriveConfirmMsg(string[] fileIds, bool saveLargeFile)
        {
            FileIds = fileIds;
            SaveLargeFile = saveLargeFile;
        }

        public string[] FileIds { get; private set; }
        public bool SaveLargeFile { get; private set; }
    }
}