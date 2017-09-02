// Essentials created by UserR00T & DeathByKorea
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class EssentialsPlugin
{

    #region Folder Locations
    private static string Directory = "Essentials/";

    private static string SettingsFile = Directory + "essentials_settings.txt";
    private static string LanguageBlockFile = Directory + "languageblock.txt";
    private static string ChatBlockFile = Directory + "chatblock.txt";
    private static string AnnouncementsFile = Directory + "announcements.txt";
    private static string IPListFile =  Directory + "ip_list.txt";
    private static string AdminListFile = "admin_list.txt";
    private static string GodListFile = Directory + "godlist.txt";
    private static string AfkListFile = Directory + "afklist.txt";
    private static string MuteListFile = Directory + "mutelist.txt";
    private static string RulesFile = Directory + "rules.txt";
    #endregion

    #region predefining variables

    // General
    private static string version;
    private static bool msgUnknownCommand;
    private static bool ChatBlock;
    private static bool LanguageBlock;
    private static string command;
    private static SvPlayer player;
    private static string msgSayPrefix;
    private static ShPlayer Splayer;


    // Arrays
    private static List<string> ChatBlockWords = new List<string>();
    private static List<string> LanguageBlockWords = new List<string>();
    private static List<string> AdminsListPlayers = new List<string>();

    private static List<string> GodListPlayers = new List<string>();
    private static List<string> AfkPlayers = new List<string>();
    private static List<string> MutePlayers = new List<string>();
    private static string[] rules;
    private static string[] rules2;


    // Messages
    private static string msgNoPerm;

    // Commands
    private static string cmdCommandCharacter;

    private static string cmdClearChat;
    private static string cmdClearChat2;

    private static string cmdSay;
    private static string cmdSay2;

    private static string cmdGodmode;
    private static string cmdGodmode2;

    private static string cmdMute;
    private static string cmdUnMute;

    private static string cmdReload;
    private static string cmdReload2;

    private static string cmdAfk;
    private static string cmdAfk2;

    private static string cmdBan;
    private static string cmdKick;

    private static string cmdRules;

    private static string cmdCheckIP;
    private static string cmdCheckPlayer;

    private static string arg1ClearChat;

    private static int announceIndex = 0;
    private static string[] announcements;
    private static int TimeBetweenAnnounce;
    private static string TimestampFormat;
    private static bool all;
    private static bool unmute; 

    #endregion

    //Code below here, Don't edit unless you know what you're doing.
    //Information about the api @ https://github.com/DeathByKorea/UniversalUnityhooks

    [Hook("SvNetMan.StartServerNetwork")]
    public static void StartServerNetwork(SvNetMan netMan)
    {

        try
        {
            Reload(true);

            Debug.Log("[INFO] Essentials - version: " + version + " Loaded in successfully!");
        }
        catch (Exception ex)
        {
            Debug.Log("[WARNING] Essentials - Settings file does not exist! Make sure to place settings.txt in the game directory!");
            Debug.Log(ex); //remove when debugging is done
        }

        if (!File.Exists(AnnouncementsFile))
        {
            Debug.Log(SetTimeStamp() + "[WARNING] Annoucements file doesn't exist! Please create " + AnnouncementsFile + " in the game directory");
        }
        Thread thread = new Thread(new ParameterizedThreadStart(AnnounceThread));
        thread.Start(netMan);

        Debug.Log(SetTimeStamp() + "[INFO] Announcer started successfully!");
    }

    //Chat Events
    [Hook("SvPlayer.SvGlobalChatMessage")]
    public static bool SvGlobalChatMessage(SvPlayer player, ref string message)
    {
        MessageLog(message, player);
        if (AfkPlayers.Contains(player.playerData.username))
        {
            afk(message, player);
        }
		//Checks if player is muted, if so, cancel message
		if (MutePlayers.Contains(player.playerData.username))
		{
			player.SendToSelf(Channel.Unsequenced, (byte)10, "You are muted.");
			return true;
		}

		// Clear chat command, self and global
		Debug.Log("clearchat");
        if (message.StartsWith(cmdClearChat) || message.StartsWith(cmdClearChat2)){
            if (message.Contains("all") || message.Contains("everyone"))
            {
                all = true;
            }
            ClearChat(message, player, all);
        }
        Debug.Log("reload");
        // Reload command
        if (message.StartsWith(cmdReload) || message.StartsWith(cmdReload2))
        {
            Reload(false, message, player);
            return true;
        }
        Debug.Log("mute");
        if (message.StartsWith(cmdMute) || message.StartsWith(cmdUnMute))
        {
            if (message.StartsWith(cmdUnMute)){
                 unmute = true;
            }

            Mute(message, player, unmute);
            return true;
        }
        Debug.Log("say");
        if (message.StartsWith(cmdSay) || (message.StartsWith(cmdSay2)))
        {
            say(message, player);
            return true;
        }
        //Debug.Log("god");
        //if (message.StartsWith(cmdGodmode) || message.StartsWith(cmdGodmode2))
        //{
        //    godMode(message, player);
        //    return true;
        //}
        if (message.StartsWith(cmdAfk) || message.StartsWith(cmdAfk2))
        {
            afk(message, player);
            return true;
        }
        Debug.Log("essentials");
        // Info command
        if (message.StartsWith("/essentials") || message.StartsWith("/ess"))
        {
            essentials(message, player);
            return true;
        }
        Debug.Log("Rules");
        if (message.StartsWith(cmdRules))
        {
            string arg1 = message.Substring(cmdRules.Length);
            int arg1int;
            bool isNumeric = int.TryParse(arg1, out arg1int);
            if (isNumeric)
            {
                printRules(message, player, arg1int);
                return true;
            }
            else if (!(isNumeric))
            {
                player.SendToSelf(Channel.Unsequenced, (byte)10, "That is not a valid argument.");
                return true;
            }
        }
        //if (message.StartsWith("/info"))
        //{
        //    if (message != "/info")
        //    {
        //        string arg1 = message.Substring("/info".Count() + 1);
        //        CheckInfo(message, player, arg1);
        //        return true;                                  //TODO: Make this work.
        //    }
        //    else
        //    {
        //        player.SendToSelf(Channel.Unsequenced, (byte)10, "A argument is needed for this command.");
        //        return true;
        //    }
        //}
        if (message.StartsWith(cmdCheckIP))
        {
            if (message != cmdCheckIP)
            {
                string arg1 = message.Substring(cmdCheckIP.Count() + 1);
                CheckIP(message, player, arg1);
                return true;
            }
            else
            {
                player.SendToSelf(Channel.Unsequenced, (byte)10, "A argument is needed for this command.");
                return true;
            }
        }
        if (message.StartsWith(cmdCheckPlayer))
        {
            if (message != cmdCheckPlayer)
            {
                string arg1 = message.Substring(cmdCheckPlayer.Count() + 1);
                CheckPlayer(message, player, arg1);
                return true;
            }
            else
            {
                player.SendToSelf(Channel.Unsequenced, (byte)10, "A argument is needed for this command.");
                return true;
            }
        }

        Debug.Log("msgunknown");
        if (message.StartsWith(cmdCommandCharacter))
        {
            if (msgUnknownCommand)
            {
                player.SendToSelf(Channel.Unsequenced, (byte)10, "Unknown command");
                return true;
            }
            else
            {
                return false;
            }
        }

        //Checks if the message is a blocked one, if it is, block it.
        if (ChatBlock || LanguageBlock)
        {
            bool blocked = BlockMessage(message, player);
            if (blocked)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        if (message.StartsWith(cmdAfk) || message.StartsWith(cmdAfk2)){
            afk(message, player);
            return true;
        }

        if (AfkPlayers.Contains(message)){
            player.SendToSelf(Channel.Unsequenced, (byte)10, "That player is AFK.");
            return true;
        }
        return false;

    }


	// These are the various functions for the commands.
	//public static void CheckInfo(string message, object oPlayer, string arg1)
	//{
	//    SvPlayer player = (SvPlayer)oPlayer;
	//    int found = 0;
	//    player.SendToSelf(Channel.Unsequenced, (byte)10, "Info about player: " + arg1);
	//    foreach (var shPlayer in GameObject.FindObjectsOfType<ShPlayer>())
	//    {
	//        if (shPlayer.svPlayer == arg1)                    //TODO: Figure out how this works also
	//        {
	//            if (shPlayer.IsRealPlayer())
	//            {
	//                player.SendToSelf(Channel.Unsequenced, (byte)10, "Username: " + shPlayer.SvPlayer.username);
	//            }
	//        }
	//    }
	//    player.SendToSelf(Channel.Unsequenced, (byte)10, "Info about player: " + arg1);
	//}

	private static void AnnounceThread(object man)
	{
		SvNetMan netMan = (SvNetMan)man;
		while (true)
		{
			foreach (var player in netMan.players)
			{
				player.svPlayer.SendToSelf(Channel.Reliable, ClPacket.GameMessage, announcements[announceIndex]);
			}
		}

		Debug.Log(SetTimeStamp() + "[INFO] Announcement made...");

		announceIndex += 1;
		if (announceIndex > announcements.Length - 1)
			announceIndex = 0;
		Thread.Sleep(TimeBetweenAnnounce * 1000);
	}

    public static void CheckIP(string message, object oPlayer, string arg1)
    {
        SvPlayer player = (SvPlayer)oPlayer;
        int found = 0;
        player.SendToSelf(Channel.Unsequenced, (byte)10, "IP for player: " + arg1);
        foreach (var line in File.ReadAllLines(IPListFile))
        {
            if (line.Contains(arg1) || line.Contains(arg1 + ":"))
            {
                ++found;
                player.SendToSelf(Channel.Unsequenced, (byte)10, line.Substring(arg1.Length + 1));
            }
        }
        player.SendToSelf(Channel.Unsequenced, (byte)10, arg1 + " occurred " + found + " times in the iplog file.");
    }
    public static void CheckPlayer(string message, object oPlayer, string arg1)
    {
        SvPlayer player = (SvPlayer)oPlayer;
        int found = 0;
        string arg1temp = null;
        player.SendToSelf(Channel.Unsequenced, (byte)10, "Accounts for IP: " + arg1);
        foreach (var line in File.ReadAllLines(IPListFile))
        {
            if (line.Contains(arg1) || line.Contains(" " + arg1))
            {
                ++found;
                arg1temp = line.Replace(": " + arg1, " ");
                player.SendToSelf(Channel.Unsequenced, (byte)10, arg1temp);
            }
        }
        player.SendToSelf(Channel.Unsequenced, (byte)10, arg1 + " occurred " + found + " times in the iplog file.");
    }

    public static void MessageLog(string message, object oPlayer)
    {
        SvPlayer player = (SvPlayer)oPlayer;
        if (!message.StartsWith(cmdCommandCharacter))
        {
            Debug.Log(SetTimeStamp() + "[MESSAGE] " + player.playerData.username + ": " + message);
        }
        else if (message.StartsWith(cmdCommandCharacter))
        {
            Debug.Log(SetTimeStamp() + "[COMMAND] " + player.playerData.username + ": " + message);
        }
    }

    public static void printRules(string message, object oPlayer, int pagenumber = 1)
    {
        SvPlayer player = (SvPlayer)oPlayer;


        rules2 = rules.Skip(5 * pagenumber - 5).ToArray();
        int linecnt = rules.Count();
        if (linecnt > 5)
        {
            player.SendToSelf(Channel.Unsequenced, (byte)10, "Rules: Type " + cmdRules + " (pagenumber) for the next page");
        }
        else if (linecnt < 5)
        {
            player.SendToSelf(Channel.Unsequenced, (byte)10, "Rules:");
        }
        int id = 0;
        foreach (string rule in rules2)
        {
            ++id;
            player.SendToSelf(Channel.Unsequenced, (byte)10, rule);
            if (id > 5)
            {
                id = 0;
                break;
            }
        }

    }

    public static bool Mute(string message, object oPlayer, bool unmute)
    {
        SvPlayer player = (SvPlayer)oPlayer;

        string muteuser = message.Split(' ').Last();

        if (AdminsListPlayers.Contains(player.playerData.username))
        {

            if (unmute)
            {
                if (MutePlayers.Contains(muteuser))
                {
                    ReadFile(MuteListFile);
                   MutePlayers = RemoveStringFromFile(MutePlayers, player.playerData.username);
                    player.SendToSelf(Channel.Unsequenced, (byte)10, muteuser + " Unmuted");
                    return true;
                }
                else if(!MutePlayers.Contains(muteuser)){
                    player.SendToSelf(Channel.Unsequenced, (byte)10, muteuser + " is not muted!");
                    return true;

                }
                
            }
            else if  (!unmute) {
                MutePlayers.Add(muteuser);
                player.SendToSelf(Channel.Unsequenced, (byte)10, muteuser + " Muted");
                return true;
            }
        }
        else {
            player.SendToSelf(Channel.Unsequenced, (byte)10, msgNoPerm);
            return false;
        }
        return false;
    }

    public static void afk(string message, object oPlayer){
        SvPlayer player = (SvPlayer)oPlayer;
        ReadFile(AfkListFile);
        if (AfkPlayers.Contains(player.playerData.username))
        {
            ReadFile(AfkListFile);
            AfkPlayers = RemoveStringFromFile(AfkPlayers, player.playerData.username);
            player.SendToSelf(Channel.Unsequenced, (byte)10, "You are no longer AFK");
        }
        else
        {
            AfkPlayers.Add(player.playerData.username);
            player.SendToSelf(Channel.Unsequenced, (byte)10, "You are now AFK");
        }
    }
    //public static void getUsers()
    //{
    //    Users = new List<string>();
    //    foreach (var shPlayer in GameObject.FindObjectsOfType<ShPlayer>()) //TODO: This might be useful for the other things
    //    {
    //        Users.Add(shPlayer);
    //    }
    //}
    public static bool BlockMessage(string message, object oPlayer)
    {
        message = message.ToLower();
        SvPlayer player = (SvPlayer)oPlayer;
        if (ChatBlock == true)
        {
            if (ChatBlockWords.Contains(message))
            {
                player.SendToSelf(Channel.Unsequenced, (byte)10, "Please don't say a blacklisted word, the message has been blocked.");
                Debug.Log(SetTimeStamp() + player.playerData.username + " Said a word that is blocked.");
                return true;
            }
        }
        if (LanguageBlock == true)
        {
            if (LanguageBlockWords.Contains(message))
            {
                if (AdminsListPlayers.Contains(player.playerData.username))
                {
                    player.SendToSelf(Channel.Unsequenced, (byte)10, "Because you are staff, your message has NOT been blocked.");
                    return false;
                }
                else
                {
                    player.SendToSelf(Channel.Unsequenced, (byte)10, "--------------------------------------------------------------------------------------------");
                    player.SendToSelf(Channel.Unsequenced, (byte)10, "             ?olo ingl�s! Tu mensaje ha sido bloqueado.");
                    player.SendToSelf(Channel.Unsequenced, (byte)10, "             Only English! Your message has been blocked.");
                    player.SendToSelf(Channel.Unsequenced, (byte)10, "--------------------------------------------------------------------------------------------");
                    return true;
                }
            }
        }
        return false;
    }

    public static bool godMode(string message, object oPlayer)
    {
        try
        {
        SvPlayer player = (SvPlayer)oPlayer;

            if (AdminsListPlayers.Contains(player.playerData.username))
            {
                if (GodListPlayers.Contains(player.playerData.username))
                {
                    ReadFile(GodListFile);
                 GodListPlayers = RemoveStringFromFile(GodListPlayers, player.playerData.username);
                    player.SendToSelf(Channel.Unsequenced, (byte)10, "Godmode disabled.");
                    return true;
                }
                else
                {
                    GodListPlayers.Add(player.playerData.username);
                    player.SendToSelf(Channel.Unsequenced, (byte)10, "Godmode enabled.");
                    return true;
                }
            }
            else
            {
                player.SendToSelf(Channel.Unsequenced, (byte)10, msgNoPerm);
                return false;
            }

        }
        catch (Exception ex)
        {

            LogExpection(ex, "godMode");
            player.SendToSelf(Channel.Unsequenced, (byte)10, "Unknown error. Check console for more info");
            return true;
        }

    }
    public static void ClearChat(string message, object oPlayer, bool all)
    {
        
        try
        {
            SvPlayer player = (SvPlayer)oPlayer;
            if (!all)
            {
                player.SendToSelf(Channel.Unsequenced, (byte)10, "Clearing the chat for yourself...");
                Thread.Sleep(500);
                for (int i = 0; i < 6; i++)
                {
                    player.SendToSelf(Channel.Unsequenced, (byte)10, " ");
                }
            }
            else if (all)
            {
                if (AdminsListPlayers.Contains(player.playerData.username))
                {
                    //SvPlayer svPlayer = (SvPlayer)player;
                    player.SendToAll(Channel.Unsequenced, (byte)10, "Clearing chat for everyone...");
                    Thread.Sleep(500);
                    for (int i = 0; i < 6; i++)
                    {
                        player.SendToAll(Channel.Unsequenced, (byte)10, " ");
                    }
                }
                else
                {
                    player.SendToSelf(Channel.Unsequenced, (byte)10, msgNoPerm);
                }
            }
            else
            {
                player.SendToSelf(Channel.Unsequenced, (byte)10, @"'" + arg1ClearChat + @"'" + " Is not a valid argument.");
            }
        }
        catch (Exception ex)
        {
            LogExpection(ex, "ClearChat");
            player.SendToSelf(Channel.Unsequenced, (byte)10, "Unknown error occured. Please check output_log.txt for more info.");
        }

    }
    public static void LogExpection(Exception ex, string Sender)
    {
        Debug.Log(SetTimeStamp() + "[ERROR] [" + Sender + "] An unknown error occured. Expection: " + ex.ToString());
        Debug.Log(SetTimeStamp() + "[ERROR] [" + Sender + "] Please post the error on GitHub please!");
        Debug.Log(SetTimeStamp() + "[ERROR] [" + Sender + "] Try reinstalling the newest version.");
    }
    public static void Reload(bool silentExecution, string message = null, object oPlayer = null)
    {
        if (!silentExecution)
        {
            SvPlayer player = (SvPlayer)oPlayer;
            if (AdminsListPlayers.Contains(player.playerData.username))
            {
                player.SendToSelf(Channel.Unsequenced, (byte)10, "Reloading config files...");
                ReadFile(SettingsFile);
                player.SendToSelf(Channel.Unsequenced, (byte)10, "[OK] Config file reloaded");
                player.SendToSelf(Channel.Unsequenced, (byte)10, "Reloading critical .txt files...");

                ReadFileStream(LanguageBlockFile, LanguageBlockWords);
                ReadFileStream(ChatBlockFile, ChatBlockWords);
                ReadFileStream(AdminListFile, AdminsListPlayers);
                LanguageBlockWords = LanguageBlockWords.ConvertAll(d => d.ToLower());
                ChatBlockWords = ChatBlockWords.ConvertAll(d => d.ToLower());
                ReadFile(AnnouncementsFile);
                ReadFile(GodListFile);
                ReadFile(MuteListFile);
                ReadFile(AfkListFile);
                ReadFile(RulesFile);

                player.SendToSelf(Channel.Unsequenced, (byte)10, "[OK] Critical .txt files reloaded");
            }
            else
            {
                player.SendToSelf(Channel.Unsequenced, (byte)10, msgNoPerm);
            }
        }
        else if (silentExecution)
        {
            ReadFile(SettingsFile);

            ReadFileStream(LanguageBlockFile, LanguageBlockWords);
            ReadFileStream(ChatBlockFile, ChatBlockWords);
            ReadFileStream(AdminListFile, AdminsListPlayers);
            LanguageBlockWords = LanguageBlockWords.ConvertAll(d => d.ToLower());
            ChatBlockWords = ChatBlockWords.ConvertAll(d => d.ToLower()); 
            ReadFile(AnnouncementsFile);
            ReadFile(GodListFile);
            ReadFile(MuteListFile);
            ReadFile(AfkListFile);
            ReadFile(RulesFile);
        }
    }

    public static void essentials(string message, object oPlayer)
    {
        SvPlayer player = (SvPlayer)oPlayer;
        player.SendToSelf(Channel.Unsequenced, (byte)10, "Essentials Created by UserR00T & DeathByKorea");
        player.SendToSelf(Channel.Unsequenced, (byte)10, "Version " + version);

        //TODO: Subcommands like /essentials reload : executes cmdReload

    }
    public static bool say(string message, object oPlayer)
    {
        SvPlayer player = (SvPlayer)oPlayer;
        try
        {

            if (AdminsListPlayers.Contains(player.playerData.username))
            {
                if ((message.Length == cmdSay.Length) || (message.Length == cmdSay2.Length))
                {
                    player.SendToSelf(Channel.Unsequenced, (byte)10, "An argument is required for this command.");
                    return true;
                }
                else
                {
                    string arg1 = "blank";
                    if (message.StartsWith(cmdSay))
                    {
                        arg1 = message.Substring(cmdSay.Length);
                    }
                    else if (message.StartsWith(cmdSay2))
                    {
                        arg1 = message.Substring(cmdSay2.Length);
                    }
                    player.SendToAll(Channel.Unsequenced, (byte)10, msgSayPrefix + " " + player.playerData.username + ": " + arg1);
                    return true;
                }
            }
            else
            {
                player.SendToSelf(Channel.Unsequenced, (byte)10, msgNoPerm);
                return false;
            }

        }
        catch (Exception ex)
        {
            LogExpection(ex, "Say");
            player.SendToSelf(Channel.Unsequenced, (byte)10, "Unknown error. Check the log for more info");
            return true;
        }
    }

    [Hook("SvPlayer.Initialize")]
    public static void Initialize(SvPlayer player)
    {
            
        if (player.playerData.username != null)
        {
            Thread thread2 = new Thread(new ParameterizedThreadStart(CheckBanned));
            thread2.Start(player);
            Thread thread = new Thread(new ParameterizedThreadStart(WriteIPToFile));
            thread.Start(player);
        }
    }
    private static void CheckBanned(object oPlayer)
    {
        Thread.Sleep(3000);
        try
        {
            SvPlayer player = (SvPlayer)oPlayer;
            if (File.ReadAllText("ban_list.txt").Contains(player.playerData.username))
            {
                Debug.Log("[WARNING] " + player.playerData.username + " Joined while banned! IP: " + player.netMan.GetAddress(player.connection));
                foreach (var shPlayer in GameObject.FindObjectsOfType<ShPlayer>())
                {
                    if (shPlayer.svPlayer == player)
                    {
                        if (shPlayer.IsRealPlayer())
                        {
                            player.netMan.AddBanned(shPlayer);
                            player.netMan.Disconnect(player.connection);
                        }
                    }
                }


            }
        }
        catch (Exception ex)
        {
            LogExpection(ex, "CheckBanned");
        }
    }
    private static void WriteIPToFile(object oPlayer)
    {
        Thread.Sleep(500);
        SvPlayer player = (SvPlayer)oPlayer;
        Debug.Log(SetTimeStamp() + "[INFO] " + "[JOIN] " + player.playerData.username + " IP is: " + player.netMan.GetAddress(player.connection));
        try
        {
            if (!File.ReadAllText(IPListFile).Contains(player.playerData.username + ": " + player.netMan.GetAddress(player.connection)))
            {
                File.AppendAllText(IPListFile, player.playerData.username + ": " + player.netMan.GetAddress(player.connection) + Environment.NewLine);
            }
        }
        catch (Exception ex)
        {
            LogExpection(ex, "WriteIPToFile");
        }

    }
    public static string RemoveStringFromFile(List<string> content, string RemoveString)
    {
           return content.Remove(RemoveString);
    }
    public static string SetTimeStamp()
    {
        try
        {
            var src = DateTime.Now;
            var hm = new DateTime(src.Year, src.Month, src.Day, src.Hour, src.Minute, src.Second);
            string PlaceHolderText = TimestampFormat;
            string Hours = hm.ToString("HH");
            string Minutes = hm.ToString("mm");
            string Seconds = hm.ToString("ss");

            if (TimestampFormat.Contains("{H}"))
            {
                PlaceHolderText = PlaceHolderText.Replace("{H}", hm.ToString("HH"));
            }
            if (TimestampFormat.Contains("{h}"))
            {
                PlaceHolderText = PlaceHolderText.Replace("{h}", hm.ToString("hh"));
            }
            if (TimestampFormat.Contains("{M}") || TimestampFormat.Contains("{m}"))
            {
                PlaceHolderText = PlaceHolderText.Replace("{M}", Minutes);
            }
            if (TimestampFormat.Contains("{S}") || TimestampFormat.Contains("{s}"))
            {
                PlaceHolderText = PlaceHolderText.Replace("{S}", Seconds.ToString());
            }

            PlaceHolderText = PlaceHolderText + " ";
            return PlaceHolderText;
        }
        catch (Exception)
        {
            return "[Failed] ";
        }
    }
    public static void ReadFileStream(string FileName, List<string> output)
    {
        foreach (var line in File.ReadAllLines(FileName))
        {
            if (line.StartsWith("#"))
            {
                continue;
            }
            else
            {
                output.Add(line);
            }
        }
    }
    static void ReadFile(string FileName)
    {
        #region SettingsFile
        if (FileName == SettingsFile)
        {
            foreach (var line in File.ReadAllLines(SettingsFile)) //TODO: THIS METHOD IS A MESS, THERE IS PROBABLY A BETTER WAY.
            {
                if (line.StartsWith("#"))
                {
                    continue;
                }
                else
                {
                    if (line.Contains("version: "))
                    {
                        version = line.Substring(9);
                    }
                    else if (line.Contains("CommandCharacter: "))
                    {
                        cmdCommandCharacter = line.Substring(18);
                    }
                    else if (line.Contains("noperm: "))
                    {
                        msgNoPerm = line.Substring(8);
                    }
                    else if (line.Contains("ClearChatCommand: "))
                    {
                        cmdClearChat = cmdCommandCharacter + line.Substring(18);
                    }
                    else if (line.Contains("ClearChatCommand2: "))
                    {
                        cmdClearChat2 = cmdCommandCharacter + line.Substring(19);
                    }
                    else if (line.Contains("SayCommand: "))
                    {
                        cmdSay = cmdCommandCharacter + line.Substring(12);
                    }
                    else if (line.Contains("SayCommand2:"))
                    {
                        cmdSay2 = cmdCommandCharacter + line.Substring(13);
                    }
                    else if (line.Contains("msgSayPrefix: "))
                    {
                        msgSayPrefix = line.Substring(14);
                    }
                    else if (line.Contains("GodmodeCommand: "))
                    {
                        cmdGodmode = cmdCommandCharacter + line.Substring(16);
                    }
                    else if (line.Contains("GodmodeCommand2: "))
                    {
                        cmdGodmode2 = cmdCommandCharacter + line.Substring(17);
                    }
                    else if (line.StartsWith("MuteCommand: "))
                    {
                        cmdMute = cmdCommandCharacter + line.Substring(13);
                    }
                    else if (line.StartsWith("UnMuteCommand: "))
                    {
                        cmdUnMute = cmdCommandCharacter + line.Substring(15);
                    }
                    else if (line.Contains("UnknownCommand: "))
                    {
                        msgUnknownCommand = Convert.ToBoolean(line.Substring(16));
                    }
                    else if (line.Contains("enableChatBlock: "))
                    {
                        ChatBlock = Convert.ToBoolean(line.Substring(17));
                    }
                    else if (line.Contains("enableLanguageBlock: "))
                    {
                        LanguageBlock = Convert.ToBoolean(line.Substring(21));
                    }
                    else if (line.Contains("RulesCommand: "))
                    {
                        cmdRules = cmdCommandCharacter + line.Substring(14);
                    }
                    else if (line.Contains("ReloadCommand: "))
                    {
                        cmdReload = cmdCommandCharacter + line.Substring(15);
                    }
                    else if (line.Contains("ReloadCommand2: "))
                    {
                        cmdReload2 = cmdCommandCharacter + line.Substring(16);
                    }
                    else if (line.Contains("TimeBetweenAnnounce: "))
                    {
                        TimeBetweenAnnounce = Int32.Parse(line.Substring(21));
                    }
                    else if (line.Contains("TimestapFormat: "))
                    {
                        TimestampFormat = line.Substring(16);
                    }
                    else if (line.Contains("AFKCommand: "))
                    {
                        cmdAfk = cmdCommandCharacter + line.Substring(12);
                    }
                    else if (line.Contains("AFKCommand2: "))
                    {
                        cmdAfk2 = cmdCommandCharacter + line.Substring(13);
                    }
                    else if (line.Contains("BanCommand: "))
                    {
                        cmdBan = cmdCommandCharacter + line.Substring(12);
                    }
                    else if (line.Contains("KickCommand: "))
                    {
                        cmdKick = cmdCommandCharacter + line.Substring(13);
                    }

                    else if (line.Contains("CheckIPCommand: "))
                    {
                        cmdCheckIP = cmdCommandCharacter + line.Substring(16);
                    }

                    else if (line.Contains("CheckPlayerCommand: "))
                    {
                        cmdCheckPlayer = cmdCommandCharacter + line.Substring(20);
                    }

                }

                //else if (line.Contains(""))
                //{
                //    = line.Substring(); //Template
                //}

            }
        }
        #endregion
        else if (FileName == AnnouncementsFile)
        {
            announcements = File.ReadAllLines(FileName);
        }
        else if (FileName == RulesFile)
        {
            rules = File.ReadAllLines(FileName);
        }
        else if (FileName == GodListFile)
        {
            GodListPlayers = File.ReadAllLines(FileName).ToList();
        }
        else if (FileName == AfkListFile)
        {
            AfkPlayers = File.ReadAllLines(FileName).ToList();
        }
        else if (FileName == MuteListFile)
        {
            MutePlayers = File.ReadAllLines(FileName).ToList();
        }

    }
}