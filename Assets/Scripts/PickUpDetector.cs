using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpDetector : MonoBehaviour
{
    public float detectionRadius = 2f;
    public LayerMask pickupLayer;

    private PickupItem currentNearlbyItem;

    private void Update()
    {
        DetectNearbyItem();
        if (currentNearlbyItem != null && Input.GetKeyDown(KeyCode.F))
        {
            currentNearlbyItem.TryPickUp();
        }
    }

    private void DetectNearbyItem()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius,pickupLayer);

        if (hits.Length > 0)
        {
            PickupItem item = hits[0].GetComponent<PickupItem>();
            if(item != null && item != currentNearlbyItem)
            {
                currentNearlbyItem = item;
                string message = $"Presione F para recoger {item.itemData._itemName}";
                if (item.itemData is AmmoItem)
                {
                    message += $"(x{item.quantity})";
                }
                //mostras mensaje por la pantalla
                Debug.Log(message);
            }
        }else if (currentNearlbyItem != null)
        {
            currentNearlbyItem = null;
            //ocultar los mensajes
        }
    }
}
