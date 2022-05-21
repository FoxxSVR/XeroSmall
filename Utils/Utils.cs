using MelonLoader;
using System;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.Core;
using VRC.DataModel;
using VRC.UI.Elements.Menus;
using XeroMain = Xero.XeroMain;

namespace Xero
{
    public static class Utils
    {
        private static GameObject avatarObject;
        private static ApiAvatar a;
        private static VRC.UI.Elements.QuickMenu _quickMenuInstance;
        private static SelectedUserMenuQM _selectedUserLocal;
        private static Sprite _createdSprite;
        private static Sprite _webSprite;

        public static Sprite SpriteFromFile(string path)
        {
            if (path != null)
            {
                byte[] spritebyte = File.ReadAllBytes(path);
                if (spritebyte != null)
                {
                    Texture2D texture = new Texture2D(512, 512);
                    if (Il2CppImageConversionManager.LoadImage(texture, spritebyte))
                    {
                        Sprite sprite = Sprite.CreateSprite(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f, 0, 0, new Vector4(), false);
                        sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                        _createdSprite = sprite;
                    }
                }
            }
            return _createdSprite;
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
                IUser field_Private_IUser_ = SelectedUserLocal.field_Private_IUser_0;
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
        public static VRC.UI.Elements.QuickMenu Instance
        {
            get
            {
                if (_quickMenuInstance == null)
                {
                    _quickMenuInstance = GameObject.Find("UserInterface").GetComponentInChildren<VRC.UI.Elements.QuickMenu>(true);
                }
                return _quickMenuInstance;
            }
        }

        public static SelectedUserMenuQM SelectedUserLocal
        {
            get
            {
                if (_selectedUserLocal == null)
                {
                    _selectedUserLocal = Instance.field_Public_Transform_0.Find("Window/QMParent/Menu_SelectedUser_Local").GetComponent<SelectedUserMenuQM>();
                }
                return _selectedUserLocal;
            }
        }
        public static Player GetPlayer(this VRCPlayer vrcPlayer)
        {
            return vrcPlayer._player;
        }
        public static VRCPlayer GetVRCPlayer(this Player player)
        {
            return player._vrcplayer;
        }
        public static Player GetPlayer(this PlayerManager playerManager, string userId)
        {
            foreach (Player player in playerManager.GetPlayers())
            {
                if (!(player == null))
                {
                    APIUser apiuser = player.GetAPIUser();
                    if (apiuser != null && apiuser.id == userId)
                    {
                        return player;
                    }
                }
            }
            return null;
        }
        public static Player[] GetPlayers(this PlayerManager playerManager)
        {
            return playerManager.prop_ArrayOf_Player_0;
        }
        public static APIUser GetAPIUser(this Player player)
        {
            return player.field_Private_APIUser_0;
        }
    }
}