namespace SSR.WebAPI.Installers;

public interface IInstaller
{
    void InstallServices(IServiceCollection services, IConfiguration configuration);
}