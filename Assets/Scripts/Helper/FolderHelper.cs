using System.Collections;
using System.Collections.Generic;
using System.IO;
/// <summary>
/// �ļ��в�����
/// </summary>
public static class FolderHelper
{
    /// <summary>
    /// �����ļ���
    /// </summary>
    /// <param name="sourceFolderName">Դ�ļ���Ŀ¼</param>
    /// <param name="destFolderName">Ŀ���ļ���Ŀ¼</param>
    /// <param name="overwrite">�������ļ�</param>
    public static void Copy(string sourceFolderName, string destFolderName, bool overwrite)
    {
        var sourceFilesPath = Directory.GetFileSystemEntries(sourceFolderName);

        for (int i = 0; i < sourceFilesPath.Length; i++)
        {
            var sourceFilePath = sourceFilesPath[i];
            var directoryName = Path.GetDirectoryName(sourceFilePath);
            var forlders = directoryName.Split('\\');
            var lastDirectory = forlders[forlders.Length - 1];
            var dest = Path.Combine(destFolderName, lastDirectory);

            if (File.Exists(sourceFilePath))
            {
                var sourceFileName = Path.GetFileName(sourceFilePath);
                if (!Directory.Exists(dest))
                {
                    Directory.CreateDirectory(dest);
                }
                File.Copy(sourceFilePath, Path.Combine(dest, sourceFileName), overwrite);
            }
            else
            {
                Copy(sourceFilePath, dest, overwrite);
            }
        }
    }
    public static void ClearDictory(string path,bool recursive)
    {
        Directory.Delete(path, recursive);
    }
}
