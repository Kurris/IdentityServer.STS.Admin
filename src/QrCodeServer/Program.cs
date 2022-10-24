using Kurisu.Startup;

namespace QrCodeServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            KurisuHost.Run<Startup>(args);
        }
    }
}