using Google.Cloud.Firestore;
using Microsoft.Maui.Controls;
using Burguer_Manager.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Burguer_Manager.Pages
{
    public partial class UserReservas : ContentPage
    {
        private readonly FirestoreDb _firestoreDb;
        private ObservableCollection<Reservas> _reservas;

        public UserReservas()
        {
            InitializeComponent();
            _firestoreDb = FirestoreHelper.InitializeFirestore();
            _reservas = new ObservableCollection<Reservas>();

            ReservasCollectionView.ItemsSource = _reservas;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CarregarReservasAsync();
        }

        // funcao assincrona para carregar as reservas
        // TCA: adicionei logs para testar
        private async Task CarregarReservasAsync()
        {
            try
            {
                LoadingLabel.IsVisible = true; // label de loading

                var usuarioLogado = App.UsuarioAtual;
                if (usuarioLogado == null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert("Erro", "Usuário não autenticado.", "OK");
                    });
                    return;
                }

                Console.WriteLine($"Email do usuário logado: {usuarioLogado.Info.Email}");

                // executa a consulta no Firestore e tem a verificacao da thread para nao atrapalhar o UI
                await Task.Run(async () =>
                {
                    var reservasQuery = _firestoreDb.Collection("reservas")
                        .WhereEqualTo("EmailCliente", usuarioLogado.Info.Email);

                    var querySnapshot = await reservasQuery.GetSnapshotAsync();
                    Console.WriteLine($"Documentos encontrados: {querySnapshot.Documents.Count}");

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        _reservas.Clear();
                    });

                    foreach (var document in querySnapshot.Documents)
                    {
                        Console.WriteLine($"Documento ID: {document.Id}");
                        foreach (var kvp in document.ToDictionary())
                        {
                            Console.WriteLine($"Campo: {kvp.Key}, Valor: {kvp.Value}");
                        }

                        // Realiza a conversão manual
                        var nomeCliente = document.GetValue<string>("NomeCliente");
                        var dataReserva = document.GetValue<string>("DataReserva");
                        var horaReserva = document.GetValue<string>("HoraReserva");
                        var numeroPessoas = document.GetValue<int>("NumeroPessoas");
                        var emailCliente = document.GetValue<string>("EmailCliente");

                        var reserva = new Reservas(nomeCliente, dataReserva, horaReserva, numeroPessoas, emailCliente);

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            _reservas.Add(reserva);
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Erro", $"Erro ao carregar reservas: {ex.Message}", "OK");
                });
            }
            finally
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    LoadingLabel.IsVisible = false;
                });
            }
        }
   
    }

    // RA: a funcionar era a classe tava a ser mal chamada

    public class Reservas
    {
        // construtor da classe reservas
        public Reservas(string nomeCliente, string dataReserva, string horaReserva, int numeroPessoas, string emailCliente)
        {
            NomeCliente = nomeCliente;
            DataReserva = "Data: " + dataReserva;
            HoraReserva = "Hora: " + horaReserva;
            NumeroPessoas = numeroPessoas;
            EmailCliente = emailCliente;
            NumeroPessoasTexto = "Quantidade de Pessoas: " + numeroPessoas;
        }

        // propriadades da classe reversas
        public string NomeCliente { get; set; }
        public string DataReserva { get; set; }
        public string HoraReserva { get; set; }
        public int NumeroPessoas { get; set; }
        public string EmailCliente { get; set; }
        public string NumeroPessoasTexto { get; }
    }



}
