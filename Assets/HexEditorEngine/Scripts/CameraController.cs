using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexEditorEngine
{
    public partial class CameraController : SerializedMonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            InitMovement();
            StartHexEditor();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateMovement();
            UpdateHexEditor();
        }
    }
}


