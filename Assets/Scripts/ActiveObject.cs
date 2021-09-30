using System.Collections;
using UnityEngine;

namespace project
{
    public class ActiveObject : MonoBehaviour
    {
        private const string TOUCHABLE_TAG = "Touchable";
        private bool isActive = true;


        void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.tag == TOUCHABLE_TAG)
            {
                GameObject otherGO = other.gameObject;
                ChangeActiveObject(otherGO);
            }
        }

        private void ChangeActiveObject(GameObject other)
        {
            Debug.Log($"Collision from {gameObject.name}");

            if (isActive && other.GetComponent<ActiveObject>() == null)
            {
                Controller.I.SetPlayerObject(other);
                other.GetComponent<MeshRenderer>().material.color = Color.red;
                other.gameObject.AddComponent<ActiveObject>();
                isActive = false;

                StartCoroutine(DeactivateOldActiveGameObjectCoroutine(other));
            }
        }

        IEnumerator DeactivateOldActiveGameObjectCoroutine(GameObject other)
        {
            GetComponent<MeshRenderer>().material.color = Color.green;
            yield return new WaitForSeconds(5);
            Destroy(gameObject.GetComponent<ActiveObject>());
        }
    }
}
