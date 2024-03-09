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
  [SerializeField] private float returnDelay = 1.0f;

  private bool isMoving = false;

  private Coroutine moveCoroutine = null;
  private Coroutine waitCoroutine = null;
  private Coroutine returnCoroutine = null;

  void Start() {
    originPosition = target.transform.position;
    destinationPosition = destination.transform.position;
  }

  void Update() {
    if (Input.GetKeyDown(KeyCode.Space)) {
      if (state == false) {
        Activate();
      } else {
        Desactivate();
      }
    }
  }

  public override void OnActivation() {
    if (returnCoroutine == null) {
      StartMovement();
    }
  }

  public override void OnDesactivation() {
    StopMovement();
    ReturnToOriginAfterDelay();
  }

  private void StartMovement() {
    isMoving = true;
    if (moveCoroutine != null) {
      StopCoroutine(moveCoroutine);
    }
    moveCoroutine = StartCoroutine(MoveTarget());
  }

  private IEnumerator MoveTarget() {
    float startTime = Time.time;
    float length = Vector3.Distance(originPosition, destinationPosition);
    while (Vector3.Distance(target.transform.position, destinationPosition) > 0.01f) {
      float distanceCovered = (Time.time - startTime) * speed;
      float frac = distanceCovered / length;
      target.transform.position = Vector3.Lerp(originPosition, destinationPosition, frac);
      yield return null;
    }

    target.transform.position = destinationPosition;
    if (waitCoroutine != null) {
      StopCoroutine(waitCoroutine);
    }
    waitCoroutine = StartCoroutine(Wait());
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

  private void StopMovement() {
    if (moveCoroutine != null) {
      StopCoroutine(moveCoroutine);
      moveCoroutine = null;
    }
    if (waitCoroutine != null) {
      StopCoroutine(waitCoroutine);
      waitCoroutine = null;
    }
    isMoving = false;
  }

  private void ReturnToOriginAfterDelay() {
    if (returnCoroutine != null) {
      StopCoroutine(returnCoroutine);
    }
    returnCoroutine = StartCoroutine(ReturnToOrigin());
  }

  private IEnumerator ReturnToOrigin() {
    yield return new WaitForSeconds(returnDelay);
    float startTime = Time.time;
    Vector3 startPosition = target.transform.position;
    float length = Vector3.Distance(startPosition, originPosition);

    while (Vector3.Distance(target.transform.position, originPosition) > 0.01f) {
      float distanceCovered = (Time.time - startTime) * speed;
      float frac = distanceCovered / length;
      target.transform.position = Vector3.Lerp(startPosition, originPosition, frac);
      yield return null;
    }

    target.transform.position = originPosition;

    returnCoroutine = null;
  }
}