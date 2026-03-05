using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseContainer;

    public PlayerMovementController playerCharacter;
    public CameraControllerFS playerCamera;

    public void Pause(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        bool isPaused = pauseContainer.activeSelf;

        if (!isPaused)
        {
            pauseContainer.SetActive(true);
            Time.timeScale = 0f;

            playerCharacter.enabled = false;
            playerCamera.enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            ResumeGame();
        }
    }

    public void ResumeButton()
    {
        ResumeGame();
    }

    void ResumeGame()
    {
        pauseContainer.SetActive(false);
        Time.timeScale = 1f;

        playerCharacter.enabled = true;
        playerCamera.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ResetPositionButton()
    {
        ResumeGame();

        playerCharacter.ResetToCheckpoint();
    }

    public void RestartButton()
    {
        Time.timeScale = 1f;

        Checkpoint.savedPosition = Vector3.zero;
        Checkpoint.hasCheckpoint = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}