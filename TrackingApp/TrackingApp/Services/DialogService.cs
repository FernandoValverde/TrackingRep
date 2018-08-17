using System.Threading.Tasks;
using TrackingApp.Interfaces;
using Xamarin.Forms;

namespace TrackingApp.Services
{
    public class DialogService
    {
        public async Task ShowMessage(string title, string message)
        {
            await App.Current.MainPage.DisplayAlert(title, message, "Aceptar");
        }

        public async Task<bool> ShowConfirm(string title, string message)
        {
            return await App.Current.MainPage.DisplayAlert(title, message, "Si", "No");
        }

        public void ShortToast(string message)
        {
            DependencyService.Get<IMessage>().ShortAlert(message);
        }

        public void LongToast(string message)
        {
            DependencyService.Get<IMessage>().LongAlert(message);
        }

        public async Task<string> DisplayActionList(string title, string[] options)
        {
            return await App.Current.MainPage.DisplayActionSheet(title, "Salir", null, options);
        }

        
    }

}
