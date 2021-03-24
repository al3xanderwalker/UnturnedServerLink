using System.IO;
using System.Linq;
using System.Reflection;
using USLModule.Models;
using USLModule.Utils;
using SDG.Framework.Modules;
using SDG.Unturned;
using UnityEngine;

namespace USLModule
{
    public class Main : MonoBehaviour, IModuleNexus
    {
        public static Config Config;
        public static string ServerName;
        private readonly bool _useLoader = true;

        private GameObject _uslObject;

        public void initialize()
        {
            CommandWindow.LogError($"Loaded UnturnedServerLink");

            UnityThread.initUnityThread();

            var patch = new Patcher();
            Patcher.DoPatching();
            
            var assembly = _useLoader ? Assembly.GetCallingAssembly() : Assembly.GetExecutingAssembly();
            var dirName = Path.GetDirectoryName(assembly.Location);
            var separator = Path.DirectorySeparatorChar;
            var configPath = $"{dirName}{separator}config.json";
            
            ConfigHelper.EnsureConfig(configPath);
            Config = ConfigHelper.ReadConfig(configPath);

            _uslObject = new GameObject();
            var componentManagers = Assembly.GetExecutingAssembly().DefinedTypes
                .Where(type => type.ImplementedInterfaces.Any(inter => inter == typeof(IObjectComponent))).ToList();
            componentManagers.ForEach(c =>
            {
                var methodInfo = typeof(GameObject)
                    .GetMethods()
                    .Where(x => x.IsGenericMethod).Single(x => x.Name == "AddComponent");
                var addComponentRef = methodInfo.MakeGenericMethod(c);
                addComponentRef.Invoke(_uslObject, null);
            });

            ServerName = Provider.serverName;
        }

        public void shutdown()
        {
            CommandWindow.Log($"Unloaded UnturnedServerLink");
            Destroy(_uslObject);
            _uslObject = null;
        }
    }
}