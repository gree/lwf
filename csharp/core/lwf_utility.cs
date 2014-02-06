/*
 * Copyright (C) 2012 GREE, Inc.
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

using System;

namespace LWF {

using Constant = Format.Constant;

public class Utility
{
	public static void CalcMatrixToPoint(
		out float dx, out float dy, float sx, float sy, Matrix m)
	{
		dx = m.scaleX * sx + m.skew0  * sy + m.translateX;
		dy = m.skew1  * sx + m.scaleY * sy + m.translateY;
	}

	public static bool GetMatrixDeterminant(Matrix matrix)
	{
		return matrix.scaleX * matrix.scaleY - matrix.skew0 * matrix.skew1 < 0;
	}

	public static void SyncMatrix(Movie movie)
	{
		int matrixId = movie.matrixId;
		float scaleX = 1;
		float scaleY = 1;
		float rotation = 0;
		Matrix matrix;
		if ((matrixId & (int)Constant.MATRIX_FLAG) == 0) {
			Translate translate = movie.lwf.data.translates[matrixId];
			matrix = new Matrix(scaleX, scaleY,
				0, 0, translate.translateX, translate.translateY);
		} else {
			matrixId &= ~(int)Constant.MATRIX_FLAG_MASK;
			matrix = movie.lwf.data.matrices[matrixId];
			bool md = GetMatrixDeterminant(matrix);
			scaleX = (float)Math.Sqrt(
				matrix.scaleX * matrix.scaleX + matrix.skew1 * matrix.skew1);
			if (md)
				scaleX = -scaleX;
			scaleY = (float)Math.Sqrt(
				matrix.scaleY * matrix.scaleY + matrix.skew0 * matrix.skew0);
			if (md)
				rotation = (float)Math.Atan2(matrix.skew1, -matrix.scaleX);
			else
				rotation = (float)Math.Atan2(matrix.skew1, matrix.scaleX);
			rotation = rotation / (float)Math.PI * 180.0f;
		}

		movie.SetMatrix(matrix, scaleX, scaleY, rotation);
	}

	public static float GetX(Movie movie)
	{
		int matrixId = movie.matrixId;
		if ((matrixId & (int)Constant.MATRIX_FLAG) == 0) {
			Translate translate = movie.lwf.data.translates[matrixId];
			return translate.translateX;
		} else {
			matrixId &= ~(int)Constant.MATRIX_FLAG_MASK;
			Matrix matrix = movie.lwf.data.matrices[matrixId];
			return matrix.translateX;
		}
	}

	public static float GetY(Movie movie)
	{
		int matrixId = movie.matrixId;
		if ((matrixId & (int)Constant.MATRIX_FLAG) == 0) {
			Translate translate = movie.lwf.data.translates[matrixId];
			return translate.translateY;
		} else {
			matrixId &= ~(int)Constant.MATRIX_FLAG_MASK;
			Matrix matrix = movie.lwf.data.matrices[matrixId];
			return matrix.translateY;
		}
	}

	public static float GetScaleX(Movie movie)
	{
		int matrixId = movie.matrixId;
		if ((matrixId & (int)Constant.MATRIX_FLAG) == 0) {
			return 1;
		} else {
			matrixId &= ~(int)Constant.MATRIX_FLAG_MASK;
			Matrix matrix = movie.lwf.data.matrices[matrixId];
			bool md = GetMatrixDeterminant(matrix);
			float scaleX = (float)Math.Sqrt(
				matrix.scaleX * matrix.scaleX + matrix.skew1 * matrix.skew1);
			if (md)
				scaleX = -scaleX;
			return scaleX;
		}
	}

	public static float GetScaleY(Movie movie)
	{
		int matrixId = movie.matrixId;
		if ((matrixId & (int)Constant.MATRIX_FLAG) == 0) {
			return 1;
		} else {
			matrixId &= ~(int)Constant.MATRIX_FLAG_MASK;
			Matrix matrix = movie.lwf.data.matrices[matrixId];
			float scaleY = (float)Math.Sqrt(
				matrix.scaleY * matrix.scaleY + matrix.skew0 * matrix.skew0);
			return scaleY;
		}
	}

	public static float GetRotation(Movie movie)
	{
		int matrixId = movie.matrixId;
		if ((matrixId & (int)Constant.MATRIX_FLAG) == 0) {
			return 0;
		} else {
			matrixId &= ~(int)Constant.MATRIX_FLAG_MASK;
			Matrix matrix = movie.lwf.data.matrices[matrixId];
			bool md = GetMatrixDeterminant(matrix);
			float rotation;
			if (md)
				rotation = (float)Math.Atan2(matrix.skew1, -matrix.scaleX);
			else
				rotation = (float)Math.Atan2(matrix.skew1, matrix.scaleX);
			rotation = rotation / (float)Math.PI * 180.0f;
			return rotation;
		}
	}

	public static void SyncColorTransform(Movie movie)
	{
		int colorTransformId = movie.colorTransformId;
		ColorTransform colorTransform;
		if ((colorTransformId & (int)Constant.COLORTRANSFORM_FLAG) == 0) {
			AlphaTransform alphaTransform =
				movie.lwf.data.alphaTransforms[colorTransformId];
			colorTransform = new ColorTransform(1, 1, 1, alphaTransform.alpha);
		} else {
			colorTransformId &= ~(int)Constant.COLORTRANSFORM_FLAG;
			colorTransform = movie.lwf.data.colorTransforms[colorTransformId];
		}

		movie.SetColorTransform(colorTransform);
	}

	public static float GetAlpha(Movie movie)
	{
		int colorTransformId = movie.colorTransformId;
		if ((colorTransformId & (int)Constant.COLORTRANSFORM_FLAG) == 0) {
			AlphaTransform alphaTransform =
				movie.lwf.data.alphaTransforms[colorTransformId];
			return alphaTransform.alpha;
		} else {
			colorTransformId &= ~(int)Constant.COLORTRANSFORM_FLAG;
			ColorTransform colorTransform =
				movie.lwf.data.colorTransforms[colorTransformId];
			return colorTransform.multi.alpha;
		}
	}

	public static float GetRed(Movie movie)
	{
		int colorTransformId = movie.colorTransformId;
		if ((colorTransformId & (int)Constant.COLORTRANSFORM_FLAG) == 0) {
			return 1;
		} else {
			colorTransformId &= ~(int)Constant.COLORTRANSFORM_FLAG;
			ColorTransform colorTransform =
				movie.lwf.data.colorTransforms[colorTransformId];
			return colorTransform.multi.red;
		}
	}

	public static float GetGreen(Movie movie)
	{
		int colorTransformId = movie.colorTransformId;
		if ((colorTransformId & (int)Constant.COLORTRANSFORM_FLAG) == 0) {
			return 1;
		} else {
			colorTransformId &= ~(int)Constant.COLORTRANSFORM_FLAG;
			ColorTransform colorTransform =
				movie.lwf.data.colorTransforms[colorTransformId];
			return colorTransform.multi.green;
		}
	}

	public static float GetBlue(Movie movie)
	{
		int colorTransformId = movie.colorTransformId;
		if ((colorTransformId & (int)Constant.COLORTRANSFORM_FLAG) == 0) {
			return 1;
		} else {
			colorTransformId &= ~(int)Constant.COLORTRANSFORM_FLAG;
			ColorTransform colorTransform =
				movie.lwf.data.colorTransforms[colorTransformId];
			return colorTransform.multi.blue;
		}
	}

	public static Matrix CalcMatrix(
		LWF lwf, Matrix dst, Matrix src0, int src1Id)
	{
		if (src1Id == 0) {
			dst.Set(src0);
		} else if ((src1Id & (int)Constant.MATRIX_FLAG) == 0) {
			Translate translate = lwf.data.translates[src1Id];
			dst.scaleX = src0.scaleX;
			dst.skew0  = src0.skew0;
			dst.translateX =
				src0.scaleX * translate.translateX +
				src0.skew0  * translate.translateY +
				src0.translateX;
			dst.skew1  = src0.skew1;
			dst.scaleY = src0.scaleY;
			dst.translateY =
				src0.skew1  * translate.translateX +
				src0.scaleY * translate.translateY +
				src0.translateY;
		} else {
			int matrixId = src1Id & ~(int)Constant.MATRIX_FLAG_MASK;
			Matrix src1 = lwf.data.matrices[matrixId];
			CalcMatrix(dst, src0, src1);
		}

		return dst;
	}

	public static Matrix CalcMatrix(Matrix dst, Matrix src0, Matrix src1)
	{
		dst.scaleX =
			src0.scaleX * src1.scaleX +
			src0.skew0  * src1.skew1;
		dst.skew0 =
			src0.scaleX * src1.skew0 +
			src0.skew0  * src1.scaleY;
		dst.translateX =
			src0.scaleX * src1.translateX +
			src0.skew0  * src1.translateY +
			src0.translateX;
		dst.skew1 =
			src0.skew1  * src1.scaleX +
			src0.scaleY * src1.skew1;
		dst.scaleY =
			src0.skew1  * src1.skew0 +
			src0.scaleY * src1.scaleY;
		dst.translateY =
			src0.skew1  * src1.translateX +
			src0.scaleY * src1.translateY +
			src0.translateY;
		return dst;
	}

	public static void FitForHeight(LWF lwf, float stageHeight)
	{
		float scale = stageHeight / lwf.height;
		float offsetX = -lwf.width / 2 * scale;
		float offsetY = -lwf.height / 2 * scale;
		lwf.property.Scale(scale, scale);
		lwf.property.Move(offsetX, offsetY);
	}

	public static void FitForWidth(LWF lwf, float stageWidth)
	{
		float scale = stageWidth / lwf.width;
		float offsetX = -lwf.width / 2 * scale;
		float offsetY = -lwf.height / 2 * scale;
		lwf.property.Scale(scale, scale);
		lwf.property.Move(offsetX, offsetY);
	}

	public static void ScaleForHeight(LWF lwf, float stageHeight)
	{
		float scale = stageHeight / lwf.height;
		lwf.property.Scale(scale, scale);
	}

	public static void ScaleForWidth(LWF lwf, float stageWidth)
	{
		float scale = stageWidth / lwf.width;
		lwf.property.Scale(scale, scale);
	}

	public static Matrix CopyMatrix(Matrix dst, Matrix src)
	{
		if (src == null)
			dst.Clear();
		else
			dst.Set(src);
		return dst;
	}

	public static void InvertMatrix(Matrix dst, Matrix src)
	{
		float dt = src.scaleX * src.scaleY - src.skew0 * src.skew1;
		if (dt != 0.0f) {
			dst.scaleX = src.scaleY / dt;
			dst.skew0 = -src.skew0 / dt;
			dst.translateX = (src.skew0 * src.translateY -
				src.translateX * src.scaleY) / dt;
			dst.skew1 = -src.skew1 / dt;
			dst.scaleY = src.scaleX / dt;
			dst.translateY = (src.translateX * src.skew1 -
				src.scaleX * src.translateY) / dt;
		} else {
			dst.Clear();
		}
	}

	public static ColorTransform CalcColorTransform(LWF lwf,
		ColorTransform dst, ColorTransform src0, int src1Id)
	{
		if (src1Id == 0) {
			dst.Set(src0);
		} else if ((src1Id & (int)Constant.COLORTRANSFORM_FLAG) == 0) {
			AlphaTransform alphaTransform =
				lwf.data.alphaTransforms[src1Id];
			dst.multi.red   = src0.multi.red;
			dst.multi.green = src0.multi.green;
			dst.multi.blue  = src0.multi.blue;
			dst.multi.alpha = src0.multi.alpha * alphaTransform.alpha;
			dst.add.Set(src0.add);
		} else {
			int colorTransformId = src1Id & ~(int)Constant.COLORTRANSFORM_FLAG;
			ColorTransform src1 = lwf.data.colorTransforms[colorTransformId];
			CalcColorTransform(dst, src0, src1);
		}
		return dst;
	}

	public static ColorTransform CalcColorTransform(ColorTransform dst,
		ColorTransform src0, ColorTransform src1)
	{
		dst.multi.red   = src0.multi.red   * src1.multi.red;
		dst.multi.green = src0.multi.green * src1.multi.green;
		dst.multi.blue  = src0.multi.blue  * src1.multi.blue;
		dst.multi.alpha = src0.multi.alpha * src1.multi.alpha;
		dst.add.red   = src0.add.red   * src1.multi.red   + src1.add.red;
		dst.add.green = src0.add.green * src1.multi.green + src1.add.green;
		dst.add.blue  = src0.add.blue  * src1.multi.blue  + src1.add.blue;
		dst.add.alpha = src0.add.alpha * src1.multi.alpha + src1.add.alpha;
		return dst;
	}

	public static ColorTransform CopyColorTransform(
		ColorTransform dst, ColorTransform src)
	{
		if (src == null)
			dst.Clear();
		else
			dst.Set(src);
		return dst;
	}

	public static void CalcColor(Color dst, Color c, ColorTransform t)
	{
		dst.red   = c.red   * t.multi.red   + t.add.red;
		dst.green = c.green * t.multi.green + t.add.green;
		dst.blue  = c.blue  * t.multi.blue  + t.add.blue;
		dst.alpha = c.alpha * t.multi.alpha + t.add.alpha;
	}
}

}	// namespace LWF
