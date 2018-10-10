using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTranslate : MonoBehaviour
{
    public enum Direction
    {
        xAxis,
        yAxis,
        zAxis
    };

    [Header("Movement Params")]
    public Direction direction;
    public float movementSpeed;
    public float maxMovementDirection;

    private Vector3 _startPosition;
    private bool _moveForward;

    // Use this for initialization
    void Start()
    {
        _startPosition = transform.position;
        _moveForward = true;
    }

    // Update is called once per frame
    void Update()
    {
        MoveObject();
        CheckOutOfLimit();
    }

    private void MoveObject()
    {
        switch (direction)
        {
            case Direction.xAxis:
                transform.Translate((_moveForward ? Vector3.right : Vector3.left) *
                    movementSpeed * Time.deltaTime);
                break;

            case Direction.yAxis:
                transform.Translate((_moveForward ? Vector3.up : Vector3.down) *
                        movementSpeed * Time.deltaTime);
                break;

            case Direction.zAxis:
                transform.Translate((_moveForward ? Vector3.forward : Vector3.back) *
                        movementSpeed * Time.deltaTime);
                break;
        }
    }

    private void CheckOutOfLimit()
    {
        switch (direction)
        {
            case Direction.xAxis:
                float xPosition = transform.position.x;
                if (xPosition > _startPosition.x + maxMovementDirection)
                    _moveForward = false;
                else if (xPosition < _startPosition.x - maxMovementDirection)
                    _moveForward = true;
                break;

            case Direction.yAxis:
                float yPosition = transform.position.y;
                if (yPosition > _startPosition.y + maxMovementDirection)
                    _moveForward = false;
                else if (yPosition < _startPosition.y - maxMovementDirection)
                    _moveForward = true;
                break;

            case Direction.zAxis:
                float zPosition = transform.position.z;
                if (zPosition > _startPosition.z + maxMovementDirection)
                    _moveForward = false;
                else if (zPosition < _startPosition.z - maxMovementDirection)
                    _moveForward = true;
                break;
        }
    }
}
