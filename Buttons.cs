using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReMod.Core;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.Managers;
using System.IO;
using MelonLoader;
using XeroMain = Xero.XeroMain;
using VRC;
using UnityEngine;
using VRC.Animation;
using UnityEngine.XR;
using System.Collections;

namespace Xero
{
    class Buttons : ModComponent
    {
        public static void CallonApplicationStart()
        {
            XeroMain.OnLocalPlayerJoined += delegate (Player player)
            {
                Controller = player.GetComponent<CharacterController>();
                State = player.GetComponent<VRCMotionState>();
            };
            XeroMain.OnLocalPlayerLeft += delegate
            {
                SetFlyActive(active: false);
                SetNoClipActive(active: false);
            };
        }
        public static void CallonUI()
        {
            try
            {
                VerticalInput = VRCInputManager.field_Private_Static_Dictionary_2_String_VRCInput_0["Vertical"];
                HorizontalInput = VRCInputManager.field_Private_Static_Dictionary_2_String_VRCInput_0["Horizontal"];
                VerticalLookInput = VRCInputManager.field_Private_Static_Dictionary_2_String_VRCInput_0["LookVertical"];
                RunInput = VRCInputManager.field_Private_Static_Dictionary_2_String_VRCInput_0["Run"];
            }
            catch { MelonLogger.Error("Error in setting Inputs"); }
            _uiManager = new UiManager("<color=#755985>Xero</color> Menu", Path.Combine(Environment.CurrentDirectory, "Xero\\Images\\Senko.png").LoadSpriteFromDisk());
            FlyPage = _uiManager.MainMenu.AddMenuPage("Fly Options", "Allows you to change options inside of fly");
            FlyToggle = FlyPage.AddToggle("Fly", "Enables / Disables Fly", delegate (bool fly)
            {
                SetFlyActive(fly);
            }, false);
            NoClipToggle = FlyPage.AddToggle("NoClip", "Enables / Disables NoClip", delegate (bool NoClip)
            {
                SetNoClipActive(NoClip);
            }, false);
            FlySpeedUp = FlyPage.AddButton("Fly Speed ▲", "Fly Speed Up", delegate ()
            {
                flyspeed++;
                FlySpeedTextButton.Text = $"Speed [{flyspeed}]";
            }, null);
            FlySpeedDown = FlyPage.AddButton("Fly Speed ▼", "Fly Speed Down", delegate ()
            {
                flyspeed--;
                FlySpeedTextButton.Text = $"Speed [{flyspeed}]";
            }, null);
            FlySpeedReset = FlyPage.AddButton("Fly Speed Reset", "Resets Fly Speed", delegate ()
            {
                flyspeed = 1;
                FlySpeedTextButton.Text = $"Speed [{flyspeed}]";
            }, null);
            FlySpeedSet = FlyPage.AddButton("Set Fly Speed", "Set Fly speed by Menu", delegate ()
            {
                Popup.CreateInputPopup("Set Fly Speed", "", "Set Fly Speed", "Cancel", "Confirm", UnityEngine.UI.InputField.InputType.Standard, true, delegate (string s)
                {
                    try
                    {
                        flyspeed = Int32.Parse(s);
                        FlySpeedTextButton.Text = $"Speed [{flyspeed}]";
                    }
                    catch { MelonLogger.Error("Unable to Parse Int {0}", s); }
                }, null, null);
            }, null);
            FlySpeedTextButton = FlyPage.AddButton($"Speed [{flyspeed}]", "Fly Speed", null, null);
            UserMenu = _uiManager.TargetMenu.AddMenuPage("<color=green>Player</color>Page", "User", null);
            TelePort = UserMenu.AddButton("Teleport", "Teleports To Player", delegate ()
              {
                  Utils.LocalPlayer.transform.position = Utils.XeroSelectedUser.transform.position;
              }, null);
            InfTelePort = UserMenu.AddToggle("Attach To Player", "Attaches To Player", delegate (bool attach)
            { Teleport(attach); }, false);
            ForceClone = UserMenu.AddButton("Force Clone", "Force Clones Their Avatar", delegate () 
            { Utils.ChangeAVIFromString(Utils.XeroSelectedUser.prop_ApiAvatar_0.id); }, null);
        }

        private static void Teleport(bool attch)
        {
            if (attch)
            {
                MelonCoroutines.Start(TeleportInf());
            }
            else
            {
                MelonCoroutines.Stop(TeleportInf());
            }
        }
        private static IEnumerator TeleportInf()
        {
            while (true)
            {

                yield return null;
            }
        }
        public static void CallonUpdate()
        {

        }

        public static void SetFlyActive(bool active)
        {
            if (active)
            {
                if (!IsFlyEnabled)
                {
                    gravity = Physics.gravity;
                    Physics.gravity = Vector3.zero;
                    if (XRDevice.isPresent)
                    {
                        coroutine = MelonCoroutines.Start(FlyCoroutineVR());
                    }
                    else
                    {
                        coroutine = MelonCoroutines.Start(FlyCoroutineDesktop());
                    }
                    IsFlyEnabled = true;
                }
            }
            else
            {
                if (coroutine != null)
                {
                    MelonCoroutines.Stop(coroutine);
                }
                coroutine = null;
                IsFlyEnabled = false;
                IsNoClipEnabled = false;
                Physics.gravity = gravity;
            }
        }
        public static void SetNoClipActive(bool active)
        {
            if (Controller != null)
            {
                Controller.enabled = !active;
            }
            IsNoClipEnabled = active;
        }

        private static IEnumerator FlyCoroutineVR()
        {
            while (true)
            {
                VRCPlayer field_Internal_Static_VRCPlayer_ = VRCPlayer.field_Internal_Static_VRCPlayer_0;
                Vector3 val = (Camera.main.transform.forward * field_Internal_Static_VRCPlayer_.field_Private_VRCPlayerApi_0.GetRunSpeed() * VerticalInput.field_Public_Single_0 + Vector3.up * field_Internal_Static_VRCPlayer_.field_Private_VRCPlayerApi_0.GetStrafeSpeed() * VerticalLookInput.field_Public_Single_0 + Camera.main.transform.right * field_Internal_Static_VRCPlayer_.field_Private_VRCPlayerApi_0.GetStrafeSpeed() * HorizontalInput.field_Public_Single_0) * flyspeed * Time.deltaTime;
                Transform transform = field_Internal_Static_VRCPlayer_.transform;
                transform.position = transform.position + val;
                State.Reset();
                yield return null;
            }
        }
        private static IEnumerator FlyCoroutineDesktop()
        {
            while (true)
            {
                VRCPlayer field_Internal_Static_VRCPlayer_ = VRCPlayer.field_Internal_Static_VRCPlayer_0;
                float num = 0f;
                num += (Input.GetKey(KeyCode.Q) ? -1 : 0);
                num += (Input.GetKey(KeyCode.E) ? 1 : 0);
                Vector3 val = !RunInput.field_Private_Boolean_0 ? Camera.main.transform.right * field_Internal_Static_VRCPlayer_.field_Private_VRCPlayerApi_0.GetStrafeSpeed() * Input.GetAxis("Horizontal") + Vector3.up * field_Internal_Static_VRCPlayer_.field_Private_VRCPlayerApi_0.GetWalkSpeed() * num + Camera.main.transform.forward * field_Internal_Static_VRCPlayer_.field_Private_VRCPlayerApi_0.GetWalkSpeed() * Input.GetAxis("Vertical") * flyspeed * Time.deltaTime
                : (Camera.main.transform.right * field_Internal_Static_VRCPlayer_.field_Private_VRCPlayerApi_0.GetStrafeSpeed() * Input.GetAxis("Horizontal") + Vector3.up * field_Internal_Static_VRCPlayer_.field_Private_VRCPlayerApi_0.GetRunSpeed() * num + Camera.main.transform.forward * field_Internal_Static_VRCPlayer_.field_Private_VRCPlayerApi_0.GetRunSpeed() * Input.GetAxis("Vertical")) * flyspeed * Time.deltaTime;
                Transform transform = (field_Internal_Static_VRCPlayer_).transform;
                transform.position = transform.position + val;
                field_Internal_Static_VRCPlayer_.prop_VRCPlayerApi_0.SetVelocity(Vector3.zero);
                yield return null;
            }
        }

        private static UiManager _uiManager;
        private static ReMenuPage FlyPage;
        private static ReMenuToggle FlyToggle;
        public static ReMenuToggle NoClipToggle;
        private static ReMenuButton FlySpeedUp;
        public static ReMenuButton FlySpeedDown;
        public static ReMenuButton FlySpeedReset;
        public static ReMenuButton FlySpeedSet;
        public static ReMenuButton FlySpeedTextButton;
        public static ReMenuPage UserMenu;
        public static ReMenuButton TelePort;
        public static ReMenuToggle InfTelePort;
        public static ReMenuButton ForceClone;
        private static CharacterController Controller;
        private static VRCMotionState State;
        private static Vector3 gravity;
        private static object coroutine;
        public static VRCInput VerticalInput;
        public static VRCInput HorizontalInput;
        public static VRCInput VerticalLookInput;
        public static VRCInput RunInput;
        public static bool IsFlyEnabled;
        public static bool IsNoClipEnabled;
        private static int flyspeed = 1;
    }
}
