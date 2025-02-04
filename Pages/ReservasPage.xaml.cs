using Burguer_Manager.Utilities;
using Google.Cloud.Firestore;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Burguer_Manager.Pages
{
    public partial class ReservasPage : ContentPage
    {
        public List<string> StatusOptions { get; private set; }
        public List<Reserva> Reservas { get; private set; }
        private readonly FirestoreDb _firestoreDb;

        public ReservasPage()
        {
            InitializeComponent();

            _firestoreDb = FirestoreHelper.InitializeFirestore();
            StatusOptions = new List<string> { "Pendente", "Confirmada", "Completa" };
            Reservas = new List<Reserva>();

            BindingContext = this;

            Task.Run(CarregarReservasAsync);
        }

        private async Task CarregarReservasAsync()
        {
            try
            {
                Reservas.Clear();

                var reservasSnapshot = await _firestoreDb.Collection("reservas").GetSnapshotAsync();
                foreach (var doc in reservasSnapshot.Documents)
                {
                    var data = doc.ToDictionary();
                    Reservas.Add(new Reserva
                    {
                        Id = doc.Id,
                        Cliente = data.GetValueOrDefault("NomeCliente", "Desconhecido").ToString(),
                        HorarioReserva = data.GetValueOrDefault("HoraReserva", "00:00").ToString(),
                        DataReserva = data.GetValueOrDefault("DataReserva", "Sem data").ToString(),
                        NumeroPessoas = Convert.ToInt32(data.GetValueOrDefault("NumeroPessoas", 0)),
                        Status = data.GetValueOrDefault("Status", "Pendente").ToString()
                    });
                }

                OnPropertyChanged(nameof(Reservas));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao carregar reservas: {ex.Message}");
                await DisplayAlert("Erro", "Erro ao carregar as reservas.", "OK");
            }
        }

        private void OnStatusChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && picker.BindingContext is Reserva reserva)
            {
                reserva.Status = picker.SelectedItem?.ToString();
                Debug.WriteLine($"Status alterado para {reserva.Status} (Reserva ID: {reserva.Id})");
            }
        }

        private async void OnSaveChangesClicked(object sender, EventArgs e)
        {
            try
            {
               
                foreach (var reserva in Reservas.Where(r => !string.IsNullOrEmpty(r.Id)))
                {
                    var docRef = _firestoreDb.Collection("reservas").Document(reserva.Id);
                    await docRef.UpdateAsync(new Dictionary<string, object>
                    {
                        { "Status", reserva.Status }
                    });
                }
                await DisplayAlert("Sucesso", "Alterações de status salvas com sucesso.", "OK");
            }

            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao salvar alterações: {ex.Message}");
                await DisplayAlert("Erro", "Erro ao salvar alterações.", "OK");
            }
        }
    }

    public class Reserva
    {
        public string Id { get; set; }
        public string Cliente { get; set; }
        public string HorarioReserva { get; set; }
        public string DataReserva { get; set; }
        public int NumeroPessoas { get; set; }
        public string Status { get; set; }
    }
}
