// This file is part of the re-motion Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// re-motion is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Remotion.Utilities;

namespace Remotion.IO.Zip
{
  /// <summary>
  /// Implements <see cref="IArchiveBuilder"/>. 
  /// At first all files of a directory are added to the generated zip file, then the directory itself is added.
  /// </summary>
  public class ZipFileBuilder : IArchiveBuilder
  {
    private readonly List<IFileSystemEntry> _files = new List<IFileSystemEntry>();
    public event EventHandler<ArchiveBuilderProgressEventArgs> Progress;
    public event EventHandler<FileOpenExceptionEventArgs> Error;

    private FileProcessingRecoveryAction _fileProcessingRecoveryAction;
    private int _currentFileIndex = 0;
    private long _currentTotalValueExcludingCurrentFileValue = 0;
    private int _currentEstimatedFileCount = 0;
    private readonly FileShare _fileShareToUse;

    /// <summary>
    /// Creates an instance of <see cref="ZipFileBuilder"/>
    /// </summary>
    /// <param name="additionalFileShareToUse">The <see cref="FileShare"/> that should be used to read the files in addition to <see cref="FileShare.Read"/>.
    /// (That means <see cref="FileShare.Read"/> is used at least in any case, regardless if it is specified or not)
    /// </param>
    public ZipFileBuilder (FileShare additionalFileShareToUse = FileShare.None)
    {
      _fileShareToUse = additionalFileShareToUse | FileShare.Read;
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
      _currentEstimatedFileCount++;
    }

    public Stream Build (string archiveFileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("archiveFileName", archiveFileName);

      using (var zipOutputStream = new ZipOutputStream (File.Create (archiveFileName)))
      {
        foreach (var fileInfo in _files)
        {
          var directoryName = fileInfo.Directory == null ? string.Empty : fileInfo.Directory.FullName;
          var nameTransform = new ZipNameTransform (directoryName);

          if (fileInfo is IFileInfo)
            AddFileToZipFile ((IFileInfo) fileInfo, zipOutputStream, nameTransform);
          else if (fileInfo is IDirectoryInfo)
            AddDirectoryToZipFile ((IDirectoryInfo) fileInfo, zipOutputStream, nameTransform);
        }
      }

      _files.Clear();
      _currentFileIndex = 0;
      _currentTotalValueExcludingCurrentFileValue = 0;
      _currentEstimatedFileCount = 0;

      return File.Open (archiveFileName, FileMode.Open, FileAccess.Read, FileShare.None);
    }

    private void AddFileToZipFile (IFileInfo fileInfo, ZipOutputStream zipOutputStream, ZipNameTransform nameTransform)
    {
      var fileStream = GetFileStream (fileInfo);
      var fileLength = fileInfo.Length;

      if (fileStream == null)
        return;

      var zipEntry = CreateZipFileEntry (fileInfo, nameTransform);
      zipOutputStream.PutNextEntry (zipEntry);

      using (fileStream)
      {

        var streamCopier = new StreamCopier();
        streamCopier.TransferProgress += (sender, args) => OnZippingProgress (args, fileInfo.FullName, fileInfo.Length);

        if (!streamCopier.CopyStream (fileStream, zipOutputStream))
        {
          zipEntry.Size = -1;
          throw new AbortException();
        }
      }

      _currentFileIndex++;
      _currentTotalValueExcludingCurrentFileValue += fileLength;
    }

    private void AddDirectoryToZipFile (IDirectoryInfo directoryInfo, ZipOutputStream zipOutputStream, ZipNameTransform nameTransform)
    {
      var files = directoryInfo.GetFiles ();
      _currentEstimatedFileCount += files.Length;

      foreach (var file in files)
        AddFileToZipFile (file, zipOutputStream, nameTransform);
      foreach (var directory in directoryInfo.GetDirectories ())
        AddDirectoryToZipFile (directory, zipOutputStream, nameTransform);
    }

    private Stream GetFileStream (IFileInfo fileInfo)
    {
      bool onErrorRetry;
      do
      {
        try
        {
          return fileInfo.Open (FileMode.Open, FileAccess.Read, _fileShareToUse);
        }
        catch (FileNotFoundException ex)
        {
          onErrorRetry = OnFileOpenError (this, new FileOpenExceptionEventArgs (fileInfo.FullName, ex));
        }
        catch (DirectoryNotFoundException ex)
        {
          onErrorRetry = OnFileOpenError (this, new FileOpenExceptionEventArgs (fileInfo.FullName, ex));
        }
        catch (IOException ex)
        {
          onErrorRetry = OnFileOpenError (this, new FileOpenExceptionEventArgs (fileInfo.FullName, ex));
        }
        catch (UnauthorizedAccessException ex)
        {
          onErrorRetry = OnFileOpenError (this, new FileOpenExceptionEventArgs (fileInfo.FullName, ex));
        }
      } while (onErrorRetry);
      
      return null;
    }

    private ZipEntry CreateZipFileEntry (IFileInfo fileInfo, INameTransform nameTransform)
    {
      return new ZipEntry (nameTransform.TransformFile (fileInfo.FullName))
             {
                 IsUnicodeText = true,
                 DateTime = fileInfo.LastWriteTimeUtc
             };
    }

    private void OnZippingProgress (StreamCopyProgressEventArgs currentFileProgress, string currentFilePath, long currentEstimatedFileSize)
    {
      if (Progress != null)
      {
        var currentFileValue = currentFileProgress.CurrentValue;
        var currentTotalValue = _currentTotalValueExcludingCurrentFileValue + currentFileValue;
        var archiveBuilderProgressEventArgs = new ArchiveBuilderProgressEventArgs (
            currentFileValue,
            currentTotalValue,
            _currentFileIndex,
            currentFilePath,
            currentEstimatedFileSize,
            _currentEstimatedFileCount);

        archiveBuilderProgressEventArgs.Cancel = currentFileProgress.Cancel;
        Progress (this, archiveBuilderProgressEventArgs);
        currentFileProgress.Cancel = archiveBuilderProgressEventArgs.Cancel;
      }
    }

    private bool OnFileOpenError (object sender, FileOpenExceptionEventArgs args)
    {
      if (Error != null)
        Error (this, args);
      else
        throw args.Exception;

      if (_fileProcessingRecoveryAction == FileProcessingRecoveryAction.Abort)
        throw new AbortException();
      if (_fileProcessingRecoveryAction == FileProcessingRecoveryAction.Ignore)
        return false;
      if (_fileProcessingRecoveryAction == FileProcessingRecoveryAction.Retry)
        return true;
      return false;
    }
  }
}