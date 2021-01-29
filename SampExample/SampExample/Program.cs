using System;

namespace SampTests
{
    class Program
    {
        static void Main(string[] args)
        {
            string ServerIp = Samp.GetServerIP();
            Console.WriteLine($"Current server ip: {ServerIp}");
            string hostname = Samp.GetServerHostname();
            Console.WriteLine($"Current hostname: {hostname}");

            bool inChat = Samp.IsInChat();
            Console.WriteLine($"You're with chat window opened: {inChat}");

            string username = Samp.GetUsername();
            Console.WriteLine($"Your current username is {username}");

            int health = Samp.GetPlayerHealth();
            Console.WriteLine($"Your current health: {health}");
            Samp.SetPlayerHealth(10);
            Console.WriteLine($"Health changed to 10");

            int armor = Samp.GetPlayerArmor();
            Console.WriteLine($"Current armor: {armor}");
            Samp.SetPlayerArmor(15);
            Console.WriteLine("Armor changed to 15");

            int money = Samp.GetPlayerMoney();
            Console.WriteLine($"Your current money {money}");
            Samp.SetPlayerMoney(2500);
            Console.WriteLine("Money changed to 2500");

            int interiorId = Samp.GetPlayerInteriorId();
            Console.WriteLine($"You current interior id: {interiorId}");

            bool isPlayerInVehicle = Samp.IsPlayerInAnyVehicle();
            Console.WriteLine($"You're in vehicle: {isPlayerInVehicle}");
            if(isPlayerInVehicle)
            {
                bool isPlayerDriver = Samp.IsPlayerDriver();
                Console.WriteLine($"You're the driver: {isPlayerDriver}");

                float carHealth = Samp.GetVehicleHealth();
                Console.WriteLine($"Current vehicle health: {carHealth}");

                int vType = Samp.GetVehicleType();
                Console.WriteLine($"Current vehicle type: {vType}");

                int model = Samp.GetVehicleModelId();
                Console.WriteLine($"Current vehicle model id: {model}");

                string vehicleName = Samp.GetVehicleModelName();
                Console.WriteLine($"Current vehicle model name: {vehicleName}");

                bool vehicleOn = Samp.IsVehicleEngineStateON();
                Console.WriteLine($"Current vehicle is on: {vehicleOn}");
            }

            float[] Coords = Samp.GetCoordinates();
            Console.WriteLine($"Coordinates: X: {Coords[0]} Y: {Coords[1]} Z: {Coords[2]}");

            bool cpActive = Samp.IsCheckpointActive();
            Console.WriteLine($"Is checkpoint active: {cpActive}");

            if(cpActive)
            {
                float[] cpCoords = Samp.GetCheckpointCoordinates();
                Console.WriteLine($"Checkpoint Coordinates: X: {cpCoords[0]} Y: {cpCoords[1]} Z: {cpCoords[2]}");
            }

            string lastMsg = Samp.GetLastChatMessage();
            if (!String.IsNullOrEmpty(lastMsg)) Console.WriteLine($"Last chat message: {lastMsg}");

            Console.ReadKey();

        }
    }
}
