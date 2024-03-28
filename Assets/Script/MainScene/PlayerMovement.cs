using UnityEngine;

public class PlayerMovement : MonoBehaviour, IResettable
{
    Rigidbody2D rb;
    Vector2 startingPosition;

    public Transform BoundaryHolder;
    Boundary playerBoundary;

    public Collider2D PlayerCollider { get; private set; }

    public PlayerController Controller;

    // Kullanýlan fare düðmesi ve fare basýlý mý tutuluyor
    bool isMouseDown = false;
    bool isMouseHeld = false;

    // Kilitlenen fare ID'si
    public int LockedFingerID { get; set; } = -1;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startingPosition = rb.position;
        PlayerCollider = GetComponent<Collider2D>();

        playerBoundary = new Boundary(
            BoundaryHolder.GetChild(0).position.y,
            BoundaryHolder.GetChild(1).position.y,
            BoundaryHolder.GetChild(2).position.x,
            BoundaryHolder.GetChild(3).position.x
        );
    }

    private void OnEnable()
    {
        Controller.Players.Add(this);
        UiManager.Instance.ResetableGameObjects.Add(this);
    }

    private void OnDisable()
    {
        Controller.Players.Remove(this);
        UiManager.Instance.ResetableGameObjects.Remove(this);
    }

    private void Update()
    {
        // Fare girdilerini kontrol etme
        if (Input.GetMouseButtonDown(0))
        {
            isMouseDown = true;
            isMouseHeld = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
            isMouseHeld = false;
            LockedFingerID = -1;
        }
        else if (Input.GetMouseButton(0))
        {
            isMouseHeld = true;
        }
    }

    private void FixedUpdate()
    {
        if (isMouseDown)
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (LockedFingerID == -1)
            {
                if (PlayerCollider.OverlapPoint(mouseWorldPos))
                {
                    LockedFingerID = 0; // Fare kullanýldýðýnda parmak ID'si 0 olarak ayarlanýr
                }
            }
            else if (LockedFingerID == 0) // Fare kullanýldýðýnda parmak ID'si 0 kabul edilir
            {
                MoveToPosition(mouseWorldPos);
            }
        }
        else if (!isMouseHeld)
        {
            LockedFingerID = -1; // Fare basýlý tutulmadýðýnda kilitlenmiþ parmak sýfýrlanýr
        }
    }

    public void MoveToPosition(Vector2 position)
    {
        Vector2 clampedMousePos = new Vector2(
            Mathf.Clamp(position.x, playerBoundary.Left, playerBoundary.Right),
            Mathf.Clamp(position.y, playerBoundary.Down, playerBoundary.Up)
        );

        rb.MovePosition(clampedMousePos);
    }

    public void ResetPosition()
    {
        rb.position = startingPosition;
    }
}
