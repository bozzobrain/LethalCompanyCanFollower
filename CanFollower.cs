using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BetterDoorWarning
{
	[HarmonyPatch]
	internal class CanFollowerFunctions
	{
		private bool lastCheckEnemy = false;

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