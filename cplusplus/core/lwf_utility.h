/*
 * Copyright (C) 2013 GREE, Inc.
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

#ifndef LWF_UTILITY_H
#define LWF_UTILITY_H

namespace LWF {

class LWF;
class Matrix;
class Movie;
class ColorTransform;

class Utility
{
public:
	static void CalcMatrixToPoint(
		float &dx, float &dy, float sx, float sy, const Matrix *m);
	static bool GetMatrixDeterminant(const Matrix *matrix);
	static void SyncMatrix(Movie *movie);
	static float GetX(const Movie *movie);
	static float GetY(const Movie *movie);
	static float GetScaleX(const Movie *movie);
	static float GetScaleY(const Movie *movie);
	static float GetRotation(const Movie *movie);
	static void SyncColorTransform(Movie *movie);
	static float GetAlpha(const Movie *movie);
	static float GetRed(const Movie *movie);
	static float GetGreen(const Movie *movie);
	static float GetBlue(const Movie *movie);
	static Matrix *CalcMatrix(
		LWF *lwf, Matrix *dst, const Matrix *src0, int src1Id);
	static Matrix *CalcMatrix(
		Matrix *dst, const Matrix *src0, const Matrix *src1);
	static Matrix *CopyMatrix(Matrix *dst, const Matrix *src);
	static void InvertMatrix(Matrix *dst, const Matrix *src);
	static ColorTransform *CalcColorTransform(LWF *lwf,
		ColorTransform *dst, const ColorTransform *src0, int src1Id);
	static ColorTransform *CalcColorTransform(ColorTransform *dst,
		const ColorTransform *src0, const ColorTransform *src1);
	static ColorTransform *CopyColorTransform(
		ColorTransform *dst, const ColorTransform *src);
	static void CalcColor(Color *dst, const Color *c, const ColorTransform *t);
	static vector<string> Split(const string &str, char d);
};

}	// namespace LWF

#endif
