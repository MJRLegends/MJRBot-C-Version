using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Xml;
using System.Xml.Linq;
using System.Collections;
using System.Windows.Forms;

namespace MJRBot
{
    class SettingsFile
    {
        public static String channel = "";
        private static String fileName = "";
        private static String fileName2 = @"C:\MJR_Bot\MainSettings.xml";

        /// <summary>
        /// Loads the Settings File
        /// </summary>
        public static void load()
        {
            fileName = @"C:\MJR_Bot\" + channel + @"\" + "Settings.xml";
            if (!File.Exists(fileName))
            {
                using (XmlWriter writer = XmlWriter.Create(fileName))
                {
                    // Root.
                    writer.WriteStartDocument();
                    writer.WriteStartElement("List");
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "Commands");
                    writer.WriteAttributeString("SettingValue", "false");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "Points");
                    writer.WriteAttributeString("SettingValue", "false");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "Games");
                    writer.WriteAttributeString("SettingValue", "false");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "Rank");
                    writer.WriteAttributeString("SettingValue", "false");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "Announcement");
                    writer.WriteAttributeString("SettingValue", "false");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "SilentJoin");
                    writer.WriteAttributeString("SettingValue", "true");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "EmoteChecker");
                    writer.WriteAttributeString("SettingValue", "false");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "SymbolChecker");
                    writer.WriteAttributeString("SettingValue", "false");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "LinkChecker");
                    writer.WriteAttributeString("SettingValue", "false");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "BadwordsChecker");
                    writer.WriteAttributeString("SettingValue", "false");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "StartingPoints");
                    writer.WriteAttributeString("SettingValue", "1000");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "AnnouncementsDelay");
                    writer.WriteAttributeString("SettingValue", "10");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "Announcement1");
                    writer.WriteAttributeString("SettingValue", "Test1");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "Announcement2");
                    writer.WriteAttributeString("SettingValue", "Test2");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "Announcement3");
                    writer.WriteAttributeString("SettingValue", "Test3");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "Announcement4");
                    writer.WriteAttributeString("SettingValue", "Test4");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "Announcement5");
                    writer.WriteAttributeString("SettingValue", "Test5");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "AutoPointsDelay");
                    writer.WriteAttributeString("SettingValue", "15");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "MaxEmotes");
                    writer.WriteAttributeString("SettingValue", "5");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "MaxSymbols");
                    writer.WriteAttributeString("SettingValue", "5");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "LinkWarning");
                    writer.WriteAttributeString("SettingValue", "you are not allowed to post links with out permission!");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "LanguageWarning");
                    writer.WriteAttributeString("SettingValue", "you are not allowed to use that language in the chat!");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "EmoteWarning");
                    writer.WriteAttributeString("SettingValue", "please dont spam emotes!");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("SettingName", "SymbolWarning");
                    writer.WriteAttributeString("SettingValue", "please dont spam symbols/emotes!");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");
                    writer.WriteEndDocument();
                }
            }
        }

        public static void loadMain()
        {
            if (!File.Exists(fileName2))
            {
                using (XmlWriter writer2 = XmlWriter.Create(fileName2))
                {
                    // Root.
                    writer2.WriteStartDocument();
                    writer2.WriteStartElement("List");
                    writer2.WriteWhitespace("\n");

                    writer2.WriteStartElement("Settings");
                    writer2.WriteAttributeString("SettingName", "Username");
                    writer2.WriteAttributeString("SettingValue", "");
                    writer2.WriteEndElement();
                    writer2.WriteWhitespace("\n");

                    writer2.WriteStartElement("Settings");
                    writer2.WriteAttributeString("SettingName", "Password");
                    writer2.WriteAttributeString("SettingValue", "");
                    writer2.WriteEndElement();
                    writer2.WriteWhitespace("\n");

                    writer2.WriteEndElement();
                    writer2.WriteWhitespace("\n");
                    writer2.WriteEndDocument();
                }
            }
        }

        /// <summary>`
        /// Get a value of a Setting from the Settings File
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public static String getSetting(String settingName)
        {
            XmlDocument xDoc = new XmlDocument();
            if (settingName.Equals("Username") || settingName.Equals("Password"))
                xDoc.Load(fileName2);
            else
                xDoc.Load(fileName);

            XmlNodeList oXmlNodeList = xDoc.SelectNodes("//Settings");
  
            foreach (XmlNode x in oXmlNodeList)
            {
                if (x.Attributes["SettingName"].Value.ToLower().Equals(settingName.ToLower()))
                {
                    string value = x.Attributes["SettingValue"].Value;
                    return value;
                }
            }
            return "";
        }

        /// <summary>
        /// Sets a value of a Setting in the Settings File
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="value"></param>
        public static void setSetting(String settingName, String value)
        {
            String loadpath = "";
            if (settingName.Equals("Username") || settingName.Equals("Password"))
                loadpath = fileName2;
            else
                loadpath = fileName;
            var document = XDocument.Load(loadpath);
            var elements = from e1 in document.Elements()
                           where e1.Name == "List"
                           from e2 in e1.Elements()
                           where e2.Name == "Settings"
                           from attribute in e2.Attributes()
                           where attribute.Name == "SettingName" && attribute.Value == settingName
                           select e2;
            var element = elements.SingleOrDefault();
            if (element != null)
            {
                element.SetAttributeValue("SettingValue", value);
                document.Save(loadpath);
            }
            document = null;
        }
    }
}
