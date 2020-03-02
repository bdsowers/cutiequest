using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Common base class for components which are part of a character hierarchy
/// and commonly need access to parts of the character that could be scattered
/// across the hierarchy.
/// This is largely a performance optimization to cache commonly used components
/// efficiently without having to edit a million prefabs. Functionality
/// going into this class is probably a mistake.
/// </summary>
public class CharacterComponentBase : MonoBehaviour
{
    public class CommonCharacterComponents
    {
        private Animator mAnimator;

        private GameObject mRoot;
        public SimpleMovement simpleMovement { get; private set; }
        public SimpleAttack simpleAttack { get; private set; }
        public Killable killable { get; private set; }
        public CharacterModel characterModel { get; private set; }
        public RevealWhenAvatarIsClose revealWhenAvatarIsClose { get; private set; }

        // note bdsowers - Animator changes more frequently than the others, so we need
        // a guard in case it becomes null
        public Animator animator
        {
            get
            {
                if (mAnimator == null)
                    mAnimator = mRoot.GetComponentInChildren<Animator>();

                return mAnimator;
            }
        }

        public CommonCharacterComponents(GameObject root)
        {
            mRoot = root;

            simpleMovement = root.GetComponentInChildren<SimpleMovement>();
            simpleAttack = root.GetComponentInChildren<SimpleAttack>();
            killable = root.GetComponentInChildren<Killable>();
            characterModel = root.GetComponentInChildren<CharacterModel>();
            revealWhenAvatarIsClose = root.GetComponentInChildren<RevealWhenAvatarIsClose>();
        }
    }

    private bool mComponentsCached;
    private GameObject mCharacterRoot;
    private CommonCharacterComponents mCommonComponents;

    public GameObject characterRoot
    {
        get
        {
            CacheComponentsIfNecessary();
            return mCharacterRoot;
        }
    }

    public CommonCharacterComponents commonComponents
    {
        get
        {
            CacheComponentsIfNecessary();
            return mCommonComponents;
        }
    }

    private void CacheComponentsIfNecessary()
    {
        if (mComponentsCached)
            return;

        mComponentsCached = true;

        FindRoot();

        mCommonComponents = new CommonCharacterComponents(mCharacterRoot);
    }

    private void FindRoot()
    {
        // Simple check - most things fall into this
        SimpleMovement movement = gameObject.GetComponentInParent<SimpleMovement>();
        if (movement != null)
        {
            mCharacterRoot = movement.gameObject;
            return;
        }

        // todo bdsowers - this is dangerous and can break at the drop of a hat.
        // More complicated check - not everything meets this criteria though
        mCharacterRoot = gameObject;
        while (mCharacterRoot.transform.parent != null)
        {
            mCharacterRoot = gameObject.transform.parent.gameObject;
        }
    }
}
