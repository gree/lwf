/*
 * Copyright (C) 2014 GREE, Inc.
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

#import <assert.h>
#import <string.h>
#import "LWFShader.h"

#define STRINGIFY(A) #A

static const char *vertexShader = STRINGIFY(
	attribute vec4 Position;
	attribute vec2 TexCoordIn;
	attribute vec4 ColorIn;
	
	uniform mat4 Projection;
	uniform mat4 ModelView;
	
	varying vec2 TexCoordOut;
	varying vec4 ColorOut;
	
	void main(void)
	{
		gl_Position = Projection * ModelView * Position;
		TexCoordOut = TexCoordIn;
		ColorOut = ColorIn;
	}
);

static const char *fragmentShader = STRINGIFY(
	varying lowp vec2 TexCoordOut;
	varying lowp vec4 ColorOut;
	uniform sampler2D Texture;
	
	void main(void)
	{
		gl_FragColor = ColorOut * texture2D(Texture, TexCoordOut);
	}
);

namespace LWF {

LWFShader *LWFShader::m_instance;

LWFShader *LWFShader::shared()
{
	static dispatch_once_t once;

	dispatch_once(&once, ^{
		m_instance = new LWFShader();
	});

	return m_instance;
}

static GLuint compile(GLenum shaderType, const char *source, int length)
{
    GLuint shaderHandle = glCreateShader(shaderType);
    glShaderSource(shaderHandle, 1, &source, &length);
    glCompileShader(shaderHandle);
    GLint compileSuccess;
    glGetShaderiv(shaderHandle, GL_COMPILE_STATUS, &compileSuccess);
	assert("can't compile shader" && compileSuccess == GL_TRUE);
    return shaderHandle;
}

LWFShader::LWFShader()
{
}

LWFShader::~LWFShader()
{
}

void LWFShader::init()
{
	GLuint vertexHandle = compile(
		GL_VERTEX_SHADER, vertexShader, (int)strlen(vertexShader));
	GLuint fragmentHandle = compile(
		GL_FRAGMENT_SHADER, fragmentShader, (int)strlen(fragmentShader));
    programHandle = glCreateProgram();
    glAttachShader(programHandle, vertexHandle);
    glAttachShader(programHandle, fragmentHandle);
    glLinkProgram(programHandle);

    GLint linkSuccess;
    glGetProgramiv(programHandle, GL_LINK_STATUS, &linkSuccess);
	assert("can't compile shader" && linkSuccess == GL_TRUE);

    positionAttribute = glGetAttribLocation(programHandle, "Position");
	texCoordAttribute = glGetAttribLocation(programHandle, "TexCoordIn");
	colorAttribute = glGetAttribLocation(programHandle, "ColorIn");

	textureUniform = glGetUniformLocation(programHandle, "Texture");

	projectionUniform = glGetUniformLocation(programHandle, "Projection");
	modelUniform = glGetUniformLocation(programHandle, "ModelView");
}

void LWFShader::load()
{
	glUseProgram(programHandle);
}

void LWFShader::setProjectionMatrix(GLKMatrix4 m)
{
	glUniformMatrix4fv(projectionUniform, 1, 0, m.m);
}

void LWFShader::setModelMatrix(GLKMatrix4 m)
{
	glUniformMatrix4fv(modelUniform, 1, 0, m.m);
}

} // namespace LWF
