using System;
using Microsoft.WindowsAzure.MobileServices.Files;

namespace SnapAndSave
{
	public interface IFileHelper
	{
		string FilePath { get; }
		string CopyFileToAppDirectory (string itemId, string filePath);
		string GetLocalFilePath (string itemId, string fileName);
		void DeleteLocalFile (MobileServiceFile file);
	}
}

