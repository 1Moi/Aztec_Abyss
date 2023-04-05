using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private bool setTargetTo = false;
    [SerializeField] private bool destroyAfterInteraction = false;

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Bullet") {
           
            target.SetActive(setTargetTo);
            if (destroyAfterInteraction == true) {
                Destroy(gameObject);
            }
            
            if(setTargetTo == true) 
            {
                setTargetTo = false;
            } 
            else 
            {
                setTargetTo = true;
            }
        }
    }
}
