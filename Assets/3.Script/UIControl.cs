using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
    public TMP_InputField id_i;
    public TMP_InputField Password_i;

    public TMP_InputField newPwd_i;

    [SerializeField] private TextMeshProUGUI log;

    public void LoginBtn()
    {
        if (id_i.text.Equals(string.Empty) || Password_i.text.Equals(string.Empty))
        {
            log.text = "아이디와 비밀번호를 입력하세요";
            return;
        }

        if (SQL_Manager.instance.Login(id_i.text, Password_i.text))
        {
            User_Info info = SQL_Manager.instance.info;
            Debug.Log(info.User_Name + " | " + info.User_PhoneNum);
            PopUP(gameObject);
        }
        else
        {
            log.text = "로그인 실패";
        }
    }

    public void RegisterBtn()
    {
        if (id_i.text.Equals(string.Empty) || Password_i.text.Equals(string.Empty))
        {
            log.text = "아이디와 비밀번호를 입력하세요";
            return;
        }

        if()
    }

    public void ChangeBtn()
    {

    }

    public void DeleteBtn()
    {

    }

    void PopUP(GameObject a)
    {
        a.SetActive(!a.activeSelf);
    }

}
