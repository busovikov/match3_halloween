using UnityEngine;
using System.Runtime.InteropServices;
public class Yandex : MonoBehaviour
{
#if UNITY_EDITOR
    static public bool auth_dumm = false;
#endif

    [DllImport("__Internal")]
    public static extern void ShowFullScreenAdv();

    [DllImport("__Internal")]
    public static extern void RateGame();

    [DllImport("__Internal")]
    public static extern void AuthorizePlayer();

    [DllImport("__Internal")]
    public static extern bool IsPlayerAuthorized();

    [DllImport("__Internal")]
    public static extern bool IsPlayerAbleReview();

    [DllImport("__Internal")]
    public static extern void SaveData(string obj);

    [DllImport("__Internal")]
    public static extern void LoadData();


    [DllImport("__Internal")]
    public static extern void SetScoreToLeaderBoard(int val);

    [DllImport("__Internal")]
    public static extern void SetBestTimeToLeaderBoard(int val);

    [DllImport("__Internal")]
    public static extern void SetBestMovesToLeaderBoard(int val);

    public enum CoinsOptions
    { 
        coins5,
        coins20,
        coins100,
    }

    [DllImport("__Internal")]
    public static extern void BuyCoins(int val);

    [DllImport("__Internal")]
    public static extern void ConsumePurchase(string token);


}
