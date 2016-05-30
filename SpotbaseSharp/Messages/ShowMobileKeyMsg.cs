namespace SpotbaseSharp.Messages
{
    public class ShowMobileKeyMsg
    {
        public ShowMobileKeyMsg(string mobileKey)
        {
            MoibleKey = mobileKey;
        }

        public string MoibleKey { get; private set; }
    }
}