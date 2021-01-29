using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

/*
    ; ###########################################################################################################################
    ; # Samp/Gta Functions:                                                                                                     #
    ; #     - IsInChat();                               [check if dialog or chatbox is active ]                                 #
    ; #     - GetUsername();                            [get local player name]                                                 #
    ; #     - GetServerIP();                            [get local player id]                                                   #
    ; #     - GetServerHostname();                      [get server hostname]                                                   #
    ; #     - IsCheckpointActive();                     [check if has a checkpoint active]                                      #
    ; #     - IsRaceCheckpointActive();                 [check if has a race-checkpoint active]                                 #
    ; #     - GetLastChatMessage();                     [get last chatbox message]                                              #
    ; #     - BlockChatInput();                         [NOP player to send message]                                            #
    ; #     - UnblockChatInput();                       [UN-NOP player to send message]                                         #
    ; ###########################################################################################################################
    ; # Player Functions:                                                                                                       #
    ; #     - GetPlayerHealth();                        [get local player health]                                               #
    ; #     - SetPlayerHealth(int wValue);              [set local player health]                                               #
    ; #     - GetPlayerArmor();                         [get local player armor]                                                #
    ; #     - SetPlayerArmor(int wValue);               [set local player armor]                                                #
    ; #     - GetPlayerInteriorId();                    [get local player interior id]                                          #
    ; #     - SetPlayerInteriorId(int wValue);          [set local player interior id]                                          #
    ; #     - GetPlayerMoney();                         [get local player money]                                                #
    ; #     - SetPlayerMoney(int wValue);               [set local player money]                                                #
    ; ###########################################################################################################################
    ; # Vehicle Functions:                                                                                                      #
    ; #     - IsPlayerInAnyVehicle();                   [check if local player is in vehicle]                                   #
    ; #     - IsPlayerDriver();                         [check if local player is driving]                                      #
    ; #     - GetVehicleHealth();                       [get local player vehicle health]                                       #
    ; #     - SetVehicleHealth();                       [set local player vehicle health]                                       #
    ; #     - GetVehicleType();                         [get local player vehicle type]                                         #
    ; #     - GetVehicleModelId();                      [get local player vehicle model id]                                     #
    ; #     - GetVehicleModelName();                    [get local player vehicle model name]                                   #
    ; #     - IsVehicleEngineStateON();                 [check if local player vehicle is turned on]                            #
    ; #     - IsVehicleLocked();                        [check if local player vehicle is locked]                               #
    ; ###########################################################################################################################
    ; # Coordinates:                                                                                                            #
    ; #     - GetCoordinates();                         [get local player coordinates]                                          #
    ; #     - GetCheckpointCoordinates();               [get local player checkpoint coordinates]                               #
    ; ###########################################################################################################################
    ; # Misc:                                                                                                                   #
    ; #     - SetVirtualKey(int KeyOffset);             [send a key press to gta]                                               #
    ; #     - SetNOP(SampNOPS offset, out int oldValue);[write a NOP in memory]                                                 #
    ; ###########################################################################################################################
    ; # Internal Stuff:                                                                                                         #
    ; # ------------------------------------------------------------------------------------------------------------------------#
    ; # - Default Invokes (Internal Stuff):                                                                                     #
    ; #     - OpenProcess(int dwDesiredAccess,IntPtr bInheritHandle,IntPtr dwProcessId);                                        #
    ; #     - ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, int bytesRead);       #
    ; #     - WriteProcessMemory(IntPtr hProcess,IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out IntPtr BytesWritten);   #   
    ; #     - CloseHandle(IntPtr Handle);                                                                                       #   
    ; #     - VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, int dwFreeType);                                     #
    ; #     - VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, int flAllocationType, int flProtect);              #
    ; #     - WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);                                                       #
    ; # ------------------------------------------------------------------------------------------------------------------------#
    ; # - Internal Functions (using invokes):
    ; #     - GetProcessHandle(processName);                                                                                    #
    ; #     - GetModuleBaseAddress(string processName, string moduleName);                                                      #
    ; #     - GetModuleBaseAddress(string processName, string moduleName);                                                      #
    ; #     - ReadDWORD(IntPtr hProcess, uint Address);                                                                         #
    ; #     - ReadFloat(IntPtr hProcess, uint Address);                                                                         #
    ; #     - ReadString(IntPtr hProcess, uint Address, int szString);                                                          #
    ; #     - WriteDWORD(IntPtr hProcess, int bAddress, int wDWORD);                                                            #
    ; #     - WriteFloat(IntPtr hProcess, int bAddress, float wFloat);                                                          #
    ; #     - WriteString(IntPtr hProcess, int Address, int wString);                                                           #
    ; #     - ReadMem(IntPtr hProcess, int dwAddress, int dwLen, string type);                                                  #
    ; #     - checkHandles();                                                                                                   #
    ; #     - RefreshGTA();                                                                                                     #
    ; #     - RefreshSAMP();                                                                                                    #
    ; #     - RefreshMemory();                                                                                                  #
    ; ###########################################################################################################################
*/


public class Samp
{
    #region Enums
    [Flags]
    public enum ProcessAccessFlags : uint
    {
        All = 0x001F0FFF,
        Terminate = 0x00000001,
        CreateThread = 0x00000002,
        VMOperation = 0x00000008,
        VMRead = 0x00000010,
        VMWrite = 0x00000020,
        DupHandle = 0x00000040,
        SetInformation = 0x00000200,
        QueryInformation = 0x00000400,
        Synchronize = 0x00100000
    }

    public enum GtaAddresses : uint
    {
        ADDR_ZONECODE = 0xA49AD4,
        ADDR_POSITION_X = 0xB6F2E4,
        ADDR_POSITION_Y = 0xB6F2E8,
        ADDR_POSITION_Z = 0xB6F2EC,
        ADDR_CPED_PTR = 0xB6F5F0,
        ADDR_CPED_HPOFF = 0x540,
        ADDR_CPED_ARMOROFF = 0x548,
        ADDR_VEHICLE_PTR = 0xBA18FC,
        ADDR_VEHICLE_HPOFF = 0x4C0,
        ADDR_CPED_MONEY = 0xB7CE50,
        ADDR_CPED_INTID = 0xA4ACE8,
        ADDR_VEHICLE_DOORSTATE = 0x4F8,
        ADDR_VEHICLE_ENGINESTATE = 0x428,
        ADDR_VEHICLE_LIGHTSTATE = 0x584,
        ADDR_VEHICLE_MODEL = 0x22,
        ADDR_VEHICLE_TYPE = 0x590,
        ADDR_VEHICLE_DRIVER = 0x460,
        ADDR_CHECKPOINT_PTR = 0x21A10C,
        ADDR_CHECKPOINT_ACTIVE_OFF = 0x24,
        ADDR_RACE_CP_ACTIVE_OFF = 0x49,
        ADDR_CHECKPOINT_OFF_X = 0xC,
        ADDR_CHECKPOINT_OFF_Y = 0x4,
        ADDR_CHECKPOINT_OFF_Z = 0x4,
        ADDR_VIRTUALKEYS_PTR = 0xB72CC8
    }
    public enum SampAddresses : uint
    {
        ADDR_SAMP_INCHAT_PTR = 0x21A10C,
        ADDR_SAMP_INCHAT_PTR_OFF = 0x55,
        ADDR_SAMP_CHATMSG_PTR = 0x21A0E4,
        ADDR_SAMP_USERNAME = 0x219A6F,
        ADDR_SAMP_SHOWDLG_PTR = 0x21A0B8,
        FUNC_SAMP_SENDCMD = 0x65C60,
        FUNC_SAMP_SENDSAY = 0x57F0,
        FUNC_SAMP_ADDTOCHATWND = 0x64520,
        FUNC_SAMP_SHOWGAMETEXT = 0x9C2C0,
        FUNC_SAMP_PLAYAUDIOSTR = 0x62DA0,
        FUNC_SAMP_STOPAUDIOSTR = 0x629A0,
        FUNC_SAMP_SHOWDIALOG = 0x6B9C0,
        FUNC_UPDATESCOREBOARD = 0x8A10
    }

    public enum SampOffsets : uint
    {
        ADDR_SAMP_INCHAT_PTR = 0x21A10C,
        ADDR_SAMP_INCHAT_PTR_OFF = 0x55,
        ADDR_SAMP_USERNAME = 0x219A6F,
        ADDR_SAMP_CHATMSG_PTR = 0x21A0E4,
        ADDR_SAMP_SHOWDLG_PTR = 0x21A0B8,
        FUNC_SAMP_SENDCMD = 0x65C60,
        FUNC_SAMP_SENDSAY = 0x57F0,
        FUNC_SAMP_ADDTOCHATWND = 0x64520,
        FUNC_SAMP_SHOWGAMETEXT = 0x9C2C0,
        FUNC_SAMP_PLAYAUDIOSTR = 0x62DA0,
        FUNC_SAMP_STOPAUDIOSTR = 0x629A0,
        FUNC_SAMP_SHOWDIALOG = 0x6B9C0,
        FUNC_UPDATESCOREBOARD = 0x8A10,
        SAMP_LAST_CHAT_MESSAGE_OFFSET = 0x62C6,

        SAMP_INFO_OFFSET = 0x21A0F8,
        SAMP_PPOOLS_OFFSET = 0x3CD,
        SAMP_PPOOL_PLAYER_OFFSET = 0x18,
        SAMP_SLOCALPLAYERID_OFFSET = 0x4,
        SAMP_ISTRLEN_LOCALPLAYERNAME_OFFSET = 0x1A,
        SAMP_SZLOCALPLAYERNAME_OFFSET = 0xA,
        SAMP_PSZLOCALPLAYERNAME_OFFSET = 0xA,
        SAMP_PREMOTEPLAYER_OFFSET = 0x2E,
        SAMP_ISTRLENNAME___OFFSET = 0x1C,
        SAMP_SZPLAYERNAME_OFFSET = 0xC,
        SAMP_PSZPLAYERNAME_OFFSET = 0xC,
        SAMP_ILOCALPLAYERPING_OFFSET = 0x26,
        SAMP_ILOCALPLAYERSCORE_OFFSET = 0x2A,
        SAMP_IPING_OFFSET = 0x28,
        SAMP_ISCORE_OFFSET = 0x24,
        SAMP_ISNPC_OFFSET = 0x4,
        SAMP_SZIP_OFFSET = 0x20,
        SAMP_SZHOSTNAME_OFFSET = 0x121
    }

    public enum SampNOPS : uint
    {
        DESACTIVE_VALUE = 0xC390,
        NOP_SETPLAYERNAME = 0X01A4F0,
        NOP_SETPLAYERPOS = 0X015970,
        NOP_SETPLAYERPOSFINDZ = 0X015A90,
        NOP_SETPLAYERHEALTH = 0X015BA0,
        NOP_TOGGLEPLAYERCONTROLLABLE = 0X0168E0,
        NOP_PLAYSOUND = 0X016980,
        NOP_SETPLAYERWORLDBOUNDS = 0X016A60,
        NOP_GIVEPLAYERMONEY = 0X016B50,
        NOP_SETPLAYERFACINGANGLE = 0X016BF0,
        NOP_RESETPLAYERMONEY = 0X014780,
        NOP_RESETPLAYERWEAPONS = 0X014790,
        NOP_GIVEPLAYERWEAPON = 0X016C90,
        NOP_SETVEHICLEPARAMSEX = 0X00E370,
        NOP_ENTERVEHICLE = 0X00E650,
        NOP_ENTEREDITOBJECT = 0X00BA90,
        NOP_CANCELEDIT = 0X00BB50,
        NOP_SETPLAYERTIME = 0X00C4E0,
        NOP_TOGGLECLOCK = 0X00C5C0,
        NOP_WORLDPLAYERADD = 0X00DBB0,
        NOP_SETPLAYERSHOPNAME = 0X014540,
        NOP_SETPLAYERSKILLLEVEL = 0X00C6A0,
        NOP_SETPLAYERDRUNKLEVEL = 0X015490,
        NOP_CREATE3DTEXTLABEL = 0X00C7D0,
        NOP_DISABLECHECKPOINT = 0X00B780,
        NOP_SETRACECHECKPOINT = 0X00D330,
        NOP_DISABLERACECHECKPOINT = 0X00B790,
        NOP_GAMEMODERESTART = 0X00B830,
        NOP_PLAYAUDIOSTREAM = 0X019990,
        NOP_STOPAUDIOSTREAM = 0X0147E0,
        NOP_REMOVEBUILDINGFORPLAYER = 0X019B00,
        NOP_CREATEOBJECT = 0X017980,
        NOP_SETOBJECTPOS = 0X018050,
        NOP_SETOBJECTROT = 0X018160,
        NOP_DESTROYOBJECT = 0X018260,
        NOP_DEATHMESSAGE = 0X01A290,
        NOP_SETPLAYERMAPICON = 0X016DE0,
        NOP_REMOVEVEHICLECOMPONENT = 0X018C00,
        NOP_UPDATE3DTEXTLABEL = 0X00C980,
        NOP_CHATBUBBLE = 0X00CA40,
        NOP_UPDATESYSTEMTIME = 0X00CFE0,
        NOP_SHOWDIALOG = 0X00CBB0,
        NOP_DESTROYPICKUP = 0X00C200,
        NOP_WEAPONPICKUPDESTROY = 0X016D50,
        NOP_LINKVEHICLETOINTERIOR = 0X016580,
        NOP_SETPLAYERARMOUR = 0X0171A0,
        NOP_SETPLAYERARMEDWEAPON = 0X015530,
        NOP_SETSPAWNINFO = 0X014640,
        NOP_SETPLAYERTEAM = 0X015D60,
        NOP_PUTPLAYERINVEHICLE = 0X015C50,
        NOP_REMOVEPLAYERFROMVEHICLE = 0X0146E0,
        NOP_SETPLAYERCOLOR = 0X015E50,
        NOP_DISPLAYGAMETEXT = 0X015F40,
        NOP_FORCECLASSSELECTION = 0X0147C0,
        NOP_ATTACHOBJECTTOPLAYER = 0X018CE0,
        NOP_INITMENU = 0X018EA0,
        NOP_SHOWMENU = 0X019160,
        NOP_HIDEMENU = 0X019210,
        NOP_CREATEEXPLOSION = 0X018350,
        NOP_SHOWPLAYERNAMETAGFORPLAYER = 0X018460,
        NOP_ATTACHCAMERATOOBJECT = 0X016640,
        NOP_INTERPOLATECAMERA = 0X016740,
        NOP_CLICKTEXTDRAW = 0X019C20,
        NOP_SETOBJECTMATERIAL = 0X017CE0,
        NOP_GANGZONESTOPFLASH = 0X0198E0,
        NOP_APPLYANIMATION = 0X016FA0,
        NOP_CLEARANIMATIONS = 0X014C70,
        NOP_SETPLAYERSPECIALACTION = 0X014D80,
        NOP_SETPLAYERFIGHTINGSTYLE = 0X014E30,
        NOP_SETPLAYERVELOCITY = 0X014F30,
        NOP_SETVEHICLEVELOCITY = 0X015030,
        NOP_SETPLAYERDRUNKVISUALS = 0X015330,
        NOP_CLIENTMESSAGE = 0X00C050,
        NOP_SETWORLDTIME = 0X00BFB0,
        NOP_CREATEPICKUP = 0X00C140,
        NOP_SCMEVENT = 0X00C340,
        NOP_SETVEHICLETIRESTATUS = 0X015250,
        NOP_MOVEOBJECT = 0X018540,
        NOP_CHAT = 0X00EEA0,
        NOP_SRVNETSTATS = 0X00B7A0,
        NOP_CLIENTCHECK = 0X00EAF0,
        NOP_ENABLESTUNTBONUSFORPLAYER = 0X014440,
        NOP_TEXTDRAWSETSTRING = 0X019540,
        NOP_DAMAGEVEHICLE = 0X00E240,
        NOP_SETCHECKPOINT = 0X00D220,
        NOP_GANGZONECREATE = 0X019650,
        NOP_PLAYCRIMEREPORT = 0X015720,
        NOP_SETPLAYERATTACHEDOBJECT = 0X0155E0,
        NOP_EDITATTACHEDOBJECT = 0X00E860,
        NOP_EDITOBJECT = 0X00E920,
        NOP_GANGZONEDESTROY = 0X019770,
        NOP_GANGZONEFLASH = 0X019820,
        NOP_STOPOBJECT = 0X0186F0,
        NOP_SETNUMBERPLATE = 0X018870,
        NOP_TOGGLEPLAYERSPECTATING = 0X018990,
        NOP_PLAYERSPECTATEPLAYER = 0X018A40,
        NOP_PLAYERSPECTATEVEHICLE = 0X018B20,
        NOP_REQUESTCLASS = 0X00D080,
        NOP_REQUESTSPAWN = 0X00D150,
        NOP_SETPLAYERWANTEDLEVEL = 0X0192C0,
        NOP_SHOWTEXTDRAW = 0X019360,
        NOP_TEXTDRAWHIDEFORPLAYER = 0X019490,
        NOP_SERVERJOIN = 0X00CDA0,
        NOP_SERVERQUIT = 0X00CF20,
        NOP_INITGAME = 0X00D710,
        NOP_REMOVEPLAYERMAPICON = 0X016F00,
        NOP_SETPLAYERAMMO = 0X017250,
        NOP_SETPLAYERGRAVITY = 0X017310,
        NOP_SETVEHICLEHEALTH = 0X0173B0,
        NOP_ATTACHTRAILERTOVEHICLE = 0X017490,
        NOP_DETACHTRAILERFROMVEHICLE = 0X0175D0,
        NOP_SETPLAYERDRUNKHANDLING = 0X0153E0,
        NOP_DESTROYPICKUPS = 0X00C2A0,
        NOP_SETWEATHER = 0X00C430,
        NOP_SETPLAYERSKIN = 0X015860,
        NOP_EXITVEHICLE = 0X00E770,
        NOP_UPDATESCORESPINGSIPS = 0X00D490,
        NOP_SETPLAYERINTERIOR = 0X016050,
        NOP_SETPLAYERCAMERAPOS = 0X0160F0,
        NOP_SETPLAYERCAMERALOOKAT = 0X0161C0,
        NOP_SETVEHICLEPOS = 0X0162C0,
        NOP_SETVEHICLEZANGLE = 0X0163D0,
        NOP_SETVEHICLEPARAMSFORPLAYER = 0X0164B0,
        NOP_SETCAMERABEHINDPLAYER = 0X014770,
        NOP_WORLDPLAYERREMOVE = 0X00DEA0,
        NOP_WORLDVEHICLEADD = 0X00B850,
        NOP_WORLDVEHICLEREMOVE = 0X00DF70,
        NOP_WORLDPLAYERDEATH = 0X00DDE0
    }
    #endregion

    #region  Variables
    public static Int32 iRefreshHandles = 0;
    public static IntPtr hGTA = IntPtr.Zero;
    public static IntPtr dwSAMP = IntPtr.Zero;
    public static IntPtr pMemory = IntPtr.Zero;
    public static IntPtr pParam1 = IntPtr.Zero;
    public static IntPtr pParam2 = IntPtr.Zero;
    public static IntPtr pParam3 = IntPtr.Zero;
    public static IntPtr pParam4 = IntPtr.Zero;
    public static IntPtr pParam5 = IntPtr.Zero;
    public static IntPtr pInjectFunc = IntPtr.Zero;

    public static int[] oAirplaneModels = { 417, 425, 447, 460, 469, 476, 487, 488, 497, 511, 512, 513, 519, 520, 548, 553, 563, 577, 592, 593 };
    public static int[] oBikeModels = { 481, 509, 510 };
    public static string[] ovehicleNames = { "Landstalker", "Bravura", "Buffalo", "Linerunner", "Perrenial", "Sentinel", "Dumper", "Firetruck", "Trashmaster", "Stretch", "Manana", "Infernus", "Voodoo", "Pony", "Mule", "Cheetah", "Ambulance", "Leviathan", "Moonbeam", "Esperanto", "Taxi", "Washington", "Bobcat", "Whoopee", "BFInjection", "Hunter", "Premier", "Enforcer", "Securicar", "Banshee", "Predator", "Bus", "Rhino", "Barracks", "Hotknife", "Trailer", "Previon", "Coach", "Cabbie", "Stallion", "Rumpo", "RCBandit", "Romero", "Packer", "Monster", "Admiral", "Squalo", "Seasparrow", "Pizzaboy", "Tram", "Trailer", "Turismo", "Speeder", "Reefer", "Tropic", "Flatbed", "Yankee", "Caddy", "Solair", "Berkley'sRCVan", "Skimmer", "PCJ-600", "Faggio", "Freeway", "RCBaron", "RCRaider", "Glendale", "Oceanic", "Sanchez", "Sparrow", "Patriot", "Quad", "Coastguard", "Dinghy", "Hermes", "Sabre", "Rustler", "ZR-350", "Walton", "Regina", "Comet", "BMX", "Burrito", "Camper", "Marquis", "Baggage", "Dozer", "Maverick", "NewsChopper", "Rancher", "FBIRancher", "Virgo", "Greenwood", "Jetmax", "Hotring", "Sandking", "BlistaCompact", "PoliceMaverick", "Boxvillde", "Benson", "Mesa", "RCGoblin", "HotringRacerA", "HotringRacerB", "BloodringBanger", "Rancher", "SuperGT", "Elegant", "Journey", "Bike", "MountainBike", "Beagle", "Cropduster", "Stunt", "Tanker", "Roadtrain", "Nebula", "Majestic", "Buccaneer", "Shamal", "hydra", "FCR-900", "NRG-500", "HPV1000", "CementTruck", "TowTruck", "Fortune", "Cadrona", "FBITruck", "Willard", "Forklift", "Tractor", "Combine", "Feltzer", "Remington", "Slamvan", "Blade", "Freight", "Streak", "Vortex", "Vincent", "Bullet", "Clover", "Sadler", "Firetruck", "Hustler", "Intruder", "Primo", "Cargobob", "Tampa", "Sunrise", "Merit", "Utility", "Nevada", "Yosemite", "Windsor", "Monster", "Monster", "Uranus", "Jester", "Sultan", "Stratum", "Elegy", "Raindance", "RCTiger", "Flash", "Tahoma", "Savanna", "Bandito", "FreightFlat", "StreakCarriage", "Kart", "Mower", "Dune", "Sweeper", "Broadway", "Tornado", "AT-400", "DFT-30", "Huntley", "Stafford", "BF-400", "NewsVan", "Tug", "Trailer", "Emperor", "Wayfarer", "Euros", "Hotdog", "Club", "FreightBox", "Trailer", "Andromada", "Dodo", "RCCam", "Launch", "PoliceCar", "PoliceCar", "PoliceCar", "PoliceRanger", "Picador", "S.W.A.T", "Alpha", "Phoenix", "GlendaleShit", "SadlerShit", "Luggage", "Luggage", "Stairs", "Boxville", "Tiller", "UtilityTrailer" };

    #endregion

    #region Kernel32 Invokes
    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(
        int dwDesiredAccess,
        IntPtr bInheritHandle,
        IntPtr dwProcessId
    );

    [DllImport("kernel32", SetLastError = true)]
    static extern bool ReadProcessMemory(
        IntPtr hProcess,
        IntPtr lpBaseAddress,
        [Out] byte[] lpBuffer,
        int dwSize,
        int bytesRead
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool WriteProcessMemory(IntPtr hProcess,
        IntPtr lpBaseAddress,
        byte[] lpBuffer,
        int dwSize,
        out IntPtr
        lpNumberOfBytesWritten
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool CloseHandle(IntPtr hProcess);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool VirtualFreeEx(IntPtr hProcess,
        IntPtr
        lpAddress,
        int dwSize, int dwFreeType
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
    uint dwSize, int flAllocationType, int flProtect);

    [DllImport("kernel32.dll")]
    static extern IntPtr CreateRemoteThread(IntPtr hProcess,
       IntPtr lpThreadAttributes,
       uint dwStackSize,
       IntPtr lpStartAddress,
       IntPtr lpParameter,
       uint dwCreationFlags,
       out IntPtr lpThreadId
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
    #endregion

    public static void a()
    {
        //string ip = GetServerIP();
        //bool inChat = IsInChat();
        //string username = GetUsername();
        //string hostname = GetServerHostname();
        //int health = GetPlayerHealth();
        //int armor = GetPlayerArmor();
        //int interiorId = GetPlayerInteriorId();
        //int money = GetPlayerMoney();
        //bool isPlayerInVehicle = IsPlayerInAnyVehicle();
        //bool isPlayerDriver = IsPlayerDriver();
        //float carHealth = GetVehicleHealth();
        //int vType = GetVehicleType();
        //int model = GetVehicleModelId();
        //string vehicleName = GetVehicleModelName();
        //bool vehicleOn = IsVehicleEngineStateON();
        //float[] Coords = GetCoordinates();
        //bool cpActive = IsCheckpointActive();
        //bool raceCpActive = IsRaceCheckpointActive();
        //float[] cpCoords = GetCheckpointCoordinates();
        //SetPlayerHealth(10);
        //SetPlayerArmor(10);
        //SetPlayerMoney(500);
        //string lastMsg = GetLastChatMessage();
        //int oldNOPValue = 0;
        //bool ok = SetNOP(SampNOPS.NOP_SETPLAYERNAME, out oldNOPValue, true, 663107305);
        //Debug.WriteLine();
    }

    #region Samp/Gta Functions
    //Check if local player has chat window opened
    public static bool IsInChat()
    {
        if (!checkHandles()) return false;
        int dwPtr = (int)dwSAMP + (int)SampAddresses.ADDR_SAMP_INCHAT_PTR;
        int dwAddress = ReadDWORD(hGTA, dwPtr) + (int)SampAddresses.ADDR_SAMP_INCHAT_PTR_OFF;
        int dwInChat = ReadDWORD(hGTA, dwAddress);
        return (dwInChat > 0);
    }

    //Get local player name or empty string
    public static string GetUsername()
    {
        if (!checkHandles()) return "";
        int dwAddress = (int)dwSAMP + (int)SampAddresses.ADDR_SAMP_USERNAME;
        string sUsername = ReadString(hGTA, dwAddress, 25);
        return sUsername;

    }

    //Get server ip or empty string
    public static string GetServerIP()
    {
        if (!checkHandles()) return "";
        int dwAddress = ReadDWORD(hGTA, (int)dwSAMP + (int)SampOffsets.SAMP_INFO_OFFSET);
        string ipAddress = ReadString(hGTA, dwAddress + (int)SampOffsets.SAMP_SZIP_OFFSET, 257);
        return ipAddress;
    }

    //Get server hostname or empty string
    public static string GetServerHostname()
    {
        if (!checkHandles()) return "";
        int dwPtr = (int)dwSAMP + (int)SampOffsets.SAMP_INFO_OFFSET;
        int dwAddress = ReadDWORD(hGTA, dwPtr) + (int)SampOffsets.SAMP_SZHOSTNAME_OFFSET;
        string hostname = ReadString(hGTA, dwAddress, 259);
        return hostname;
    }

    //Check if local player has a activated checkpoint
    public static bool IsCheckpointActive()
    {
        if (!checkHandles()) return false;
        int dwPtr = (int)dwSAMP + (int)GtaAddresses.ADDR_CHECKPOINT_PTR;
        int dwAddress = ReadDWORD(hGTA, dwPtr) + (int)GtaAddresses.ADDR_CHECKPOINT_ACTIVE_OFF;
        int val = ReadDWORD(hGTA, dwAddress);
        return val > 0;
    }

    //Check if local player has a activated race checkpoint
    public static bool IsRaceCheckpointActive()
    {
        if (!checkHandles()) return false;
        int dwPtr = (int)dwSAMP + (int)GtaAddresses.ADDR_CHECKPOINT_PTR;
        int dwAddress = ReadDWORD(hGTA, dwPtr) + (int)GtaAddresses.ADDR_RACE_CP_ACTIVE_OFF;
        int val = ReadDWORD(hGTA, dwAddress);
        return val > 0;
    }

    //Returns last message string
    public static string GetLastChatMessage()
    {
        if (!checkHandles()) return "";

        int dwPtr = (int)dwSAMP + (int)SampAddresses.ADDR_SAMP_CHATMSG_PTR;

        int dwAddr = ReadDWORD(hGTA, dwPtr) + (int)SampOffsets.SAMP_LAST_CHAT_MESSAGE_OFFSET;
        string msg = ReadString(hGTA, dwAddr, 128);
        return msg;
    }

    //NOP - block raknet client and server communication - CHAT
    public static bool BlockChatInput()
    {
        if (!checkHandles()) return false;
        byte[] nop = new byte[2];
        int dwFunc = (int)dwSAMP + (int)SampAddresses.FUNC_SAMP_SENDSAY;
        IntPtr bytesWritten = IntPtr.Zero;
        nop = BitConverter.GetBytes(0x04C2);
        if (!WriteProcessMemory(hGTA, (IntPtr)dwFunc, nop, nop.Length, out bytesWritten)) return false;
        dwFunc = (int)dwSAMP + (int)SampAddresses.FUNC_SAMP_SENDCMD;
        if (!WriteProcessMemory(hGTA, (IntPtr)dwFunc, nop, nop.Length, out bytesWritten)) return false;
        return true;
    }

    //NOP - unblock raknet client and server communication - CHAT
    public static bool UnblockChatInput()
    {
        if (!checkHandles()) return false;
        byte[] nop = new byte[2];
        int dwFunc = (int)dwSAMP + (int)SampAddresses.FUNC_SAMP_SENDSAY;
        IntPtr bytesWritten = IntPtr.Zero;
        nop = BitConverter.GetBytes(0xA164);
        if (!WriteProcessMemory(hGTA, (IntPtr)dwFunc, nop, nop.Length, out bytesWritten)) return false;
        dwFunc = (int)dwSAMP + (int)SampAddresses.FUNC_SAMP_SENDCMD;
        if (!WriteProcessMemory(hGTA, (IntPtr)dwFunc, nop, nop.Length, out bytesWritten)) return false;
        return true;
    }
    #endregion

    #region Player Functions
    //Get local player Health (Returns -1 on error)
    public static int GetPlayerHealth()
    {
        if (!checkHandles()) return -1;
        int dwCPedPtr = ReadDWORD(hGTA, (int)GtaAddresses.ADDR_CPED_PTR);
        if (dwCPedPtr <= 0) return -1;

        int dwAddr = dwCPedPtr + (int)GtaAddresses.ADDR_CPED_HPOFF;
        float fHealth = ReadFloat(hGTA, dwAddr);
        return Convert.ToInt32(fHealth);
    }

    //Set local player health
    public static bool SetPlayerHealth(int wValue)
    {
        if (!checkHandles()) return false;
        int dwCPedPtr = ReadDWORD(hGTA, (int)GtaAddresses.ADDR_CPED_PTR);
        if (dwCPedPtr <= 0) return false;

        int dwAddr = dwCPedPtr + (int)GtaAddresses.ADDR_CPED_HPOFF;
        return WriteFloat(hGTA, dwAddr, (float)wValue);
    }

    //Get local player Armor (Returns -1 on error)
    public static int GetPlayerArmor()
    {
        if (!checkHandles()) return -1;
        int dwCPedPtr = ReadDWORD(hGTA, (int)GtaAddresses.ADDR_CPED_PTR);
        if (dwCPedPtr <= 0) return -1;

        int dwAddr = dwCPedPtr + (int)GtaAddresses.ADDR_CPED_ARMOROFF;
        float fArmor = ReadFloat(hGTA, dwAddr);
        return Convert.ToInt32(fArmor);
    }

    //Set local player Armor
    public static bool SetPlayerArmor(int wValue)
    {
        if (!checkHandles()) return false;
        int dwCPedPtr = ReadDWORD(hGTA, (int)GtaAddresses.ADDR_CPED_PTR);
        if (dwCPedPtr <= 0) return false;

        int dwAddr = dwCPedPtr + (int)GtaAddresses.ADDR_CPED_ARMOROFF;
        return WriteFloat(hGTA, dwAddr, (float)wValue);
    }

    //Get local player actual interior
    //http://weedarr.wikidot.com/interior
    public static int GetPlayerInteriorId()
    {
        if (!checkHandles()) return -1;
        int iId = ReadDWORD(hGTA, (int)GtaAddresses.ADDR_CPED_INTID);
        if (iId < 0) return -1;
        return iId;
    }

    //Set local player Interior
    public static bool SetPlayerInteriorId(int wValue)
    {
        if (!checkHandles()) return false;
        return WriteDWORD(hGTA, (int)GtaAddresses.ADDR_CPED_INTID, wValue);
    }

    //Get local player money (Returns -1 on error)
    public static int GetPlayerMoney()
    {
        if (!checkHandles()) return -1;

        int iMoney = ReadDWORD(hGTA, (int)GtaAddresses.ADDR_CPED_MONEY);
        if (iMoney < 0) return -1;
        return iMoney;
    }

    //Set local player money
    public static bool SetPlayerMoney(int wValue)
    {
        if (!checkHandles()) return false;
        return WriteDWORD(hGTA, (int)GtaAddresses.ADDR_CPED_MONEY, wValue);
    }
    #endregion

    #region Vehicles Functions
    //Check if local player is in any vehicle
    public static bool IsPlayerInAnyVehicle()
    {
        if (!checkHandles()) return false;
        int dwVehPtr = ReadDWORD(hGTA, (int)GtaAddresses.ADDR_VEHICLE_PTR);
        return dwVehPtr > 0;
    }

    //Check if local player is driver
    public static bool IsPlayerDriver()
    {
        if (!checkHandles()) return false;
        int dwAddr = ReadDWORD(hGTA, (int)GtaAddresses.ADDR_VEHICLE_PTR);
        if (dwAddr <= 0) return false;
        int dwCPedPtr = ReadDWORD(hGTA, (int)GtaAddresses.ADDR_CPED_PTR);
        if (dwCPedPtr <= 0) return false;
        int dwVal = ReadDWORD(hGTA, dwAddr + (int)GtaAddresses.ADDR_VEHICLE_DRIVER);
        return dwVal == dwCPedPtr;
    }

    //Get local player vehicle health (Returns -1 on error)
    public static float GetVehicleHealth()
    {
        if (!checkHandles()) return -1;
        int dwVehPtr = ReadDWORD(hGTA, (int)GtaAddresses.ADDR_VEHICLE_PTR);
        if (dwVehPtr <= 0) return -1;
        int dwAddr = dwVehPtr + (int)GtaAddresses.ADDR_VEHICLE_HPOFF;
        if (dwAddr <= 0) return -1;
        float fHealth = ReadFloat(hGTA, dwAddr);
        return fHealth;
    }

    //Set local player vehicle health
    public static bool SetVehicleHealth(int wHealth)
    {
        if (!checkHandles()) return false;
        int dwVehPtr = ReadDWORD(hGTA, (int)GtaAddresses.ADDR_VEHICLE_PTR);
        if (dwVehPtr <= 0) return false;
        int dwAddr = dwVehPtr + (int)GtaAddresses.ADDR_VEHICLE_HPOFF;
        if (dwAddr <= 0) return false;
        return WriteFloat(hGTA, dwAddr, (float)wHealth);
    }

    //Get local player vehicle type
    //1 = car, 2=boat, 3=train, 4=motorbike, 5=plane, 6=bike
    public static int GetVehicleType()
    {
        if (!checkHandles()) return -1;
        int dwAddr = ReadDWORD(hGTA, (int)GtaAddresses.ADDR_VEHICLE_PTR);
        if (dwAddr <= 0) return -1;
        int cVal = ReadDWORD(hGTA, dwAddr + (int)GtaAddresses.ADDR_VEHICLE_TYPE);
        if (cVal <= 0)
        {
            int vehicleId = GetVehicleModelId();
            foreach (int plane in oAirplaneModels)
            {
                if (plane == vehicleId) return 5;
            }
            return 1;
        }
        else if (cVal == 5)
        {
            return 2;
        }
        else if (cVal == 6)
        {
            return 3;
        }
        else if (cVal == 9)
        {
            int vehicleId = GetVehicleModelId();
            foreach (int bike in oBikeModels)
            {
                if (bike == vehicleId) return 6;
            }
            return 4;
        }
        return 0;
    }


    //Get local player vehicle model id (Returns -1 on error)
    public static int GetVehicleModelId()
    {
        if (!checkHandles()) return -1;
        int dwAddr = ReadDWORD(hGTA, (int)GtaAddresses.ADDR_VEHICLE_PTR);
        if (dwAddr <= 0) return -1;
        int sVal = ReadShort(hGTA, dwAddr + (int)GtaAddresses.ADDR_VEHICLE_MODEL);
        return sVal;
    }

    //Get local player vehicle model name (Returns empty on error)
    public static string GetVehicleModelName()
    {
        int vModelId = GetVehicleModelId();
        if (vModelId > 400 && vModelId < 611)
        {
            return ovehicleNames[vModelId - 400];
        }
        return "";
    }

    //Check if local player vehicle is on
    public static bool IsVehicleEngineStateON()
    {
        if (!checkHandles()) return false;
        int dwAddr = ReadDWORD(hGTA, (int)GtaAddresses.ADDR_VEHICLE_PTR);
        if (dwAddr <= 0) return false;
        int cVal = ReadChar(hGTA, dwAddr + (int)GtaAddresses.ADDR_VEHICLE_ENGINESTATE);
        if (cVal <= 0) return false;
        return (cVal == 16 || cVal == 24 || cVal == 56 || cVal == 88 || cVal == 120);
    }

    //Check if local player vehicle is locked
    public static bool IsVehicleLocked()
    {
        if (!checkHandles()) return false;
        int dwAddr = ReadDWORD(hGTA, (int)GtaAddresses.ADDR_VEHICLE_PTR);
        if (dwAddr <= 0) return false;
        int dwVal = ReadDWORD(hGTA, dwAddr + (int)GtaAddresses.ADDR_VEHICLE_DOORSTATE);
        return dwVal == 2;
    }
    #endregion

    #region Coordinates
    //Get Local player coordinates
    // float[] Coords = GetCoordinates()
    //float[] = null [Error]
    // Coords[0] = X
    // Coords[1] = Y
    // Coords[2] = Z
    public static float[] GetCoordinates()
    {
        if (!checkHandles()) return null;

        float fX = ReadFloat(hGTA, (int)GtaAddresses.ADDR_POSITION_X);
        float fY = ReadFloat(hGTA, (int)GtaAddresses.ADDR_POSITION_Y);
        float fZ = ReadFloat(hGTA, (int)GtaAddresses.ADDR_POSITION_Z);

        return new float[] { fX, fY, fZ };
    }

    //Get local player checkpoint coordinates
    public static float[] GetCheckpointCoordinates()
    {
        if (!checkHandles()) return null;
        int dwPtr = (int)dwSAMP + (int)GtaAddresses.ADDR_CHECKPOINT_PTR;
        int dwAddress = ReadDWORD(hGTA, dwPtr);
        dwAddress += 0xC;
        float fX = ReadFloat(hGTA, dwAddress);
        dwAddress += 0x4;
        float fY = ReadFloat(hGTA, dwAddress);
        dwAddress += 0x4;
        float fZ = ReadFloat(hGTA, dwAddress);
        return new float[] { fX, fY, fZ };
    }

    #endregion

    #region Misc
    //Send a keypress to GTA
    //kOffset = Key Offset
    //All Virtual Key Codes: https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
    public static bool SetVirtualKey(int kOffset, bool state = true)
    {
        if (!checkHandles()) return false;

        kOffset *= 2;
        int dwAddress = (int)GtaAddresses.ADDR_VIRTUALKEYS_PTR + kOffset;
        int value = state ? 255 : 0;

        byte[] buffer = new byte[1];
        buffer = BitConverter.GetBytes(value);

        IntPtr bytesWritten = IntPtr.Zero;
        return WriteProcessMemory(hGTA, (IntPtr)dwAddress, buffer, buffer.Length, out bytesWritten);
    }

    //Active/Desactive a SAMP NOP 
    //offset = SampNOPS enum offsets
    // out oldValue = Store the old memory value case u need to restore the default value
    //checkState = Set true if you want to restore the old nop value
    // setValue = The default value of the nop that is stored in oldValue var
    //int oldValue = 0;
    //SetNOP(SampNOPS.NOP_SETPLAYERNAME, out oldValue, true, oldNOPValue);
    public static bool SetNOP(SampNOPS offset, out int oldValue, bool checkState = false, int setValue = 0)
    {
        oldValue = -1;
        if (!checkHandles()) return false;

        int dwPtr = (int)dwSAMP + (int)offset;
        oldValue = ReadDWORD(hGTA, dwPtr);

        if (checkState)
        {
            if (setValue == 0) return false;
            return WriteDWORD(hGTA, dwPtr, setValue);
        }
        else
        {
            return WriteDWORD(hGTA, dwPtr, (int)SampNOPS.DESACTIVE_VALUE);
        }

    }
    #endregion

    #region [~ INTERNAL ~] INTERNAL STUFF [~ INTERNAL ~]
    #region [~ INTERNAL ~] Get Handles (Base/Modules)
    public static IntPtr GetProcessHandle(String ProcessName)
    {
        try
        {
            Process process = Process.GetProcessesByName(ProcessName)[0];
            IntPtr processHandle = OpenProcess((int)ProcessAccessFlags.All, IntPtr.Zero, new IntPtr(process.Id));
            return processHandle;
        }
        catch (IndexOutOfRangeException)
        {
            return IntPtr.Zero;
        }
    }

    public static IntPtr GetModuleBaseAddress(string processName, string moduleName)
    {
        Process process;

        try
        {
            process = Process.GetProcessesByName(processName)[0];
        }

        catch (IndexOutOfRangeException)
        {
            return IntPtr.Zero;
        }
        var module = process.Modules.Cast<ProcessModule>().SingleOrDefault(m => string.Equals(m.ModuleName, moduleName, StringComparison.OrdinalIgnoreCase));
        return module?.BaseAddress ?? IntPtr.Zero;
    }

    #endregion

    #region [~ INTERNAL ~] Read Memory Stuff
    public static Int32 ReadDWORD(IntPtr hProcess, int bAddress)
    {
        if (hProcess == IntPtr.Zero) return -1;
        byte[] buffer = new byte[4];
        ReadProcessMemory(hProcess, new IntPtr(bAddress), buffer, buffer.Length, 0);
        return BitConverter.ToInt32(buffer, 0);
    }

    public static String ReadString(IntPtr hProcess, int bAddress, int szString)
    {
        if (hProcess == IntPtr.Zero) return null;
        byte[] buffer = new byte[szString];
        ReadProcessMemory(hProcess, new IntPtr(bAddress), buffer, buffer.Length, 0);
        return Encoding.UTF8.GetString(RemoveNullBytes(buffer));
    }

    public static byte[] RemoveNullBytes(byte[] packet)
    {
        var i = packet.Length - 1;
        while (packet[i] == 0)
        {
            --i;
        }
        var temp = new byte[i + 1];
        Array.Copy(packet, temp, i + 1);
        return temp;
    }

    public static float ReadFloat(IntPtr hProcess, int bAddress)
    {
        if (hProcess == IntPtr.Zero) return -1;
        byte[] buffer = new byte[4];
        ReadProcessMemory(hProcess, new IntPtr(bAddress), buffer, buffer.Length, 0);
        return BitConverter.ToSingle(buffer, 0);
    }

    public static int ReadShort(IntPtr hProcess, int bAddress)
    {
        if (hProcess == IntPtr.Zero) return -1;
        byte[] buffer = new byte[2];
        ReadProcessMemory(hGTA, (IntPtr)bAddress, buffer, buffer.Length, 0);
        return BitConverter.ToInt16(buffer, 0);
    }

    public static int ReadChar(IntPtr hProcess, int bAddress)
    {
        if (hProcess == IntPtr.Zero) return -1;
        byte[] buffer = new byte[1];
        ReadProcessMemory(hProcess, (IntPtr)bAddress, buffer, buffer.Length, 0);
        return buffer[0];
    }
    public static dynamic ReadMem(IntPtr hProcess, int dwAddress, int dwLen = 4, string type = "UInt")
    {
        if (hProcess == IntPtr.Zero) return null;
        byte[] buffer = new byte[dwLen];
        int bytesRead = 0;
        ReadProcessMemory(hProcess, (IntPtr)dwAddress, buffer, dwLen, bytesRead);
        if (type == "String")
        {
            return Encoding.UTF8.GetString(buffer);
        }
        else if (type == "Char")
        {
            return buffer[0];
        }
        else if (type == "Float")
        {
            return BitConverter.ToSingle(buffer, 0);
        }
        else if (type == "Short")
        {
            return BitConverter.ToInt16(buffer, 0);
        }
        else
        {
            return BitConverter.ToInt32(buffer, 0);
        }
    }
    #endregion

    #region [~ INTERNAL ~] Write Memory Stuff
    public static bool WriteDWORD(IntPtr hProcess, int bAddress, int wDWORD)
    {
        //1. Create buffer and pointer
        byte[] dataBuffer = BitConverter.GetBytes(wDWORD);
        IntPtr bytesWritten = IntPtr.Zero;

        //2. Write
        WriteProcessMemory(hProcess, (IntPtr)bAddress, dataBuffer, dataBuffer.Length, out bytesWritten);

        //3. Error handling
        if (bytesWritten == IntPtr.Zero || bytesWritten.ToInt32() < dataBuffer.Length)
        {
            Debug.WriteLine("We didn't write anything!");
            return false;
        }
        return true;
    }

    public static bool WriteString(IntPtr hProcess, int bAddress, String wString)
    {
        byte[] dataBuffer = Encoding.UTF8.GetBytes(wString);
        IntPtr bytesWritten = IntPtr.Zero;

        WriteProcessMemory(hProcess, (IntPtr)bAddress, dataBuffer, dataBuffer.Length, out bytesWritten);

        if (bytesWritten == IntPtr.Zero || bytesWritten.ToInt32() < dataBuffer.Length + 1)
        {
            Debug.WriteLine("We didn't write anything!");
            return false;
        }
        return true;
    }

    public static bool WriteFloat(IntPtr hProcess, int bAddress, float wFloat)
    {
        byte[] dataBuffer = BitConverter.GetBytes(wFloat);
        IntPtr bytesWritten = IntPtr.Zero;

        WriteProcessMemory(hProcess, (IntPtr)bAddress, dataBuffer, dataBuffer.Length, out bytesWritten);

        if (bytesWritten == IntPtr.Zero || bytesWritten.ToInt32() < dataBuffer.Length)
        {
            Debug.WriteLine("We didn't write anything!");
            return false;
        }
        return true;
    }
    #endregion

    #region [~ INTERNAL ~] Refreshes (Handles, GTA, SAMP, Memory)
    public static bool checkHandles()
    {
        if (IntPtr.Size == 8)
        {
            throw new Exception("This process is 64bits, this won't work! Compile with 32bits");
        }
        if (iRefreshHandles + 500 > Environment.TickCount) return true;
        iRefreshHandles = Environment.TickCount;

        if (RefreshGTA() && RefreshSAMP() && RefreshMemory())
        {
            return true;
        }

        return false;
    }

    public static bool RefreshGTA()
    {
        try
        {
            Process pGta = Process.GetProcessesByName("gta_sa")[0];
        }
        catch (IndexOutOfRangeException)
        {
            if (hGTA != IntPtr.Zero)
            {
                VirtualFreeEx(hGTA, pMemory, 0, 0x8000);
                CloseHandle(hGTA);
                hGTA = IntPtr.Zero;
            }
            hGTA = IntPtr.Zero;
            dwSAMP = IntPtr.Zero;
            pMemory = IntPtr.Zero;
            return false;
        }

        if (hGTA == IntPtr.Zero)
        {
            IntPtr processHandle = GetProcessHandle("gta_sa");
            if (processHandle == IntPtr.Zero)
            {
                hGTA = IntPtr.Zero;
                dwSAMP = IntPtr.Zero;
                pMemory = IntPtr.Zero;
                return false;
            }
            hGTA = processHandle;
            dwSAMP = IntPtr.Zero;
            pMemory = IntPtr.Zero;
            return true;
        }

        return true;
    }

    public static bool RefreshSAMP()
    {
        if (dwSAMP != IntPtr.Zero) return true;
        dwSAMP = GetModuleBaseAddress("gta_sa", "samp.dll");
        if (dwSAMP == IntPtr.Zero) return false;
        return true;
    }

    public static bool RefreshMemory()
    {
        if (pMemory == IntPtr.Zero)
        {
            pMemory = VirtualAllocEx(hGTA, IntPtr.Zero, 6144, 0x1000 | 0x2000, 0x40);
            if (pMemory == IntPtr.Zero) return false;
            pParam1 = pMemory;
            pParam2 = pMemory + 1024;
            pParam3 = pMemory + 2048;
            pParam4 = pMemory + 3072;
            pParam5 = pMemory + 4096;
            pInjectFunc = pMemory + 5120;
        }
        return true;
    }
    #endregion
    #endregion
}

