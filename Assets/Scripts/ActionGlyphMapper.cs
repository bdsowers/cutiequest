using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class ActionGlyphMapper : MonoBehaviour
{
    public enum ActionGlyphType
    {
        ConfirmActivate,
        CancelDance,
        Spell,
        QuestR,
    }

    [System.Serializable]
    public struct GlyphSet
    {
        public Sprite confirm;
        public Sprite cancel;
        public Sprite spell;
        public Sprite questR;
    }

    public GlyphSet xboxSet;
    public GlyphSet keyboardSet;
    public GlyphSet genericSet;

    public Sprite SpriteForAction(ActionGlyphType glyphType)
    {
        if (DeviceID() == "Keyboard")
        {
            if (glyphType == ActionGlyphType.ConfirmActivate)
                return keyboardSet.confirm;
            else if (glyphType == ActionGlyphType.CancelDance)
                return keyboardSet.cancel;
            else if (glyphType == ActionGlyphType.Spell)
                return keyboardSet.spell;
            else
                return keyboardSet.questR;
        }
        else if (DeviceID() == "XBox")
        {
            if (glyphType == ActionGlyphType.ConfirmActivate)
                return xboxSet.confirm;
            else if (glyphType == ActionGlyphType.CancelDance)
                return xboxSet.cancel;
            else if (glyphType == ActionGlyphType.Spell)
                return xboxSet.spell;
            else
                return xboxSet.questR;
        }
        else
        {
            if (glyphType == ActionGlyphType.ConfirmActivate)
                return genericSet.confirm;
            else if (glyphType == ActionGlyphType.CancelDance)
                return genericSet.cancel;
            else if (glyphType == ActionGlyphType.Spell)
                return genericSet.spell;
            else
                return genericSet.questR;
        }
    }

    public static string ReplaceActionCodesWithGlyphs(string str)
    {
        // Default mappings have the following:
        //  [ACTION_DANCE] -> Cancel (B)
        //  [ACTION_MOVE] -> Keyboard or Joystick (not an image, but should be if we want to localize)
        //  [ACTION_SPELL] -> Spell (X)

        string platform = DeviceID();

        str = str.Replace("[ACTION_DANCE]", "<sprite=\"" + platform + "\" name=\"Cancel\">");
        str = str.Replace("[ACTION_MOVE]", MovementType());
        str = str.Replace("[ACTION_SPELL]", "<sprite=\"" + platform + "\" name=\"Spell\">");
        str = str.Replace("[ACTION_CONFIRM]", "<sprite=\"" + platform + "\" name=\"Confirm\">");

        return str;
    }

    private static string MovementType()
    {
        if (DeviceID() == "Keyboard")
            return "Arrow Keys";
        else
            return "Joystick";
    }

    private static string DeviceID()
    {
        // Keyboard
        if (Game.instance.actionSet.boundDevice == null)
        {
            return "Keyboard";
        }
        else if (Game.instance.actionSet.boundDevice.Name.Contains("XBox") ||
            Game.instance.actionSet.boundDevice.Name.Contains("xbox") ||
            Game.instance.actionSet.boundDevice.Name.Contains("xBox") ||
            Game.instance.actionSet.boundDevice.Name.Contains("XBOX") ||
            Game.instance.actionSet.boundDevice.Name.Contains("XInput"))
        {
            return "XBox";
        }

        return "Generic";
    }

    public bool GlyphIsDoubleWidth(ActionGlyphType glyphType)
    {
        return DeviceID() == "Keyboard" && glyphType == ActionGlyphType.Spell;
    }

    public bool GlyphIsBigger(ActionGlyphType glyphType)
    {
        return DeviceID() == "Generic";
    }

}
