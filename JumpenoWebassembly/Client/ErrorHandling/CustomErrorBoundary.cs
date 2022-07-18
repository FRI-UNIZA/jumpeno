using JumpenoWebassembly.Client.Services;
using JumpenoWebassembly.Shared.ErrorHandling;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System;
using System.Threading.Tasks;

namespace JumpenoWebassembly.Client.ErrorHandling
{
    public class CustomErrorBoundary : ErrorBoundary
    {
        [Inject]
        private IWebAssemblyHostEnvironment env { get; set; }
        protected override Task OnErrorAsync(Exception exception)
        { 
            if (env.IsDevelopment())
            {
                return base.OnErrorAsync(exception);
            }
            return Task.CompletedTask;
        }

        public void Clear()
        {
            Recover();
        }
    }
}
        