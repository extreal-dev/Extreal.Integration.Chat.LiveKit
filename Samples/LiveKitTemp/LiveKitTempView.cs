using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Extreal.Integration.Chat.LiveKit.LiveKitTemp
{
    public class LiveKitTempView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private Button connectButton;
        [SerializeField] private Button disconnectButton;

        [SerializeField] private TextMeshProUGUI muteText;
        [SerializeField] private Button muteUnmuteButton;

        [SerializeField] private Slider volumeSlider;
        [SerializeField] private TextMeshProUGUI volumeText;

        public IObservable<string> OnConnectButtonClicked => connectButton.OnClickAsObservable().Select(_ => nameInputField.text).TakeUntilDestroy(this);
        public IObservable<Unit> OnDisconnectButtonClicked => disconnectButton.OnClickAsObservable().TakeUntilDestroy(this);

        public IObservable<Unit> OnMuteUnmuteButtonClicked => muteUnmuteButton.OnClickAsObservable().TakeUntilDestroy(this);

        public void SetMuteText(bool isMute)
        {
            var text = $"Mic: {(isMute ? "Mute" : "Unmute")}";
            muteText.text = text;
        }

        public IObservable<float> OnVolumeSliderChange => volumeSlider.OnValueChangedAsObservable().TakeUntilDestroy(this);

        public void SetVolumeText(float value)
            => volumeText.text = $"Volume: {value:F2}";
    }
}
