namespace SpotbaseSharp.Messages
{
    public class ExportFaildMsg
    {
        public ExportFaildMsg(string errorMsg)
        {
            ErrorMsg = errorMsg;
        }

        public string ErrorMsg { get; set; }
    }
}