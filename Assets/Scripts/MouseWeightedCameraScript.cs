using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseWeightedCameraScript : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float mouseWeight = 0.5f;

    private Vector3 _velocity = Vector3.zero;
    private Coroutine _findPlayerCoroutine;
    private float _cameraZ;                      // Locked Z, set once

    void Start()
    {
        _cameraZ = transform.position.z;         // Lock Z from the start
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartFindingPlayer();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        target = null;
        _velocity = Vector3.zero;                // Clear momentum between scenes
        StartFindingPlayer();
    }

    void StartFindingPlayer()
    {
        if (_findPlayerCoroutine != null)
            StopCoroutine(_findPlayerCoroutine);
        _findPlayerCoroutine = StartCoroutine(RetryFindPlayer());
    }

    IEnumerator RetryFindPlayer()
    {
        while (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                SetTarget(player.transform);
                Debug.Log($"Camera: Locked onto '{player.name}'");
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
            yield return null;
        }
        _findPlayerCoroutine = null;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        // Snap camera to player X/Y, keep our locked Z
        transform.position = new Vector3(target.position.x, target.position.y, _cameraZ);
        _velocity = Vector3.zero;
    }

    void LateUpdate()
    {
        if (!target|| DialogueManager.Instance.isDialogueActive) return;

        // Read player position — only X and Y, so Y-rotation flips don't interfere
        Vector3 playerPos = new Vector3(target.position.x, target.position.y, 0f);

        Vector2 mp = Input.mousePosition;

        bool validMouse = !float.IsNaN(mp.x) && !float.IsNaN(mp.y) &&
                          !float.IsInfinity(mp.x) && !float.IsInfinity(mp.y) &&
                          mp.x >= 0 && mp.x <= Screen.width &&
                          mp.y >= 0 && mp.y <= Screen.height;

        Vector3 mouseOffset = Vector3.zero;

        if (validMouse)
        {
            // Map screen-space mouse [-1, 1] so offset is stable regardless of camera position
            float halfW = Screen.width * 0.5f;
            float halfH = Screen.height * 0.5f;
            Vector2 mouseDir = new Vector2(
                (mp.x - halfW) / halfW,
                (mp.y - halfH) / halfH
            );
            mouseDir = Vector2.ClampMagnitude(mouseDir, 1f);
            mouseOffset = new Vector3(mouseDir.x, mouseDir.y, 0f) * 3f;
        }

        Vector3 desiredPosition = new Vector3(
            playerPos.x + mouseOffset.x * mouseWeight,
            playerPos.y + mouseOffset.y * mouseWeight,
            _cameraZ                                    // Z never drifts
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref _velocity,
            1f / smoothSpeed
        );

        // Hard-lock Z in case SmoothDamp drifts it by a float epsilon
        transform.position = new Vector3(transform.position.x, transform.position.y, _cameraZ);
    }
}