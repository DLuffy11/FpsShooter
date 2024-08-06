using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private TextMeshProUGUI _bulletsText;
    [SerializeField] private TextMeshProUGUI _clipsText;

    [SerializeField] private TextMeshProUGUI _winScoreText;
    [SerializeField] private TextMeshProUGUI _loseScoreText;

    [SerializeField] private TextMeshProUGUI _healthPlayer;

    [SerializeField] private GameObject _crosshair;
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject _diedPanel;
    [SerializeField] private GameObject _winPanel;

    [SerializeField] private GameObject _scorePanel;

    [SerializeField] private GameObject _winPanelEnd;
    [SerializeField] private GameObject _losePanelEnd;
    #endregion

    #region Private Fields
    private CharacterControlls _input;
    private PlayerShooting _playerShooting;

    private bool _isMenuActive;
    private int _winScore;
    private int _loseScore;
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        InitializeUI();
        LoadScores();
    }

    private void Update()
    {
        UpdateAmmoDisplay();
    }

    private void OnEnable()
    {
        SetupInputActions();
        RegisterEventListeners();
    }

    private void OnDisable()
    {
        CleanupInputActions();
        UnregisterEventListeners();
    }
    #endregion

    #region UI Methods
    private void InitializeUI()
    {
        _playerShooting = FindObjectOfType<PlayerShooting>();
        _crosshair.SetActive(false);
        _menuPanel.SetActive(false);
        _diedPanel.SetActive(false);
        _winPanel.SetActive(false);
        _scorePanel.SetActive(false);
        _winPanelEnd.SetActive(false);
        _losePanelEnd.SetActive(false);
    }

    private void UpdateAmmoDisplay()
    {
        if (_playerShooting != null && _playerShooting.CurrentWeapon != null)
        {
            _bulletsText.text = $"Bullets: {_playerShooting.CurrentWeapon.CurrentBullets}";
            _clipsText.text = $"Clips: {_playerShooting.CurrentWeapon.CurrentClips}";
        }
    }

    private void ShowMenu(InputAction.CallbackContext context)
    {
        if (_diedPanel.activeSelf || _winPanel.activeSelf)
        {
            return;
        }

        _isMenuActive = !_isMenuActive;
        _menuPanel.SetActive(_isMenuActive);
        Cursor.visible = _isMenuActive;
        Cursor.lockState = _isMenuActive ? CursorLockMode.None : CursorLockMode.Locked; ;
        Time.timeScale = _isMenuActive ? 0f : 1f;
    }

    public void UpdateHealthDisplay(int currentHealth, int maxHealth)
    {
        if (_healthPlayer != null)
        {
            _healthPlayer.text = $"Health: {currentHealth}/{maxHealth}";
        }
    }

    private void ShowCrosshair()
    {
        if (_crosshair != null)
        {
            _crosshair.SetActive(true);
        }
    }

    private void HideCrosshair()
    {
        if (_crosshair != null)
        {
            _crosshair.SetActive(false);
        }
    }

    public void IncrementLoseScore()
    {
        _loseScore++;
        SaveScores();
        UpdateScoreUI();
        ShowDeathUI();
    }

    public void IncrementWinScore()
    {
        _winScore++;
        SaveScores();
        UpdateScoreUI();
        ShowWinUI();
    }

    private void LoadScores()
    {
        _winScore = SaveSystem.LoadWinScore();
        _loseScore = SaveSystem.LoadLoseScore();
        UpdateScoreUI();
    }

    private void SaveScores()
    {
        SaveSystem.SaveScores(_winScore, _loseScore);
    }

    private void UpdateScoreUI()
    {
        if (_winScoreText != null)
        {
            _winScoreText.text = $"Wins: {_winScore}";
        }
        if (_loseScoreText != null)
        {
            _loseScoreText.text = $"Losses: {_loseScore}";
        }
    }

    public void ShowWinUI()
    {
        if (_winPanel != null)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _winPanel.SetActive(true);
            StartCoroutine(ShowDeathUIAfterDelay(_winPanel, 1f, 3f, _winPanelEnd, _scorePanel));
        }
    }

    public void ShowDeathUI()
    {
        if (_diedPanel != null)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _diedPanel.SetActive(true);
            StartCoroutine(ShowDeathUIAfterDelay(_diedPanel, 1f, 3f, _losePanelEnd, _scorePanel));
        }
    }
    #endregion

    #region Button Methods
    public void ResumeGame()
    {
        _isMenuActive = !_isMenuActive;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1.0f;
    }



    public void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ReloadScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        Time.timeScale = 1.0f;
    }
    #endregion

    #region Coroutines
    private IEnumerator ShowDeathUIAfterDelay(GameObject panel, float delay, float fadeDuration, GameObject endPanel, GameObject scorePanel)
    {
        yield return new WaitForSeconds(delay);

        if (panel != null)
        {
            Image panelImage = panel.GetComponent<Image>();
            if (panelImage != null)
            {
                yield return StartCoroutine(FadeInPanel(panelImage, fadeDuration));
                Time.timeScale = 0f;

                if (endPanel != null)
                {
                    endPanel.SetActive(true);
                }
                if (scorePanel != null)
                {
                    scorePanel.SetActive(true);
                }
            }
        }
    }

    private IEnumerator FadeInPanel(Image panelImage, float duration)
    {
        if (panelImage == null) yield break;

        Color color = panelImage.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            color.a = alpha;
            panelImage.color = color;
            yield return null;
        }

        color.a = 1f;
        panelImage.color = color;
    }
    #endregion

    #region Input Handling
    private void SetupInputActions()
    {
        _input = new CharacterControlls();
        _input.Player.Menu.performed += ShowMenu;
        _input.Enable();
    }

    private void CleanupInputActions()
    {
        _input.Player.Menu.performed -= ShowMenu;
        _input.Disable();
    }

    private void RegisterEventListeners()
    {
        EventBus.OnAimPressed += ShowCrosshair;
        EventBus.OnAimReleased += HideCrosshair;
        EventBus.OnCharacterDied += IncrementLoseScore;
    }

    private void UnregisterEventListeners()
    {
        EventBus.OnAimPressed -= ShowCrosshair;
        EventBus.OnAimReleased -= HideCrosshair;
        EventBus.OnCharacterDied -= IncrementLoseScore;
    }
    #endregion
}
