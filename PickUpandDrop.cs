using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    public Transform[] objectsToPickUp; // An array of objects you want to pick up
    public Transform playerHand; // The player's hand where the objects should appear
    public KeyCode pickupKey = KeyCode.E; // Key to pick up the objects
    public KeyCode dropKey = KeyCode.Q; // Key to drop the objects
    public float pickupRange = 2.0f; // The range at which you can pick up objects
    private bool[] isObjectPickedUp; // An array to track if each object is picked up
    private Transform[] originalParents; // An array to store the original parents of the objects

    private void Start()
    {
        isObjectPickedUp = new bool[objectsToPickUp.Length];
        originalParents = new Transform[objectsToPickUp.Length];

        for (int i = 0; i < isObjectPickedUp.Length; i++)
        {
            isObjectPickedUp[i] = false;
        }

        // Store the original parents of the objects
        for (int i = 0; i < objectsToPickUp.Length; i++)
        {
            originalParents[i] = objectsToPickUp[i].parent;
        }
    }

    private void Update()
    {
        // Find the nearest object within the pickup range
        int nearestObjectIndex = FindNearestObjectIndex();

        // Check for pickup and drop actions
        if (nearestObjectIndex != -1)
        {
            if (!isObjectPickedUp[nearestObjectIndex] && Input.GetKeyDown(pickupKey))
            {
                PickUpObject(nearestObjectIndex);
            }
            else if (isObjectPickedUp[nearestObjectIndex] && Input.GetKeyDown(dropKey))
            {
                DropObject(nearestObjectIndex);
            }
        }
    }

    int FindNearestObjectIndex()
    {
        int nearestObjectIndex = -1;
        float nearestDistance = pickupRange;

        for (int i = 0; i < objectsToPickUp.Length; i++)
        {
            if (objectsToPickUp[i] != null)
            {
                float distance = Vector3.Distance(playerHand.position, objectsToPickUp[i].position);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestObjectIndex = i;
                }
            }
        }

        return nearestObjectIndex;
    }

    void PickUpObject(int index)
    {
        if (objectsToPickUp[index] != null && playerHand != null)
        {
            // Disable the object's collider
            objectsToPickUp[index].GetComponent<Collider>().enabled = false;

            // Calculate an offset to avoid collision with the player
            Vector3 offset = Vector3.down * 0.45f; // Adjust this value as needed

            // Set the object's parent to the player's hand with the offset
            objectsToPickUp[index].transform.SetParent(playerHand);
            objectsToPickUp[index].transform.localPosition = offset;

            // Reset local rotation
            objectsToPickUp[index].transform.localRotation = Quaternion.identity;

            isObjectPickedUp[index] = true;
        }
    }


    void DropObject(int index)
    {
        if (isObjectPickedUp[index])
        {
            // Enable the object's collider
            objectsToPickUp[index].GetComponent<Collider>().enabled = true;

            // Return the object to its original parent
            objectsToPickUp[index].transform.SetParent(originalParents[index]);

            isObjectPickedUp[index] = false;
        }
    }
}