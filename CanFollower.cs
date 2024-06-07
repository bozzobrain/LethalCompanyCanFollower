using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BetterDoorWarning
{
	[HarmonyPatch]
	internal class CanFollowerFunctions
	{
		public string CanPlayerName { get; set; } = string.Empty;

		[HarmonyPatch(typeof(PlayerActions), "OnPingScan")]
		private static class Patch
		{
			private static CanFollowerFunctions canFollowerFunctions = new();

			[HarmonyPostfix]
			private static void OnPingScan(InputAction.CallbackContext context)
			{
				return;
			}
		}
	}
}