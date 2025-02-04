using Firebase.Auth;

namespace Burguer_Manager
{
    public partial class App : Application
    {
        // propriadade para armazenar o utilizador atual
        public static User UsuarioAtual { get; private set; }

        public App()
        {
            InitializeComponent();

            // config da pagina principal
            MainPage = new NavigationPage(new HomePage());
        }

        // metodo para definir o utilizador atual
        public static void SetUsuarioAtual(User user)
        {
            UsuarioAtual = user;
        }
    }
}
