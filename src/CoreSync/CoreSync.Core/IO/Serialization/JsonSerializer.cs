// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

#endregion

namespace CoreSync.Core.IO.Serialization
{
    public class JsonSerializer : ISerializer
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets <see cref="Exception"/> instance of <see cref="JsonSerializer"/>.
        /// </summary>
        public Exception Error { get; set; }

        #endregion

        #region Public Functions

        /// <summary>
        /// Contains singleton instance of <see cref="JsonSerializer"/>.
        /// </summary>
        private static JsonSerializer singletonInstance;

        /// <summary>
        /// Gets singleton instance of <see cref="JsonSerializer"/>.
        /// </summary>
        public static JsonSerializer SingletonInstance
        {
            get
            {
                singletonInstance = singletonInstance ?? new JsonSerializer();

                return singletonInstance;
            }
        }

        /// <summary>
        /// Serializes instance of <typeparamref name="T"/> with <see cref="DataContractJsonSerializer"/> and copies bytes to <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Contains <see cref="Type"/> of <typeparamref name="T"/>.
        /// </typeparam>
        /// <param name="obj">
        /// Contains source instance of <typeparamref name="T"/>.
        /// </param>
        /// <param name="target">
        /// Contains target <see cref="Stream"/>.
        /// </param>
        /// <param name="indent">
        /// Contains whether the output uses multiline format.
        /// </param>
        public void Serialize<T>(T obj, Stream target, bool indent = false)
        {
            try
            {
                var writer = JsonReaderWriterFactory.CreateJsonWriter(target, Encoding.UTF8, true, indent, "  ");

                var serializer = new DataContractJsonSerializer(typeof(T));

                serializer.WriteObject(writer, obj);

                writer.Flush();
            }
            catch (Exception e)
            {
                Error = e;
            }
        }

        /// <summary>
        /// Serializes instance of <typeparamref name="T"/> with <see cref="DataContractJsonSerializer"/> to target filename.
        /// </summary>
        /// <typeparam name="T">
        /// Contains <see cref="Type"/> of <typeparamref name="T"/>.
        /// </typeparam>
        /// <param name="obj">
        /// Contains source instance of <typeparamref name="T"/>.
        /// </param>
        /// <param name="filename">
        /// Contains <see cref="string"/> value target filename.
        /// </param>
        /// <param name="indent">
        /// Contains whether the output uses multiline format.
        /// </param>
        public void Serialize<T>(T obj, string filename, bool indent = false)
        {
            try
            {
                using (var stream = File.Create(filename))
                {
                    Serialize<T>(obj, stream, indent);
                }
            }
            catch (Exception e)
            {
                Error = e;
            }
        }

        /// <summary>
        /// Deserializes instance of <typeparamref name="T"/> with <see cref="DataContractJsonSerializer"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Contains <see cref="Type"/> of <typeparamref name="T"/>.
        /// </typeparam>
        /// <param name="source">
        /// Contains source <see cref="Stream"/>.
        /// </param>
        /// <returns>
        /// Returns instance of <typeparamref name="T"/>.
        /// </returns>
        public T Deserialize<T>(Stream source)
        {
            try
            {
                var deserializer = new DataContractJsonSerializer(typeof(T));

                return (T)deserializer.ReadObject(source);
            }
            catch (Exception e)
            {
                Error = e;

                return default(T);
            }
        }

        /// <summary>
        /// Deserializes instance of <typeparamref name="T"/> with <see cref="DataContractJsonSerializer"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Contains <see cref="Type"/> of <typeparamref name="T"/>.
        /// </typeparam>
        /// <param name="source">
        /// Contains source <see cref="string"/> with JSON data or filename.
        /// </param>
        /// <param name="filename">
        /// Contains whether source <see cref="string"/> is filename.
        /// </param>
        /// <returns>
        /// Returns instance of <typeparamref name="T"/>.
        /// </returns>
        public T Deserialize<T>(string source, bool filename = true)
        {
            Stream stream = null;

            try
            {
                if (filename)
                {
                    stream = File.Open(source, FileMode.Open);
                }
                else
                {
                    stream = new MemoryStream();

                    var writer = new StreamWriter(stream);

                    writer.Write(source);
                    writer.Flush();

                    stream.Seek(0, SeekOrigin.Begin);
                }
            }
            catch (Exception e)
            {
                Error = e;
            }

            T result = this.Deserialize<T>(stream);

            stream?.Dispose();

            return result;
        }

        #endregion
    }
}
