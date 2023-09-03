using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static event Action<ClearCounter> OnSelectedCounterChanged;

    [SerializeField] private GameInput gameInput;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float rotateSpeed = 10f;
    [SerializeField] LayerMask layerCounter;

    private bool isWalking = false;
    private Vector3 lastMoveDir;
    private ClearCounter selectedCounter;

    private void Start()
    {
        gameInput.OnInteractActionE += OnInteractActions;
    }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();
    }

    private void OnInteractActions()
    {
        selectedCounter?.Interact();
    }

    private void HandleInteraction()
    {
        Vector2 inputVector = gameInput.GetNormalizedMove();

        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            lastMoveDir = moveDir;
        }

        float interactDistance = 2f;

        if (Physics.Raycast(transform.position, lastMoveDir, out RaycastHit raycastHit, interactDistance, layerCounter))
        {
            if (raycastHit.transform.TryGetComponent<ClearCounter>(out ClearCounter clearCounter))
            {
                SelectedCounterIs(clearCounter);
            }
            else
            {
                SelectedCounterIs(null);
            }
        }
        else
        {
            SelectedCounterIs(null);
        }

    }
    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetNormalizedMove();

        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float hightPlayer = 2f;
        float radiusPlayer = 0.7f;
        Vector3 topOfThePlayer = transform.position + Vector3.up * hightPlayer;
        bool canMove = !Physics.CapsuleCast(transform.position, topOfThePlayer, radiusPlayer, moveDir, moveDistance);

        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(transform.position, topOfThePlayer, radiusPlayer, moveDirX, moveDistance);
            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, topOfThePlayer, radiusPlayer, moveDirZ, moveDistance);
                if (canMove)
                {
                    moveDir = moveDirZ;
                }

            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;

        transform.forward = Vector3.Slerp(transform.forward, moveDir, rotateSpeed * Time.deltaTime);
    }

    private void SelectedCounterIs(ClearCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(selectedCounter);

    }


    public bool IsWalking()
    {
        return isWalking;
    }
}
