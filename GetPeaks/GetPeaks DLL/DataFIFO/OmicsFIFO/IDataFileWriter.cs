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
    public interface IDataFileWriter<T>
    {
        /// <summary>
        /// Writes the data to the file path provided.
        /// </summary>
        /// <param name="path">Path of file to write.</param>
        /// <param name="data">List of data to write.</param>
        void WriteFile(string path, ICollection<T> data);
    }
}
