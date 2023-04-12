using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class JJMButton : XRBaseInteractable
{
    public UnityEvent OnPress = null;

    public float yMin, yMax;
    private bool previousPress = false, pressedState;
    private float previousHandHeight = 0.0f;
    private IXRHoverInteractor hoverInteractor = null;

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        StartPress(args.interactorObject);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        EndPress(args.interactorObject);
    }

    private void StartPress(IXRHoverInteractor interactor)
    {
        hoverInteractor = interactor;
        previousHandHeight = GetLocalYPosition(hoverInteractor.transform.position);
    }

    private void EndPress(IXRHoverInteractor interactor)
    {
        hoverInteractor = null;
        previousHandHeight = 0.0f;

        previousPress = false;
        pressedState = false;
        SetYPosition(yMax);
    }

    private void Start()
    {
        SetMinMax();
    }

    private void SetMinMax()
    {
        Collider collider = GetComponent<Collider>();
        if (yMin == 0)
            yMin = transform.localPosition.y - /*(collider.bounds.size.y / 2f);*/ 0.1f;
        yMax = transform.localPosition.y;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);
        if (hoverInteractor != null)
        {
            float newHandHeight = GetLocalYPosition(hoverInteractor.transform.position);
            float handDifference = previousHandHeight - newHandHeight;
            previousHandHeight = newHandHeight;

            float newPosition = transform.localPosition.y - handDifference;
            SetYPosition(newPosition);

            CheckPress();
        }
    }

    private float GetLocalYPosition(Vector3 position)
    {
        Vector3 localPosition = transform.root.InverseTransformPoint(position);

        return localPosition.y;
    }

    private void SetYPosition(float position)
    {
        Vector3 newPosition = transform.localPosition;
        newPosition.y = Mathf.Clamp(position, yMin, yMax);
        transform.localPosition = newPosition;

        if (newPosition.y == yMax) pressedState = false;
    }

    private void CheckPress()
    {
        bool inPosition = isInPosition();

        if (inPosition && inPosition != previousPress && !pressedState)
        {
            OnPress.Invoke();
            pressedState = true;
        }

        previousPress = inPosition;
    }

    private bool isInPosition()
    {
        float inRange = Mathf.Clamp(transform.localPosition.y, yMin, yMin + 0.01f);

        return transform.localPosition.y == inRange;
    }
}
