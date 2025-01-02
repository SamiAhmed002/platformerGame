using UnityEngine;

public class PortalGunHolderEvents : MonoBehaviour
{
    public CutSceneController cutSceneController;

    public void OnPortalGunPickupComplete()
    {
        if (cutSceneController != null)
        {
            cutSceneController.OnPortalGunPickupComplete();
        }
        else
        {
            Debug.LogError("CutSceneController reference is missing!");
        }
    }
}
