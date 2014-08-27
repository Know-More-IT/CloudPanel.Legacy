
namespace CloudPanel.services
{
    public class Authenticated : IStatus
    {
        /// <summary>
        /// Username
        /// </summary>
        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }        
    }
}