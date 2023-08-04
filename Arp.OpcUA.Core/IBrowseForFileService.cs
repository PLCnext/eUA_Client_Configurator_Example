// Copyright PHOENIX CONTACT Electronics GmbH

using System.IO;
using System.Threading.Tasks;

namespace Arp.OpcUA.Core
{
    public interface IBrowseForFileService
    {
        /// <summary>
        /// Shows the standard file open dialog to select a file to overwrite or a new file.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="filter">e.g. "Word Documents|*.doc"</param>
        /// <returns>filename or null</returns>
        Task<Stream> OpenFileForWriteAsync(string title, string filter, FileMode fileMode);
        /// <summary>
        /// Shows the standard file open dialog to select an existing file to read from.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="filter">e.g. "Word Documents|*.doc"</param>
        /// <returns>filename or null</returns>
        Task<Stream> OpenFileForReadAsync(string title, string filter, FileMode fileMode);
    }
}
