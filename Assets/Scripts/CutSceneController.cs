//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System.Collections;

//public class CutSceneController : MonoBehaviour
//{
//    public float cutSceneDuration = 10f; // Duration of the cut-scene in seconds

//    void Start()
//    {
//        // Automatically transition after the cut-scene duration
//        StartCoroutine(EndCutScene());
//    }

//    IEnumerator EndCutScene()
//    {
//        yield return new WaitForSeconds(cutSceneDuration);

//        // Transition to the tutorial scene
//        SpawnLocation.spawnPosition = new Vector3(127, 6, 0);
//        SceneManager.LoadScene("SampleScene"); // Replace with your tutorial scene name
//    }
//}

//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System.Collections;

//public class CutSceneController : MonoBehaviour
//{
//    public float cutSceneDuration = 3600f; // Total duration of the cut-scene (testing purposes)
//    public Animator cameraAnimator;       // Reference to the camera Animator
//    public Animator portalGunAnimator;    // Reference to the portal gun Animator
//    public string cameraAnimationName = "CameraAnimation"; // Camera animation state name

//    void Start()
//    {
//        Debug.Log("Starting cut-scene sequence...");
//        StartCoroutine(PlayCutSceneSequence());
//    }

//    private IEnumerator PlayCutSceneSequence()
//    {
//        // Play the first animation (CameraAnimation)
//        Debug.Log("Playing Camera Animation...");
//        cameraAnimator.Play(cameraAnimationName);

//        // Wait until the first animation is complete
//        while (true)
//        {
//            var animStateInfo = cameraAnimator.GetCurrentAnimatorStateInfo(0);
//            if (animStateInfo.IsName(cameraAnimationName) && animStateInfo.normalizedTime >= 1f)
//            {
//                break; // Animation is complete
//            }
//            yield return null; // Wait for the next frame
//        }
//        Debug.Log("Camera Animation complete.");

//        // Trigger the portal gun animation
//        Debug.Log("Triggering Portal Gun Animation...");
//        portalGunAnimator.SetTrigger("StartPickingUpGun");

//        // Wait until the portal gun animation is complete
//        while (true)
//        {
//            var animStateInfo = portalGunAnimator.GetCurrentAnimatorStateInfo(0);
//            if (animStateInfo.IsName("PickingupPortalGunAnimation") && animStateInfo.normalizedTime >= 1f)
//            {
//                break; // Animation is complete
//            }
//            yield return null; // Wait for the next frame
//        }
//        Debug.Log("Portal Gun Animation complete.");

//        // Wait for the remaining duration of the cut-scene
//        float remainingDuration = cutSceneDuration - (cameraAnimator.GetCurrentAnimatorStateInfo(0).length + portalGunAnimator.GetCurrentAnimatorStateInfo(0).length);
//        Debug.Log($"Remaining cut-scene duration: {remainingDuration}");
//        if (remainingDuration > 0)
//        {
//            Debug.Log("Waiting for remaining cut-scene duration...");
//            yield return new WaitForSeconds(remainingDuration);
//        }

//        // Transition to the tutorial scene
//        Debug.Log("Transitioning to the tutorial scene...");
//        SpawnLocation.spawnPosition = new Vector3(127, 6, 0);
//        SceneManager.LoadScene("SampleScene"); // Replace with your tutorial scene name
//    }
//}


using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutSceneController : MonoBehaviour
{
    public float cutSceneDuration = 3600f; // Total duration of the cut-scene (testing purposes)
    public Animator cameraAnimator;       // Reference to the camera Animator
    public Animator portalGunAnimator;    // Reference to the portal gun Animator
    public Animator finalSceneAnimator;   // Reference to the FinalSceneManager Animator

    public string cameraAnimationName = "CameraAnimation";         // Camera animation state name
    public string portalGunAnimationName = "PickingupPortalGunAnimation"; // Portal gun animation state name
    public string finalAnimationName = "FinalSceneAnimation";      // Final animation state name

    void Start()
    {
        Debug.Log("Starting cut-scene sequence...");
        StartCoroutine(PlayCutSceneSequence());
    }

    private IEnumerator PlayCutSceneSequence()
    {
        // Step 1: Play the Camera Animation
        Debug.Log("Playing Camera Animation...");
        cameraAnimator.Play(cameraAnimationName);

        while (true)
        {
            var animStateInfo = cameraAnimator.GetCurrentAnimatorStateInfo(0);
            if (animStateInfo.IsName(cameraAnimationName) && animStateInfo.normalizedTime >= 1f)
            {
                Debug.Log("Camera Animation complete.");
                break;
            }
            yield return null;
        }

        // Step 2: Play the Portal Gun Animation
        Debug.Log("Triggering Portal Gun Animation...");
        portalGunAnimator.SetTrigger("StartPickingUpGun");

        while (true)
        {
            var animStateInfo = portalGunAnimator.GetCurrentAnimatorStateInfo(0);
            if (animStateInfo.IsName(portalGunAnimationName) && animStateInfo.normalizedTime >= 1f)
            {
                Debug.Log("Portal Gun Animation complete.");
                break;
            }
            yield return null;
        }

        // Step 3: Play the Final Scene Animation
        Debug.Log("Triggering Final Scene Animation...");
        finalSceneAnimator.SetTrigger("StartFinalScene");

        while (true)
        {
            var animStateInfo = finalSceneAnimator.GetCurrentAnimatorStateInfo(0);
            if (animStateInfo.IsName(finalAnimationName) && animStateInfo.normalizedTime >= 1f)
            {
                Debug.Log("Final Scene Animation complete.");
                break;
            }
            yield return null;
        }

        Debug.Log("All animations complete. Remaining cut-scene duration: " + cutSceneDuration);
        yield return new WaitForSeconds(cutSceneDuration);

        Debug.Log("Transitioning to the tutorial scene...");
        SpawnLocation.spawnPosition = new Vector3(127, 6, 0);
        SceneManager.LoadScene("SampleScene");
    }
}





