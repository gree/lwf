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

#include "lwf_core.h"
#include "lwf_data.h"
#include "lwf_format.h"
#include "lwf_movie.h"
#include "lwf_property.h"
#include "lwf_utility.h"

#define Constant Format

namespace LWF {

void Utility::CalcMatrixToPoint(
	float &dx, float &dy, float sx, float sy, const Matrix *m)
{
	dx = m->scaleX * sx + m->skew0  * sy + m->translateX;
	dy = m->skew1  * sx + m->scaleY * sy + m->translateY;
}

bool Utility::GetMatrixDeterminant(const Matrix *matrix)
{
	return matrix->scaleX * matrix->scaleY - matrix->skew0 * matrix->skew1 < 0;
}

void Utility::SyncMatrix(Movie *movie)
{
	int matrixId = movie->matrixId;
	float scaleX = 1;
	float scaleY = 1;
	float rotation = 0;
	Matrix matrix;
	if ((matrixId & Constant::MATRIX_FLAG) == 0) {
		const Translate &translate = movie->lwf->data->translates[matrixId];
		matrix.Set(scaleX, scaleY,
			0, 0, translate.translateX, translate.translateY);
	} else {
		matrixId &= ~Constant::MATRIX_FLAG_MASK;
		matrix.Set(&movie->lwf->data->matrices[matrixId]);
		bool md = GetMatrixDeterminant(&matrix);
		scaleX = sqrtf(
			matrix.scaleX * matrix.scaleX + matrix.skew1 * matrix.skew1);
		if (md)
			scaleX = -scaleX;
		scaleY = sqrtf(
			matrix.scaleY * matrix.scaleY + matrix.skew0 * matrix.skew0);
		if (md)
			rotation = atan2f(matrix.skew1, -matrix.scaleX);
		else
			rotation = atan2f(matrix.skew1, matrix.scaleX);
		rotation = rotation / M_PI * 180.0f;
	}

	movie->SetMatrix(&matrix, scaleX, scaleY, rotation);
}

float Utility::GetX(const Movie *movie)
{
	int matrixId = movie->matrixId;
	if ((matrixId & Constant::MATRIX_FLAG) == 0) {
		const Translate &translate = movie->lwf->data->translates[matrixId];
		return translate.translateX;
	} else {
		matrixId &= ~Constant::MATRIX_FLAG_MASK;
		const Matrix &matrix = movie->lwf->data->matrices[matrixId];
		return matrix.translateX;
	}
}

float Utility::GetY(const Movie *movie)
{
	int matrixId = movie->matrixId;
	if ((matrixId & Constant::MATRIX_FLAG) == 0) {
		const Translate &translate = movie->lwf->data->translates[matrixId];
		return translate.translateY;
	} else {
		matrixId &= ~Constant::MATRIX_FLAG_MASK;
		const Matrix &matrix = movie->lwf->data->matrices[matrixId];
		return matrix.translateY;
	}
}

float Utility::GetScaleX(const Movie *movie)
{
	int matrixId = movie->matrixId;
	if ((matrixId & Constant::MATRIX_FLAG) == 0) {
		return 1;
	} else {
		matrixId &= ~Constant::MATRIX_FLAG_MASK;
		const Matrix &matrix = movie->lwf->data->matrices[matrixId];
		bool md = GetMatrixDeterminant(&matrix);
		float scaleX = sqrtf(
			matrix.scaleX * matrix.scaleX + matrix.skew1 * matrix.skew1);
		if (md)
			scaleX = -scaleX;
		return scaleX;
	}
}

float Utility::GetScaleY(const Movie *movie)
{
	int matrixId = movie->matrixId;
	if ((matrixId & Constant::MATRIX_FLAG) == 0) {
		return 1;
	} else {
		matrixId &= ~Constant::MATRIX_FLAG_MASK;
		const Matrix &matrix = movie->lwf->data->matrices[matrixId];
		float scaleY = sqrtf(
			matrix.scaleY * matrix.scaleY + matrix.skew0 * matrix.skew0);
		return scaleY;
	}
}

float Utility::GetRotation(const Movie *movie)
{
	int matrixId = movie->matrixId;
	if ((matrixId & Constant::MATRIX_FLAG) == 0) {
		return 0;
	} else {
		matrixId &= ~Constant::MATRIX_FLAG_MASK;
		const Matrix &matrix = movie->lwf->data->matrices[matrixId];
		bool md = GetMatrixDeterminant(&matrix);
		float rotation;
		if (md)
			rotation = atan2f(matrix.skew1, -matrix.scaleX);
		else
			rotation = atan2f(matrix.skew1, matrix.scaleX);
		rotation = rotation / M_PI * 180.0f;
		return rotation;
	}
}

void Utility::SyncColorTransform(Movie *movie)
{
	int colorTransformId = movie->colorTransformId;
	if ((colorTransformId & Constant::COLORTRANSFORM_FLAG) == 0) {
		const AlphaTransform &alphaTransform =
			movie->lwf->data->alphaTransforms[colorTransformId];
		ColorTransform c(1, 1, 1, alphaTransform.alpha);
		movie->SetColorTransform(&c);
	} else {
		colorTransformId &= ~Constant::COLORTRANSFORM_FLAG;
		movie->SetColorTransform(
			&movie->lwf->data->colorTransforms[colorTransformId]);
	}
}

float Utility::GetAlpha(const Movie *movie)
{
	int colorTransformId = movie->colorTransformId;
	if ((colorTransformId & Constant::COLORTRANSFORM_FLAG) == 0) {
		const AlphaTransform &alphaTransform =
			movie->lwf->data->alphaTransforms[colorTransformId];
		return alphaTransform.alpha;
	} else {
		colorTransformId &= ~Constant::COLORTRANSFORM_FLAG;
		const ColorTransform &colorTransform =
			movie->lwf->data->colorTransforms[colorTransformId];
		return colorTransform.multi.alpha;
	}
}

float Utility::GetRed(const Movie *movie)
{
	int colorTransformId = movie->colorTransformId;
	if ((colorTransformId & Constant::COLORTRANSFORM_FLAG) == 0) {
		return 1;
	} else {
		colorTransformId &= ~Constant::COLORTRANSFORM_FLAG;
		const ColorTransform &colorTransform =
			movie->lwf->data->colorTransforms[colorTransformId];
		return colorTransform.multi.red;
	}
}

float Utility::GetGreen(const Movie *movie)
{
	int colorTransformId = movie->colorTransformId;
	if ((colorTransformId & Constant::COLORTRANSFORM_FLAG) == 0) {
		return 1;
	} else {
		colorTransformId &= ~Constant::COLORTRANSFORM_FLAG;
		const ColorTransform &colorTransform =
			movie->lwf->data->colorTransforms[colorTransformId];
		return colorTransform.multi.green;
	}
}

float Utility::GetBlue(const Movie *movie)
{
	int colorTransformId = movie->colorTransformId;
	if ((colorTransformId & Constant::COLORTRANSFORM_FLAG) == 0) {
		return 1;
	} else {
		colorTransformId &= ~Constant::COLORTRANSFORM_FLAG;
		const ColorTransform &colorTransform =
			movie->lwf->data->colorTransforms[colorTransformId];
		return colorTransform.multi.blue;
	}
}

Matrix *Utility::CalcMatrix(
	LWF *lwf, Matrix *dst, const Matrix *src0, int src1Id)
{
	if (src1Id == 0) {
		dst->Set(src0);
	} else if ((src1Id & Constant::MATRIX_FLAG) == 0) {
		const Translate &translate = lwf->data->translates[src1Id];
		dst->scaleX = src0->scaleX;
		dst->skew0  = src0->skew0;
		dst->translateX =
			src0->scaleX * translate.translateX +
			src0->skew0  * translate.translateY +
			src0->translateX;
		dst->skew1  = src0->skew1;
		dst->scaleY = src0->scaleY;
		dst->translateY =
			src0->skew1  * translate.translateX +
			src0->scaleY * translate.translateY +
			src0->translateY;
	} else {
		int matrixId = src1Id & ~Constant::MATRIX_FLAG_MASK;
		const Matrix &src1 = lwf->data->matrices[matrixId];
		CalcMatrix(dst, src0, &src1);
	}

	return dst;
}

Matrix *Utility::CalcMatrix(Matrix *dst, const Matrix *src0, const Matrix *src1)
{
	dst->scaleX =
		src0->scaleX * src1->scaleX +
		src0->skew0  * src1->skew1;
	dst->skew0 =
		src0->scaleX * src1->skew0 +
		src0->skew0  * src1->scaleY;
	dst->translateX =
		src0->scaleX * src1->translateX +
		src0->skew0  * src1->translateY +
		src0->translateX;
	dst->skew1 =
		src0->skew1  * src1->scaleX +
		src0->scaleY * src1->skew1;
	dst->scaleY =
		src0->skew1  * src1->skew0 +
		src0->scaleY * src1->scaleY;
	dst->translateY =
		src0->skew1  * src1->translateX +
		src0->scaleY * src1->translateY +
		src0->translateY;
	return dst;
}

Matrix *Utility::CopyMatrix(Matrix *dst, const Matrix *src)
{
	if (src)
		dst->Set(src);
	else
		dst->Clear();
	return dst;
}

void Utility::InvertMatrix(Matrix *dst, const Matrix *src)
{
	float dt = src->scaleX * src->scaleY - src->skew0 * src->skew1;
	if (dt != 0.0f) {
		dst->scaleX = src->scaleY / dt;
		dst->skew0 = -src->skew0 / dt;
		dst->translateX = (src->skew0 * src->translateY -
			src->translateX * src->scaleY) / dt;
		dst->skew1 = -src->skew1 / dt;
		dst->scaleY = src->scaleX / dt;
		dst->translateY = (src->translateX * src->skew1 -
			src->scaleX * src->translateY) / dt;
	} else {
		dst->Clear();
	}
}

ColorTransform *Utility::CalcColorTransform(LWF *lwf,
	ColorTransform *dst, const ColorTransform *src0, int src1Id)
{
	if (src1Id == 0) {
		dst->Set(src0);
	} else if ((src1Id & Constant::COLORTRANSFORM_FLAG) == 0) {
		const AlphaTransform &alphaTransform =
			lwf->data->alphaTransforms[src1Id];
		dst->multi.red   = src0->multi.red;
		dst->multi.green = src0->multi.green;
		dst->multi.blue  = src0->multi.blue;
		dst->multi.alpha = src0->multi.alpha * alphaTransform.alpha;
#if LWF_USE_ADDITIONALCOLOR
		dst->add.Set(src0->add);
#endif
	} else {
		int colorTransformId = src1Id & ~Constant::COLORTRANSFORM_FLAG;
		const ColorTransform &src1 =
			lwf->data->colorTransforms[colorTransformId];
		CalcColorTransform(dst, src0, &src1);
	}
	return dst;
}

ColorTransform *Utility::CalcColorTransform(ColorTransform *dst,
	const ColorTransform *src0, const ColorTransform *src1)
{
	dst->multi.red   = src0->multi.red   * src1->multi.red;
	dst->multi.green = src0->multi.green * src1->multi.green;
	dst->multi.blue  = src0->multi.blue  * src1->multi.blue;
	dst->multi.alpha = src0->multi.alpha * src1->multi.alpha;
#if LWF_USE_ADDITIONALCOLOR
	dst->add.red   = src0->add.red   * src1->multi.red   + src1->add.red;
	dst->add.green = src0->add.green * src1->multi.green + src1->add.green;
	dst->add.blue  = src0->add.blue  * src1->multi.blue  + src1->add.blue;
	dst->add.alpha = src0->add.alpha * src1->multi.alpha + src1->add.alpha;
#endif
	return dst;
}

ColorTransform *Utility::CopyColorTransform(
	ColorTransform *dst, const ColorTransform *src)
{
	if (src)
		dst->Set(src);
	else
		dst->Clear();
	return dst;
}

void Utility::CalcColor(Color *dst, const Color *c, const ColorTransform *t)
{
#if LWF_USE_ADDITIONALCOLOR
	dst->red   = c->red   * t->multi.red   + t->add.red;
	dst->green = c->green * t->multi.green + t->add.green;
	dst->blue  = c->blue  * t->multi.blue  + t->add.blue;
	dst->alpha = c->alpha * t->multi.alpha + t->add.alpha;
#else
	dst->red   = c->red   * t->multi.red;
	dst->green = c->green * t->multi.green;
	dst->blue  = c->blue  * t->multi.blue;
	dst->alpha = c->alpha * t->multi.alpha;
#endif
}

vector<string> Utility::Split(const string &str, char d)
{
	vector<string> a;
	size_t pos = 0;
	for (;;) {
		size_t next = str.find_first_of(d, pos);
		if (next == string::npos)
			break;

		a.push_back(string(str, pos, next - pos));
		pos = next + 1;
	}
	a.push_back(string(str, pos, str.size() - pos));
	return a;
}

}	// namespace LWF
