using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class Language : MonoBehaviour
{
    public LangObject ru;
    public LangObject en;
    public LangObject tr;
    static public LangObject current;

    public Text language;

    public Text _menu_play_time;
    public Text _menu_play_moves;
    public Text _menu_play_exit;
    public Text _menu_play_credits;
    public Text _menu_play_back;
    public Text _menu_play_menu;
    public Text _menu_play_next;
    public Text _menu_play_repeate;

    public Text _hud_score_label;
    public Text _popup_score_label;
    public Text _popup_best_label;
    public Text _popup_total_label;
    public Text _hud_level_label;
    public Text _hud_play_time;
    public Text _hud_play_moves;

    public Text _popup_win_label;
    public Text _popup_lose_label;

    public Text _popup_start_collect;
    public Text _popup_start_button_start;

    public Text _auth_label;
    public Text _open_bonus_chest;

    public Text _purchase_popup_close_button;
    public Text _purchase_description_coin5;
    public Text _purchase_description_coin20;
    public Text _purchase_description_coin100;

    public Text _developed_by;

    public void ChangeLang(string lang)
    {

        if (language) language.text = lang;

        if (lang == "ru" || lang == "be" || lang == "kk" || lang == "uk" || lang == "uz")
        {
            current = ru;
        }
        else if (lang == "en")
        {
            current = en;
        }
        else if (lang == "tr")
        {
            current = tr;
        }
        else 
        {
            current = en;
        }
    }

    private void UpdateText()
    {
        if (null != _menu_play_time) _menu_play_time.text = current._menu_play_time;
        if (null != _menu_play_moves) _menu_play_moves.text = current._menu_play_moves;
        if (null != _menu_play_exit) _menu_play_exit.text = current._menu_play_exit;
        if (null != _menu_play_credits) _menu_play_credits.text = current._menu_play_credits;
        if (null != _menu_play_back) _menu_play_back.text = current._menu_play_back;
        if (null != _menu_play_menu) _menu_play_menu.text = current._menu_play_menu;
        if (null != _menu_play_next) _menu_play_next.text = current._menu_play_next;
        if (null != _menu_play_repeate) _menu_play_repeate.text = current._menu_play_repeate;
        
        if (null != _hud_score_label) _hud_score_label.text = current._hud_score_label;
        if (null != _popup_score_label) _popup_score_label.text = current._popup_score_label;
        if (null != _popup_best_label) _popup_best_label.text = current._popup_best_label;
        if (null != _popup_total_label) _popup_total_label.text = current._popup_total_label;
        if (null != _hud_level_label) _hud_level_label.text = current._hud_level_label;
        if (null != _hud_play_time) _hud_play_time.text = current._hud_play_time;
        if (null != _hud_play_moves) _hud_play_moves.text = current._hud_play_moves;

        if (null != _popup_start_collect) _popup_start_collect.text = current._popup_start_collect;
        if (null != _popup_start_button_start) _popup_start_button_start.text = current._popup_start_button_start;

        if (null != _auth_label) _auth_label.text = current._auth_label;
        if (null != _open_bonus_chest) _open_bonus_chest.text = current._open_bonus_chest;

        if (null != _developed_by)_developed_by.text = current._developed_by;

        if (null != _purchase_popup_close_button) _purchase_popup_close_button.text = current._purchase_popup_close_button;
        if (null != _purchase_description_coin5) _purchase_description_coin5.text = current._purchase_description_coin5;
        if (null != _purchase_description_coin20) _purchase_description_coin20.text = current._purchase_description_coin20;
        if (null != _purchase_description_coin100) _purchase_description_coin100.text = current._purchase_description_coin100;
}

    private void Start()
    {
        string lang = "en";
#if PLATFORM_WEBGL && !UNITY_EDITOR
        lang = Yandex.GetLang();
#endif
        ChangeLang(lang);
        UpdateText();
    }

    public void ReceivedLanguage(string lang)
    {
        Debug.Log("ReceivedLanguage");
        ChangeLang(lang);
        UpdateText();
    }

}
