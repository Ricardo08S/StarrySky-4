using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject hoverPanel;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Slider cameraSensitivitySlider;
    [SerializeField] private Slider starSizeMinSlider;
    [SerializeField] private Slider starSizeMaxSlider;

    private StarField _starField;

    private void Start()
    {
        if (hoverPanel != null)
        {
            hoverPanel.SetActive(false);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitToMenu);
        }

        if (cameraSensitivitySlider != null)
        {
            cameraSensitivitySlider.onValueChanged.AddListener(OnCameraSensitivityChange);
        }
        if (starSizeMinSlider != null)
        {
            starSizeMinSlider.onValueChanged.AddListener(OnStarSizeMinChange);
        }
        if (starSizeMaxSlider != null)
        {
            starSizeMaxSlider.onValueChanged.AddListener(OnStarSizeMaxChange);
        }

    }

    public void SetStarFieldReference(StarField starField)
    {
        _starField = starField;
    }

    public void ShowHoverScreen()
    {
        if (hoverPanel != null)
        {
            hoverPanel.SetActive(true);
        }
        UpdateSliderValues();
    }


    public void HideHoverScreen()
    {
        if (hoverPanel != null)
        {
            hoverPanel.SetActive(false);
        }
    }


    private void OnCameraSensitivityChange(float value)
    {
        if (_starField != null)
        {
            _starField.SetCameraSensitivity(value);
            Debug.Log($"Camera sensitivity changed to {value}");
        }
        else
        {
            Debug.Log("No star field found");
        }
    }

    private void OnStarSizeMinChange(float value)
    {
        if (_starField != null)
        {
            _starField.SetStarSizeMin(value);
            Debug.Log($"Star size min changed to {value}");
        }
        else
        {
            Debug.Log("No star field found");
        }
    }

    private void OnStarSizeMaxChange(float value)
    {
        if (_starField != null)
        {
            _starField.SetStarSizeMax(value);
            Debug.Log($"Star size max changed to {value}");
        }
        else
        {
            Debug.Log("No star field found");
        }
    }

    void ExitToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    private void UpdateSliderValues()
    {
        if (_starField == null)
        {
            Debug.Log("No star field found");
            return;
        }
        if (cameraSensitivitySlider != null)
        {
            cameraSensitivitySlider.value = _starField.GetCameraSensitivity();
        }
        if (starSizeMinSlider != null)
        {
            starSizeMinSlider.value = _starField.GetStarSizeMin();
        }
        if (starSizeMaxSlider != null)
        {
            starSizeMaxSlider.value = _starField.GetStarSizeMax();
        }
    }
}