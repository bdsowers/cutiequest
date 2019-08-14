using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PixelatedRenderer), PostProcessEvent.AfterStack, "Custom/Pixelated")]
public sealed class Pixelated : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Pixelated effect intensity.")]
    public FloatParameter blend = new FloatParameter { value = 0.5f };
}

public sealed class PixelatedRenderer : PostProcessEffectRenderer<Pixelated>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Pixelated"));
        sheet.properties.SetFloat("_Blend", settings.blend);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}