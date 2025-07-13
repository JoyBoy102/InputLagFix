using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace INPUTLAGFIX.Models
{
    public class CleanFilesModel
    {
        private XmlManager _xmlManager;
        public CleanFilesModel()
        {
            _xmlManager = new XmlManager();
        }
        public void CleanFolder(string folder)
        {
            folder = Environment.ExpandEnvironmentVariables(folder);
            try
            {
                // Проверка существования папки
                if (!Directory.Exists(folder))
                {
                    Logger.GetLogger().AllLogMessages.Add($"Папка не существует: {folder}");
                    return;
                }

                // Безопасное удаление файлов
                foreach (var file in Directory.GetFiles(folder))
                {
                    try
                    {
                        File.Delete(file);
                        Logger.GetLogger().AllLogMessages.Add($"Удалён файл: {file}");
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Logger.GetLogger().AllLogMessages.Add($"Нет прав на удаление: {file}");
                    }
                    catch (IOException ex)
                    {
                        Logger.GetLogger().AllLogMessages.Add($"Ошибка при удалении {file}: {ex.Message}");
                    }
                }

                // Рекурсивная очистка подпапок (по желанию)
                foreach (var subDir in Directory.GetDirectories(folder))
                {
                    try
                    {
                        Directory.Delete(subDir, true);
                        Logger.GetLogger().AllLogMessages.Add($"Удалена папка: {subDir}");
                    }
                    catch (Exception ex)
                    {
                        Logger.GetLogger().AllLogMessages.Add($"Ошибка при удалении {subDir}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GetLogger().AllLogMessages.Add($"Критическая ошибка: {ex.Message}");
            }
        }

        public void SetCategoryFoldersSize(CleaningCategoryItem item)
        {
            long totalSize = 0;

            foreach (var folder in item.Folders)
            {
                var folderCorrectPath = Environment.ExpandEnvironmentVariables(folder);
                if (Directory.Exists(folderCorrectPath))
                {
                    try
                    {
                        // Получаем все файлы в папке и подпапках
                        var files = Directory.GetFiles(folderCorrectPath, "*.*", SearchOption.AllDirectories);

                        foreach (var file in files)
                        {
                            try
                            {
                                var fileInfo = new FileInfo(file);
                                totalSize += fileInfo.Length;
                            }
                            catch (Exception ex)
                            {
                                // Пропускаем файлы, к которым нет доступа
                                Debug.WriteLine($"Ошибка доступа к файлу {file}: {ex.Message}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Пропускаем папки, к которым нет доступа
                        Debug.WriteLine($"Ошибка доступа к папке {folderCorrectPath}: {ex.Message}");
                    }
                }
            }

            // Конвертируем байты в мегабайты (1 MB = 1024*1024 bytes)
            double sizeInMB = totalSize / (1024.0 * 1024.0);

            // Записываем результат в AdditionalInfo с округлением до 2 знаков
            item.AdditionalInfo = $"{sizeInMB:0.00} MB";
        }

        public ObservableCollection<CleaningCategoryItem> GetAllCleaningCategoryItems()
        {
            string currentUsername = Environment.UserName;
            List<CleaningCategoryItem> AllCleaninCategoryItems = _xmlManager.GetAllCleaningCategoryItems();
            foreach (var item in AllCleaninCategoryItems)
            {
                SetCategoryFoldersSize(item);
            }

            ObservableCollection<CleaningCategoryItem> parsedCleaningCategoryItems =  new ObservableCollection<CleaningCategoryItem>(AllCleaninCategoryItems);
            return parsedCleaningCategoryItems;
        }
    }
   
}
