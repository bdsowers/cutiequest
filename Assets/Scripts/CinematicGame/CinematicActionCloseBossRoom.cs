using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OMM.RDG;

public class CinematicActionCloseBossRoom : CinematicAction
{
    public override string actionName
    {
        get { return "close_boss_room"; }
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        // Mark all the spaces below the rock as unwalkable so nobody teleports there
        RandomDungeon dungeon = Game.instance.levelGenerator.dungeon;
        for (int x = 0; x < dungeon.width; ++x)
        {
            for (int y = 13; y < dungeon.height; ++y)
            {
                if (Game.instance.levelGenerator.collisionMap.SpaceMarking(x,y) == 0)
                    Game.instance.levelGenerator.collisionMap.MarkSpace(x, y, 1);
            }
        }

        yield break;
    }
}
