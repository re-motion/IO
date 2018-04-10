using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Remotion.Development.UnitTesting.IO;

namespace Remotion.IO.Archive.Zip.UnitTests
{
  [TestFixture]
  public class IntegrationTest
  {
     [Test]
    public void BuildArchive_ExtractArchive_WithMultipleRoots ()
    {
      //root1
      //-file1
      //-Directory1
      //--file2
      //root2
      //-file3
      //-Directory2
      //--Directory3 ü
      //---file4
      //---file5
      //--file6
      //root3
      //-file7

      var file1 = new TempFile();
      var file2 = new TempFile();
      var file3 = new TempFile();
      var file4 = new TempFile();
      var file5 = new TempFile();
      var file6 = new TempFile();
      var file7 = new TempFile();

      var bytes = new byte[8191];
      for (int i = 0; i < 8191; i++)
        bytes[i] = (byte) i;

      file1.WriteAllBytes (bytes);
      file2.WriteAllBytes (bytes);
      file3.WriteAllBytes (bytes);
      file4.WriteAllBytes (bytes);
      file5.WriteAllBytes (bytes);
      file6.WriteAllBytes (bytes);

      var commonRootPath = Path.Combine (Path.GetTempPath(), Guid.NewGuid().ToString());

      var root1 = Directory.CreateDirectory (Path.Combine (commonRootPath, "root1"));
      var root2 = Directory.CreateDirectory (Path.Combine (commonRootPath, "root2"));
      var root3 = Directory.CreateDirectory (Path.Combine (commonRootPath, "root3"));

      var directory1 = Directory.CreateDirectory (Path.Combine (root1.FullName, "Directory1"));
      var directory2 = Directory.CreateDirectory (Path.Combine (root2.FullName, "Directory2"));
      var directory3 = Directory.CreateDirectory (Path.Combine (directory2.FullName, "Directory3 ü"));

      var file1NewLocation = Path.Combine (root1.FullName, Path.GetFileName (file1.FileName));
      var file2NewLocation = Path.Combine (directory1.FullName, Path.GetFileName (file2.FileName));
      var file3NewLocation = Path.Combine (root2.FullName, Path.GetFileName (file3.FileName));
      var file4NewLocation = Path.Combine (directory3.FullName, Path.GetFileName (file4.FileName));
      var file5NewLocation = Path.Combine (directory3.FullName, Path.GetFileName (file5.FileName));
      var file6NewLocation = Path.Combine (directory2.FullName, Path.GetFileName (file6.FileName));
      var file7NewLocation = Path.Combine (root3.FullName, Path.GetFileName (file7.FileName));

      File.Move (file1.FileName, file1NewLocation);
      File.Move (file2.FileName, file2NewLocation);
      File.Move (file3.FileName, file3NewLocation);
      File.Move (file4.FileName, file4NewLocation);
      File.Move (file5.FileName, file5NewLocation);
      File.Move (file6.FileName, file6NewLocation);
      File.Move (file7.FileName, file7NewLocation);

      var zipFileName = Path.GetTempFileName();

      var zipBuilder = new ZipFileBuilder();
      zipBuilder.Progress += (sender, e) => { };
      zipBuilder.AddDirectory (new DirectoryInfoWrapper (directory1));
      zipBuilder.AddDirectory (new DirectoryInfoWrapper (directory2));
      zipBuilder.AddDirectory (new DirectoryInfoWrapper (directory3));
      zipBuilder.AddFile (new FileInfoWrapper(new FileInfo (file1NewLocation)));
      zipBuilder.AddFile (new FileInfoWrapper(new FileInfo (file3NewLocation)));
      zipBuilder.AddFile (new FileInfoWrapper(new FileInfo (file7NewLocation)));

      var expectedRelativePaths = new List<string>
                                  {
                                      file1NewLocation.Substring (root1.FullName.Length + 1),
                                      file2NewLocation.Substring (root2.FullName.Length + 1),
                                      file3NewLocation.Substring (root2.FullName.Length + 1),
                                      file4NewLocation.Substring (root2.FullName.Length + 1),
                                      file5NewLocation.Substring (root2.FullName.Length + 1),
                                      file6NewLocation.Substring (root2.FullName.Length + 1),
                                      file7NewLocation.Substring (root3.FullName.Length + 1)
                                  };

      using (var zipStream = zipBuilder.Build (zipFileName))
      {
        using (var zipExtractor = new ZipFileExtractor (zipStream))
        {
          AssertZipFileEntriesHaveFileNamesTransformed (zipExtractor.GetFiles(), expectedRelativePaths);
          Directory.Delete (commonRootPath, true);
        } 
      }
    }

    private void AssertZipFileEntriesHaveFileNamesTransformed (IEnumerable<IFileInfo> files, ICollection<string> expectedFiles)
    {
      foreach (var file in files)
        expectedFiles.Remove (file.FullName);

      Assert.That (expectedFiles, Is.Empty);
    }
  }
}
