using UnityEngine;
using UnityEngine.UI;

public class GlobalVolumeControl : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    void Start()
    {
        // 初始化 Slider 值
        volumeSlider.value = AudioListener.volume;
        // 绑定回调
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }
}
