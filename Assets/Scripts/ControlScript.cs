using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class ControlScript : MonoBehaviour
{
    public GameObject _cube;

    private GameObject _cube2; 
    private MLInputController _controller;

    Vector3 _controllerPositionOffset;
    Quaternion _controllerOrientationOffset;


    private bool _drag = false; 


    public enum ButtonStates
    {
        Normal,
        Pressed,
        JustReleased
    };


    //BUtton State field
    public ButtonStates BtnState;






    void Start()
    {

        _cube2 = GameObject.Find("Cube"); 

        MLInput.Start();


        BtnState = ButtonStates.Normal;



        MLInput.OnControllerButtonDown += OnButtonDown;
        MLInput.OnControllerButtonUp += OnButtonUp;

        _controller = MLInput.GetController(MLInput.Hand.Left);

        
    }

    void OnDestroy()
    {
        MLInput.OnControllerButtonDown -= OnButtonDown;
        MLInput.OnControllerButtonUp -= OnButtonUp;
        MLInput.Stop();
    }

    void Update()
    {

        transform.position = _controller.Position;
        transform.rotation = _controller.Orientation; 

        if (BtnState == ButtonStates.JustReleased)
        {
            BtnState = ButtonStates.Normal;
        }

    }


     void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Cube") && _controller.TriggerValue > 0.2f)
        {
            Vector3 relativeDirection = other.transform.position - transform.position;
            _controllerPositionOffset = other.transform.InverseTransformDirection(relativeDirection);
            _controllerOrientationOffset = Quaternion.Inverse(transform.rotation) * other.transform.rotation;


            other.transform.position = transform.position + transform.TransformDirection(_controllerPositionOffset);
            other.transform.rotation = transform.rotation * _controllerOrientationOffset;
        }
    }

    void createCube()
    {
        Instantiate(_cube, _controller.Position + Vector3.forward * 0.1f , _controller.Orientation);
    }


    void OnButtonDown(byte controller_id, MLInputControllerButton button)
    {
        if (button == MLInputControllerButton.Bumper)
        {

            createCube(); 

            BtnState = ButtonStates.Pressed;
        }
    }

    void OnButtonUp(byte controller_id, MLInputControllerButton button)
    {
        if (button == MLInputControllerButton.Bumper)
        {
            BtnState = ButtonStates.JustReleased;
        }
    }

}