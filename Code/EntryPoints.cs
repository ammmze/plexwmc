using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PlexWMC
{
    static class EntryPoints
    {
        public static string GetEntryPointsXmlFromSections(List<PlexAPI.Directory> sections)
        {
            string app = "<application title=\"Plex Libraries\" id=\"{5AB6C12C-741A-4F43-8F5C-F2E3E504CA7F}\" StartMenuStripTitle=\"Plex Libraries\" StartMenuStripCategory=\"Custom Start Menu\\Plex Libraries\">";
            foreach (PlexAPI.Directory section in sections)
            {
                app = app + GetEntryPointXmlFromSection(section);
            }
            return app + "</application>";
        }

        public static PlexAPI.Directory GetSectionFromGuid(List<PlexAPI.Directory> sections, string guid)
        {
            foreach (PlexAPI.Directory section in sections)
            {
                if (guid == GetSectionGuid(section.uuid))
                {
                    return section;
                }
            }
            return null;
        }

        private static string GetEntryPointXmlFromSection(PlexAPI.Directory section)
        {
            /*
             <entrypoint id="{7A952F65-6641-401D-826F-5C2F300D9FFC}"
                          addin="PlexWMC.Settings, PlexWMC,Culture=Neutral,Version=1.0.0.0,PublicKeyToken=4ae5afde023f77cc"
                          title="Settings"
                          description="Plex Settings"
                          imageURL=".\Application.png">
                <category category="Custom Start Menu\Plex" order="102"/>
              </entrypoint>
             */
            string guid = GetSectionGuid(section.uuid);
            string addin = "PlexWMC.Section, PlexWMC,Culture=Neutral,Version=1.0.0.0,PublicKeyToken=4ae5afde023f77cc";
            string title = section.title;
            if (!section.server.owned)
            {
                title = title + " on " + section.server.name;
            }
            string description = section.title + " on " + section.server.name;
            string image = section.server.GetBaseUrl() + section.thumb + "?X-Plex-Token=" + section.user.authenticationToken;// ".\\Application.png";
            string context = guid;
            string category = "Custom Start Menu\\Plex Libraries";
            int order = 0;
            string ep = "<entrypoint id=\"{0}\" addin=\"{1}\" title=\"{2}\" description=\"{3}\" imageURL=\"{4}\" context=\"{5}\"> <category category=\"{6}\" order=\"{7}\"/></entrypoint>";
            return string.Format(ep, guid, addin, title, description, image, context, category, order);
        }

        private static string GetSectionGuid(string uuid)
        {
            string input = uuid;
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
                return new Guid(hash).ToString();
            }
        }
    }
}
