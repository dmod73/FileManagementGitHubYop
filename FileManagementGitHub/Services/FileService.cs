using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace FileManagementGitHub.Services
{
    public class FileService
    {
        private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles");

        public FileService()
        {
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        public List<string> GetCategories()
        {
            if (!Directory.Exists(_basePath))
            {
                return new List<string>();
            }

            var directories = Directory.GetDirectories(_basePath);
            var categories = new List<string>();
            foreach (var dir in directories)
            {
                categories.Add(Path.GetFileName(dir));
            }
            return categories;
        }

        public void CreateCategory(string categoryName)
        {
            var categoryPath = Path.Combine(_basePath, categoryName);
            if (!Directory.Exists(categoryPath))
            {
                Directory.CreateDirectory(categoryPath);
            }
        }

        public async Task UploadFileAsync(string category, IBrowserFile file)
        {
            var categoryPath = Path.Combine(_basePath, category);
            if (!Directory.Exists(categoryPath))
            {
                Directory.CreateDirectory(categoryPath);
            }

            var filePath = Path.Combine(categoryPath, file.Name);

            await using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await stream.CopyToAsync(fileStream);

            // Hacer commit y push automáticamente después de subir un archivo
            CommitAndPushChanges($"🆕 Archivo {file.Name} agregado en {category}");
        }


        public List<string> GetFiles(string category)
        {
            var categoryPath = Path.Combine(_basePath, category);
            if (!Directory.Exists(categoryPath))
            {
                return new List<string>();
            }

            var files = Directory.GetFiles(categoryPath);
            var fileNames = new List<string>();
            foreach (var file in files)
            {
                fileNames.Add(Path.GetFileName(file));
            }
            return fileNames;
        }

        public void DeleteFile(string category, string fileName)
        {
            var filePath = Path.Combine(_basePath, category, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        // Hacer pública la función para que pueda ser llamada desde FileManager.razor
        public void CommitAndPushChanges(string commitMessage)
        {
            try
            {
                RunGitCommand("git add .");
                RunGitCommand($"git commit -m \"{commitMessage}\"");
                RunGitCommand("git push origin main");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al hacer commit y push: {ex.Message}");
            }
        }

        private void RunGitCommand(string command)
        {
            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Git Error: {error}");
                }
                else
                {
                    Console.WriteLine($"Git Output: {output}");
                }
            }
        }
    }
}
