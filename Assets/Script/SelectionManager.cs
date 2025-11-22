using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public GameObject interaction_Info_UI;
    Text interaction_text;

    private void Start()
    {
        interaction_text = interaction_Info_UI.GetComponent<Text>();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;

            if (selectionTransform.GetComponent<InteractableObject>())
            {
                InteractableObject interactable = selectionTransform.GetComponent<InteractableObject>();
                
                // Hiển thị tên và hướng dẫn
                string displayText = interactable.GetItemName();
                
                if (interactable.canBePickedUp)
                {
                    // Kiểm tra xem có phải prey không
                    AI_Movement_Prey preyMovement = selectionTransform.GetComponent<AI_Movement_Prey>();
                }
                
                interaction_text.text = displayText;
                interaction_Info_UI.SetActive(true);
            }
            else
            {
                interaction_Info_UI.SetActive(false);
            }
        }
        else
        {
            interaction_Info_UI.SetActive(false);
        }
    }
}