using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkInputHandler : NetworkBehaviour
{
    [System.Serializable]
    public class InputPackage
    {
        public Vector3 headControllerPosition;
        public Vector3 leftControllerPosition;
        public Vector3 rightControllerPosition;
        public Quaternion headControllerRotation;
        public Quaternion leftControllerRotation;
        public Quaternion rightControllerRotation;

        public Vector2 input_move;
        public bool input_jump;
        public int input_gripLState;
        public int input_gripRState;

        public bool input_fly;

        public InputPackage()
        {
            headControllerPosition = new Vector3();
            leftControllerPosition = new Vector3();
            rightControllerPosition = new Vector3();
            headControllerRotation = Quaternion.identity;
            leftControllerRotation = Quaternion.identity;
            rightControllerRotation = Quaternion.identity;

            input_move = new Vector2();
            input_jump = false;
            input_gripLState = 0;
            input_gripRState = 0;
        }
    }

    public Transform headController;
    public Transform leftController;
    public Transform rightController;

    private SteamVR_TrackedObject leftTrackedObj;
    private SteamVR_TrackedObject rightTrackedObj;

    PlayerController pc;
    PartBodyController pbc;

    //SteamVR_Controller.Device leftDevice;
    //SteamVR_Controller.Device rightDevice;
    private SteamVR_Controller.Device leftDevice { get { return SteamVR_Controller.Input((int)leftTrackedObj.index); } }
    private SteamVR_Controller.Device rightDevice { get { return SteamVR_Controller.Input((int)rightTrackedObj.index); } }

    InputPackage currentInput;
    
    public InputPackage CurrentInput { get { return currentInput; } set { currentInput = value; } }

    void Start ()
    {
        if (!GetComponent<NetworkIdentity>().isLocalPlayer && !GetComponent<NetworkIdentity>().isServer)
        {
            enabled = false;
        }

        pc = GetComponent<PlayerController>();
        pbc = GetComponent<PartBodyController>();

        if(GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            leftTrackedObj = leftController.GetComponent<SteamVR_TrackedObject>();
            rightTrackedObj = rightController.GetComponent<SteamVR_TrackedObject>();
            //leftDevice = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost));
            //rightDevice = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost));
        }

        currentInput = new InputPackage();
    }

	void Update ()
    {
        if (GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            CmdSendInput(PackageInput());
        }

        if (GetComponent<NetworkIdentity>().isServer)
        {
            pbc.SetPartTransforms(currentInput);
        }

    }
    InputPackage PackageInput()
    {
        //lastInput = currentInput;
        currentInput.headControllerPosition = headController.localPosition;
        currentInput.leftControllerPosition = leftController.localPosition;
        currentInput.rightControllerPosition = rightController.localPosition;
        currentInput.headControllerRotation = headController.localRotation;
        currentInput.leftControllerRotation = leftController.localRotation;
        currentInput.rightControllerRotation = rightController.localRotation;
        currentInput = pc.HandleActions(currentInput, leftDevice, rightDevice);
        
        return currentInput;
    }
    [Command]
    void CmdSendInput(InputPackage input)
    {
        currentInput = input;
    }
}
