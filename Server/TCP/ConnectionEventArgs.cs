using System;

namespace Server.TCP
{
    public class ConnectionEventArgs : EventArgs
    {
        public ConnectionEventArgs(int id)
        {
            ClientId = id;
        }
        public int ClientId { get; set; }
    }
}