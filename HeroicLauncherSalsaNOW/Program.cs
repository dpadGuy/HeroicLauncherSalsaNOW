using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace HeroicLauncherSalsaNOW
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string heroicDL = "https://salsanowfiles.work/zips/Heroic.zip";
            string salsaNowPath = "I:\\Apps\\SalsaNOW";
            string heroicPath = "I:\\Apps\\SalsaNOW\\HeroicLauncherGFN";
            string epicGamesPath = "I:\\Apps\\SalsaNOW\\EpicGamesPortable-1.0.0";
            string powershellPath = "I:\\Apps\\SalsaNOW\\SilentApps\\Powershell\\pwsh.exe";
            string powershellRename = "I:\\Apps\\SalsaNOW\\SilentApps\\Powershell\\powershell.exe";
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string epicShortcut = Path.Combine(desktopPath, "Epic Games.lnk");

            // Update or create Heroic config to set checkForUpdatesOnStartup to false
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string heroicConfigDir = Path.Combine(appDataPath, "heroic");
            string configJsonPath = Path.Combine(heroicConfigDir, "config.json");
                        string configContent = File.ReadAllText(configJsonPath);

            if (!Directory.Exists(heroicPath))
            {
                Console.WriteLine("Installing Heroic Launcher...");

                WebClient webClient = new WebClient();

                await webClient.DownloadFileTaskAsync(heroicDL, salsaNowPath + "\\Heroic.zip");

                ZipFile.ExtractToDirectory(salsaNowPath + "\\Heroic.zip", heroicPath);

                File.Delete(salsaNowPath + "\\Heroic.zip");

                if (Directory.Exists(epicGamesPath))
                {
                    Console.WriteLine("Removing Epic Games Portable...");

                    Directory.Delete(epicGamesPath, true);

                    if (File.Exists(epicShortcut))
                    {
                        Console.WriteLine("Removing Epic Games shortcut...");
                        File.Delete(epicShortcut);
                    }
                }
            }

            if (File.Exists(powershellPath))
            {
                Console.WriteLine("Renaming pwsh.exe to powershell.exe...");

                if (File.Exists(powershellRename))
                    File.Delete(powershellRename);

                File.Move(powershellPath, powershellRename);
            }

            Directory.CreateDirectory(heroicConfigDir);

            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c \"set \"PATH=I:\\Apps\\SalsaNOW\\SilentApps\\Powershell;%PATH%\" && start \"\" \"I:\\Apps\\SalsaNOW\\HeroicLauncherGFN\\Heroic.exe\"\"",
                UseShellExecute = false
            });

            Thread.Sleep(1000); // Wait for 1 second to ensure Heroic Launcher has started

            if (File.Exists(powershellRename))
            {
                Console.WriteLine("Renaming powershell.exe back to pwsh.exe...");

                if (File.Exists(powershellPath))
                    File.Delete(powershellPath);

                File.Move(powershellRename, powershellPath);
            }

            if (configContent.Contains("\"checkForUpdatesOnStartup\": true"))
            {
                Console.WriteLine("Updating Heroic config.json... DO NOT CLOSE ME");

                Thread.Sleep(2000);

                configContent = configContent.Replace("\"checkForUpdatesOnStartup\": true", "\"checkForUpdatesOnStartup\": false");
                File.WriteAllText(configJsonPath, configContent);
            }
        }
    }
}