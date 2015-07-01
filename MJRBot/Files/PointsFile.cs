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
    class PointsFile
    {
        private static String fileName = @"C:\MJR_Bot\" + @"\" + BotClient.getChannel(false) + @"\" + "Points.xml";
        /// <summary>
        /// Loads the Points File
        /// </summary>
        public static void load()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            if (!File.Exists(fileName))
            {
                using (XmlWriter writer = XmlWriter.Create(fileName))
                {
                    // Root.
                    writer.WriteStartDocument();
                    writer.WriteStartElement("List");
                    writer.WriteWhitespace("\n");
                    writer.WriteStartElement("Users");

                    writer.WriteAttributeString("Name", "mjrbot");
                    writer.WriteAttributeString("Points", "1000");

                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
        }

        /// <summary>
        /// To get the amount of points from the User
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public static int getPoints(String User)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(fileName);

            //select a list of Nodes matching xpath expression
            XmlNodeList oXmlNodeList = xDoc.SelectNodes("//List//Users");
  
            foreach (XmlNode x in oXmlNodeList)
            {
                if (x.Attributes["Name"].Value.ToLower().Equals(User.ToLower()))
                {
                    string points = x.Attributes["Points"].Value;
                    return Convert.ToInt32(points);
                }
            }
            return 0;
        }

        /// <summary>
        /// To set the amount of points of a User
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Points"></param>
        public static void setPoints(String User, int Points)
        {
            isOnList(User);
            var document = XDocument.Load(fileName);
            var elements = from e1 in document.Elements()
                           where e1.Name == "List"
                           from e2 in e1.Elements()
                           where e2.Name == "Users"
                           from attribute in e2.Attributes()
                           where attribute.Name == "Name" && attribute.Value == User.ToLower()
                           select e2;
            var element = elements.SingleOrDefault();
            if (element != null)
            {
                element.SetAttributeValue("Points", Points);
                document.Save(fileName);
            }
        }

        /// <summary>
        /// To add points to a User
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Points"></param>
        public static void AddPoints(String User, int Points)
        {
            int newPoints = getPoints(User) + Points;
            setPoints(User, newPoints);
        }

        /// <summary>
        /// To add random amount of points to a User
        /// </summary>
        /// <param name="User"></param>
        public static int AddRandomPoints(String User)
        {
            Random ran = new Random();
            int points = ran.Next(0,100);
            int newPoints = getPoints(User) + points;
            setPoints(User, newPoints);
            return points;
        }

        /// <summary>
        /// To remove points to a User
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Points"></param>
        public static void RemovePoints(String User, int Points) 
        {
            int newPoints = getPoints(User) - Points;
            setPoints(User, newPoints);
        }

        /// <summary>
        /// Checks a User has a certain amount of points
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Points"></param>
        /// <returns></returns>
        public static Boolean hasPoints(String User, int Points)
        {
            if (getPoints(User) > Points)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a User exists within the Points File
        /// </summary>
        /// <param name="User"></param>
        public static void isOnList(String User)
        {
            bool Found = false;
            XDocument xmlDoc = XDocument.Load(fileName);

            Found = (from data in xmlDoc.Element("List").Elements("Users") where (string)data.Attribute("Name") == User select data).Any();
            
            if (Found == false)
            {
                XDocument xDocument = XDocument.Load(fileName);
                XElement root = xDocument.Element("List");
                IEnumerable<XElement> rows = root.Descendants("Users");
                XElement firstRow = rows.First();
                firstRow.AddBeforeSelf(
                   new XElement("Users",
                   new XAttribute("Name", User),
                   new XAttribute("Points", "1000")));
                xDocument.Save(fileName);
            }
        }
    }
}
