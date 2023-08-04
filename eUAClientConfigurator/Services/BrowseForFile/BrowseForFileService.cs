// Copyright PHOENIX CONTACT Electronics GmbH

using Arp.OpcUA.Core;
using Microsoft.Win32;
using System.IO;
using System.Threading.Tasks;

namespace eUAClientConfigurator.Services.BrowseForFile
{
    class BrowseForFileService : IBrowseForFileService
    {
        public Task<Stream?> OpenFileForReadAsync(string title, string filter, FileMode fileMode)
        {
            // for the open file dialog 
            System.Threading.Thread.CurrentThread.SetApartmentState(System.Threading.ApartmentState.STA);

            var dialog = new OpenFileDialog();
            dialog.Title = title;
            dialog.Filter = filter;
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == true)
            {
                return Task.FromResult<Stream?>(File.Open(dialog.FileName, fileMode));
            }
            return Task.FromResult<Stream?>(null);
        }

        public Task<Stream?> OpenFileForWriteAsync(string title, string filter, FileMode fileMode)
        {
            // for the save file dialog 
            System.Threading.Thread.CurrentThread.SetApartmentState(System.Threading.ApartmentState.STA);

            var dialog = new SaveFileDialog();
            dialog.Title = title;
            dialog.Filter = filter;
            if (dialog.ShowDialog() == true)
            {
                return Task.FromResult<Stream?>(File.Open(dialog.FileName, fileMode));
            }
            return Task.FromResult<Stream?>(null);
        }
    }
}
