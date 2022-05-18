using System.Collections.Generic;
using UnityEngine;

namespace AjsUtilityPackage
{
    public class RandomGenerator : MonoBehaviour
    {
        public static float GetRandomNumber(float min, float max)
        {
            return Random.Range(min, max);
        }

        public static int GetRandomNumber(int min, int max)
        {
            return Random.Range(min, max + 1);
        }

        public static bool GetRandomBool()
        {
            int coinFlip = GetRandomNumber(0, 1);

            return coinFlip == 0;
        }

        public static string GetRandomString(string[] strings)
        {
            return strings[GetRandomNumber(0, strings.Length - 1)];
        }

        public static string GetRandomString(List<string> strings)
        {
            return strings[GetRandomNumber(0, strings.Count - 1)];
        }

        public static GameObject GetRandomGameObject(GameObject[] gameObjects)
        {
            return gameObjects[GetRandomNumber(0, gameObjects.Length - 1)];
        }

        public static GameObject GetRandomGameObject(List<GameObject> gameObjects)
        {
            return gameObjects[GetRandomNumber(0, gameObjects.Count - 1)];
        }

        public static object GetRandomObject(object[] objects)
        {
            return objects[GetRandomNumber(0, objects.Length - 1)];
        }

        public static object GetRandomObject(List<object> objects)
        {
            return objects[GetRandomNumber(0, objects.Count - 1)];
        }

        public static Color32 GetRandomColor()
        {
            byte r = (byte)GetRandomNumber(0f, 255f);
            byte g = (byte)GetRandomNumber(0f, 255f);
            byte b = (byte)GetRandomNumber(0f, 255f);
            return new Color32(r, g, b, 255);
        }

        public static Color32 GetRandomColor(float alpha)
        {
            byte r = (byte)GetRandomNumber(0f, 255f);
            byte g = (byte)GetRandomNumber(0f, 255f);
            byte b = (byte)GetRandomNumber(0f, 255f);

            alpha = Mathf.Clamp(alpha, 0f, 225f);
            byte a = (byte)alpha; 

            return new Color32(r, g, b, a);
        }
    }
}