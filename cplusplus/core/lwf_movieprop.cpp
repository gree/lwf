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

#include "lwf_movie.h"
#include "lwf_property.h"
#include "lwf_utility.h"

namespace LWF {

float Movie::GetX() const
{
	if (m_property->hasMatrix)
		return m_property->matrix.translateX;
	else
		return Utility::GetX(this);
}

void Movie::SetX(float value)
{
	if (!m_property->hasMatrix)
		Utility::SyncMatrix(this);
	m_property->MoveTo(value, m_property->matrix.translateY);
}

float Movie::GetY() const
{
	if (m_property->hasMatrix)
		return m_property->matrix.translateY;
	else
		return Utility::GetY(this);
}

void Movie::SetY(float value)
{
	if (!m_property->hasMatrix)
		Utility::SyncMatrix(this);
	m_property->MoveTo(m_property->matrix.translateX, value);
}

float Movie::GetScaleX() const
{
	if (m_property->hasMatrix)
		return m_property->scaleX;
	else
		return Utility::GetScaleX(this);
}

void Movie::SetScaleX(float value)
{
	if (!m_property->hasMatrix)
		Utility::SyncMatrix(this);
	m_property->ScaleTo(value, m_property->scaleY);
}

float Movie::GetScaleY() const
{
	if (m_property->hasMatrix)
		return m_property->scaleY;
	else
		return Utility::GetScaleY(this);
}

void Movie::SetScaleY(float value)
{
	if (!m_property->hasMatrix)
		Utility::SyncMatrix(this);
	m_property->ScaleTo(m_property->scaleX, value);
}

float Movie::GetRotation() const
{
	if (m_property->hasMatrix)
		return m_property->rotation;
	else
		return Utility::GetRotation(this);
}

void Movie::SetRotation(float value)
{
	if (!m_property->hasMatrix)
		Utility::SyncMatrix(this);
	m_property->RotateTo(value);
}

float Movie::GetAlpha() const
{
	if (m_property->hasColorTransform)
		return m_property->colorTransform.multi.alpha;
	else
		return Utility::GetAlpha(this);
}

void Movie::SetAlpha(float value)
{
	if (!m_property->hasColorTransform)
		Utility::SyncColorTransform(this);
	m_property->SetAlpha(value);
}

float Movie::GetRed() const
{
	if (m_property->hasColorTransform)
		return m_property->colorTransform.multi.red;
	else
		return Utility::GetRed(this);
}

void Movie::SetRed(float value)
{
	if (!m_property->hasColorTransform)
		Utility::SyncColorTransform(this);
	m_property->SetRed(value);
}

float Movie::GetGreen() const
{
	if (m_property->hasColorTransform)
		return m_property->colorTransform.multi.green;
	else
		return Utility::GetGreen(this);
}

void Movie::SetGreen(float value)
{
	if (!m_property->hasColorTransform)
		Utility::SyncColorTransform(this);
	m_property->SetGreen(value);
}

float Movie::GetBlue() const
{
	if (m_property->hasColorTransform)
		return m_property->colorTransform.multi.blue;
	else
		return Utility::GetBlue(this);
}

void Movie::SetBlue(float value)
{
	if (!m_property->hasColorTransform)
		Utility::SyncColorTransform(this);
	m_property->SetBlue(value);
}

}	// namespace LWF
