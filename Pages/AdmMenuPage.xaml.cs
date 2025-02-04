using Firebase.Auth;

namespace Burguer_Manager.Pages;

public partial class AdmMenuPage : ContentPage
{
    private readonly FirebaseAuthClient _authClient;

    public AdmMenuPage(FirebaseAuthClient authClient)
    {
        InitializeComponent();
        _authClient = authClient; // armazena a mensagem do fireAuthClient
    }

    private async void OnReservasButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ReservasPage());
    }

    private async void OnAddReservasButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddReservasPage());
    }

    private async void OnAddFuncButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddFuncPage(_authClient));
    }
}
