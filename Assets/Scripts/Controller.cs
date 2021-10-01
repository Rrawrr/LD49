using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace project
{
    public class Controller : MonoBehaviour
    {
        public static Controller I { get; private set; }

        private Camera camera;
        [SerializeField] private float force = 2f;
        [SerializeField] private float cameraRotation = 3f;
        [SerializeField] private GameObject playerObject;
        [SerializeField] private CinemachineVirtualCamera cameraVirt;


        void Awake()
        {
            if (I != null && I != this)
            {
                Destroy(gameObject);
            }
            else
            {
                camera = GetComponent<Camera>();
                I = this;
            }

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

            if (Input.GetKey(KeyCode.A))
            {
                cameraVirt.transform.Rotate(Vector3.up,cameraRotation,Space.World);
            }
            if (Input.GetKey(KeyCode.D))
            {
                cameraVirt.transform.Rotate(Vector3.up, -cameraRotation, Space.World);
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

                if(hit.transform.gameObject == playerObject)
                {
                    Vector3 forceDirection = Vector3.ProjectOnPlane(transform.forward,playerObject.transform.up);
                    hit.transform.GetComponent<Rigidbody>().AddForceAtPosition(forceDirection * force, hit.point, ForceMode.Impulse);
                    Debug.DrawRay(hit.point, forceDirection, Color.green, Mathf.Infinity);
                }

            }
        }

        public void SetPlayerObject(GameObject go)
        {
            playerObject = go;
            cameraVirt.Follow = go.transform;
            cameraVirt.LookAt = go.transform;
        }
    }
}
