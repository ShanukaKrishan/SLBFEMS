using SLBFEMS.Models;
using SLBFEMS.ViewModels.Authentication;

namespace SLBFEMS.Interfaces
{
    public interface IAuthService
    {
        public void SendEmail(string type, string inputEmail = null, ApplicationUserModel user = null, string token = null);

        public NICInfoViewModel GetNICInfo(string NIC);
    }
}
