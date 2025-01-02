using UnityEngine;

public static class TransformExtensions
{
    /// <summary>
    /// �d����w���|�U���l����������ե�C
    /// </summary>
    /// <typeparam name="T">�ե������A���~�Ӧ� Component�C</typeparam>
    /// <param name="parent">������ Transform�C</param>
    /// <param name="path">�l���󪺸��|�C</param>
    /// <returns>�p�G���A��^�ե�F�_�h��^ null�C</returns>
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
                Debug.LogError($"�l���� '{path}' �S�����ե� {typeof(T)}�I");
                return null;
            }
        }
        else
        {
            Debug.LogError($"�L�k�b '{parent.name}' �U���l���� '{path}'�I");
            return null;
        }
    }
}