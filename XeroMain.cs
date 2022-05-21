using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using MelonLoader;
using Photon.Realtime;
using UnityEngine;
using VRC;
using Player = VRC.Player;

namespace Xero
{
    public class XeroMain : MelonMod
    {
        public override void OnApplicationStart()
        {
            DownloadUpdateFromGitHub.DownloadFromGitHub("XeroSmall", out _);
            MelonCoroutines.Start(HookOnUiManagerInit());
            Buttons.CallonApplicationStart();
        }

        public override void OnApplicationLateStart()
        {
        }

        public override void OnUpdate()
        {
            Buttons.CallonUpdate();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            base.OnSceneWasInitialized(buildIndex, sceneName);
        }

        private System.Collections.IEnumerator HookOnUiManagerInit()
        {
            while (VRCUiManager.field_Private_Static_VRCUiManager_0 == null)
            {
                yield return null;
            }
            OnUiManagerInit(); // This is called when the UI is being created so that way I can start to implement my buttons as well!
        }

        public void OnUiManagerInit()
        {
            Buttons.CallonUI();
            try
            {
                InitiatePatches();
            }
            catch (Exception ex) { MelonLogger.Error("Error in InitPatches [{0}]", ex.Message); }
        }

        private void InitiatePatches()
        {
            try
            {
                new Patches.Patch(typeof(LoadBalancingClient), typeof(XeroMain), "OnEvent", "OnEvent", BindingFlags.Static, BindingFlags.NonPublic);
            }
            catch { } // No Events Should be sent Yet
        }

        private bool OnEvent(EventData __0)
        {
            return EventBool;
        }

        private static void OnPlayerJoin(Player player)
        {
            if (player == null)
                return;
            try
            {
                if (player.field_Private_APIUser_0 == null || player.field_Private_APIUser_0.IsSelf)
                {
                    OnLocalPlayerJoined?.DelegateSafeInvoke(player);
                }
            }
            catch { MelonLogger.Error("Error in Player Join"); }
        }

        private static void OnPlayerLeave(Player player)
        {
            if (player == null)
                return;
            try
            {
                if (player.field_Private_APIUser_0 == null || player.field_Private_APIUser_0.IsSelf)
                {
                    OnLocalPlayerLeft?.DelegateSafeInvoke(player);
                }
            }
            catch { MelonLogger.Error("Error in Player Leave"); }
        }

        public static event Action<Player> OnLocalPlayerJoined;

        public string directory = Path.Combine(Environment.CurrentDirectory, "Mods\\XeroSmall.dll");

        public static bool EventBool = true;

        public static event Action<Player> OnLocalPlayerLeft;

        private static string _directory = Path.Combine(Environment.CurrentDirectory, ("UserData\\Xero"));

        public static Sprite SenkoSprite;
    }
}
