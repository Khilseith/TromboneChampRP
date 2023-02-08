using BepInEx;
using BepInEx.Logging;
using System;
using UnityEngine.SceneManagement;

namespace TromboneRP;

[BepInPlugin("Khilseith.TromboneRP", "Trombone Champ Rich Presence", "1.0.0")]
public class TromboneRP : BaseUnityPlugin
{

    public static TromboneRP Instance;
    internal static DiscordRPC.RichPresence prsnc;
    public static ManualLogSource Log;


    void Awake()
    {
        Log = Logger;
        Logger.LogInfo("Loaded Trombone Champ Rich Presence 1.0.0.");

        DiscordRPC.EventHandlers eventHandlers = default;
        eventHandlers.readyCallback = (DiscordRPC.ReadyCallback)Delegate.Combine(eventHandlers.readyCallback, new DiscordRPC.ReadyCallback(ReadyCallback));
        eventHandlers.disconnectedCallback = (DiscordRPC.DisconnectedCallback)Delegate.Combine(eventHandlers.disconnectedCallback, new DiscordRPC.DisconnectedCallback(DisconnectedCallback));
        eventHandlers.errorCallback = (DiscordRPC.ErrorCallback)Delegate.Combine(eventHandlers.errorCallback, new DiscordRPC.ErrorCallback(ErrorCallback));

        DiscordRPC.Initialize("1070122620172898367", ref eventHandlers, true, "0612");
        prsnc = default;
        SetStatus();
        ReadyCallback();

        SceneManager.sceneLoaded += OnScenceLoaded;
    }

    private static void ErrorCallback(int errorCode, string message)
    {
        Log.LogError($"ErrorCallback: {errorCode}: {message}");
    }
    private static void DisconnectedCallback(int errorCode, string message)
    {
        Log.LogDebug($"DisconnectedCallback: {errorCode}: {message}");
    }
    private static void ReadyCallback()
    {
        Log.LogDebug("Working");
    }

    private void SetStatus()
    {
        prsnc.state = "Main Menu";
        prsnc.details = null;
        prsnc.startTimestamp = 0;
        prsnc.largeImageKey = "game_icon";
        prsnc.largeImageText = "Toot toot";
        prsnc.smallImageKey = null;
        prsnc.smallImageText = null;
        prsnc.partyMax= 0;
        prsnc.partySize = 0;
        prsnc.endTimestamp = 0;
        DiscordRPC.UpdatePresence(ref prsnc);
    }

    private void SetGameStatus(SingleTrackData trackData)
    {
        prsnc.state = "Playing a Song";
        prsnc.details = trackData.trackname_long;
        prsnc.endTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds() + trackData.length;
        DiscordRPC.UpdatePresence(ref prsnc);
    }

    private void OnScenceLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Additive)
            return;

        Logger.LogDebug($"Level {scene.buildIndex} ({scene.name}) was loaded");

        if (scene.name == GameplaySceneName)
        {
            Logger.LogInfo("Gameplay scene loaded.");
            _isInGameplay = true;
            SetGameStatus(GlobalVariables.chosen_track_data);
        }
        else if (_isInGameplay)
        {
            Logger.LogInfo("Gameplay scene unloaded.");
            _isInGameplay = false;
            SetStatus();
        }

        if (scene.name == TrackselectSceneName)
        {
            prsnc.state = "Selecting a Track";
            DiscordRPC.UpdatePresence(ref prsnc);
        }
    }

    private static bool _isInGameplay;
    private const string GameplaySceneName = "gameplay";
    private const string TrackselectSceneName = "levelselect";
}