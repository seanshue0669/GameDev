using UnityEngine;

public static class TransformExtensions
{
    /// <summary>
    /// 查找指定路徑下的子物件並獲取其組件。
    /// </summary>
    /// <typeparam name="T">組件類型，需繼承自 Component。</typeparam>
    /// <param name="parent">父物件的 Transform。</param>
    /// <param name="path">子物件的路徑。</param>
    /// <returns>如果找到，返回組件；否則返回 null。</returns>
    public static T FindChildComponent<T>(this Transform parent, string path) where T : Component
    {
        if (parent == null)
        {
            Debug.LogError("Parent Transform is null!");
            return null;
        }

        Transform child = parent.Find(path);
        if (child != null)
        {
            T component = child.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
            else
            {
                Debug.LogError($"子物件 '{path}' 沒有找到組件 {typeof(T)}！");
                return null;
            }
        }
        else
        {
            Debug.LogError($"無法在 '{parent.name}' 下找到子物件 '{path}'！");
            return null;
        }
    }
}