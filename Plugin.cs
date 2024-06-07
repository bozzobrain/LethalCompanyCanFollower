using BepInEx;
using HarmonyLib;
using System.Reflection;

namespace CanFollower
{
	[BepInPlugin(GUID, NAME, VERSION)]
	internal class CanFollower : BaseUnityPlugin
	{
		public static CanFollower instance;
		private const string GUID = "CanFollower";
		private const string NAME = "CanFollower";
		private const string VERSION = "1.0.0";

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