using TMPro;
using UnityEngine;
using UnityEngine.Events;
public class Message_Pooling : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] Message_Box;
    public UnityAction<string> Message;
    private string current_me = string.Empty;
    private string past_me;

    private void Start()
    {
        Message_Box=transform.GetComponentsInChildren<TextMeshProUGUI>();
        Message=AddingMessage;
        past_me=current_me;
    }

    public void AddingMessage(string me)
    {
        current_me=me;
    }
    public void Reatext(string me)
    {
        bool isinput=false;
        for(int i=0;i<Message_Box.Length;i++)
        {
            if(Message_Box[i].text.Equals(""))
            {
                Message_Box[i].text=me;
                isinput=true;
                break;
            }
        }
        if(isinput==false)
        {
            for(int i=1;i<Message_Box.Length;i++)
            {
                Message_Box[i-1]=Message_Box[i];
            }
            Message_Box[Message_Box.Length-1].text=me;
        }
    }



}
