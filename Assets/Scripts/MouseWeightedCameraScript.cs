using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class MouseWeightedCameraScript : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField, Range(0f, 1.5f)] private float mouseWeight = 0.5f;
    [SerializeField, Tooltip("Max world-units the camera can shift toward the mouse, regardless of mouseWeight.")]
    private float maxOffset = 3f;

    private static MouseWeightedCameraScript _instance;

    private Vector3 _velocity = Vector3.zero;
    private Coroutine _findPlayerCoroutine;
    private float _cameraZ;                      // Locked Z, set once

    void Awake()
    {
        // Singleton: DontDestroyOnLoad camera must not duplicate on scene load.
        // Without this every scene adds another Main Camera and they fight.
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        _cameraZ = transform.position.z;
    }

    void Start()
    {
        if (_instance != null && _instance != this) return;
        // Don't crash if the first scene has no Player (menu, loading, etc.)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            target = player.transform;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartFindingPlayer();
    }

    void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
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
        // Re-acquire target if it was destroyed/re-spawned mid-scene
        // (e.g. FloorScript.CreatePlayer instantiates a new player without a scene load).
        if (!target)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                SetTarget(player.transform);
            else
                return;
        }

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
            // Clamp total offset so an insane inspector value (e.g. weight = 12)
            // can never throw the camera 30+ units into the void.
            Vector3 rawOffset = new Vector3(mouseDir.x, mouseDir.y, 0f) * maxOffset * mouseWeight;
            mouseOffset = Vector3.ClampMagnitude(rawOffset, maxOffset);
            
            if (mouseDir.magnitude < 0.1f) mouseDir = Vector2.zero;
        }

        Vector3 desiredPosition = new Vector3(
            playerPos.x + mouseOffset.x,
            playerPos.y + mouseOffset.y,
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