using Bark.Extensions;
using Bark.Modules;
using Bark.Tools;
using BepInEx.Configuration;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Bark.Networking;

public class NetworkPropertyHandler : MonoBehaviourPunCallbacks
{
    private const float refreshRate = 1f;

    public static NetworkPropertyHandler Instance;
    public static string versionKey = "BarkVersion";
    private readonly Hashtable properties = new();

    private float lastPropertyUpdate;
    public Dictionary<NetPlayer?, NetworkedPlayer> networkedPlayers = new();
    public Action<NetPlayer?> OnPlayerJoined;
    public Action<NetPlayer?> OnPlayerLeft;
    public Action<NetPlayer, string, bool> OnPlayerModStatusChanged;
    private static ConfigEntry<bool>? _network;

    private void Awake()
    {
        Instance = this;

        var inputDesc = new ConfigDescription("Set to true if you want networking, if set to true mod checkers can detect that you have bark.");
        _network = Plugin.ConfigFile.Bind("General",
            "network",
            false,
            inputDesc
        );

        ChangeProperty(versionKey, PluginInfo.Version);
    }

    private void Start()
    {
        foreach (var player in PhotonNetwork.PlayerList) StartCoroutine(CreateNetworkedPlayer(player));
        NetworkSystem.Instance.OnPlayerJoined += OnPlayerEnteredRoom;
        NetworkSystem.Instance.OnPlayerLeft += OnPlayerLeftRoom;
    }

    private void FixedUpdate()
    {
        if (_network.Value = false) return;

        if (properties.Count == 0 || Time.time - lastPropertyUpdate < refreshRate) return;
        Logging.Debug($"Updated properties ({properties.Count}):");
        foreach (var property in properties)
        {
            Logging.Debug(property.Key, ":", property.Value);
            if ((string)property.Key == BarkModule.enabledModulesKey)
                foreach (var mod in (Dictionary<string, bool>)property.Value)
                    if (mod.Value)
                        Logging.Debug("    ", property.Key, "is enabled");
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
        properties.Clear();
        lastPropertyUpdate = Time.time;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        var targetNetPlayer = NetworkSystem.Instance.GetPlayer(targetPlayer.ActorNumber);
        if (targetNetPlayer != NetworkSystem.Instance.LocalPlayer)
            if (changedProps.ContainsKey(BarkModule.enabledModulesKey))
            {
                networkedPlayers[targetPlayer].hasBark = true;
                var enabledModules = (Dictionary<string, bool>)changedProps[BarkModule.enabledModulesKey];
                //Logging.Debug(targetPlayer.NickName, "toggled mods:");
                foreach (var mod in enabledModules)
                    //Logging.Debug(mod.Value ? "  +" : "  -", mod.Key, mod.Value);
                    OnPlayerModStatusChanged?.Invoke(targetPlayer, mod.Key, mod.Value);
            }
    }

    public void OnPlayerLeftRoom(NetPlayer? otherPlayer)
    {
        OnPlayerLeft?.Invoke(otherPlayer);
        if (networkedPlayers.ContainsKey(otherPlayer))
        {
            Destroy(networkedPlayers[otherPlayer]);
            networkedPlayers.Remove(otherPlayer);
        }
    }

    public void OnPlayerEnteredRoom(NetPlayer? newPlayer)
    {
        try
        {
            OnPlayerJoined?.Invoke(newPlayer);
            StartCoroutine(CreateNetworkedPlayer(newPlayer));
        }
        catch (Exception e)
        {
            Logging.Exception(e);
        }
    }

    private IEnumerator CreateNetworkedPlayer(NetPlayer? player = null, VRRig? rig = null)
    {
        switch (player)
        {
            case null when rig is null:
                throw new Exception("Both player and rig are null");
            case null:
                player = rig.Creator;
                break;
            default:
            {
                if (rig is null)
                    for (var i = 0; i < 10; i++)
                    {
                        rig = player.Rig();
                        if (rig is null) yield return new WaitForSeconds(.1f);
                    }

                break;
            }
        }

        var np = rig?.gameObject.GetOrAddComponent<NetworkedPlayer>();
        if (np != null)
        {
            np.owner = player;
            np.rig = rig;
            networkedPlayers.AddOrUpdate(player, np);
        }
    }

    public void ChangeProperty(string key, object value)
    {
        if (!properties.TryAdd(key, value))
            properties[key] = value;
    }
}