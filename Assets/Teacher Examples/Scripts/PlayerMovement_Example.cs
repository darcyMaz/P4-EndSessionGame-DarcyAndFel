using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement_Example : MonoBehaviour
{
    public float speedMovement = 0.5f;
    private SpriteRenderer myPlayerRender;

    private ProjectActions myAweasomeSystem;
    private InputAction move;
    private InputAction fire;

    private void Awake()
    {
        myAweasomeSystem = new ProjectActions();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myPlayerRender = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        move = myAweasomeSystem.Player.Move;
        fire = myAweasomeSystem.FindAction("Fire");
        //move = myAweasomeSystem.FindAction("Move");
        move.Enable();
        fire.Enable();
        fire.performed += Fire;
    }

    private void OnDisable()
    {
        move.Disable();

        fire.performed -= Fire;
        fire.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        //float direction = Input.GetAxis("Horizontal");
        Vector2 movement = move.ReadValue<Vector2>();

        if (movement.x != 0)
        {
            myPlayerRender.flipX = movement.x < 0;
        }

        transform.Translate(Vector2.right * movement.x * speedMovement * Time.deltaTime);    
    }

    private void Fire(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Debug.Log("Done");
        }
    }
}
