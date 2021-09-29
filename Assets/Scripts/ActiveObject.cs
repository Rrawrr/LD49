using UnityEngine;

namespace project
{
    public class ActiveObject : MonoBehaviour
    {
        private const string TOUCHABLE_TAG = "Touchable";

        void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.tag == TOUCHABLE_TAG)
            {
                Debug.Log("Collision");
                Controller.I.SetPlayerObject(other.gameObject);
                other.gameObject.AddComponent<ActiveObject>();
                Destroy(gameObject.GetComponent<ActiveObject>());

            }
        }
    }
}
