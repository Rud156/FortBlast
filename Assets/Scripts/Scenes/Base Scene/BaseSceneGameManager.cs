using FortBlast.BaseClasses;

public class BaseSceneGameManager : GameManager
{
    #region Singleton

    public static BaseSceneGameManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        if (instance != this)
            Destroy(gameObject);
    }

    #endregion
}