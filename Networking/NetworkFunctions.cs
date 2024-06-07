using GameNetcodeStuff;
using HarmonyLib;
using System.ComponentModel;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace CanFollower.Networking
{
	public class NetworkFunctions : NetworkBehaviour
	{
		public static void NetworkManagerInit()
		{
			CanFollower.Log("Registering named message");
			NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("SetPlayerSelectedCanFollower", (senderClientId, reader) =>
			{
				if (senderClientId != GameNetworkManager.Instance.localPlayerController.playerClientId)
				{
					// Get identity of selected player
					reader.ReadValueSafe(out int currentSelectedCanPlayer);

					CanFollowerFunctions.SetPlayerToCanAttack(currentSelectedCanPlayer);
				}
			});
		}

		[HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
		[HarmonyPostfix]
		public static void OnLocalPlayerConnect(PlayerControllerB __instance)
		{
			GameNetworkManager.Instance.localPlayerController = __instance;

			CanFollower.Log("Local Player Connect");

			NetworkManagerInit();

			CanFollower.Log("Registering Manager");
		}

		[ClientRpc]
		public static void SetPlayerToCanAttackClientRpc(int playerSelected)
		{
			NetworkManager networkManager = NetworkManager.Singleton;
			if ((object)networkManager == null || !networkManager.IsListening)
			{
				return;
			}

			FastBufferWriter bufferWriter = new FastBufferWriter(256, Unity.Collections.Allocator.Temp);
			bufferWriter.WriteValueSafe(playerSelected);
			NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll("SetPlayerSelectedCanFollower", bufferWriter, NetworkDelivery.Reliable);

			if (!GameNetworkManager.Instance.localPlayerController.IsOwner)
			{
				CanFollowerFunctions.SetPlayerToCanAttack(playerSelected);
			}
		}

		[ServerRpc]
		public static void SetPlayerToCanAttackServerRpc(int playerSelected)
		{
			NetworkManager networkManager = NetworkManager.Singleton;
			if ((object)networkManager == null)
			{
				CanFollower.LogError("Network Manager == null");
				return;
			}
			if (!networkManager.IsListening)
			{
				CanFollower.LogError("Network Manager not listening");
				return;
			}

			CanFollower.LogError("before lpc");
			if (GameNetworkManager.Instance.localPlayerController.OwnerClientId != networkManager.LocalClientId)
			{
				if (networkManager.LogLevel <= LogLevel.Normal)
				{
					CanFollower.LogError("Only the owner can invoke a ServerRpc that requires ownership!");
				}

				return;
			}

			FastBufferWriter bufferWriter = new FastBufferWriter(256, Unity.Collections.Allocator.Temp);

			bufferWriter.WriteValueSafe(playerSelected);
			NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll("SetPlayerSelectedCanFollower", bufferWriter, NetworkDelivery.Reliable);

			CanFollowerFunctions.SetPlayerToCanAttack(playerSelected);
		}
	}
}