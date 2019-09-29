#pragma once
#include "pch.h"

/// 旋转图片
static Mat rotate_image(const Mat img, int degree)
{
	degree = -degree;
	const double angle = degree * CV_PI / 180; // 弧度
	const double a = sin(angle);
	const double b = cos(angle);
	const int width = img.cols;
	const int height = img.rows;
	const int width_rotate = int(height * fabs(a) + width * fabs(b));
	const int height_rotate = int(width * fabs(a) + height * fabs(b));
	// 旋转数组map
	float map[6];
	const Mat map_matrix = Mat(2, 3, CV_32F, map);
	// 旋转中心
	const CvPoint2D32f center = CvPoint2D32f(static_cast<float>(width) / 2, static_cast<float>(height) / 2);
	CvMat map_matrix2 = map_matrix;
	cv2DRotationMatrix(center, degree, 1.0, &map_matrix2);
	map[2] += (width_rotate - width) / 2;
	map[5] += (height_rotate - height) / 2;
	Mat img_rotate;
	// 对图像做仿射变换
	warpAffine(img, img_rotate, map_matrix, Size(width_rotate, height_rotate), 1, 0, 0);
	return img_rotate;
}

/// Mat to byte[]
static BYTE* mat_to_bytes(const Mat image)
{
	const int size = static_cast<int>(image.total() * image.elemSize());
	BYTE* bytes = new BYTE[size];  // you will have to delete[] that later
	std::memcpy(bytes, image.data, size * sizeof(BYTE));
	return bytes;
}

/// Byte[] to Mat.
static Mat bytes_to_mat(BYTE* bytes, const int width, const int height, const int type)
{
	Mat image = Mat(height, width, type, bytes).clone(); // make a copy
	return image;
}


/// 释放内存
bool _stdcall ReleaseMemUseFree(unsigned char* buf)
{
	if (nullptr == buf)
	{
		return false;
	}
	free(buf);
	buf = nullptr;
	return true;
}

/// 释放new成员时产生的内存
bool _stdcall ReleaseMemUseDelete(unsigned char* pArray)
{
	if (nullptr == pArray)
	{
		return false;
	}
	delete[] pArray;
	pArray = nullptr;
	return true;
}