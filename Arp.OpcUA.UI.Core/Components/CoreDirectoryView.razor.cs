// Copyright PHOENIX CONTACT Electronics GmbH

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Arp.OpcUA.UI.Core.Components
{
    public partial class CoreDirectoryView
    {
        protected override async Task OnInitializedAsync()
        {
            await ListFiles();
        }

        [Parameter]
        public string BaseFolder { get; set; }
        bool Uploading = false;

        string LargeUploadMessage = "";
        long UploadedBytes;
        long TotalBytes;
        List<string> FileUrls = new List<string>();

        // support for drag/drop
        string dropClass = string.Empty;
        void HandleDragEnter()
        {
            dropClass = "dropAreaDrug";
        }
        public void UpdateUI()
        {
            InvokeAsync(() => StateHasChanged());
        }
        void HandleDragLeave()
        {
            dropClass = string.Empty;
        }

        async Task OnInputFileChange(InputFileChangeEventArgs args)
        {
            UploadedBytes = 0;

            // Disable the file input field
            UpdateUI();

            // calculate the chunks we have to send
            TotalBytes = args.File.Size;
            long percent = 0;
            long chunkSize = 400000; // fairly arbitrary
            long numChunks = TotalBytes / chunkSize;
            long remainder = TotalBytes % chunkSize;

            // get new filename with a bit of entropy
            string justFileName = Path.GetFileNameWithoutExtension(args.File.Name);
            string extension = Path.GetExtension(args.File.Name);
            string newFileNameWithoutPath = $"{justFileName}-{DateTime.Now.Ticks.ToString()}{extension}";
            string filename = Path.Combine(Environment.CurrentDirectory, BaseFolder, newFileNameWithoutPath);

            // Delete the file if it already exists in our \Files folder
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            // Open the input and output file streams
            using (var inStream = args.File.OpenReadStream(long.MaxValue))
            {
                using (var outStream = File.OpenWrite(filename))
                {
                    // Read and Write

                    for (int i = 0; i < numChunks; i++)
                    {
                        // Read the next chunk
                        var buffer = new byte[chunkSize];
                        await inStream.ReadAsync(buffer, 0, buffer.Length);
                        // Write it
                        await outStream.WriteAsync(buffer, 0, buffer.Length);
                        // Update our progress data and UI
                        UploadedBytes += chunkSize;
                        percent = UploadedBytes * 100 / TotalBytes;
                        // Report progress with a string
                        LargeUploadMessage = $"Uploading {args.File.Name} {percent}%";
                        UpdateUI();
                    }

                    if (remainder > 0)
                    {
                        // Same stuff as above, just with the leftover chunk data
                        var buffer = new byte[remainder];
                        await inStream.ReadAsync(buffer, 0, buffer.Length);
                        await outStream.WriteAsync(buffer, 0, buffer.Length);
                        UploadedBytes += remainder;
                        percent = UploadedBytes * 100 / TotalBytes;
                        LargeUploadMessage = $"Uploading {args.File.Name} {percent}%";
                        UpdateUI();
                    }
                }
            }

            LargeUploadMessage = "Upload Complete.";
            await ListFiles();
        }

        Task ListFiles()
        {
            FileUrls.Clear();
            try
            {
                var files = Directory.GetFiles(Path.Combine(BaseFolder), "*.*");
                foreach (var filename in files)
                {
                    var file = Path.GetFileName(filename);
                    string url = Path.Combine(BaseFolder, file);
                    FileUrls.Add(url);
                }
                UpdateUI();
            }
            catch (DirectoryNotFoundException)
            {
            }
            return Task.CompletedTask;
        }

    }
}
