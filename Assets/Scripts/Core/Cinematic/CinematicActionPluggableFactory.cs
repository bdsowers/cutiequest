using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CinematicActionPluggableFactory
{
    List<CinematicAction> mRegisteredActionTemplates = new List<CinematicAction>();

    Dictionary<string, CinematicAction> mAliases = new Dictionary<string, CinematicAction>();

	public CinematicActionPluggableFactory()
    {
        System.Type[] types = Assembly.GetAssembly(typeof(CinematicAction)).GetTypes();
        
        for (int i = 0; i < types.Length; ++i)
        {
            System.Type type = types[i];
            if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(CinematicAction)))
            {
                CinematicAction template = (CinematicAction)Activator.CreateInstance(type);
                RegisterActionTemplate(template);

                mAliases.Add(template.actionName, template);

                if (template.aliases != null)
                {
                    for (int aliasIdx = 0; aliasIdx < template.aliases.Length; ++aliasIdx)
                    {
                        mAliases.Add(template.aliases[aliasIdx], template);
                    }
                }
            }
        }
    }

    public void RegisterActionTemplate(CinematicAction actionTemplate)
    {
        mRegisteredActionTemplates.Add(actionTemplate);
    }

    public CinematicAction ConstructNewActionByName(string name)
    {
        CinematicAction newAction = (CinematicAction)mAliases[name].Clone();
        return newAction;
    }

    public bool ActionExists(string name)
    {
        return mAliases.ContainsKey(name);
    }
}
