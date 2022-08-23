using System.Threading.Tasks;
using SLBFEMS.ViewModels.CVParser;

namespace SLBFEMS.Interfaces
{
    public interface ICVParserService
    {
        Task<CVResponseVIewModel> GetCvData(string url);
    }
}
