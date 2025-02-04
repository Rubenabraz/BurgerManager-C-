using Burguer_Manager.Utilities;
using Firebase.Auth;
using Google.Cloud.Firestore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Burguer_Manager.Pages;

    public partial class RegisterPage : ContentPage
    {
        private readonly FirebaseAuthClient _authClient;
        private readonly FirestoreDb _firestoreDb;

        public RegisterPage(FirebaseAuthClient authClient)
        {
            InitializeComponent();
            _authClient = authClient;

            try
            {
                
                _firestoreDb = FirestoreHelper.InitializeFirestore();
            }
            catch (Exception ex)
            {
                ResultLabel.TextColor = Colors.Red;
                ResultLabel.Text = $"Erro ao inicializar Firestore: {ex.Message}";
                return;
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            var email = EmailEntry?.Text;
            var password = PasswordEntry?.Text;

            // verifica os inputs
            if (string.IsNullOrWhiteSpace(email))
            {
                ResultLabel.TextColor = Colors.Red;
                ResultLabel.Text = "O campo de email � obrigat�rio.";
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ResultLabel.TextColor = Colors.Red;
                ResultLabel.Text = "O campo de senha � obrigat�rio.";
                return;
            }

            if (password.Length < 6)
            {
                ResultLabel.TextColor = Colors.Red;
                ResultLabel.Text = "A senha deve ter pelo menos 6 caracteres.";
                return;
            }

            try
            {
                // registrar o utilizador no Firebase Authentication
                var userCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);

                // verificar se o registo foi OK
                if (userCredential?.User == null)
                {
                    ResultLabel.TextColor = Colors.Red;
                    ResultLabel.Text = "Erro ao criar utilizador. Tente novamente.";
                    return;
                }

                // ir buscar o UID do utilizar para trabalhar com ele
                var userId = userCredential.User.Uid;

                // definir o nivel de acesso (padr�o 1 = utilizador)
                int nivel = 1;

                // sar save dos dados
                await SaveUserToFirestore(userId, email, password, nivel);

                ResultLabel.TextColor = Colors.Green;
                ResultLabel.Text = "Conta criada com sucesso!";

                // back pro login
                await Navigation.PopAsync();
            }
            catch (FirebaseAuthException ex)
            {
                ResultLabel.TextColor = Colors.Red;
                ResultLabel.Text = $"Erro de autentica��o: {ex.Reason}";
            }
            catch (Exception ex)
            {
                ResultLabel.TextColor = Colors.Red;
                ResultLabel.Text = $"Erro: {ex.Message}";
            }
        }

    // funcao para dar save do utilizador na bd
    private async Task SaveUserToFirestore(string userId, string email, string password, int nivel)
    {
        try
        {
            if (_firestoreDb == null)
            {
                Console.WriteLine("Firestore n�o inicializado.");
                ResultLabel.TextColor = Colors.Red;
                ResultLabel.Text = "Erro: Firestore n�o est� inicializado.";
                return;
            }

            Console.WriteLine("Tentando criar documento para o usu�rio...");
            DocumentReference docRef = _firestoreDb.Collection("utilizadores").Document(userId);

            var userData = new
            {
                Email = email,
                Password = password,
                Nivel = nivel,
            };

            await docRef.SetAsync(userData);
            Console.WriteLine("Usu�rio salvo com sucesso no Firestore.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao salvar no Firestore: {ex.Message}");
            ResultLabel.TextColor = Colors.Red;
            ResultLabel.Text = $"Erro inesperado: {ex.Message}";
        }
    }

}

