using CanFollower.Networking;
using HarmonyLib;
using Steamworks.Ugc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static CanFollower.Networking.NetworkFunctions;

namespace CanFollower
{
	[HarmonyPatch]
	internal class CanFollowerFunctions
	{
		public static List<DTItem> spawnableItems = new List<DTItem>();
		private static int currentPlayer = 0;
		public static string CanPlayerName { get; set; } = string.Empty;

		public static string SetPlayerToCanAttack()
		{
			GameNetworkManager netManagerLC = GameNetworkManager.Instance;
			CanFollower.Log($"previous player {currentPlayer}");
			currentPlayer++;
			if (currentPlayer == netManagerLC.connectedPlayers)
				currentPlayer = 0;

			CanFollower.Log($"new player {currentPlayer}");
			if (StartOfRound.Instance != null && StartOfRound.Instance.OtherClients.ElementAt(currentPlayer) != null)
			{
				CanPlayerName = StartOfRound.Instance.OtherClients.ElementAt(currentPlayer).playerUsername;
				CanFollower.Log($"new player {CanPlayerName}");
			}

			// Transmit curernt player RPC
			SetPlayerToCanAttackServerRpc(currentPlayer);

			return CanPlayerName;
		}

		public static string SetPlayerToCanAttack(int player)
		{
			CanFollower.Log($"Setting player to attack {player}");
			currentPlayer = player;
			if (StartOfRound.Instance != null && StartOfRound.Instance.OtherClients.ElementAt(currentPlayer) != null)
				CanPlayerName = StartOfRound.Instance.OtherClients.ElementAt(currentPlayer).playerUsername;
			// Transmit curernt player RPC
			return CanPlayerName;
		}

		public static GameObject SpawnObject(UnityEngine.Vector3 position)
		{
			if (!spawnableItems.Any((DTItem x) => x.name.ToLower().Contains("Red Soda")))
			{
				CanFollower.Log("No red sodas available - wait a round?");
				return null;
			}
			var num3 = spawnableItems.Find((DTItem x) => x.name.ToLower().Contains("Red Soda")).id;

			GameObject newObject = UnityEngine.Object.Instantiate<GameObject>(StartOfRound.Instance.allItemsList.itemsList[num3].spawnPrefab, position, UnityEngine.Quaternion.identity);
			ScanNodeProperties component = newObject.GetComponent<ScanNodeProperties>();
			if ((UnityEngine.Object)(object)component == (UnityEngine.Object)null)
			{
				component = newObject.AddComponent<ScanNodeProperties>();
				component.scrapValue = 0;
				component.subText = $"Value: ${0}";
			}
			newObject.GetComponent<GrabbableObject>().fallTime = 0f;
			newObject.GetComponent<GrabbableObject>().scrapValue = 0;
			newObject.GetComponent<GrabbableObject>().itemProperties.weight = 0;
			newObject.GetComponent<GrabbableObject>().itemProperties.creditsWorth = 0;
			newObject.GetComponent<GrabbableObject>().SetScrapValue(0);
			newObject.GetComponent<NetworkObject>().Spawn(false);
			return newObject;
		}

		private static void GetAllItems()
		{
			DTItem item = default(DTItem);
			string text = "Started Items:";
			AllItemsList allItemsList = StartOfRound.Instance.allItemsList;
			text += $"{allItemsList.itemsList.Count} <- item 0";
			for (int i = 0; i < allItemsList.itemsList.Count; i++)
			{
				item.name = allItemsList.itemsList[i].itemName;
				item.id = i;
				item.prefab = allItemsList.itemsList[i].spawnPrefab;
				spawnableItems.Add(item);
				text += $"\n{i} | {item.name}";
			}
		}

		public struct DTItem
		{
			public int id;
			public string name;
			public GameObject prefab;
		}

		[HarmonyPatch(typeof(PlayerActions), "OnPingScan")]
		private static class Patch
		{
			private static CanFollowerFunctions canFollowerFunctions = new();

			[HarmonyPostfix]
			private static void OnPingScan(InputAction.CallbackContext context)
			{
				if (CanPlayerName == GameNetworkManager.Instance.localPlayerController.playerUsername)
				{
					// Handle can drop RPC
					CanFollower.Log($"Scan detected from selected player {GameNetworkManager.Instance.localPlayerController.playerUsername}, dropping can");
					SpawnObject(GameNetworkManager.Instance.localPlayerController.transform.position);
				}
				return;
			}
		}
	}
}