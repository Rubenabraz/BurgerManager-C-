using Burguer_Manager.Pages;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Burguer_Manager.Utilities;

namespace Burguer_Manager
{
    public partial class HomePage : ContentPage
    {
        private readonly FirebaseAuthClient _authClient;
        private readonly FirestoreDb _firestoreDb;

        public HomePage()
        {
            InitializeComponent();

            // iniciar FirebaseAuth para poder utilizar o uid do utilizador na relacao da bd
            _authClient = new FirebaseAuthClient(new FirebaseAuthConfig
            {
                ApiKey = "AIzaSyBn2achqZ1n_-EM1BbTq4wgtR5BS1cPotQ", 
                AuthDomain = "burgermanager-3b131.firebaseapp.com",  
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            });

            
            _firestoreDb = FirestoreHelper.InitializeFirestore();
        }

        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            
            await Navigation.PushAsync(new RegisterPage(_authClient));
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            var email = EmailEntry?.Text;
            var password = PasswordEntry?.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Erro", "Preencha todos os campos.", "OK");
                return;
            }

            try
            {
                var userCredential = await _authClient.SignInWithEmailAndPasswordAsync(email, password);
                App.SetUsuarioAtual(userCredential.User);

                var nivel = await ObterNivelUsuarioAsync(userCredential.User.Uid);

                if (nivel == null)
                {
                    await DisplayAlert("Erro", "Usuário não encontrado no Firestore.", "OK");
                    return;
                }

                switch (nivel)
                {
                    case 1:
                        await Navigation.PushAsync(new UserMenuPage());
                        break;
                    case 2:
                        await Navigation.PushAsync(new MenuPage());
                        break;
                    case 3:
                        await Navigation.PushAsync(new AdmMenuPage(_authClient)); // passa o FirebaseAuthClient para a proxima pagina
                        break;
                    default:
                        await DisplayAlert("Erro", "Nível de usuário inválido.", "OK");
                        break;
                }

            }
            catch (FirebaseAuthException ex)
            {
                await DisplayAlert("Erro de autenticação", $"Código: {ex.Reason}\nMensagem: {ex.Message}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro inesperado: {ex.Message}", "OK");
            }
        }


        private async Task<int?> ObterNivelUsuarioAsync(string uid)
        {
            try
            {
                
                var docRef = _firestoreDb.Collection("utilizadores").Document(uid);
                var snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    // retornar o valor do campo nivel
                    return snapshot.GetValue<int>("Nivel");
                }

                return null; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar nível do usuário: {ex.Message}");
                return null;
            }
        }
    }
}
