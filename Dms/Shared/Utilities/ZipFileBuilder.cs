// This file is part of re-vision (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Remotion.Dms.Shared.Utilities
{
  /// <summary>
  /// Implements <see cref="IArchiveBuilder"/>. 
  /// At first all files of a directory are added to the generated zip file, then the directory itself is added.
  /// </summary>
  public class ZipFileBuilder : IArchiveBuilder
  {
    private readonly List<IFileSystemEntry> _files = new List<IFileSystemEntry>();
    private bool _onErrorRetry;
    public event EventHandler<StreamCopyProgressEventArgs> ArchiveProgress;
    public event EventHandler<FileOpenExceptionEventArgs> ArchiveError;

    private FileProcessingRecoveryAction _fileProcessingRecoveryAction;

    public ZipFileBuilder ()
    {
    }

    public FileProcessingRecoveryAction FileProcessingRecoveryAction
    {
      get { return _fileProcessingRecoveryAction; }
      set { _fileProcessingRecoveryAction = value; }
    }

    public void AddDirectory (IDirectoryInfo directoryInfo)
    {
      ArgumentUtility.CheckNotNull ("directoryInfo", directoryInfo);
      _files.Add (directoryInfo);
    }

    public void AddFile (IFileInfo fileInfo)
    {
      ArgumentUtility.CheckNotNull ("fileInfo", fileInfo);
      _files.Add (fileInfo);
    }

    public Stream Build (string archiveFileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("archiveFileName", archiveFileName);

      using (var zipOutputStream = new ZipOutputStream (File.Create (archiveFileName)))
      {
        StreamCopier streamCopier = new StreamCopier();
        foreach (var fileInfo in _files)
        {
          if (fileInfo is IFileInfo)
            AddFileToZipFile ((IFileInfo) fileInfo, fileInfo.Name, zipOutputStream, streamCopier);
          else if (fileInfo is IDirectoryInfo)
            AddDirectoryToZipFile ((DirectoryInfoWrapper) fileInfo, zipOutputStream, streamCopier, string.Empty);
        }
      }
      _files.Clear();
      return File.Open (archiveFileName, FileMode.Open, FileAccess.Read, FileShare.None);
    }

    private void AddFileToZipFile (IFileInfo fileInfo, string path, ZipOutputStream zipOutputStream, StreamCopier streamCopier)
    {
      Stream fileStream = null;
      do
      {
        try
        {
          _onErrorRetry = false;
          fileStream = fileInfo.Open (FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        catch (FileNotFoundException ex)
        {
          OnFileOpenError (this, new FileOpenExceptionEventArgs (fileInfo.FullName, ex));
        }
        catch (DirectoryNotFoundException ex)
        {
          OnFileOpenError (this, new FileOpenExceptionEventArgs (fileInfo.FullName, ex));
        }
        catch (IOException ex)
        {
          OnFileOpenError (this, new FileOpenExceptionEventArgs (fileInfo.FullName, ex));
        }
        catch (UnauthorizedAccessException ex)
        {
          OnFileOpenError (this, new FileOpenExceptionEventArgs (fileInfo.FullName, ex));
        }
      } while (_onErrorRetry);

      if (fileStream != null)
      {
        ZipEntry zipEntry = new ZipEntry (path);
        zipOutputStream.PutNextEntry (zipEntry);
        streamCopier.TransferProgress += OnZippingProgress;
        streamCopier.CopyStream (fileStream, zipOutputStream, fileStream.Length);
        fileStream.Dispose();
      }
    }

    private void AddDirectoryToZipFile (
        DirectoryInfoWrapper directoryInfo,
        ZipOutputStream zipOutputStream,
        StreamCopier streamCopier,
        string parentDirectory)
    {
      parentDirectory = Path.Combine (parentDirectory, directoryInfo.Name);
      foreach (var file in directoryInfo.GetFiles())
        AddFileToZipFile ((FileInfoWrapper) file, Path.Combine (parentDirectory, file.Name), zipOutputStream, streamCopier);
      foreach (var directory in directoryInfo.GetDirectories())
        AddDirectoryToZipFile ((DirectoryInfoWrapper) directory, zipOutputStream, streamCopier, parentDirectory);
    }

    private void OnZippingProgress (object sender, StreamCopyProgressEventArgs args)
    {
      if (ArchiveProgress != null)
        ArchiveProgress (this, args);
    }

    private void OnFileOpenError (object sender, FileOpenExceptionEventArgs args)
    {
      if (ArchiveError != null)
        ArchiveError (this, args);
      else
        throw args.Exception;

      if (_fileProcessingRecoveryAction == FileProcessingRecoveryAction.Abort)
        throw new AbortException();
      if (_fileProcessingRecoveryAction == FileProcessingRecoveryAction.Ignore)
        return;
      if (_fileProcessingRecoveryAction == FileProcessingRecoveryAction.Retry)
        _onErrorRetry = true;
    }
  }
}