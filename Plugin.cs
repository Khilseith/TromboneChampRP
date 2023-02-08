using System;
using BepInEx;
using BepInEx.Logging;
using UnityEngine.SceneManagement;

namespace TromboneRP;

[BepInDependency("TCDRP")]
[BepInPlugin("Khilseith.TromboneRP", "Trombone Champ Rich Presence", "1.1.0")]
public class TromboneRP : BaseUnityPlugin
{
    public static ManualLogSource Log;
    private static readonly long DiscordRPClientID = 1070122620172898367;


    void Awake()
    {
        Log = Logger;
        Logger.LogInfo("Loaded Trombone Champ Rich Presence 1.1.0.");

        TCDRP.API.InitRPC(DiscordRPClientID);

        SceneManager.sceneLoaded += OnScenceLoaded;
    }

    private void OnScenceLoaded(Scene scene, LoadSceneMode mode)
    {

        Logger.LogDebug($"Level {scene.buildIndex} ({scene.name}) was loaded");


        if (scene.name == GameplaySceneName)
        {
            TCDRP.API.SetActivity(
                DiscordRPClientID,
                state: "Playing a song",
                detail: $"{GlobalVariables.chosen_track_data.artist} - {GlobalVariables.chosen_track_data.trackname_long}",
                largeImage: "game_icon",
                endTime: DateTimeOffset.Now.ToUnixTimeSeconds() + GlobalVariables.chosen_track_data.length
                );

        }
        else if (scene.name == TrackselectSceneName)
        {
            TCDRP.API.SetActivity(DiscordRPClientID, state: "Selecting a track", largeImage: "game_icon");
        }
        else
        {
            TCDRP.API.SetActivity(DiscordRPClientID, state: "Browsing the menus", largeImage: "game_icon");
        }
    }

    private const string GameplaySceneName = "gameplay";
    private const string TrackselectSceneName = "levelselect";
}