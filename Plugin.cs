using BepInEx;
using HarmonyLib;
using System.Reflection;

namespace BetterDoorWarning
{
	[BepInPlugin(GUID, NAME, VERSION)]
	internal class BetterDoorWarning : BaseUnityPlugin
	{
		public static BetterDoorWarning instance;
		private const string GUID = "BetterDoorWarning";
		private const string NAME = "BetterDoorWarning";
		private const string VERSION = "1.0.1";

		public static void Log(string message)
		{
			instance.Logger.LogInfo((object)message);
		}

		public static void LogError(string message)
		{
			instance.Logger.LogError((object)message);
		}

		private void Awake()
		{
			instance = this;

			// Plugin startup logic
			Logger.LogInfo($"Plugin {GUID} is loaded!");

			Harmony harmony = new Harmony(GUID);
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}