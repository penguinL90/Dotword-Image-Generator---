using Microsoft.Win32;

namespace dot_picture_generator.Class
{
    static class OpenFile
    {
        public static string Open_File()
        {
            OpenFileDialog ofd = new()
            {
                Filter = "Image File|*.png;*.jpg;*.jpeg;*.heic",
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
            };
            if (ofd.ShowDialog() == true)
            {
                return ofd.FileName;
            }
            return string.Empty;
        }

        public static string Open_Folder()
        {
            OpenFolderDialog ofd = new()
            {
                AddToRecent = true,
                Multiselect = false,
            };
            if ( ofd.ShowDialog() == true )
            {
                return ofd.FolderName;
            }
            return string.Empty;
        }
    }
}
