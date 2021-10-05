using System.Collections;
using UnityEngine;

namespace project
{
    public class ActiveObject : MonoBehaviour
    {
        [SerializeField] private float delayToChangeObject = 3f;
        [SerializeField] private GameObject particles;
        private const string TOUCHABLE_TAG = "Touchable";
        private bool isActive;


        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1);
            isActive = true;
        }

        void OnCollisionEnter(Collision other)
        {
            //Debug.Log($"Collision from {gameObject.name}");

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
                other.GetComponent<MeshRenderer>().material = Controller.I.material;
                other.gameObject.AddComponent<ActiveObject>();

                Controller.I.PlayParticlesAtPosition(other.transform);

                StartCoroutine(DeactivateOldActiveGameObjectCoroutine());
            }
        }

        IEnumerator DeactivateOldActiveGameObjectCoroutine()
        {
            var particles = GetComponentsInChildren<ParticleSystem>();
            foreach (var part in particles)
            {
                part.Stop();
            }
            isActive = false;
            GetComponent<MeshRenderer>().material = Controller.I.material_abadoned;
            yield return new WaitForSeconds(delayToChangeObject);
            Destroy(gameObject.GetComponent<ActiveObject>());
        }


    }
}
