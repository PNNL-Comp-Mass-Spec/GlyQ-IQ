using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PNNLOmics.IO
{
    /// <summary>
    /// Interface for objects that read lists of data from a file.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataFileReader<T>
    {
        /// <summary>
        /// Reads the data from the file path provided.
        /// </summary>
        /// <param name="path">Path of file to read.</param>
        /// <returns>List of data objects to read from</returns>
        ICollection<T> ReadFile(string path);
    }
}
