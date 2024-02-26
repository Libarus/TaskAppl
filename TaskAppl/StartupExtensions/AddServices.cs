using TaskAppl.DataAccess.Interfaces;
using TaskAppl.DataAccess.Services;

namespace TaskAppl.StartupExtensions
{
    /// <summary>
    /// Класс сервисных рсширений
    /// </summary>
    public static class ServiceExtentions
    {
        /// <summary>
        /// Добавляет сервисы для DI
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<ITaskFileService, TaskFileService>();

            return services;
        }
    }
}
