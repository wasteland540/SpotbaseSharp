namespace SpotbaseSharp.Messages
{
    public class ChooseExportPatConfirmMsg
    {
        public string Filename { get; set; }

        public ChooseExportPatConfirmMsg(string filename)
        {
            Filename = filename;
        }
    }
}