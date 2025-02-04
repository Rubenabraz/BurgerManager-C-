using Burguer_Manager.Utilities;
using Firebase.Auth;
using Google.Cloud.Firestore;
using Microsoft.Maui.Controls;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Burguer_Manager.Pages
{
    public partial class AddFuncPage : ContentPage
    {
        private readonly FirebaseAuthClient _authClient;
        private readonly FirestoreDb _firestoreDb;

        public AddFuncPage(FirebaseAuthClient authClient)
        {
            InitializeComponent();
            Debug.WriteLine("Inicializando a página AddFuncPage");

            
            _authClient = authClient;
            _firestoreDb = FirestoreHelper.InitializeFirestore();
        }

        private async void OnSalvarFuncionarioClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Botão 'Adicionar Funcionário' clicado");

            // recolhes os valores dos inputs
            var nomeFuncionario = NomeEntry?.Text;
            var emailFuncionario = EmailEntry?.Text;
            var senhaFuncionario = PasswordEntry?.Text;

            Debug.WriteLine($"Valores capturados: Nome={nomeFuncionario}, Email={emailFuncionario}");

            // validacoes
            if (string.IsNullOrWhiteSpace(nomeFuncionario))
            {
                Debug.WriteLine("Validação falhou: Nome do funcionário está vazio");
                await DisplayAlert("Erro", "O nome do funcionário é obrigatório.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(emailFuncionario) || !emailFuncionario.Contains("@"))
            {
                Debug.WriteLine("Validação falhou: E-mail do funcionário inválido");
                await DisplayAlert("Erro", "E-mail inválido.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(senhaFuncionario) || senhaFuncionario.Length < 6)
            {
                Debug.WriteLine("Validação falhou: Senha inválida");
                await DisplayAlert("Erro", "A senha deve ter pelo menos 6 caracteres.", "OK");
                return;
            }

            try
            {
                // regista o func no firebaseAuth
                var userCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(emailFuncionario, senhaFuncionario);
                if (userCredential?.User == null)
                {
                    Debug.WriteLine("Erro ao registrar o funcionário no Firebase Authentication");
                    await DisplayAlert("Erro", "Erro ao registrar funcionário. Tente novamente.", "OK");
                    return;
                }

                var userId = userCredential.User.Uid;

                Debug.WriteLine("Criando objeto de funcionário");
                // cria o objeto func
                var funcionario = new
                {
                    Nome = nomeFuncionario,
                    Email = emailFuncionario,
                    Nivel = 2 // nivel padrao dos funcionarios
                };

                Debug.WriteLine("Enviando funcionário para o Firestore");
                
                DocumentReference docRef = _firestoreDb.Collection("utilizadores").Document(userId);
                await docRef.SetAsync(funcionario);

                Debug.WriteLine("Funcionário salvo com sucesso no Firestore");

             
                await DisplayAlert("Sucesso", "Funcionário adicionado com sucesso!", "OK");

                // limpar os campos
                Debug.WriteLine("Limpando campos após o sucesso");
                NomeEntry.Text = string.Empty;
                EmailEntry.Text = string.Empty;
                PasswordEntry.Text = string.Empty;
            }
            catch (FirebaseAuthException ex)
            {
                Debug.WriteLine($"Erro de autenticação: {ex.Reason}");
                await DisplayAlert("Erro", $"Erro de autenticação: {ex.Reason}", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao salvar o funcionário: {ex.Message}");
                await DisplayAlert("Erro", $"Erro ao salvar o funcionário: {ex.Message}", "OK");
            }
        }
    }
}
