using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace MJRBot
{
    public partial class MJRBot : DevComponents.DotNetBar.Metro.MetroForm
    {
        public MJRBot()
        {
            InitializeComponent();
        }

        private bool connected = false;
        private bool selected = false;
        private bool commandsSelected = false;
        private bool pointsSelected = false;
        private bool gamesSelected = false;
        private bool announcementSelected = false;
        private bool slientJoinSelected = false;
        private bool wordsSelected = false;
        private bool linksSelected = false;
        private bool emotesSelected = false;
        private bool symbolsSelected = false;
        private bool rankSelected = false;

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (connected == false)
            {
                if (txtChannel.Text == "")
                {
                    BotClient.chatMessages.Add("You need to enter a channel!");
                    return;
                }
                txtChannel.Text = txtChannel.Text.ToLower();
                BotClient.channel = "#" + txtChannel.Text;
                if (!Directory.Exists(@"C:\MJR_Bot\" + BotClient.getChannel(false) + @"\"))
                    try
                    {
                        Directory.CreateDirectory(@"C:\MJR_Bot\" + BotClient.getChannel(false) + @"\");
                    }
                    catch (Exception ex)
                    {
                        BotClient.chatMessages.Add("Unable to create directory!");
                        return;
                    }

                PointsFile.load();
                SettingsFile.loadMain();
                SettingsFile.load(BotClient.getChannel(false));
                RanksFile.load();
                CommandsFile.load();
                if (SettingsFile.getSetting(null, "Username") != "" && SettingsFile.getSetting(null, "Password") != "")
                {
                    BotClient.connectToServer("irc.twitch.tv", 6667);
                    BotClient.joinChannel(txtChannel.Text);
                    connected = true;
                    btnConnect.Text = "Disconnect";
                    txtChannel.ReadOnly = true;
                    if (SettingsFile.getSetting(BotClient.getChannel(false), "SilentJoin").Equals("false"))
                        BotClient.sendChatMessage(SettingsFile.getSetting(null, "Username") + " Connected!");

                    timerAutoPoints.Interval = 60000 * Convert.ToInt32(SettingsFile.getSetting(BotClient.getChannel(false), "AutoPointsDelay"));
                    timerAutoPoints.Enabled = true;
                    timerAnnouncements.Interval = 60000 * Convert.ToInt32(SettingsFile.getSetting(BotClient.getChannel(false), "AnnouncementsDelay"));
                    timerAnnouncements.Enabled = true;

                    Viewers.getViewers();
                    Followers.getFollowersNum();
                    Followers.getFollowers();
                    txtFollowers.Text = "";
                    txtModerators.Text = "";
                    foreach (String user in Followers.followers)
                    {
                        txtFollowers.AppendText(user.ToLower() + Environment.NewLine);
                    }
                    btnConnect.Checked = true;
                    btnSideTab.Enabled = true;
                    tabSettings.Visible = true;
                    tabIModsandFollowers.Visible = true;
                    txtMessage.Enabled = true;
                    btnSendMessage.Enabled = true;
                }
                else
                {
                    BotClient.chatMessages.Add("Error! No Login details were set! Go to settings to enter them! \n then reload the program when done!");
                    return;
                }
            }
            else
            {
                if (SettingsFile.getSetting(BotClient.getChannel(false), "SilentJoin").Equals("false"))
                {
                    BotClient.sendChatMessage(SettingsFile.getSetting(null, "Username") + " Disconnected!");
                    Thread.Sleep(6000);
                }
                BotClient.disconnectFromServer();
                disconnect();
            }
        }

        public void disconnect()
        {
            btnConnect.Text = "Connect";
            btnSideTab.Text = ">>";
            MJRBot.ActiveForm.Width = 725;
            btnSideTab.Checked = false;
            selected = false;
            connected = false;
            txtChannel.ReadOnly = false;
            timerAutoPoints.Enabled = false;
            timerAnnouncements.Enabled = false;
            btnConnect.Checked = false;
            btnSideTab.Enabled = false;
            txtMessage.Enabled = false;
            btnSendMessage.Enabled = false;
            tabIModsandFollowers.Visible = false;
            txtFollowers.Text = "";
            txtModerators.Text = "";
            Viewers.moderators.Clear();
            Followers.followers.Clear();
            Followers.followersNum = 0;
            txtUsers.Text = "";
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            SettingsFile.setSetting(null, "Username", txtUsername.Text);
            SettingsFile.setSetting(null, "Password", txtPassword.Text);
            if (comboChannel.Text.Length >= 1)
            {
                SettingsFile.setSetting(comboChannel.Text, "AnnouncementsDelay", txtAnnouncementDelay.Text);
                SettingsFile.setSetting(comboChannel.Text, "StartingPoints", txtStartPoints.Text);
                SettingsFile.setSetting(comboChannel.Text, "AutoPointsDelay", txtAutoPoints.Text);
                SettingsFile.setSetting(comboChannel.Text, "Announcement1", txtAnnouncement1.Text);
                SettingsFile.setSetting(comboChannel.Text, "Announcement2", txtAnnouncement2.Text);
                SettingsFile.setSetting(comboChannel.Text, "Announcement3", txtAnnouncement3.Text);
                SettingsFile.setSetting(comboChannel.Text, "Announcement4", txtAnnouncement4.Text);
                SettingsFile.setSetting(comboChannel.Text, "Announcement5", txtAnnouncement5.Text);

                SettingsFile.setSetting(comboChannel.Text, "MaxEmotes", txtMaxEmotes.Text);
                SettingsFile.setSetting(comboChannel.Text, "MaxSymbols", txtMaxSymbols.Text);
                SettingsFile.setSetting(comboChannel.Text, "LinkWarning", txtLinkMessage.Text);
                SettingsFile.setSetting(comboChannel.Text, "LanguageWarning", txtLanguageMessage.Text);
                SettingsFile.setSetting(comboChannel.Text, "EmoteWarning", txtEmoteMessage.Text);
                SettingsFile.setSetting(comboChannel.Text, "SymbolWarning", txtSymbolMessage.Text);

                if (comboChannel.Text.ToLower().Equals(BotClient.getChannel(false).ToLower()))
                {
                    timerAnnouncements.Interval = Convert.ToInt32(SettingsFile.getSetting(comboChannel.Text, "AnnouncementsDelay")) * 60000;
                    timerAutoPoints.Interval = Convert.ToInt32(SettingsFile.getSetting(comboChannel.Text, "AutoPointsDelay")) * 60000;
                }
                clearSettingTab(false);
                MessageBox.Show("Settings have been updated!");
            }
        }
        private void btnSideTab_Click(object sender, EventArgs e)
        {
            if (selected != false)
            {
                btnSideTab.Text = ">>";
                MJRBot.ActiveForm.Width = 725;
                btnSideTab.Checked = false;
                selected = false;
            }
            else
            {
                loadSettings();
                btnSideTab.Text = "<<";
                MJRBot.ActiveForm.Width = 900;
                btnSideTab.Checked = true;
                selected = true;
            }
        }
        private void btnCommands_Click(object sender, EventArgs e)
        {
            if (commandsSelected != false)
            {
                if (SettingsFile.getSetting(BotClient.getChannel(false), "Points").Equals("true"))
                {
                    btnPoints.Text = "Enable Points";
                    SettingsFile.setSetting(BotClient.getChannel(false), "Points", "false");
                    btnPoints.Checked = false;
                    pointsSelected = false;

                }
                if (SettingsFile.getSetting(BotClient.getChannel(false), "Games").Equals("true"))
                {
                    btnGames.Text = "Enable Games";
                    SettingsFile.setSetting(BotClient.getChannel(false), "Games", "false");
                    btnGames.Checked = false;
                    gamesSelected = false;

                }
                btnCommands.Text = "Enable Commands";
                SettingsFile.setSetting(BotClient.getChannel(false), "Commands", "false");
                btnCommands.Checked = false;
                commandsSelected = false;
            }
            else
            {
                btnCommands.Text = "Disable Commands";
                SettingsFile.setSetting(BotClient.getChannel(false), "Commands", "true");
                btnCommands.Checked = true;
                commandsSelected = true;
            }
        }
        private void btnPoints_Click(object sender, EventArgs e)
        {
            if (SettingsFile.getSetting(BotClient.getChannel(false), "Commands").Equals("true"))
                if (pointsSelected != false)
                {
                    if (SettingsFile.getSetting(BotClient.getChannel(false), "Games").Equals("true"))
                    {
                        btnGames.Text = "Enable Games";
                        SettingsFile.setSetting(BotClient.getChannel(false), "Games", "false");
                        btnGames.Checked = false;
                        gamesSelected = false;
                    }
                    btnPoints.Text = "Enable Points";
                    SettingsFile.setSetting(BotClient.getChannel(false), "Points", "false");
                    btnPoints.Checked = false;
                    pointsSelected = false;
                }
                else
                {
                    btnPoints.Text = "Disable Points";
                    SettingsFile.setSetting(BotClient.getChannel(false), "Points", "true");
                    btnPoints.Checked = true;
                    pointsSelected = true;
                    timerAutoPoints.Interval = 60000 * Convert.ToInt32(SettingsFile.getSetting(BotClient.getChannel(false), "AutoPointsDelay"));
                }
            else
                BotClient.chatMessages.Add("[MJRBot Info]" + "You need to enable commands first!");
        }
        private void btnGames_Click(object sender, EventArgs e)
        {
            if (SettingsFile.getSetting(BotClient.getChannel(false), "Commands").Equals("true"))
                if (SettingsFile.getSetting(BotClient.getChannel(false), "Points").Equals("true"))
                    if (gamesSelected != false)
                    {
                        btnGames.Text = "Enable Games";
                        SettingsFile.setSetting(BotClient.getChannel(false), "Games", "false");
                        btnGames.Checked = false;
                        gamesSelected = false;
                    }
                    else
                    {
                        btnGames.Text = "Disable Games";
                        SettingsFile.setSetting(BotClient.getChannel(false), "Games", "true");
                        btnGames.Checked = true;
                        gamesSelected = true;
                    }
                else
                    BotClient.chatMessages.Add("[MJRBot Info]" + "You need to enable Points first!");
            else
                BotClient.chatMessages.Add("[MJRBot Info]" + "You need to enable Commands first!");
            
        }
        private void btnAnnouncement_Click(object sender, EventArgs e)
        {
            if (announcementSelected != false)
            {
                btnAnnouncement.Text = "Enable Announcements";
                SettingsFile.setSetting(BotClient.getChannel(false), "Announcement", "false");
                btnAnnouncement.Checked = false;
                announcementSelected = false;
            }
            else
            {
                btnAnnouncement.Text = "Disable Announcements";
                SettingsFile.setSetting(BotClient.getChannel(false), "Announcement", "true");
                btnAnnouncement.Checked = true;
                announcementSelected = true;
            }
        }
        private void btnRank_Click(object sender, EventArgs e)
        {
            if (rankSelected != false)
            {
                btnRank.Text = "Enable Ranks";
                SettingsFile.setSetting(BotClient.getChannel(false), "Rank", "false");
                btnRank.Checked = false;
                rankSelected = false;
            }
            else
            {
                btnRank.Text = "Disable Ranks";
                SettingsFile.setSetting(BotClient.getChannel(false), "Rank", "true");
                btnRank.Checked = true;
                rankSelected = true;
            }
        }
        private void btnModerationWords_Click(object sender, EventArgs e)
        {
            if (wordsSelected != false)
            {
                btnModerationWords.Text = "Enable Moderation Words";
                SettingsFile.setSetting(BotClient.getChannel(false), "BadwordsChecker", "false");
                btnModerationWords.Checked = false;
                wordsSelected = false;
            }
            else
            {
                btnModerationWords.Text = "Disable Moderation Words";
                SettingsFile.setSetting(BotClient.getChannel(false), "BadwordsChecker", "true");
                btnModerationWords.Checked = true;
                wordsSelected = true;
            }
        }

        private void btnModerationLinks_Click(object sender, EventArgs e)
        {
            if (linksSelected != false)
            {
                btnModerationLinks.Text = "Enable Moderation Links";
                SettingsFile.setSetting(BotClient.getChannel(false), "LinkChecker", "false");
                btnModerationLinks.Checked = false;
                linksSelected = false;
            }
            else
            {
                btnModerationLinks.Text = "Disable Moderation Links";
                SettingsFile.setSetting(BotClient.getChannel(false), "LinkChecker", "true");
                btnModerationLinks.Checked = true;
                linksSelected = true;
            }
        }

        private void btnModerationEmote_Click(object sender, EventArgs e)
        {
            if (emotesSelected != false)
            {
                btnModerationEmote.Text = "Enable Moderation Emotes";
                SettingsFile.setSetting(BotClient.getChannel(false), "EmoteChecker", "false");
                btnModerationEmote.Checked = false;
                emotesSelected = false;
            }
            else
            {
                btnModerationEmote.Text = "Disable Moderation Emotes";
                SettingsFile.setSetting(BotClient.getChannel(false), "EmoteChecker", "true");
                btnModerationEmote.Checked = true;
                emotesSelected = true;
            }
        }

        private void btnModerationSymbol_Click(object sender, EventArgs e)
        {
            if (symbolsSelected != false)
            {
                btnModerationSymbol.Text = "Enable Moderation Symbols";
                SettingsFile.setSetting(BotClient.getChannel(false), "SymbolChecker", "false");
                btnModerationSymbol.Checked = false;
                symbolsSelected = false;
            }
            else
            {
                btnModerationSymbol.Text = "Disable Moderation Symbols";
                SettingsFile.setSetting(BotClient.getChannel(false), "SymbolChecker", "true");
                btnModerationSymbol.Checked = true;
                symbolsSelected = true;
            }
        }
        private void btnSlientJoin_Click(object sender, EventArgs e)
        {
            if (slientJoinSelected != false)
            {
                btnSlientJoin.Text = "Enable SlientJoin";
                SettingsFile.setSetting(BotClient.getChannel(false), "SilentJoin", "false");
                btnSlientJoin.Checked = false;
                slientJoinSelected = false;
            }
            else
            {
                btnSlientJoin.Text = "Disable SlientJoin";
                SettingsFile.setSetting(BotClient.getChannel(false), "SilentJoin", "true");
                btnSlientJoin.Checked = true;
                slientJoinSelected = true;
            }
        }
        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            BotClient.sendChatMessage(txtMessage.Text);
            txtMessage.Text = "";
        }
        private void timerUpdateGUI_Tick(object sender, EventArgs e)
        {
            Followers.checkForNewFollowers();
            txtChat.Text = BotClient.getChatMessages();
            lblViewersNumber.Text = BotClient.onlineUsers.Count.ToString();
            lblFollowersNum.Text = Followers.followersNum.ToString();
            lblFollowerNum2.Text = Followers.followersNum.ToString();
            lblModeratorsNum.Text = Viewers.moderators.Count.ToString();
        }
        private void timerUpdateUserList_Tick(object sender, EventArgs e)
        {
            if (Followers.followers != null)
                if (Followers.followers.Count > 1)
                    foreach (String user in Followers.followers)
                        if(!txtFollowers.Text.ToLower().Contains(user.ToLower()))
                            txtFollowers.AppendText(user.ToLower() + Environment.NewLine);
            txtModerators.Text = "";
            if (Viewers.moderators != null)
                foreach (String user in Viewers.moderators)
                {
                    if (!user.Equals(""))
                        txtModerators.AppendText(user.ToLower() + Environment.NewLine);
                }
            txtModerators.SelectionStart = 0;
            txtModerators.ScrollToCaret();
            if (BotClient.setup)
            {
                txtUsers.Text = BotClient.getUserList();
                txtUsers.SelectionStart = 0;
                txtUsers.ScrollToCaret();
            }
            else
            {
                txtUsers.Text = BotClient.getUserList();
                BotClient.setup = true;
            }
        }
        
        private void timerAutoPoints_Tick(object sender, EventArgs e)
        {
            if (SettingsFile.getSetting(BotClient.getChannel(false), "Points").Equals("true"))
            {
                foreach (String user in BotClient.onlineUsers)
                {
                    BotClient.chatMessages.Add("[Auto Points]" + "Added 1 points to " + user);
                    PointsFile.AddPoints(user.ToLower(),1);
                }
            }
        }
        private void timerAnnouncements_Tick(object sender, EventArgs e)
        {
            if (SettingsFile.getSetting(BotClient.getChannel(false), "Announcement").Equals("true"))
            {
                int Random = 0;
                Random rnd = new Random();
                Random = rnd.Next(1, 5);
                BotClient.sendChatMessage(SettingsFile.getSetting(BotClient.getChannel(false), "Announcement" + Random));
            }
        }
        private void tabSettings_Click(object sender, EventArgs e)
        {
            MJRBot.ActiveForm.Width = 725;
            clearSettingTab(true);
            if (connected)
            {
                comboChannel.Text = txtChannel.Text;
                SettingsFile.load(comboChannel.Text);
                txtAnnouncementDelay.Text = SettingsFile.getSetting(comboChannel.Text, "AnnouncementsDelay");
                txtStartPoints.Text = SettingsFile.getSetting(comboChannel.Text, "StartingPoints");
                txtAutoPoints.Text = SettingsFile.getSetting(comboChannel.Text, "AutoPointsDelay");

                txtAnnouncement1.Text = SettingsFile.getSetting(comboChannel.Text, "Announcement1");
                txtAnnouncement2.Text = SettingsFile.getSetting(comboChannel.Text, "Announcement2");
                txtAnnouncement3.Text = SettingsFile.getSetting(comboChannel.Text, "Announcement3");
                txtAnnouncement4.Text = SettingsFile.getSetting(comboChannel.Text, "Announcement4");
                txtAnnouncement5.Text = SettingsFile.getSetting(comboChannel.Text, "Announcement5");

                txtMaxEmotes.Text = SettingsFile.getSetting(comboChannel.Text, "MaxEmotes");
                txtMaxSymbols.Text = SettingsFile.getSetting(comboChannel.Text, "MaxSymbols");
                txtLanguageMessage.Text = SettingsFile.getSetting(comboChannel.Text, "LanguageWarning");
                txtEmoteMessage.Text = SettingsFile.getSetting(comboChannel.Text, "EmoteWarning");
                txtLinkMessage.Text = SettingsFile.getSetting(comboChannel.Text, "LinkWarning");
                txtSymbolMessage.Text = SettingsFile.getSetting(comboChannel.Text, "SymbolWarning");
            }
            txtUsername.Text = SettingsFile.getSetting(null, "Username");
            txtPassword.Text = SettingsFile.getSetting(null, "Password");
            string[] folders = System.IO.Directory.GetDirectories(@"C:\MJR_Bot\", "*", System.IO.SearchOption.AllDirectories);
            foreach (String folder in folders)
            {
                comboChannel.Items.Add(folder.ToString().Remove(0, folder.LastIndexOf('\\') + 1));
            }
        }
        private void loadSettings(){
            if(SettingsFile.getSetting(BotClient.getChannel(false), "Commands").Equals("true"))
            {
                btnCommands.Text = "Disable Commands";
                btnCommands.Checked = true;
                commandsSelected = true;
            }
            else
            {
                btnCommands.Text = "Enable Commands";
                btnCommands.Checked = false;
                commandsSelected = false;
            }
            if (SettingsFile.getSetting(BotClient.getChannel(false), "Points").Equals("true"))
            {
                btnPoints.Text = "Disable Points";
                btnPoints.Checked = true;
                pointsSelected = true;
            }
            else
            {
                btnPoints.Text = "Enable Points";
                btnPoints.Checked = false;
                pointsSelected = false;
            }
            if (SettingsFile.getSetting(BotClient.getChannel(false), "Games").Equals("true"))
            {
                btnGames.Text = "Disable Games";
                btnGames.Checked = true;
                gamesSelected = true;
            }
            else
            {
                btnGames.Text = "Enable Games";
                btnGames.Checked = false;
                gamesSelected = false;
            }
            if (SettingsFile.getSetting(BotClient.getChannel(false), "Announcement").Equals("true"))
            {
                btnAnnouncement.Text = "Disable Announcement";
                btnAnnouncement.Checked = true;
                announcementSelected = true;
            }
            else
            {
                btnAnnouncement.Text = "Enable Announcement";
                btnAnnouncement.Checked = false;
                announcementSelected = false;
            }
            if (SettingsFile.getSetting(BotClient.getChannel(false), "Rank").Equals("true"))
            {
                btnRank.Text = "Disable Ranks";
                btnRank.Checked = true;
                rankSelected = true;
            }
            else
            {
                btnRank.Text = "Enable Ranks";
                btnRank.Checked = false;
                rankSelected = false;
            }
            if (SettingsFile.getSetting(BotClient.getChannel(false), "SilentJoin").Equals("true"))
            {
                btnSlientJoin.Text = "Disable SlientJoin";
                btnSlientJoin.Checked = true;
                slientJoinSelected = true;
            }
            else
            {
                btnSlientJoin.Text = "Enable SlientJoin";
                btnSlientJoin.Checked = false;
                slientJoinSelected = false;
            }
            if (SettingsFile.getSetting(BotClient.getChannel(false), "BadwordsChecker").Equals("true"))
            {
                btnModerationWords.Text = "Disable Moderation Words";
                btnModerationWords.Checked = true;
                wordsSelected = true;
            }
            else
            {
                btnModerationWords.Text = "Enable Moderation Words";
                btnModerationWords.Checked = false;
                wordsSelected = false;
            }
            if (SettingsFile.getSetting(BotClient.getChannel(false), "LinkChecker").Equals("true"))
            {
                btnModerationLinks.Text = "Disable Moderation Links";
                btnModerationLinks.Checked = true;
                linksSelected = true;
            }
            else
            {
                btnModerationLinks.Text = "Enable Moderation Links";
                btnModerationLinks.Checked = false;
                linksSelected = false;
            }
            if (SettingsFile.getSetting(BotClient.getChannel(false), "EmoteChecker").Equals("true"))
            {
                btnModerationEmote.Text = "Disable Moderation Emote";
                btnModerationEmote.Checked = true;
                emotesSelected = true;
            }
            else
            {
                btnModerationEmote.Text = "Enable Moderation Emote";
                btnModerationEmote.Checked = false;
                emotesSelected = false;
            }
            if (SettingsFile.getSetting(BotClient.getChannel(false), "SymbolChecker").Equals("true"))
            {
                btnModerationSymbol.Text = "Disable Moderation Symbol";
                btnModerationSymbol.Checked = true;
                symbolsSelected = true;
            }
            else
            {
                btnModerationSymbol.Text = "Enable Moderation Symbol";
                btnModerationSymbol.Checked = false;
                symbolsSelected = false;
            }
        }

        private void MJRBot_Load(object sender, EventArgs e)
        {
            btnSideTab.Enabled = false;
            if (!Directory.Exists(@"C:\MJR_Bot\"))
                try
                {
                    Directory.CreateDirectory(@"C:\MJR_Bot\");
                    SettingsFile.loadMain();
                }
                catch (Exception ex)
                {
                    BotClient.chatMessages.Add("Unable to create directory!");
                    return;
                }
        }

        private void comboChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            clearSettingTab(false);
            if (comboChannel.Text.Length > 1)
            {
                SettingsFile.load(comboChannel.Text);
                txtAnnouncementDelay.Text = SettingsFile.getSetting(comboChannel.Text, "AnnouncementsDelay");
                txtStartPoints.Text = SettingsFile.getSetting(comboChannel.Text, "StartingPoints");
                txtAutoPoints.Text = SettingsFile.getSetting(comboChannel.Text, "AutoPointsDelay");

                txtAnnouncement1.Text = SettingsFile.getSetting(comboChannel.Text, "Announcement1");
                txtAnnouncement2.Text = SettingsFile.getSetting(comboChannel.Text, "Announcement2");
                txtAnnouncement3.Text = SettingsFile.getSetting(comboChannel.Text, "Announcement3");
                txtAnnouncement4.Text = SettingsFile.getSetting(comboChannel.Text, "Announcement4");
                txtAnnouncement5.Text = SettingsFile.getSetting(comboChannel.Text, "Announcement5");

                txtMaxEmotes.Text = SettingsFile.getSetting(comboChannel.Text, "MaxEmotes");
                txtMaxSymbols.Text = SettingsFile.getSetting(comboChannel.Text, "MaxSymbols");
                txtLanguageMessage.Text = SettingsFile.getSetting(comboChannel.Text, "LanguageWarning");
                txtEmoteMessage.Text = SettingsFile.getSetting(comboChannel.Text, "EmoteWarning");
                txtLinkMessage.Text = SettingsFile.getSetting(comboChannel.Text, "LinkWarning");
                txtSymbolMessage.Text = SettingsFile.getSetting(comboChannel.Text, "SymbolWarning");
            }
        }

        private void tabControlPanel3_Enter(object sender, EventArgs e)
        {
        }
        private void clearSettingTab(bool all)
        {
            if (all)
            {
                comboChannel.Items.Clear();
                txtUsername.Text = "";
                txtPassword.Text = "";
            }
            txtAnnouncementDelay.Text = "";
            txtStartPoints.Text = "";
            txtAutoPoints.Text = "";
            txtAnnouncement1.Text = "";
            txtAnnouncement2.Text = "";
            txtAnnouncement3.Text = "";
            txtAnnouncement4.Text = "";
            txtAnnouncement5.Text = "";
            txtMaxEmotes.Text = "";
            txtMaxSymbols.Text = "";
            txtLanguageMessage.Text = "";
            txtEmoteMessage.Text = "";
            txtLinkMessage.Text = "";
            txtSymbolMessage.Text = "";
        }

        private void tabHome_Click(object sender, EventArgs e)
        {
            btnSideTab.Text = ">>";
            MJRBot.ActiveForm.Width = 725;
            btnSideTab.Checked = false;
            selected = false;
        }

        private void tabIModsandFollowers_Click(object sender, EventArgs e)
        {
            btnSideTab.Text = ">>";
            MJRBot.ActiveForm.Width = 725;
            btnSideTab.Checked = false;
            selected = false;
        }
    }
}
