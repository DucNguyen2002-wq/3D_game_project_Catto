using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    public GameObject player;
    public Transform holdPos;
    
    public float pickUpRange = 5f; //how far the player can pickup the object from
    
    private GameObject heldObj; //object which we pick up
    private Rigidbody heldObjRb; //rigidbody of object we pick up
    private int LayerNumber; //layer index
    private InteractableObject heldObjInteractable; //InteractableObject component
    
    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("holdLayer");
        
        // Kiem tra player da duoc gan chua
        if (player == null)
        {
            Debug.LogError("Player chua duoc gan vao PickUpScript!");
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) //press E to pick up or drop
        {
            if (heldObj == null) //if currently not holding anything
            {
                //perform raycast to check if player is looking at object within pickuprange
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    //make sure pickup tag is attached
                    if (hit.transform.gameObject.tag == "canPickUp")
                    {
                        // KIEM TRA NEU LA CONSUMABLE - TU DONG TIEU THU LUON
                        ConsumableItem consumable = hit.transform.GetComponent<ConsumableItem>();
                        if (consumable != null)
                        {
                            Debug.Log($"[PickUpScript] Auto-consuming {hit.transform.gameObject.name}...");
                            consumable.Consume();
                            return; // Ket thuc, khong pick up
                        }
                        
                        // Kiem tra xem co phai la prey khong
                        AI_Movement_Prey preyMovement = hit.transform.GetComponent<AI_Movement_Prey>();
                        
                        if (preyMovement != null)
                        {
                            // Neu la prey song thi bat (goi Die)
                            if (!preyMovement.isDead)
                            {
                                preyMovement.Die();
                                
                                InteractableObject interactable = hit.transform.GetComponent<InteractableObject>();
                                string itemName = interactable != null ? interactable.GetItemName() : "Prey";
                                Debug.Log($"Successfully caught {itemName}!");
                            }
                            // Neu prey da chet thi nhat xac len
                            else
                            {
                                PickUpObject(hit.transform.gameObject);
                            }
                        }
                        else
                        {
                            // Object thuong - pick up
                            PickUpObject(hit.transform.gameObject);
                        }
                    }
                }
            }
            else
            {
                StopClipping(); //prevents object from clipping through walls
                DropObject();
            }
        }
        
        // Nhan F de an/uong item dang cam
        if (Input.GetKeyDown(KeyCode.F) && heldObj != null)
        {
            ConsumeItem();
        }
        
        if (heldObj != null) //if player is holding object
        {
            MoveObject(); //keep object position at holdPos
        }
    }
    
    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>()) //make sure the object has a RigidBody
        {
            heldObj = pickUpObj; //assign heldObj to the object that was hit by the raycast
            heldObjRb = pickUpObj.GetComponent<Rigidbody>(); //assign Rigidbody
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos.transform; //parent object to holdposition
            
            heldObj.layer = LayerNumber; //change the object layer to the holdLayer
            
            // Ignore collision voi player - an toan hon
            if (player != null)
            {
                Collider objCollider = heldObj.GetComponent<Collider>();
                Collider playerCollider = player.GetComponent<Collider>();
                
                // Neu player khong co Collider, thu tim CharacterController
                if (playerCollider == null)
                {
                    CharacterController charController = player.GetComponent<CharacterController>();
                    if (charController != null)
                    {
                        // CharacterController cung la Collider
                        playerCollider = charController;
                    }
                }
                
                if (objCollider != null && playerCollider != null)
                {
                    Physics.IgnoreCollision(objCollider, playerCollider, true);
                }
            }
            
            // Tat InteractableObject khi dang cam
            heldObjInteractable = heldObj.GetComponent<InteractableObject>();
            if (heldObjInteractable != null)
            {
                heldObjInteractable.enabled = false;
            }
            
            // Xoay object ve goc nam ngang (reset rotation) - Dat SAU KHI parent
            heldObj.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            
            string itemName = heldObjInteractable != null ? heldObjInteractable.GetItemName() : "Object";
            Debug.Log($"Picked up {itemName}");
        }
    }
    
    void DropObject()
    {
        // Re-enable collision voi player - an toan hon
        if (player != null && heldObj != null)
        {
            Collider objCollider = heldObj.GetComponent<Collider>();
            Collider playerCollider = player.GetComponent<Collider>();
            
            // Neu player khong co Collider, thu tim CharacterController
            if (playerCollider == null)
            {
                CharacterController charController = player.GetComponent<CharacterController>();
                if (charController != null)
                {
                    playerCollider = charController;
                }
            }
            
            if (objCollider != null && playerCollider != null)
            {
                Physics.IgnoreCollision(objCollider, playerCollider, false);
            }
        }
        
        heldObj.layer = 0; //object assigned back to default layer
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null; //unparent object
        
        // Bat lai InteractableObject khi tha
        if (heldObjInteractable != null)
        {
            heldObjInteractable.enabled = true;
        }
        
        Debug.Log("Dropped object");
        heldObj = null; //undefine game object
        heldObjInteractable = null; //clear reference
    }
    
    void ConsumeItem()
    {
        ConsumableItem consumable = heldObj.GetComponent<ConsumableItem>();
        
        if (consumable != null)
        {
            consumable.Consume();
            
            heldObj = null;
            heldObjRb = null;
            heldObjInteractable = null;
            
            Debug.Log("[PickUpScript] Item consumed");
        }
        else
        {
            Debug.Log("[PickUpScript] Item cannot be consumed");
        }
    }
    
    void MoveObject()
    {
        //keep object position the same as the holdPosition position
        heldObj.transform.position = holdPos.transform.position;
        
        // Giu object luon o local rotation (0, 90, 0) - khong bi anh huong boi parent rotation
        heldObj.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
    }
    
    void StopClipping() //function only called when dropping
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position); //distance from holdPos to the camera
        //have to use RaycastAll as object blocks raycast in center screen
        //RaycastAll returns array of all colliders hit within the cliprange
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        //if the array length is greater than 1, meaning it has hit more than just the object we are carrying
        if (hits.Length > 1)
        {
            //change object position to camera position 
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player
        }
    }
}
