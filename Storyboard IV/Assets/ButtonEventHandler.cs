using System;
using UnityEngine;
using UnityEngine.Events;
using Vuforia;

/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
///
/// Changes made to this file could be overwritten when upgrading the Vuforia version.
/// When implementing custom event handler behavior, consider inheriting from this class instead.
/// </summary>
[Obsolete("DefaultTrackableEventHandler is obsolete and will be removed soon. Please use DefaultObserverEventHandler instead")]
public class ButtonEventHandler : MonoBehaviour
{
    public enum TrackingStatusFilter
    {
        Tracked,
        Tracked_ExtendedTracked,
        Tracked_ExtendedTracked_Limited
    }

    /// <summary>
    /// A filter that can be set to either:
    /// - Only consider a target if it's in view (TRACKED)
    /// - Also consider the target if's outside of the view, but the environment is tracked (EXTENDED_TRACKED)
    /// - Even consider the target if tracking is in LIMITED mode, e.g. the environment is just 3dof tracked.
    /// </summary>
    public TrackingStatusFilter StatusFilter = TrackingStatusFilter.Tracked_ExtendedTracked_Limited;
    public UnityEvent OnTargetFound;
    public UnityEvent OnTargetLost;

    protected ObserverBehaviour mTrackableBehaviour;
    protected TargetStatus mPreviousTargetStatus = TargetStatus.NotObserved;
    protected bool mCallbackReceivedOnce;

    protected virtual void Start()
    {
        mTrackableBehaviour = GetComponent<ObserverBehaviour>();

        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
            mTrackableBehaviour.OnBehaviourDestroyed += OnTrackableBehaviourDestroyed;
            
            OnTargetStatusChanged(mTrackableBehaviour, mTrackableBehaviour.TargetStatus);
        }
    }
    
    protected virtual void OnDestroy()
    {
        if (mTrackableBehaviour)
            OnTrackableBehaviourDestroyed(mTrackableBehaviour);
    }
    
    void OnTrackableBehaviourDestroyed(ObserverBehaviour behaviour)
    {
        mTrackableBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        mTrackableBehaviour.OnBehaviourDestroyed -= OnTrackableBehaviourDestroyed;
        mTrackableBehaviour = null;
    }

    void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        var name = mTrackableBehaviour.TargetName;
        if (mTrackableBehaviour is VuMarkBehaviour vuMarkBehaviour && vuMarkBehaviour.InstanceId != null)
        {
            name += " (" + vuMarkBehaviour.InstanceId + ")";
        }
        
        Debug.Log($"Target status: { name } { targetStatus.Status } -- { targetStatus.StatusInfo }");

        HandleTargetStatusChanged(mPreviousTargetStatus.Status, targetStatus.Status);
        HandleTrackableStatusInfoChanged(targetStatus.StatusInfo);

        mPreviousTargetStatus = targetStatus;
    }

    protected virtual void HandleTargetStatusChanged(Status previousStatus, Status newStatus)
    {
        if (!ShouldBeRendered(previousStatus) && ShouldBeRendered(newStatus))
        {
            OnTrackingFound();
        }
        else if (ShouldBeRendered(previousStatus) && !ShouldBeRendered(newStatus))
        {
            OnTrackingLost();
        }
        else
        {
            if (!mCallbackReceivedOnce && !ShouldBeRendered(newStatus))
            {
                // This is the first time we are receiving this callback, and the target is not visible yet.
                // --> Hide the augmentation.
                OnTrackingLost();
            }
        }

        mCallbackReceivedOnce = true;
    }

    protected virtual void HandleTrackableStatusInfoChanged(StatusInfo newStatusInfo)
    {
        if (newStatusInfo == StatusInfo.WRONG_SCALE)
        {
            Debug.LogErrorFormat("The target {0} appears to be scaled incorrectly. " +
                                 "This might result in tracking issues. " +
                                 "Please make sure that the target size corresponds to the size of the " +
                                 "physical object in meters and regenerate the target or set the correct " +
                                 "size in the target's inspector.", mTrackableBehaviour.TargetName);
        }
    }

    protected bool ShouldBeRendered(Status status)
    {
        if (status == Status.TRACKED)
        {
            // always render the augmentation when status is TRACKED, regardless of filter
            return true;
        }

        if (StatusFilter == TrackingStatusFilter.Tracked_ExtendedTracked && status == Status.EXTENDED_TRACKED)
        {
            // also return true if the target is extended tracked
            return true;
        }

        if (StatusFilter == TrackingStatusFilter.Tracked_ExtendedTracked_Limited && 
            (status == Status.EXTENDED_TRACKED || status == Status.LIMITED))
        {
            // in this mode, render the augmentation even if the target's tracking status is LIMITED.
            // this is mainly recommended for Anchors.
            return true;
        }

        return false;
    }

    protected virtual void OnTrackingFound()
    {
        if (mTrackableBehaviour)
        {
            var rendererComponents = mTrackableBehaviour.GetComponentsInChildren<Renderer>(true);
            var colliderComponents = mTrackableBehaviour.GetComponentsInChildren<Collider>(true);
            // var canvasComponents = mTrackableBehaviour.GetComponentsInChildren<Canvas>(true);

            // Enable rendering:
            foreach (var component in rendererComponents)
                component.enabled = true;

            // Enable colliders:
            foreach (var component in colliderComponents)
                component.enabled = true;

            // // Enable canvas':
            // foreach (var component in canvasComponents)
            //     component.enabled = true;
        }

        OnTargetFound?.Invoke();
        Sistem.instance.Gui_Utama.SetActive(true);
        Sistem.instance.TempatSpawn.GetComponent<Animation>().Play("PopUp");
        KumpulanSuara.instance.Panggil_Suara(1);
    }

    protected virtual void OnTrackingLost()
    {
        if (mTrackableBehaviour)
        {
            var rendererComponents = mTrackableBehaviour.GetComponentsInChildren<Renderer>(true);
            var colliderComponents = mTrackableBehaviour.GetComponentsInChildren<Collider>(true);
            // var canvasComponents = mTrackableBehaviour.GetComponentsInChildren<Canvas>(true);

            // Disable rendering:
            foreach (var component in rendererComponents)
                component.enabled = false;

            // Disable colliders:
            foreach (var component in colliderComponents)
                component.enabled = false;

            // // Disable canvas':
            // foreach (var component in canvasComponents)
            //     component.enabled = false;
        }

        OnTargetLost?.Invoke();
        Sistem.instance.Gui_Utama.SetActive(false);
    }
}