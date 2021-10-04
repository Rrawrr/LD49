using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        [SerializeField] private TextMeshProUGUI objectsText;
        private GameObject activeObject
        {
            get => _activeObject;
            set
            {
                _activeObject = value;
            }
        }

        [SerializeField] private GameObject particles;
        //[SerializeField] public Dictionary<GameObject,string> objectsToFind;
        [Serializable]
        public struct ObjectToFind
        {
            public GameObject go;
            public string name;
        }

        public List<ObjectToFind> objectsToFind; 


        private Camera camera;
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
            SetObjectsNamesList();

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

            if (Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }
        }

        private void AddHorizontalImpulse()
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log($"{hit.transform.name} was clicked");
                //Debug.DrawRay(transform.position, hit.point - transform.position, Color.red, Mathf.Infinity);

                if(hit.transform.gameObject == activeObject)
                {
                    Vector3 forceDirection = Vector3.ProjectOnPlane(transform.forward,Vector3.up);
                    hit.transform.GetComponent<Rigidbody>().AddForceAtPosition(forceDirection * force, hit.point, ForceMode.Impulse);
                    //Debug.DrawRay(hit.point, forceDirection, Color.green, Mathf.Infinity);
                }

            }
        }

        private void AddImpulseToJump()
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log($"{hit.transform.name} was clicked");
                //Debug.DrawRay(transform.position, hit.point - transform.position, Color.red, Mathf.Infinity);

                if (hit.transform.gameObject == activeObject)
                {
                    Vector3 forceDirection = transform.up;
                    hit.transform.GetComponent<Rigidbody>().AddForce(forceDirection * jumpForce, ForceMode.Impulse);
                    //Debug.DrawRay(hit.point, forceDirection, Color.green, Mathf.Infinity);

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

        public void PlayParticlesAtPosition(Vector3 pos)
        {
            Instantiate(particles, pos,Quaternion.identity);
        }


        private void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private ObjectToFind GetObjectToFind(GameObject obj)
        {
            var o = new ObjectToFind();
            foreach (var objectToFind in objectsToFind)
            {
                if (objectToFind.go == obj)
                {
                    o = objectToFind;
                }
            } 
            return o;
        }

        private bool isObjectToFind(GameObject obj)
        {
            return !string.IsNullOrEmpty(GetObjectToFind(obj).name);
        }

        public void TryToCompleteFindingObject(GameObject obj)
        {
            if (isObjectToFind(obj))
            {
                Debug.Log($"Found object {GetObjectToFind(obj)}");
                var foundedObject = GetObjectToFind(obj);
                objectsToFind.Remove(foundedObject);

                SetObjectsNamesList();
            }
        }

        private void SetObjectsNamesList()
        {
            objectsText.text = string.Empty;

            foreach (var obj in objectsToFind)
            {
                string name = $"{obj.name} \n";
                objectsText.text += name;
            }
        }

    }
}
