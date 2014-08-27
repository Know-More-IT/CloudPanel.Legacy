using CloudPanel.Modules.Base.Class;

namespace CloudPanel.services
{
    public class IStatus
    {
        public Enumerations.Status Status { get; set; }
        public string ErrorMessage { get; set; }
    }
}