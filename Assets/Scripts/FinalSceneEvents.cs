using UnityEngine;

public class FinalSceneEvents : MonoBehaviour
{
    public CutSceneController cutSceneController;

    public void OnFinalAnimationComplete()
    {
        if (cutSceneController != null)
        {
            Debug.Log("Final animation completed. Notifying CutSceneController to transition to the next scene...");
            cutSceneController.TransitionToNextScene();
        }
        else
        {
            Debug.LogError("CutSceneController reference is missing!");
        }
    }
}
