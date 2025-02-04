using Burguer_Manager.Utilities;
using Google.Cloud.Firestore;
using Microsoft.Maui.Controls;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Burguer_Manager.Pages
{
    public partial class UserReserva : ContentPage
    {
        // config das propriaedades para depois usar o binding no UI
        public DateTime ReservaData { get; set; }
        public TimeSpan ReservaHora { get; set; }
        private readonly FirestoreDb _firestoreDb;

        public UserReserva()
        {
            InitializeComponent();

            Debug.WriteLine("Inicializando a página AddReservasPage");

            ReservaData = DateTime.Now;
            ReservaHora = new TimeSpan(18, 0, 0); // estabelecer o horario base como 18

            // associa os dados
            BindingContext = this;

            Debug.WriteLine("Inicializando Firestore");
            
            _firestoreDb = FirestoreHelper.InitializeFirestore();

            // definir a data minima e maxima para o DatePicker para nao haver reservas para tras e demasiado para a frente
            Debug.WriteLine("Configurando DatePicker: Data mínima e máxima");
            DataReservaPicker.MinimumDate = DateTime.Now;
            DataReservaPicker.MaximumDate = DateTime.Now.AddDays(2); // 2 dias a frente max
        }

        private async void OnSalvarReservaClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Botão 'Adicionar Reserva' clicado");

            // recolhe os valores dos inputs
            var nomeCliente = ClienteEntry?.Text;
            var dataReserva = ReservaData;
            var horaReserva = ReservaHora;
            var numeroPessoasText = NumeroPessoasEntry?.Text;

            Debug.WriteLine($"Valores capturados: NomeCliente={nomeCliente}, DataReserva={dataReserva}, HoraReserva={horaReserva}, NumeroPessoas={numeroPessoasText}");

            // validações dos campos basicas
            if (string.IsNullOrWhiteSpace(nomeCliente))
            {
                Debug.WriteLine("Validação falhou: Nome do cliente está vazio");
                await DisplayAlert("Erro", "O nome do cliente é obrigatório.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(numeroPessoasText) || !int.TryParse(numeroPessoasText, out int numeroPessoas) || numeroPessoas <= 0)
            {
                Debug.WriteLine("Validação falhou: Número de pessoas inválido");
                await DisplayAlert("Erro", "Número de pessoas inválido.", "OK");
                return;
            }

            // pegar no email do user logado
            var usuarioLogado = App.UsuarioAtual;
            if (usuarioLogado == null || string.IsNullOrWhiteSpace(usuarioLogado.Info?.Email))
            {
                Debug.WriteLine("Erro: Usuário não autenticado");
                await DisplayAlert("Erro", "Usuário não autenticado.", "OK");
                return;
            }

            var emailCliente = usuarioLogado.Info.Email;

            try
            {
                Debug.WriteLine("Criando objeto de reserva");
                // objeto das reservas
                var reserva = new
                {
                   
                    NomeCliente = nomeCliente,
                    EmailCliente = emailCliente, 
                    DataReserva = dataReserva.ToString("yyyy-MM-dd"), 
                    HoraReserva = horaReserva.ToString(@"hh\:mm"),    
                    NumeroPessoas = numeroPessoas,
                    Status = "Pendente" // campo do status comeca sempre em Pendente
                };

                Debug.WriteLine("Enviando reserva para o Firestore");
          
                DocumentReference docRef = _firestoreDb.Collection("reservas").Document();
                await docRef.SetAsync(reserva);

                Debug.WriteLine("Reserva salva com sucesso no Firestore");

               
                await DisplayAlert("Sucesso", "Reserva adicionada com sucesso!", "OK");

                // limpar os campos
                Debug.WriteLine("Limpando campos após o sucesso");
                ClienteEntry.Text = string.Empty;
                NumeroPessoasEntry.Text = string.Empty;
                ReservaData = DateTime.Now;
                ReservaHora = new TimeSpan(18, 0, 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao salvar a reserva: {ex.Message}");
                await DisplayAlert("Erro", $"Erro ao salvar a reserva: {ex.Message}", "OK");
            }
        }

    }
}
