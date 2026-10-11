namespace Bark.Patches;

public static class DebugPatches
{
    public static string[] ignoreables = new[]
    {
        "JoinRandomRoom failed.",
        "PhotonView does not exist!",
        "Photon.Pun.PhotonHandler.DelGloves ()",
        "Photon.Voice.Unity.VoiceConnection.DelGloves ()",
        "GorillaNetworking.PhotonNetworkController.DisconnectCleanup ()"
    };

    public static bool Ignore(string s)
    {
        foreach (var i in ignoreables)
            if (s.Contains(i))
                return true;
        return false;
    }
}