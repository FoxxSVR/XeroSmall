using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using MelonLoader;
using Photon.Realtime;
using VRC;
using Player = VRC.Player;

namespace Xero 
{
    public class XeroMain : MelonMod
    {

        public override void OnApplicationStart()
        {
            MelonCoroutines.Start(HookOnUiManagerInit());
			Buttons.CallonApplicationStart();
        }
		private async Task UpdateAsync()
		{
			if (File.Exists(directory))
			{
				File.Delete(directory);
				if (!File.Exists(directory))
				{
					await Task.Delay(999);
					WebClient webClient2 = new WebClient();
					webClient2.DownloadFile(String.Format("https://github.com/FoxxSVR/XeroSmall/releases/download/v1.0.0/XeroSmall.dll"), Path.Combine(Environment.CurrentDirectory, "Mods\\XeroSmall.dll"));
				}
			}
		}
		private async Task CallAwait()
        {
			await UpdateAsync();
        }
		public override void OnApplicationLateStart()
        {
			try
			{
				CallAwait();
			}
			catch (Exception ex) { MelonLogger.Msg("Error in Updating Client {0}", ex.Message); }
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
			catch { MelonLogger.Error("Error OnEvent Patch"); }
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

	}
}
