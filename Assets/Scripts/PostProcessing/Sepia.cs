using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(SepiaRenderer), PostProcessEvent.AfterStack, "Custom/Sepia")]
public sealed class Sepia : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Sepia effect intensity.")]
    public FloatParameter blend = new FloatParameter { value = 0.5f };
}

public sealed class SepiaRenderer : PostProcessEffectRenderer<Sepia>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Sepia"));
        sheet.properties.SetFloat("_Blend", settings.blend);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}