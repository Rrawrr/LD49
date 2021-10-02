using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace project
{
    public class Controller : MonoBehaviour
    {
        public static Controller I { get; private set; }

        [SerializeField] private CinemachineVirtualCamera cameraVirt;
        [SerializeField] private float force = 2f;
        [SerializeField] private float jumpForce = 3f;
        [SerializeField] private float cameraRotation = 2f;
        [SerializeField] private GameObject _activeObject;

        private GameObject activeObject
        {
            get => _activeObject;
            set
            {
                _activeObject = value;
                activeObjectRigidbody = _activeObject.GetComponent<Rigidbody>();
            }
        }

        private Camera camera;
        private Rigidbody activeObjectRigidbody;
        private int jumpCount;

        private bool _isCanJump;
        public bool isCanJump
        {
            get => _isCanJump;
            set
            {
                jumpCount = 0;
                _isCanJump = value;
            }
        }

        void Awake()
        {
            activeObject = _activeObject;
            SetPlayerObject(activeObject);

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
                AddHorizontalImpulse();
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (isCanJump)
                {
                    AddImpulseToJump();
                }
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

        private void AddHorizontalImpulse()
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log($"{hit.transform.name} was clicked");
                Debug.DrawRay(transform.position, hit.point - transform.position, Color.red, Mathf.Infinity);

                if(hit.transform.gameObject == activeObject)
                {
                    Vector3 forceDirection = Vector3.ProjectOnPlane(transform.forward,Vector3.up);
                    hit.transform.GetComponent<Rigidbody>().AddForceAtPosition(forceDirection * force, hit.point, ForceMode.Impulse);
                    Debug.DrawRay(hit.point, forceDirection, Color.green, Mathf.Infinity);
                }

            }
        }

        private void AddImpulseToJump()
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log($"{hit.transform.name} was clicked");
                Debug.DrawRay(transform.position, hit.point - transform.position, Color.red, Mathf.Infinity);

                if (hit.transform.gameObject == activeObject)
                {
                    Vector3 forceDirection = transform.up;
                    hit.transform.GetComponent<Rigidbody>().AddForce(forceDirection * jumpForce, ForceMode.Impulse);
                    Debug.DrawRay(hit.point, forceDirection, Color.green, Mathf.Infinity);

                    jumpCount++;
                    if (jumpCount >= 2)
                    {
                        isCanJump = false;
                    }
                }

            }
        }

        public void SetPlayerObject(GameObject go)
        {
            activeObject = go;
            cameraVirt.Follow = go.transform;
            cameraVirt.LookAt = go.transform;
        }
    }
}
