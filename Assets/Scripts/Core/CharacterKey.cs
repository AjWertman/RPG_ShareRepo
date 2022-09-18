namespace RPGProject.Core
{
    /// <summary>
    /// Key to distinguish Unit (SOs) and PlayableCharacter (SOs).
    /// </summary>
    public enum CharacterKey
    {
        None = 0,

        //Player Characters
        Protagonist = 1,
        PDR = 2,
        Astra = 3,

        //Unqiue Enemies
        Boss = 4,

        //Generic Enemies
        rEnemy = 5,
        mEnemy = 6,
        Officer = 7
    }
}