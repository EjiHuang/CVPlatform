#pragma once
#include "pch.h"

static void calcHistgram(const Mat& src, int* hist)
{
	for (int y = 0; y < src.rows; y++)
	{
		for (int x = 0; x < src.cols; x++)
		{
			hist[src.ptr<uchar>(y)[x]]++;
		}
	}
}