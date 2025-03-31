using UnityEngine;
using System.Collections;
using Cinemachine;

public class FrameSwitch : MonoBehaviour
{
    [Header("Frame Settings")]
    public GameObject frameBack; // Previous frame (Frame 1)
    public GameObject frameForward; // Next frame (Frame 2)
    public Transform player;

    [Header("Spawn Points (Leave Empty If Falling)")]
    public Transform frameForwardSpawnPoint;
    public Transform frameBackSpawnPoint;

    private Collider2D playerCollider;
    private Rigidbody2D playerRb;
    private CinemachineBrain cinemachineBrain;
    private CinemachineVirtualCamera activeVirtualCam;
    private float originalGravityScale;
    private bool transitionInProgress = false; // ? Prevents multiple activations

    private void Start()
    {
        if (player != null)
        {
            playerCollider = player.GetComponent<Collider2D>();
            playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                originalGravityScale = playerRb.gravityScale;
            }
        }

        cinemachineBrain = Camera.main?.GetComponent<CinemachineBrain>();
        if (cinemachineBrain != null)
        {
            activeVirtualCam = cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && playerRb != null && !transitionInProgress)
        {
            transitionInProgress = true; // ? Prevents multiple activations

            if (frameForward != null && frameBack != null)
            {
                bool movingForward = frameBack.activeSelf; // ? Checks if moving forward

                Debug.Log(movingForward
                    ? $"? Moving FORWARD: {frameBack.name} ? {frameForward.name}"
                    : $"?? Moving BACK: {frameForward.name} ? {frameBack.name}");

                // **Determine new position**
                Vector3 newPosition = player.position;
                if (movingForward && frameForwardSpawnPoint != null)
                {
                    newPosition = frameForwardSpawnPoint.position;
                }
                else if (!movingForward && frameBackSpawnPoint != null)
                {
                    newPosition = frameBackSpawnPoint.position;
                }
                else
                {
                    Debug.LogWarning("?? No spawn point assigned! Using current position.");
                }

                player.position = newPosition;
                Physics2D.SyncTransforms();

                // **Switch frames based on direction**
                if (movingForward)
                {
                    if (!frameForward.activeSelf)
                    {
                        frameForward.SetActive(true);
                        Debug.Log($"? Enabled {frameForward.name}");
                    }
                    frameBack.SetActive(false);
                    Debug.Log($"? Disabled {frameBack.name}");
                }
                else
                {
                    if (!frameBack.activeSelf)
                    {
                        frameBack.SetActive(true);
                        Debug.Log($"? Enabled {frameBack.name}");
                    }
                    frameForward.SetActive(false);
                    Debug.Log($"? Disabled {frameForward.name}");
                }

                StartCoroutine(ResetTransition());
            }
            else
            {
                Debug.LogError("?? Frame references are missing in FrameSwitch! Check Inspector.");
            }
        }
    }

    private IEnumerator ResetTransition()
    {
        yield return new WaitForFixedUpdate();
        transitionInProgress = false;
    }
}
