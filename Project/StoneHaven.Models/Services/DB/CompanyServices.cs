using Rhino.Commons;

namespace StoneHaven.Models.Services.DB
{
    public class CompanyServices : ICompanyServices
    {
        public Company SaveCompany(Company company)
        {
            return Repository<Company>.SaveOrUpdate(company);
        }
    }
}