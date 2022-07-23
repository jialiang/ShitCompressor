using System;
using System.Runtime.InteropServices;

namespace Imazen.WebP.Extern
{
    public partial class NativeMethods
    {
        /// <summary>
        /// Retrieve basic header information: width, height.
        /// This function will also validate the header and return 0 in
        /// case of formatting error.
        /// Pointers 'width' and 'height' can be passed NULL if deemed irrelevant.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="data_size"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [DllImport("webp", EntryPoint = "WebPGetInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPGetInfo([In()] IntPtr data, UIntPtr data_size, ref int width, ref int height);

        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImport("webp", EntryPoint = "WebPDecodeBGRAInto", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeBGRAInto([In()] IntPtr data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);
    }
}
