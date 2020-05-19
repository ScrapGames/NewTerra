
namespace Notifications
{
    public class Message
    {
        public event System.Action<Message> MessageDeleted;
        public Category category;
        public string title;
        public string text;
        public bool isRead = false;
        public int id;

        // Message Action/Callback

        public void OnMessageDeleted()
        {
            MessageDeleted?.Invoke(this);
        }
    }
}