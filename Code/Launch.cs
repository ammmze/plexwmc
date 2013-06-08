using System.Collections.Generic;
using Microsoft.MediaCenter.Hosting;

namespace PlexWMC
{
    public class Settings : IAddInModule, IAddInEntryPoint
    {
        private static HistoryOrientedPageSession s_session;

        public void Initialize(Dictionary<string, object> appInfo, Dictionary<string, object> entryPointInfo)
        {
        }

        public void Uninitialize()
        {
        }

        public void Launch(AddInHost host)
        {
            if (host != null && host.ApplicationContext != null)
            {
                host.ApplicationContext.SingleInstance = true;
            }
            s_session = new HistoryOrientedPageSession();
            Application app = new Application(s_session, host);
            app.GoToSettings();
        }
    }
}