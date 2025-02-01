using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagementApp.Services
{
    public class FileService
    {
        private readonly string _basePath = "wwwroot/files";  // Carpeta donde se guardarán los archivos

        public FileService()
        {
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        // 🔹 Obtener todas las categorías (carpetas)
        public List<string> GetCategories()
        {
            return Directory.GetDirectories(_basePath).Select(Path.GetFileName).ToList();
        }

        // 🔹 Crear una nueva categoría
        public void CreateCategory(string category)
        {
            string categoryPath = Path.Combine(_basePath, category);
            if (!Directory.Exists(categoryPath))
            {
                Directory.CreateDirectory(categoryPath);
            }
        }

        // 🔹 Subir un archivo a una categoría
        public async Task UploadFileAsync(string category, Stream fileStream, string fileName)
        {
            string categoryPath = Path.Combine(_basePath, category);
            if (!Directory.Exists(categoryPath))
            {
                Directory.CreateDirectory(categoryPath);
            }

            string filePath = Path.Combine(categoryPath, fileName);
            using (var fileStreamOutput = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileStreamOutput);
            }
        }

        // 🔹 Obtener archivos dentro de una categoría
        public List<string> GetFiles(string category)
        {
            string categoryPath = Path.Combine(_basePath, category);
            if (!Directory.Exists(categoryPath)) return new List<string>();

            return Directory.GetFiles(categoryPath).Select(Path.GetFileName).ToList();
        }

        // 🔹 Descargar archivo
        public string GetFilePath(string category, string fileName)
        {
            return Path.Combine(_basePath, category, fileName);
        }

        // 🔹 Eliminar archivo
        public void DeleteFile(string category, string fileName)
        {
            string filePath = GetFilePath(category, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}

