namespace Pagina_proyecto.Services
{
    using Google.Apis.Drive.v3;
    using Google.Apis.Services;

    public class GoogleDriveService
    {
        private readonly DriveService _driveService;

        public GoogleDriveService(DriveService driveService)
        {
            _driveService = driveService;
        }

        public async Task<int> ContarArchivosAsync(string linkCarpeta)
        {
            string folderId = ExtraerFolderId(linkCarpeta);

            var request = _driveService.Files.List();
            request.Q = $"'{folderId}' in parents and trashed = false";
            request.Fields = "files(id)";
            request.PageSize = 1000;

            var result = await request.ExecuteAsync();
            return result.Files.Count;
        }

        private string ExtraerFolderId(string link)
        {
            if (string.IsNullOrEmpty(link))
                throw new ArgumentException("El link de Drive es inválido");

            // Caso /folders/{id}
            var foldersIndex = link.IndexOf("/folders/");
            if (foldersIndex != -1)
            {
                return link.Substring(foldersIndex + 9).Split('?', '/')[0];
            }

            // Caso ?id={id}
            var uri = new Uri(link);
            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
            if (query.ContainsKey("id"))
            {
                return query["id"];
            }

            throw new Exception("No se pudo extraer el ID de la carpeta de Drive");
        }

    }
}
