using ETL_Shared.InputModels;
using System.Threading.Tasks;

namespace ETL_Front.Services 
{
    public interface IHttpClientWrapper
    {
        Task<string> GetStringAsync(string v);
        Task PostJsonAsync(string v, PatientInputModel model);
    }
}
