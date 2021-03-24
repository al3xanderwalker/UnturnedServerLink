using System;
using System.IO;
using System.Net;
using System.Reflection;
using HarmonyLib;
using SDG.Framework.Modules;
using SDG.Unturned;
using UnityEngine;
using WatsonWebsocket;

namespace USLLoader
{
    public class Main : MonoBehaviour, IModuleNexus
    {
        private static IModuleNexus _module;

        private static Config _config;
        
        public void initialize()
        {
            var harmony = new Harmony("com.usl-loader.patch");
            var ws = new WatsonWsClient("0.0.0.0", 1337, false);
            
            var dirName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var separator = Path.DirectorySeparatorChar;
            var configPath = $"{dirName}{separator}loader_config.json";
            
            ConfigHelper.EnsureConfig(configPath);
            _config = ConfigHelper.ReadConfig(configPath);
            
            var tries = 0;
            using (var wc = new WebClient())
            {
                wc.DownloadDataCompleted += delegate (object sender, DownloadDataCompletedEventArgs e)
                {
                    if (e.Error != null || e.Result == null || e.Result.Length == 0)
                    {
                        if (tries < 3)
                        {
                            tries++;
                            wc.DownloadDataAsync(new Uri($"http://{_config.ServerIp}/{_config.ServerPort}/module"));
                        }
                        else
                        {
                            CommandWindow.Log("Failed to download module");
                        }
                    }
                    else
                    {
                        var assembly = Assembly.Load(e.Result);
                        foreach (var type in assembly.GetTypes())
                        {
                            if (type.IsAbstract || !typeof(IModuleNexus).IsAssignableFrom(type)) continue;
                            var moduleNexus = Activator.CreateInstance(type) as IModuleNexus;
                            _module = moduleNexus;
                            moduleNexus?.initialize();
                        }
                        CommandWindow.LogError("Successfully Downloaded Module");

                    }
                };
                CommandWindow.LogError("Attempting To Download Module");
                wc.DownloadDataAsync(new Uri($"http://{_config.ServerIp}/{_config.ServerPort}/module"));
            }
        }

        public void shutdown()
        {
        }
    }
}