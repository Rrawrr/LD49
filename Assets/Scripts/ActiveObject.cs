using System.Collections;
using UnityEngine;

namespace project
{
    public class ActiveObject : MonoBehaviour
    {
        [SerializeField] private float delayToChangeObject = 2f;
        private const string TOUCHABLE_TAG = "Touchable";
        private bool isActive;


        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1);
            isActive = true;
        }

        void OnCollisionEnter(Collision other)
        {
            Debug.Log($"Collision from {gameObject.name}");

            if (other.gameObject.tag == TOUCHABLE_TAG)
            {
                GameObject otherGO = other.gameObject;
                ChangeActiveObject(otherGO);

                Controller.I.TryToCompleteFindingObject(other.gameObject);
            }

            Controller.I.isCanJump = true;
        }

        private void ChangeActiveObject(GameObject other)
        {
            if (isActive && other.GetComponent<ActiveObject>() == null)
            {
                Controller.I.SetPlayerObject(other);
                other.GetComponent<MeshRenderer>().material.color = Color.red;
                other.gameObject.AddComponent<ActiveObject>();

                Controller.I.PlayParticlesAtPosition(other.transform.position);

                StartCoroutine(DeactivateOldActiveGameObjectCoroutine());
            }
        }

        IEnumerator DeactivateOldActiveGameObjectCoroutine()
        {
            isActive = false;
            GetComponent<MeshRenderer>().material.color = Color.green;
            yield return new WaitForSeconds(delayToChangeObject);
            Destroy(gameObject.GetComponent<ActiveObject>());
        }
    }
}
