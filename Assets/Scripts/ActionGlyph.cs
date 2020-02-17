using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class ActionGlyph : MonoBehaviour
{
    public ActionGlyphMapper.ActionGlyphType actionGlyphType;

    private SpriteRenderer mSpriteRenderer;
    private Image mImage;

    private Vector3 mOriginalScale;

    // Start is called before the first frame update
    void Start()
    {
        mOriginalScale = transform.localScale;

        mSpriteRenderer = GetComponent<SpriteRenderer>();
        mImage = GetComponent<Image>();

        UpdateGlyph();
    }

    void Update()
    {
        UpdateGlyph();
    }

    void UpdateGlyph()
    {
        // If the sprite is the space bar & is longer, we need to give it more space...
        Vector3 adjustedScale = mOriginalScale;
        if (Game.instance.actionGlyphMapper.GlyphIsDoubleWidth(actionGlyphType))
        {
            adjustedScale.x *= 2;
        }

        if (mSpriteRenderer != null)
        {
            mSpriteRenderer.sprite = Game.instance.actionGlyphMapper.SpriteForAction(actionGlyphType);
            mSpriteRenderer.transform.localScale = adjustedScale;
        }

        if (mImage != null)
        {
            mImage.sprite = Game.instance.actionGlyphMapper.SpriteForAction(actionGlyphType);
            mImage.transform.localScale = adjustedScale;
        }
    }
}
