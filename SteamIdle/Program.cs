using System.Runtime.InteropServices;

// Appel direct à steam_api64.dll — aucune dépendance externe
[DllImport("steam_api64.dll", CallingConvention = CallingConvention.Cdecl)]
static extern bool SteamAPI_Init();

[DllImport("steam_api64.dll", CallingConvention = CallingConvention.Cdecl)]
static extern void SteamAPI_RunCallbacks();

[DllImport("steam_api64.dll", CallingConvention = CallingConvention.Cdecl)]
static extern void SteamAPI_Shutdown();

if (args.Length == 0) { Console.Error.WriteLine("Usage: steam-idle.exe <AppId>"); return; }

var dir = AppDomain.CurrentDomain.BaseDirectory;
File.WriteAllText(Path.Combine(dir, "steam_appid.txt"), args[0]);

if (!SteamAPI_Init())
{
    Console.Error.WriteLine($"SteamAPI_Init() échoué pour AppId={args[0]}");
    return;
}

Console.WriteLine($"[OK] Idling AppId={args[0]}");
AppDomain.CurrentDomain.ProcessExit += (_, _) => SteamAPI_Shutdown();

while (true)
{
    SteamAPI_RunCallbacks();
    Thread.Sleep(1000);
}
