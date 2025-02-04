using Android.OS;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Android.App;

namespace Burguer_Manager
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Não estamos mais alterando a cor da barra de status.
            // A barra de status usará o padrão do sistema.
        }
    }
}
