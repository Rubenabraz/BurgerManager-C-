namespace Burguer_Manager
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                // log detalhado da excecao em caso de erro a iniciar a aplicacao
                Console.WriteLine($"Exception: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }


    }
}
