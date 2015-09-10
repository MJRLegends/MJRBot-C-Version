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
    class RanksFile
    {
        private static String fileName = @"C:\MJR_Bot\" + @"\" + BotClient.getChannel(false) + @"\" + "Ranks.xml";

        public static String[] ranks = { "gold", "sliver", "bronze", "none" };

        public static int GoldPrice = 5000;
        public static int SliverPrice = 3500;
        public static int BronzePrice = 2000;

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
                    writer.WriteAttributeString("Rank", "None");

                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
        }

        public static String getRank(String User)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(fileName);

            //select a list of Nodes matching xpath expression
            XmlNodeList oXmlNodeList = xDoc.SelectNodes("//List//Users");

            foreach (XmlNode x in oXmlNodeList)
            {
                if (x.Attributes["Name"].Value.ToLower().Equals(User.ToLower()))
                {
                    string rank = x.Attributes["Rank"].Value;
                    return rank;
                }
            }
            return "";
        }

        public static void setRank(String User, String rank, bool buy)
        {
            if (buy)
            {
                if (hasRank(User, rank.ToLower()))
                {
                    BotClient.sendChatMessage(User + " you already have the rank " + rank + "!");
                    return;
                }
                else if (PointsFile.getPoints(User) < getRankPrice(rank))
                {
                    BotClient.sendChatMessage(User + " you do not have enough to buy the rank " + rank + "! You need to have " + getRankPrice(rank) + " points!");
                    return;
                }
                else
                {
                    PointsFile.RemovePoints(User, getRankPrice(rank));
                }
            }
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
                element.SetAttributeValue("Rank", rank);
                document.Save(fileName);
                BotClient.sendChatMessage(User + " your rank has now been updated to " + rank);
            }
        }

        public static void RemoveRank(String User)
        {
            setRank(User, "None", false);
        }

        public static Boolean hasRank(String User, String rank)
        {
            String newRank = getRank(User);
            newRank.ToLower();
            if (rank.ToLower().Equals(newRank))
                return true;
            return false;
        }

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
                   new XAttribute("Rank", "None")));
                xDocument.Save(fileName);
            }
        }
        public static Boolean isValidRank(String Rank)
        {
            Rank = Rank.ToLower();
            String ranksList = "";
            foreach (String rank in ranks)
            {
                ranksList = ranksList + " " + rank;
            }
            if (ranksList.Contains(Rank))
                return true;
            else
                return false;
        }
        public static int getRankPrice(String Rank)
        {
            Rank = Rank.ToLower();
            if (Rank.ToLower().Equals(ranks[0]))
                return GoldPrice;
            else if (Rank.ToLower().Equals(ranks[1]))
                return SliverPrice;
            else if (Rank.ToLower().Equals(ranks[2]))
                return BronzePrice;
            return 0;
        }
    }
}
