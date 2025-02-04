namespace Burguer_Manager.Pages
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        private async void OnReservasButtonClicked(object sender, EventArgs e)
        {
            
            await Navigation.PushAsync(new ReservasPage()); 
        }

        private async void OnAddReservasButtonClicked(object sender, EventArgs e)
        {
           
            await Navigation.PushAsync(new AddReservasPage()); 
        }
    }
}
