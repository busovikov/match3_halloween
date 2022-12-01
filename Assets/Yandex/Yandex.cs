using UnityEngine;
using System.Runtime.InteropServices;
public class Yandex : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void ShowFullScreenAdv();

}
