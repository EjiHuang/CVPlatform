#include "pch.h"

/// 交换数据结构
struct ImageInfo
{
	unsigned char* data;
	unsigned char* pMat;
	int size;
	float r1;
	float r2;
	double* r3;
};

/// Opencv wrapper - imread
void _stdcall CvpImread(char* pImagePath, ImageInfo& imgInfo)
{
	const Mat im_src = imread(pImagePath, IMREAD_COLOR);

	std::vector<unsigned char> bytes;
	imencode(".bmp", im_src, bytes);
	imgInfo.size = static_cast<int>(bytes.size());
	imgInfo.data = static_cast<unsigned char*>(calloc(imgInfo.size, sizeof(unsigned char)));
	std::copy(bytes.begin(), bytes.end(), imgInfo.data);
}

/// Opencv wrapper - gray
void _stdcall CvpGray(unsigned char* pBuf, const int width, const int height, const int step, ImageInfo& imgInfo)
{
	Mat im_src = Mat(height, width, CV_8UC4, pBuf, step);
	cvtColor(im_src, im_src, CV_BGR2GRAY);

	std::vector<unsigned char> bytes;
	imencode(".bmp", im_src, bytes);
	imgInfo.size = static_cast<int>(bytes.size());
	imgInfo.data = static_cast<unsigned char*>(calloc(imgInfo.size, sizeof(unsigned char)));
	std::copy(bytes.begin(), bytes.end(), imgInfo.data);
}

/// Opencv wrapper - blur
void _stdcall CvpBlur(unsigned char* pBuf, const int width, const int height, const int step, const int kw, const int kh, ImageInfo& imgInfo)
{
	const Mat im_src = Mat(height, width, CV_8UC4, pBuf, step);
	Mat im_dst;
	if (kw > 0 && kh > 0)
	{
		blur(im_src, im_dst, Size(kw, kh));

		std::vector<unsigned char> bytes;
		imencode(".bmp", im_dst, bytes);
		imgInfo.size = static_cast<int>(bytes.size());
		imgInfo.data = static_cast<unsigned char*>(calloc(imgInfo.size, sizeof(unsigned char)));
		std::copy(bytes.begin(), bytes.end(), imgInfo.data);
	}
	else
	{
		std::vector<unsigned char> bytes;
		imencode(".bmp", im_src, bytes);
		imgInfo.size = static_cast<int>(bytes.size());
		imgInfo.data = static_cast<unsigned char*>(calloc(imgInfo.size, sizeof(unsigned char)));
		std::copy(bytes.begin(), bytes.end(), imgInfo.data);
	}

}

/// Opencv wrapper - threshold
void _stdcall CvpThreshold(unsigned char* pBuf, const int width, const int height, const int step, const int thresh, const int tMax, const int type, ImageInfo& imgInfo)
{
	Mat im_src = Mat(height, width, CV_8UC4, pBuf, step);
	Mat im_dst;

	threshold(im_src, im_dst, thresh, tMax, type);

	std::vector<unsigned char> bytes;
	imencode(".bmp", im_dst, bytes);
	imgInfo.size = static_cast<int>(bytes.size());
	imgInfo.data = static_cast<unsigned char*>(calloc(imgInfo.size, sizeof(unsigned char)));
	std::copy(bytes.begin(), bytes.end(), imgInfo.data);
}


/// 仿射变换匹配
//struct _data {
//	Mat im;
//	std::vector<Point2f> points;
//};
//void MouseHandler(int event, int x, int y, int flags, void* data_ptr)
//{
//	if (event == EVENT_LBUTTONDOWN)
//	{
//		_data* data = ((_data*)data_ptr);
//		circle(data->im, Point(x, y), 3, Scalar(0, 0, 255), 5, CV_AA);
//		imshow("Image", data->im);
//		if (data->points.size() < 4)
//		{
//			data->points.push_back(Point2f((float)x, (float)y));
//		}
//	}
//}
//void _stdcall AlgorithmsHomography(char* pImagePath, ImageInfo& imgInfo)
//{
//	Mat im_src = imread(pImagePath);
//	Mat im_temp = im_src.clone();
//	_data src_data, dst_data;
//	src_data.im = im_temp;
//
//	// 显示图片，并且用鼠标描点确定坐标
//	imshow("Image", im_temp);
//	setMouseCallback("Image", MouseHandler, &src_data);
//	int key = waitKey(-1);
//
//	if (key)
//	{
//		destroyAllWindows();
//		Mat im_src_rotate = rotate_image(im_src, 50);
//		im_temp = im_src_rotate.clone();
//		dst_data.im = im_temp;
//		imshow("Image", im_temp);
//		setMouseCallback("Image", MouseHandler, &dst_data);
//		int key = waitKey(-1);
//		if (key)
//		{
//			// destroyAllWindows();
//			// 计算homography
//			Mat h = findHomography(src_data.points, dst_data.points);
//			// 将原图像变换到目标图像
//			Mat im_dst = Mat::zeros(dst_data.im.size(), CV_8UC3);
//			warpPerspective(im_src, im_dst, h, im_dst.size());
//			// 回调给UI
//			std::vector<unsigned char> bytes;
//			imencode(".bmp", im_dst, bytes);
//			ReleaseMemUseFree(imgInfo.data);
//			imgInfo.size = static_cast<int>(bytes.size());
//			imgInfo.data = static_cast<unsigned char*>(calloc(imgInfo.size, sizeof(unsigned char)));
//			std::copy(bytes.begin(), bytes.end(), imgInfo.data);
//		}
//	}
//
//}