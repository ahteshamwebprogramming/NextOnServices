using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;

namespace NextOnServices.Services.DBContext;

public class UnitOfWork : IUnitOfWork
{
    private readonly NextOnServicesDbContext _context;
    private readonly DapperDBSetting _dbSetting;

    public UnitOfWork(NextOnServicesDbContext context, DapperDBSetting dbSetting)
    {
        _context = context;
        _dbSetting = dbSetting;
        User = new UserRepository(_context);
        Project = new ProjectRepository(dbSetting);
        GProject = new GProjectRepository(_context);
        Client = new ClientRepository(dbSetting);

        CountryMaster = new CountryMasterRepository(_context);
        StatusMaster = new StatusMasterRepository(_context);
        ProjectsUrl = new ProjectsUrlRepository(dbSetting);
        ProjectMapping = new ProjectMappingRepository(dbSetting);
        Suppliers = new SuppliersRepository(dbSetting);
        SupplierPanelSize = new SupplierPanelSizeRepository(dbSetting);
        SupplierLogin = new SupplierLoginRepository(dbSetting);
        GenOperations = new GenOperationsRepository(dbSetting);
        SupplierProjects = new SupplierProjectsRepository(dbSetting);
        SupplierProjectMessages = new SupplierProjectMessageRepository(dbSetting);
    }
    public IUserRepository User
    { get; private set; }
    public IProjectRepository Project
    { get; private set; }
    public IGProjectRepository GProject
    { get; private set; }
    public IClientRepository Client
    { get; private set; }

    public ICountryMasterRepository CountryMaster
    { get; private set; }
    public IStatusMasterRepository StatusMaster { get; private set; }
    public IProjectMappingRepository ProjectMapping { get; private set; }
    public IProjectsUrlRepository ProjectsUrl { get; private set; }
    public ISuppliersRepository Suppliers { get; private set; }
    public ISupplierPanelSizeRepository SupplierPanelSize { get; private set; }
    public ISupplierLoginRepository SupplierLogin { get; private set; }
    public IGenOperationsRepository GenOperations { get; private set; }
    public ISupplierProjectsRepository SupplierProjects { get; private set; }
    public ISupplierProjectMessageRepository SupplierProjectMessages { get; private set; }
    public void Dispose()
    {
        try { _context.Dispose(); }
        catch { }
    }

    public int Save()
    {
        return _context.SaveChanges();
    }
}
