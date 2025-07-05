using Microsoft.Extensions.DependencyInjection;
using PennyPilot.Backend.Application.Interfaces;
using PennyPilot.Backend.Application.Services;
using PennyPilot.Backend.Domain.Interfaces;
using PennyPilot.Backend.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IIncomeService, IncomeService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IChartsService, ChartsService>();
            services.AddScoped<ICardsService, CardsService>();
            services.AddScoped<IFilterService, FilterService>();   

            return services;
        }
    }
}
