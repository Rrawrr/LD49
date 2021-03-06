using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace project
{
    public class Controller : MonoBehaviour
    {
        public static Controller I { get; private set; }

        [SerializeField] private CinemachineVirtualCamera cameraVirt;
        [SerializeField] private float force = 2f;
        [SerializeField] private float jumpForce = 3f;
        [SerializeField] private float cameraRotation = 2f;
        [SerializeField] private int itemsToSpawn = 10;
        [SerializeField] public Material material;
        [SerializeField] public Material material_abadoned;

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
        [SerializeField] private Transform boundsTransform;
        [Serializable]
        public struct ObjectToFind
        {
            public GameObject go;
            public string name;
        }

        public List<ObjectToFind> objectsToFind; 
        [SerializeField] private List<GameObject> items;
        private AudioSource audioSource;
        [SerializeField] private AudioClip[] clips;
        private int audioNumber;


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
            audioSource = GetComponent<AudioSource>();
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

        void Start()
        {
            //audioSource.clip = clips[0];
            //audioNumber = 0;
            //audioSource.Play();
	    StartCoroutine(routine:PlayMusicCoroutine());
        }

IEnumerator PlayMusicCoroutine()
        {
            while (true)
            {
                foreach (var clip in clips)
                {
                    audioSource.clip = clip;
                    float length = audioSource.clip.length;

                    audioSource.Play();
                    yield return new WaitForSeconds(length);
                }
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

            //if (!audioSource.isPlaying)
            //{
            //    audioNumber++;
            //    audioSource.clip = clips[audioNumber];
            //    audioSource.Play();
            //    if (audioNumber >= 2)
            //    {
            //        audioNumber = 0;
            //    }
            //}

            //if (Input.GetKeyDown(KeyCode.P))
            //{
            //    CreateRandomItems();
            //}
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

        public void PlayParticlesAtPosition(Transform transform)
        {
            Instantiate(particles, transform.position,Quaternion.identity,transform);
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
                //Debug.Log($"Found object {GetObjectToFind(obj)}");
                var foundedObject = GetObjectToFind(obj);
                objectsToFind.Remove(foundedObject);
                SetObjectsNamesList();

                CreateRandomItems();
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



        private void CreateRandomItems()
        {
            for (int i = 0; i < itemsToSpawn; i++)
            {
                var rndItem = items[UnityEngine.Random.Range(0, items.Count)];
                Vector3 rndPos = RandomPointInBounds(boundsTransform.GetComponent<Renderer>().bounds);
                Instantiate(rndItem, rndPos, Quaternion.identity);
            }
            //foreach (var item in items)
            //{
            //    Vector3 rndPos = RandomPointInBounds(boundsTransform.GetComponent<Renderer>().bounds);
            //    Instantiate(item, rndPos, Quaternion.identity);
            //}
        }

        public static Vector3 RandomPointInBounds(Bounds bounds)
        {
            return new Vector3(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
                UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
            );
        }

    }
}
