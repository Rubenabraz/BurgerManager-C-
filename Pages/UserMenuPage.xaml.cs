namespace Burguer_Manager.Pages
{
    public partial class UserMenuPage : ContentPage
    {
        public UserMenuPage()
        {
            InitializeComponent();
        }

        private async void OnReservarButtonClicked(object sender, EventArgs e)
        {
            
            await Navigation.PushAsync(new UserReserva()); 
        }

        private async void OnReservasButtonClicked(object sender, EventArgs e)
        {
            
            await Navigation.PushAsync(new UserReservas()); 
        }

        private async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            
            await Navigation.PopToRootAsync();
        }
    }
}
