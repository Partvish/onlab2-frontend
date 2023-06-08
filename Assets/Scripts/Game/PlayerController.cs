using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController: MonoBehaviour
{
    private CharacterController _characterController;

    struct PlayerState { public string id; public Dictionary<string, string> attributes; }
    private PlayerState state;
    private Quaternion cachedHeadRotation;
    private Vector2 currentLookRotation = Vector2.zero;
    private float gravityValue = -9.81f;
    private bool groundedPlayer;
    private bool IsMine = true;

    [SerializeField]
    private Gun gun = null;

    [SerializeField]
    private Transform headRoot = null;

    private bool isPaused;

    [SerializeField]
    private float jumpHeight = 1.0f;

    [SerializeField]
    private float lookSpeed = 3.0f;

    [SerializeField]
    private float playerSpeed = 2.0f;

    [SerializeField]
    private Crosshair crosshair;

    [SerializeField]
    private GameObject ScoreboardPanel;

    [SerializeField]
    private GameObject lookCamera;

    private Vector3 playerVelocity;

    public string userName;

    public bool isReady = false;

    [SerializeField]
    private TextMeshPro userNameDisplay = null;

    [SerializeField]
    private PlayerStatusIndicator statusIndicator = null;

    public string Id { get; internal set; }

    protected void Start()
    {
        userName = string.Empty;
        _characterController = GetComponent<CharacterController>();
        isPaused = false;
        SetCursorUnlocked(false);
        lookCamera.gameObject.SetActive(false);
        StartCoroutine("WaitForConnect");
    }
    public void OnEntityRemoved()
    {
        Debug.Log("REMOVING ENTITY");
        Destroy(gameObject);
    }

    protected void UpdateViewFromState()
    {
    

        if (state.attributes.ContainsKey("xViewRot"))
        {
            cachedHeadRotation.x = float.Parse(state.attributes["xViewRot"]);
            cachedHeadRotation.y = float.Parse(state.attributes["yViewRot"]);
            cachedHeadRotation.z = float.Parse(state.attributes["zViewRot"]);
            cachedHeadRotation.w = float.Parse(state.attributes["wViewRot"]);
        }

        if (string.IsNullOrEmpty(userName) && state.attributes.ContainsKey("userName"))
        {
            userName = state.attributes["userName"];
            userNameDisplay.text = userName;
        }

        if (state.attributes.ContainsKey("isReady"))
        {
            isReady = bool.Parse(state.attributes["isReady"]);
            statusIndicator.UpdateReady(isReady);
        }
    }

    protected void UpdateStateFromView()
    {
    

        Dictionary<string, string> updatedAttributes = new Dictionary<string, string>
        {
            {"xViewRot", headRoot.localRotation.x.ToString()},
            {"yViewRot", headRoot.localRotation.y.ToString()},
            {"zViewRot", headRoot.localRotation.z.ToString()},
            {"wViewRot", headRoot.localRotation.w.ToString()}
        };
        
    }

    protected void Update()
    {
     
        if( !IsMine)
        {
            return;
        }

        HandleInput();
        HandleLook();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
        {
            SetPause(!isPaused);
        }

        if (Input.GetKeyDown(KeyCode.Tab)) 
        {
            ScoreboardPanel.SetActive(true);
           
        }

        if(Input.GetKeyUp(KeyCode.Tab))
        {
            ScoreboardPanel.SetActive(false);
            SetPause(!isPaused);
        }

        if (isPaused)
        {
            return;
        }

        groundedPlayer = _characterController.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = transform.TransformDirection(move);
        _characterController.Move(move * Time.deltaTime * playerSpeed);

        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        _characterController.Move(playerVelocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    public void FireGunRFC()
    {

        gun.Fire(state.id, "");
    }

    public void Reload()
    {
        gun.Reload();
    }

    private void HandleLook()
    {
        if (isPaused)
        {
            return;
        }

        currentLookRotation.y += Input.GetAxis("Mouse X");
        currentLookRotation.x += -Input.GetAxis("Mouse Y");

        Vector3 bodyRot = transform.eulerAngles;
        Vector3 look = currentLookRotation * lookSpeed;

        bodyRot.y = look.y;
        transform.eulerAngles = bodyRot;
        Vector3 head = headRoot.eulerAngles;
        head.x = Mathf.Clamp(look.x, -80, 80); 
        headRoot.eulerAngles = head;
    }

    private void SetCursorUnlocked(bool val)
    {
        Cursor.visible = val;
        Cursor.lockState = val ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void SetPause(bool pause)
    {
        isPaused = pause;
        SetCursorUnlocked(isPaused);
    }

    public void ShowTargetHit()
    {
        if (IsMine)
        {
            crosshair.ShowHit();
        }
    }

    public void UpdateReadyState(bool ready)
    {
        if (IsMine)
        {
            isReady = ready;
            SetAttributes(new Dictionary<string, string>() { { "isReady", isReady.ToString() } });
            statusIndicator.UpdateReady(ready);
        }
    }

    private void SetAttributes(Dictionary<string, string> dictionary)
    {
        
    }
}