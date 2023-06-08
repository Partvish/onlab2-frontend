using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ModellController : MonoBehaviour
{
    public virtual void OnHit(string entityID, string myId)
    {
        
        if (!entityID.Equals(myId))
        {
            string id = gameObject.GetComponentInParent<PlayerController>().Id;
            Debug.Log("Shot to: " + id + " From: " + entityID);
        }
    }
}
