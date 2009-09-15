using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Remotion.Dms.Shared.Globalization;

namespace Remotion.Dms.Shared.Utilities
{
  /// <summary>
  /// Implements <see cref="IFileSystemHelper"/>
  /// </summary>
  [Serializable]
  public class FileSystemHelper : IFileSystemHelper
  {
    private const int c_maxPathLength = 259;

    public FileSystemHelper ()
    {
    }

    public FileStream OpenFile (string path, FileMode mode, FileAccess access, FileShare share)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);
      return File.Open (path, mode, access, share);
    }

    public void CopyFile (string sourceFileName, string destFileName, bool overwrite)
    {
      ArgumentUtility.CheckNotNull ("sourceFileName", sourceFileName);
      ArgumentUtility.CheckNotNull ("destFileName", destFileName);

      File.Copy (sourceFileName, destFileName, overwrite);

      while (!File.Exists (destFileName))
        Thread.Sleep (10);
    }

    public bool FileExists (string path)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);
      return File.Exists (path);
    }

    public bool DirectoryExists (string path)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);
      return Directory.Exists (path);
    }

    public string GetTempFileName ()
    {
      return Path.GetTempFileName();
    }

    public DateTime GetLastWriteTime (string path)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);
      return File.GetLastWriteTime (path);
    }

    public void Delete (string path)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);
      File.Delete (path);

      while (File.Exists (path))
        Thread.Sleep (10);
    }

    public void Move (string sourceFile, string destinationFile)
    {
      ArgumentUtility.CheckNotNull ("sourceFile", sourceFile);
      ArgumentUtility.CheckNotNull ("destinationFile", destinationFile);

      File.Move (sourceFile, destinationFile);

      while (File.Exists (sourceFile) || !File.Exists (destinationFile))
        Thread.Sleep (10);
    }

    public string MakeUniqueAndValidFileName (string path, string proposedFileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);
      ArgumentUtility.CheckNotNullOrEmpty ("proposedFileName", proposedFileName);

      if (!path.EndsWith (@"\"))
        path = path + @"\";

      var extensionStartIndex = proposedFileName.LastIndexOf ('.');
      var extension = extensionStartIndex == -1 ? string.Empty : proposedFileName.Substring (extensionStartIndex);
      var fileName = extensionStartIndex == -1 ? proposedFileName : proposedFileName.Substring (0, extensionStartIndex);

      int remainingFileNameLength = c_maxPathLength - path.Length - extension.Length;
      if (remainingFileNameLength < 0)
        throw new ArgumentException (FileSystemHelperResource.FileNameLengthErrorMessage);

      fileName = RemoveInvalidChars (fileName);
      return AppendUniqueNumber (path, extension, fileName, remainingFileNameLength);
    }

    private string RemoveInvalidChars (string fileName)
    {
      var builder = new StringBuilder (fileName);
      foreach (var invalidFileNameChar in Path.GetInvalidFileNameChars())
        builder.Replace (invalidFileNameChar.ToString(), "");
      fileName = builder.ToString();
      return fileName;
    }

    private string AppendUniqueNumber (string path, string extension, string fileName, int remainingFileNameLength)
    {
      int fileIndex = 0;
      var resultingFileName = path + fileName.Substring (0, Math.Min (fileName.Length, remainingFileNameLength)) + extension;
      while (File.Exists (resultingFileName))
      {
        fileIndex++;
        var fileIndexString = string.Format (" ({0})", fileIndex);
        resultingFileName =
            path +
            fileName.Substring (0, Math.Min (fileName.Length, remainingFileNameLength - fileIndexString.Length)) +
            fileIndexString +
            extension;
      }
      return resultingFileName;
    }

    public IFileInfo[] GetFilesOfDirectory (string path)
    {
      DirectoryInfo directoryInfo = new DirectoryInfo (path);
      List<IFileInfo> fileInfos = new List<IFileInfo>();
      foreach (var fileInfo in directoryInfo.GetFiles())
        fileInfos.Add (new FileInfoWrapper (fileInfo));
      return fileInfos.ToArray();
    }

    public void DeleteDirectory (string path, bool recursive)
    {
      Directory.Delete (path, recursive);
    }
  }
}