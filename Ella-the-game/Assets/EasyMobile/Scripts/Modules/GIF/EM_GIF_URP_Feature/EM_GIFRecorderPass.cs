using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if EM_URP
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EM_GIFRecorderPass : ScriptableRenderPass
{
    string profilerTag;

    private RenderTargetIdentifier cameraColorTargetIdent;
    private RenderTexture targetBlit;

    public EM_GIFRecorderPass(
        string profilerTag,
        RenderPassEvent renderPassEvent)
    {
        this.profilerTag = profilerTag;
        this.renderPassEvent = renderPassEvent;
    }

    public void Setup(RenderTargetIdentifier cameraColorTargetIdent, RenderTexture targetBlit)
    {
        this.cameraColorTargetIdent = cameraColorTargetIdent;
        this.targetBlit = targetBlit;
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        // cmd.GetTemporaryRT(tempTexture.id, cameraTextureDescriptor);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
        cmd.Clear();
        cmd.Blit(cameraColorTargetIdent, targetBlit);
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        // cmd.ReleaseTemporaryRT(tempTexture.id);
    }
}
#endif