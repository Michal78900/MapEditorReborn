namespace MapEditorReborn
{
    using System;
    using System.Threading;
    using Exiled.API.Features;

    internal static class ServerCountHandler
    {
        internal static void Loop()
        {
            Thread.Sleep(10000);

            while (true)
            {
                try
                {
                    HttpQuery.Get($"https://mer.scpsecretlab.pl/?address={Server.IpAddress}:{Server.Port}");
                }
                catch (Exception)
                {
                    // ignored
                }

                Thread.Sleep(1800000);
            }
        }
    }
}