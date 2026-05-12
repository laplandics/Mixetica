using UnityEngine;

public static class R
{
    public static T Get<T>(string path) where T : Object => Resources.Load<T>(path);
    public static T[] GetAll<T>(string folder) where T : Object => Resources.LoadAll<T>(folder);
    public static ResourceRequest GetAsync<T>(string path) where T : Object => Resources.LoadAsync<T>(path);
}