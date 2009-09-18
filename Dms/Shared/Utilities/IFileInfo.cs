using System;
using System.IO;
using System.Security.AccessControl;

namespace Remotion.Dms.Shared.Utilities
{
  public interface IFileInfo
  {
    string FullName { get; }
    string Extension { get; }
    string Name { get; }
    long Length { get; }
    string DirectoryName { get; }
    DirectoryInfo Directory { get; }
    bool IsReadOnly { get; set; }
    bool Exists { get; }
    DateTime CreationTime { get; set; }
    DateTime LastAccessTime { get; set; }
    DateTime LastWriteTime { get; set; }
    Stream Open (FileMode mode, FileAccess access, FileShare share);
    
    DateTime CreationTimeUtc { get; set; }
    DateTime LastAccessTimeUtc { get; set; }
    DateTime LastWriteTimeUtc { get; set; }
    FileAttributes Attributes { get; set; }
    FileSecurity GetAccessControl ();
    FileSecurity GetAccessControl (AccessControlSections includeSections);
    void SetAccessControl (FileSecurity fileSecurity);
    StreamReader OpenText ();
    StreamWriter CreateText ();
    StreamWriter AppendText ();
    FileInfo CopyTo (string destFileName);
    FileInfo CopyTo (string destFileName, bool overwrite);
    FileStream Create ();
    void Delete ();
    void Decrypt ();
    void Encrypt ();
    Stream Open (FileMode mode);
    Stream Open (FileMode mode, FileAccess access);
    Stream OpenRead ();
    Stream OpenWrite ();
    void MoveTo (string destFileName);
    FileInfo Replace (string destinationFileName, string destinationBackupFileName);
    FileInfo Replace (string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors);
    void Refresh ();
  }
}