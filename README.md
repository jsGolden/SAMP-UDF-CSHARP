SAMP UDF for C#
=======================
Version 0.3.7 R1
----------
### Net Framework Version: 4.0+
This collection of functions allows to access native GTA/SAMP functions.
You can create useful scripts to automate repeating tasks.

Functions:

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


*COMPILE*
-compile it with 86x architecture
