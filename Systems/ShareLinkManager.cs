using UnityEngine;
using UnityEngine.Networking;

public class ShareLinkManager : MonoBehaviour
{
    public string gameStoreLink = "https://play.google.com/store/apps/details?id=com.TanoshiMobile.Wordfitt";

    public void ShareOnWhatsApp()
    {
        string message = "Check out this cool game: " + gameStoreLink;
        Application.OpenURL("https://api.whatsapp.com/send?text=" + UnityWebRequest.EscapeURL(message));
    }

    public void ShareOnTelegram()
    {
        string message = "Check out this cool game: " + gameStoreLink;
        Application.OpenURL("https://t.me/share/url?url=" + UnityWebRequest.EscapeURL(gameStoreLink) + "&text=" + UnityWebRequest.EscapeURL(message));
    }

    public void ShareOnMessenger()
    {
        string message = "Check out this cool game: " + gameStoreLink;
        Application.OpenURL("fb-messenger://share?link=" + UnityWebRequest.EscapeURL(gameStoreLink));
    }

    public void ShareOnTwitter()
    {
        string message = "Check out this cool game: " + gameStoreLink;
        Application.OpenURL("https://twitter.com/intent/tweet?text=" + UnityWebRequest.EscapeURL(message));
    }

    public void ShareOnEmail()
    {
        string subject = "Check out this game!";
        string body = "Check out this cool game: " + gameStoreLink;
        Application.OpenURL("mailto:?subject=" + UnityWebRequest.EscapeURL(subject) + "&body=" + UnityWebRequest.EscapeURL(body));
    }
}
