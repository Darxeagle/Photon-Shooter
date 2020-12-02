using PhotonShooter.Scripts.Gameplay.Ui;
using UniRx;
using UnityEngine;
using Zenject;

namespace PhotonShooter.Scripts.Gameplay.Actors
{
    public class CharacterInput : MonoBehaviour
    {
        [SerializeField] private Transform moveTransform;
        [SerializeField] private Transform lookTransform;
        [SerializeField] private CharacterController characterController;

        [Inject] private ExitUI exitUI;

        private KeyCode[] alphaNums = {KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3};
        private float moveSpeed = 3f;
        private float sensitivityX = 15F;
        private float sensitivityY = 15F;
        private float minimumX = -360F;
        private float maximumX = 360F;
        private float minimumY = -60F;
        private float maximumY = 60F;
        private float rotationX = 0F;
        private float rotationY = 0F;
        private Quaternion originalRotation;
        
        public bool FirePressed { get; private set; } = false;
        public Subject<int> WeaponSelected { get; private set; } = new Subject<int>();

        void Start()
        {
            originalRotation = transform.rotation;
        }

        void Update()
        {
            if (exitUI.IsOpen) return;
            
            //rotation
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            rotationY = ClampAngle(rotationY, minimumY, maximumY);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);
            moveTransform.localRotation = originalRotation * xQuaternion;
            lookTransform.localRotation = originalRotation * yQuaternion;
            
            //movement
            Vector3 move = moveTransform.right * Input.GetAxis("Horizontal") +
                           moveTransform.forward * Input.GetAxis("Vertical");
            characterController.SimpleMove(move * moveSpeed);

            //shoot
            FirePressed = Input.GetButton("Fire1");
            
            //switch weapons
            for (var i = 0; i < alphaNums.Length; i++)
            {
                if (Input.GetKeyDown(alphaNums[i]))
                {
                    WeaponSelected.OnNext(i);
                }
            }
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}