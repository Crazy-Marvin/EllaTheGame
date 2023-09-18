using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if EM_URP

using UnityEngine.Rendering.Universal;
public class EM_GIFRecorderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public bool IsEnabled = true;
        public RenderPassEvent WhenToInsert = RenderPassEvent.AfterRendering;
    }

    public Settings settings = new Settings();
    EM_GIFRecorderPass gifRecorderPass;

    private static Dictionary<Camera, RenderTexture> cameraTargetRTDict = new Dictionary<Camera, RenderTexture>();

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!settings.IsEnabled)
            return;
        RenderTexture targetBlit = null;
        if (cameraTargetRTDict.ContainsKey(renderingData.cameraData.camera))
            targetBlit = cameraTargetRTDict[renderingData.cameraData.camera];
        if (targetBlit == null)
            return;
        var cameraColorTargetIdent = renderer.cameraColorTarget;
        gifRecorderPass.Setup(cameraColorTargetIdent, targetBlit);

        renderer.EnqueuePass(gifRecorderPass);
    }

    public static RenderTexture GetBlitRT(Camera camera)
    {
        if (!cameraTargetRTDict.ContainsKey(camera))
            return null;
        return cameraTargetRTDict[camera];
    }

    public static void SetBlitRT(Camera camera, RenderTexture rt)
    {
        if (!cameraTargetRTDict.ContainsKey(camera))
            cameraTargetRTDict.Add(camera, null);
        cameraTargetRTDict[camera] = rt;
    }

    public override void Create()
    {
        gifRecorderPass = new EM_GIFRecorderPass(
            "EM_GifRecorderPass",
            settings.WhenToInsert
        );
    }
}
#endif