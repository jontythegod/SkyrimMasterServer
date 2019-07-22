namespace SkyrimMasterServer
{
    public class SkyrimServer
    {
        public string Title;
        public string Password;
        public int Port;

        public SkyrimServer(string pTitle, string pPassword, int pPort)
        {
            Title = pTitle;
            Password = pPassword;
            Port = pPort;
        }
    }
}
