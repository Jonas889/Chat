using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WFClient
{
    [Serializable]
    public class Message
    {
        //public int Id { get; set; }
        public string Username { get; set; }
        public string ChatMessage { get; set; }
        public DateTime Time { get; set; }
        public List<string> Users { get; set; }
       
        
        #region constructors

        public Message()
        {
            
        }

        public Message(string userName, string chatMessage)
        {
            Username = userName;
            ChatMessage = chatMessage;
            Time = DateTime.Now;
           
        }
        #endregion
    }
}
