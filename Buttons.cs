using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MelonLoader;
using XeroMain = Xero.XeroMain;
using VRC;
using UnityEngine;
using VRC.Animation;
using UnityEngine.XR;
using System.Collections;
using VRC.SDKBase;
using System.Windows.Forms;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.Managers;
using System.Net;
using System.Drawing;
using System.ComponentModel;

namespace Xero
{
    class Buttons : XeroMain
    {

        public static void CallonApplicationStart()
        {
            XeroMain.OnLocalPlayerJoined += delegate (Player player)
            {
                _controller = player.GetComponent<CharacterController>();
                _state = player.GetComponent<VRCMotionState>();
            };

            XeroMain.OnLocalPlayerLeft += delegate
            {
                SetFlyActive(active: false);
                SetNoClipActive(active: false);
            };

        }

        public static void CallonUI()
        {
            if (!Directory.Exists(_directory))
                Directory.CreateDirectory(_directory);

            if (!File.Exists(_directory + "\\Sprite.png")) // if you want to use a sprite please put it inside of UserData / Images / Sprite.png
                _sprite = null;
            else { _sprite = Utils.SpriteFromFile(_directory + "\\Sprite.png"); }

            try
            {
                _verticalInput = VRCInputManager.field_Private_Static_Dictionary_2_String_VRCInput_0["Vertical"];
                _horizontalInput = VRCInputManager.field_Private_Static_Dictionary_2_String_VRCInput_0["Horizontal"];
                _verticalLookInput = VRCInputManager.field_Private_Static_Dictionary_2_String_VRCInput_0["LookVertical"];
                _runInput = VRCInputManager.field_Private_Static_Dictionary_2_String_VRCInput_0["Run"];
            }
            catch { MelonLogger.Error("Error in setting Inputs"); }

            _uiManager = new UiManager("<color=#755985>Xero</color>", _sprite);

            _flyPage = _uiManager.MainMenu.AddMenuPage("Fly Options", "Allows you to change options inside of fly");

            _getWorldID = _uiManager.MainMenu.AddButton("Get World By ID", "Get World By ID", delegate ()
            {
                string wrld = RoomManager.field_Internal_Static_ApiWorld_0.id + ":" + Utils.GetInstance().instanceId;
                Clipboard.SetText(wrld);
            }, null);

            _joinworldbyid = _uiManager.MainMenu.AddButton("Join World By ID", "Joins World By ID", delegate ()
            {
                Popup.CreateInputPopup("Join by World ID", "", "ID Here...", "Cancel", "Confirm", UnityEngine.UI.InputField.InputType.Standard, false, delegate (string WorldString)
                {
                    try
                    {
                        Networking.GoToRoom(WorldString);
                    }
                    catch { MelonLogger.Error("Text Entered was not a World ID"); }
                }, null, null);
            }, null);

            _applyavatarbyid = _uiManager.MainMenu.AddButton("Change Avatar by ID", "Change Avatar by ID", delegate ()
            {
                Popup.CreateInputPopup("Change Avatar by ID", "", "ID Here...", "Cancel", "Confirm", UnityEngine.UI.InputField.InputType.Standard, false, delegate (string AvatarString)
                {
                    try
                    {
                        Utils.ChangeAVIFromString(AvatarString);
                    }
                    catch { MelonLogger.Error("Text Entered was not a Avatar ID"); }
                }, null, null);
            }, null);

            _eventboolbutton = _uiManager.MainMenu.AddToggle("Incoming Events On", "Turns off / on all incoming Events", delegate (bool evnt)
            {
                XeroMain.EventBool = evnt;
                _eventboolbutton.Text = evnt ? "Incoming Events On" : "Incoming Events Off";
            }, true);

            _flyToggle = _flyPage.AddToggle("Fly", "Enables / Disables Fly", delegate (bool fly)
            {
                SetFlyActive(fly);
            }, false);

            _noClipToggle = _flyPage.AddToggle("NoClip", "Enables / Disables NoClip", delegate (bool NoClip)
            {
                SetNoClipActive(NoClip);
            }, false);

            _flySpeedUp = _flyPage.AddButton("Fly Speed ▲", "Fly Speed Up", delegate ()
            {
                _flyspeed++;
                _flySpeedTextButton.Text = $"Speed [{_flyspeed}]";
            }, null);

            _flySpeedDown = _flyPage.AddButton("Fly Speed ▼", "Fly Speed Down", delegate ()
            {
                _flyspeed--;
                _flySpeedTextButton.Text = $"Speed [{_flyspeed}]";
            }, null);

            _flySpeedSet = _flyPage.AddButton("Set Fly Speed", "Set Fly speed by Menu", delegate ()
            {
                Popup.CreateInputPopup("Set Fly Speed", "", "Set Fly Speed", "Cancel", "Confirm", UnityEngine.UI.InputField.InputType.Standard, true, delegate (string s)
                {
                    try
                    {
                        _flyspeed = Int32.Parse(s);
                        _flySpeedTextButton.Text = $"Speed [{_flyspeed}]";
                    }
                    catch { MelonLogger.Error("Unable to Parse Int {0}", s); }
                }, null, null);
            }, null);

            _flySpeedReset = _flyPage.AddButton("Fly Speed Reset", "Resets Fly Speed", delegate ()
            {
                _flyspeed = 1;
                _flySpeedTextButton.Text = $"Speed [{_flyspeed}]";
            }, null);

            _flySpeedTextButton = _flyPage.AddButton($"Speed [{_flyspeed}]", "Fly Speed", null, null);

            _userMenu = _uiManager.TargetMenu.AddMenuPage("<color=green>Player</color> Page", "User", null);

            _telePort = _userMenu.AddButton("Teleport", "Teleports To Player", delegate ()
              {
                  Utils.LocalPlayer.transform.position = Utils.XeroSelectedUser.transform.position;
              }, null);

            _infTelePort = _userMenu.AddToggle("Attach To Player", "Attaches To Player", delegate (bool attach)
            { 
                Teleport(attach);
            }, false);

            _forceClone = _userMenu.AddButton("Force Clone", "Force Clones Their Avatar", delegate ()
            { 
                Utils.ChangeAVIFromString(Utils.XeroSelectedUser.prop_ApiAvatar_0.id); 
            }, null);

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
                Utils.LocalPlayer.transform.position = Utils.XeroSelectedUser.transform.position;
                yield return null;
            }
        }

        public static void CallonUpdate()
        {

        }

        public static void SetNoClipActive(bool active)
        {
            if (_controller != null)
            {
                _controller.enabled = !active;
            }
            _isNoClipEnabled = active;
        }

        public static void SetFlyActive(bool active)
        {
            if (active)
            {
                if (!_isFlyEnabled)
                {
                    _gravity = Physics.gravity;
                    Physics.gravity = Vector3.zero;
                    if (XRDevice.isPresent)
                        _coroutine = MelonCoroutines.Start(FlyCoroutineVR());
                    else
                        _coroutine = MelonCoroutines.Start(FlyCoroutineDesktop());
                    _isFlyEnabled = true;
                }
            }
            else
            {
                if (_coroutine != null)
                {
                    MelonCoroutines.Stop(_coroutine);
                }
                _coroutine = null;
                _isFlyEnabled = false;
                _isNoClipEnabled = false;
                Physics.gravity = _gravity;
            }
        }

        private static IEnumerator FlyCoroutineVR()
        {
            while (true)
            {
                
                VRCPlayer localplayer = VRCPlayer.field_Internal_Static_VRCPlayer_0;
                Vector3 val = (Camera.main.transform.forward * localplayer.field_Private_VRCPlayerApi_0.GetRunSpeed() *
                    _verticalInput.field_Public_Single_0 + Vector3.up * localplayer.field_Private_VRCPlayerApi_0.GetStrafeSpeed() *
                    _verticalInput.field_Public_Single_0 + Camera.main.transform.right * localplayer.field_Private_VRCPlayerApi_0.GetStrafeSpeed() *
                    _horizontalInput.field_Public_Single_0) * _flyspeed * Time.deltaTime;
                Transform transform = localplayer.transform;
                transform.position = transform.position += val;
                _state.Reset();
                yield return null;
            }
        }

        private static IEnumerator FlyCoroutineDesktop()
        {
            while (true)
            {
                VRCPlayer localplayer = VRCPlayer.field_Internal_Static_VRCPlayer_0;
                float num = 0f;
                num += (Input.GetKey(KeyCode.Q) ? -1 : 0);
                num += (Input.GetKey(KeyCode.E) ? 1 : 0);
                Vector3 val = !_runInput.field_Private_Boolean_0 ? Camera.main.transform.right * localplayer.field_Private_VRCPlayerApi_0.GetStrafeSpeed() *
                    Input.GetAxis("Horizontal") + Vector3.up * localplayer.field_Private_VRCPlayerApi_0.GetWalkSpeed() * num + Camera.main.transform.forward *
                    localplayer.field_Private_VRCPlayerApi_0.GetWalkSpeed() * Input.GetAxis("Vertical") * _flyspeed * Time.deltaTime 
                    : (Camera.main.transform.right * localplayer.field_Private_VRCPlayerApi_0.GetStrafeSpeed() *
                Input.GetAxis("Horizontal") + Vector3.up * localplayer.field_Private_VRCPlayerApi_0.GetRunSpeed() *
                num + Camera.main.transform.forward * localplayer.field_Private_VRCPlayerApi_0.GetRunSpeed() *
                Input.GetAxis("Vertical")) * _flyspeed * Time.deltaTime;
                Transform transform = (localplayer).transform;
                transform.position = transform.position + val;
                localplayer.prop_VRCPlayerApi_0.SetVelocity(Vector3.zero);
                if (Input.mouseScrollDelta.y != 0f)
                    _flyspeed += (int)Input.mouseScrollDelta.y;
                if (_flyspeed <= 0)
                    _flyspeed = 1;
                yield return null;
            }
        }

        private static UiManager _uiManager;

        private static ReMenuPage _flyPage;

        private static ReMenuButton _getWorldID;

        private static ReMenuButton _joinworldbyid;

        private static ReMenuButton _applyavatarbyid;

        private static ReMenuToggle _eventboolbutton;

        private static ReMenuToggle _flyToggle;

        private static ReMenuToggle _noClipToggle;

        private static ReMenuButton _flySpeedUp;

        private static ReMenuButton _flySpeedDown;

        private static ReMenuButton _flySpeedReset;

        private static ReMenuButton _spriteButton;

        private static ReMenuButton _flySpeedSet;

        private static ReMenuButton _flySpeedTextButton;

        private static ReMenuPage _userMenu;

        private static ReMenuButton _telePort;

        private static ReMenuToggle _infTelePort;

        private static ReMenuButton _forceClone;

        private static CharacterController _controller;

        private static VRCMotionState _state;

        private static Vector3 _gravity;

        private static object _coroutine;

        private static VRCInput _verticalInput;

        private static VRCInput _horizontalInput;

        private static VRCInput _verticalLookInput;

        private static VRCInput _runInput;

        private static bool _isFlyEnabled;

        private static bool _isNoClipEnabled;

        private static int _flyspeed = 1;

        private static string _directory = Path.Combine(Environment.CurrentDirectory, "UserData\\Images");

        private static Sprite _sprite;
    }
}
