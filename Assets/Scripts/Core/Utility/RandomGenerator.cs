using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Core
{
    public class RandomGenerator : MonoBehaviour
    {
        public static float GetRandomNumber(float _min, float _max)
        {
            return Random.Range(_min, _max);
        }

        public static int GetRandomNumber(int _min, int _max)
        {
            return Random.Range(_min, _max + 1);
        }

        public static bool GetRandomBool()
        {
            int coinFlip = GetRandomNumber(0, 1);

            return coinFlip == 0;
        }

        public static string GetRandomString(string[] _strings)
        {
            return _strings[GetRandomNumber(0, _strings.Length - 1)];
        }

        public static string GetRandomString(List<string> _strings)
        {
            return _strings[GetRandomNumber(0, _strings.Count - 1)];
        }

        public static GameObject GetRandomGameObject(GameObject[] _gameObjects)
        {
            return _gameObjects[GetRandomNumber(0, _gameObjects.Length - 1)];
        }

        public static GameObject GetRandomGameObject(List<GameObject> _gameObjects)
        {
            return _gameObjects[GetRandomNumber(0, _gameObjects.Count - 1)];
        }

        public static object GetRandomObject(object[] _objects)
        {
            return _objects[GetRandomNumber(0, _objects.Length - 1)];
        }

        public static object GetRandomObject(List<object> _objects)
        {
            return _objects[GetRandomNumber(0, _objects.Count - 1)];
        }

        public static Color32 GetRandomColor()
        {
            byte r = (byte)GetRandomNumber(0f, 255f);
            byte g = (byte)GetRandomNumber(0f, 255f);
            byte b = (byte)GetRandomNumber(0f, 255f);
            return new Color32(r, g, b, 255);
        }

        public static Color32 GetRandomColor(float _alpha)
        {
            byte r = (byte)GetRandomNumber(0f, 255f);
            byte g = (byte)GetRandomNumber(0f, 255f);
            byte b = (byte)GetRandomNumber(0f, 255f);

            _alpha = Mathf.Clamp(_alpha, 0f, 225f);
            byte a = (byte)_alpha; 

            return new Color32(r, g, b, a);
        }
    }
}