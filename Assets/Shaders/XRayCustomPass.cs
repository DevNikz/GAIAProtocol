using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering.RendererUtils;

[System.Serializable]
public class XRayCustomPass : CustomPass
{
    public LayerMask xrayLayer;
    public Material xrayOccludedMaterial;

    ShaderTagId[] shaderTags;

    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
    {
        shaderTags = new ShaderTagId[]
        {
            new ShaderTagId("Forward"),
            new ShaderTagId("ForwardOnly"),
            new ShaderTagId("SRPDefaultUnlit"),
            new ShaderTagId("HDLitMesh"),
        };
    }

    protected override void Execute(CustomPassContext ctx)
    {
        // PASS 1: Draw X-Ray ONLY where object is BEHIND geometry
        DrawXRayOccluded(ctx);

        // PASS 2: Restore normal render ONLY where object is IN FRONT
        // This overwrites the XRay pixels that shouldn't be visible
        DrawNormalOnTop(ctx);
    }

    void DrawXRayOccluded(CustomPassContext ctx)
    {
        if (xrayOccludedMaterial == null) return;

        var stateBlock = new RenderStateBlock(RenderStateMask.Depth | RenderStateMask.Raster)
        {
            depthState = new DepthState(
                writeEnabled: false,
                compareFunction: CompareFunction.Greater  // Only behind walls
            ),
            rasterState = new RasterState(cullingMode: CullMode.Back)
        };

        RendererListDesc desc = new RendererListDesc(shaderTags, ctx.cullingResults, ctx.hdCamera.camera)
        {
            renderQueueRange          = RenderQueueRange.all,
            layerMask                 = xrayLayer,
            overrideMaterial          = xrayOccludedMaterial,
            overrideMaterialPassIndex = 0,
            stateBlock                = stateBlock,
            sortingCriteria           = SortingCriteria.CommonOpaque,
        };

        ctx.cmd.DrawRendererList(ctx.renderContext.CreateRendererList(desc));
    }

    void DrawNormalOnTop(CustomPassContext ctx)
    {
        // Draw the object with its OWN material where it's visible
        // This paints over any X-Ray pixels that leaked into visible areas
        var stateBlock = new RenderStateBlock(RenderStateMask.Depth | RenderStateMask.Raster)
        {
            depthState = new DepthState(
                writeEnabled: true,
                compareFunction: CompareFunction.LessEqual  // Only in front / visible
            ),
            rasterState = new RasterState(cullingMode: CullMode.Back)
        };

        var desc = new RendererListDesc(shaderTags, ctx.cullingResults, ctx.hdCamera.camera)
        {
            renderQueueRange = RenderQueueRange.all,
            layerMask        = xrayLayer,
            stateBlock       = stateBlock,
            sortingCriteria  = SortingCriteria.CommonOpaque,
            // No overrideMaterial — uses the object's own material
        };

        ctx.cmd.DrawRendererList(ctx.renderContext.CreateRendererList(desc));
    }

    protected override void Cleanup() { }
}