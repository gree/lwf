/* compile.h
 * 
 * $Id$
 * 
 * Notice: This header file contains declarations of functions and types that
 * are just used internally. All library functions and types that are supposed
 * to be publicly accessable are defined in ./src/ming.h.
 */

#ifndef SWF_COMPILE_H_INCLUDED
#define SWF_COMPILE_H_INCLUDED

#include "ming.h"

extern int swfVersion;

typedef struct _buffer *Buffer;

/* shut up bison.simple */
void yyerror(char *msg);
int yylex();

#ifndef max
  #define max(x,y)	(((x)>(y))?(x):(y))
#endif

enum
{
  PUSH_STRING = 0,
  PUSH_FLOAT = 1,
  PUSH_NULL = 2,
  PUSH_UNDEF = 3,
  PUSH_REGISTER = 4,
  PUSH_BOOLEAN = 5,
  PUSH_DOUBLE = 6,
  PUSH_INT = 7,
  PUSH_CONSTANT = 8,
  PUSH_CONSTANT16 = 9
};

typedef enum
{
  FUNCTION_RANDOM,
  FUNCTION_LENGTH,
  FUNCTION_TIME,
  FUNCTION_INT,
  FUNCTION_CONCAT,
  FUNCTION_DUPLICATECLIP
} SWFActionFunction;

typedef enum
{
  GETURL_METHOD_NOSEND = 0,
  GETURL_METHOD_GET    = 1,
  GETURL_METHOD_POST   = 2
} SWFGetUrl2Method;

typedef enum
{
	/** Bind one register to "this" */
	PRELOAD_THIS = 1,

	/** No "this" variable accessible by-name */
	SUPPRESS_THIS = 2,

	/** Bind one register to "arguments" */
	PRELOAD_ARGUMENTS = 4,

	/** No "argument" variable accessible by-name */
	SUPPRESS_ARGUMENTS = 8,

	/** Bind one register to "super" */
	PRELOAD_SUPER = 16,

	/** No "super" variable accessible by-name */
	SUPPRESS_SUPER = 32,

	/** Bind one register to "_root" */
	PRELOAD_ROOT = 64,

	/** Bind one register to "_parent" */
	PRELOAD_PARENT = 128,

	/** Bind one register to "_global" */
	PRELOAD_GLOBAL = 256

} SWFDefineFunction2Flags;

#define GETURL_LOADMOVIE 0x40
#define GETURL_LOADVARIABLES 0x80

#define MAGIC_CONTINUE_NUMBER 0x7FFE
#define MAGIC_BREAK_NUMBER    0x7FFF

#define MAGIC_CONTINUE_NUMBER_LO 0xFE
#define MAGIC_CONTINUE_NUMBER_HI 0x7F
#define MAGIC_BREAK_NUMBER_LO    0xFF
#define MAGIC_BREAK_NUMBER_HI    0x7F

#define BUFFER_INCREMENT 128

struct _buffer
{
  byte *buffer;
  byte *pos;
  int buffersize;
  int free;
  byte *pushloc;
  int hasObject;  // simplify grammar (e.g. DELETE rule);
};

#define BUFFER_SIZE sizeof(struct _buffer)

struct exprlist_s
{
	Buffer buffer;
	int count;
};

struct function_s
{
	char *name;
	struct exprlist_s params;
	Buffer code;
	int flags;
};
typedef struct function_s *ASFunction;

struct variable_s	
{	
	char *name;
	Buffer initCode;
};
typedef struct variable_s *ASVariable;

typedef enum
{
	UNDEF,
	METHOD,	
	VARIABLE,
	BUFF
} ClassMemberType;

struct class_member_s
{
	ClassMemberType type;
	union
	{
		ASFunction function;
		ASVariable var;
		Buffer buffer;
	} element;
	struct class_member_s *next;
};
typedef struct class_member_s *ASClassMember;

struct class_s
{
	char *name;
	char *extends;
	ASClassMember members;
};
typedef struct class_s *ASClass;

struct switchcase
{	Buffer cond, action;
	int condlen, actlen, isbreak;
};

struct switchcases
{
	struct switchcase *list;
	int count;
};

enum ctx
{
	CTX_FUNCTION = 1,
	CTX_LOOP,
	CTX_FOR_IN,
	CTX_SWITCH,

	CTX_BREAK,
	CTX_CONTINUE
};

void addctx(enum ctx val);
void delctx(enum ctx val);
int chkctx(enum ctx val);

void checkByteOrder();

/* create/destroy buffer object */
Buffer newBuffer();
void destroyBuffer(Buffer out);
int bufferConcat(Buffer a, Buffer b);        /* destroys b. */
int bufferConcatSimple(Buffer a, Buffer b);
int bufferWriteBuffer(Buffer a, Buffer b);   /* doesn't. */

/* utilities for writing */
void bufferGrow(Buffer out);
void bufferCheckSize(Buffer out, int bytes);

int bufferLength(Buffer out);

/* constant pool stuff */
int addConstant(const char *s);
int bufferWriteConstants(Buffer out);
#define MAXCONSTANTPOOLSIZE 65533

/* write data to buffer */
int bufferWriteOp(Buffer out, int data);
int bufferWritePushOp(Buffer out);
int bufferWriteU8(Buffer out, int data);
int bufferWriteS16(Buffer out, int data);
int bufferWriteData(Buffer out, const byte *buffer, int bytes);
int bufferWriteHardString(Buffer out, const char *string, int length);
int bufferWriteConstantString(Buffer out, const char *string, int length);
int bufferWriteString(Buffer out, const char *string, int length);
int bufferWritePushString(Buffer out, char *string, int length);
int bufferWriteInt(Buffer out, int i);
int bufferWriteFloat(Buffer out, float f);
int bufferWriteDouble(Buffer out, double d);
int bufferWriteNull(Buffer out);
int bufferWriteUndef(Buffer out);
int bufferWriteBoolean(Buffer out, int val);
int bufferWriteRegister(Buffer out, int num);
int bufferWriteSetRegister(Buffer out, int num);
int bufferWriteProperty(Buffer out, char *string);
int bufferWriteWTHITProperty(Buffer out);
int lookupProperty(char *string);

/* concat b to a, destroy b */
char *stringConcat(char *a, char *b);

/* resolve magic number standins to relative offsets */
#define bufferResolveJumps(buf) bufferResolveJumpsFull(buf, \
    buf->pos, buf->buffer)
void bufferResolveJumpsFull(Buffer out, byte *break_ptr, byte *continue_ptr);
void bufferResolveSwitch(Buffer buffer, struct switchcases *slp);

void bufferPatchPushLength(Buffer buffer, int len);

int bufferWriteFunction(Buffer out, ASFunction function, int version);
int bufferWriteClass(Buffer out, ASClass clazz);

ASFunction newASFunction();
ASVariable newASVariable(char *, Buffer);
ASClass newASClass(char *name, char *extends, ASClassMember members);

ASClassMember newASClassMember_function(ASFunction func);
ASClassMember newASClassMember_function(ASFunction func);
ASClassMember newASClassMember_buffer(Buffer buf);
ASClassMember newASClassMember_variable(ASVariable var);
void ASClassMember_append(ASClassMember m0, ASClassMember end);

/* rather than setting globals... */
void swf4ParseInit(const char *string, int debug, int version);
void swf5ParseInit(const char *string, int debug, int version);

int swf4parse(void *b);
int swf5parse(void *b);

#endif /* SWF_COMPILE_H_INCLUDED */
