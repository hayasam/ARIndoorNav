﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class ARPositionTracking : MonoBehaviour
{
    public TrackingErrorHandling _TrackingErrorHandling;
    
    // Start is called before the first frame update
    void Start()
    {
        QuitOnConnectionErrors();

    }

    // Update is called once per frame
    void Update()
    {
        // Maybe FeaturePoint drift
    }


    /**
    ARCore needs to capture and process enough information to start tracking the user's movements in the real world.
    Once ARCore is tracking, the Frame object is used to interact with ARCore.
    Source: https://codelabs.developers.google.com/codelabs/arcore-intro/index.html?index=..%2F..io2018#2 

    Returns true if ARCore is currently tracking
    */
    private bool IsInTrackingState()
    {
        // Case: ARCore is not tracking
        if (Session.Status != SessionStatus.Tracking)
        {
            //Resets the screen timeout after tracking is lost.
            int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            return false;
        }
        // Case: ARCore is tracking
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        return true;
    }



    /* This method checks the state of the ARCore Session to make sure ARCore is working in our app:
     * 
     * 1: Is the permission to use the camera granted? ARCore uses the camera to sense the real world. 
     * The user is prompted to grant this permission the first time the application is run. 
     * This check is done by ARCore automatically.
     * 
     * 2: Can the ARCore library connect to the ARCore Services? ARCore relies on AR Services which runs on the device in a separate process.
     * 
     * Source: https://codelabs.developers.google.com/codelabs/arcore-intro/index.html?index=..%2F..io2018#2 
     */
    void QuitOnConnectionErrors()
    {
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            StartCoroutine(ToastAndExit(
                "Camera permission is needed to run this application.", 5));
        }
        else if (Session.Status.IsError())
        {
            StartCoroutine(ToastAndExit(
                "ARCore encountered a problem connecting. Please restart the app.", 5));
        }
    }

    /// <summary>Coroutine to display an error then exit.</summary>
    /// Source: https://codelabs.developers.google.com/codelabs/arcore-intro/index.html?index=..%2F..io2018#0
    public static IEnumerator ToastAndExit(string message, int seconds)
    {
        _ShowAndroidToastMessage(message);
        yield return new WaitForSeconds(seconds);
        Application.Quit();
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    /// <param name="length">Toast message time length.</param>
    public static void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass =
                new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject =
                    toastClass.CallStatic<AndroidJavaObject>(
                        "makeText", unityActivity,
                        message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
