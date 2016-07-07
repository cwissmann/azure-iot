namespace ConnectionMonitor
{
    public class ConnectionResult
    {
        private bool online;

        public ConnectionResult(bool online)
        {
            this.Online = online;
        }

        public bool Online
        {
            get
            {
                return this.online;
            }

            set
            {
                if (value == this.online)
                {
                    return;
                }

                this.online = value;
            }
        }
    }
}
