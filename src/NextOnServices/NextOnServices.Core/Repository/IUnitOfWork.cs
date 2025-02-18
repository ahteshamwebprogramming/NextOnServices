using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Core.Repository;

public interface IUnitOfWork : IDisposable
{
    public IUserRepository User { get; }
    public IProjectRepository Project { get; }
    public IGProjectRepository GProject { get; }
    public IClientRepository Client { get; }
   
    public ICountryMasterRepository CountryMaster { get; }
    public IStatusMasterRepository StatusMaster { get; }
    public IProjectMappingRepository ProjectMapping { get; }
    public IProjectsUrlRepository ProjectsUrl { get; }
    public ISuppliersRepository Suppliers { get; }
    public ISupplierPanelSizeRepository SupplierPanelSize { get; }
    public ISupplierLoginRepository SupplierLogin { get; }
    int Save();
}
