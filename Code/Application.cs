using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;
using PlexAPI;
using System;

namespace PlexWMC
{
    public class Application : ModelItem
    {
        private AddInHost host;
        private HistoryOrientedPageSession session;

        public Application()
            : this(null, null)
        {
        }

        public Application(HistoryOrientedPageSession session, AddInHost host)
        {
            this.session = session;
            this.host = host;
        }

        public MediaCenterEnvironment MediaCenterEnvironment
        {
            get
            {
                if (host == null) return null;
                return host.MediaCenterEnvironment;
            }
        }
        
        /// <summary>
        /// Navigate back to the previous page.
        /// </summary>
        public void GoBack()
        {
            if (session == null)
            {
                Debug.WriteLine("GoBack");
                return;
            }
            session.BackPage();
        }

        public void GoToMenu()
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Application"] = this;

            if (session != null)
            {
                session.GoToPage("resx://PlexWMC/PlexWMC.Resources/Menu", properties);
            }
            else
            {
                Debug.WriteLine("GoToMenu");
            }
        }

        public void GoToLogin()
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Application"] = this;

            if (session != null)
            {
                session.GoToPage("resx://PlexWMC/PlexWMC.Resources/Login", properties);
            }
            else
            {
                Debug.WriteLine("GoToLogin");
            }
        }

        public void GoToSettings()
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Application"] = this;

            if (session != null)
            {
                session.GoToPage("resx://PlexWMC/PlexWMC.Resources/Settings", properties);
            }
            else
            {
                Debug.WriteLine("GoToSettings");
            }
        }

        public void GoToSectionsSettings()
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Application"] = this;

            if (session != null)
            {
                session.GoToPage("resx://PlexWMC/PlexWMC.Resources/SectionsSettings", properties);
            }
            else
            {
                Debug.WriteLine("GoToSectionsSettings");
            }
        }

        public List<ThumbnailCommand> thumbnails { get; set; }

        public void GoToBrowse()
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Application"] = this;
            thumbnails = new List<ThumbnailCommand>();

            foreach (Directory section in sections)
            {
                ThumbnailCommand t = new ThumbnailCommand();
                t.Title = section.title;
                t.Image = new Image(section.server.GetBaseUrl() + section.thumb);
                thumbnails.Add(t);
            }

            if (session != null)
            {
                session.GoToPage("resx://PlexWMC/PlexWMC.Resources/Browse", properties);
            }
            else
            {
                Debug.WriteLine("GoToSectionsSettings");
            }
        }

        public void LoginToMyPlex(EditableText username, EditableText password)
        {
            this.username = username.Value;
            
            MyPlex api = new MyPlex();
            user = api.Authenticate(username.Value, password.Value);
            servers = api.GetServers(user);
            
            DialogTest(user.authenticationToken);
            //GoBack();
        }

        public void DialogTest(string strClickedText)
        {
            int timeout = 5;
            bool modal = true;
            string caption = Resources.DialogCaption;

            if (session != null)
            {
                MediaCenterEnvironment.Dialog(strClickedText,
                                              caption,
                                              new object[] { DialogButtons.Ok },
                                              timeout,
                                              modal,
                                              null,
                                              delegate(DialogResult dialogResult) { });
            }
            else
            {
                Debug.WriteLine("DialogTest");
            }
        }

        public string[] MyData
        {
            get
            {
                return new string[4] { "Alpha", "Bravo", "Charlie", "Delta" };
            }
        }

        public string username { get; set; }
        public User user { get; set; }
        public List<Server> servers { get; set; }

        public List<string> serverNames
        {
            get
            {
                List<string> names = new List<string>();
                foreach (Server server in servers)
                {
                    names.Add(server.name);
                }
                return names;
            }
        }

        public List<Directory> sections
        {
            get
            {
                List<Directory> sections = new List<Directory>();
                foreach (Server server in servers)
                {
                    sections.AddRange(server.GetLibrarySections());
                }
                return sections;
            }
        }
    }
}