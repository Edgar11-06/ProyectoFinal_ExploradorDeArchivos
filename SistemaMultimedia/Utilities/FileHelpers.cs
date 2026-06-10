using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace SistemaMultimedia.Utilities
{
    /// <summary>
    /// Métodos auxiliares para obtener información legible de archivos/carpetas,
    /// iconos del sistema y cálculo de tamaño de carpetas.
    /// </summary>
    public static class FileHelpers
    {
        // P/Invoke para obtener icono del sistema
        private const uint SHGFI_ICON = 0x000000100;
        private const uint SHGFI_LARGEICON = 0x000000000; // 'large icon'
        private const uint SHGFI_SMALLICON = 0x000000001; // 'small icon'
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;

        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;
        private const uint FILE_ATTRIBUTE_FILE = 0x80;

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        /// <summary>
        /// Calcula recursivamente el número de archivos y subdirectorios dentro de la carpeta.
        /// Maneja excepciones de acceso y sigue contando en el resto de la estructura.
        /// </summary>
        public static (int files, int directories, bool completed) GetDirectoryCounts(string path)
        {
            int files = 0;
            int dirs = 0;
            bool completed = true;
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return (0, 0, true);

            try
            {
                var stack = new System.Collections.Generic.Stack<string>();
                stack.Push(path);

                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    try
                    {
                        string[] subDirs = Directory.GetDirectories(current);
                        foreach (var d in subDirs)
                        {
                            dirs++;
                            stack.Push(d);
                        }
                    }
                    catch (UnauthorizedAccessException) { completed = false; }
                    catch { completed = false; }

                    try
                    {
                        string[] f = Directory.GetFiles(current);
                        files += f.Length;
                    }
                    catch (UnauthorizedAccessException) { completed = false; }
                    catch { completed = false; }
                }
            }
            catch { completed = false; }

            return (files, dirs, completed);
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        /// <summary>
        /// Obtiene un icono asociado a una ruta (archivo o carpeta). Devuelve null si falla.
        /// </summary>
        public static Icon? GetSystemIcon(string path, bool small = false, bool treatAsDirectory = false)
        {
            try
            {
                uint flags = SHGFI_ICON | (small ? SHGFI_SMALLICON : SHGFI_LARGEICON);
                uint attr = FILE_ATTRIBUTE_FILE;
                if (treatAsDirectory) attr = FILE_ATTRIBUTE_DIRECTORY;

                var shfi = new SHFILEINFO();
                IntPtr res = SHGetFileInfo(path, attr, out shfi, (uint)Marshal.SizeOf(shfi), flags | SHGFI_USEFILEATTRIBUTES);
                if (res == IntPtr.Zero) return null;
                if (shfi.hIcon == IntPtr.Zero) return null;

                var ico = Icon.FromHandle(shfi.hIcon);
                // Clone to detach from unmanaged handle
                var clone = (Icon)ico.Clone();
                DestroyIcon(shfi.hIcon);
                return clone;
            }
            catch { return null; }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        /// <summary>
        /// Retorna tamaño legible (KB, MB, GB) con 2 decimales.
        /// </summary>
        public static string GetReadableSize(long bytes)
        {
            try
            {
                if (bytes < 1024) return $"{bytes} B";
                double kb = bytes / 1024d;
                if (kb < 1024) return $"{kb:F2} KB";
                double mb = kb / 1024d;
                if (mb < 1024) return $"{mb:F2} MB";
                double gb = mb / 1024d;
                return $"{gb:F2} GB";
            }
            catch { return "(desconocido)"; }
        }

        /// <summary>
        /// Calcula recursivamente el tamaño total de una carpeta en bytes.
        /// Maneja excepciones de acceso y continúa.
        /// </summary>
        public static long GetDirectorySize(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return 0;
            long total = 0;
            try
            {
                var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
                foreach (var f in files)
                {
                    try { var info = new FileInfo(f); total += info.Length; }
                    catch { }
                }
            }
            catch { }
            return total;
        }

        /// <summary>
        /// Detecta tipo simple de archivo por extensión: Imagen, Video, Audio, Otro.
        /// </summary>
        public static string DetectFileCategory(string path)
        {
            if (string.IsNullOrEmpty(path)) return "Desconocido";
            var ext = Path.GetExtension(path).ToLowerInvariant();
            var images = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".webp" };
            var videos = new[] { ".mp4", ".mov", ".wmv", ".avi", ".mkv", ".flv" };
            var audios = new[] { ".mp3", ".wav", ".flac", ".aac", ".m4a", ".wma" };

            if (Array.Exists(images, e => e == ext)) return "Imagen";
            if (Array.Exists(videos, e => e == ext)) return "Video";
            if (Array.Exists(audios, e => e == ext)) return "Audio";
            return "Archivo";
        }
    }
}
