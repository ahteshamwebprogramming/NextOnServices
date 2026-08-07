using NextOnServices.Infrastructure.Models.APIProjects;

namespace NextOnServices.WebUI.VT.Services;

public interface IZampliaClient
{
    Task<ZampliaProxyResponse> ExecuteAsync(string actionPath, ZampliaProxyRequest request);
}
