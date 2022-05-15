using MelonLoader;
using ReMod.Core.VRChat;
using System;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.Core;
using VRC.DataModel;
using XeroMain = Xero.XeroMain; 

namespace Xero
{
    public static class Utils
    {
        private static GameObject avatarObject;
        private static ApiAvatar a;

        public static Sprite LoadSpriteFromDisk(this string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            byte[] array = File.ReadAllBytes(path);
            if (array == null || array.Length == 0)
                return null;

            Texture2D texture2D = new Texture2D(512, 512);
            if (!Il2CppImageConversionManager.LoadImage(texture2D, array))
                return null;

            Sprite sprite = Sprite.CreateSprite(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100f, 0U, 0, default(Vector4), false);
            sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return sprite;
        }

        public static void LoadSpriteFromUrl(this Image Instance, string url)
        {
            if (string.IsNullOrEmpty(url))
                return;

            byte[] array = new WebClient().DownloadData(url);
            if (array == null || array.Length == 0)
                return;

            Texture2D texture2D = new Texture2D(512, 512);
            if (!Il2CppImageConversionManager.LoadImage(texture2D, array))
                return;

            Sprite sprite = Sprite.CreateSprite(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100f, 0U, 0, default(Vector4), false);
            sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            Instance.sprite = sprite;
            Instance.color = Color.white;
        }
        public static void DelegateSafeInvoke(this Delegate @delegate, params object[] args)
        {
            Delegate[] invocationList = @delegate.GetInvocationList();
            for (int i = 0; i < invocationList.Length; i++)
            {
                try
                {
                    invocationList[i].DynamicInvoke(args);
                }
                catch { MelonLogger.Error("Error in DelegateSafeInvoke"); }
            }
        }
        public static VRCPlayer LocalPlayer
        {
            get
            {
                return VRCPlayer.field_Internal_Static_VRCPlayer_0;
            }
        }
        public static VRC.Player XeroSelectedUser
        {
            get
            {
                IUser field_Private_IUser_ = QuickMenuEx.SelectedUserLocal.field_Private_IUser_0;
                Player player = PlayerManager.field_Private_Static_PlayerManager_0.GetPlayer(field_Private_IUser_.prop_String_0);
                VRCPlayer vrcplayer = player.GetVRCPlayer();

                return vrcplayer._player;
            }
        }
        public static void ChangeAVIFromString(string stwing)
        {
            try
            {
                avatarObject = new GameObject();
                avatarObject.name = "DESTROYMEPLEASE";
                var avatarPedestal = avatarObject.AddComponent<AvatarPedestal>();
                a = new ApiAvatar
                {
                    id = stwing
                };
                avatarPedestal.field_Private_ApiAvatar_0 = a;
                // Need to test these later.
                avatarPedestal.Method_Private_Void_4();
                GameObject.Destroy(avatarObject);
            }
            catch { if (!stwing.Contains("avtr_")) MelonLogger.Error("String was not an avtr id"); if (a.releaseStatus == "private") MelonLogger.Error("Avatar is not public"); }
           
        }
        public static ApiWorldInstance GetInstance()
        {
            return RoomManager.field_Internal_Static_ApiWorldInstance_0;
        }
    }
}




