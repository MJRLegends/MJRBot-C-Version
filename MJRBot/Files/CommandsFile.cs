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
    class CommandsFile
    {
        private static String fileName = @"C:\MJR_Bot\" + @"\" + BotClient.getChannel(false) + @"\" + "Commands.xml";

        /// <summary>
        /// Loads the Settings File
        /// </summary>
        public static void load()
        {
            if (!File.Exists(fileName))
            {
                using (XmlWriter writer = XmlWriter.Create(fileName))
                {
                    // Root.
                    writer.WriteStartDocument();
                    writer.WriteStartElement("List");
                    writer.WriteWhitespace("\n");

                    writer.WriteStartElement("Commands");
                    writer.WriteAttributeString("CommandName", "test");
                    writer.WriteAttributeString("CommandResponse", "Test Command");
                    writer.WriteAttributeString("CommandEnabled", "true");
                    writer.WriteAttributeString("CommandPermissions", "User");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");
                    writer.WriteEndDocument();
                }
            }
        }

        /// <summary>`
        /// Get a value of a Setting from the Settings File
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public static String getCommandResponse(String name)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(fileName);

            XmlNodeList oXmlNodeList = xDoc.SelectNodes("//Commands");

            foreach (XmlNode x in oXmlNodeList)
            {
                if (x.Attributes["CommandName"].Value.ToLower().Equals(name.ToLower()))
                {
                    string value = x.Attributes["CommandResponse"].Value;
                    return value;
                }
            }
            return "";
        }

        public static String getCommandPermissions(String name)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(fileName);

            XmlNodeList oXmlNodeList = xDoc.SelectNodes("//Commands");

            foreach (XmlNode x in oXmlNodeList)
            {
                if (x.Attributes["CommandName"].Value.ToLower().Equals(name.ToLower()))
                {
                    string value = x.Attributes["CommandPermissions"].Value;
                    return value;
                }
            }
            return "";
        }

        public static String getCommandState(String name)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(fileName);

            XmlNodeList oXmlNodeList = xDoc.SelectNodes("//Commands");

            foreach (XmlNode x in oXmlNodeList)
            {
                if (x.Attributes["CommandName"].Value.ToLower().Equals(name.ToLower()))
                {
                    string value = x.Attributes["CommandEnabled"].Value;
                    xDoc = null;
                    return value;
                }
            }
            xDoc = null;
            return "";
        }

        /// <summary>
        /// Sets a value of a Setting in the Settings File
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="value"></param>
        public static void setCommand(String name, String response, String enabled, String permission)
        {
            String loadpath = fileName;

            var document = XDocument.Load(loadpath);
            var elements = from e1 in document.Elements()
                           where e1.Name == "List"
                           from e2 in e1.Elements()
                           where e2.Name == "Commands"
                           from attribute in e2.Attributes()
                           where attribute.Name == "CommandName" && attribute.Value == name
                           select e2;
            var element = elements.SingleOrDefault();
            if (element != null)
            {
                element.SetAttributeValue("CommandResponse", response);
                element.SetAttributeValue("CommandEnabled", enabled);
                element.SetAttributeValue("CommandPermissions", permission);
                document.Save(loadpath);
            }
            document = null;
        }

        public static void addCommand(String name, String response, String enabled, String permission)
        {
            bool Found = false;
            XDocument xmlDoc = XDocument.Load(fileName);

            Found = (from data in xmlDoc.Element("List").Elements("Commands") where (string)data.Attribute("CommandName") == name select data).Any();

            if (Found == false)
            {
                XDocument xDocument = XDocument.Load(fileName);
                XElement root = xDocument.Element("List");
                IEnumerable<XElement> rows = root.Descendants("Commands");
                XElement firstRow = rows.First();
                firstRow.AddBeforeSelf(
                   new XElement("Commands",
                   new XAttribute("CommandName", name),
                   new XAttribute("CommandResponse", response),
                   new XAttribute("CommandEnabled", enabled),
                   new XAttribute("CommandPermissions", permission)));
                xDocument.Save(fileName);
            }
        }
    }
}
