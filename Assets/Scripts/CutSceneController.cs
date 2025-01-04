using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutSceneController : MonoBehaviour
{
    public Animator cameraAnimator;       // Reference to the camera Animator
    public Animator portalGunAnimator;    // Reference to the portal gun Animator
    public Animator finalSceneAnimator;   // Reference to the FinalSceneManager Animator

    public string cameraAnimationName = "CameraAnimation";         // Camera animation state name
    public string portalGunAnimationName = "PickingupPortalGunAnimation"; // Portal gun animation state name
    public string finalAnimationName = "FinalSceneAnimation";      // Final animation state name

    public GameObject duplicatePortalGun; // Assign the duplicate portal gun in the Inspector
    public GameObject finalAnimationObject; // Assign the final-animation object in the Inspector

    void Start()
    {
        //Debug.Log("CutSceneController: Starting cut-scene sequence...");
        StartCoroutine(PlayCutSceneSequence());
    }

    private IEnumerator PlayCutSceneSequence()
    {
        // Ensure the final animation object is hidden initially
        finalAnimationObject.SetActive(false);
        //Debug.Log("Final animation object hidden initially.");

        // Step 1: Play the Camera Animation
        //Debug.Log("Playing Camera Animation...");
        cameraAnimator.Play(cameraAnimationName);
        yield return WaitForAnimation(cameraAnimator, cameraAnimationName);
        //Debug.Log("Camera Animation complete.");

        // Step 2: Play the Portal Gun Animation
        //Debug.Log("Triggering Portal Gun Animation...");
        portalGunAnimator.SetTrigger("StartPickingUpGun");
        yield return WaitForAnimation(portalGunAnimator, portalGunAnimationName);
        //Debug.Log("Portal Gun Animation complete.");

        // Step 3: Prepare for Final Scene Animation
        //Debug.Log("Triggering Final Scene Animation...");
        OnPortalGunPickupComplete(); // Ensure the objects are switched before the final animation starts
        finalSceneAnimator.SetTrigger("StartFinalAnimation");
    }

    private IEnumerator WaitForAnimation(Animator animator, string animationName)
    {
        //Debug.Log($"Waiting for animation '{animationName}' to start...");
        bool animationStarted = false;

        while (true)
        {
            var animStateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (animStateInfo.IsName(animationName))
            {
                if (!animationStarted)
                {
                    //Debug.Log($"Animation '{animationName}' started.");
                    animationStarted = true;
                }

                if (animStateInfo.normalizedTime >= 1f)
                {
                    //Debug.Log($"Animation '{animationName}' completed.");
                    break;
                }
            }

            yield return null;
        }
    }

    public void OnPortalGunPickupComplete()
    {
        //Debug.Log("Portal Gun pickup animation complete. Switching objects...");

        // Hide the duplicate portal gun
        duplicatePortalGun.SetActive(false);
        //Debug.Log("Duplicate portal gun hidden.");

        // Unhide the final-animation object
        finalAnimationObject.SetActive(true);
        //Debug.Log("Final animation object activated.");

        // Force the final animation to play
        finalSceneAnimator.Rebind(); // Reset the animator
        finalSceneAnimator.Play(finalAnimationName);
        //Debug.Log($"Final animation '{finalAnimationName}' forced to play.");
    }

    // Function triggered by the Animation Event
    public void TransitionToNextScene()
    {
        //Debug.Log("Transitioning to the next scene...");
        SceneManager.LoadScene("SampleScene");
    }
}










