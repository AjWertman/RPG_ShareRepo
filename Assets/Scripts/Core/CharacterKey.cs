namespace RPGProject.Core
{
    //Refactor - remove and implement AssetBundles or Addressables
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