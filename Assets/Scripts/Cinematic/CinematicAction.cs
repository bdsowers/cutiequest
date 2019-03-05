using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CinematicAction : System.ICloneable
{
    /// <summary>
    /// Each action should have a unique action name that will be used in a script to trigger
    /// the action.
    /// </summary>
    public abstract string actionName { get; }

    /// <summary>
    /// Actions can have aliases, or other names that represent the action but may use different
    /// default parameter values under the hood.
    /// Example: "lerp_material_property" may be called "fade_in" when we know we want to fade an alpha
    /// property between 0 & 1 and don't need to explicitly say that everytime in the script.
    /// </summary>
    public virtual string[] aliases {  get { return null; } }

    /// <summary>
    /// Many actions only need a single parameter. Example: wait_seconds usually only needs to know how many seconds.
    /// It's cumbersome to type wait_seconds seconds: 3 every time when we could just say wait_seconds 3.
    /// This specifies the name of the simple parameter so that it can be placed in the parameter list properly.
    /// </summary>
    public virtual string simpleParameterName {  get { return null; } }

    /// <summary>
    /// All actions can be given an identifier so they can be found later.
    /// </summary>
    public string id { get; protected set; }

    /// <summary>
    /// Actions may auto-play in response to some events. The CinematicDirector will listen for these and play
    /// the appropriate cinematic.
    /// </summary>
    public string triggerEvent { get; set; }

    /// <summary>
    /// Has the action finished playing?
    /// It's up to individual actions to set this appropriately.
    /// </summary>
    public bool complete { get; protected set; }

    /// <summary>
    /// Should the action halt progression of the containing animation from moving forward until it's complete?
    /// Usually the default is no, though individual actions can override this default, and it can 
    /// be specified explicitly within the script.
    /// </summary>
    public bool shouldYield { get; set; }

    /// <summary>
    /// If an action was created via an alias instead of its default actionName, this will be filled
    /// in with the alias used. Actions can use this information to setup appropriate defaults.
    /// </summary>
    public string alias { get; set; }

    /// <summary>
    /// Some actions are children of others. This allows an action to see if its parent, or null if it has no parent
    /// </summary>
    public CinematicAction parentAction { get; set; }

    /// <summary>
    /// Some actions require child actions to perform their work.
    /// While any action can technically have child actions, it really only currently makes
    /// sense for CinematicAction.
    /// </summary>
    protected List<CinematicAction> mChildActions = new List<CinematicAction>();

    protected Dictionary<string, string> mParameters = new Dictionary<string, string>();

    public void SetParameters(Dictionary<string, string> parameters)
    {
        mParameters = parameters;

        // These parameters need to be interpreted immediately and don't have any real need for variable support
        string newId = null;
        if (parameters.TryGetValue("id", out newId))
        {
            id = newId;
        }

        string shouldYieldStr = null;
        if (parameters.TryGetValue("yield", out shouldYieldStr))
        {
            shouldYield = (shouldYieldStr == "yes" || shouldYieldStr == "true" || shouldYieldStr == "1");
        }

        string newTriggerEvent = null;
        if (parameters.TryGetValue("trigger", out newTriggerEvent))
        {
            triggerEvent = newTriggerEvent;
        }
    }

    /// <summary>
    /// Gets the raw parameter value. Does not do any kind of variable mapping or data manipulation.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetParameter(string key)
    {
        return mParameters[key];
    }

    /// <summary>
    /// Gets called if key/value pairs are provided for the action
    /// </summary>
    /// <param name="parameters"></param>
    public virtual void InterpretParameters(CinematicDataProvider dataProvider)
    {
    }

    /// <summary>
    /// Every action must override this coroutine. This will be responsible for actually handling the
    /// playback of the action and should not finish until the action has fully completed.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public abstract IEnumerator PlayInternal(CinematicDirector player);

    public IEnumerator Play(CinematicDirector player)
    {
        complete = false;

        InterpretParameters(player.dataProvider);
        
        yield return PlayInternal(player);

        complete = true;

        yield break;
    }

    public virtual void AddChildAction(CinematicAction childAction)
    {
        mChildActions.Add(childAction);
    }

    /// <summary>
    /// Clones an action using a memberwise clone (but clears out the child action list).
    /// This is only used by the pluggable factory to be able to create new cinematic actions
    /// without requiring extra setup code. Technically this can be overridden, but use-cases
    /// of that are sparse.
    /// </summary>
    /// <returns></returns>
    public virtual object Clone()
    {
        CinematicAction clone = (CinematicAction)this.MemberwiseClone();
        clone.mChildActions = new List<CinematicAction>();

        return (object)clone;
    }

    /// <summary>
    /// Plays all the child actions of this action, respecting if they should yield or not before moving forward.
    /// It's only necessary to call this if we expect to have child actions, which very few actions have.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    protected IEnumerator PlayChildActions(CinematicDirector player)
    {
        for (int i = 0; i < mChildActions.Count; ++i)
        {
            // TODO bdsowers - this is getting called too much now
            mChildActions[i].InterpretParameters(player.dataProvider);

            if (mChildActions[i].shouldYield)
            {
                yield return player.PlayAction(mChildActions[i]);
            }
            else
            {
                player.PlayAction(mChildActions[i]);
            }
        }

        while (!AllChildActionsFinished())
        {
            yield return null;
        }

        yield break;
    }

    protected bool AllChildActionsFinished()
    {
        bool allCoroutinesFinished = true;

        for (int i = 0; i < mChildActions.Count; ++i)
        {
            if (!mChildActions[i].complete)
            {
                allCoroutinesFinished = false;
                break;
            }
        }

        return allCoroutinesFinished;
    }

    // Allow us to navigate the parents/children of CinematicActions easily
    public T GetParentOfType<T>() where T : CinematicAction
    {
        CinematicAction parent = this;
        while (parent != null)
        {
            if (parent is T)
                return parent as T;

            parent = parent.parentAction;
        }

        return null;
    }

    public CinematicAction GetRootAction()
    {
        CinematicAction parent = this;
        while (parent.parentAction != null)
        {
            parent = parent.parentAction;
        }

        return parent;
    }

    public T GetChildOfType<T>() where T : CinematicAction
    {
        return GetChildOfType<T>(this);
    }

    public CinematicAction GetChildWithId(string id)
    {
        return GetChildWithId(id, this);
    }

    public int GetChildrenOfType<T>(T[] result) where T : CinematicAction
    {
        return GetChildrenOfType<T>(result, this, 0);
    }

    private T GetChildOfType<T>(CinematicAction root) where T : CinematicAction
    {
        if (root is T)
            return root as T;

        for (int i = 0; i < root.mChildActions.Count; ++i)
        {
            CinematicAction child = root.mChildActions[i];
            CinematicAction result = GetChildOfType<T>(child);
            if (result != null)
            {
                return result as T;
            }
        }

        return null;
    }

    private int GetChildrenOfType<T>(T[] result, CinematicAction root, int count) where T : CinematicAction
    {
        if (root is T)
        {
            result[count] = root as T;
            count++;
        }

        for (int i = 0; i < root.mChildActions.Count; ++i)
        {
            CinematicAction child = root.mChildActions[i];
            count = GetChildrenOfType<T>(result, child, count); 
        }

        return count;
    }

    private CinematicAction GetChildWithId(string id, CinematicAction root)
    {
        if (root.id == id)
            return root;

        for (int i = 0; i < root.mChildActions.Count; ++i)
        {
            CinematicAction child = root.mChildActions[i];
            CinematicAction result = GetChildWithId(id, child);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}
