using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;
using PlexAPI;
using System;
using System.Xml;
using System.Reflection;

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

        protected Dictionary<string, object> GetDefaultViewProperties()
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Application"] = this;
            properties["User"] = Properties.Settings.Default.PlexUser;

            if (properties["User"] == null)
            {
                properties["User"] = new User();
            }

            return properties;
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
            var properties = GetDefaultViewProperties();

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
            var properties = GetDefaultViewProperties();

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
            var properties = GetDefaultViewProperties();

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
            var properties = GetDefaultViewProperties();

            RegisterSectionEntryPoints(sections);

            if (session != null)
            {
                session.GoToPage("resx://PlexWMC/PlexWMC.Resources/SectionsSettings", properties);
            }
            else
            {
                Debug.WriteLine("GoToSectionsSettings");
            }
        }

        public void GoToSection(string guid)
        {
            var properties = GetDefaultViewProperties();

            properties["Section"] = EntryPoints.GetSectionFromGuid(sections, guid);

            if (session != null)
            {
                session.GoToPage("resx://PlexWMC/PlexWMC.Resources/Section", properties);
            }
            else
            {
                Debug.WriteLine("GoToSection");
            }
        }

        public void RegisterSectionEntryPoints(List<Directory> sections)
        {
            //string assemblyFolder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //throw new Exception(assemblyFolder);
            string app = EntryPoints.GetEntryPointsXmlFromSections(sections);
            XmlReader reader = XmlReader.Create(new System.IO.StringReader(app));
            ApplicationContext.RegisterApplication(reader, false, true, "");
        }

        public List<ThumbnailCommand> thumbnails { get; set; }

        public void GoToBrowse()
        {
            var properties = GetDefaultViewProperties();
            thumbnails = new List<ThumbnailCommand>();
            throw new Exception(host.ApplicationContext.EntryPointInfo["context"].ToString());
            foreach (Directory section in sections)
            {
                ThumbnailCommand t = new ThumbnailCommand();
                t.Title = section.title;
                t.Image = new Image(section.server.GetBaseUrl() + section.thumb + "?X-Plex-Token=" + section.user.authenticationToken);
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

            Properties.Settings.Default.Save();
            
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

        public string username { get; set; }
        public User user
        {
            get
            {
                if (Properties.Settings.Default.PlexUser == null)
                {
                    user = new User();
                }
                return Properties.Settings.Default.PlexUser;
            }
            set
            {
                Properties.Settings.Default.PlexUser = value;
            }
        }
        public ServerList servers
        {
            get
            {
                try
                {
                    string servers = Properties.Settings.Default.PlexServers;
                    return Serialization.Deserialize<ServerList>(servers);
                }
                catch (Exception e)
                {
                    return new ServerList();
                }
                
            }
            set
            {
                Properties.Settings.Default.PlexServers = Serialization.Serialize(value);
            }
        }

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