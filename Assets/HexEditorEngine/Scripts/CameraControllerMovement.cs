using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexEditorEngine
{
    public partial class CameraController
    {
        float tlit = 0f;
        float yRotation = 0f;
        public Vector2 mouseSensitivity = Vector2.one;
        public Vector3 wsadSensitivity = Vector3.one;
        public float shiftMultip = 2;
        Vector3 mousePrevPos = Vector3.zero;
        // Start is called before the first frame update
        void InitMovement()
        {
            yRotation = transform.rotation.eulerAngles.y;
            tlit = transform.rotation.eulerAngles.x;
        }

        // Update is called once per frame
        void UpdateMovement()
        {
            if (Input.GetMouseButtonDown(1))
                mousePrevPos = Input.mousePosition;
            if (Input.GetMouseButton(1))
            {
                Vector3 dr = Input.mousePosition - mousePrevPos;
                mousePrevPos = Input.mousePosition;
                dr *= 2;
                yRotation += dr.x * mouseSensitivity.x / 10;
                tlit -= dr.y * mouseSensitivity.y / 10;
                tlit = Mathf.Clamp(tlit, -90, 90);
                transform.rotation = Quaternion.Euler(tlit, yRotation, 0);
            }

            Vector3 d = new Vector3(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical"),
                (Input.GetKey(KeyCode.Q) ? -1 : 0) + (Input.GetKey(KeyCode.E) ? 1 : 0)
            ) * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? shiftMultip : 1) *8;

            transform.position += d.y * transform.forward * wsadSensitivity.x;
            transform.position += d.x * transform.right * wsadSensitivity.y;
            transform.position += d.z * transform.up * wsadSensitivity.z;
        }
    }
}