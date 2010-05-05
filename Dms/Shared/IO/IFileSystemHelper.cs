using System;
using System.IO;

namespace Remotion.Dms.Shared.IO
{
  /// <summary>
  /// Encapsulate FileIO
  /// </summary>
  public interface IFileSystemHelper
  {
    FileStream OpenFile (string path, FileMode mode, FileAccess access, FileShare share);
    bool FileExists (string path);
    bool DirectoryExists (string path);
    string GetTempFileName ();
    string GetTempFolder ();
    DateTime GetLastWriteTime(string path);
    void Delete (string path);
    void Move (string sourceFile, string destinationFile);
    string MakeUniqueAndValidFileName (string path, string proposedFileName);
    IFileInfo[] GetFilesOfDirectory (string path);
    void DeleteDirectory (string path, bool recursive);
    string Combine (string path1, string path2);
    DirectoryInfo CreateDirectory (string path);
    string GetPathWithEnvironmentVariable (string path);
  }
}