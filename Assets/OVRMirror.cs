using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class OVRMirror : MonoBehaviour
{
    public bool oculusEnabled = false;
    public bool oculusCameraBinded = false;

    // Window
    [HideInInspector]
    public bool openWindowOnEnable = true;
    [HideInInspector]
    public bool openWindowWithMaximized = true;
    [HideInInspector]
    public bool disableCameraOnCloseWindow = true;
    [HideInInspector]
    public bool overwriteViewportRect = true;

    // Oculus
    [HideInInspector]
    public int oculusRenderEventID = 1; // Memo: Catch EndFrame from Unity SDK for Unity 0.4.2
    [HideInInspector]
    public float oculusCameraViewportHeight = 0.55f;
    // Memo: 0 ... BeginFrame(Oculus SDK for Unity 0.4.2)
    // Memo: 1 ... EndFrame(Oculus SDK for Unity 0.4.2)

    // Plugin
    [HideInInspector]
    public bool issuePluginEventEnabled = true; // false, if oculusEnabled == true.
    [HideInInspector]
    public bool renderEventIDEnabled = true;
    [HideInInspector]
    public int renderEventID = 1411031728;

    // Objects
    [HideInInspector]
    public Camera targetCamera;
    [HideInInspector]
    public Texture targetTexture;

    [HideInInspector]
    public bool _inspectorAdvanced;

    uint _clientWidth;
    uint _clientHeight;

    void _BindInstance()
    {
        if (this.targetCamera == null)
        {
            this.targetCamera = this.GetComponent<Camera>();
        }
        if (this.targetTexture == null)
        {
            if (this.targetCamera != null)
            {
                this.targetTexture = this.targetCamera.targetTexture;
            }
        }
    }

    void Awake()
    {
        _BindInstance();
    }

    void OnEnable()
    {
        _BindInstance();
        OVRMirrorCreate();

#if !UNITY_EDITOR
if( this.openWindowWithMaximized ) {
OVRMirrorSetWindowFlags( 0x01 );
}
if( this.openWindowOnEnable ) {
OVRMirrorOpenWindow();
}
#endif

        StartCoroutine(CallPluginAtEndOfFrames());
    }

    void OnDisable()
    {
        OVRMirrorDestroy();
    }

    void OnDestroy()
    {
        OVRMirrorDestroy();
    }

    void LateUpdate()
    {
        _BindInstance();

        int windowState = OVRMirrorGetWindowState();

        {
            ulong clientSize = OVRMirrorGetClientSize();
            _clientWidth = (uint)(clientSize & 0xfffffffful);
            _clientHeight = (uint)(clientSize >> 32);
        }

        if (this.oculusEnabled)
        {
            // Always runInBackground = true
            Application.runInBackground = true;
            // Override Screen.showCursor
            Cursor.visible = ((windowState & (int)OVRMirrorWindowState.Mouseover) != 0);
        }

        if (!(this.oculusEnabled && this.oculusCameraBinded))
        {
            if (this.disableCameraOnCloseWindow)
            {
                if (this.targetCamera != null)
                {
                    bool cameraEnabled = (windowState != 0)
                    && (windowState & (int)OVRMirrorWindowState.Minimized) == 0;

                    this.targetCamera.enabled = cameraEnabled;
                }
            }
        }

        if (!(this.oculusEnabled && this.oculusCameraBinded) && this.overwriteViewportRect)
        {
            if (_clientWidth != 0 && _clientHeight != 0)
            {
                unchecked
                {
                    Camera cam = this.targetCamera;
                    if (cam != null)
                    {
                        Rect rc = cam.rect;
                        if (_clientWidth > _clientHeight)
                        {
                            rc.width = 1.0f;
                            rc.height = (float)_clientHeight / (float)_clientWidth;
                        }
                        else if (_clientWidth < _clientHeight)
                        {
                            rc.width = (float)_clientWidth / (float)_clientHeight;
                            rc.height = 1.0f;
                        }
                        else
                        {
                            rc.width = rc.height = 1.0f;
                        }
                        cam.rect = rc;
                    }
                }
            }
        }
    }

    IEnumerator CallPluginAtEndOfFrames()
    {
        for (; ; )
        {
            // Wait until all frame rendering is done
            yield return new WaitForEndOfFrame();

            _BindInstance();

            Camera cam = this.targetCamera;

            if (this.targetTexture == null)
            {
                if (cam != null)
                {
                    this.targetTexture = cam.targetTexture;
                }
            }

            if (this.targetTexture)
            {
                OVRMirrorSetRenderTexture(this.targetTexture.GetNativeTexturePtr());
            }
            else
            {
                OVRMirrorSetRenderTexture(System.IntPtr.Zero);
            }

            if (cam != null)
            {
                Rect rc = cam.rect;
                if (this.oculusEnabled && this.oculusCameraBinded)
                {
                    if (_clientWidth != 0 && _clientHeight != 0)
                    {
                        float clipH = Mathf.Clamp01(this.oculusCameraViewportHeight);
                        float clipW = Mathf.Clamp01(clipH * (float)_clientWidth / (float)_clientHeight);

                        float viewportWidth = rc.width * clipW;
                        float viewportHeight = (rc.height * clipH);
                        rc.x += (rc.width - viewportWidth) * 0.5f;
                        rc.y += (rc.height - viewportHeight) * 0.5f;
                        rc.width = viewportWidth;
                        rc.height = viewportHeight;

                    }
                }
                OVRMirrorSetViewportRect(rc.x, rc.y, rc.width, rc.height);
            }
            else
            {
                OVRMirrorSetViewportRect(0.0f, 0.0f, 1.0f, 1.0f);
            }

            if (this.oculusEnabled)
            {
                OVRMirrorSetRenderEventID(this.oculusRenderEventID);
            }
            else if (this.renderEventIDEnabled)
            {
                OVRMirrorSetRenderEventID(this.renderEventID);
            }
            else
            {
                OVRMirrorResetRenderEventID();
            }

            if (!this.oculusEnabled && this.issuePluginEventEnabled)
            {
                GL.IssuePluginEvent(this.renderEventID);
            }
        }
    }

    [System.Flags]
    enum OVRMirrorWindowState
    {
        Opened = 0x00000001,
        Maximized = 0x00000002,
        Minimized = 0x00000004,
        Mouseover = 0x00000008,
    }

    [DllImport("OVRMirror")]
    static extern void OVRMirrorCreate();
    [DllImport("OVRMirror")]
    static extern void OVRMirrorDestroy();
    [DllImport("OVRMirror")]
    static extern void OVRMirrorSetWindowFlags(int windowFlags);
    [DllImport("OVRMirror")]
    static extern void OVRMirrorOpenWindow();
    [DllImport("OVRMirror")]
    static extern void OVRMirrorCloseWindow();
    [DllImport("OVRMirror")]
    static extern int OVRMirrorGetWindowState();
    [DllImport("OVRMirror")]
    static extern ulong OVRMirrorGetClientSize();
    [DllImport("OVRMirror")]
    static extern int OVRMirrorSetRenderTexture(System.IntPtr texture);
    [DllImport("OVRMirror")]
    static extern void OVRMirrorSetViewportRect(float x, float y, float w, float h);
    [DllImport("OVRMirror")]
    static extern void OVRMirrorSetRenderEventID(int eventID);
    [DllImport("OVRMirror")]
    static extern void OVRMirrorResetRenderEventID();
}