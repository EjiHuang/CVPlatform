using System;
using System.Runtime.InteropServices;
using System.Text;

namespace App_ui.DllImport
{
    public struct ImageInfo
    {
        public IntPtr data;
        public IntPtr pMat;
        public int size;
        public float r1;
        public float r2;
        public IntPtr r3;
        public IntPtr r4;
    }

    public struct BitmapInfo
    {
        // 位图数据
        public byte[] data { get; set; }
        // 位图步长，防止偏移
        public int step { get; set; }
    }

    public class CVAlgorithms
    {
        /// Opencv wrapper - imread
        // void _stdcall CvpImread(char* pImagePath, int flag, ImageInfo &imgInfo)
        [DllImport("CVAlgorithms.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void CvpImread(StringBuilder path, ref ImageInfo imgInfo);

        /// Opencv wrapper - gray
        // void CvpGray(unsigned char* pBuf, int width, int height, ImageInfo &imgInfo)
        [DllImport("CVAlgorithms.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void CvpGray(byte[] pBuf, int width, int height, int step, ref ImageInfo imgInfo);

        /// Opencv wrapper - blur
        // void _stdcall CvpBlur(unsigned char* pBuf, int width, int height, int step, int kw, int kh, ImageInfo &imgInfo) 
        [DllImport("CVAlgorithms.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void CvpBlur(byte[] pBuf, int width, int height, int step, int kw, int kh, ref ImageInfo imgInfo);

        /// Opencv wrapper - threshold
        // void _stdcall CvpThreshold(unsigned char* pBuf, int width, int height, int step, int thresh, int tMax, int type, ImageInfo &imgInfo)
        [DllImport("CVAlgorithms.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void CvpThreshold(byte[] pBuf, int width, int height, int step, int thresh, int kMax, int type, ref ImageInfo imgInfo);

        /// Opencv wrapper - calcHist
        // void _stdcall CvpCalcGrayHist(unsigned char* pBuf, const int width, const int height, const int step, ImageInfo& imgInfo)
        [DllImport("CVAlgorithms.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void CvpCalcGrayHist(byte[] pBuf, int width, int height, int step, ref ImageInfo imgInfo);


        /// 释放new成员时产生的内存
        // bool _stdcall ReleaseMemUseDelete(double* pArray)
        [DllImport("CVAlgorithms.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool ReleaseMemUseDelete(IntPtr ptr);

        /// 释放内存
        // bool _stdcall ReleaseMemUseFree(unsigned char* buf)
        [DllImport("CVAlgorithms.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool ReleaseMemUseFree(IntPtr ptr);
    }

    #region Opencv struct

    [StructLayout(LayoutKind.Explicit)]
    public struct CvMat
    {
        /// <summary>
        /// CvMat signature (CV_MAT_MAGIC_VAL), element type and flags
        /// </summary>
        [FieldOffset(0)]
        public int type;

        /// <summary>
        /// full row length in bytes
        /// </summary>
        [FieldOffset(4)]
        public int step;

        /// <summary>
        /// for internal use only
        /// </summary>
        [FieldOffset(8)]
        public IntPtr refcount;

        /// <summary>
        /// for internal use only
        /// </summary>
        [FieldOffset(12)]
        public int hdr_refcount;

        /// <summary>
        /// underlaying data pointer
        /// </summary>
        [FieldOffset(16)]
        public IntPtr data;

        /// <summary>
        /// number of rows
        /// </summary>
        [FieldOffset(20)]
        public int rows;

        /// <summary>
        /// number of rows
        /// </summary>
        [FieldOffset(20)]
        public int height;

        /// <summary>
        /// number of columns
        /// </summary>
        [FieldOffset(24)]
        public int cols;

        /// <summary>
        /// number of columns
        /// </summary>
        [FieldOffset(24)]
        public int width;

        /// <summary>
        /// this pointer
        /// </summary>
        [FieldOffset(28)]
        public IntPtr ptr;
    }

    #endregion
}
