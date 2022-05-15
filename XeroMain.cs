using System;
using MelonLoader;
using VRC;

namespace Xero
{
    public class XeroMain : MelonMod
    {
        public override void OnApplicationStart()
        {
            MelonCoroutines.Start(HookOnUiManagerInit());
			Buttons.CallonApplicationStart();
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

		public static event Action<Player> OnLocalPlayerLeft;

	}
}
