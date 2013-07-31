/*
    Ming, an SWF output library
    Copyright (C) 2002  Opaque Industries - http://www.opaque.net/

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

#ifndef WIN32
	#include <unistd.h>
#endif

#include <stdio.h>
#include <stdlib.h>
#include <ctype.h>
#include <string.h>

#include "compile.h"
#include "actiontypes.h"

/* Define this to have some debugging output when outputting DEFINEFUNCTION2 */
#undef MING_DEBUG_FUNCTION2

// XXX: set by swf[4|5]Init()
int swfVersion = 0;

static int nConstants = {0}, maxConstants = {0}, sizeConstants = {0};
static char **constants = NULL;

char *stringConcat(char *a, char *b)
{
	if ( a != NULL )
	{
		if ( b != NULL )
		{
			a = (char*)realloc(a, strlen(a)+strlen(b)+1);
			if(a == NULL)
				return NULL;
			strcat(a, b);
			free(b);
		}

		return a;
	}
	else
		return b;
}

void bufferPatchLength(Buffer buffer, int back)
{
	unsigned char *output = buffer->buffer;
	int len = bufferLength(buffer);

	output[len-back-1] = (back>>8) & 0xff;
	output[len-back-2] = back & 0xff;
}


/* add len more bytes to length of the pushdata opcode pointed to by
	 buffer->pushloc */

void bufferPatchPushLength(Buffer buffer, int len)
{
	int oldsize;

	if(buffer->pushloc != NULL)
	{
		oldsize = (buffer->pushloc[0] & 0xff) | ((buffer->pushloc[1] & 0xff) << 8);
		oldsize += len;
		buffer->pushloc[0] = oldsize & 0xff;
		buffer->pushloc[1] = (oldsize >> 8) & 0xff;
	}
	else
		SWF_error("problem with bufferPatchPushLength\n");
}


static int useConstants = 1;
void Ming_useConstants(int flag)
{	useConstants = flag;
}


int addConstant(const char *s)
{
	int i;

	for(i=0; i<nConstants; ++i)
	{
		if(strcmp(s, constants[i]) == 0)
			return i;
	}

	/* Don't let constant pool biggern then allowed */
	if ( sizeConstants+strlen(s)+1 > MAXCONSTANTPOOLSIZE ) return -1;

	if(nConstants == maxConstants)
		constants = (char **) realloc(constants, (maxConstants += 64) * sizeof(char *));
	constants[nConstants] = strdup(s);
	sizeConstants += (strlen(s)+1);
	return nConstants++;
}

int bufferWriteConstants(Buffer out)
{
	int i, len=2;

	if(nConstants == 0)
		return 0;

	bufferWriteU8(out, SWFACTION_CONSTANTPOOL);
	bufferWriteS16(out, 0); /* length */
	bufferWriteS16(out, nConstants);

	for(i=0; i<nConstants; ++i)
	{
		len += bufferWriteHardString(out, constants[i], strlen(constants[i])+1);
		free(constants[i]);
	}

	nConstants = 0;
	sizeConstants = 0;
	bufferPatchLength(out, len);

	return len+3;
}

Buffer newBuffer()
{
	Buffer out = (Buffer)malloc(BUFFER_SIZE);
	if(out == NULL)
		return NULL;
	memset(out, 0, BUFFER_SIZE);

	out->buffer = (byte*)malloc(BUFFER_INCREMENT);
	out->pos = out->buffer;
	*(out->pos) = 0;
	out->buffersize = out->free = BUFFER_INCREMENT;
	out->pushloc = NULL;
	out->hasObject = 0;

	return out;
}

void destroyBuffer(Buffer out)
{
	free(out->buffer);
	free(out);
}

int bufferLength(Buffer out)
{
	if(out)
		return (out->pos)-(out->buffer);
	else
		return 0;
}

/* make sure there's enough space for bytes bytes */
void bufferCheckSize(Buffer out, int bytes)
{
	if(bytes > out->free)
	{
		int New = BUFFER_INCREMENT * ((bytes-out->free-1)/BUFFER_INCREMENT + 1);

		int num = bufferLength(out); /* in case buffer gets displaced.. */
		unsigned char *newbuf = (unsigned char*)realloc(out->buffer, out->buffersize+New);

		if(newbuf != out->buffer)
		{
			int pushd = 0;

			if(out->pushloc)
	pushd = out->pos - out->pushloc;

			out->pos = newbuf+num;

			if(out->pushloc)
	out->pushloc = out->pos - pushd;
		}

		out->buffer = newbuf;
		out->buffersize += New;
		out->free += New;
	}
}

int bufferWriteData(Buffer b, const byte *data, int length)
{
	int i;

	bufferCheckSize(b, length);

	for(i=0; i<length; ++i)
		bufferWriteU8(b, data[i]);

	return length;
}

int bufferWriteBuffer(Buffer a, Buffer b)
{
	if(!a)
		return 0;

	if(b)
		return bufferWriteData(a, b->buffer, bufferLength(b));

	return 0;
}

/* if a's last op and b's first op are both PUSH, concat into one op */

int bufferWriteDataAndPush(Buffer a, Buffer b)
{
	int i, pushd = 0;

	byte *data = b->buffer;
	int length = b->pos - b->buffer;

	if(a->pushloc && (b->buffer[0] == SWFACTION_PUSH) && swfVersion > 4)
	{
		pushd = (b->buffer[1] & 0xff) | ((b->buffer[2] & 0xff) << 8);
		bufferPatchPushLength(a, pushd);
		data += 3;
		length -= 3;
	}

	if(b->pushloc)
		pushd = b->pos - b->pushloc;

	bufferCheckSize(a, length);

	for(i=0; i<length; ++i)
		bufferWriteU8(a, data[i]);

	if(a->pushloc &&
		 (b->buffer[0] == SWFACTION_PUSH) && (b->pushloc == b->buffer+1))
		; /* b is just one pushdata, so do nothing.. */
	else if(b->pushloc)
		a->pushloc = a->pos - pushd;
	else
		a->pushloc = 0;

	return length;
}

int bufferConcatSimple(Buffer a, Buffer b)
{
	int len = 0;

	if(!a)
		return 0;

	if(b)
	{	len = bufferWriteBuffer(a, b);
		destroyBuffer(b);
	}

	return len;
}

int bufferConcat(Buffer a, Buffer b)
{
	int len = 0;

	if(!a)
		return 0;

	if(b)
	{	len = bufferWriteDataAndPush(a, b);
		destroyBuffer(b);
	}

	return len;
}

int bufferWriteOp(Buffer out, int data)
{
	bufferWriteU8(out, data);
	out->pushloc = NULL;

	return 1;
}

int bufferWritePushOp(Buffer out)
{
	bufferWriteU8(out, SWFACTION_PUSH);
	out->pushloc = out->pos;

	return 1;
}

int bufferWriteU8(Buffer out, int data)
{
	bufferCheckSize(out, 1);
	*(out->pos) = data;
	out->pos++;
	out->free--;

	return 1;
}

int bufferWriteS16(Buffer out, int data)
{
	if(data < 0)
		data = (1<<16)+data;

	bufferWriteU8(out, data%256);
	data >>= 8;
	bufferWriteU8(out, data%256);

	return 2;
}

int bufferWriteHardString(Buffer out, const char *string, int length)
{
	int i;

	for(i=0; i<length; ++i)
		bufferWriteU8(out, (byte)string[i]);

	return length;
}

int bufferWriteConstantString(Buffer out, const char *string, int length)
{
	int n;

	if(swfVersion < 5)
		return -1;

	if(useConstants)
		n = addConstant((char*) string);
	else
		n = -1;

	if(n == -1)
	{
		bufferWriteU8(out, PUSH_STRING);
		return bufferWriteHardString(out, string, length) + 1;
	}
	else if(n < 256)
	{
		bufferWriteU8(out, PUSH_CONSTANT);
		return bufferWriteU8(out, n) + 1;
	}
	else
	{
		bufferWriteU8(out, PUSH_CONSTANT16);
		return bufferWriteS16(out, n) + 1;
	}
}

/* allow pushing STRINGs for SWF>=5 */
int bufferWritePushString(Buffer out, char *string, int length)
{
	int l, len = 0;
	if(out->pushloc == NULL || swfVersion < 5)
	{
		len = 3;
		bufferWritePushOp(out);
		bufferWriteS16(out, length+1);
	}
	
	bufferWriteU8(out, PUSH_STRING);
	l = bufferWriteHardString(out, string, length);
	bufferPatchPushLength(out, l + 1);	
	return len + l + 1;
}

int bufferWriteString(Buffer out, const char *string, int length)
{
	if(swfVersion < 5)
	{
		bufferWritePushOp(out);
		bufferWriteS16(out, length+1);
		bufferWriteU8(out, PUSH_STRING);
		bufferWriteHardString(out, string, length);

		return 4 + length;
	}
	else
	{
		int l;

		if(out->pushloc == NULL)
		{
			bufferWritePushOp(out);
			bufferWriteS16(out, 0);
		}

		l = bufferWriteConstantString(out, string, length);

		bufferPatchPushLength(out, l);
		return l;
	}
}

int bufferWriteInt(Buffer out, int i)
{
	int len = 0;
	unsigned char *p = (unsigned char *)&i;

	if(out->pushloc == NULL || swfVersion < 5)
	{
		len = 3;
		bufferWritePushOp(out);
		bufferWriteS16(out, 5);
	}
	else
		bufferPatchPushLength(out, 5);

	bufferWriteU8(out, PUSH_INT);

#if SWF_LITTLE_ENDIAN
	bufferWriteU8(out, p[0]);
	bufferWriteU8(out, p[1]);
	bufferWriteU8(out, p[2]);
	bufferWriteU8(out, p[3]);
#else 
	bufferWriteU8(out, p[3]);
	bufferWriteU8(out, p[2]);
	bufferWriteU8(out, p[1]);
	bufferWriteU8(out, p[0]);
#endif

	return len + 5;
}

int bufferWriteFloat(Buffer out, float f)
{
	int len = 0;
	unsigned char *p = (unsigned char *)&f;

	if(out->pushloc == NULL || swfVersion < 5)
	{
		len = 3;
		bufferWritePushOp(out);
		bufferWriteS16(out, 5);
	}
	else
		bufferPatchPushLength(out, 5);

	bufferWriteU8(out, PUSH_FLOAT);

#if SWF_LITTLE_ENDIAN
	bufferWriteU8(out, p[0]);
	bufferWriteU8(out, p[1]);
	bufferWriteU8(out, p[2]);
	bufferWriteU8(out, p[3]);	
#else
	bufferWriteU8(out, p[3]);
	bufferWriteU8(out, p[2]);
	bufferWriteU8(out, p[1]);
	bufferWriteU8(out, p[0]);
#endif
	return len + 5;
}

int bufferWriteDouble(Buffer out, double d)
{
	int len = 0;
	unsigned char *p = (unsigned char *)&d;

	if(out->pushloc == NULL || swfVersion < 5)
	{
		len = 3;
		bufferWritePushOp(out);
		bufferWriteS16(out, 9);
	}
	else
		bufferPatchPushLength(out, 5);

	bufferWriteU8(out, PUSH_DOUBLE);

#if SWF_LITTLE_ENDIAN
	bufferWriteU8(out, p[4]);
	bufferWriteU8(out, p[5]);
	bufferWriteU8(out, p[6]);
	bufferWriteU8(out, p[7]);
	bufferWriteU8(out, p[0]);
	bufferWriteU8(out, p[1]);
	bufferWriteU8(out, p[2]);
	bufferWriteU8(out, p[3]);
#else
	bufferWriteU8(out, p[3]);
	bufferWriteU8(out, p[2]);
	bufferWriteU8(out, p[1]);
	bufferWriteU8(out, p[0]);
	bufferWriteU8(out, p[7]);
	bufferWriteU8(out, p[6]);
	bufferWriteU8(out, p[5]);
	bufferWriteU8(out, p[4]);
#endif

	return len + 9;
}

int bufferWriteNull(Buffer out)
{
	int len = 0;

	if(out->pushloc == NULL || swfVersion < 5)
	{
		len = 3;
		bufferWritePushOp(out);
		bufferWriteS16(out, 1);
	}
	else
		bufferPatchPushLength(out, 1);

	bufferWriteU8(out, PUSH_NULL);

	return len + 1;
}

int bufferWriteUndef(Buffer out)
{
	int len = 0;

	if(out->pushloc == NULL || swfVersion < 5)
	{
		len = 3;
		bufferWritePushOp(out);
		bufferWriteS16(out, 1);
	}
	else
		bufferPatchPushLength(out, 1);

	bufferWriteU8(out, PUSH_UNDEF);

	return len + 1;
}

int bufferWriteBoolean(Buffer out, int val)
{
	int len = 0;

	if(out->pushloc == NULL || swfVersion < 5)
	{
		len = 3;
		bufferWritePushOp(out);
		bufferWriteS16(out, 2);
	}
	else
		bufferPatchPushLength(out, 2);

	bufferWriteU8(out, PUSH_BOOLEAN);
	bufferWriteU8(out, val ? 1 : 0);

	return len + 2;
}

int bufferWriteRegister(Buffer out, int num)
{
	int len = 0;

	if(out->pushloc == NULL || swfVersion < 5)
	{
		len = 3;
		bufferWritePushOp(out);
		bufferWriteS16(out, 2);
	}
	else
		bufferPatchPushLength(out, 2);

	bufferWriteU8(out, PUSH_REGISTER);
	bufferWriteU8(out, num);

	return len + 2;
}

int bufferWriteSetRegister(Buffer out, int num)
{
	bufferWriteU8(out, SWFACTION_STOREREGISTER);
	bufferWriteS16(out, 1);
	bufferWriteU8(out, num);
	return 4;
}

void lower(char *s)
{
	while(*s)
	{
		*s = tolower(*s);
		++s;
	}
}

/* this code will eventually help to pop extra values off the
 stack and make sure that continue and break address the proper
 context
 */
static enum ctx *ctx_stack = {0};
static int ctx_count = {0}, ctx_len = {0};
void addctx(enum ctx val)
{	
	if(ctx_count >= ctx_len)
		ctx_stack = (enum ctx*) realloc(ctx_stack, (ctx_len += 10) * sizeof(enum ctx));
	ctx_stack[ctx_count++] = val;
}
void delctx(enum ctx val)
{	
	if(ctx_count <= 0)  
		SWF_error("consistency check in delctx: stack empty!\n");
	else if (ctx_stack[--ctx_count] != val)
		SWF_error("consistency check in delctx: val %i != %i\n", ctx_stack[ctx_count], val);

}

int chkctx(enum ctx val)
{	int n, ret = 0;
	switch(val)
	{	case CTX_FUNCTION:
			for(n = ctx_count ; --n >= 0 ; )
				switch(ctx_stack[n])
				{	case CTX_SWITCH:
					case CTX_FOR_IN:
						ret++;
						break;
					case CTX_FUNCTION:
						return ret;
					default: ; /* computers are stupid */
				}
			return -1;
		case CTX_BREAK:
			for(n = ctx_count ; --n >= 0 ; )
				switch(ctx_stack[n])
				{	case CTX_SWITCH:
						return CTX_SWITCH;
					case CTX_LOOP:
						return CTX_LOOP;
					case CTX_FOR_IN:
						return CTX_FOR_IN;
					case CTX_FUNCTION:
						return -1;
					case CTX_BREAK:
						return CTX_BREAK;
					default: ; /* computers are stupid */
				}
			return -1;
		case CTX_CONTINUE:
			for(n = ctx_count ; --n >= 0 ; )
				switch(ctx_stack[n])
				{	case CTX_LOOP:
					case CTX_FOR_IN:
						return 0;
					case CTX_FUNCTION:
						return -1;
					default: ; /* computers are stupid */
				}
		default: return -1;; /* computers are stupid */
	}
}

/* replace MAGIC_CONTINUE_NUMBER and MAGIC_BREAK_NUMBER with jumps to
	 head or tail, respectively */
/* jump offset is relative to end of jump instruction */
/* I can't believe this actually worked */

void bufferResolveJumpsFull(Buffer out, byte *break_ptr, byte *continue_ptr)
{
	byte *p = out->buffer;
	int l, target;

	while(p < out->pos)
	{
		if(*p & 0x80) /* then it's a multibyte instruction */
		{
			if(*p == SWFACTION_JUMP)
			{
	p += 3; /* plus instruction plus two-byte length */

	if(*p == MAGIC_CONTINUE_NUMBER_LO &&
		 *(p+1) == MAGIC_CONTINUE_NUMBER_HI)
	{
		target = continue_ptr - (p+2);
		*p = target & 0xff;
		*(p+1) = (target>>8) & 0xff;
	}
	else if(*p == MAGIC_BREAK_NUMBER_LO &&
		*(p+1) == MAGIC_BREAK_NUMBER_HI)
	{
		target = break_ptr - (p+2);
		*p = target & 0xff;
		*(p+1) = (target>>8) & 0xff;
	}

	p += 2;
			}
			else
			{
	++p;
	l = *p;
	++p;
	l += *p<<8;
	++p;

	p += l;
			}
		}
		else
			++p;
	}
}

// handle SWITCH statement

void bufferResolveSwitch(Buffer buffer, struct switchcases *slp)
{	struct switchcase *scp;
	int n, len;
	unsigned char *output;
			
	len = bufferLength(buffer);
	for(n = 0, scp = slp->list ; n < slp->count ; n++, scp++)
	{	scp->actlen = bufferLength(scp->action);
		if((n < slp->count-1))
			scp->actlen += 5;
		if(scp->cond)
		{	scp->condlen = bufferLength(scp->cond) + 8;
			bufferWriteOp(buffer, SWFACTION_PUSHDUP);
			bufferConcat(buffer, scp->cond);
			bufferWriteOp(buffer, SWFACTION_EQUALS2);
			bufferWriteOp(buffer, SWFACTION_LOGICALNOT);
			bufferWriteOp(buffer, SWFACTION_IF);
			bufferWriteS16(buffer, 2);
			bufferWriteS16(buffer, scp->actlen);
		}
		else
			scp->condlen = 0;
		bufferConcat(buffer, scp->action);
		bufferWriteOp(buffer, SWFACTION_JUMP);
		bufferWriteS16(buffer, 2);
		bufferWriteS16(buffer, scp->isbreak ? MAGIC_BREAK_NUMBER : 0);
		if(!scp->cond)
		{	slp->count = n+1;
			break;
		}
	}
	for(n = 0, scp = slp->list ; n < slp->count ; n++, scp++)
	{	len += scp->condlen;
		output = buffer->buffer + len;
		if((n < slp->count-1) && !scp->isbreak)
		{	output[scp->actlen-2] = (scp+1)->condlen & 0xff;
			output[scp->actlen-1] = (scp+1)->condlen >> 8;
		}
		len += scp->actlen;
	}
}
	
int lookupProperty(char *string)
{
	lower(string);

	if(strcmp(string, "_x") == 0)		return PROPERTY_X;
	if(strcmp(string, "_y") == 0)		return PROPERTY_Y;
	if(strcmp(string, "_xscale") == 0)	return PROPERTY_XSCALE;
	if(strcmp(string, "_yscale") == 0)	return PROPERTY_YSCALE;
	if(strcmp(string, "_currentframe") == 0) return PROPERTY_CURRENTFRAME;
	if(strcmp(string, "_totalframes") == 0)	return PROPERTY_TOTALFRAMES;
	if(strcmp(string, "_alpha") == 0)	return PROPERTY_ALPHA;
	if(strcmp(string, "_visible") == 0)	return PROPERTY_VISIBLE;
	if(strcmp(string, "_width") == 0)	return PROPERTY_WIDTH;
	if(strcmp(string, "_height") == 0)	return PROPERTY_HEIGHT;
	if(strcmp(string, "_rotation") == 0)	return PROPERTY_ROTATION;
	if(strcmp(string, "_target") == 0)	return PROPERTY_TARGET;
	if(strcmp(string, "_framesloaded") == 0)	return PROPERTY_FRAMESLOADED;
	if(strcmp(string, "_name") == 0)		return PROPERTY_NAME;
	if(strcmp(string, "_droptarget") == 0)	return PROPERTY_DROPTARGET;
	if(strcmp(string, "_url") == 0)		return PROPERTY_URL;
	if(strcmp(string, "_highquality") == 0)	return PROPERTY_HIGHQUALITY;
	if(strcmp(string, "_focusrect") == 0)	return PROPERTY_FOCUSRECT;
	if(strcmp(string, "_soundbuftime") == 0)	return PROPERTY_SOUNDBUFTIME;
	if(strcmp(string, "_quality")==0)	return PROPERTY_QUALITY;
	if(strcmp(string, "_xmouse") == 0)	return PROPERTY_XMOUSE;
	if(strcmp(string, "_ymouse") == 0)	return PROPERTY_YMOUSE;

	SWF_error("No such property: %s\n", string);
	return -1;
}

int bufferWriteProperty(Buffer out, char *string)
{
	int property = lookupProperty(string);
	return bufferWriteFloat(out, property);
}

// XXX: ???
int bufferWriteWTHITProperty(Buffer out)
{
	bufferWriteU8(out, SWFACTION_PUSH);
	bufferWriteS16(out, 5);
	bufferWriteU8(out, PUSH_FLOAT);
	bufferWriteS16(out, 0);
	bufferWriteS16(out, 0x4680);

	return 8;
}

/**
 * @param func_name
 * 	Function name, NULL for anonymous functions.
 *
 * @param num_regs
 * 	Number of registers.
 *
 * @param flags
 * 	See SWFDefineFunction2Flags enum.
 */
static int bufferWriteDefineFunction2(Buffer out, char *func_name, 
		Buffer args, Buffer code, int flags, int num_regs)
{
	Buffer c;
	char buf[1024];
	int num_args = 0, i;
	char *p = (char *) args->buffer;
	size_t taglen;	
	strcpy(buf, "");
		
	// REGISTERPARAM records
	c = newBuffer();
	// TODO: rewrite this function, all these calls to strncat
	//       seem overkill to me
	for(i = 0; i < bufferLength(args); i++)
	{
		if(p[i] == '\0')
		{
			bufferWriteU8(c, 0);
			bufferWriteHardString(c, buf, strlen(buf)+1);	
			strcpy(buf, "");
			num_args++;
		}
		else
		{
			strncat(buf, &p[i], 1);
		}
	}

	bufferWriteOp(out, SWFACTION_DEFINEFUNCTION2);

	if(func_name == NULL)
	{
		taglen =
			+ 1			/* function name (empty) */
			+ 2			/* arg count (short) */
			+ 1			/* reg count (byte) */
			+ 2			/* flags */
			+ bufferLength(c)	/* swf_params */
			+ 2 			/* body size */
			;

#ifdef MING_DEBUG_FUNCTION2
		printf("adding anonymouse SWF_DEFINEFUNCTION2 nargs=%d flags=%d"
				" arglen=%d codelen=%d taglen=%d\n",
				num_args, flags, bufferLength(args),
				bufferLength(code), taglen);
#endif
		bufferWriteS16(out, taglen);

		bufferWriteU8(out, 0); /* empty function name */
	}
	else
	{
		taglen = 0
			+ strlen(func_name)+1	/* function name */
			+ 2			/* arg count (short) */
			+ 1			/* reg count (byte) */
			+ 2			/* flags */
			+ bufferLength(c)	/* swf_params */
			+ 2 			/* body size */
			;

#ifdef MING_DEBUG_FUNCTION2
		printf("adding named SWF_DEFINEFUNCTION2 name=%s nargs=%d flags=%d"
				" regparamlen=%d arglen=%d codelen=%d taglen=%d\n",
				func_name, num_args, flags,
				bufferLength(c),
				bufferLength(args),
				bufferLength(code), taglen);
#endif
		bufferWriteS16(out, taglen);
		bufferWriteHardString(out, func_name, strlen(func_name)+1);	 
	}
	bufferWriteS16(out, num_args); /* number of params */
 	bufferWriteU8(out, num_regs); /* register count */
 	bufferWriteS16(out, flags);    /* flags */
 	//bufferWriteS16(out, 0);    /* flags */
 	bufferConcat(out, c);
	bufferWriteS16(out, bufferLength(code)); /* code size */
	bufferConcat(out, code);
	return taglen;
}

void destroyASFunction(ASFunction func)
{
	free(func->name);
	free(func);
}

int bufferWriteFunction(Buffer out, ASFunction function, int version)
{
	int tagLen; 
	
	if(version == 2)
	{
		tagLen = bufferWriteDefineFunction2(out, function->name, 
			function->params.buffer, function->code, function->flags, 0);
	}
	else
	{
		tagLen = 5; 
		tagLen += bufferLength(function->params.buffer);
		if(function->name != NULL)
			tagLen += strlen(function->name); 
	
		bufferWriteOp(out, SWFACTION_DEFINEFUNCTION);
		bufferWriteS16(out, tagLen);
		if(function->name == NULL) 
			bufferWriteU8(out, 0); /* empty function name */
		else
			bufferWriteHardString(out, function->name, strlen(function->name) +1 );
		bufferWriteS16(out, function->params.count);
		bufferConcat(out, function->params.buffer);
		bufferWriteS16(out, bufferLength(function->code));
		bufferConcat(out, function->code);
	}
	destroyASFunction(function);
	return tagLen;
}

ASFunction newASFunction()
{
	ASFunction func;
	func = (ASFunction) malloc(sizeof(struct function_s));
	func->flags = 0;
	func->code = NULL;
	func->params.count = 0;
	func->params.buffer = NULL;
	func->name = NULL;
	return func;
}

void destroyASClass(ASClass clazz)
{
	ASClassMember member;
	if(clazz->name)
		free(clazz->name);
	if(clazz->extends)
		free(clazz->extends);
	
	member = clazz->members;
	while(member)
	{	
		ASClassMember _this = member;
		member = member->next;
		free(_this);
	}
	free(clazz);
}

ASFunction ASClass_getConstructor(ASClass clazz)
{
	ASClassMember member;
	member = clazz->members;
	while(member)
	{
		ASFunction func;
		ASClassMember _this = member;
		member = member->next;

		if(_this->type != METHOD)
			continue;
		func = _this->element.function;
		if(!func || !func->name)
			continue;
		if(strcmp(func->name, clazz->name) != 0)
			continue;
		
		_this->element.function = NULL;
		return func;
	}
	return newASFunction(); // default empty constructor
}

static int bufferWriteClassConstructor(Buffer out, ASClass clazz)
{
	int len = 0;
	ASFunction func;

	/* class constructor */
	len += bufferWriteString(out, "_global", strlen("_global") + 1);
	len += bufferWriteOp(out, SWFACTION_GETVARIABLE);
	len += bufferWriteString(out, clazz->name, strlen(clazz->name) + 1);
	func = ASClass_getConstructor(clazz);
	if(func->name)
	{
		free(func->name);
		func->name = NULL;
	}
	len += bufferWriteFunction(out, func, 1);
	len += bufferWriteSetRegister(out, 1);
	len += bufferWriteOp(out, SWFACTION_SETMEMBER);

	if(clazz->extends)
	{
		len += bufferWriteRegister(out, 1);
		len += bufferWriteString(out, clazz->extends,
		                         strlen(clazz->extends) + 1);
		len += bufferWriteOp(out, SWFACTION_GETVARIABLE);
		len += bufferWriteOp(out, SWFACTION_EXTENDS);
	}
	
	len += bufferWriteRegister(out, 1);
	len += bufferWriteString(out, "prototype", strlen("prototype") + 1);
	len += bufferWriteOp(out, SWFACTION_GETMEMBER);
	len += bufferWriteSetRegister(out, 2);
	
	len += bufferWriteOp(out, SWFACTION_POP);

	return len;
}

static int bufferWriteClassMethods(Buffer out, ASClass clazz)
{
	ASClassMember member = clazz->members;
	int len = 0;
	while(member)
	{
		ASFunction func;
		ASClassMember _this = member;
		member = member->next;
		if(_this->type != METHOD)
			continue;
		func = _this->element.function;
		if(!func || !func->name)
			continue;
	
		if(strcmp(func->name, clazz->name) == 0)
		{
			SWF_error("only one class constructor allowed\n");
		}
	
		len += bufferWriteRegister(out, 2);
		len += bufferWriteString(out, func->name, strlen(func->name) + 1);
		free(func->name);
		func->name = NULL;
		len += bufferWriteFunction(out, func, 1);
		len += bufferWriteOp(out, SWFACTION_SETMEMBER);
		_this->element.function = NULL;
	}
	return len;
}

static int bufferWriteClassVariable(Buffer out, ASVariable var)
{
	int len = 0;
	if(var->initCode != NULL)
	{
		len += bufferWriteRegister(out, 2);
		len += bufferWriteString(out, var->name, strlen(var->name)+1);
		len += bufferConcat(out, var->initCode);
		len += bufferWriteOp(out, SWFACTION_SETMEMBER); 
	}
	free(var->name);
	free(var); // aka destroyASVariable
	return len;
}

static int bufferWriteClassMembers(Buffer out, ASClass clazz)
{
	ASClassMember member = clazz->members;
	int len = 0;
	while(member)
	{
		ASVariable var;
		ASClassMember _this = member;
		member = member->next;
		if(_this->type != VARIABLE)
			continue;
		var = _this->element.var;
		if(!var)
			continue;
		bufferWriteClassVariable(out, var);	
		_this->element.var = NULL;
	}
	return len;
}


int bufferWriteClass(Buffer out, ASClass clazz)
{
	int len = 0;
	len += bufferWriteClassConstructor(out, clazz);
	len += bufferWriteClassMembers(out, clazz);	
	len += bufferWriteClassMethods(out, clazz);
	/* set class properties */
	len += bufferWriteInt(out, 1);
	len += bufferWriteNull(out);
	len += bufferWriteString(out, "_global", strlen("_global") + 1);
	len += bufferWriteOp(out, SWFACTION_GETVARIABLE);

	len += bufferWriteString(out, clazz->name, strlen(clazz->name) + 1);
	len += bufferWriteOp(out, SWFACTION_GETMEMBER);

	len += bufferWriteString(out, "prototype", strlen("prototype") + 1);
	len += bufferWriteOp(out, SWFACTION_GETMEMBER);
		
	len += bufferWriteInt(out, 3);
	len += bufferWriteString(out, "ASSetPropFlags", strlen("ASSetPropFlags") + 1);
	len += bufferWriteOp(out, SWFACTION_CALLFUNCTION);
	len += bufferWriteOp(out, SWFACTION_POP);

	destroyASClass(clazz);
	return len;
}

void ASClassMember_append(ASClassMember m0, ASClassMember end)
{
	ASClassMember mb = m0;
	while(mb->next)
		mb = mb->next;
	mb->next = end;
}

ASClass newASClass(char *name, char *extends, ASClassMember members)
{
	ASClass clazz;
	clazz = (ASClass) malloc(sizeof(struct class_s));
	clazz->name = name;
	clazz->extends = extends;
	clazz->members = members;
	return clazz;	
}

ASClassMember newASClassMember_function(ASFunction func)
{
	ASClassMember member = (ASClassMember) malloc(sizeof(struct class_member_s));
	member->element.function = func;
	member->type = METHOD;
	member->next = NULL; 
	return member;
}

ASClassMember newASClassMember_variable(ASVariable var)
{
	ASClassMember member = (ASClassMember) malloc(sizeof(struct class_member_s));
	member->element.var = var;
	member->type = VARIABLE;
	member->next = NULL; 
	return member;
}


ASClassMember newASClassMember_buffer(Buffer buf)
{
	ASClassMember member = (ASClassMember) malloc(sizeof(struct class_member_s));
	member->element.buffer = buf;
	member->type = BUFF;
	member->next = NULL; 
	return member;
}
ASVariable newASVariable(char *name, Buffer buf)
{
	ASVariable var = (ASVariable) malloc(sizeof(struct variable_s));
	var->name = name;
	var->initCode = buf;
	return var;
}
/*
 * Local variables:
 * tab-width: 2
 * c-basic-offset: 2
 * End:
 */
