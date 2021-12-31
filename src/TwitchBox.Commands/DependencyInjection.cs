using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace TwitchBox.Commands
{
    public static class DependencyInjection
    {
        public static void AddCommands(this IServiceCollection services)
            => services.AddMediatR(typeof(DependencyInjection));
    }
}