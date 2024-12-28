using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject linkedPortal;  // Reference to the other portal
    public float teleportCooldown = 2f;  // 2-second cooldown
    private bool isCooldownActive = false;  // Track if cooldown is active

    void OnTriggerEnter(Collider other)
    {
        // Only teleport if:
        // 1. Cooldown is not active
        // 2. There is a linked portal
        // 3. The linked portal exists in the scene
        if (!isCooldownActive && linkedPortal != null && linkedPortal.activeInHierarchy)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(TeleportWithCooldown(other));
            }
            else if (other.CompareTag("Enemy"))
            {
                StartCoroutine(TeleportEnemyWithCooldown(other));
            }
            else if (other.CompareTag("Floating"))
            {
                StartCoroutine(TeleportFloatingObjectWithCooldown(other));
            }
        }
    }

    IEnumerator TeleportEnemyWithCooldown(Collider enemy)
    {
        Debug.Log("Teleporting enemy, starting cooldown...");
        isCooldownActive = true;

        // Calculate the teleport position with a horizontal offset only
        Vector3 horizontalOffset = new Vector3(
            linkedPortal.transform.forward.x,
            0, // Zero out the Y component
            linkedPortal.transform.forward.z
        ).normalized * 2f;

        // Move the enemy to the linked portal's position
        enemy.transform.position = linkedPortal.transform.position + horizontalOffset;
        enemy.GetComponent<CRAZYEnemyMovement>().hasTeleported = false;

        Rigidbody rb = enemy.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false; // Disable gravity after teleport
        }

        Debug.Log("Enemy teleported to: " + enemy.transform.position);  // Log the enemy's new position

        // Also set the cooldown on the linked portal to avoid immediate back-and-forth teleport
        linkedPortal.GetComponent<Portal>().isCooldownActive = true;

        // Wait for the cooldown duration (2 seconds)
        yield return new WaitForSeconds(teleportCooldown);

        // Cooldown ends
        isCooldownActive = false;
        linkedPortal.GetComponent<Portal>().isCooldownActive = false;

        Debug.Log("Cooldown ended, portals are ready.");  // Log when cooldown ends
    }

    IEnumerator TeleportWithCooldown(Collider player)
    {
        Debug.Log("Teleporting player, starting cooldown...");  // Log teleport initiation
        // Set cooldown active
        isCooldownActive = true;

        CharacterController playerController = player.GetComponent<CharacterController>();

        // Disable the CharacterController to avoid any physics issues during teleport
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Move the player to the linked portal's position
        player.transform.position = linkedPortal.transform.position + linkedPortal.transform.forward * 2f;

        Debug.Log("Player teleported to: " + player.transform.position);  // Log the player's new position

        // Re-enable the CharacterController
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // Also set the cooldown on the linked portal to avoid immediate back-and-forth teleport
        linkedPortal.GetComponent<Portal>().isCooldownActive = true;

        // Wait for the cooldown duration (2 seconds)
        yield return new WaitForSeconds(teleportCooldown);

        // Cooldown ends
        isCooldownActive = false;
        linkedPortal.GetComponent<Portal>().isCooldownActive = false;

        Debug.Log("Cooldown ended, portals are ready.");  // Log when cooldown ends
    }

    IEnumerator TeleportFloatingObjectWithCooldown(Collider floatingObject)
    {
        isCooldownActive = true;

        // Get the Rigidbody if it exists
        Rigidbody rb = floatingObject.GetComponent<Rigidbody>();
        Vector3 currentVelocity = Vector3.zero;
        if (rb != null)
        {
            currentVelocity = rb.velocity;
            rb.isKinematic = true;
        }

        // Teleport the object
        floatingObject.transform.position = linkedPortal.transform.position + linkedPortal.transform.forward * 2f;

        // Restore the object's velocity in the new direction
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = linkedPortal.transform.forward * currentVelocity.magnitude;
        }

        // Set cooldown on both portals
        linkedPortal.GetComponent<Portal>().isCooldownActive = true;

        yield return new WaitForSeconds(teleportCooldown);

        isCooldownActive = false;
        linkedPortal.GetComponent<Portal>().isCooldownActive = false;
    }
}
