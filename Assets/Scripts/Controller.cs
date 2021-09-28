using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace project
{
    public class Controller : MonoBehaviour
    {
        private Camera camera;

        [SerializeField] private float force;

        void Awake()
        {
            camera = GetComponent<Camera>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HitObject(force);
            }
            if (Input.GetMouseButtonDown(1))
            {
                HitObject(force * 2);
            }
        }

        private void HitObject(float force)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log($"{hit.transform.name} was clicked");
                Debug.DrawRay(transform.position, hit.point - transform.position, Color.red, Mathf.Infinity);

                if (hit.transform.GetComponent<Player>())
                {
                    hit.transform.GetComponent<Rigidbody>().AddForceAtPosition(Vector3.forward * force, hit.point, ForceMode.Impulse);//TODO change the angle of impulse
                }
            }
        }
    }
}
