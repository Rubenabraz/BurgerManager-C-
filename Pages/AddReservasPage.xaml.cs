using Burguer_Manager.Utilities;
using Google.Cloud.Firestore;
using Microsoft.Maui.Controls;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Burguer_Manager.Pages
{
    public partial class AddReservasPage : ContentPage
    {
       
        public DateTime ReservaData { get; set; }
        public TimeSpan ReservaHora { get; set; }
        private readonly FirestoreDb _firestoreDb;

        public AddReservasPage()
        {
            InitializeComponent();

            Debug.WriteLine("Inicializando a p�gina AdmAddReserva");

            
            ReservaData = DateTime.Now;
            ReservaHora = new TimeSpan(18, 0, 0); 

            
            BindingContext = this;

            Debug.WriteLine("Inicializando Firestore");
            
            _firestoreDb = FirestoreHelper.InitializeFirestore();

            // configura o DatePicker para permitir apenas datas futuras
            Debug.WriteLine("Configurando DatePicker: Data m�nima e m�xima");
            DataReservaPicker.MinimumDate = DateTime.Now;
            DataReservaPicker.MaximumDate = DateTime.Now.AddDays(2); // max 2 dias a frente
        }

        private async void OnSalvarReservaClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Bot�o 'Adicionar Reserva' clicado");

            // recolher valores dos inputs do ui
            var nomeCliente = ClienteEntry?.Text;
            var dataReserva = ReservaData;
            var horaReserva = ReservaHora;
            var numeroPessoasText = NumeroPessoasEntry?.Text;

            Debug.WriteLine($"Valores capturados: NomeCliente={nomeCliente}, DataReserva={dataReserva}, HoraReserva={horaReserva}, NumeroPessoas={numeroPessoasText}");

            // validacao dos inputs
            if (string.IsNullOrWhiteSpace(nomeCliente))
            {
                Debug.WriteLine("Valida��o falhou: Nome do cliente est� vazio");
                await DisplayAlert("Erro", "O nome do cliente � obrigat�rio.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(numeroPessoasText) || !int.TryParse(numeroPessoasText, out int numeroPessoas) || numeroPessoas <= 0)
            {
                Debug.WriteLine("Valida��o falhou: N�mero de pessoas inv�lido");
                await DisplayAlert("Erro", "N�mero de pessoas inv�lido.", "OK");
                return;
            }

            try
            {
                Debug.WriteLine("Criando objeto de reserva");
                // criacao do objeto da reserva
                var reserva = new
                {
                    NomeCliente = nomeCliente,
                    DataReserva = dataReserva.ToString("yyyy-MM-dd"), 
                    HoraReserva = horaReserva.ToString(@"hh\:mm"),    
                    NumeroPessoas = numeroPessoas
                };

                Debug.WriteLine("Enviando reserva para o Firestore");
                // enviar reserva para o Firestore
                DocumentReference docRef = _firestoreDb.Collection("reservas").Document();
                await docRef.SetAsync(reserva);

                Debug.WriteLine("Reserva salva com sucesso no Firestore");

                
                await DisplayAlert("Sucesso", "Reserva adicionada com sucesso!", "OK");

                // limpar campos
                Debug.WriteLine("Limpando campos ap�s o sucesso");
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
