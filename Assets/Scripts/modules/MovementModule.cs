using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementModule : ModuleBase {
  [SerializeField] private GameObject target;
  [SerializeField] private GameObject destination;

  private Vector3 originPosition;
  private Vector3 destinationPosition;

  [SerializeField] private float speed = 1.0f;
  [SerializeField] private float waitTime = 5.0f;

  private bool isMoving = false;

  void Start() {
    originPosition = target.transform.position;
    destinationPosition = destination.transform.position;
    Activate();
  }

  public override void OnActivation() {
    StartMovement();
  }

  private void StartMovement() {
    isMoving = true;
    StartCoroutine(MoveTarget());
  }

  private IEnumerator MoveTarget() {
    float startTime = Time.time;
    float lenght = Vector3.Distance(originPosition, destinationPosition);
    while (Vector3.Distance(target.transform.position, destinationPosition) > 0.01f) {
      float distanceCovered = (Time.time - startTime) * speed;
      float frac = distanceCovered / lenght;
      target.transform.position = Vector3.Lerp(originPosition, destinationPosition, frac);
      yield return null;
    }

    target.transform.position = destinationPosition;
    StartCoroutine(Wait());
  }

  private IEnumerator Wait() {
    yield return new WaitForSeconds(waitTime);

    Vector3 tempPosition = originPosition;
    originPosition = destinationPosition;
    destinationPosition = tempPosition;

    if (isMoving) {
      StartMovement();
    }
  }

}
