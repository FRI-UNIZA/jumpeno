using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System;
using System.Threading.Tasks;

namespace JumpenoWebassembly.Client.ExceptionHandling
{
    public class CustomErrorBoundary : ErrorBoundary
    {
        [Inject]
        private IWebAssemblyHostEnvironment env { get; set; }
        [Inject]
        private NavigationManager navigationManager { get; set; }
        protected override Task OnErrorAsync(Exception exception)
        {
            Console.WriteLine("SAHFASHFDKJLASHFJK");
            if (env.IsDevelopment())
            {
                return base.OnErrorAsync(exception);
            }
            return Task.CompletedTask;
        }

        public void Clear()
        {
            //TODO post exception to server through REST
            Recover();
        }
    }
}
        