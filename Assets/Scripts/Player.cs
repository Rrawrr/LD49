using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace project
{
    public class Player : MonoBehaviour
    {
        private const string TOUCHABLE_TAG = "Touchable";
        [SerializeField] private GameObject playerBody;

        void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.tag == TOUCHABLE_TAG)
            {
                Debug.Log("Collision");
                
            }
        }
    }
}
