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

            if (selectionTransform.CompareTag("Prey"))
            {
                AI_Movement_Prey preyMovement = selectionTransform.GetComponent<AI_Movement_Prey>();
                if (preyMovement != null && !preyMovement.isDead)
                {
                    InteractableObject interactable = selectionTransform.GetComponent<InteractableObject>();
                    string preyName = interactable != null ? interactable.GetItemName() : "Prey";
                    
                    interaction_text.text = preyName + "\n[E] Catch";
                    interaction_Info_UI.SetActive(true);
                }
                else
                {
                    interaction_Info_UI.SetActive(false);
                }
            }
            else if (selectionTransform.GetComponent<InteractableObject>())
            {
                InteractableObject interactable = selectionTransform.GetComponent<InteractableObject>();
                string displayText = interactable.GetItemName();
                
                if (interactable.canBePickedUp)
                {
                    // KIEM TRA NEU LA CONSUMABLE - HIEN THI CONSUME
                    ConsumableItem consumable = selectionTransform.GetComponent<ConsumableItem>();
                    if (consumable != null)
                    {
                        string typeText = consumable.itemType == ConsumableItem.ItemType.Food ? "Eat" :
                                         consumable.itemType == ConsumableItem.ItemType.Water ? "Drink" : "Consume";
                        displayText += $"\n[E] {typeText}";
                        interaction_text.text = displayText;
                        interaction_Info_UI.SetActive(true);
                        return;
                    }
                    
                    AI_Movement_Prey preyMovement = selectionTransform.GetComponent<AI_Movement_Prey>();
                    
                    if (preyMovement != null && !preyMovement.isDead)
                    {
                        displayText += "\n[E] Catch";
                    }
                    else
                    {
                        displayText += "\n[E] Pick Up";
                    }
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