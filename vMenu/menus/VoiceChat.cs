using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuAPI;
using Newtonsoft.Json;
using CitizenFX.Core;
using static CitizenFX.Core.UI.Screen;
using static CitizenFX.Core.Native.API;
using static vMenuClient.CommonFunctions;
using static vMenuShared.PermissionsManager;

namespace vMenuClient
{
    public class VoiceChat
    {
        // Variables
        private Menu menu;
        public bool EnableVoicechat = UserDefaults.VoiceChatEnabled;
        public bool ShowCurrentSpeaker = UserDefaults.ShowCurrentSpeaker;
        public bool ShowVoiceStatus = UserDefaults.ShowVoiceStatus;
        public float currentProximity = UserDefaults.VoiceChatProximity;
        public List<string> channels = new List<string>()
        {
            "����� 1 (Default)",
            "����� 2",
            "����� 3",
            "����� 4",
        };
        public string currentChannel;
        private List<float> proximityRange = new List<float>()
        {
            5f, // 5m
            10f, // 10m
            15f, // 15m
            20f, // 20m
            100f, // 100m
            300f, // 300m
            1000f, // 1.000m
            2000f, // 2.000m
            0f, // global
        };


        private void CreateMenu()
        {
            currentChannel = channels[0];
            if (IsAllowed(Permission.VCStaffChannel))
            {
                channels.Add("Staff Channel");
            }

            // Create the menu.
            menu = new Menu("YDDY:RP", "��������� ���������� ����");

            MenuCheckboxItem voiceChatEnabled = new MenuCheckboxItem("�������� ��������� ���", "", EnableVoicechat);
            MenuCheckboxItem showCurrentSpeaker = new MenuCheckboxItem("���������� ����������", "", ShowCurrentSpeaker);
            MenuCheckboxItem showVoiceStatus = new MenuCheckboxItem("������ ���������", "", ShowVoiceStatus);

            List<string> proximity = new List<string>()
            {
                "5 m",
                "10 m",
                "15 m",
                "20 m",
                "100 m",
                "300 m",
                "1 km",
                "2 km",
                "����������",
            };
            MenuListItem voiceChatProximity = new MenuListItem("��������� ����", proximity, proximityRange.IndexOf(currentProximity), "��������� ���������� ���������� ����.");
            MenuListItem voiceChatChannel = new MenuListItem("����� ��������� ����", channels, channels.IndexOf(currentChannel), "");

            if (IsAllowed(Permission.VCEnable))
            {
                menu.AddMenuItem(voiceChatEnabled);

                // Nested permissions because without voice chat enabled, you wouldn't be able to use these settings anyway.
                if (IsAllowed(Permission.VCShowSpeaker))
                {
                    menu.AddMenuItem(showCurrentSpeaker);
                }

                menu.AddMenuItem(voiceChatProximity);
                menu.AddMenuItem(voiceChatChannel);
                menu.AddMenuItem(showVoiceStatus);
            }

            menu.OnCheckboxChange += (sender, item, index, _checked) =>
            {
                if (item == voiceChatEnabled)
                {
                    EnableVoicechat = _checked;
                }
                else if (item == showCurrentSpeaker)
                {
                    ShowCurrentSpeaker = _checked;
                }
                else if (item == showVoiceStatus)
                {
                    ShowVoiceStatus = _checked;
                }
            };

            menu.OnListIndexChange += (sender, item, oldIndex, newIndex, itemIndex) =>
            {
                if (item == voiceChatProximity)
                {
                    currentProximity = proximityRange[newIndex];
                    Subtitle.Custom($"����� ���������: ~b~{proximity[newIndex]}~s~.");
                }
                else if (item == voiceChatChannel)
                {
                    currentChannel = channels[newIndex];
                    Subtitle.Custom($"����� �����: ~b~{channels[newIndex]}~s~.");
                }
            };

        }

        /// <summary>
        /// Create the menu if it doesn't exist, and then returns it.
        /// </summary>
        /// <returns>The Menu</returns>
        public Menu GetMenu()
        {
            if (menu == null)
            {
                CreateMenu();
            }
            return menu;
        }
    }
}
