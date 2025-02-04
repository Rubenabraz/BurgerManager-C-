using Google.Cloud.Firestore;
using System;
using System.IO;
using System.Reflection;

namespace Burguer_Manager.Utilities
{
    public static class FirestoreHelper
    {
        private static FirestoreDb _firestoreDb;

        public static FirestoreDb InitializeFirestore()
        {
            // verifica se o firesotre esta iniciado
            if (_firestoreDb != null)
            {
                Console.WriteLine("Firestore já foi iniciado.");
                return _firestoreDb;
            }

            // caminho para o arquivo das credenciais
            string pathToCredentials = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "burgermanager-3b131-firebase-adminsdk-f8uuc-e4cbfe62f3.json");

            try
            {
                // copia a recurso para o arquivo local
                CopyEmbeddedResourceToLocalFile("Burguer_Manager.Resources.burgermanager-3b131-firebase-adminsdk-f8uuc-e4cbfe62f3.json", pathToCredentials);

                // verifica se o arquivo foi copiado ok
                if (File.Exists(pathToCredentials))
                {
                    Console.WriteLine($"Arquivo de credenciais encontrado em: {pathToCredentials}");
                }
                else
                {
                    Console.WriteLine($"Erro: O arquivo de credenciais não foi extraído corretamente.");
                    return null;
                }

                // configura a variavel de ambiente com o caminho do arquivo das credenciais
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToCredentials);

                // inicia o Firestore
                _firestoreDb = FirestoreDb.Create("burgermanager-3b131");

                Console.WriteLine("Firestore inicializado com sucesso.");
                return _firestoreDb;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar o Firestore: {ex.Message}");
                return null;
            }
        }

        private static void CopyEmbeddedResourceToLocalFile(string resourceName, string outputPath)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    if (stream == null)
                    {
                        Console.WriteLine($"Erro: Não foi possível localizar o recurso embutido: {resourceName}");
                        return;
                    }

                    // copia o recurso para o arquivo local
                    stream.CopyTo(fileStream);
                    Console.WriteLine($"Recurso embutido extraído para: {outputPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao copiar o recurso embutido: {ex.Message}");
            }
        }
    }
}
